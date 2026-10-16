using System;
using Server;
using UnityEngine;

public class PsUIFriendlyOpponentDeclinedBanner : PsUIVersusBanner
{
	private PsUIWaitPopup m_waitPopup;

	public PsUIFriendlyOpponentDeclinedBanner(UIComponent _parent, PsGameLoopVersus _loop, Action _clickAction = null)
		: base(_parent, _loop, _clickAction)
	{
	}

	public override void CreateRoundInfo(UIComponent _parent)
	{
		string text = "Opponent\ndeclined!";
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
		m_proceed.SetDepthOffset(-5f);
		UIVerticalList uIVerticalList = new UIVerticalList(m_proceed, string.Empty);
		uIVerticalList.SetWidth(0.285f, RelativeTo.ScreenHeight);
		uIVerticalList.RemoveDrawHandler();
		UIText uIText = new UIText(uIVerticalList, false, string.Empty, "Okay", "KGSecondChances_Font", 0.04f, RelativeTo.ScreenHeight, m_proceed.m_textColor);
	}

	public override void Step()
	{
		if (m_proceed.m_hit)
		{
			Claim();
		}
		base.Step();
	}

	protected void Claim()
	{
		Friendly.Claim(m_loop.m_versusData.timedEventId, ClaimOk, ClaimFailed);
		m_waitPopup = new PsUIWaitPopup();
	}

	private void ClaimOk(HttpC _c)
	{
		DestroyWaitPopup();
		Debug.Log("Friendly race claim ok, id: " + m_loop.m_versusData.timedEventId);
		m_clickAction();
	}

	private void ClaimFailed(HttpC _c)
	{
		DestroyWaitPopup();
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), delegate
		{
			HttpC httpC = Friendly.Claim((string)_c.objectData, ClaimOk, ClaimFailed);
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
