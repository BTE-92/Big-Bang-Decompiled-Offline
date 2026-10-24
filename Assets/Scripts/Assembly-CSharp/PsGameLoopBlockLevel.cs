using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsGameLoopBlockLevel : PsGameLoop
{
	public PsPathBlockType m_blockType;

	public int m_requiredStars;

	public string m_unlockedProgression;

	public string m_willUnlockProgression;

	public int m_blockCoins;

	public int m_blockDiamonds;

	public int m_blockBolts;

	public int m_blockKeys;

	public string m_rewardCue;

	public bool m_skip;

	private List<PsGameLoop> m_addedNodes;

	public PsGameLoopBlockLevel(PsMinigameContext _context, string _minigameId, PsPathBlockType _blockType, int _requiredStars, string _unlockedProgression, string _willUnlockProgression, PsPlanetPath _path, int _id, int _score, int _blockCoins, int _blockDiamonds, int _blockBolts, int _blockKeys, bool _unlocked, string _rewardCue)
		: base(_context, _minigameId, _path, _id, -1, _score, _unlocked)
	{
		m_blockType = _blockType;
		m_requiredStars = _requiredStars;
		m_unlockedProgression = _unlockedProgression;
		m_willUnlockProgression = _willUnlockProgression;
		m_blockCoins = _blockCoins;
		m_blockDiamonds = _blockDiamonds;
		m_blockBolts = _blockBolts;
		m_blockKeys = _blockKeys;
		m_rewardCue = _rewardCue;
	}

	public override void SetUnlockedProgressionName(string _name)
	{
		m_unlockedProgression = _name;
	}

	public override string GetUnlockedProgressionName()
	{
		return m_unlockedProgression;
	}

	public override string GetWillUnlockProgressionName()
	{
		return m_willUnlockProgression;
	}

	public override void StartLoop()
	{
		m_winFirtTime = false;
		if (PsState.m_activeGameLoop == null)
		{
			if (Input.GetKey(KeyCode.LeftShift) || (true && PsState.m_adminModeSkipping))
			{
				m_skip = true;
			}
			else
			{
				m_skip = false;
			}
			if (m_nodeId == m_path.m_currentNodeId)
			{
				SetAsActiveLoop();
				new PsUnlockCheckpointFlow(LoopUnlocked, StartCancelled, this);
			}
			else if (m_nodeId < m_path.m_currentNodeId)
			{
				SetAsActiveLoop();
				LoopUnlocked();
			}
		}
	}

	public override void LoadMinigameMetaData(Action _callback = null)
	{
	}

	public override float GiveCheckpointReward()
	{
		float result = 0f;
		if (!m_unlocked)
		{
			if (m_node != null)
			{
				m_node.Claim();
				TimerS.AddComponent(EntityManager.AddEntity(), "Reward", 0.4f, 0f, true, delegate
				{
					Vector2 startPos = PsPlanetManager.GetCurrentPlanet().m_planetCamera.WorldToScreenPoint(m_node.m_prefab.p_parentTC.transform.position + Vector3.up * 0.005f * Screen.height) - new Vector3(Screen.width, Screen.height, 0f) * 0.5f;
					if (m_blockBolts > 0)
					{
						PsMetagameManager.m_playerStats.CumulateBoltsWithFlyingResources(m_blockBolts, startPos);
					}
					if (m_blockCoins > 0)
					{
						PsMetagameManager.m_playerStats.CumulateCoinsWithFlyingResources(m_blockCoins, startPos);
					}
					if (m_blockDiamonds > 0)
					{
						PsMetagameManager.m_playerStats.CumulateDiamondsWithFlyingResources(m_blockDiamonds, startPos);
					}
					if (m_blockKeys > 0)
					{
						PsMetagameManager.m_playerStats.CumulateKeysWithFlyingResources(m_blockKeys, startPos);
					}
				});
			}
			else
			{
				if (m_blockBolts > 0)
				{
					PsMetagameManager.m_playerStats.CumulateBolts(m_blockBolts);
				}
				if (m_blockCoins > 0)
				{
					PsMetagameManager.m_playerStats.CumulateCoins(m_blockCoins);
				}
				if (m_blockDiamonds > 0)
				{
					PsMetagameManager.m_playerStats.CumulateDiamonds(m_blockDiamonds);
				}
				if (m_blockKeys > 0)
				{
					PsMetagameManager.m_playerStats.CumulateKeys(m_blockKeys);
				}
			}
			if (m_blockBolts > 0 || m_blockCoins > 0 || m_blockDiamonds > 0 || m_blockKeys > 0)
			{
				result = 1f;
				SoundS.PlaySingleShot("/Metagame/ChestOpen", Vector3.zero);
			}
		}
		return result;
	}

	public override void LoopUnlocked()
	{
		PsMetagameManager.ShowResources(null, false, true, true, true, true, 0.03f, true);
		m_backAtMenu = true;
		if (m_node != null)
		{
			m_node.m_hideUI = true;
			TimerS.AddComponent(EntityManager.AddEntity(), "Timer", 0.3f, 0f, true, LoopUnlockedTask);
		}
		else
		{
			LoopUnlockedTask(null);
		}
	}

	public void LoopUnlockedTask(TimerC _c)
	{
		m_addedNodes = new List<PsGameLoop>();
		if (!string.IsNullOrEmpty(m_minigameId) && !m_skip)
		{
			if (m_nodeId == m_path.m_currentNodeId)
			{
				float delay = GiveCheckpointReward();
				new PsMenuDialogueFlow(this, PsNodeEventTrigger.LoopStart, delay, delegate
				{
					PsLoadingScreen.FadeIn(delegate
					{
						LoadMinigame();
					});
				});
			}
			else if (m_nodeId < m_path.m_currentNodeId)
			{
				PsLoadingScreen.FadeIn(delegate
				{
					LoadMinigame();
				});
			}
		}
		else if (m_nodeId == m_path.m_currentNodeId)
		{
			float delay2 = GiveCheckpointReward();
			new PsMenuDialogueFlow(this, PsNodeEventTrigger.LoopStart, delay2, delegate
			{
				SaveProgression(m_unlocked ? true : false);
			});
		}
		else
		{
			StartCancelled();
		}
		if (m_nodeId == m_path.m_currentNodeId && !m_unlocked)
		{
			m_unlocked = true;
			if (m_path.m_name == "MainPath")
			{
				PsMetagameManager.m_playerStats.stars = 0;
				m_addedNodes.Add(this);
				Hashtable progressionJson = ClientTools.GenerateProgressionPathJson(this, m_path.m_currentNodeId);
				PsMetagameManager.SetPlayerDataAndProgression(new Hashtable(), progressionJson, false);
			}
			else
			{
				Hashtable progressionJson2 = ClientTools.GenerateProgressionPathJson(m_path);
				PsMetagameManager.SetPlayerDataAndProgression(new Hashtable(), progressionJson2, false);
			}
		}
	}

	public override void StartCancelled()
	{
		PsMetagameManager.ShowResources(null, false, true, true, true, true, 0.03f, true);
		SetActiveLoopNull();
	}

	public override void LoadMinigame()
	{
		if (m_loadingMetaData)
		{
			Debug.LogWarning("ALREADY LOADING MINIGAME META DATA. ADDING TO CALLBACK");
			if (m_loadMetadataCallback != null)
			{
				m_loadMetadataCallback = (Action)Delegate.Combine(m_loadMetadataCallback, new Action(LoadMinigame));
			}
			else
			{
				m_loadMetadataCallback = LoadMinigame;
			}
		}
		else
		{
			base.LoadMinigame();
		}
	}

	protected override void LoadMetaDataSUCCESS(PsMinigameMetaData _minigameMetaData)
	{
		base.LoadMetaDataSUCCESS(_minigameMetaData);
		LoadMinigameBytes();
	}

	protected override void MinigameDownloadOk(byte[] _levelData)
	{
		base.MinigameDownloadOk(_levelData);
		Main.m_currentGame.m_sceneManager.ChangeScene(new GameScene("GameScene"));
	}

	public override void EnterMinigame()
	{
		m_backAtMenu = false;
		PsGameLoop nodeInfo = m_path.GetNodeInfo(m_nodeId + 1);
		if (nodeInfo != null && nodeInfo.m_minigameMetaData == null)
		{
			nodeInfo.LoadMinigameMetaData();
		}
		CameraS.m_updateComponents = true;
		PsLoadingScreen.FadeOut();
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameEnter, 0.5f, delegate
		{
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GameStartState());
			PsMetagameManager.ShowResources(null, true, true, true, true, false);
			if (!m_unlockedProgression.Equals("unlock_nitros"))
			{
				PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUIVehicleSelect), null, null, null, false);
			}
			PsIngameMenu.OpenController(true);
		});
	}

	public override void InitializeMinigame()
	{
		base.InitializeMinigame();
		SetPlaybackGhostAndCoins();
	}

	public override void StartMinigame()
	{
		if (PsState.m_activeMinigame.m_music != null)
		{
			SoundS.SetSoundParameter(PsState.m_activeMinigame.m_music, "MusicState", 1f);
		}
		PsMetagameManager.HideResources();
		PsState.m_activeMinigame.m_gameStarted = true;
		m_gameMode.CreatePlayMenu(RestartMinigame, PauseMinigame);
		UnityEngine.Random.seed = (int)(Main.m_gameTimeSinceAppStarted * 60.0);
		PsState.m_physicsPaused = false;
		PsState.m_activeMinigame.m_gameOn = true;
		PsState.m_activeMinigame.m_gameStartCount++;
		PsState.m_activeMinigame.m_playerReachedGoal = false;
		CreatePlaybackGhosts();
		SoundS.PlaySingleShot("/InGame/GameStart", Vector3.zero);
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GamePlayState());
	}

	public override void CreateGameMode()
	{
		m_gameMode = new PsGameModeBlock(this);
	}

	public override void PauseMinigame()
	{
		if (!PsState.m_activeMinigame.m_gamePaused && PsState.m_activeMinigame.m_gameStarted && !PsState.m_activeMinigame.m_gameEnded)
		{
			PsState.m_activeMinigame.SetGamePaused(true);
			PauseElapsedTimer();
			PsIngameMenu.CloseAll();
			PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterPauseRace), typeof(PsUITopPauseRace), null, null, false);
			PsIngameMenu.m_popupMenu.SetAction("Exit", ExitMinigame);
			PsIngameMenu.m_popupMenu.SetAction("Restart", RestartMinigame);
			PsIngameMenu.m_popupMenu.SetAction("Resume", ResumeMinigame);
		}
	}

	public override void CreateMinigame()
	{
		if (m_unlockedProgression.Equals("unlock_nitros"))
		{
			m_rentedVehicle = true;
			m_userHasSelectedVehicle = true;
			m_selectedVehicle = PsSelectedVehicle.CreatorVehicle;
			Vehicle vehicle = Activator.CreateInstance(Type.GetType(m_minigameMetaData.playerUnit), new string[0]) as Vehicle;
			PsState.m_activeGameLoop.m_rentUpgradeValues = vehicle.ParseUpgradeValues(m_minigameMetaData.creatorUpgrades);
			vehicle.Destroy();
		}
		base.CreateMinigame();
	}

	public override void ResumeMinigame()
	{
		PsIngameMenu.CloseAll();
		m_gameMode.CreatePlayMenu(RestartMinigame, PauseMinigame);
		PsIngameMenu.OpenController();
		PsState.m_activeMinigame.SetGamePaused(false);
		ResumeElapsedTimer();
	}

	public override void RestartMinigame()
	{
		base.RestartMinigame();
		PsIngameMenu.CloseAll();
		PsState.m_activeMinigame.SetGamePaused(false);
		ResumeElapsedTimer();
		EntityManager.RemoveEntitiesByTag("GTAG_AUTODESTROY");
		CameraS.RemoveAllTargetComponents();
		PsState.m_activeMinigame.m_groundNode.RevertGroundFromPlay();
		LevelManager.ResetCurrentLevel();
		InitializeMinigame();
		PsMetagameManager.ShowResources(null, true, true, true, true, false);
		if (!m_unlockedProgression.Equals("unlock_nitros"))
		{
			PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUIVehicleSelect), null, null, null, false);
		}
		PsIngameMenu.OpenController(true);
	}

	public override void LoseMinigame()
	{
		PsIngameMenu.CloseAll();
		PsState.m_activeMinigame.m_gameEnded = true;
		PsState.m_activeMinigame.m_gameOn = false;
		PsState.m_activeMinigame.m_gameDeathCount++;
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameLose, 1f, delegate
		{
			PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterLoseRace), typeof(PsUITopLoseRace), null, null, false);
			PsIngameMenu.m_popupMenu.SetAction("Exit", ExitMinigame);
			PsIngameMenu.m_popupMenu.SetAction("Restart", RestartMinigame);
		});
	}

	public override void WinMinigame()
	{
		base.WinMinigame();
		PsIngameMenu.CloseAll();
		PsState.m_activeMinigame.m_gameEnded = true;
		PsState.m_activeMinigame.m_playerReachedGoal = true;
		PsState.m_activeMinigame.m_gameOn = false;
		SaveProgression();
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameWin, 1f, delegate
		{
			ExitMinigame();
		});
	}

	private void ExitMinigameAction()
	{
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameExit, 0f, ExitMinigame);
	}

	public override void ExitMinigame()
	{
		base.ExitMinigame();
		PsIngameMenu.CloseAll();
		Main.m_currentGame.m_sceneManager.ChangeScene(new PsMenuScene("MenuScene"), new FadeLoadingScene(Color.black));
	}

	public override void DestroyMinigame()
	{
		base.DestroyMinigame();
		EntityManager.RemoveEntitiesByTag("GTAG_AUTODESTROY");
		CameraS.RemoveAllTargetComponents();
		LevelManager.DestroyCurrentLevel();
		PsState.m_activeMinigame = null;
	}

	public override void SaveProgression(bool _save = true)
	{
		if (m_nodeId == m_path.GetLastBlockId())
		{
			Debug.Log("is last blockid");
			m_scoreBest = 0;
			m_path.m_currentNodeId = m_nodeId + 1;
			PsMetagameManager.m_playerStats.level++;
			if (m_path.m_name == "MainPath")
			{
				Debug.Log("GIVE NEXT STRIP");
				m_addedNodes = m_planet.GiveNextStrip(this);
				PsGameLoop nodeInfo = m_path.GetNodeInfo(m_path.GetNextBlockId(m_nodeId));
				IncreaseItemLevel(nodeInfo.GetUnlockedProgressionName());
				if (_save)
				{
					Hashtable progressionJson = ClientTools.GenerateProgressionPathJson(m_addedNodes, m_path.m_currentNodeId);
					PsMetagameManager.SetPlayerDataAndProgression(new Hashtable(), progressionJson, true);
				}
			}
			else if (_save)
			{
				Hashtable progressionJson2 = ClientTools.GenerateProgressionPathJson(m_path);
				PsMetagameManager.SetPlayerDataAndProgression(new Hashtable(), progressionJson2, false);
			}
		}
		else if (m_nodeId == m_path.m_currentNodeId)
		{
			m_path.m_currentNodeId = m_nodeId + 1;
			PsMetagameManager.m_playerStats.level++;
			PsGameLoop nodeInfo2 = m_path.GetNodeInfo(m_path.GetNextBlockId(m_nodeId));
			IncreaseItemLevel(nodeInfo2.GetUnlockedProgressionName());
			if (m_path.m_name == "MainPath")
			{
				Hashtable progressionJson3 = ClientTools.GenerateProgressionPathJson(this, m_path.m_currentNodeId);
				PsMetagameManager.SetPlayerDataAndProgression(new Hashtable(), progressionJson3, true);
			}
			else
			{
				Hashtable progressionJson4 = ClientTools.GenerateProgressionPathJson(m_path);
				PsMetagameManager.SetPlayerDataAndProgression(new Hashtable(), progressionJson4, false);
			}
		}
		if (m_skip || string.IsNullOrEmpty(m_minigameId) || m_path.m_name != "MainPath")
		{
			StartLoopClaimedDialogueFlow();
		}
	}

	public override void BackAtMenu()
	{
		if (!m_backAtMenu && !m_released)
		{
			m_backAtMenu = true;
			StartLoopClaimedDialogueFlow();
		}
	}

	public void StartLoopClaimedDialogueFlow()
	{
		if (m_path.m_name == "MainPath" && m_nodeId + 1 == m_path.m_currentNodeId && m_path.GetCurrentNodeInfo() != null && m_path.GetCurrentNodeInfo().m_keepNodeHidden)
		{
			new PsMenuDialogueFlow(this, PsNodeEventTrigger.LoopClaimed, 0f, delegate
			{
				m_planet.StartRevealSquence(m_addedNodes);
			});
		}
		else
		{
			new PsMenuDialogueFlow(this, PsNodeEventTrigger.LoopClaimed, 0f, ReleaseLoop);
		}
	}

	public override void ReleaseLoop()
	{
		PsMenuScene.m_lastState = null;
		if (!m_released)
		{
			base.ReleaseLoop();
			if (m_node != null)
			{
				m_node.RemoveHighlight();
			}
			new PsMenuDialogueFlow(this, PsNodeEventTrigger.LoopReleased, 0f, delegate
			{
				SetActiveLoopNull();
				ActivateCurrentLoop();
			});
		}
	}
}
