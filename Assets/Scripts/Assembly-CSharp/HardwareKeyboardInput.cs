using UnityEngine;

public enum HardwareKeyAction
{
	None = 0,
	Submit = 1,
	Cancel = 2
}

// Shared hardware-keyboard text entry used by UITextInput, UIInputText and Keyboard.
//
// Hardware input is used on Editor / Standalone / WebGL. On native mobile builds
// every method here is a no-op, so the TouchScreenKeyboard code path is untouched.
public static class HardwareKeyboardInput
{
	// True on platforms where Input.inputString is used to read typed text.
	public static bool IsSupported
	{
		get
		{
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
			return true;
#else
			return false;
#endif
		}
	}

	// Whether a TouchScreenKeyboard should be opened at all.
	//   Editor / Standalone : never (hardware keyboard only)
	//   WebGL               : only if the browser/device supports it (mobile browsers)
	//   Native mobile       : always (original behaviour)
	public static bool ShouldOpenTouchKeyboard
	{
		get
		{
#if UNITY_EDITOR || UNITY_STANDALONE
			return false;
#elif UNITY_WEBGL
			return TouchScreenKeyboard.isSupported;
#else
			return true;
#endif
		}
	}

	// Reads this frame's hardware keyboard input.
	//
	// text        : current text. Updated in place when 'changed' is true.
	// maxLength   : max characters, or < 0 for no limit.
	// type        : filters accepted characters (NumberPad = digits only,
	//               NumbersAndPunctuation = digits, one '.', leading '-',
	//               ASCIICapable = printable ASCII, anything else = any non-control char).
	// ignore      : extra characters that are always rejected (may be null).
	// changed     : true if 'text' was modified (typing, backspace, paste).
	//
	// Returns Submit on Enter, Cancel on Escape. 'text' is updated before the action is
	// returned, so characters typed in the same frame as Enter are not lost.
	public static HardwareKeyAction Poll(ref string text, int maxLength, TouchScreenKeyboardType type, char[] ignore, out bool changed)
	{
		changed = false;
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
		string current = text ?? string.Empty;

		// Ctrl+V / Cmd+V paste. Newlines and tabs become spaces, everything else
		// goes through the same filters as typed characters.
		// Note: Unity WebGL cannot read the system clipboard, so paste does nothing there.
		if (IsPasteShortcutPressed())
		{
			string clip = GUIUtility.systemCopyBuffer;
			if (!string.IsNullOrEmpty(clip))
			{
				for (int i = 0; i < clip.Length; i++)
				{
					char c = clip[i];
					if (c == '\r' || c == '\n' || c == '\t')
					{
						c = ' ';
					}
					if (TryAppend(ref current, c, maxLength, type, ignore))
					{
						changed = true;
					}
				}
			}
		}

		string typed = Input.inputString;
		for (int j = 0; j < typed.Length; j++)
		{
			char c2 = typed[j];
			if (c2 == '\b')
			{
				if (current.Length > 0)
				{
					int remove = 1;
					if (current.Length > 1 && char.IsLowSurrogate(current[current.Length - 1]) && char.IsHighSurrogate(current[current.Length - 2]))
					{
						remove = 2;
					}
					current = current.Substring(0, current.Length - remove);
					changed = true;
				}
			}
			else if (c2 == '\u007f')
			{
				// Ctrl+Backspace on Windows: delete the previous word
				int end = current.Length;
				while (end > 0 && current[end - 1] == ' ')
				{
					end--;
				}
				while (end > 0 && current[end - 1] != ' ')
				{
					end--;
				}
				if (end != current.Length)
				{
					current = current.Substring(0, end);
					changed = true;
				}
			}
			else if (c2 == '\n' || c2 == '\r')
			{
				continue;
			}
			else if (TryAppend(ref current, c2, maxLength, type, ignore))
			{
				changed = true;
			}
		}

		if (changed)
		{
			text = current;
		}

		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			return HardwareKeyAction.Submit;
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			return HardwareKeyAction.Cancel;
		}
#endif
		return HardwareKeyAction.None;
	}

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
	private static bool IsPasteShortcutPressed()
	{
		bool modifier = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);
		return modifier && Input.GetKeyDown(KeyCode.V);
	}

	private static bool TryAppend(ref string current, char c, int maxLength, TouchScreenKeyboardType type, char[] ignore)
	{
		if (ignore != null && System.Array.IndexOf(ignore, c) >= 0)
		{
			return false;
		}
		if (!IsCharAllowed(c, current, type))
		{
			return false;
		}
		if (maxLength > -1 && current.Length >= maxLength)
		{
			return false;
		}
		current += c;
		return true;
	}

	private static bool IsCharAllowed(char c, string current, TouchScreenKeyboardType type)
	{
		switch (type)
		{
		case TouchScreenKeyboardType.NumberPad:
			return c >= '0' && c <= '9';
		case TouchScreenKeyboardType.NumbersAndPunctuation:
			if (c >= '0' && c <= '9')
			{
				return true;
			}
			if (c == '-')
			{
				return current.Length == 0;
			}
			if (c == '.')
			{
				return current.IndexOf('.') < 0;
			}
			return false;
		case TouchScreenKeyboardType.ASCIICapable:
			return c >= 32 && c <= 126;
		default:
			return !char.IsControl(c);
		}
	}
#endif
}
