using System.Collections.Generic;
using Server;

public class PsUIChallengeFriendList : UIScrollableCanvas
{
	public bool m_started;

	private PsUIGenericButton m_exit;

	private UIVerticalList m_friendList;

	private List<PsUIGenericButton> m_friends;

	private PsUIGenericButton m_fbButton;

	private string m_challengedFriendId;

	private PsUIWaitPopup m_waitingPopup;

	public PsUIChallengeFriendList(UIComponent _parent)
		: base(_parent, "FriendPvpList")
	{
		RemoveDrawHandler();
		SetMargins(0.04f);
		m_friends = new List<PsUIGenericButton>();
		m_friendList = new UIVerticalList(this, "FriendList");
		m_friendList.SetVerticalAlign(1f);
		m_friendList.SetSpacing(0.015f, RelativeTo.ScreenHeight);
		m_friendList.RemoveDrawHandler();
		LoadContent();
	}

	public void LoadContent(bool _forceRefresh = false)
	{
		if (string.IsNullOrEmpty("x"))
		{
			m_fbButton = new PsUIGenericButton(m_friendList);
			m_fbButton.SetIcon("menu_icon_facebook", 0.044999998f);
			m_fbButton.SetText("Connect to Facebook");
		}
		new PsUILoadingAnimation(m_friendList, false);
		Update();
		Debug.Log("Get friends");
		PsMetagameManager.GetFriends(CreateContent, _forceRefresh);
	}

	public override void Step()
	{
		if (!m_started)
		{
			if (m_exit != null && m_exit.m_hit)
			{
				(GetRoot() as PsUIBasePopup).CallAction("Exit");
			}
			else
			{
				m_friends.ForEach(delegate(PsUIGenericButton x)
				{
					if (x.m_hit)
					{
						StartFriendlyVersus(x.m_identifier);
					}
				});
			}
			if (m_fbButton != null && m_fbButton.m_hit && !m_started)
			{
				m_waitingPopup = new PsUIWaitPopup();
				FacebookManager.Login(FacebookLoginDone);
			}
		}
		base.Step();
	}

	private void FacebookLoginDone()
	{
		Debug.Log("Facebook login done");
		if (m_friendList != null && m_friendList.m_childs.Count > 0)
		{
			m_friendList.DestroyChildren();
			m_fbButton = null;
		}
		LoadContent(true);
		DestroyWaitingPopup();
	}

	public void CreateContent(Friends _friendData)
	{
		if (m_TC.p_entity == null)
		{
			return;
		}
		if (m_fbButton == null)
		{
			m_friendList.DestroyChildren(0);
		}
		else
		{
			m_friendList.DestroyChildren(1);
		}
		if (_friendData.friends.Count < 1)
		{
			string text = "No friends connected. " + ((!string.IsNullOrEmpty("x")) ? "Invite them to play!" : "Connect to Facebook!");
			new UIText(m_friendList, false, string.Empty, "No friends connected. Invite them to play!", "KGSecondChances_Font", 0.03f, RelativeTo.ScreenHeight);
		}
		else
		{
			for (int i = 0; i < _friendData.friends.Count; i++)
			{
				if (string.IsNullOrEmpty(_friendData.friends[i].name))
				{
					continue;
				}
				bool flag = false;
				if (PsPlanetManager.m_timedEvents != null)
				{
					for (int j = 0; j < PsPlanetManager.m_timedEvents.friendlyChallenges.Count; j++)
					{
						if (PsPlanetManager.m_timedEvents.friendlyChallenges[j].challengedPlayer == _friendData.friends[i].playerId)
						{
							flag = true;
						}
						else
						{
							for (int k = 0; k < PsPlanetManager.m_timedEvents.friendlyChallenges[j].participants.Length; k++)
							{
								if (PsPlanetManager.m_timedEvents.friendlyChallenges[j].participants[k].score.playerId == _friendData.friends[i].playerId)
								{
									flag = true;
								}
							}
						}
						if (flag)
						{
							break;
						}
					}
				}
				PsUIGenericButton psUIGenericButton = new PsUIGenericButton(m_friendList);
				psUIGenericButton.SetSpacing(0.0675f, RelativeTo.OwnHeight);
				PsUIProfileImage psUIProfileImage = new PsUIProfileImage(psUIGenericButton, false, "profilePic", _friendData.friends[i].facebookId, _friendData.friends[i].gameCenterId);
				psUIProfileImage.SetSize(0.06f, 0.06f, RelativeTo.ScreenHeight);
				psUIProfileImage.SetHorizontalAlign(0f);
				psUIGenericButton.SetText(_friendData.friends[i].name);
				if (flag)
				{
					psUIGenericButton.SetGrayColors();
					continue;
				}
				psUIGenericButton.m_identifier = _friendData.friends[i].playerId;
				m_friends.Add(psUIGenericButton);
			}
		}
		Update();
	}

	public void StartFriendlyVersus(string _opponentId)
	{
		m_challengedFriendId = _opponentId;
		Debug.Log("Identifier: " + _opponentId);
		if (!string.IsNullOrEmpty(_opponentId) && !m_started)
		{
			m_started = true;
			string minigameId = PsState.m_activeGameLoop.m_minigameId;
			m_waitingPopup = (m_waitingPopup = new PsUIWaitPopup());
			PsAchievementManager.Complete("challengeFriend");
			Friendly.Create(minigameId, _opponentId, FriendlyCreateOk, FriendlyCreateFailed, string.Empty);
		}
	}

	public void FriendlyCreateOk(VersusMetaData _metadata)
	{
		Debug.Log("New friendly versus: " + _metadata.timedEventId + " Participant_amount:" + _metadata.participants.Length);
		(GetRoot() as PsUIBasePopup).CallAction("Exit");
		DestroyWaitingPopup();
		PsState.m_activeGameLoop.SwitchToVersusChallenge(_metadata);
	}

	public void FriendlyCreateFailed(HttpC _c)
	{
		ServerManager.ThrowServerErrorException("Connection error", _c.www, delegate
		{
			HttpC httpC = Friendly.Create(PsState.m_activeGameLoop.m_minigameId, m_challengedFriendId, FriendlyCreateOk, FriendlyCreateFailed, string.Empty);
			httpC.objectData = _c.objectData;
			return httpC;
		});
	}

	public void DestroyWaitingPopup()
	{
		if (m_waitingPopup != null)
		{
			m_waitingPopup.Destroy();
			m_waitingPopup = null;
		}
	}
}
