using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using UnityEngine;

public abstract class PsGameModeBase
{
	public Entity m_ghostLoaderEntity;

	public bool m_waitForHighscoreAndGhost;

	public bool m_waitForNextGhost;

	public bool m_exitingMinigame;

	public PsGameMode m_gameMode;

	public PsGameLoop m_gameLoop;

	public List<GhostObject> m_playbackGhosts;

	public Ghost m_recordingGhost;

	public int m_ghostRetryCounter;

	public bool m_ghostLoadComplete;

	public List<PsStreak> m_streaks;

	public List<int> m_ghostScores;

	public PsGameModeBase(PsGameMode _gameMode, PsGameLoop _info)
	{
		m_gameMode = _gameMode;
		m_gameLoop = _info;
		m_playbackGhosts = new List<GhostObject>();
	}

	public virtual void CreateEnvironment()
	{
		if (!PsState.m_muteMusic)
		{
			CreateMusic();
		}
		if (PsState.m_activeMinigame.m_backgroundPrefab != null)
		{
			PrefabS.RemoveComponent(PsState.m_activeMinigame.m_backgroundPrefab);
			PsState.m_activeMinigame.m_backgroundPrefab = null;
		}
		int num = (int)PsState.m_activeMinigame.m_settings["domeSizeIndex"];
		GameObject gameObject = null;
		switch (num)
		{
		case 0:
			gameObject = ResourceManager.GetGameObject(RESOURCE.Map_Play_Small_GameObject);
			break;
		case 1:
			gameObject = ResourceManager.GetGameObject(RESOURCE.Map_Play_Medium_GameObject);
			break;
		case 2:
			gameObject = ResourceManager.GetGameObject(RESOURCE.Map_Play_Large_GameObject);
			break;
		}
		PsState.m_activeMinigame.m_backgroundPrefab = PrefabS.AddComponent(PsState.m_activeMinigame.m_environmentTC, new Vector3(0f, 30f, 70f), gameObject);
		GameObject gameObject2 = PsState.m_activeMinigame.m_backgroundPrefab.p_gameObject.transform.Find("Static").gameObject;
		if ((bool)gameObject2)
		{
			StaticBatchingUtility.Combine(gameObject2);
		}
	}

	public virtual void CreateMusic()
	{
		Debug.LogError("CreateMusic");
	}

	public virtual void ApplySettings(bool _createAreaLimits = true, bool _updateDomeGfx = false)
	{
		PsState.m_activeMinigame.ApplySettings(_createAreaLimits, _updateDomeGfx);
		GenerateCollectableCoins();
	}

