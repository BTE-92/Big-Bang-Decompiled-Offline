public class PsUICenterNotEnoughStars : PsUIHeaderedCanvas
{
	private PsUIGenericButton m_buy;

	private int m_missingStars;

	public PsUICenterNotEnoughStars(UIComponent _parent)
		: base(_parent, string.Empty)
	{
		SetWidth(0.65f, RelativeTo.ScreenWidth);
		SetHeight(0.45f, RelativeTo.ScreenHeight);
		SetVerticalAlign(0.4f);
		SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
		SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
		m_header.SetDrawHandler(PsUIDrawHandlers.ScrollingUIHeader);
		m_header.SetMargins(0.0125f, 0.0125f, 0.0125f, 0f);
		CreateContent(this);
		CreateHeaderContent(m_header);
	}

	public void CreateHeaderContent(UIComponent _parent)
	{
		UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.025f, 0.025f, 0f, 0f);
		uIHorizontalList.SetHorizontalAlign(0.5f);
		UIText parent = new UIText(uIHorizontalList, false, string.Empty, "Not enough stars", "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
		UICanvas uICanvas = new UICanvas(parent, false, string.Empty, null, string.Empty);
		uICanvas.SetHorizontalAlign(0f);
		uICanvas.SetMargins(-1.5f, 0f, -0.125f, -0.125f, RelativeTo.ParentHeight);
		uICanvas.SetDepthOffset(-2f);
		uICanvas.RemoveDrawHandler();
		Frame frame = PsState.m_uiSheet.m_atlas.GetFrame("hud_icon_star_full");
		UISprite uISprite = new UISprite(uICanvas, false, string.Empty, PsState.m_uiSheet, frame);
		uISprite.SetSize(1f, 1f * (frame.height / frame.width), RelativeTo.ParentHeight);
		uISprite.SetHorizontalAlign(0f);
	}

	public void CreateContent(UIComponent _parent)
	{
		m_missingStars = 0;
		if (PsState.m_activeGameLoop is PsGameLoopBlockLevel)
		{
			m_missingStars = (PsState.m_activeGameLoop as PsGameLoopBlockLevel).m_requiredStars - PsMetagameManager.m_playerStats.stars;
		}
		else if (PsState.m_activeGameLoop is PsGameLoopBlockSignal)
		{
			m_missingStars = (PsState.m_activeGameLoop as PsGameLoopBlockSignal).m_requiredStars - PsMetagameManager.m_playerStats.stars;
		}
		else if (PsState.m_activeGameLoop is PsGameLoopBlockSignalWithoutLevel)
		{
			m_missingStars = (PsState.m_activeGameLoop as PsGameLoopBlockSignalWithoutLevel).m_requiredStars - PsMetagameManager.m_playerStats.stars;
		}
		_parent.RemoveTouchAreas();
		string text = "Get the missing <color=#fbdd28>" + m_missingStars.ToString() + "</color> STARS by using <color=#f15bfb>" + PsMetagameManager.StarsToDiamonds(m_missingStars) + "</color> " + ((PsMetagameManager.StarsToDiamonds(m_missingStars) != 1) ? "GEMS" : "GEM") + "?";
		UITextbox uITextbox = new UITextbox(_parent, false, string.Empty, text, "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, false, Align.Center, Align.Middle);
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetAlign(1f, 0f);
		uIHorizontalList.SetMargins(0f, 0.1f, 0.05f, -0.05f);
		m_buy = new PsUIGenericButton(uIHorizontalList);
		m_buy.SetAlign(1f, 1f);
		m_buy.SetText("Get Stars", 0.05f);
		m_buy.SetDiamondPrice(PsMetagameManager.StarsToDiamonds(m_missingStars), 0.05f);
		m_buy.SetGreenColors();
	}

	public override void Step()
	{
		if (m_buy.m_hit)
		{
			if (PsMetagameManager.m_playerStats.diamonds >= PsMetagameManager.StarsToDiamonds(m_missingStars))
			{
				(GetRoot() as PsUIBasePopup).CallAction("Purchase");
			}
			else
			{
				new PsGetDiamondsFlow();
			}
		}
		base.Step();
	}
}
