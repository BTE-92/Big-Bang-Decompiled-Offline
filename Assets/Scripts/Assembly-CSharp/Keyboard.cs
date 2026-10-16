using System;
using UnityEngine;

public class Keyboard
{
	private KeyboardDataMode m_keyboardDataMode;

	private Action<string> m_doneCallback;

	private Action<string> m_keyPressedCallback;

	private Action m_cancelCallback;

	private Action m_tooShortStringCallback;

	private int m_arrayTargetIndex;

	private TouchScreenKeyboard m_keyboard;

	private TouchScreenKeyboardType m_keyboardType;

	private bool m_multiline;

	private int m_maxCharacterCount;

	private int m_minCharacterCount;

	private string m_originalText;

	private string m_text;

	private string m_placeholderText;

	private TextModel m_textModel;

	public static bool m_active;

	public Keyboard(Action<string> _doneCallback, Action _cancelCallback, Action _tooShortStringCallback = null, Action<string> _keyPressedCallback = null, TouchScreenKeyboardType _keyboardType = TouchScreenKeyboardType.ASCIICapable)
	{
		m_keyboardDataMode = KeyboardDataMode.Callback;
		m_doneCallback = _doneCallback;
		m_cancelCallback = _cancelCallback;
		m_keyPressedCallback = _keyPressedCallback;
		Construct();
	}

	public Keyboard(TextModel _model, Action _tooShortStringCallbackUIComponent = null, TouchScreenKeyboardType _keyboardType = TouchScreenKeyboardType.ASCIICapable)
	{
		m_keyboardDataMode = KeyboardDataMode.TextModel;
		m_keyboardType = _keyboardType;
		m_textModel = _model;
		m_text = m_textModel.m_text;
		Construct();
	}

	public void Construct()
	{
		m_placeholderText = string.Empty;
		m_multiline = false;
		m_minCharacterCount = -1;
		m_maxCharacterCount = 128;
	}

	public void SetPlaceholderText(string _placeholder)
	{
		m_placeholderText = _placeholder;
	}

	public void SetMaxCharacterCount(int _value)
	{
		m_maxCharacterCount = _value;
	}

	public void SetMinCharacterCount(int _value)
	{
		m_minCharacterCount = _value;
	}

	public void SetValue(object _value)
	{
		m_text = _value.ToString();
		if (m_keyboardDataMode == KeyboardDataMode.TextModel)
		{
			m_textModel.m_uiModel.SetValue(_value.ToString(), "m_text");
		}
	}

	public virtual void OpenKeyboard()
	{
		Debug.Log("OPEN KEYBOARD");
		m_active = true;
		if (m_keyboard == null)
		{
			if (m_placeholderText != string.Empty)
			{
				m_keyboard = TouchScreenKeyboard.Open(m_text, m_keyboardType, false, m_multiline, false, true, m_placeholderText);
				return;
			}
			m_keyboard = TouchScreenKeyboard.Open(m_text, m_keyboardType, false, m_multiline, false, true);
			m_keyboard.text = m_text;
		}
		else
		{
			m_keyboard.active = true;
		}
	}

	public void CloseKeyboard(bool _cancel = false)
	{
		Debug.Log("CLOSE KEYBOARD");
		m_active = false;
		if (m_keyboard != null)
		{
			m_keyboard.active = false;
		}
		if (m_text.Length >= m_minCharacterCount || m_tooShortStringCallback == null)
		{
			if (m_keyboard == null)
			{
				return;
			}
			if (m_keyboard.wasCanceled || _cancel)
			{
				if (m_cancelCallback != null)
				{
					m_cancelCallback();
				}
				m_textModel.m_uiModel.SetValue(true, "m_cancelled");
				return;
			}
			if (m_textModel == null)
			{
				if (m_doneCallback != null)
				{
					m_doneCallback(m_text);
				}
				return;
			}
			if (m_keyboardType == TouchScreenKeyboardType.NumberPad)
			{
				int result = 0;
				if (int.TryParse(m_text, out result))
				{
					SetValue(result);
				}
			}
			else if (m_keyboardType == TouchScreenKeyboardType.NumbersAndPunctuation)
			{
				float result2 = 0f;
				if (float.TryParse(m_text, out result2))
				{
					SetValue(result2);
				}
			}
			else
			{
				SetValue(m_text);
			}
			m_textModel.m_uiModel.SetValue(true, "m_done");
		}
		else if (m_tooShortStringCallback != null)
		{
			m_tooShortStringCallback();
		}
	}

	public virtual void UpdateKeyboard()
	{
		string text = m_text;
		text = m_keyboard.text;
		if (m_maxCharacterCount > -1 && m_keyboard.text.Length > m_maxCharacterCount)
		{
			text = m_keyboard.text.Substring(0, m_maxCharacterCount);
			m_keyboard.text = text;
		}
		if (m_keyboard.done)
		{
			CloseKeyboard(m_keyboard.wasCanceled);
		}
		else if (m_text != text)
		{
			SetValue(ToolBox.GetUTF8String(text));
			if (m_keyPressedCallback != null)
			{
				m_keyPressedCallback(text);
			}
		}
	}
}
