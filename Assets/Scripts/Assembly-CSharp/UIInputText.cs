using System;
using UnityEngine;

public class UIInputText : UIScrollableCanvas
{
    private UICanvas m_mainArea;

    private PsUIInputTextField m_content;

    private PsUIGenericButton m_backButton;

    private UITooShortNamePopup m_errorPopup;

    private string m_placeholderText;

    private bool m_multiline;

    private char[] m_ignoreCharacters;

    private string m_text = string.Empty;

    private TouchScreenKeyboard m_keyboard;

    public UIInputText(PsUIInputTextField _content, params char[] _ignoreCharacters)
        : base(null, string.Empty)
    {
        m_text = _content.GetText();
        m_ignoreCharacters = _ignoreCharacters;
        m_placeholderText = string.Empty;
        m_multiline = false;
        SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
        m_maxScrollInertialY = 0f;
        m_mainArea = new UICanvas(this, false, string.Empty, null, string.Empty);
        m_mainArea.SetHeight(0.4f, RelativeTo.ScreenHeight);
        m_mainArea.SetWidth(1f, RelativeTo.ScreenWidth);
        m_mainArea.SetAlign(0f, 1f);
        m_mainArea.RemoveDrawHandler();
        UIHorizontalList uIHorizontalList = new UIHorizontalList(m_mainArea, "UpperLeft");
        uIHorizontalList.SetMargins(0.025f, RelativeTo.ScreenShortest);
        uIHorizontalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
        uIHorizontalList.SetAlign(0f, 1f);
        uIHorizontalList.RemoveDrawHandler();
        m_backButton = new PsUIGenericButton(uIHorizontalList);
        m_backButton.SetIcon("hud_icon_back", 0.06f);
        m_backButton.SetOrangeColors();
        m_content = _content;
        m_content.Parent(m_mainArea);
        m_content.SetAlign(0.5f, 0.5f);
    }

    public override void Update()
    {
        OpenKeyboard();
        base.Update();
    }

    protected virtual void UpdateView()
    {
        m_content.SetText(m_text);
        m_mainArea.Update();
    }

    public override void Step()
    {
        if (m_backButton.m_hit)
        {
            CloseKeyboard(true);
            return;
        }
        UpdateKeyboard();
        base.Step();
    }

    protected virtual void OpenKeyboard()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // Standalone / Editor: Do not open TouchScreenKeyboard
        return;
#elif UNITY_WEBGL
		// WebGL: Open TouchScreenKeyboard only if the device/browser supports it (e.g. mobile browser)
		if (!TouchScreenKeyboard.isSupported)
		{
			return;
		}

		if (m_keyboard == null)
		{
			if (m_placeholderText != string.Empty)
			{
				m_keyboard = TouchScreenKeyboard.Open(m_text, m_content.m_keyboardType, false, m_multiline, false, true, m_placeholderText);
				return;
			}
			m_keyboard = TouchScreenKeyboard.Open(m_text, m_content.m_keyboardType, false, m_multiline, false, true);
			m_keyboard.text = m_text;
		}
		else
		{
			m_keyboard.active = true;
		}
#else
		// Mobile platforms: Exact original code
		if (m_keyboard == null)
		{
			if (m_placeholderText != string.Empty)
			{
				m_keyboard = TouchScreenKeyboard.Open(m_text, m_content.m_keyboardType, false, m_multiline, false, true, m_placeholderText);
				return;
			}
			m_keyboard = TouchScreenKeyboard.Open(m_text, m_content.m_keyboardType, false, m_multiline, false, true);
			m_keyboard.text = m_text;
		}
		else
		{
			m_keyboard.active = true;
		}
#endif
    }

    protected virtual void UpdateKeyboard()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        UpdateHardwareKeyboard();
#elif UNITY_WEBGL
		// WebGL: Process TouchScreenKeyboard if active, as well as hardware input
		if (m_keyboard != null)
		{
			UpdateTouchKeyboard();
		}
		UpdateHardwareKeyboard();
#else
		// Mobile platforms: Exact original code
		UpdateTouchKeyboard();
