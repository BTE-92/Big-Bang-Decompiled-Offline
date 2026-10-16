public class PsUICenterNotEnoughKeys : PsUIScrollableBase
{
	private PsUIGenericButton m_refill;

	private PsUIGenericButton m_increase;

	private PsUIGenericButton m_video;

	private int m_missingStars;

	public PsUICenterNotEnoughKeys(UIComponent _parent)
		: base(_parent)
	{
	}

	public override void CreateHeaderContent(UIComponent _parent)
	{
		UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.025f, 0.025f, 0f, 0f);
		uIHorizontalList.SetHorizontalAlign(0.5f);
		UIText parent = new UIText(uIHorizontalList, false, string.Empty, "Out of keys", "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
		UICanvas uICanvas = new UICanvas(parent, false, string.Empty, null, string.Empty);
		uICanvas.SetHorizontalAlign(0f);
		uICanvas.SetMargins(-1.5f, 0f, -0.125f, -0.125f, RelativeTo.ParentHeight);
		uICanvas.SetDepthOffset(-2f);
		uICanvas.RemoveDrawHandler();
		Frame frame = PsState.m_uiSheet.m_atlas.GetFrame("menu_resources_tester_icon_no_outline");
		UISprite uISprite = new UISprite(uICanvas, false, string.Empty, PsState.m_uiSheet, frame);
		uISprite.SetSize(1f, 1f * (frame.height / frame.width), RelativeTo.ParentHeight);
		uISprite.SetHorizontalAlign(0f);
		PsUIInfoBar psUIInfoBar = new PsUIInfoBar(GetRoot(), string.Empty);
		psUIInfoBar.SetVerticalAlign(0f);
		psUIInfoBar.SetText("Access INSTANTLY by gaining the missing keys.");
	}

	public override void CreateContent(UIComponent _parent)
	{
		UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetHorizontalAlign(0.5f);
		uIHorizontalList.SetSpacing(0.01f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.025f);
		m_refill = new PsUIGenericButton(uIHorizontalList);
		m_refill.SetShopButton(PsMetagameManager.GetKeyReloadPrice().ToString(), "REFILL KEYS", "menu_shop_item_refillKeys");
		m_refill.m_TAC.m_letTouchesThrough = true;
		if (PsMetagameManager.m_playerStats.maxKeys < 20)
		{
			m_increase = new PsUIGenericButton(uIHorizontalList);
			m_increase.SetShopButton(PsMetagameManager.GetKeyIncreasePrice(PsMetagameManager.m_playerStats.maxKeys + 1).ToString(), "MAX KEYS +1 AND REFILL", "menu_shop_item_refillKeysMaxPlus1");
			m_increase.m_TAC.m_letTouchesThrough = true;
		}
		if (PsAdMediation.adsAvailable())
		{
			m_video = new PsUIGenericButton(uIHorizontalList);
			m_video.SetShopButton("VIDEO", "FREE UNLOCK", "menu_shop_item_refillKeys", false);
			m_video.m_TAC.m_letTouchesThrough = true;
			m_video.SetGreenColors();
		}
	}

	public override void Step()
	{
		if (m_refill.m_hit)
		{
			if (PsMetagameManager.m_playerStats.diamonds >= PsMetagameManager.GetKeyReloadPrice())
			{
				(GetRoot() as PsUIBasePopup).CallAction("Refill");
			}
			else
			{
				new PsGetDiamondsFlow();
			}
		}
		else if (m_increase != null && m_increase.m_hit)
		{
			int keyIncreasePrice = PsMetagameManager.GetKeyIncreasePrice(PsMetagameManager.m_playerStats.maxKeys + 1);
			if (PsMetagameManager.m_playerStats.diamonds >= keyIncreasePrice)
			{
				(GetRoot() as PsUIBasePopup).CallAction("IncreaseAndRefill");
			}
			else
			{
				new PsGetDiamondsFlow();
			}
		}
		else if (m_video != null && m_video.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Video");
		}
		base.Step();
	}
}
