using System;

public class PsGameModeStarCollectEditor : PsGameModeStarCollect
{
	protected PsGameModeStarCollectEditor(PsGameMode _gameMode, PsGameLoop _info)
		: base(_gameMode, _info)
	{
	}

	public PsGameModeStarCollectEditor(PsGameLoop _info)
		: base(PsGameMode.StarCollect, _info)
	{
	}

	public override void CreatePlayMenu(Action _restartAction, Action _pauseAction)
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_playMenu = new PsUIEditorStarCollectPlay(_restartAction, _pauseAction);
		PsIngameMenu.OpenController();
	}

	public override void CreateEditMenu(Action _exitAction, Action _testAction)
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_playMenu = new PsUIEditorEdit(_exitAction, _testAction);
	}

	public override void GiveRewards()
	{
	}
}
