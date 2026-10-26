using System;
using UnityEngine;

public class UITextInput : UIScrollableCanvas
{
    private KeyboardDataMode m_textInputType;

    private Action<string> m_doneCallback;

    private Action<string> m_keyPressedCallback;

    private Action m_cancelCallback;

    private int m_arrayTargetIndex;

    private TouchScreenKeyboard m_keyboard;

    private TouchScreenKeyboardType m_keyboardType;

    private bool m_hideUI;

    private bool m_multiline;

    private int m_maxCharacterCount;

    private int m_minCharacterCount;

    private string m_originalText;

    private string m_text;

    private string m_placeholderText;

    private string m_labelText;

    private UITooShortNamePopup m_errorPopup;

    private PsUIGenericButton m_doneButton;

    private PsUIGenericButton m_cancelButton;

    private UICanvas m_canvas;

    private UITextbox m_textbox;

    private UITextbox m_label;

    public UITextInput(string _label, Action<string> _doneCallback, Action _cancelCallback, Action<string> _keyPressedCallback = null, TouchScreenKeyboardType _keyboardType = TouchScreenKeyboardType.ASCIICapable, bool _hideOtherUIComponents = true, int _maxCharacterCount = 128)
        : base(null, "TextInput")
    {
        m_textInputType = KeyboardDataMode.Callback;
        m_keyboardType = _keyboardType;
        m_doneCallback = _doneCallback;
        m_cancelCallback = _cancelCallback;
        m_keyPressedCallback = _keyPressedCallback;
        m_labelText = _label;
        m_maxCharacterCount = _maxCharacterCount;
        Construct();
    }

    public UITextInput(string _label, UIModel _model, string _STRINGfieldName, TouchScreenKeyboardType _keyboardType = TouchScreenKeyboardType.ASCIICapable, bool _hideOtherUIComponents = true, int _maxCharacterCount = 128)
        : base(null, "TextInput")
    {
        m_textInputType = KeyboardDataMode.Reflection;
        m_keyboardType = _keyboardType;
        m_hideUI = _hideOtherUIComponents;
        if (m_hideUI)
        {
            EntityManager.SetVisibilityOfEntitiesWithTag("UIComponent", false);
        }
        m_labelText = _label;
        m_maxCharacterCount = _maxCharacterCount;
        m_model = _model;
        m_fieldName = _STRINGfieldName;
        Construct();
    }

    public UITextInput(string _label, UIModel _model, string _STRINGARRAYfieldName, int _arrayTargetIndex, TouchScreenKeyboardType _keyboardType = TouchScreenKeyboardType.ASCIICapable, bool _hideOtherUIComponents = true, int _maxCharacterCount = 128)
        : base(null, "TextInput")
    {
        m_textInputType = KeyboardDataMode.ArrayReflection;
        m_keyboardType = _keyboardType;
        m_hideUI = _hideOtherUIComponents;
        if (m_hideUI)
        {
            EntityManager.SetVisibilityOfEntitiesWithTag("UIComponent", false);
        }
        m_labelText = _label;
        m_maxCharacterCount = _maxCharacterCount;
        m_model = _model;
        m_fieldName = _STRINGARRAYfieldName;
        m_arrayTargetIndex = _arrayTargetIndex;
        Construct();
    }

    public void Construct()
    {
        SetWidth(1f, RelativeTo.ScreenWidth);
        SetHeight(1f, RelativeTo.ScreenHeight);
        m_text = string.Empty;
        m_placeholderText = string.Empty;
        m_multiline = false;
        m_minCharacterCount = -1;
        SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
        m_maxScrollInertialY = 0f;
        UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
        uIHorizontalList.SetMargins(0.025f);
        uIHorizontalList.SetAlign(0f, 1f);
        uIHorizontalList.RemoveDrawHandler();
        m_cancelButton = new PsUIGenericButton(uIHorizontalList);
        m_cancelButton.SetText("Cancel", 0.04f);
        m_cancelButton.SetHeight(0.06f, RelativeTo.ScreenHeight);
        CreateView();
    }

    public void SetText(string _text)
    {
        m_text = _text;
        if (m_textbox != null)
        {
            m_textbox.SetText("<color=#000000>" + m_text + "</color>");
        }
    }

    public void SetPlaceholderText(string _placeholder)
    {
        m_placeholderText = _placeholder;
        if (m_textbox != null)
        {
            m_textbox.SetText("<color=#666666>" + m_placeholderText + "</color>");
        }
    }

    public void SetMaxCharacterCount(int _value)
    {
        m_maxCharacterCount = _value;
    }

    public void SetMinCharacterCount(int _value)
    {
        m_minCharacterCount = _value;
    }

    public override void Update()
    {
        OpenKeyboard();
        base.Update();
    }

    public override void Step()
    {
        if (m_cancelButton.m_hit)
        {
            CloseKeyboard(true);
            return;
        }
        if (m_doneButton.m_hit)
        {
            CloseKeyboard();
            return;
        }
        UpdateKeyboard();
        base.Step();
    }

    public override void SetValue(object _value)
    {
        if (m_textInputType == KeyboardDataMode.Reflection)
        {
            base.SetValue(_value);
        }
        else if (m_textInputType == KeyboardDataMode.ArrayReflection)
        {
            object[] array = (object[])GetValue();
            array[m_arrayTargetIndex] = _value;
            base.SetValue(array);
        }
    }

