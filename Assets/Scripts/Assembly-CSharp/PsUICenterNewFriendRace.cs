using System;
using UnityEngine;

public class PsUICenterNewFriendRace : PsUIHeaderedCanvas
{
	private PsUIGenericButton m_accept;

	private PsUIGenericButton m_decline;

	private PsUIWaitPopup m_waitPopup;

	private VersusParticipant m_opponentInfo;

	public PsUICenterNewFriendRace(UIComponent _parent)
		: base(_parent, string.Empty)
	{
		SetWidth(0.65f, RelativeTo.ScreenWidth);
		SetHeight(0.45f, RelativeTo.ScreenHeight);
		SetVerticalAlign(0.4f);
		SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
		SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
		m_header.SetDrawHandler(PsUIDrawHandlers.ScrollingUIHeader);
		m_header.SetMargins(0.0125f, 0.0125f, 0.0125f, 0f);
		if ((PsState.m_activeGameLoop as PsGameLoopVersus).m_versusData.GetOpponentInfo() != null)
		{
			m_opponentInfo = (PsState.m_activeGameLoop as PsGameLoopVersus).m_versusData.GetOpponentInfo();
			CreateContent(this);
			CreateHeaderContent(m_header);
		}
		else
		{
			Debug.LogError("No opponent, this is bad");
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
		}
	}

	public void Create(VersusParticipant _opponentInfo)
	{
	}

	public void CreateHeaderContent(UIComponent _parent)
	{
		UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.025f, 0.025f, 0f, 0f);
		uIHorizontalList.SetHorizontalAlign(0.5f);
		string text = "<color=#46fff1>" + m_opponentInfo.score.name + "</color> wants your head!";
		UIText uIText = new UIText(uIHorizontalList, false, string.Empty, text, "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
		PsUIProfileImage psUIProfileImage = new PsUIProfileImage(_parent, false, string.Empty, m_opponentInfo.score.facebookId, m_opponentInfo.score.gameCenterId, m_opponentInfo.rank);
		psUIProfileImage.SetSize(0.12f, 0.12f, RelativeTo.ScreenHeight);
		psUIProfileImage.SetAlign(0f, 0.5f);
		psUIProfileImage.SetDepthOffset(-5f);
		psUIProfileImage.SetMargins(-0.333f, 0f, -0.333f, 0f, RelativeTo.OwnWidth);
		TransformS.Rotate(psUIProfileImage.m_TC, new Vector3(0f, 0f, 6f));
	}

	public void CreateContent(UIComponent _parent)
	{
		_parent.RemoveTouchAreas();
		string text = "Your friend <color=#46fff1>" + m_opponentInfo.score.name + "</color> challenged you to race!" + Environment.NewLine + "Do you want to accept?";
		UITextbox uITextbox = new UITextbox(_parent, false, string.Empty, text, "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, false, Align.Center, Align.Middle);
		uITextbox.SetHorizontalAlign(0.6f);
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetAlign(1f, 0f);
		uIHorizontalList.SetMargins(0f, 0.1f, 0.05f, -0.05f);
		uIHorizontalList.SetSpacing(0.3f, RelativeTo.ScreenHeight);
		m_accept = new PsUIGenericButton(uIHorizontalList);
		m_accept.SetAlign(1f, 1f);
		m_accept.SetText("Accept", 0.05f);
		m_accept.SetGreenColors();
		m_decline = new PsUIGenericButton(uIHorizontalList);
		m_decline.SetAlign(1f, 1f);
		m_decline.SetText("Decline", 0.05f);
		m_decline.SetRedColors();
	}

	public override void Step()
	{
		if (m_accept != null && m_accept.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Accept");
		}
		else if (m_decline != null && m_decline.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Decline");
		}
		base.Step();
	}
}