#endif
    }

    private void UpdateTouchKeyboard()
    {
        string text = m_text;
        text = m_keyboard.text;
        if (m_ignoreCharacters != null)
        {
            for (int i = 0; i < m_ignoreCharacters.Length; i++)
            {
                text = text.Replace(m_ignoreCharacters[i].ToString(), string.Empty);
            }
            m_keyboard.text = text;
        }
        if (m_content.m_maxCharacterCount > -1 && m_keyboard.text.Length > m_content.m_maxCharacterCount)
        {
            text = m_keyboard.text.Substring(0, m_content.m_maxCharacterCount);
            m_keyboard.text = text;
        }
        if (m_keyboard.done)
        {
            CloseKeyboard(m_keyboard.wasCanceled);
            return;
        }
        if (!m_keyboard.active)
        {
            CloseKeyboard(true);
        }
        if (m_text != text)
        {
            m_text = ToolBox.GetUTF8String(text);
            if (m_text.IndexOf(" ", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                m_text = m_text.Remove(0, 1);
            }
            UpdateView();
        }
    }

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
    private void UpdateHardwareKeyboard()
    {
        if (m_errorPopup != null)
        {
            return;
        }

        // Submit on Enter
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            CloseKeyboard(false);
            return;
        }

        // Cancel on Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseKeyboard(true);
            return;
        }

        string text = m_text;
        bool textChanged = false;

        for (int i = 0; i < Input.inputString.Length; i++)
        {
            char c = Input.inputString[i];

            if (c == '\b') // Backspace
            {
                if (text.Length > 0)
                {
                    text = text.Substring(0, text.Length - 1);
                    textChanged = true;
                }
            }
            else if (c == '\n' || c == '\r')
            {
                continue;
            }
            else
            {
                // Filter ignoreCharacters
                if (m_ignoreCharacters != null && Array.IndexOf(m_ignoreCharacters, c) >= 0)
                {
                    continue;
                }

                // ASCIICapable filter
                if (m_content != null && m_content.m_keyboardType == TouchScreenKeyboardType.ASCIICapable)
                {
                    if (c < 32 || c > 126)
                    {
                        continue;
                    }
                }

                // Character Limit check
                if (m_content != null && m_content.m_maxCharacterCount > -1 && text.Length >= m_content.m_maxCharacterCount)
                {
                    continue;
                }

                text += c;
                textChanged = true;
            }
        }

        if (textChanged)
        {
            m_text = ToolBox.GetUTF8String(text);
            if (m_text.IndexOf(" ", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                m_text = m_text.Remove(0, 1);
            }
            UpdateView();
        }
    }
#endif

    public void CloseKeyboard(bool _cancel = false)
    {
        if (m_keyboard != null)
        {
            m_keyboard.active = false;
        }
        if (m_text.Length >= m_content.m_minCharacterCount || _cancel)
        {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            bool isCanceled = _cancel || (m_keyboard != null && m_keyboard.wasCanceled);
            if (isCanceled)
            {
                m_content.Cancel();
            }
            else
            {
                m_content.Done();
            }
#else
			if (m_keyboard != null)
			{
				if (m_keyboard.wasCanceled || _cancel)
				{
					m_content.Cancel();
				}
				else
				{
					m_content.Done();
				}
			}
#endif
            Destroy();
        }
        else if (m_errorPopup == null)
        {
            m_errorPopup = new UITooShortNamePopup(m_content.m_minCharacterCount, ErrorPopupClosed);
        }
    }

    private void ErrorPopupClosed()
    {
        m_errorPopup = null;
#if UNITY_EDITOR || UNITY_STANDALONE
        // No touch keyboard reopen needed
#elif UNITY_WEBGL
		if (TouchScreenKeyboard.isSupported)
		{
			m_keyboard = TouchScreenKeyboard.Open(m_text, m_content.m_keyboardType, false, m_multiline, false, true);
			m_keyboard.text = m_text;
		}
#else
		m_keyboard = TouchScreenKeyboard.Open(m_text, m_content.m_keyboardType, false, m_multiline, false, true);
		m_keyboard.text = m_text;
#endif
    }
}