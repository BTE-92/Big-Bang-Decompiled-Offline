using System.Collections.Generic;
using Server;
using UnityEngine;

public class PsUICenterCurrentVersusChallenges : UICanvas
{
	protected UIScrollableCanvas m_scrollArea;

	protected UIVerticalList m_contentArea;

	private PsUIGenericButton m_challenge;

	private PsUIBasePopup m_waitingPopup;

	private VersusMetaData m_versusData;

	private bool m_created;

	private bool m_resources;

	private bool m_tutorial;

	public PsUICenterCurrentVersusChallenges(UIComponent _parent)
		: base(_parent, false, string.Empty, null, string.Empty)
	{
		if (!string.IsNullOrEmpty(PsMetagameManager.m_tutorialKey) && PsMetagameManager.m_tutorialKey == "Versus")
		{
			m_tutorial = true;
			PsMetagameManager.m_tutorialKey = null;
		}
		PsMetagameManager.HideResources();
		m_resources = false;
		GetRoot().SetDrawHandler(PsUIDrawHandlers.DebriefBackground);
		SetWidth(0.75f, RelativeTo.ScreenWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
		SetVerticalAlign(0f);
		UIHorizontalList uIHorizontalList = _parent as UIHorizontalList;
		uIHorizontalList.SetSpacing(0.25f, RelativeTo.ScreenWidth);
		uIHorizontalList.SetMargins(0.125f, 0.125f, 0f, 0f, RelativeTo.ScreenWidth);
		float num = (float)Screen.height * 1.3f / (float)Screen.height;
		RemoveDrawHandler();
		m_scrollArea = new UIScrollableCanvas(this, string.Empty);
		m_scrollArea.SetHeight(1f - num * 0.85f, RelativeTo.ScreenHeight);
		m_scrollArea.SetVerticalAlign(0f);
		m_scrollArea.RemoveDrawHandler();
		m_scrollArea.m_passTouchesToScrollableParents = true;
		m_contentArea = new UIVerticalList(m_scrollArea, string.Empty);
		m_contentArea.SetWidth(1f, RelativeTo.ParentWidth);
		m_contentArea.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		m_contentArea.SetMargins(0.025f, 0.025f, 0.05f, 0.05f);
		m_contentArea.SetVerticalAlign(1f);
		m_contentArea.RemoveDrawHandler();
		CreateContent(m_contentArea);
		UIScrollableCanvas uIScrollableCanvas = new UIScrollableCanvas(this, "Header");
		uIScrollableCanvas.SetWidth(1.3f, RelativeTo.ScreenHeight);
		uIScrollableCanvas.SetHeight(num, RelativeTo.ScreenHeight);
		uIScrollableCanvas.SetVerticalAlign(1f);
		uIScrollableCanvas.RemoveTouchAreas();
		uIScrollableCanvas.RemoveDrawHandler();
	}

	public void ButtonBackgroundDrawhandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] arc = DebugDraw.GetArc(_c.m_actualHeight * 2f, 60, 180f, 0f, Vector2.zero);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 5f + Vector3.down, arc, (float)Screen.height * 0.2f, new Color(0f, 0.87f, 1f, 0.85f), ResourceManager.GetMaterial("Framework/Line8GradientMat"), _c.m_camera, Position.Inside, false);
	}

	public void CreateContent(UIComponent _parent)
	{
		if (PsPlanetManager.m_timedEvents == null)
		{
			return;
		}
		List<VersusMetaData> versusChallenges = PsPlanetManager.m_timedEvents.versusChallenges;
		VersusComparer comparer = new VersusComparer();
		versusChallenges.Sort(comparer);
		for (int i = 0; i < versusChallenges.Count; i++)
		{
			VersusMetaData versusMetaData = versusChallenges[i];
			PsGameLoopVersus loop = new PsGameLoopVersus(PsMinigameContext.Versus, versusMetaData, versusMetaData.timedEventId, versusMetaData.gameId, null, 1, -1, 0, versusMetaData.timeLeft, true);
			if (versusMetaData.currentPlayer == PlayerPrefsX.GetUserId() && versusMetaData.GetOwnInfo().tries < PsState.m_versusChallengeTryAmount && string.IsNullOrEmpty(versusMetaData.winner))
			{
				PsUIVersusOwnTurnBanner psUIVersusOwnTurnBanner = new PsUIVersusOwnTurnBanner(m_contentArea, loop);
			}
			else if (versusMetaData.currentPlayer != PlayerPrefsX.GetUserId() && versusMetaData.GetOpponentInfo() != null && string.IsNullOrEmpty(versusMetaData.winner))
			{
				PsUIVersusOpponentBanner psUIVersusOpponentBanner = new PsUIVersusOpponentBanner(m_contentArea, loop);
			}
			else if (versusMetaData.GetOpponentInfo() == null && string.IsNullOrEmpty(versusMetaData.winner))
			{
				PsUIVersusWaitingBanner psUIVersusWaitingBanner = new PsUIVersusWaitingBanner(m_contentArea, loop);
			}
		}
		m_created = true;
	}

	public override void Step()
	{
		if (m_tutorial)
		{
			m_tutorial = false;
			new PsUITutorialArrowUI(m_challenge);
		}
		PsUIBasePopup psUIBasePopup = GetRoot() as PsUIBasePopup;
		float currentScrollX = psUIBasePopup.m_scrollableCanvas.m_currentScrollX;
		if (!m_resources && currentScrollX > 0.35f && currentScrollX < 0.65f)
		{
			psUIBasePopup.RemovePageButtons();
			psUIBasePopup.CreatePageButtonLeft("Published\nLevels");
			psUIBasePopup.CreatePageButtonRight("Friend\nLevels");
			PsMetagameManager.HideResources();
			m_resources = true;
		}
		else if (m_resources && (currentScrollX < 0.15f || currentScrollX > 0.85f))
		{
			PsMetagameManager.HideResources();
			psUIBasePopup.RemovePageButtons();
			m_resources = false;
		}
		if (PsPlanetManager.m_timedEvents != null && !m_created)
		{
			CreateContent(m_scrollArea);
		}
		base.Step();
	}

	public void CreateVersusChallenge()
	{
		m_waitingPopup = new PsUIBasePopup(typeof(PsUIVersusWaitingPopup), null, null, null, false);
		string gameId = null;
		HttpC httpC = Versus.Create(CreateVersusChallengeSUCCEED, CreateVersusChallengeFAILED, gameId);
	}

	public void CreateVersusChallengeSUCCEED(VersusMetaData _versusData)
	{
		if (m_waitingPopup != null)
		{
			m_waitingPopup.Destroy();
			m_waitingPopup = null;
		}
		m_versusData = _versusData;
		if (m_versusData != null)
		{
			PsUIBasePopup psUIBasePopup = new PsUIBasePopup(typeof(PsUIVersusCreatePopup), null, null, null, false);
			psUIBasePopup.SetAction("SwitchLoop", SwitchToVersusChallenge);
			(psUIBasePopup.m_mainContent as PsUIVersusCreatePopup).m_versusData = m_versusData;
		}
	}

	public void CreateVersusChallengeFAILED(HttpC _c)
	{
		string networkError = ServerErrors.GetNetworkError(_c.www.error);
		ServerManager.ThrowServerErrorException("Connection error", networkError, () => Versus.Create(CreateVersusChallengeSUCCEED, CreateVersusChallengeFAILED));
	}

	public void SwitchToVersusChallenge()
	{
		PsGameLoopVersus psGameLoopVersus = new PsGameLoopVersus(PsMinigameContext.Versus, m_versusData, m_versusData.timedEventId, m_versusData.gameId, null, -1, -1, 0, m_versusData.timeLeft, true, m_versusData.timeLeft);
		PsMetagameManager.HideResources();
		psGameLoopVersus.StartLoop();
	}
}
