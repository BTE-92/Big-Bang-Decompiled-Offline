using System.Collections.Generic;
using Server;

public class PsUIVersusRaceList : UIVerticalList
{
	private bool m_created;

	private PsUIGenericButton m_newChallenge;

	private VersusMetaData m_versusData;

	private PsUIBasePopup m_waitingPopup;

	public PsUIVersusRaceList(UIComponent _parent)
		: base(_parent, string.Empty)
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetSpacing(0.05f, RelativeTo.ScreenHeight);
		SetMargins(0.025f, 0.025f, 0.1125f, 0.05f);
		SetVerticalAlign(1f);
		RemoveDrawHandler();
		new PsUILoadingAnimation(this, false);
		m_parent.Update();
		CreateContent();
		TimedEvents.SubscribeToLoadFinished(CreateContent);
	}

	private void CreateContent()
	{
		if (PsPlanetManager.m_timedEvents != null)
		{
			DestroyChildren();
			if (true)
			{
				m_newChallenge = new PsUIGenericButton(this, 0.25f, 0.25f, 0.008f);
				m_newChallenge.SetText("New Versus");
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
					PsUIVersusOwnTurnBanner psUIVersusOwnTurnBanner = new PsUIVersusOwnTurnBanner(this, loop);
				}
				else if (versusMetaData.currentPlayer != PlayerPrefsX.GetUserId() && versusMetaData.GetOpponentInfo() != null && string.IsNullOrEmpty(versusMetaData.winner))
				{
					PsUIVersusOpponentBanner psUIVersusOpponentBanner = new PsUIVersusOpponentBanner(this, loop);
				}
				else if (versusMetaData.GetOpponentInfo() == null && string.IsNullOrEmpty(versusMetaData.winner))
				{
					PsUIVersusWaitingBanner psUIVersusWaitingBanner = new PsUIVersusWaitingBanner(this, loop);
				}
			}
			if (versusChallenges.Count == 0)
			{
				UITextbox uITextbox = new UITextbox(this, false, string.Empty, "Find and start new versus races from planet path.", "HurmeSemiBold_Font", 0.04f, RelativeTo.ScreenHeight, false, Align.Center);
				uITextbox.SetWidth(1f, RelativeTo.ParentWidth);
				uITextbox.SetHeight(1f, RelativeTo.ParentHeight);
			}
			List<VersusMetaData> friendlyChallenges = PsPlanetManager.m_timedEvents.friendlyChallenges;
			new UIText(this, false, string.Empty, "Friend Challenges", "KGSecondChances_Font", 0.04f, RelativeTo.ScreenHeight);
			for (int j = 0; j < friendlyChallenges.Count; j++)
			{
				VersusMetaData versusMetaData2 = friendlyChallenges[j];
				PsGameLoopFriendly loop2 = new PsGameLoopFriendly(PsMinigameContext.Friendly, versusMetaData2, versusMetaData2.timedEventId, versusMetaData2.gameId, null, 1, -1, 0, versusMetaData2.timeLeft, true);
				if (versusMetaData2.status == "waiting" && versusMetaData2.challengedPlayer == PlayerPrefsX.GetUserId() && !string.IsNullOrEmpty(versusMetaData2.currentPlayer) && versusMetaData2.currentPlayer == versusMetaData2.GetOpponentInfo().score.playerId)
				{
					continue;
				}
				if (versusMetaData2.challengedPlayer == PlayerPrefsX.GetUserId() && versusMetaData2.participants.Length < 2 && versusMetaData2.status != "declined")
				{
					PsUIFriendlyNewBanner psUIFriendlyNewBanner = new PsUIFriendlyNewBanner(this, loop2, Refresh);
				}
				else if (versusMetaData2.currentPlayer == PlayerPrefsX.GetUserId() && versusMetaData2.GetOwnInfo().tries < PsState.m_versusChallengeTryAmount && string.IsNullOrEmpty(versusMetaData2.winner))
				{
					PsUIVersusOwnTurnBanner psUIVersusOwnTurnBanner2 = new PsUIVersusOwnTurnBanner(this, loop2);
				}
				else if (versusMetaData2.currentPlayer != PlayerPrefsX.GetUserId() && versusMetaData2.GetOpponentInfo() != null && string.IsNullOrEmpty(versusMetaData2.winner))
				{
					PsUIVersusOpponentBanner psUIVersusOpponentBanner2 = new PsUIVersusOpponentBanner(this, loop2);
				}
				else if (versusMetaData2.GetOpponentInfo() == null && string.IsNullOrEmpty(versusMetaData2.winner))
				{
					if (versusMetaData2.status == "declined")
					{
						PsUIFriendlyOpponentDeclinedBanner psUIFriendlyOpponentDeclinedBanner = new PsUIFriendlyOpponentDeclinedBanner(this, loop2, Refresh);
					}
					else
					{
						PsUIVersusWaitingBanner psUIVersusWaitingBanner2 = new PsUIVersusWaitingBanner(this, loop2);
					}
				}
				else if (!string.IsNullOrEmpty(versusMetaData2.winner))
				{
					PsUIFriendlyClaimableBanner psUIFriendlyClaimableBanner = new PsUIFriendlyClaimableBanner(this, loop2, Refresh);
				}
			}
			CreateFollowerList(this);
			m_created = true;
		}
		else
		{
			new PsUILoadingAnimation(this, true);
		}
		m_parent.Update();
	}

	private void CreateFollowerList(UIComponent _parent)
	{
		new PsUIFriendList(_parent);
	}

	protected void Refresh()
	{
		DestroyChildren();
		PsPlanetManager.m_timedEvents = null;
		new PsUILoadingAnimation(this, false);
		m_parent.Update();
		m_created = false;
		TimedEvents.Initialize(null);
	}

	public override void Step()
	{
		if (PsPlanetManager.m_timedEvents != null && !m_created)
		{
			CreateContent();
		}
		if (m_newChallenge != null && m_newChallenge.m_hit)
		{
			CreateVersus();
		}
		base.Step();
	}

	public override void Destroy()
	{
		TimedEvents.UnsubscribeToLoadFinished(CreateContent);
		base.Destroy();
	}

	private void CreateVersus()
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
		SwitchToVersusChallenge();
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
