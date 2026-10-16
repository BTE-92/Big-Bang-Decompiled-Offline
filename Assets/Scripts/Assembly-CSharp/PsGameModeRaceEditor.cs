using System;

public class PsGameModeRaceEditor : PsGameModeRace
{
	protected PsGameModeRaceEditor(PsGameMode _gameMode, PsGameLoop _info)
		: base(_info)
	{
	}

	public PsGameModeRaceEditor(PsGameLoop _info)
		: base(_info)
	{
	}

	public override void GiveRewards()
	{
	}

	public override void CreatePlayMenu(Action _restartAction, Action _pauseAction)
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_playMenu = new PsUIEditorRacePlay(_restartAction, _pauseAction);
		PsIngameMenu.OpenController();
	}

	public override void CreateEditMenu(Action _exitAction, Action _testAction)
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_playMenu = new PsUIEditorEdit(_exitAction, _testAction);
	}
}
