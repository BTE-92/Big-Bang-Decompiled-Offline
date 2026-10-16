using System;
using Server;
using UnityEngine;

public class PsUIFriendlyNewBanner : PsUIVersusBanner
{
	private PsUIGenericButton m_decline;

	private PsUIWaitPopup m_waitPopup;

	public PsUIFriendlyNewBanner(UIComponent _parent, PsGameLoopVersus _loop, Action _clickAction = null)
		: base(_parent, _loop, _clickAction)
	{
	}

	public override void CreateRoundInfo(UIComponent _parent)
	{
		string text = "New Challenge";
		string color = "ffffff";
		UICanvas uICanvas = new UICanvas(_parent, false, string.Empty, null, string.Empty);
		uICanvas.SetVerticalAlign(1f);
		uICanvas.SetMargins(0f, 0f, -0.01f, 0.01f);
		uICanvas.SetHeight(0.08f, RelativeTo.ScreenHeight);
		uICanvas.RemoveDrawHandler();
		UIText uIText = new UIText(uICanvas, false, string.Empty, text, "KGSecondChances_Font", 0.04f, RelativeTo.ScreenHeight, color, "000000");
		uIText.SetVerticalAlign(0f);
		uIText.SetShadowShift(new Vector2(1f, -0.5f), 0.05f);
	}

	public override void CreateLowerInfo(UIComponent _parent)
	{
	}

	public override void CreateOwnInfo(UIComponent _parent)
	{
		base.CreateOwnInfo(_parent);
		m_proceed = new PsUIGenericButton(_parent, 0.25f, 0.25f, 0.008f);
		m_proceed.SetBlueColors();
		m_proceed.SetVerticalAlign(0f);
		m_proceed.SetHorizontalAlign(0f);
		m_proceed.SetDepthOffset(-5f);
		UIVerticalList uIVerticalList = new UIVerticalList(m_proceed, string.Empty);
		uIVerticalList.SetWidth(0.285f, RelativeTo.ScreenHeight);
		uIVerticalList.RemoveDrawHandler();
		UIText uIText = new UIText(uIVerticalList, false, string.Empty, "Accept!", "KGSecondChances_Font", 0.04f, RelativeTo.ScreenHeight, m_proceed.m_textColor);
		m_timeLeft = new UIText(uIVerticalList, false, string.Empty, "Time left: " + PsMetagameManager.GetTimeStringFromSeconds(m_loop.SecondsLeft()), "KGSecondChances_Font", 0.025f, RelativeTo.ScreenHeight);
		m_decline = new PsUIGenericButton(_parent, 0.25f, 0.25f, 0.008f);
		m_decline.SetRedColors();
		m_decline.SetVerticalAlign(0f);
		m_decline.SetHorizontalAlign(1f);
		m_decline.SetDepthOffset(-5f);
		m_decline.SetText("Decline!");
	}

	public override void Step()
	{
		if (m_proceed.m_hit)
		{
			JoinGame();
		}
		else if (m_decline.m_hit)
		{
			DeclineGame();
		}
		base.Step();
	}

	protected void JoinGame()
	{
		Friendly.Join(m_loop.m_versusData.timedEventId, JoinOk, JoinFailed);
		PsAchievementManager.Complete("challengeAccepted");
		m_waitPopup = new PsUIWaitPopup();
	}

	private void JoinOk(VersusMetaData _versusData)
	{
		DestroyWaitPopup();
		m_loop.m_versusData = _versusData;
		m_loop.StartLoop();
	}

	private void JoinFailed(HttpC _c)
	{
		DestroyWaitPopup();
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), delegate
		{
			HttpC httpC = Friendly.Join((string)_c.objectData, JoinOk, JoinFailed);
			httpC.objectData = _c.objectData;
			return httpC;
		});
	}

	protected void DeclineGame()
	{
		Friendly.Decline(m_loop.m_versusData.timedEventId, DeclineOk, DeclineFailed);
		m_waitPopup = new PsUIWaitPopup();
	}

	private void DeclineOk(VersusMetaData _versusData)
	{
		DestroyWaitPopup();
		Debug.LogWarning("Declined successfully, refreshing");
		m_clickAction();
	}

	private void DeclineFailed(HttpC _c)
	{
		DestroyWaitPopup();
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), delegate
		{
			HttpC httpC = Friendly.Decline((string)_c.objectData, DeclineOk, DeclineFailed);
			httpC.objectData = _c.objectData;
			return httpC;
		});
	}

	private void DestroyWaitPopup()
	{
		if (m_waitPopup != null)
		{
			m_waitPopup.Destroy();
			m_waitPopup = null;
		}
	}
}
