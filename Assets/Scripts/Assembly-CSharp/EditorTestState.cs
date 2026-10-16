using UnityEngine;

public class EditorTestState : BasicState
{
	public override void Enter(IStatedObject _parent)
	{
		EditorScene.m_cameraManIdleTimer = Random.Range(3, 6);
		EditorScene.ClearScreenshots();
		AutoGeometryManager.SetDirtyTileTracking(true);
	}

	public override void Execute()
	{
		if (!PsState.m_physicsPaused && PsState.m_activeMinigame.m_gameStarted && !PsState.m_activeMinigame.m_gameEnded && !PsState.m_activeMinigame.m_gameTicksFreezed)
		{
			PsState.m_activeMinigame.m_gameTicks = PsState.m_activeMinigame.m_gameTicks + 1f * Main.m_timeScale;
			if (PsState.m_cameraManTakeShot && EditorScene.m_cameraManShotDelay <= 0)
			{
				EditorScene.TakeScreenshot();
				PsState.m_cameraManTakeShot = false;
				EditorScene.m_cameraManShotDelay = Random.Range(120, 240);
			}
			if (EditorScene.m_cameraManShotDelay > 0)
			{
				EditorScene.m_cameraManShotDelay--;
			}
			else
			{
				EditorScene.m_cameraManIdleTimer--;
			}
			if (EditorScene.m_cameraManIdleTimer <= 0)
			{
				EditorScene.m_cameraManIdleTimer = Random.Range(3, 6);
				PsState.m_cameraManTakeShot = true;
			}
		}
	}

	public override void Exit()
	{
	}
}
