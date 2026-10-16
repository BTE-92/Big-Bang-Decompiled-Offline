using System;

public class PsUIEditorTutorialPlay : PsUITopPlayStarCollect, IPlayMenu
{
	public PsUIEditorTutorialPlay(Action _restartAction = null, Action _pauseAction = null)
		: base(_restartAction, _pauseAction)
	{
	}

	public override void CreateCoinArea()
	{
	}

	public override void CreateRestartArea(UIComponent _parent)
	{
		m_restartButton = new PsUIGenericButton(_parent);
		m_restartButton.SetIcon("hud_icon_restart", 0.06f);
		m_restartButton.SetOrangeColors();
		m_pauseButton = new PsUIGenericButton(_parent);
		m_pauseButton.SetText("EDIT", 0.05f);
		m_pauseButton.SetOrangeColors();
	}

	public override void ApplyLeftySettings()
	{
	}
}
