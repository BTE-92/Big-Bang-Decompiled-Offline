using System;
using Server;
using UnityEngine;

public class EditorSaveState : BasicState
{
	private UICanvas m_savePopupContainer;

	private string m_creationTime;

	private bool m_saveLocally;

	public EditorSaveState(bool _saveLocally = false)
	{
		m_saveLocally = _saveLocally;
	}

	public override void Enter(IStatedObject _parent)
	{
		m_savePopupContainer = new UICanvas(null, true, "SavePopupContainer", null, string.Empty);
		m_savePopupContainer.SetWidth(1f, RelativeTo.ScreenWidth);
		m_savePopupContainer.SetHeight(1f, RelativeTo.ScreenHeight);
		m_savePopupContainer.SetMargins(0.125f, 0.125f, 0f, 0f, RelativeTo.ScreenWidth);
		m_savePopupContainer.SetDrawHandler(UIDrawHandlers.EditorPopupBackground);
		UIVerticalList uIVerticalList = new UIVerticalList(m_savePopupContainer, "SavePopup");
		uIVerticalList.SetHeight(0.764f, RelativeTo.ParentHeight);
		uIVerticalList.RemoveDrawHandler();
		new UIPopupHeader(uIVerticalList, "SavePopupHeader", "saving...", "Sending to server, please wait.");
		m_savePopupContainer.Update();
		m_creationTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
		SendLevelToServer();
	}

	private HttpC SendLevelToServer()
	{
		if (PsState.m_activeGameLoop.m_minigameMetaData.published)
		{
			PsState.m_activeGameLoop.m_minigameMetaData.id = string.Empty;
		}
		PsState.m_activeMinigame.SetLayerItems();
		if (PsState.m_activeGameLoop.m_minigameMetaData.template)
		{
			return Template.Save(PsState.m_activeGameLoop.m_minigameMetaData.id, LevelSendToServerOk, LevelSendToServerFailed);
		}
		return MiniGame.Save(PsState.m_activeGameLoop, null, LevelSendToServerOk, LevelSendToServerFailed, false, m_creationTime);
	}

	private void LevelSendToServerOk(PsMinigameMetaData _metaData)
	{
		PsState.m_activeGameLoop.m_minigameMetaData = _metaData;
		PsState.m_activeGameLoop.m_minigameId = _metaData.id;
		PsMetrics.LevelSaved();
		Debug.Log("MINIGAME SAVED");
		Debug.Log("TIME EDITED: " + _metaData.timeSpentEditing);
		PsMetricsData.m_timeInEditor += (int)PsState.m_activeGameLoop.ElapsedTime();
		PsCaches.m_savedList.Clear();
		if (m_saveLocally)
		{
			LevelManager.SaveCurrentLevelLocally(_metaData.id);
		}
		if (PsState.m_activeGameLoop is PsGameLoopEditorTemplate)
		{
			(PsState.m_activeGameLoop as PsGameLoopEditorTemplate).ExitAfterEveryplay();
		}
		else
		{
			Main.m_currentGame.m_sceneManager.ChangeScene(new PsMenuScene("MenuScene"), new FadeLoadingScene(Color.black));
		}
	}

	private void LevelSendToServerFailed(HttpC _c)
	{
		Debug.LogError("MINIGAME SAVE FAILED");
		string networkError = ServerErrors.GetNetworkError(_c.www.error);
		ServerManager.ThrowServerErrorException("Connection error", networkError, SendLevelToServer);
	}

	public override void Execute()
	{
	}

	public override void Exit()
	{
		if (m_savePopupContainer != null)
		{
			m_savePopupContainer.Destroy();
			m_savePopupContainer = null;
		}
	}
}
