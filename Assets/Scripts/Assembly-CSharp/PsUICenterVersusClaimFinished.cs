using System;
using System.Collections.Generic;
using Server;
using UnityEngine;

public class PsUICenterVersusClaimFinished : UICanvas
{
	public PsUIProfileShield m_shield;

	public UIVerticalList m_versusUIList;

	public UIVerticalList m_contentList;

	public PsUIGenericButton m_proceedButton;

	public bool m_claiming;

	public int m_claimedCount;

	public int m_oldCups;

	public int m_oldRank;

	public PsUICenterVersusClaimFinished(UIComponent _parent)
		: base(_parent, false, "ClaimScreen", null, string.Empty)
	{
		GetRoot().SetDrawHandler(PsUIDrawHandlers.DebriefBackground);
		RemoveDrawHandler();
		SetMargins(0.05f, 0.05f, 0.05f, 0.05f);
		m_proceedButton = new PsUIGenericButton(this);
		m_proceedButton.SetText("Claim");
		m_proceedButton.SetAlign(1f, 0f);
		m_contentList = new UIVerticalList(this, "ContentList");
		m_contentList.SetVerticalAlign(1f);
		m_contentList.SetSpacing(0.03f, RelativeTo.ScreenHeight);
		m_contentList.RemoveDrawHandler();
		CreateProflieShield(m_contentList);
		m_versusUIList = new UIVerticalList(m_contentList, "VersusList");
		m_versusUIList.SetVerticalAlign(1f);
		m_versusUIList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		m_versusUIList.RemoveDrawHandler();
		AddFinished(m_versusUIList);
	}

	public virtual void CreateProflieShield(UIComponent _parent)
	{
		m_shield = new PsUIProfileShield(m_contentList, false);
		m_shield.RemoveDrawHandler();
		m_shield.SetHeight(0.42f, RelativeTo.ScreenHeight);
		m_shield.SetMargins(0f, 0f, 0.05f, 0.02f);
	}

	public virtual void AddFinished(UIComponent _parent)
	{
		List<VersusMetaData> versusChallenges = PsPlanetManager.m_timedEvents.versusChallenges;
		for (int i = 0; i < versusChallenges.Count; i++)
		{
			if (!string.IsNullOrEmpty(versusChallenges[i].winner) || (string.IsNullOrEmpty(versusChallenges[i].winner) && versusChallenges[i].currentPlayer == PlayerPrefsX.GetUserId() && versusChallenges[i].GetOpponentInfo() == null && versusChallenges[i].GetOwnInfo().score.time == 0 && versusChallenges[i].GetOwnInfo().tries >= PsState.m_versusChallengeTryAmount))
			{
				VersusMetaData versusMetaData = versusChallenges[i];
				bool win = false;
				if (versusMetaData.winner == PlayerPrefsX.GetUserId())
				{
					win = true;
				}
				PsMetagameManager.VersusChallengeRewards(win);
				Versus.Claim(versusChallenges[i].timedEventId, ClaimSUCCEED, ClaimFAILED);
				PsGameLoopVersus loop = new PsGameLoopVersus(PsMinigameContext.Versus, versusMetaData, versusMetaData.timedEventId, versusMetaData.gameId, null, 1, -1, 0, versusMetaData.timeLeft, true);
				PsUIVersusClaimBanner psUIVersusClaimBanner = new PsUIVersusClaimBanner(m_versusUIList, loop);
			}
		}
	}

	public override void Step()
	{
		if (m_proceedButton != null && m_proceedButton.m_hit && !m_claiming)
		{
			m_claiming = true;
			ClaimOne();
		}
		base.Step();
	}

	public void ClaimOne()
	{
		m_proceedButton.SetGrayColors();
		m_proceedButton.Update();
		Action callback = delegate
		{
			UIComponent versus = m_versusUIList.m_childs[0];
			float versusHeight = m_versusUIList.m_childs[0].m_actualHeight + m_versusUIList.m_actualSpacing;
			Vector3 localPosition = versus.m_TC.transform.localPosition;
			Vector3 endValue = localPosition + new Vector3(-770f, 0f, 0f);
			TweenC tweenComponent = TweenS.AddTransformTween(versus.m_TC, TweenedProperty.Position, TweenStyle.QuadIn, localPosition, endValue, 0.5f, 0f, false);
			TweenS.AddTweenEndEventListener(tweenComponent, delegate(TweenC _c)
			{
				TweenS.RemoveComponent(_c);
				m_versusUIList.m_childs.RemoveAt(0);
				versus.Destroy();
				float actualHeight = m_versusUIList.m_actualHeight;
				Vector3 listPos = m_versusUIList.m_TC.transform.localPosition;
				TimerC timerC = TimerS.AddComponent(EntityManager.AddEntity(), string.Empty, 0f, 0.03f, true, delegate
				{
					if (m_versusUIList.m_childs.Count == 0)
					{
						(GetRoot() as PsUIBasePopup).CallAction("Exit");
					}
					else
					{
						float actualHeight2 = m_versusUIList.m_actualHeight;
						Vector3 endValue2 = listPos + new Vector3(0f, versusHeight, 0f);
						TweenC tweenComponent2 = TweenS.AddTransformTween(m_versusUIList.m_TC, TweenedProperty.Position, TweenStyle.QuadInOut, endValue2, 0.5f, 0f, false);
						TweenS.AddTweenEndEventListener(tweenComponent2, delegate(TweenC c)
						{
							TweenS.RemoveComponent(c);
							m_claiming = false;
							m_proceedButton.SetBlueColors();
							m_proceedButton.Update();
						});
					}
				});
			});
		};
		if ((m_versusUIList.m_childs[0] as PsUIVersusClaimBanner).m_loop.m_versusData.winner == PlayerPrefsX.GetUserId())
		{
			m_shield.Win(callback);
		}
		else
		{
			m_shield.Lose(callback);
		}
	}

	public void ClaimSUCCEED(HttpC _c)
	{
		Debug.Log("Claim succeed");
	}

	public void ClaimFAILED(HttpC _c)
	{
		string networkError = ServerErrors.GetNetworkError(_c.www.error);
		ServerManager.ThrowServerErrorException("Connection error", networkError, delegate
		{
			HttpC httpC = Versus.Claim((string)_c.objectData, ClaimSUCCEED, ClaimFAILED);
			httpC.objectData = _c.objectData;
			return httpC;
		});
	}
}
