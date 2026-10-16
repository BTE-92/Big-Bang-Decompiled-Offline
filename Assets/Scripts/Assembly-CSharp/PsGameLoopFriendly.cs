using System;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using Server;
using UnityEngine;

public class PsGameLoopFriendly : PsGameLoopVersus
{
	private PsUIWaitPopup m_waitPopup;

	private PsUIBasePopup m_claimPopup;

	private bool m_claimed;

	private bool m_claimPressed;

	private string m_opponentName;

	public PsGameLoopFriendly(PsMinigameContext _context, VersusMetaData _versusData, string _timedEventId, string _minigameId, PsPlanetPath _path, int _id, int _levelNumber, int _score, int _timeleft, bool _removedFromPath, int _duration = -1, string _endTime = null, bool _eventOver = false, bool _domeDestroyed = false)
		: base(_context, _versusData, _timedEventId, _minigameId, _path, _id, _levelNumber, _score, _timeleft, true, _duration, _endTime, _eventOver, _domeDestroyed)
	{
	}

	public override void SetUp()
	{
		if (m_versusData != null)
		{
			m_minigameMetaData = m_versusData.gameMetaData;
			m_gameMode = new PsGameModeRaceFriendly(this);
			m_tryCount = ((m_versusData.GetOwnInfo() != null) ? m_versusData.GetOwnInfo().tries : 0);
		}
		m_setTriesRetryCount = 0;
		m_nextRound = false;
	}

	public override void StartLoop()
	{
		m_versusWon = false;
		if (PsState.m_activeGameLoop != null || (m_path != null && m_nodeId > m_path.m_currentNodeId && m_path.m_name != "FloatingPath"))
		{
			return;
		}
		if (!m_eventOver)
		{
			if (m_versusData != null)
			{
				if (!string.IsNullOrEmpty(m_versusData.winner))
				{
					bool playerWon = m_versusData.winner == PlayerPrefsX.GetUserId();
					ClaimVersus(playerWon);
					return;
				}
				if (string.IsNullOrEmpty(m_versusData.currentPlayer) && m_versusData.participants.Length < 2)
				{
					if (m_versusData.challengedPlayer == PlayerPrefsX.GetUserId())
					{
						if (m_versusData.status == "declined")
						{
							ClaimAndSave();
						}
						else
						{
							ShowNewFriendRacePopup();
						}
					}
					else if (m_versusData.status == "declined")
					{
						Debug.LogError("Friend declined");
						ClaimAndSave();
					}
					else
					{
						Debug.LogError("Waiting friend to respond");
					}
					return;
				}
				if (m_versusData.currentPlayer != PlayerPrefsX.GetUserId() || PsState.m_versusChallengeTryAmount <= m_versusData.GetOwnInfo().tries)
				{
					Debug.LogWarning("Opponents turn");
					return;
				}
			}
			StartFriendlyLoop();
		}
		else
		{
			Debug.LogWarning("Event was already over");
		}
	}

	public override void ClaimVersus(bool _playerWon)
	{
		Debug.LogWarning("Winner: " + ((!_playerWon) ? "Opponent" : "You!"));
		SetAsActiveLoop();
		m_claimed = false;
		m_removedFromPath = true;
		m_path.m_nodeInfos.Remove(this);
		string text = Json.Serialize(ClientTools.GenerateProgressionFloatingPathJson(m_path));
		HttpC httpC = Friendly.ClaimAndSave(m_timedEventId, text, ClaimingNodeSucceed, ClaimingNodeFailed);
		httpC.objectData = text;
		m_claimPopup = new PsUIBasePopup(typeof(PsUICenterClaimFriendRace));
		m_claimPopup.SetAction("Exit", delegate
		{
			SetActiveLoopNull();
			m_claimPopup.Destroy();
			m_claimPopup = null;
			if (!m_claimed)
			{
				m_waitPopup = new PsUIWaitPopup();
			}
			else
			{
				DestroyNode();
			}
		});
	}

