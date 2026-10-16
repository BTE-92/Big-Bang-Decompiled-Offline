using UnityEngine;

public class UITextArea : UIVerticalList
{
	private UITextbox m_label;

	private UITextbox m_value;

	private UITextbox m_tip;

	protected string m_labelText;

	private int m_maxCharacterCount;

	public UITextArea(UIComponent _parent, string _tag, string _label, string _tip, Camera _camera, UIModel _model, string _STRINGfieldName, int rows = -1, int _maxCharacterCount = 128)
		: base(_parent, _tag)
	{
		m_model = _model;
		m_fieldName = _STRINGfieldName;
		m_labelText = _label;
		m_maxCharacterCount = _maxCharacterCount;
		CreateTouchAreas();
		RemoveDrawHandler();
		UIVerticalList uIVerticalList = new UIVerticalList(this, _tag);
		uIVerticalList.SetSpacing(-0.01f, RelativeTo.ScreenHeight);
		uIVerticalList.RemoveDrawHandler();
		if (!string.IsNullOrEmpty(_label))
		{
			m_label = new UITextbox(uIVerticalList, false, _tag, "<color=#464646>" + _label + "</color>", "HurmeSemiBold_Font", 0.0225f, RelativeTo.ScreenHeight, true, Align.Left, Align.Middle);
			m_label.SetMargins(0.025f, 0.025f, 0.01f, 0.01f);
			m_label.SetHorizontalAlign(0f);
			m_label.SetDrawHandler(UIDrawHandlers.Textfield);
		}
		m_value = new UITextbox(uIVerticalList, false, _tag, "<color=#464646>" + (string)GetValue() + "</color>", "HurmeSemiBold_Font", 0.04f, RelativeTo.ScreenHeight);
		m_value.SetMargins(0.0375f, 0.0375f, 0.025f, 0.025f);
		m_value.SetWidth(1f, RelativeTo.ParentWidth);
		m_value.SetHeight(0.3f, RelativeTo.OwnWidth);
		m_value.SetHorizontalAlign(0f);
		m_value.SetDrawHandler(UIDrawHandlers.Textfield);
		if (rows > 0)
		{
			m_value.SetMinRows(3);
			m_value.SetMaxRows(3);
		}
		TextMeshS.SetAlign(m_value.m_tmc, Align.Left);
		if (_tip != string.Empty)
		{
			m_tip = new UITextbox(this, false, _tag, _tip, "HurmeRegular_Font", 0.02f, RelativeTo.ScreenHeight);
			m_tip.SetMargins(0.025f, 0f, 0.015f, 0f);
			m_tip.RemoveDrawHandler();
		}
	}

	public override void SetValue(object _value)
	{
		base.SetValue(_value);
		m_value.SetText("<color=#464646>" + (string)_value + "</color>");
	}
}
