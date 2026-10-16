using UnityEngine;

public class PsUICreateLevelBanner : UIHorizontalList
{
	protected PsUIScreenshot m_screenshot;

	public PsUIGenericButton m_button;

	public UIRectSpriteButton m_bannerEndIcon;

	protected string m_bannerIconName;

	protected string m_gameModeIconName;

	public PsGameLoop m_loop;

	public PsUnlockable m_signal;

	public PsUICreateLevelBanner(UIComponent _parent, PsGameLoop _gameLoop, PsUnlockable _signal = null)
		: base(_parent, "levelBanner")
	{
		m_loop = _gameLoop;
		m_signal = _signal;
		Initialize();
		CreateUI();
	}

	public virtual void CreateUI()
	{
		SetHeight(0.2f, RelativeTo.ScreenHeight);
		SetSpacing(-0.0125f, RelativeTo.ScreenHeight);
		RemoveDrawHandler();
		CreateScreenshot(this);
		UIVerticalList uIVerticalList = new UIVerticalList(this, string.Empty);
		uIVerticalList.SetSpacing(-0.0125f, RelativeTo.ScreenHeight);
		uIVerticalList.RemoveDrawHandler();
		UIFittedSprite uIFittedSprite = new UIFittedSprite(uIVerticalList, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_gamebanner_main_top"));
		uIFittedSprite.SetHeight(0.07f, RelativeTo.ScreenHeight);
		uIFittedSprite.SetMargins(0f, -0.05f, 0.0125f, 0.0125f);
		uIFittedSprite.SetDepthOffset(10f);
		CreateBannerEndIcon(uIFittedSprite);
		string empty = string.Empty;
		UIFittedText uIFittedText = new UIFittedText(_text: (m_loop == null) ? m_signal.m_name : m_loop.GetName(), _parent: uIFittedSprite, _touchable: false, _tag: string.Empty, _fontResourcePath: "KGSecondChances_Font", _fitTextToWidth: true, _color: "#FFFFFF", _shadowColor: "#000000");
		uIFittedText.SetMargins(0.05f, 0.05f, 0f, 0f);
		uIFittedText.SetHorizontalAlign(0f);
		UISprite uISprite = new UISprite(uIVerticalList, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_gamebanner_main_bottom"));
		uISprite.SetHeight(0.1f, RelativeTo.ScreenHeight);
		uISprite.SetWidth(0.625f, RelativeTo.ScreenHeight);
		uISprite.SetMargins(0.05f, 0.05f, 0.0125f, -0.025f);
		uISprite.SetHorizontalAlign(0f);
		uISprite.SetDepthOffset(20f);
		CreateButton(uISprite);
	}

	public virtual void CreateScreenshot(UIComponent _parent)
	{
		m_screenshot = new PsUIScreenshot(_parent, false, string.Empty, Vector2.zero, m_loop, false, true, 0.02f, true);
		m_screenshot.SetSize(0.28f, 0.18f, RelativeTo.ScreenHeight);
		m_screenshot.SetMargins(-0.0125f, -0.0125f, -0.025f, -0.025f);
		m_screenshot.SetDrawHandler(PsUIScreenshot.AltDrawHandler);
		CreateGameMode(m_screenshot);
	}

	public virtual void CreateGameMode(UIComponent _parent)
	{
		UIFittedSprite uIFittedSprite = new UIFittedSprite(_parent, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(m_gameModeIconName));
		uIFittedSprite.SetHeight(0.085f, RelativeTo.ScreenHeight);
		uIFittedSprite.SetAlign(0f, 1f);
	}

	public virtual void CreateBannerEndIcon(UIComponent _parent)
	{
		m_bannerEndIcon = new UIRectSpriteButton(_parent, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(m_bannerIconName));
		m_bannerEndIcon.SetHeight(0.085f, RelativeTo.ScreenHeight);
		m_bannerEndIcon.SetMargins(-0.01f, -0.01f, -0.01f, -0.01f);
		m_bannerEndIcon.SetAlign(1f, 0.5f);
		m_bannerEndIcon.SetDepthOffset(-20f);
	}

	public virtual void CreateButton(UIComponent _parent)
	{
		m_button = new PsUIGenericButton(_parent);
		m_button.SetText("Play");
		m_button.SetHorizontalAlign(1f);
		m_button.SetDepthOffset(-20f);
	}

	public virtual void Initialize()
	{
		m_bannerIconName = "menu_difficulty_";
		m_gameModeIconName = "item_mode_";
		switch (m_loop.GetDifficulty())
		{
		case PsGameDifficulty.Easy:
		case PsGameDifficulty.New:
			m_bannerIconName += "casual";
			break;
		case PsGameDifficulty.Tricky:
			m_bannerIconName += "tricky";
			break;
		case PsGameDifficulty.Insane:
			m_bannerIconName += "hardcore";
			break;
		case PsGameDifficulty.Impossible:
			m_bannerIconName += "impossible";
			break;
		}
		switch (m_loop.GetGameMode())
		{
		case PsGameMode.StarCollect:
			m_gameModeIconName += "3stars";
			break;
		case PsGameMode.Race:
			m_gameModeIconName += "timeattack";
			break;
		}
	}
}
