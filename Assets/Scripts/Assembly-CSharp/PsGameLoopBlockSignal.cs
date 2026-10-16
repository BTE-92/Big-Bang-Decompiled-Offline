using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsGameLoopBlockSignal : PsGameLoop
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

	public PsGameLoopBlockSignal(PsMinigameContext _context, string _minigameId, PsPathBlockType _blockType, int _requiredStars, string _unlockedProgression, string _willUnlockProgression, PsPlanetPath _path, int _id, int _score, int _blockCoins, int _blockDiamonds, int _blockBolts, int _blockKeys, bool _unlocked, string _rewardCue)
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

	public override void LoadGhosts()
	{
		Main.m_currentGame.m_sceneManager.ChangeScene(new GameScene("GameScene"));
	}

	public override void LoadMinigame()
	{
		TouchAreaS.Enable();
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

	public override void LoadMinigameMetaData(Action _callback = null)
	{
	}

	protected override void MinigameDownloadOk(byte[] _levelData)
	{
		base.MinigameDownloadOk(_levelData);
		LoadGhosts();
	}

	public override void StartLoop()
	{
		m_winFirtTime = false;
		if (PsState.m_activeGameLoop == null)
		{
			if (Input.GetKey(KeyCode.LeftShift) || (PsState.m_adminMode && PsState.m_adminModeSkipping))
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

	public override float GiveCheckpointReward()
	{
		float result = 0f;
		if (!m_unlocked)
		{
			if (m_node != null)
			{
				m_node.Claim();
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

	public void LoopUnlockedTask(TimerC _c)
	{
		m_addedNodes = new List<PsGameLoop>();
		if (!string.IsNullOrEmpty(m_minigameId) && !m_skip)
		{
			TouchAreaS.Disable();
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
				Hashtable progressionJson = ClientTools.GenerateProgressionPathJson(m_addedNodes, m_path.m_currentNodeId);
				PsMetagameManager.SetPlayerDataAndProgression(new Hashtable(), progressionJson, false);
			}
			else
			{
				Hashtable progressionJson2 = ClientTools.GenerateProgressionPathJson(m_path);
				PsMetagameManager.SetPlayerDataAndProgression(new Hashtable(), progressionJson2, false);
			}
		}
	}

	public override void LoopUnlocked()
	{
		if (m_nodeId > 1)
		{
			PsMetagameManager.ShowResources(null, false, true, true, true, true, 0.03f, true);
		}
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

	public override void StartCancelled()
	{
		PsMetagameManager.ShowResources(null, false, true, true, true, true, 0.03f, true);
		SetActiveLoopNull();
	}

	public override void CreateMinigame()
	{
		m_rentedVehicle = true;
		m_userHasSelectedVehicle = true;
		m_selectedVehicle = PsSelectedVehicle.CreatorVehicle;
		Vehicle vehicle = Activator.CreateInstance(Type.GetType(m_minigameMetaData.playerUnit), new string[0]) as Vehicle;
		PsState.m_activeGameLoop.m_rentUpgradeValues = vehicle.ParseUpgradeValues(m_minigameMetaData.creatorUpgrades);
		vehicle.Destroy();
		base.CreateMinigame();
	}

	public override void CreateEnvironment()
	{
		PsGameLoop nodeInfo = m_path.GetNodeInfo(m_nodeId + 1);
		if (nodeInfo != null && nodeInfo.m_minigameMetaData == null)
		{
			nodeInfo.LoadMinigameMetaData();
		}
		if (PsState.m_activeMinigame.m_backgroundPrefab != null)
		{
			PrefabS.RemoveComponent(PsState.m_activeMinigame.m_backgroundPrefab);
			PsState.m_activeMinigame.m_backgroundPrefab = null;
		}
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.Map_Play_Human_Large_GameObject);
		PsState.m_activeMinigame.m_backgroundPrefab = PrefabS.AddComponent(PsState.m_activeMinigame.m_environmentTC, new Vector3(0f, 30f, 70f), gameObject);
		GameObject gameObject2 = PsState.m_activeMinigame.m_backgroundPrefab.p_gameObject.transform.FindChild("Static").gameObject;
		if ((bool)gameObject2)
		{
			StaticBatchingUtility.Combine(gameObject2);
		}
	}

	public override void EnterMinigame()
	{
		m_backAtMenu = false;
		CameraS.m_updateComponents = true;
		PsLoadingScreen.FadeOut();
		if (PsState.m_activeGameLoop.m_nodeId < PsState.m_activeGameLoop.m_path.m_currentNodeId && PsIngameMenu.m_playMenu == null)
		{
			PsIngameMenu.m_playMenu = new PsUITopPlaySignal(delegate
			{
				PsState.m_activeGameLoop.ExitMinigame();
			});
		}
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameEnter, 0.5f, delegate
		{
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GameSignalTuneInState());
		});
	}

	public override void InitializeMinigame()
	{
		base.InitializeMinigame();
	}

	public override void StartMinigame()
	{
		UnityEngine.Random.seed = (int)(Main.m_gameTimeSinceAppStarted * 60.0);
		PsState.m_physicsPaused = false;
		PsState.m_activeMinigame.m_gameStarted = true;
		PsState.m_activeMinigame.m_gameStartCount++;
		PsState.m_activeMinigame.m_playerReachedGoal = false;
		CreatePlaybackGhosts();
		SoundS.PlaySingleShot("/InGame/GameStart", Vector3.zero);
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GamePlayState());
	}

	public override void PauseMinigame()
	{
		if (!PsState.m_activeMinigame.m_gamePaused && PsState.m_activeMinigame.m_gameStarted && !PsState.m_activeMinigame.m_gameEnded)
		{
			PsState.m_activeMinigame.SetGamePaused(true);
			PauseElapsedTimer();
			PsIngameMenu.CloseAll();
			PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterPauseSignal));
			PsIngameMenu.m_popupMenu.SetAction("Exit", ExitMinigame);
			PsIngameMenu.m_popupMenu.SetAction("Resume", ResumeMinigame);
		}
	}

	public override void ResumeMinigame()
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.OpenController();
		if (PsState.m_activeGameLoop.m_nodeId < PsState.m_activeGameLoop.m_path.m_currentNodeId && PsIngameMenu.m_playMenu == null)
		{
			PsIngameMenu.m_playMenu = new PsUITopPlaySignal(delegate
			{
				PsState.m_activeGameLoop.ExitMinigame();
			});
		}
		PsState.m_activeMinigame.SetGamePaused(false);
		ResumeElapsedTimer();
	}

	public override void LoseMinigame()
	{
		PsIngameMenu.CloseAll();
		if (!PsState.m_activeMinigame.m_gameEnded)
		{
			PsState.m_activeMinigame.m_gameEnded = true;
			PsState.m_activeMinigame.m_gameDeathCount++;
			new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameLose, 1f, delegate
			{
				PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterLoseRace), null, null, null, false);
				PsIngameMenu.m_popupMenu.SetAction("Restart", RestartMinigame);
			});
		}
	}

	public override void RestartMinigame()
	{
		base.RestartMinigame();
		PsIngameMenu.CloseAll();
		ResumeElapsedTimer();
		PsState.m_activeMinigame.SetGamePaused(false);
		EntityManager.RemoveEntitiesByTag("GTAG_AUTODESTROY");
		CameraS.RemoveAllTargetComponents();
		PsState.m_activeMinigame.m_groundNode.RevertGroundFromPlay();
		LevelManager.ResetCurrentLevel();
		InitializeMinigame();
		PsIngameMenu.OpenController(true);
		if (PsState.m_activeGameLoop.m_nodeId < PsState.m_activeGameLoop.m_path.m_currentNodeId && PsIngameMenu.m_playMenu == null)
		{
			PsIngameMenu.m_playMenu = new PsUITopPlaySignal(delegate
			{
				PsState.m_activeGameLoop.ExitMinigame();
			});
		}
	}

	public override void WinMinigame()
	{
		base.WinMinigame();
		PsIngameMenu.CloseAll();
		if (PsState.m_activeMinigame.m_gameEnded)
		{
			return;
		}
		PsState.m_activeMinigame.m_gameEnded = true;
		TimerS.AddComponent(EntityManager.AddEntity(), string.Empty, 0f, 2f, false, delegate(TimerC _c)
		{
			TimerS.RemoveComponent(_c);
			new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameWin, 3f, delegate
			{
				Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GameSignalTuneOutState());
			});
		});
	}

	public override void ExitMinigame()
	{
		GameSceneEffectManager.DestroyCrew();
		PsIngameMenu.CloseAll();
		base.ExitMinigame();
		if (PsState.m_openPopups.Count > 0)
		{
			PsState.m_openPopups[0].Destroy();
		}
		SaveProgression();
		Main.m_currentGame.m_sceneManager.ChangeScene(new PsMenuScene("MenuScene"), new FadeLoadingScene(Color.black));
	}

	public override void DestroyMinigame()
	{
		base.DestroyMinigame();
		EntityManager.RemoveEntitiesByTag("GTAG_AUTODESTROY");
		CameraS.RemoveAllTargetComponents();
		LevelManager.DestroyCurrentLevel();
		PsState.m_activeMinigame = null;
		GameSceneEffectManager.Destroy();
	}

	public override void SaveProgression(bool _save = true)
	{
		if (m_nodeId == m_path.GetLastBlockId())
		{
			m_scoreBest = 0;
			m_path.m_currentNodeId = m_nodeId + 1;
			PsMetagameManager.m_playerStats.level++;
			if (m_path.m_name == "MainPath")
			{
				m_addedNodes = m_planet.GiveNextStrip(this);
				PsGameLoop nodeInfo = m_path.GetNodeInfo(m_path.GetNextBlockId(m_nodeId));
				IncreaseItemLevel(nodeInfo.GetUnlockedProgressionName());
				PsMetagameManager.AddNewEditorItems(m_unlockableList);
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
			PsMetagameManager.AddNewEditorItems(m_unlockableList);
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
		if (m_skip || m_path.m_name != "MainPath")
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
		if (m_nodeId + 1 == m_path.m_currentNodeId && m_path.GetCurrentNodeInfo().m_keepNodeHidden && m_path.m_name == "MainPath")
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
		if (m_released)
		{
			return;
		}
		base.ReleaseLoop();
		if (m_node != null)
		{
			m_node.RemoveHighlight();
		}
		if (m_nodeId == m_path.m_currentNodeId + 1)
		{
			new PsMenuDialogueFlow(this, PsNodeEventTrigger.LoopReleased, 0f, delegate
			{
				SetActiveLoopNull();
				ActivateCurrentLoop();
			});
		}
		else
		{
			SetActiveLoopNull();
			ActivateCurrentLoop();
		}
	}
}