	public virtual void CreatePlayMenu(Action _restartAction, Action _pauseAction)
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.OpenController();
	}

	public virtual void CreateEditMenu(Action _exitAction, Action _testAction)
	{
		PsIngameMenu.CloseAll();
	}

	public virtual void EnterMinigame()
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUIVehicleSelect), typeof(PsUITopStartRace), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Skip", m_gameLoop.SkipMinigame);
		PsMetagameManager.ShowResources(PsIngameMenu.m_popupMenu.m_overlayCamera, true, true, true, true, false, 0.03f, true);
	}

	public virtual void StartMinigame()
	{
		PsIngameMenu.CloseAll();
		PsMetagameManager.HideResources();
		CreatePlayMenu(m_gameLoop.RestartMinigame, m_gameLoop.PauseMinigame);
		if (PsState.m_activeMinigame.m_music != null)
		{
			SoundS.SetSoundParameter(PsState.m_activeMinigame.m_music, "MusicState", 1f);
		}
	}

	public virtual void PauseMinigame()
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterPauseRace), typeof(PsUITopPause), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Restart", m_gameLoop.RestartMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Resume", m_gameLoop.ResumeMinigame);
	}

	public virtual void WinMinigame()
	{
		GiveRewards();
		PsAchievementManager.SendToServer();
	}

	protected virtual void CreateWinUI()
	{
	}

	public virtual void GiveRewards()
	{
		if (m_gameLoop.m_scoreCurrent > m_gameLoop.m_rewardOld)
		{
			int stars = m_gameLoop.m_scoreCurrent - m_gameLoop.m_rewardOld;
			PsMetagameManager.GiveGoalReward(stars, 0, m_gameLoop.GetDifficulty());
			m_gameLoop.m_rewardOld = m_gameLoop.m_scoreCurrent;
			PsMetagameManager.m_playerStats.updated = false;
		}
		PsMetagameManager.SetPlayerData(new Hashtable(), false, PsMetagameManager.PlayerDataSetSUCCEED, PsMetagameManager.PlayerDataSetFAILED);
	}

	public virtual void LoseMinigame()
	{
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterLoseRace), typeof(PsUITopLoseRace), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Restart", m_gameLoop.RestartMinigame);
	}

	public virtual bool isMaterialAvailable(int _index)
	{
		PsUnlockable psUnlockable = PsMetagameData.m_gameMaterials[0].m_items[_index];
		return psUnlockable.m_unlocked || PsState.m_adminMode;
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

	public virtual void Destroy()
	{
		DestroyGhosts();
		DestroyGhostLoaderEntity();
	}

	public void CreateGhostLoaderEntity()
	{
		if (m_ghostLoaderEntity != null)
		{
			Debug.LogError("trying to create second ghost loader entity");
		}
		Debug.LogWarning("------------------------> CREATING GHOST LOADER ENTITY");
		m_ghostLoaderEntity = EntityManager.AddEntity("GhostLoaderEntity");
		m_ghostLoaderEntity.m_persistent = true;
	}

	public virtual void DestroyGhostLoaderEntity()
	{
		if (m_ghostLoaderEntity != null)
		{
			Debug.LogWarning("------------------------> DESTROYING GHOST LOADER ENTITY");
			EntityManager.RemoveEntity(m_ghostLoaderEntity);
			m_ghostLoaderEntity = null;
		}
	}

	public virtual void LoadGhosts(Action<GhostData[]> _customSUCCESSCallback = null)
	{
		DestroyPlaybackGhosts();
		m_ghostRetryCounter = 0;
		m_ghostLoadComplete = true;
		Debug.LogWarning("NO GHOSTS IN THIS GAME MODE");
	}

	public virtual HttpC ServerGetGhosts(GhostGetData _data, Action<GhostData[]> _customSUCCESSCallback = null)
	{
		Action<GhostData[]> okCallback = GhostsLoadSUCCEED;
		if (_customSUCCESSCallback != null)
		{
			okCallback = _customSUCCESSCallback;
		}
		Debug.LogWarning("REQUEST GHOST FROM SERVER");
		HttpC httpC = ((!string.IsNullOrEmpty(_data.playerId)) ? Server.Ghost.GetCreatorNew(_data.minigameInfo.m_minigameMetaData, okCallback, GhostsLoadFAILED, GhostLoadErrorHandler) : Server.Ghost.GetNext(_data.minigameInfo.GetGameId(), _data.times, okCallback, GhostsLoadFAILED, GhostLoadErrorHandler));
		httpC.objectData = _data;
		CreateGhostLoaderEntity();
		EntityManager.AddComponentToEntity(m_ghostLoaderEntity, httpC);
		return httpC;
	}

	public void GhostLoadErrorHandler()
	{
		m_ghostLoadComplete = true;
		m_gameLoop.GhostsLoaded();
	}

	public virtual void GhostsLoadSUCCEED(GhostData[] _data)
	{
		Debug.Log("LOLLLLLLL");
		DestroyGhostLoaderEntity();
		GhostsLoaded(_data);
		m_gameLoop.GhostsLoaded();
	}

	public virtual void GhostsLoaded(GhostData[] _data)
	{
		m_ghostScores = new List<int>();
		Debug.LogWarning(_data.Length + " GHOSTS DOWNLOADED");
		for (int i = 0; i < _data.Length; i++)
		{
			if (_data[i].ghost != null && _data[i].ghost.Length > 0)
			{
				byte[] array = FilePacker.UnZipBytes(_data[i].ghost);
				Ghost ghost = Ghost.DeSerializeFromBytes(array);
				Debug.LogWarning("GHOST NO. " + (i + 1) + ": " + _data[i].ghost.Length + " (unzipped: " + array.Length + ")");
				if (ghost != null)
				{
					m_ghostScores.Add(_data[i].time);
					GhostObject item = new GhostObject(_data[i].name, _data[i].time, _data[i].playerId, ghost);
					m_playbackGhosts.Add(item);
					Debug.LogWarning("CREATED PLAYBACK GHOST:" + _data[i].name);
					PsMetrics.GhostLoaded(PsState.m_activeGameLoop.m_context.ToString(), _data[i].ghost.Length.ToString(), PsState.m_activeGameLoop.GetGameId());
				}
				else
				{
					Debug.LogError("GHOST DATA CORRUPTED!");
				}
			}
			else
			{
				Debug.LogWarning("GHOST DOWNLOAD FAILED - NULL OR CORRUPT DATA");
			}
		}
	}

	public virtual void GhostsLoadFAILED(HttpC _c)
	{
		Debug.LogWarning("GHOST DOWNLOAD FAILED");
		if (m_ghostRetryCounter < 2)
		{
			ServerGetGhosts((GhostGetData)_c.objectData);
			m_ghostRetryCounter++;
			Debug.LogWarning("RETRYING GHOST DOWNLOAD: " + m_ghostRetryCounter);
		}
		else
		{
			GhostLoadErrorHandler();
		}
	}

	public virtual void CreatePlaybackGhostVisuals()
	{
		for (int i = 0; i < m_playbackGhosts.Count; i++)
		{
			m_playbackGhosts[i].DestroyVisuals();
			m_playbackGhosts[i].m_ghost.m_playbackTick = 0f;
			string text = m_playbackGhosts[i].m_ghost.m_unitClass;
			if (string.IsNullOrEmpty(text))
			{
				text = PsState.m_activeGameLoop.m_minigameMetaData.playerUnit;
			}
			if (text == "OffroadCar")
			{
				CreateOffroadCarPlaybackGhostEntity(m_playbackGhosts[i]);
			}
			else if (text == "Motorcycle")
			{
				CreateMotorcyclePlaybackGhostEntity(m_playbackGhosts[i]);
			}
			Debug.LogWarning("CREATED PLAYBACK GHOST VISUALS: " + m_playbackGhosts[i].m_name);
		}
	}

	public void DestroyPlaybackGhostVisuals()
	{
		for (int i = 0; i < m_playbackGhosts.Count; i++)
		{
			Debug.LogWarning("DESTROYING PLAYBACK GHOST VISUALS: " + m_playbackGhosts[i].m_name);
			m_playbackGhosts[i].DestroyVisuals();
		}
	}

	public void DestroyPlaybackGhosts()
	{
		while (m_playbackGhosts.Count > 0)
		{
			int index = m_playbackGhosts.Count - 1;
			Debug.LogWarning("DESTROYING PLAYBACK GHOST: " + m_playbackGhosts[index].m_name);
			m_playbackGhosts[index].Destroy();
			m_playbackGhosts.RemoveAt(index);
		}
		m_streaks = null;
	}

	public void DestroyGhosts()
	{
		if (m_recordingGhost != null)
		{
			m_recordingGhost.Destroy();
			m_recordingGhost = null;
		}
		DestroyPlaybackGhosts();
		m_streaks = null;
	}

	public void CreateRecordingGhost(string _unitClass, Hashtable _upgradeValues, List<int> _upgradeSteps)
	{
		DestroyRecordingGhost();
		m_recordingGhost = new Ghost(_unitClass, _upgradeValues, _upgradeSteps);
	}

	public void DestroyRecordingGhost()
	{
		if (m_recordingGhost != null)
		{
			m_recordingGhost.Destroy();
			m_recordingGhost = null;
		}
	}

	public void StopRecordingGhost()
	{
		if (m_recordingGhost != null && m_recordingGhost.m_recording)
		{
			m_recordingGhost.StopRecord();
		}
	}

	public void UpdateGhosts()
	{
		if (PsState.m_activeMinigame.m_gameStarted && !PsState.m_activeMinigame.m_gameEnded && m_recordingGhost != null)
		{
			m_recordingGhost.Update();
		}
		for (int i = 0; i < m_playbackGhosts.Count; i++)
		{
			m_playbackGhosts[i].Update();
		}
	}

	public void CreateOffroadCarPlaybackGhostEntity(GhostObject _ghost)
	{
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.OffroadCarGhostNewPrefab_GameObject);
		GhostPart ghostPart = new GhostPart(gameObject.transform.Find("Body").gameObject, Vector3.zero, "chassis");
		GhostPart ghostPart2 = new GhostPart(gameObject.transform.Find("Parts/CarTireFront").gameObject, new Vector3(0f, 0f, -23f), "frontWheel");
		GhostPart ghostPart3 = new GhostPart(gameObject.transform.Find("Parts/CarTireFront").gameObject, new Vector3(0f, 0f, 23f), "frontWheel");
		ghostPart3.prefab.transform.rotation.eulerAngles.Set(0f, 180f, 0f);
		GhostPart ghostPart4 = new GhostPart(gameObject.transform.Find("Parts/CarTireRear").gameObject, new Vector3(0f, 0f, -23f), "rearWheel");
		GhostPart ghostPart5 = new GhostPart(gameObject.transform.Find("Parts/CarTireRear").gameObject, new Vector3(0f, 0f, 23f), "rearWheel");
		ghostPart5.prefab.transform.rotation.eulerAngles.Set(0f, 180f, 0f);
		_ghost.CreateVisuals(new GhostPart[5] { ghostPart, ghostPart2, ghostPart3, ghostPart4, ghostPart5 });
	}

	public void CreateMotorcyclePlaybackGhostEntity(GhostObject _ghost)
	{
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.DirtBikeGhostNewPrefab_GameObject);
		GhostPart ghostPart = new GhostPart(gameObject.transform.Find("DirtBike").gameObject, new Vector3(0f, -3f, 0f), "chassis");
		GhostPart ghostPart2 = new GhostPart(gameObject.transform.Find("Parts/Eturengas").gameObject, Vector3.zero, "frontWheel");
		GhostPart ghostPart3 = new GhostPart(gameObject.transform.Find("Parts/Takarengas").gameObject, Vector3.zero, "rearWheel");
		_ghost.CreateVisuals(new GhostPart[3] { ghostPart, ghostPart2, ghostPart3 });
	}

	public void GenerateCollectableCoins()
	{
		if (m_playbackGhosts == null || m_playbackGhosts.Count == 0)
		{
			return;
		}
		Ghost ghost = m_playbackGhosts[m_playbackGhosts.Count - 1].m_ghost;
		m_streaks = new List<PsStreak>();
		UnityEngine.Random.seed = (int)Main.m_gameTimeSinceAppStarted;
		float pathWorldDistance = ghost.GetPathWorldDistance("chassis");
		int num = ToolBox.limitBetween((int)(pathWorldDistance / 1000f), 1, int.MaxValue);
		int num2 = ToolBox.limitBetween(UnityEngine.Random.Range(num / 2, num + 1), 1, int.MaxValue);
		int min = 3;
		int num3 = 10;
		float num4 = 50f;
		float num5 = num4 * num4 * 0.8f;
		float num6 = 640000f;
		int num7 = 6;
		List<Entity> entitiesByTag = EntityManager.GetEntitiesByTag("CollectibleStar");
		List<UnitC> list = new List<UnitC>();
		for (int i = 0; i < entitiesByTag.Count; i++)
		{
			list.Add(EntityManager.GetComponentByEntity((ComponentType)30, entitiesByTag[i]) as UnitC);
		}
		GhostNode ghostNode = ghost.m_nodes["chassis"] as GhostNode;
		int num8 = ghost.m_keyframeCount / num2;
		int num9 = 0;
		if (num8 < 1)
		{
			return;
		}
		for (int j = 0; j < num2; j++)
		{
			int num10 = UnityEngine.Random.Range(min, num3 + 1);
			int val = num9 + UnityEngine.Random.Range(0, num8 + 1);
			val = ToolBox.limitBetween(val, 0, ghost.m_keyframeCount - 1);
			List<PsCoin> list2 = new List<PsCoin>();
			PsStreak psStreak = new PsStreak();
			for (int k = 0; k < num10; k++)
			{
				Vector3 vector = new Vector3(0f, UnityEngine.Random.Range(20, 40), 0f);
				vector = Quaternion.AngleAxis(ghostNode.GetKeyFrameRotation(val), Vector3.forward) * vector;
				Vector3 vector2 = new Vector2(vector.x, vector.y);
				Vector3 vector3 = ghostNode.GetKeyFramePos(val) + vector2;
				bool flag = true;
				int num11 = 0;
				foreach (PsCoin item in list2)
				{
					Vector3 vector4 = item.m_pos;
					float sqrMagnitude = (vector3 - vector4).sqrMagnitude;
					if (sqrMagnitude < num6)
					{
						num11++;
						if (num11 > num7)
						{
							flag = false;
							break;
						}
					}
					if (sqrMagnitude < num5)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					foreach (PsStreak streak in m_streaks)
					{
						foreach (PsCoin coin in streak.m_coins)
						{
							Vector3 vector5 = coin.m_pos;
							float sqrMagnitude2 = (vector3 - vector5).sqrMagnitude;
							if (sqrMagnitude2 < num6)
							{
								num11++;
								if (num11 > num7)
								{
									flag = false;
									break;
								}
							}
							if (sqrMagnitude2 < num5)
							{
								flag = false;
								break;
							}
						}
						if (!flag)
						{
							break;
						}
					}
				}
				if (flag)
				{
					foreach (UnitC item2 in list)
					{
						CollectibleStar collectibleStar = item2.m_unit as CollectibleStar;
						if ((vector3 - collectibleStar.m_pos).sqrMagnitude < num5 * 3f)
						{
							flag = false;
							break;
						}
					}
				}
				if (flag && (vector3 - PsState.m_activeMinigame.m_playerUnit.GetUnitCenterPosition()).sqrMagnitude < num5 * 5f)
				{
					flag = false;
				}
				if (flag)
				{
					list2.Add(new PsCoin(vector3, psStreak, list2.Count));
				}
				val = ghost.GetNextKeyframe("chassis", val, num4);
				if (val >= ghost.m_keyframeCount - 1)
				{
					break;
				}
			}
			psStreak.SetCoins(list2);
			m_streaks.Add(psStreak);
			num9 += num8;
		}
	}

	public void PlaceCoins()
	{
		if (m_streaks == null)
		{
			return;
		}
		for (int i = 0; i < m_streaks.Count; i++)
		{
			for (int j = 0; j < m_streaks[i].m_coins.Count; j++)
			{
				new CollectibleCoin(m_streaks[i].m_coins[j]);
			}
		}
	}

	public void CheckAchievementsOnWin()
	{
		if (m_gameLoop.GetDifficulty() == PsGameDifficulty.Tricky)
		{
			PsAchievementManager.Complete("finishHardLevel");
			if (PsState.m_activeMinigame.m_gameStartCount == 1)
			{
				PsAchievementManager.Complete("finishHardLevelFirstTry");
			}
		}
		else if (m_gameLoop.GetDifficulty() == PsGameDifficulty.Insane)
		{
			PsAchievementManager.Complete("dealWithIt");
			PsAchievementManager.IncrementProgress("epicDealWithIt", 1);
		}
		else if (m_gameLoop.GetDifficulty() == PsGameDifficulty.Impossible)
		{
			PsAchievementManager.Complete("impossibru");
		}
	}

	public void CheckAchievementsOnEnd()
	{
		if (m_gameLoop.m_context == PsMinigameContext.Fresh)
		{
			PsAchievementManager.IncrementProgress("playRateThirty", 1);
		}
		else if (m_gameLoop.m_context == PsMinigameContext.Social)
		{
			PsAchievementManager.Complete("testFriendLevels");
		}
	}
}
