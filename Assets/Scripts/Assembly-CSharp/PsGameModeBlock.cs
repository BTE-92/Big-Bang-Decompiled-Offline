using System;

public class PsGameModeBlock : PsGameModeBase
{
	public PsGameModeBlock(PsGameLoop _info)
		: base(PsGameMode.Race, _info)
	{
	}

	public override void CreateMusic()
	{
		Minigame minigame = LevelManager.m_currentLevel as Minigame;
		minigame.m_music = SoundS.AddComponent(minigame.m_environmentTC, "/Music/RacingMusic2", 1f, true);
		SoundS.PlaySound(minigame.m_music, true);
		SoundS.SetSoundParameter(minigame.m_music, "MusicState", 0f);
	}

	public override void CreatePlayMenu(Action _restartAction, Action _pauseAction)
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.OpenController();
	}
}
