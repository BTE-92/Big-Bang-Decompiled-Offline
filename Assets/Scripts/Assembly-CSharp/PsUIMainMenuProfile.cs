public class PsUIMainMenuProfile : UIVerticalList
{
	private PsUIProfileImage m_image;

	private UIText m_name;

	private PsUIGenericButton m_settingsButton;

	public PsUIMainMenuProfile(UIComponent _parent)
		: base(_parent, "MainMenuProfile")
	{
		SetAlign(0f, 1f);
		SetSpacing(0.005f, RelativeTo.ScreenHeight);
		RemoveDrawHandler();
		m_name = new UIText(this, false, string.Empty, PlayerPrefsX.GetUserName(), "KGSecondChances_Font", 0.03f, RelativeTo.ScreenShortest, "#ffffff");
		m_name.SetHorizontalAlign(0f);
		m_settingsButton = new PsUIGenericButton(this);
		m_settingsButton.SetIcon("menu_icon_settings_small", 0.08f);
		m_settingsButton.SetMargins(0.01f, 0.02f, 0.012f, 0.012f);
		m_settingsButton.SetHorizontalAlign(0f);
		m_image = new PsUIProfileImage(m_settingsButton, false, string.Empty, PlayerPrefsX.GetFacebookId(), PlayerPrefsX.GetGameCenterId(), PsMetagameManager.m_playerStats.rank);
		m_image.SetSize(0.07f, 0.07f, RelativeTo.ScreenShortest);
		m_image.SetHorizontalAlign(0f);
	}

	public override void Step()
	{
		if (m_settingsButton.m_hit)
		{
			PsPlanetManager.GetCurrentPlanet().FastForward();
			PsUIBaseState psUIBaseState = new PsUIBaseState(typeof(PsUICenterSettings));
			psUIBaseState.SetAction("Exit", ExitPlayerProfiler);
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(psUIBaseState);
			PsMenuScene.CreateBlur();
		}
		base.Step();
	}

	private void ExitPlayerProfiler()
	{
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new PsMainMenuState());
	}
}
