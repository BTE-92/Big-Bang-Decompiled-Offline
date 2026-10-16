using System.Collections.Generic;

public class PsUITabbedProfile : UICanvas
{
	protected UIScrollableCanvas m_scrollArea;

	public List<PsUICreatePublishedBanner> m_levels = new List<PsUICreatePublishedBanner>();

	private PsUIGenericButton m_fbButton;

	private PsUIBasePopup m_waitingPopup;

	private bool m_contentLoaded;

	private UIScrollableCanvas m_tabCanvas;

	private UICanvas m_tab1;

	private UICanvas m_tab2;

	private UICanvas m_tab3;

	public static int m_selectedTab = 1;

	public PsUITabbedProfile(UIComponent _parent)
		: base(_parent, false, "ScrollableContent", null, string.Empty)
	{
		GetRoot().SetDrawHandler(PsUIDrawHandlers.DebriefBackground);
		m_contentLoaded = false;
		SetWidth(0.9f, RelativeTo.ScreenWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
		SetVerticalAlign(0f);
		RemoveDrawHandler();
		m_scrollArea = new UIScrollableCanvas(this, string.Empty);
		m_scrollArea.SetHeight(0.85f, RelativeTo.ScreenHeight);
		m_scrollArea.SetVerticalAlign(0f);
		m_scrollArea.RemoveDrawHandler();
		m_scrollArea.m_passTouchesToScrollableParents = true;
		CreateContent(m_scrollArea);
		CreateTabCanvas(false);
	}

	private void CreateTabCanvas(bool _update)
	{
		if (m_tabCanvas != null)
		{
			m_tabCanvas.Destroy();
		}
		m_tabCanvas = new UIScrollableCanvas(this, "Header");
		m_tabCanvas.SetWidth(1f, RelativeTo.ScreenWidth);
		m_tabCanvas.SetHeight(0.175f, RelativeTo.ScreenHeight);
		m_tabCanvas.SetVerticalAlign(1f);
		m_tabCanvas.RemoveTouchAreas();
		m_tabCanvas.RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(m_tabCanvas, string.Empty);
		uIHorizontalList.SetHeight(1f, RelativeTo.ParentHeight);
		uIHorizontalList.RemoveDrawHandler();
		m_tab1 = new UICanvas(uIHorizontalList, true, string.Empty, null, string.Empty);
		m_tab1.SetWidth(0.25f, RelativeTo.ScreenHeight);
		m_tab1.SetHeight(1f, RelativeTo.ParentHeight);
		m_tab1.SetMargins(0f, 0f, 0.02f, 0f);
		m_tab1.RemoveDrawHandler();
		CreateTab(m_tab1, "MY\nFRIENDS", m_selectedTab == 1);
		m_tab2 = new UICanvas(uIHorizontalList, true, string.Empty, null, string.Empty);
		m_tab2.SetWidth(0.25f, RelativeTo.ScreenHeight);
		m_tab2.SetHeight(1f, RelativeTo.ParentHeight);
		m_tab2.SetMargins(0f, 0f, 0.02f, 0f);
		m_tab2.RemoveDrawHandler();
		CreateTab(m_tab2, "MY\nLEVELS", m_selectedTab == 2);
		m_tab3 = new UICanvas(uIHorizontalList, true, string.Empty, null, string.Empty);
		m_tab3.SetWidth(0.25f, RelativeTo.ScreenHeight);
		m_tab3.SetHeight(1f, RelativeTo.ParentHeight);
		m_tab3.SetMargins(0f, 0f, 0.02f, 0f);
		m_tab3.RemoveDrawHandler();
		CreateTab(m_tab3, "FRIEND\nLEVELS", m_selectedTab == 3);
		UISprite uISprite = new UISprite(m_tabCanvas, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_tab_border"));
		uISprite.SetWidth(1f, RelativeTo.ScreenWidth);
		uISprite.SetHeight(0.05f, RelativeTo.ScreenHeight);
		uISprite.SetVerticalAlign(0f);
		if (_update)
		{
			Update();
		}
	}

	private void CreateTab(UICanvas _parent, string _text, bool _active)
	{
		if (_active)
		{
			UIFittedSprite uIFittedSprite = new UIFittedSprite(_parent, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_tab_active"));
			uIFittedSprite.SetVerticalAlign(1f);
			UIText uIText = new UIText(_parent, false, string.Empty, _text, "KGSecondChances_Font", 0.0375f, RelativeTo.ScreenHeight, "ffd22e", "184272");
			uIText.SetVerticalAlign(0.9f);
		}
		else
		{
			UIFittedSprite uIFittedSprite2 = new UIFittedSprite(_parent, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_tab_deactive"));
			uIFittedSprite2.SetDepthOffset(20f);
			uIFittedSprite2.SetVerticalAlign(0.9f);
			UIText uIText2 = new UIText(_parent, false, string.Empty, _text, "KGSecondChances_Font", 0.0375f, RelativeTo.ScreenHeight, "18324d");
			uIText2.SetVerticalAlign(0.8f);
		}
	}

	public virtual void CreateContent(UIComponent _parent)
	{
		_parent.DestroyChildren();
		if (m_selectedTab == 1)
		{
			new PsUIFriendList(_parent);
		}
		else if (m_selectedTab == 2)
		{
			new PsUIYourLevelList(_parent);
		}
		else if (m_selectedTab == 3)
		{
			new PsUIFriendLevelList(_parent);
		}
	}

	public override void Step()
	{
		if (m_tab1.m_hit && m_selectedTab != 1)
		{
			m_selectedTab = 1;
			CreateTabCanvas(true);
			CreateContent(m_scrollArea);
		}
		if (m_tab2.m_hit && m_selectedTab != 2)
		{
			m_selectedTab = 2;
			CreateTabCanvas(true);
			CreateContent(m_scrollArea);
		}
		if (m_tab3.m_hit && m_selectedTab != 3)
		{
			m_selectedTab = 3;
			CreateTabCanvas(true);
			CreateContent(m_scrollArea);
		}
		base.Step();
	}
}
