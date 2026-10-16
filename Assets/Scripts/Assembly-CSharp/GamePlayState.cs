using UnityEngine;

public class GamePlayState : BasicState
{
	public override void Enter(IStatedObject _parent)
	{
		NotificationManager.Pause();
	}

	public override void Execute()
	{
		if (!PsState.m_physicsPaused && PsState.m_activeMinigame.m_gameStarted && !PsState.m_activeMinigame.m_gameEnded && !PsState.m_activeMinigame.m_gameTicksFreezed)
		{
			PsState.m_activeMinigame.m_gameTicks = PsState.m_activeMinigame.m_gameTicks + 1f * Main.m_timeScale;
			PsState.m_activeMinigame.m_realTimeSpent = PsState.m_activeMinigame.m_realTimeSpent + Time.deltaTime * Main.m_timeScale;
			PsState.m_debugCoinsTime += Main.m_gameDeltaTime;
		}
	}

	public void ContinueToBeamOut()
	{
		Debug.Log("CONTINUE TO BEAM OUT");
	}

	public void ResumeGame()
	{
		Debug.Log("RESUME GAME");
	}

	public override void Exit()
	{
	}
}
