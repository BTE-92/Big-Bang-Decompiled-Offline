public class PsUIVersusCreatePopup : PsUIHeaderedCanvas
{
	private PsUIGenericButton m_ok;

	public VersusMetaData m_versusData;

	private string m_headerText;

	private string m_contentText;

	private string m_buttonText;

	private bool m_initialized;

	public PsUIVersusCreatePopup(UIComponent _parent)
		: base(_parent, string.Empty)
	{
		SetWidth(0.65f, RelativeTo.ScreenWidth);
		SetHeight(0.45f, RelativeTo.ScreenHeight);
		SetVerticalAlign(0.4f);
		SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
		SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
		m_header.SetDrawHandler(PsUIDrawHandlers.ScrollingUIHeader);
		m_header.SetMargins(0.0125f, 0.0125f, 0.0125f, 0f);
		m_initialized = false;
	}

	public void Initialize()
	{
		if (m_versusData.GetOpponentInfo() != null)
		{
			m_headerText = "OPPONENT FOUND";
			m_contentText = "First player to <color=#51ff08>win two rounds in a row</color> earns a trophy!";
			m_buttonText = "RACE";
		}
		else
		{
			m_headerText = "NEW RACE";
			m_contentText = "First player to <color=#51ff08>win two rounds in a row</color> earns a trophy!";
			m_buttonText = "RACE";
		}
		m_initialized = true;
	}

	public void CreateHeaderContent(UIComponent _parent)
	{
		if (m_initialized)
		{
			UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
			uIHorizontalList.RemoveDrawHandler();
			uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
			uIHorizontalList.SetMargins(0.025f, 0.025f, 0f, 0f);
			uIHorizontalList.SetHorizontalAlign(0.5f);
			UIText uIText = new UIText(uIHorizontalList, false, string.Empty, m_headerText, "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
		}
	}

	public void CreateContent(UIComponent _parent)
	{
		if (m_initialized)
		{
			_parent.RemoveTouchAreas();
			UITextbox uITextbox = new UITextbox(_parent, false, string.Empty, m_contentText, "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, false, Align.Center, Align.Middle);
			UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
			uIHorizontalList.RemoveDrawHandler();
			uIHorizontalList.SetAlign(0.5f, 0f);
			uIHorizontalList.SetMargins(0f, 0f, 0.075f, -0.075f);
			m_ok = new PsUIGenericButton(uIHorizontalList);
			m_ok.SetAlign(1f, 1f);
			m_ok.SetText(m_buttonText);
			m_ok.SetGreenColors();
		}
	}

	public override void Step()
	{
		if (m_versusData != null && !m_initialized)
		{
			Initialize();
			CreateContent(this);
			CreateHeaderContent(m_header);
			Update();
			m_header.Update();
		}
		if (m_ok != null && m_ok.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("SwitchLoop");
			(GetRoot() as PsUIBasePopup).Destroy();
		}
		base.Step();
	}
}
