public class PsUICenterGameCardFresh : PsUICenterGameCard
{
	public PsUICenterGameCardFresh(UIComponent _parent)
		: base(_parent)
	{
	}

	public override void CreateHeaderText(UICanvas _parent)
	{
		string text = "Fresh Level!";
		m_contextText = new UIFittedText(_parent, false, string.Empty, text, "KGSecondChances_Font", true, "#ffffff", "#5c3a0a");
		m_contextText.SetHorizontalAlign(0f);
	}

	public override void CreateHeaderScore(UICanvas _parent)
	{
		int num = 0;
		if (m_metadataLoaded)
		{
			num = PsState.m_activeGameLoop.m_scoreBest;
		}
		string frameName = "menu_mode_key_" + num;
		m_scoreIcon = new UIFittedSprite(_parent, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(frameName));
		m_scoreIcon.SetAlign(1f, 1f);
	}

	public override void SetMetadataInfo()
	{
		m_animation.Destroy();
		m_animation = null;
		string frameName = "item_player_monster_car_icon";
		if (PsState.m_activeGameLoop.GetPlayerUnit() == "Motorcycle")
		{
			frameName = "item_player_motorcycle_icon";
		}
		m_unitIcon.SetFrame(PsState.m_uiSheet.m_atlas.GetFrame(frameName));
		m_unitIcon.SetOverrideShader(null);
		m_scoreIcon.SetFrame(PsState.m_uiSheet.m_atlas.GetFrame("menu_mode_key_" + PsState.m_activeGameLoop.m_scoreBest));
		m_screenshot.m_gameId = PsState.m_activeGameLoop.GetGameId();
		m_screenshot.LoadPicture();
		m_profileIcon.m_facebookId = PsState.m_activeGameLoop.GetFacebookId();
		m_profileIcon.LoadPicture();
		CreateLevelAndCreatorNames(m_nameList);
		m_description.SetText(PsState.m_activeGameLoop.GetDescription());
		m_playButton.SetGreenColors();
		m_playButton.EnableTouchAreas();
		m_metadataLoaded = true;
		(GetRoot() as PsUIBasePopup).Update();
	}
}
