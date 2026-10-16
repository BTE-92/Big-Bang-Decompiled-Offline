public class UIArrayPropertyButton : UIVerticalListButton
{
	protected object[] m_values;

	public UIText m_label;

	public UIText m_value;

	public UIArrayPropertyButton(UIComponent _parent, UIScrollableCanvas _scrollableCanvas, UIPagedCanvas _pagedCanvas, string _tag, string _label, UIModel _model, string _fieldName, object[] _values)
		: base(_parent, _scrollableCanvas, _pagedCanvas, _tag, _model, _fieldName)
	{
		m_values = _values;
		SetWidth(UIVerticalListButton.m_defaultWidth, UIVerticalListButton.m_defaultWidthRelativeTo);
		SetHeight(UIVerticalListButton.m_defaultHeight, UIVerticalListButton.m_defaultHeightRelativeTo);
		SetMargins(UIVerticalListButton.m_defaultMargins, UIVerticalListButton.m_defaultMarginsRelativeTo);
		string text = "-";
		if (GetValue() != null)
		{
			text = GetValue().ToString();
		}
		m_label = new UIText(this, false, _tag, _label, "HurmeRegular_Font", 0.02f, RelativeTo.ScreenHeight);
		m_label.SetAlign(0f, 1f);
		m_value = new UIText(this, false, _tag, text, "HurmeSemiBold_Font", 0.02f, RelativeTo.ScreenHeight);
		m_value.SetAlign(1f, 0f);
	}

	public override void OnValueChange(object _newValue, object _oldValue)
	{
		string text = "-";
		if (_newValue != null)
		{
			text = _newValue.ToString();
		}
		m_value.SetText(text);
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		if (m_pagedCanvas.IsChangingPage())
		{
			return;
		}
		base.OnTouchRelease(_touch, _inside);
		if (_inside)
		{
			UIScrollableCanvas uIScrollableCanvas = new UIScrollableCanvas(m_scrollableCanvas.m_parent, string.Empty);
			uIScrollableCanvas.SetWidth(1f, RelativeTo.ParentWidth);
			uIScrollableCanvas.SetHeight(1f, RelativeTo.ParentHeight);
			UIVerticalList uIVerticalList = new UIVerticalList(uIScrollableCanvas, "VerticalArea");
			uIVerticalList.SetVerticalAlign(1f);
			UIText uIText = new UIText(uIVerticalList, false, string.Empty, m_label.m_text, "HurmeSemiBold_Font", 0.02f, RelativeTo.ScreenHeight, "#000000");
			for (int i = 0; i < m_values.Length; i++)
			{
				new UIArraySelectionButton(uIVerticalList, uIScrollableCanvas, m_pagedCanvas, "ListSelectorButton", m_model, m_fieldName, m_values[i].ToString(), m_values[i]);
			}
			m_pagedCanvas.NextPage();
			m_pagedCanvas.Update();
		}
	}
}
