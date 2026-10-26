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
        // Editor / Standalone: hardware keyboard only.
        // WebGL: only open the touch keyboard if the browser/device supports it.
        // Native mobile: always (original behaviour).
        if (!HardwareKeyboardInput.ShouldOpenTouchKeyboard)
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
    }

    protected virtual void UpdateKeyboard()
    {
        // One input source at a time. If a touch keyboard is open it owns the text
        // (on WebGL mobile a connected hardware keyboard types into it as well);
        // otherwise read the hardware keyboard. Reading both would let the touch
        // keyboard's text overwrite what was typed on the hardware keyboard.
        if (m_keyboard != null)
        {
            UpdateTouchKeyboard();
        }
        else
        {
            UpdateHardwareKeyboard();
        }
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

    private void UpdateHardwareKeyboard()
    {
        if (m_errorPopup != null)
        {
            return;
        }

        string text = m_text;
        bool changed;
        HardwareKeyAction action = HardwareKeyboardInput.Poll(ref text, m_content.m_maxCharacterCount, m_content.m_keyboardType, m_ignoreCharacters, out changed);

        if (changed)
        {
            m_text = ToolBox.GetUTF8String(text);
            if (m_text.IndexOf(" ", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                m_text = m_text.Remove(0, 1);
            }
            UpdateView();
        }

        if (action == HardwareKeyAction.Submit)
        {
            CloseKeyboard(false);
        }
        else if (action == HardwareKeyAction.Cancel)
        {
            CloseKeyboard(true);
        }
    }

    public void CloseKeyboard(bool _cancel = false)
    {
        if (m_keyboard != null)
        {
            m_keyboard.active = false;
        }
        if (m_text.Length >= m_content.m_minCharacterCount || _cancel)
        {
            if (m_keyboard != null || HardwareKeyboardInput.IsSupported)
            {
                bool isCanceled = _cancel || (m_keyboard != null && m_keyboard.wasCanceled);
                if (isCanceled)
                {
                    m_content.Cancel();
                }
                else
                {
                    m_content.Done();
                }
            }
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
        if (HardwareKeyboardInput.ShouldOpenTouchKeyboard)
        {
            m_keyboard = TouchScreenKeyboard.Open(m_text, m_content.m_keyboardType, false, m_multiline, false, true);
            m_keyboard.text = m_text;
        }
    }
}