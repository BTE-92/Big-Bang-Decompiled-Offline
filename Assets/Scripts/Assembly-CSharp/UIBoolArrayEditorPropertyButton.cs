public class UIBoolArrayEditorPropertyButton : UIVerticalListButton
{
	public UIText m_label;

	public UIText m_value;

	public UIScrollableCanvas scrollableCanvas;

	public PsUIGenericButton m_backButton;

	public UIBoolArrayEditorPropertyButton(UIComponent _parent, UIScrollableCanvas _scrollableCanvas, UIPagedCanvas _pagedCanvas, string _tag, string _label, UIModel _model, string _BOOLARRAYfieldName)
		: base(_parent, _scrollableCanvas, _pagedCanvas, _tag, _model, _BOOLARRAYfieldName)
	{
		SetWidth(UIVerticalListButton.m_defaultWidth, UIVerticalListButton.m_defaultWidthRelativeTo);
		SetHeight(UIVerticalListButton.m_defaultHeight, UIVerticalListButton.m_defaultHeightRelativeTo);
		SetMargins(UIVerticalListButton.m_defaultMargins, UIVerticalListButton.m_defaultMarginsRelativeTo);
		string text = "-";
		bool[] array = (bool[])GetValue();
		for (int i = 0; i < array.Length; i++)
		{
			text = ((i != 0) ? (text + ", " + array.GetValue(i).ToString()) : array.GetValue(i).ToString());
		}
		m_label = new UIText(this, false, _tag, _label, "HurmeRegular_Font", 0.02f, RelativeTo.ScreenHeight);
		m_label.SetAlign(0f, 1f);
		m_value = new UIText(this, false, _tag, text, "HurmeSemiBold_Font", 0.02f, RelativeTo.ScreenHeight);
		m_value.SetAlign(1f, 0f);
	}

	public override void OnValueChange(object _newValue, object _oldValue)
	{
		string text = "-";
		bool[] array = (bool[])GetValue();
		for (int i = 0; i < array.Length; i++)
		{
			text = ((i != 0) ? (text + ", " + array.GetValue(i).ToString()) : array.GetValue(i).ToString());
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
			scrollableCanvas = new UIScrollableCanvas(m_scrollableCanvas.m_parent, string.Empty);
			scrollableCanvas.SetWidth(1f, RelativeTo.ParentWidth);
			scrollableCanvas.SetHeight(1f, RelativeTo.ParentHeight);
			UIVerticalList uIVerticalList = new UIVerticalList(scrollableCanvas, "VerticalArea");
			uIVerticalList.SetVerticalAlign(1f);
			UIText uIText = new UIText(uIVerticalList, false, string.Empty, m_label.m_text, "HurmeSemiBold_Font", 0.02f, RelativeTo.ScreenHeight, "#000000");
			bool[] array = (bool[])GetValue();
			for (int i = 0; i < array.Length; i++)
			{
				new UIBoolPropertyButton(uIVerticalList, "BoolEditButton", "[" + i + "]", m_model, m_fieldName, new string[2] { "No", "Yes" }, i);
			}
			m_backButton = new PsUIGenericButton(uIVerticalList);
			m_backButton.SetText("Back");
			m_backButton.SetOrangeColors();
			m_pagedCanvas.NextPage();
			m_pagedCanvas.Update();
		}
	}

	public override void Step()
	{
		if (m_backButton != null && m_backButton.m_hit)
		{
			scrollableCanvas.Destroy();
			scrollableCanvas = null;
			m_pagedCanvas.PreviousPage();
			m_pagedCanvas.Update();
		}
		base.Step();
	}
}
