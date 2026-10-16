using System;
using System.Collections.Generic;
using Server;
using UnityEngine;

public class EditorPublishHiddenState : BasicState
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
		SendLevelToServer();
	}

	private HttpC SendLevelToServer()
	{
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
		PsState.m_activeGameLoop.m_minigameMetaData.id = string.Empty;
		HttpC httpC = MiniGame.SaveHidden(PsState.m_activeGameLoop, screenshotData, LevelSendToServerOk, LevelSendToServerFailed, m_creationTime);
		httpC.objectData = PsState.m_activeGameLoop;
		return httpC;
	}

	private void LevelSendToServerOk(PsMinigameMetaData _metaData)
	{
		Minigame minigame = LevelManager.m_currentLevel as Minigame;
		PsState.m_activeGameLoop.m_minigameMetaData = _metaData;
		PsState.m_activeGameLoop.m_minigameId = _metaData.id;
		PsMetrics.LevelPublished();
		PsMetricsData.m_timeInEditor += minigame.GetTimeSinceInit();
		Debug.Log("MINIGAME SAVED");
		SendHighscoreAndGhost();
	}

	private void LevelSendToServerFailed(HttpC _c)
	{
		Debug.LogError("MINIGAME SAVE FAILED");
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), SendLevelToServer);
	}

	private HttpC SendHighscoreAndGhost()
	{
		PsState.m_activeMinigame.m_lastSentTimeScore = HighScores.TicksToTimeScore(Mathf.RoundToInt(PsState.m_activeMinigame.m_gameTicks), true);
		PsState.m_activeMinigame.m_lastSentScore = 3;
		DataBlob ghostData = default(DataBlob);
		if (PsState.m_activeGameLoop.m_gameMode.m_recordingGhost != null && !PsState.m_activeGameLoop.m_gameMode.m_recordingGhost.m_recording)
		{
			PsState.m_activeGameLoop.m_minigameMetaData.timeScore = PsState.m_activeMinigame.m_lastSentTimeScore;
			PsState.m_activeGameLoop.m_minigameMetaData.score = PsState.m_activeMinigame.m_lastSentScore;
			ghostData = ClientTools.CreateGhostDataBlob(PsState.m_activeGameLoop.m_gameMode.m_recordingGhost);
		}
		PsMetricsData.m_lastGhostSize = ghostData.data.Length;
		return SendHighscoreAndGhostSafe(new HighScore.HighScoreAndGhostData(PsState.m_activeGameLoop.m_context, PsState.m_activeGameLoop.ElapsedTime(), ghostData, ghostData.data.Length, PsState.m_activeGameLoop.m_minigameMetaData, PsState.m_activeMinigame.m_lastSentTimeScore, PsState.m_activeMinigame.m_lastSentScore, false, PsState.m_activeMinigame.m_playerUnitName, false, GetUpgradeSum(), PsState.m_activeMinigame.m_gameDeathCount));
	}

	private HttpC SendHighscoreAndGhostSafe(HighScore.HighScoreAndGhostData _data)
	{
		PsMetricsData.m_lastGhostSize = _data.ghostSize;
		return HighScore.SendHighscoreAndGhostSafe(_data, HighscoreServerRequestOK, delegate(HttpC c)
		{
			HighscoreServerRequestFAILED(c, _data);
		});
	}

	private void HighscoreServerRequestFAILED(HttpC _c, HighScore.HighScoreAndGhostData _data)
	{
		string networkError = ServerErrors.GetNetworkError(_c.www.error);
		ServerManager.ThrowServerErrorException("Connection error", networkError, () => SendHighscoreAndGhostSafe(_data));
	}

	private void HighscoreServerRequestOK(ScoreTable _data)
	{
		Minigame minigame = LevelManager.m_currentLevel as Minigame;
		PsCaches.m_savedList.Clear();
		PsCaches.m_publishedList.Clear();
		PsMetrics.GhostSaved("Editor", PsMetricsData.m_lastGhostSize.ToString(), PsState.m_activeGameLoop.m_minigameMetaData.id, true);
		PsState.m_activeGameLoop.ExitEditor(true);
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