    public void CloseKeyboard(bool _cancel = false)
    {
        if (m_keyboard != null)
        {
            m_keyboard.active = false;
        }
        if (m_text.Length >= m_minCharacterCount || _cancel)
        {
            if (m_keyboard != null || HardwareKeyboardInput.IsSupported)
            {
                if ((m_keyboard != null && m_keyboard.wasCanceled) || _cancel)
                {
                    if (m_cancelCallback != null)
                    {
                        m_cancelCallback();
                    }
                }
                else if (m_model == null)
                {
                    if (m_doneCallback != null)
                    {
                        m_doneCallback(m_text);
                    }
                }
                else if (m_keyboardType == TouchScreenKeyboardType.NumberPad)
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
            }
            Destroy();
        }
        else if (m_errorPopup == null)
        {
            m_errorPopup = new UITooShortNamePopup(m_minCharacterCount, ErrorPopupClosed);
        }
    }

    private void ErrorPopupClosed()
    {
        m_errorPopup = null;
        if (HardwareKeyboardInput.ShouldOpenTouchKeyboard)
        {
            m_keyboard = TouchScreenKeyboard.Open(m_text, m_keyboardType, false, m_multiline, false, true);
            m_keyboard.text = m_text;
        }
        m_doneButton.m_hit = false;
        Debug.Log("error popup closed");
    }

    protected virtual void OpenKeyboard()
    {
        if (!HardwareKeyboardInput.ShouldOpenTouchKeyboard)
        {
            return;
        }
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

    protected virtual void CreateView()
    {
        m_canvas = new UICanvas(this, false, string.Empty, null, string.Empty);
        m_canvas.SetHeight(0.4f, RelativeTo.ScreenHeight);
        m_canvas.SetVerticalAlign(1f);
        m_canvas.RemoveDrawHandler();
        UIHorizontalList uIHorizontalList = new UIHorizontalList(m_canvas, string.Empty);
        uIHorizontalList.SetSpacing(0.025f, RelativeTo.ScreenHeight);
        uIHorizontalList.RemoveDrawHandler();
        uIHorizontalList.SetHorizontalAlign(0.7f);
        UIVerticalList uIVerticalList = new UIVerticalList(uIHorizontalList, string.Empty);
        uIVerticalList.SetSpacing(-0.01f, RelativeTo.ScreenHeight);
        uIVerticalList.RemoveDrawHandler();
        m_label = new UITextbox(uIVerticalList, false, string.Empty, "<color=#464646>" + m_labelText + "</color>", "HurmeSemiBold_Font", 0.0225f, RelativeTo.ScreenHeight, true, Align.Left, Align.Middle);
        m_label.SetMargins(0.025f, 0.025f, 0.01f, 0.01f);
        m_label.SetHorizontalAlign(0f);
        m_label.SetDrawHandler(UIDrawHandlers.Textfield);
        string text = "<color=#000000>" + m_text + "</color>";
        if (m_placeholderText != string.Empty)
        {
            text = "<color=#666666>" + m_placeholderText + "</color>";
        }
        m_textbox = new UITextbox(uIVerticalList, false, string.Empty, text, "HurmeSemiBold_Font", 0.035f, RelativeTo.ScreenHeight);
        m_textbox.SetWidth(0.618f, RelativeTo.ScreenWidth);
        m_textbox.SetDrawHandler(UIDrawHandlers.DebugRect);
        m_textbox.SetMargins(0.0375f, 0.0375f, 0.025f, 0.025f);
        m_textbox.SetDrawHandler(UIDrawHandlers.Textfield);
        m_doneButton = new PsUIGenericButton(uIHorizontalList);
        m_doneButton.SetGreenColors();
        m_doneButton.SetText("Ok");
        m_doneButton.SetVerticalAlign(0f);
    }

    protected virtual void DestroyView()
    {
    }

    protected virtual void UpdateView()
    {
        m_textbox.SetText("<color=#000000>" + m_text + "</color>");
        m_canvas.Update();
    }

    protected virtual void UpdateKeyboard()
    {
        if (m_keyboard == null)
        {
            UpdateHardwareKeyboard();
            return;
        }
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
            m_text = ToolBox.GetUTF8String(text);
            if (m_keyPressedCallback != null)
            {
                m_keyPressedCallback(text);
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
        HardwareKeyAction action = HardwareKeyboardInput.Poll(ref text, m_maxCharacterCount, m_keyboardType, null, out changed);
        if (changed)
        {
            m_text = ToolBox.GetUTF8String(text);
            if (m_keyPressedCallback != null)
            {
                m_keyPressedCallback(text);
            }
            UpdateView();
        }
        if (action == HardwareKeyAction.Submit)
        {
            CloseKeyboard();
        }
        else if (action == HardwareKeyAction.Cancel)
        {
            CloseKeyboard(true);
        }
    }

    public override void Destroy()
    {
        DestroyView();
        base.Destroy();
        if (m_hideUI)
        {
            EntityManager.SetVisibilityOfEntitiesWithTag("UIComponent", true);
        }
    }
}