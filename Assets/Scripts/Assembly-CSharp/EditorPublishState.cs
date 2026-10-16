using System;
using System.Collections.Generic;
using MiniJSON;
using Server;
using UnityEngine;

public class EditorPublishState : BasicState
{
	private UICanvas m_publishPopupContainer;

	private string m_creationTime;

	public override void Enter(IStatedObject _parent)
	{
		m_publishPopupContainer = new UICanvas(null, true, "PublishPopupContainer", null, string.Empty);
		m_publishPopupContainer.SetWidth(1f, RelativeTo.ScreenWidth);
		m_publishPopupContainer.SetHeight(1f, RelativeTo.ScreenHeight);
		m_publishPopupContainer.SetMargins(0.125f, 0.125f, 0f, 0f, RelativeTo.ScreenWidth);
		m_publishPopupContainer.SetDrawHandler(UIDrawHandlers.EditorPopupBackground);
		UIVerticalList uIVerticalList = new UIVerticalList(m_publishPopupContainer, "PublishPopup");
		uIVerticalList.SetHeight(0.764f, RelativeTo.ParentHeight);
		uIVerticalList.RemoveDrawHandler();
		new UIPopupHeader(uIVerticalList, "SavePopupHeader", "publishing...", "Sending to server, please wait.");
		m_publishPopupContainer.Update();
		m_creationTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
		PublishLevel();
	}

	private DataBlob GetGhostData()
	{
		PsState.m_activeMinigame.m_lastSentTimeScore = HighScores.TicksToTimeScore(Mathf.RoundToInt(PsState.m_activeMinigame.m_gameTicks), true);
		PsState.m_activeMinigame.m_lastSentScore = 3;
		DataBlob result = default(DataBlob);
		Debug.LogWarning("GHOST " + PsState.m_activeGameLoop.m_gameMode.m_recordingGhost.m_recording);
		if (PsState.m_activeGameLoop.m_gameMode.m_recordingGhost != null && !PsState.m_activeGameLoop.m_gameMode.m_recordingGhost.m_recording)
		{
			PsState.m_activeGameLoop.m_minigameMetaData.timeScore = PsState.m_activeMinigame.m_lastSentTimeScore;
			PsState.m_activeGameLoop.m_minigameMetaData.score = PsState.m_activeMinigame.m_lastSentScore;
			result = ClientTools.CreateGhostDataBlob(PsState.m_activeGameLoop.m_gameMode.m_recordingGhost);
			PsMetricsData.m_lastGhostSize = result.data.Length;
		}
		else
		{
			PsMetricsData.m_lastGhostSize = 0;
		}
		return result;
	}

	private HttpC PublishLevel()
	{
		DataBlob ghostData = GetGhostData();
		PsState.m_activeMinigame.SetLayerItems();
		byte[] screenshotData = null;
		Screenshot selectedScreenshot = EditorScene.GetSelectedScreenshot();
		if (selectedScreenshot != null)
		{
			screenshotData = selectedScreenshot.GetScreenshotJPGBytes();
		}
		else
		{
			Debug.LogError("NO SCREENSHOT! -- DAS IST PROBLEM");
		}
		if (PsState.m_activeGameLoop.m_minigameMetaData.published)
		{
			PsState.m_activeGameLoop.m_minigameMetaData.id = string.Empty;
		}
		string pathData = string.Empty;
		if (PsState.m_activeGameLoop is PsGameLoopEditorTemplate)
		{
			(PsState.m_activeGameLoop as PsGameLoopEditorTemplate).m_removedFromPath = true;
			if (PsState.m_activeGameLoop.m_path != null)
			{
				PsState.m_activeGameLoop.m_path.m_currentNodeId++;
				pathData = Json.Serialize(ClientTools.GenerateProgressionPathJson(PsState.m_activeGameLoop.m_path));
			}
		}
		bool hidden = PsState.m_activeGameLoop.m_minigameMetaData.hidden;
		PsAchievementManager.Complete("publishLevel");
		if (PsState.m_activeGameLoop.ElapsedTime() > 10800)
		{
			PsAchievementManager.Complete("longTimeInEditor");
		}
		return MiniGame.Publish(PsState.m_activeGameLoop, screenshotData, pathData, ghostData, PublishToServerOk, PublishToServerFailed, hidden, m_creationTime);
	}

	private void PublishToServerOk(PsMinigameMetaData _metaData)
	{
		Minigame minigame = LevelManager.m_currentLevel as Minigame;
		PsState.m_activeGameLoop.m_minigameMetaData = _metaData;
		PsState.m_activeGameLoop.m_minigameId = _metaData.id;
		PsMetrics.LevelPublished();
		PsMetricsData.m_timeInEditor += minigame.GetTimeSinceInit();
		Debug.Log("MINIGAME SAVED");
		PsCaches.m_savedList.Clear();
		PsCaches.m_publishedList.Clear();
		PsMetrics.GhostSaved("Editor", PsMetricsData.m_lastGhostSize.ToString(), PsState.m_activeGameLoop.m_minigameMetaData.id, true);
		PsState.m_activeGameLoop.ExitEditor(true);
	}

	private void PublishToServerFailed(HttpC _c)
	{
		Debug.LogError("MINIGAME SAVE FAILED");
		string networkError = ServerErrors.GetNetworkError(_c.www.error);
		ServerManager.ThrowServerErrorException("Connection error", networkError, PublishLevel);
	}

	public override void Execute()
	{
	}

	public override void Exit()
	{
		if (m_publishPopupContainer != null)
		{
			m_publishPopupContainer.Destroy();
			m_publishPopupContainer = null;
		}
	}

	public int GetUpgradeSum()
	{
		int num = 0;
		List<KeyValuePair<string, int>> upgrades = (PsState.m_activeMinigame.m_playerUnit as Vehicle).GetUpgrades();
		foreach (KeyValuePair<string, int> item in upgrades)
		{
			KeyValuePair<string, int> keyValuePair = item;
			if (keyValuePair.Key != "tier")
			{
				num += item.Value;
			}
		}
		return num;
	}
}