	public void ClaimingNodeSucceed(HttpC _c)
	{
		m_claimed = true;
		if (m_waitPopup != null)
		{
			m_waitPopup.Destroy();
			m_waitPopup = null;
			DestroyNode();
		}
	}

	public void ClaimingNodeFailed(HttpC _c)
	{
		ServerManager.ThrowServerErrorException("Connection error", _c.www, delegate
		{
			HttpC httpC = Friendly.ClaimAndSave(m_timedEventId, (string)_c.objectData, ClaimingNodeSucceed, ClaimingNodeFailed);
			httpC.objectData = _c.objectData;
			return httpC;
		});
	}

	public void ClaimAndSave()
	{
		m_removedFromPath = true;
		m_path.m_nodeInfos.Remove(this);
		DestroyNode();
		string text = Json.Serialize(ClientTools.GenerateProgressionFloatingPathJson(m_path));
		HttpC httpC = Friendly.ClaimAndSave(m_timedEventId, text, ClaimingNodeSucceed, ClaimingNodeFailed);
		httpC.objectData = text;
	}

	public void ClaimAndSaveSucceed(HttpC _c)
	{
		Debug.Log("Friend race claimed (opponent declined)");
	}

	public void ClaimAndSaveFailed(HttpC _c)
	{
		ServerManager.ThrowServerErrorException("Connection error", _c.www, delegate
		{
			HttpC httpC = Friendly.ClaimAndSave(m_timedEventId, (string)_c.objectData, ClaimAndSaveSucceed, ClaimAndSaveFailed);
			httpC.objectData = _c.objectData;
			return httpC;
		});
	}

	public new void DestroyNode()
	{
		if (!m_domeDestroyed)
		{
			m_domeDestroyed = true;
			if (m_node != null)
			{
				(m_node as PsFloatingPlanetNode).DestroyNode();
			}
		}
	}

	public void StartFriendlyLoop()
	{
		SetAsActiveLoop();
		StartDialogueFlow();
	}

	public void StartDialogueFlow()
	{
		new PsMenuDialogueFlow(this, PsNodeEventTrigger.LoopStart, 0f, delegate
		{
			LoadMinigame();
		});
	}

	protected void ShowNewFriendRacePopup()
	{
		SetAsActiveLoop();
		PsUIBasePopup popup = new PsUIBasePopup(typeof(PsUICenterNewFriendRace));
		popup.SetAction("Exit", delegate
		{
			popup.Destroy();
			SetActiveLoopNull();
		});
		popup.SetAction("Accept", delegate
		{
			popup.Destroy();
			JoinGame();
		});
		popup.SetAction("Decline", delegate
		{
			popup.Destroy();
			DeclineGame();
			SetActiveLoopNull();
		});
	}

	protected void JoinGame()
	{
		m_waitPopup = new PsUIWaitPopup();
		PsAchievementManager.Complete("challengeAccepted");
		Friendly.Join(m_versusData.timedEventId, JoinOk, JoinFailed);
	}

	private void JoinOk(VersusMetaData _versusData)
	{
		DestroyWaitPopup();
		m_versusData = _versusData;
		StartDialogueFlow();
	}

