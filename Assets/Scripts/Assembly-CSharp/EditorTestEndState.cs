using System;

public class EditorTestEndState : BasicState
{
	public UIVerticalList m_varea;

	public UIHorizontalList m_area;

	public PsUIGenericButton m_publishButton;

	public UIText m_timerText;

	public UIRectSpriteButton m_restartButton;

	public UIRectSpriteButton m_editButton;

	private double m_lastPublish;

	private int m_timeLeft;

	private double m_publishCooldown;

	private UICanvas m_endCanvas;

	public EditorTestEndState()
	{
		if (PsMetagamePlayerData.m_playerData.ContainsKey("lastPublish"))
		{
			m_lastPublish = double.Parse((string)PsMetagamePlayerData.m_playerData["lastPublish"]);
		}
		else
		{
			m_lastPublish = 0.0;
		}
		m_publishCooldown = 1800.0;
		m_timeLeft = (int)Math.Ceiling(m_lastPublish + m_publishCooldown - Main.m_EPOCHSeconds);
		string timeStringFromSeconds = PsMetagameManager.GetTimeStringFromSeconds(m_timeLeft);
	}

	public override void Enter(IStatedObject _parent)
	{
	}

	public static void Edit()
	{
		PsState.m_activeGameLoop.ExitMinigame();
	}

	public static void Save()
	{
		EditorBaseState.RemoveTransformGizmo();
		UndoManager.Purge();
		Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorSavePopupState());
		EntityManager.RemoveEntitiesByTag("GTAG_AUTODESTROY");
		(LevelManager.m_currentLevel as Minigame).m_groundNode.RevertGroundFromPlay();
		LevelManager.ResetCurrentLevel();
	}

	public static void Publish()
	{
		EditorBaseState.RemoveTransformGizmo();
		UndoManager.Purge();
		PsState.m_activeGameLoop.m_minigameMetaData.creatorUpgrades = (PsState.m_activeMinigame.m_playerUnit as Vehicle).GetUpgradeValues();
		if (PsState.m_adminMode)
		{
			Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorPublishHiddenPopupState());
		}
		else
		{
			Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorPublishPopupState());
		}
		EntityManager.RemoveEntitiesByTag("GTAG_AUTODESTROY");
		(LevelManager.m_currentLevel as Minigame).m_groundNode.RevertGroundFromPlay();
		LevelManager.ResetCurrentLevel();
	}

	public override void Exit()
	{
		m_endCanvas.Destroy();
	}
}
