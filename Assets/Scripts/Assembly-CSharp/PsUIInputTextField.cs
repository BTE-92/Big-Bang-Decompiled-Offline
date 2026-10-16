using System;
using UnityEngine;

public abstract class PsUIInputTextField : UIHorizontalList
{
	private PsUIInputTextField m_root;

	public TouchScreenKeyboardType m_keyboardType;

	public int m_minCharacterCount = -1;

	public int m_maxCharacterCount = -1;

	protected UIComponent m_textField;

	private Action<string> m_doneCallback;

	private Action m_cancelCallback;

	public PsUIInputTextField(UIComponent _parent)
		: base(_parent, "InputTextField")
	{
		m_keyboardType = TouchScreenKeyboardType.ASCIICapable;
		RemoveDrawHandler();
		ConstructUI();
	}

	protected abstract void ConstructUI();

	public override void Step()
	{
		if (m_root == null && ((m_textField != null && m_textField.m_hit) || m_textField.m_parent.m_hit))
		{
			PsUIInputTextField psUIInputTextField = Activator.CreateInstance(GetType()) as PsUIInputTextField;
			psUIInputTextField.m_textField.RemoveTouchAreas();
			psUIInputTextField.SetText(GetText());
			psUIInputTextField.m_root = this;
			UIInputText uIInputText = ((!PsState.m_adminMode) ? new UIInputText(psUIInputTextField, '>', '<', '\\', '/') : new UIInputText(psUIInputTextField));
			uIInputText.Update();
		}
		base.Step();
	}

	public void SetTextField(UITextbox _field)
	{
		m_textField = _field;
	}

	public void SetTextField(UIFittedText _field)
	{
		m_textField = _field;
	}

	public void SetText(string text)
	{
		if (m_textField != null)
		{
			if (m_textField is UITextbox)
			{
				(m_textField as UITextbox).SetText(text);
			}
			else if (m_textField is UIFittedText)
			{
				(m_textField as UIFittedText).SetText(text);
				m_textField.m_parent.Update();
			}
		}
	}

	public string GetText()
	{
		if (m_textField != null)
		{
			if (m_textField is UITextbox)
			{
				return (m_textField as UITextbox).m_text;
			}
			if (m_textField is UIFittedText)
			{
				return (m_textField as UIFittedText).m_text;
			}
		}
		return string.Empty;
	}

	public void SetCallbacks(Action<string> _done, Action _cancel = null)
	{
		m_doneCallback = _done;
		m_cancelCallback = _cancel;
	}

	public void Done()
	{
		if (m_root != null)
		{
			m_root.SetText(GetText());
			if (m_root.m_doneCallback != null)
			{
				m_root.m_doneCallback(GetText());
			}
		}
	}

	public void Cancel()
	{
		if (m_root != null && m_root.m_cancelCallback != null)
		{
			m_root.m_cancelCallback();
		}
	}

	public void SetMinMaxCharacterCount(int _minCharacterCount, int _maxCharacterCount)
	{
		m_minCharacterCount = _minCharacterCount;
		m_maxCharacterCount = _maxCharacterCount;
	}
}