	private void JoinFailed(HttpC _c)
	{
		DestroyWaitPopup();
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), delegate
		{
			HttpC httpC = Friendly.Join((string)_c.objectData, JoinOk, JoinFailed);
			httpC.objectData = _c.objectData;
			return httpC;
		});
	}

	protected void DeclineGame()
	{
		m_waitPopup = new PsUIWaitPopup();
		Friendly.Decline(m_versusData.timedEventId, DeclineOk, DeclineFailed);
	}

	private void DeclineOk(VersusMetaData _versusData)
	{
		DestroyWaitPopup();
		Debug.LogWarning("Declined successfully, refreshing");
	}

	private void DeclineFailed(HttpC _c)
	{
		DestroyWaitPopup();
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), delegate
		{
			HttpC httpC = Friendly.Decline((string)_c.objectData, DeclineOk, DeclineFailed);
			httpC.objectData = _c.objectData;
			return httpC;
		});
	}

	private void DestroyWaitPopup()
	{
		if (m_waitPopup != null)
		{
			m_waitPopup.Destroy();
			m_waitPopup = null;
		}
	}

	private string GetOpponentName()
	{
		string result = null;
		if (m_versusData.GetOpponentInfo() != null)
		{
			return m_versusData.GetOpponentInfo().score.name;
		}
		if (m_versusData.challengedPlayer != PlayerPrefsX.GetUserId() && PsMetagameManager.m_friends != null && PsMetagameManager.m_friends.friends != null)
		{
			for (int i = 0; i < PsMetagameManager.m_friends.friends.Count; i++)
			{
				if (m_versusData.challengedPlayer == PsMetagameManager.m_friends.friends[i].playerId)
				{
					result = PsMetagameManager.m_friends.friends[i].name;
					break;
				}
			}
		}
		return result;
	}

	public override string GetBannerString()
	{
		PsFloatingVersusNode psFloatingVersusNode = m_node as PsFloatingVersusNode;
		if (m_versusData == null)
		{
			psFloatingVersusNode.AddGreyScale();
			return "Loading...";
		}
		string text = "friend";
		if (!string.IsNullOrEmpty(m_opponentName))
		{
			text = m_opponentName;
		}
		else
		{
			string opponentName = GetOpponentName();
			if (!string.IsNullOrEmpty(opponentName))
			{
				text = (m_opponentName = opponentName);
			}
		}
		if (!string.IsNullOrEmpty(m_versusData.winner))
		{
			psFloatingVersusNode.RemoveGreyScale();
			if (m_versusData.winner == PlayerPrefsX.GetUserId())
			{
				return "You won!";
			}
			return "You lost";
		}
		if (string.IsNullOrEmpty(m_versusData.currentPlayer) && m_versusData.participants.Length < 2)
		{
			if (m_versusData.challengedPlayer == PlayerPrefsX.GetUserId())
			{
				if (m_versusData.status == "declined")
				{
					psFloatingVersusNode.AddGreyScale();
					return "Expired";
				}
				psFloatingVersusNode.RemoveGreyScale();
				return string.Empty + m_versusData.GetOpponentInfo().score.name + Environment.NewLine + "wants your head!";
			}
			if (m_versusData.status == "declined")
			{
				psFloatingVersusNode.RemoveGreyScale();
				return text + "\ndeclined";
			}
			psFloatingVersusNode.AddGreyScale();
			return "Waiting for\n" + text;
		}
		if (m_versusData.currentPlayer != PlayerPrefsX.GetUserId() || PsState.m_versusChallengeTryAmount <= m_versusData.GetOwnInfo().tries)
		{
			psFloatingVersusNode.AddGreyScale();
			return text + "'s\nturn";
		}
		psFloatingVersusNode.RemoveGreyScale();
		int num = SecondsLeft();
		string text2 = ((num >= 3601 || num < 0) ? string.Empty : ("\n" + PsMetagameManager.GetTimeStringFromSeconds(num)));
		return "Your turn!" + text2;
	}

	public override void CreateGameMode()
	{
		m_gameMode = new PsGameModeRaceFriendly(this);
	}

	public override void LoadMinigame()
	{
		if (m_loaderEntity != null)
		{
			EntityManager.RemoveEntity(m_loaderEntity);
		}
		m_loaderEntity = EntityManager.AddEntity();
		if (m_versusData == null)
		{
			Debug.LogError("ERROR: versusdata null");
		}
		if (m_minigameId != string.Empty && m_minigameMetaData == null)
		{
			Debug.Log("Loading MetaData");
			m_loadingMetaData = true;
			HttpC httpC = MiniGame.Get(m_minigameId, LoadMetaDataSUCCESS, LoadMetaDataFAILED);
			httpC.objectData = m_minigameId;
			EntityManager.AddComponentToEntity(m_loaderEntity, httpC);
			Debug.LogWarning("LOAD METADATA");
		}
		else
		{
			LoadMetaDataSUCCESS(m_minigameMetaData);
		}
		PreviewLoop();
	}

	protected override void LoadMetaDataSUCCESS(PsMinigameMetaData _minigameMetaData)
	{
		m_loadingMetaData = false;
		m_minigameMetaData = _minigameMetaData;
		m_minigameId = m_versusData.gameId;
		if (m_gameMode == null)
		{
			CreateGameMode();
		}
		m_scoreCurrent = 0;
		m_timeScoreCurrent = int.MaxValue;
		m_timeScoreBest = _minigameMetaData.timeScore;
		if (m_timeScoreBest == 0)
		{
			m_timeScoreBest = int.MaxValue;
		}
		m_timeScoreOld = m_timeScoreBest;
		m_scoreOld = m_scoreBest;
		m_rewardOld = m_scoreBest;
		Debug.LogWarning("BEST TIME: " + HighScores.TimeScoreToTime(m_timeScoreBest));
		Debug.LogWarning("BEST SCORE: " + m_scoreBest);
		if (m_loadMetadataCallback != null)
		{
			Debug.LogWarning("------- CALLING CALLBACKS:");
			Delegate[] invocationList = m_loadMetadataCallback.GetInvocationList();
			Delegate[] array = invocationList;
			foreach (Delegate obj in array)
			{
				Debug.LogWarning(obj.Method.Name);
			}
			m_loadMetadataCallback();
		}
		m_loadMetadataCallback = null;
		LoadMinigameBytes();
	}

	public override void SendForfeit()
	{
		ExitMinigame();
		Cancel();
		PsState.m_activeMinigame.m_gameStarted = false;
		EveryplayManager.StopRecording(0f);
		PsIngameMenu.CloseAll();
		PsMetagameManager.HideResources();
		PsMetagameManager.VersusChallengeRewards(false);
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameWin, 1f, delegate
		{
			if (m_versusData.GetOpponentInfo() == null)
			{
				Friendly.Claim(m_timedEventId, ClaimSUCCEED, ClaimFAILED);
			}
			else
			{
				Friendly.Forfeit(m_timedEventId, m_minigameId, PsState.m_activeMinigame.m_gameStartCount, ForfeitSUCCEED, ForfeitFAILED);
			}
			PsPlanetManager.m_timedEvents = null;
			PsMetagameManager.HideResources();
			m_eventOver = true;
			Main.m_currentGame.m_sceneManager.ChangeScene(new PsMenuScene("MenuScene"), new FadeLevelEndLoadingScene(Color.black, this));
		});
	}

	public override void ExitMinigameVersus()
	{
		ExitMinigame();
		Cancel();
		PsState.m_activeMinigame.m_gameStarted = false;
		EveryplayManager.StopRecording(0f);
		PsIngameMenu.CloseAll();
		PsMetagameManager.HideResources();
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameWin, 1f, delegate
		{
			if (m_sendQuit)
			{
				Friendly.Quit(m_timedEventId, m_minigameId, PsState.m_activeMinigame.m_gameStartCount, false, false, FriendlyQuitSUCCEED, FriendlyQuitFAILED, string.Empty);
			}
			PsPlanetManager.m_timedEvents = null;
			PsMetagameManager.HideResources();
			Main.m_currentGame.m_sceneManager.ChangeScene(new PsMenuScene("MenuScene"), new FadeLevelEndLoadingScene(Color.black, this));
		});
	}

	protected override void SetTries()
	{
		new PsServerQueueFlow(delegate
		{
			Friendly.SetTries(m_versusData.timedEventId, m_tryCount, SetTriesSUCCEED, SetTriesFAILED);
		}, null, new string[1] { "setTries" });
	}

	private void ClaimSUCCEED(HttpC _c)
	{
		Debug.Log("Claim succeed");
	}

	private void ClaimFAILED(HttpC _c)
	{
		ServerManager.ThrowServerErrorException("Connection error", _c.www, delegate
		{
			HttpC httpC = Friendly.Claim((string)_c.objectData, ClaimSUCCEED, ClaimFAILED);
			httpC.objectData = _c.objectData;
			return httpC;
		});
	}

	private void ForfeitSUCCEED(HttpC _c)
	{
		Debug.Log("FORFEIT SUCCEED");
	}

	private void ForfeitFAILED(HttpC _c)
	{
		Debug.Log("FORFEIT FAILED");
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), () => Friendly.Forfeit(m_timedEventId, m_minigameId, PsState.m_activeMinigame.m_gameStartCount, ForfeitSUCCEED, ForfeitFAILED));
	}

	private void SetTriesSUCCEED(HttpC _c)
	{
		Debug.Log("SET TRIES SUCCEED");
	}

	private void SetTriesFAILED(HttpC _c)
	{
		Debug.Log("SET TRIES FAILED");
		if (m_setTriesRetryCount < 3)
		{
			Versus.SetTries(m_versusData.timedEventId, m_tryCount, SetTriesSUCCEED, SetTriesFAILED);
		}
		else
		{
			ServerManager.ThrowServerErrorException("Connection error", _c.www, () => Friendly.SetTries(m_versusData.timedEventId, m_tryCount, SetTriesSUCCEED, SetTriesFAILED));
		}
		m_setTriesRetryCount++;
	}

	protected override void SendQuit(bool _winTrue)
	{
		Friendly.Quit(m_timedEventId, m_minigameId, PsState.m_activeMinigame.m_gameStartCount, _winTrue, m_tryCount >= PsState.m_versusChallengeTryAmount && !_winTrue, FriendlyQuitSUCCEED, FriendlyQuitFAILED, string.Empty);
	}

	private void FriendlyQuitSUCCEED(HttpC _c)
	{
		Debug.Log("VERSUS QUIT SUCCEED");
	}

	private void FriendlyQuitFAILED(HttpC _c)
	{
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), () => Friendly.Quit(m_timedEventId, m_minigameId, PsState.m_activeMinigame.m_gameStartCount, m_versusWon, m_tryCount >= PsState.m_versusChallengeTryAmount && !m_versusWon, FriendlyQuitSUCCEED, FriendlyQuitFAILED, string.Empty));
	}

	public override Hashtable GetRentUpgradeValues()
	{
		if (m_gameMode.m_playbackGhosts != null && m_gameMode.m_playbackGhosts.Count > 0 && m_gameMode.m_playbackGhosts[0].m_ghost.m_upgradeValues != null)
		{
			return m_gameMode.m_playbackGhosts[0].m_ghost.m_upgradeValues;
		}
		return base.GetRentUpgradeValues();
	}

	public override Type GetRentVehicleType()
	{
		if (m_gameMode.m_playbackGhosts != null && m_gameMode.m_playbackGhosts.Count > 0)
		{
			return Type.GetType(m_gameMode.m_playbackGhosts[0].m_ghost.m_unitClass);
		}
		return base.GetRentVehicleType();
	}

	public override List<int> GetRentVehicleValues()
	{
		if (m_gameMode.m_playbackGhosts != null && m_gameMode.m_playbackGhosts.Count > 0 && m_gameMode.m_playbackGhosts[0].m_ghost.m_upgradeSteps != null)
		{
			return m_gameMode.m_playbackGhosts[0].m_ghost.m_upgradeSteps;
		}
		return base.GetRentVehicleValues();
	}

	public override void DestroyMinigame()
	{
		if (m_previousLoop != null && m_previousLoop.m_path != null && m_previousLoop.m_path.m_name != "FloatingPath" && (PsState.m_activeMinigame.m_playerReachedGoalCount > 0 || m_tryCount < PsState.m_versusChallengeTryAmount))
		{
			m_previousLoop.m_createFlyingNode = true;
			m_previousLoop.m_versusData = m_versusData;
		}
		base.DestroyMinigame();
	}

	public override void ReleaseLoop()
	{
		if (!m_released)
		{
			Debug.LogWarning("LOOP RELEASED");
			CleanLoop();
			m_scoreOld = m_scoreBest;
			m_rewardOld = m_scoreBest;
			SetActiveLoopNull();
			if (m_previousLoop != null)
			{
				m_previousLoop.SetAsActiveLoop();
			}
			m_previousLoop = null;
		}
	}
}
