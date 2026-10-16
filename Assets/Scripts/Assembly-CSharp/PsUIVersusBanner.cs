using System;
using UnityEngine;

public class PsUIVersusBanner : UICanvas
{
	public PsGameLoopVersus m_loop;

	public UIText m_timeLeft;

	public Frame m_bgFrame;

	public PsUIGenericButton m_proceed;

	public Action m_clickAction;

	public UIFittedSprite m_bg;

	public UIFittedSprite m_vs;

	public UIVerticalList m_own;

	public UIVerticalList m_opponent;

	public PsUIProfileImage m_opponentImage;

	public UIText m_opponentName;

	public PsUIVersusBanner(UIComponent _parent, PsGameLoopVersus _loop, Action _clickAction = null)
		: base(_parent, false, string.Empty, null, string.Empty)
	{
		Initialize();
		m_loop = _loop;
		m_clickAction = _clickAction;
		SetHeight(0.285f, RelativeTo.ScreenHeight);
		SetWidth(0.8f, RelativeTo.ScreenHeight);
		RemoveDrawHandler();
		m_bg = new UIFittedSprite(this, false, string.Empty, PsState.m_uiSheet, m_bgFrame);
		m_bg.SetHeight(0.175f, RelativeTo.ScreenHeight);
		m_bg.SetVerticalAlign(0.325f);
		m_vs = new UIFittedSprite(this, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("challenge_VS_logo"));
		m_vs.SetHeight(0.1f, RelativeTo.ScreenHeight);
		m_vs.SetVerticalAlign(0.625f);
		m_vs.SetDepthOffset(-5f);
		CreateRoundInfo(this);
		CreateOwnInfo(this);
		CreateLowerInfo(this);
		CreateOpponentInfo(this);
	}

	public virtual void Initialize()
	{
		m_bgFrame = PsState.m_uiSheet.m_atlas.GetFrame("challenge_banner");
	}

	public virtual void CreateRoundInfo(UIComponent _parent)
	{
		UIText uIText = new UIText(_parent, false, string.Empty, "Round " + m_loop.m_versusData.round, "KGSecondChances_Font", 0.04f, RelativeTo.ScreenHeight, null, "000000");
		uIText.SetVerticalAlign(0.98f);
		uIText.SetShadowShift(new Vector2(1f, -0.5f), 0.05f);
	}

	public virtual void CreateOwnInfo(UIComponent _parent)
	{
		UICanvas uICanvas = new UICanvas(_parent, false, string.Empty, null, string.Empty);
		uICanvas.SetSize(0.01f, 0.01f, RelativeTo.ScreenHeight);
		uICanvas.SetAlign(0.2f, 1f);
		uICanvas.RemoveDrawHandler();
		m_own = new UIVerticalList(uICanvas, string.Empty);
		m_own.SetAlign(0.5f, 1f);
		m_own.SetSpacing(0.012f, RelativeTo.ScreenHeight);
		m_own.RemoveDrawHandler();
		PsUIProfileImage psUIProfileImage = new PsUIProfileImage(m_own, false, string.Empty, PlayerPrefsX.GetFacebookId(), PlayerPrefsX.GetGameCenterId(), PsMetagameManager.m_playerStats.rank);
		psUIProfileImage.SetSize(0.115f, 0.115f, RelativeTo.ScreenHeight);
		TransformS.SetRotation(psUIProfileImage.m_TC, new Vector3(0f, 0f, 5f));
		UIText uIText = new UIText(m_own, false, string.Empty, PlayerPrefsX.GetUserName(), "HurmeSemiBoldMN_Font", 0.035f, RelativeTo.ScreenHeight);
	}

	public virtual void CreateOpponentInfo(UIComponent _parent)
	{
		UICanvas uICanvas = new UICanvas(_parent, false, string.Empty, null, string.Empty);
		uICanvas.SetSize(0.01f, 0.01f, RelativeTo.ScreenHeight);
		uICanvas.SetAlign(0.8f, 1f);
		uICanvas.RemoveDrawHandler();
		m_opponent = new UIVerticalList(uICanvas, string.Empty);
		m_opponent.SetAlign(0.5f, 1f);
		m_opponent.SetSpacing(0.012f, RelativeTo.ScreenHeight);
		m_opponent.RemoveDrawHandler();
		string facebookId = null;
		string gameCenterId = null;
		int rank = -1;
		string text = "Looking for\nopponent";
		if (m_loop.m_versusData != null && m_loop.m_versusData.GetOpponentInfo() != null)
		{
			facebookId = m_loop.m_versusData.GetOpponentInfo().score.facebookId;
			gameCenterId = m_loop.m_versusData.GetOpponentInfo().score.gameCenterId;
			text = m_loop.m_versusData.GetOpponentInfo().score.name;
			rank = m_loop.m_versusData.GetOpponentInfo().rank;
		}
		m_opponentImage = new PsUIProfileImage(m_opponent, false, string.Empty, facebookId, gameCenterId, rank);
		m_opponentImage.SetSize(0.115f, 0.115f, RelativeTo.ScreenHeight);
		TransformS.SetRotation(m_opponentImage.m_TC, new Vector3(0f, 0f, -5f));
		m_opponentName = new UIText(m_opponent, false, string.Empty, text, "HurmeSemiBoldMN_Font", 0.035f, RelativeTo.ScreenHeight);
	}

	public virtual void CreateLowerInfo(UIComponent _parent)
	{
		m_proceed = new PsUIGenericButton(_parent, 0.25f, 0.25f, 0.008f);
		m_proceed.SetBlueColors();
		m_proceed.SetVerticalAlign(0f);
		m_proceed.SetDepthOffset(-5f);
		UIVerticalList uIVerticalList = new UIVerticalList(m_proceed, string.Empty);
		uIVerticalList.SetWidth(0.285f, RelativeTo.ScreenHeight);
		uIVerticalList.RemoveDrawHandler();
		UIText uIText = new UIText(uIVerticalList, false, string.Empty, "Race!", "KGSecondChances_Font", 0.04f, RelativeTo.ScreenHeight, m_proceed.m_textColor);
		m_timeLeft = new UIText(uIVerticalList, false, string.Empty, "Time left: " + PsMetagameManager.GetTimeStringFromSeconds(m_loop.SecondsLeft()), "KGSecondChances_Font", 0.025f, RelativeTo.ScreenHeight);
	}

	public override void Step()
	{
		if (m_timeLeft != null)
		{
			string timeStringFromSeconds = PsMetagameManager.GetTimeStringFromSeconds(m_loop.SecondsLeft());
			if (!timeStringFromSeconds.Equals(m_timeLeft.m_text))
			{
				m_timeLeft.SetText("Time left: " + timeStringFromSeconds);
			}
		}
		base.Step();
	}
}
