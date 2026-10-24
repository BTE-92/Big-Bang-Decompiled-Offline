using System.Collections.Generic;
using Server;

public class PsUIFriendList : UIVerticalList
{
	private const float m_fbImageSize = 0.06f;

	private const float m_namePicSpacing = 0.025f;

	private bool m_created;

	private PsUIWaitPopup m_waitPopup;

	private PsUIBasePopup m_waitingPopup;

	private PsUIGenericButton m_fbButton;

	private List<PsUIGenericButton> m_addFriendButtons;

	private List<PsUIGenericButton> m_unfollowButtons;

	private bool m_alive = true;

	public PsUIFriendList(UIComponent _parent)
		: base(_parent, "Follower List")
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetSpacing(0.035f, RelativeTo.ScreenHeight);
		SetMargins(0.025f, 0.025f, 0.1125f, 0.05f);
		SetVerticalAlign(1f);
		SetWidth(0.3f, RelativeTo.ScreenHeight);
		RemoveDrawHandler();
		GetPlayerData(true);
	}

	public void GetPlayerData(bool _forceRefresh = false)
	{
		m_created = false;
		if (string.IsNullOrEmpty("x"))
		{
			m_fbButton = new PsUIGenericButton(this);
			m_fbButton.SetIcon("menu_icon_facebook", 0.045f);
			m_fbButton.SetText("Connect to Facebook");
		}
		new PsUILoadingAnimation(this, false);
		Update();
		PsMetagameManager.GetFriends(CreateContent, _forceRefresh);
	}

	public void CreateContent(Friends _friendData)
	{
		if (!m_alive)
		{
			return;
		}
		if (m_fbButton == null)
		{
			DestroyChildren(0);
		}
		else
		{
			DestroyChildren(1);
		}
		m_addFriendButtons = new List<PsUIGenericButton>();
		m_unfollowButtons = new List<PsUIGenericButton>();
		new UIText(this, false, string.Empty, "Friends", "KGSecondChances_Font", 0.05f, RelativeTo.ScreenHeight);
		if (_friendData.friends == null || _friendData.friends.Count < 1)
		{
			new UIText(this, false, string.Empty, "Invite your friends to play!", "KGSecondChances_Font", 0.03f, RelativeTo.ScreenHeight, "#d3d3d3");
		}
		else
		{
			for (int i = 0; i < _friendData.friends.Count; i++)
			{
				UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
				uIHorizontalList.RemoveDrawHandler();
				uIHorizontalList.SetHorizontalAlign(0f);
				uIHorizontalList.SetSpacing(0.025f, RelativeTo.ScreenHeight);
				PsUIProfileImage psUIProfileImage = new PsUIProfileImage(uIHorizontalList, false, string.Empty, _friendData.friends[i].facebookId, _friendData.friends[i].gameCenterId);
				psUIProfileImage.SetSize(0.06f, 0.06f, RelativeTo.ScreenHeight);
				UIText uIText = new UIText(uIHorizontalList, false, string.Empty, _friendData.friends[i].name, "KGSecondChances_Font", 0.03f, RelativeTo.ScreenHeight);
			}
		}
		new UIText(this, false, string.Empty, "\nYou follow", "KGSecondChances_Font", 0.05f, RelativeTo.ScreenHeight);
		if (_friendData.followees == null || _friendData.followees.Count < 1)
		{
			new UIText(this, false, string.Empty, "You are not following anyone yet.", "KGSecondChances_Font", 0.03f, RelativeTo.ScreenHeight, "#d3d3d3");
		}
		else
		{
			for (int j = 0; j < _friendData.followees.Count; j++)
			{
				UIHorizontalList uIHorizontalList2 = new UIHorizontalList(this, string.Empty);
				uIHorizontalList2.RemoveDrawHandler();
				uIHorizontalList2.SetSpacing(0.025f, RelativeTo.ScreenHeight);
				PsUIProfileImage psUIProfileImage2 = new PsUIProfileImage(uIHorizontalList2, false, string.Empty, _friendData.followees[j].facebookId, _friendData.followees[j].gameCenterId);
				psUIProfileImage2.SetSize(0.06f, 0.06f, RelativeTo.ScreenHeight);
				UIText uIText2 = new UIText(uIHorizontalList2, false, string.Empty, _friendData.followees[j].name, "KGSecondChances_Font", 0.03f, RelativeTo.ScreenHeight);
				PsUIGenericButton psUIGenericButton = new PsUIGenericButton(uIHorizontalList2);
				psUIGenericButton.SetText("Unfollow");
				psUIGenericButton.m_identifier = _friendData.followees[j].playerId;
				m_unfollowButtons.Add(psUIGenericButton);
			}
		}
		new UIText(this, false, string.Empty, "\nYour followers", "KGSecondChances_Font", 0.05f, RelativeTo.ScreenHeight);
		if (_friendData.followers == null || _friendData.followers.Count < 1)
		{
			new UIText(this, false, string.Empty, "No followers. Create great levels to get followers!", "KGSecondChances_Font", 0.03f, RelativeTo.ScreenHeight, "#d3d3d3");
		}
		else
		{
			for (int k = 0; k < _friendData.followers.Count; k++)
			{
				UIHorizontalList uIHorizontalList3 = new UIHorizontalList(this, string.Empty);
				uIHorizontalList3.RemoveDrawHandler();
				uIHorizontalList3.SetSpacing(0.025f, RelativeTo.ScreenHeight);
				PsUIProfileImage psUIProfileImage3 = new PsUIProfileImage(uIHorizontalList3, false, string.Empty, _friendData.followers[k].facebookId, _friendData.followers[k].gameCenterId);
				psUIProfileImage3.SetSize(0.06f, 0.06f, RelativeTo.ScreenHeight);
				UIText uIText3 = new UIText(uIHorizontalList3, false, string.Empty, _friendData.followers[k].name, "KGSecondChances_Font", 0.03f, RelativeTo.ScreenHeight);
				PsUIGenericButton psUIGenericButton2 = new PsUIGenericButton(uIHorizontalList3);
				psUIGenericButton2.SetText("Add friend");
				psUIGenericButton2.m_identifier = _friendData.followers[k].playerId;
				m_addFriendButtons.Add(psUIGenericButton2);
			}
		}
		Update();
		m_parent.Update();
		m_created = true;
	}

	public override void Step()
	{
		if (m_fbButton != null && m_fbButton.m_hit)
		{
			m_waitingPopup = new PsUIBasePopup(typeof(PsUIPopupFacebookWaiting), null, null, null, false);
			FacebookManager.Login(FacebookLoginDone);
		}
		if (m_created)
		{
			for (int i = 0; i < m_addFriendButtons.Count; i++)
			{
				if (m_addFriendButtons[i].m_hit)
				{
					string identifier = m_addFriendButtons[i].m_identifier;
					Follow(identifier);
				}
			}
			for (int j = 0; j < m_unfollowButtons.Count; j++)
			{
				if (m_unfollowButtons[j].m_hit)
				{
					string identifier2 = m_unfollowButtons[j].m_identifier;
					Unfollow(identifier2);
				}
			}
		}
		base.Step();
	}

	private void FacebookLoginDone()
	{
		Debug.Log("Facebook login done");
		DestroyChildren();
		m_fbButton = null;
		GetPlayerData(true);
		if (m_waitingPopup != null)
		{
			m_waitingPopup.Destroy();
			m_waitingPopup = null;
		}
	}

	private void Follow(string _id)
	{
		m_waitPopup = new PsUIWaitPopup();
		Player.Follow(_id, FollowOk, FollowFailed);
	}

	public void FollowOk(HttpC _c)
	{
		DestroyWaitPopup();
		DestroyChildren();
		GetPlayerData(true);
	}

	public void FollowFailed(HttpC _c)
	{
		DestroyWaitPopup();
	}

	private void Unfollow(string _id)
	{
		Debug.LogError("Unfollowing");
		m_waitPopup = new PsUIWaitPopup();
		Player.UnFollow(_id, FollowOk, FollowFailed);
	}

	private void DestroyWaitPopup()
	{
		if (m_waitPopup != null)
		{
			m_waitPopup.Destroy();
			m_waitPopup = null;
		}
	}

	public override void Destroy()
	{
		m_alive = false;
		base.Destroy();
	}
}
