using System;
using System.Collections.Generic;

namespace Server
{
	public static class TimedEvents
	{
		private struct ChallengeEndInfo
		{
			public VersusMetaData data;

			public bool winRound;
		}

		private static bool m_isSpaceEntity;

		private static Entity m_loaderEntity;

		private static int m_versusCount;

		private static Action m_callback;

		public static bool m_loading = false;

		private static Action m_finishedLoading = null;

		public static Action m_subscribers = delegate
		{
			Debug.Log("TimedEvents loaded");
		};

		public static void Initialize(Action _callback, Entity _e = null)
		{
			if (_e != null)
			{
				m_isSpaceEntity = true;
				m_loaderEntity = _e;
			}
			else
			{
				m_isSpaceEntity = false;
				m_loaderEntity = EntityManager.AddEntity();
			}
			if (_callback == null)
			{
				_callback = delegate
				{
					Debug.Log("No callback.");
				};
			}
			m_callback = _callback;
			Load(_callback);
		}

		public static void SubscribeToLoadFinished(Action _a)
		{
			m_subscribers = (Action)Delegate.Combine(m_subscribers, _a);
		}

		public static void UnsubscribeToLoadFinished(Action _a)
		{
			m_subscribers = (Action)Delegate.Remove(m_subscribers, _a);
		}

		private static void Load(Action _callback)
		{
			m_loading = true;
			HttpC timedEvents = Metagame.GetTimedEvents(delegate(HttpC c)
			{
				TimedEventLoadOk(c, _callback);
			}, delegate(HttpC c)
			{
				TimedEventLoadFail(c, _callback);
			});
			EntityManager.AddComponentToEntity(m_loaderEntity, timedEvents);
		}

		private static void LoadFresh(Action _callback)
		{
			if (m_loaderEntity != null)
			{
				HttpC fresh = MiniGame.GetFresh(delegate(PsMinigameMetaData meta)
				{
					FreshLoadOk(meta, _callback);
				}, delegate(HttpC c)
				{
					FreshLoadFail(c, _callback);
				});
				EntityManager.AddComponentToEntity(m_loaderEntity, fresh);
			}
		}

		private static void FreshLoadOk(PsMinigameMetaData _meta, Action _callback)
		{
			if (m_loaderEntity != null && PsPlanetManager.m_timedEvents != null)
			{
				if (_meta != null && !PsPlanetManager.m_timedEvents.freshAndFree.Contains(_meta))
				{
					PsPlanetManager.m_timedEvents.freshAndFree.Add(_meta);
				}
				if (m_versusCount <= 0)
				{
					_callback();
				}
				if (!m_isSpaceEntity)
				{
					EntityManager.RemoveEntity(m_loaderEntity);
				}
				m_loaderEntity = null;
			}
		}

		private static void FreshLoadFail(HttpC _c, Action _callback)
		{
			LoadFresh(_callback);
		}

		private static void TimedEventLoadOk(HttpC _c, Action _callback)
		{
			PsPlanetManager.m_timedEvents = ClientTools.ParseTimedEvents(_c);
			m_loading = false;
			EndDueOwnTurnChallenges();
		}

		private static void TimedEventLoadFail(HttpC _c, Action _callback)
		{
			Load(_callback);
		}

		private static void EndDueOwnTurnChallenges()
		{
			List<VersusMetaData> versusChallenges = PsPlanetManager.m_timedEvents.versusChallenges;
			for (int num = versusChallenges.Count - 1; num >= 0; num--)
			{
				if (string.IsNullOrEmpty(versusChallenges[num].winner) && versusChallenges[num].currentPlayer == PlayerPrefsX.GetUserId() && versusChallenges[num].GetOwnInfo().tries >= PsState.m_versusChallengeTryAmount)
				{
					if (versusChallenges[num].GetOpponentInfo() == null && versusChallenges[num].GetOwnInfo().score.time > 0)
					{
						HttpC httpC = Versus.Quit(versusChallenges[num].timedEventId, versusChallenges[num].gameId, 0, true, false, VersusEndSUCCEED, VersusQuitFAILED);
						httpC.objectData = new ChallengeEndInfo
						{
							data = versusChallenges[num],
							winRound = true
						};
						m_versusCount++;
					}
					else
					{
						HttpC httpC2 = Versus.Quit(versusChallenges[num].timedEventId, versusChallenges[num].gameId, 0, false, true, VersusEndSUCCEED, VersusQuitFAILED);
						httpC2.objectData = new ChallengeEndInfo
						{
							data = versusChallenges[num],
							winRound = false
						};
						m_versusCount++;
					}
					Debug.LogError("VersusQuit send on id: " + versusChallenges[num].timedEventId);
				}
			}
			versusChallenges = PsPlanetManager.m_timedEvents.friendlyChallenges;
			for (int num2 = versusChallenges.Count - 1; num2 >= 0; num2--)
			{
				if (string.IsNullOrEmpty(versusChallenges[num2].winner) && versusChallenges[num2].currentPlayer == PlayerPrefsX.GetUserId() && versusChallenges[num2].GetOwnInfo() != null && versusChallenges[num2].GetOwnInfo().tries >= PsState.m_versusChallengeTryAmount)
				{
					if (versusChallenges[num2].GetOpponentInfo() == null && versusChallenges[num2].GetOwnInfo() != null && versusChallenges[num2].GetOwnInfo().score.time > 0)
					{
						HttpC httpC3 = Friendly.Quit(versusChallenges[num2].timedEventId, versusChallenges[num2].gameId, 0, true, false, VersusEndSUCCEED, VersusQuitFAILED, string.Empty);
						httpC3.objectData = new ChallengeEndInfo
						{
							data = versusChallenges[num2],
							winRound = true
						};
						m_versusCount++;
					}
					else
					{
						HttpC httpC4 = Friendly.Quit(versusChallenges[num2].timedEventId, versusChallenges[num2].gameId, 0, false, true, VersusEndSUCCEED, VersusQuitFAILED, string.Empty);
						httpC4.objectData = new ChallengeEndInfo
						{
							data = versusChallenges[num2],
							winRound = false
						};
						m_versusCount++;
					}
				}
			}
			if (m_versusCount <= 0)
			{
				m_callback();
			}
		}

		private static void VersusEndSUCCEED(HttpC _c)
		{
			Debug.LogWarning("VERSUS END SUCCEED");
			ChallengeEndInfo challengeEndInfo = (ChallengeEndInfo)_c.objectData;
			m_versusCount--;
			if (m_versusCount <= 0)
			{
				m_callback();
			}
		}

		private static void VersusQuitFAILED(HttpC _c)
		{
			Debug.LogWarning("VERSUS QUIT FAILED");
			ChallengeEndInfo challengeEndInfo = (ChallengeEndInfo)_c.objectData;
			HttpC httpC = Versus.Quit(challengeEndInfo.data.timedEventId, challengeEndInfo.data.gameId, 0, challengeEndInfo.winRound, !challengeEndInfo.winRound, VersusEndSUCCEED, VersusQuitFAILED);
			httpC.objectData = challengeEndInfo;
		}
	}
}
