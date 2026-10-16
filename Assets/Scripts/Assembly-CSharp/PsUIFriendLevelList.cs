using System.Collections.Generic;
using Server;

public class PsUIFriendLevelList : UIVerticalList
{
	private bool m_created;

	private List<PsUICreateFolloweeBanner> m_levels;

	private PsUIGenericButton m_fbButton;

	private PsUIBasePopup m_waitingPopup;

	public PsUIFriendLevelList(UIComponent _parent)
		: base(_parent, string.Empty)
	{
		m_levels = new List<PsUICreateFolloweeBanner>();
		SetWidth(1f, RelativeTo.ParentWidth);
		SetSpacing(0.02f, RelativeTo.ScreenHeight);
		SetMargins(0.025f, 0.025f, 0.1125f, 0.05f);
		SetVerticalAlign(1f);
		RemoveDrawHandler();
		LoadContent();
	}

	public void LoadContent()
	{
		new PsUILoadingAnimation(this, false);
		m_parent.Update();
		HttpC followeeMinigames = MiniGame.GetFolloweeMinigames(DataSUCCEED, DataFAILED, 100);
		EntityManager.AddComponentToEntity(m_TC.p_entity, followeeMinigames);
	}

	private void DataSUCCEED(PsMinigameMetaData[] _data)
	{
		DestroyChildren();
		if (string.IsNullOrEmpty(PlayerPrefsX.GetFacebookId()))
		{
			m_fbButton = new PsUIGenericButton(this);
			m_fbButton.SetIcon("menu_icon_facebook", 0.045f);
			m_fbButton.SetText("Connect to Facebook");
		}
		for (int i = 0; i < _data.Length; i++)
		{
			PsGameLoopSocial gameLoop = new PsGameLoopSocial(_data[i].id, _data[i]);
			m_levels.Add(new PsUICreateFolloweeBanner(this, gameLoop));
		}
		m_parent.Update();
	}

	private void DataFAILED(HttpC _c)
	{
		Debug.Log("GET FOLLOWEE MINIGAMES FAILED");
		string networkError = ServerErrors.GetNetworkError(_c.www.error);
		ServerManager.ThrowServerErrorException("Connection error", networkError, delegate
		{
			HttpC followeeMinigames = MiniGame.GetFolloweeMinigames(DataSUCCEED, DataFAILED);
			EntityManager.AddComponentToEntity((GetRoot() as PsUIBasePopup).m_utilityEntity, followeeMinigames);
			return followeeMinigames;
		});
	}

	public override void Step()
	{
		if (m_fbButton != null && m_fbButton.m_hit)
		{
			m_waitingPopup = new PsUIBasePopup(typeof(PsUIPopupFacebookWaiting), null, null, null, false);
			FacebookManager.Login(FacebookLoginDone);
		}
		for (int i = 0; i < m_levels.Count; i++)
		{
			if (m_levels[i].m_button.m_hit)
			{
				PsMetagameManager.HideResources();
				m_levels[i].m_loop.StartLoop();
			}
		}
		base.Step();
	}

	public override void Destroy()
	{
		if (m_waitingPopup != null)
		{
			m_waitingPopup.Destroy();
			m_waitingPopup = null;
		}
		base.Destroy();
	}

	private void FacebookLoginDone()
	{
		Debug.Log("Facebook login done");
		LoadContent();
		if (m_waitingPopup != null)
		{
			m_waitingPopup.Destroy();
			m_waitingPopup = null;
		}
	}
}
