using UnityEngine;

public class PsUICenterClaimFriendRace : PsUIHeaderedCanvas
{
	private PsUIGenericButton m_accept;

	private PsUIWaitPopup m_waitPopup;

	private VersusMetaData m_versusdata;

	private VersusParticipant m_opponentInfo;

	private bool m_playerWon;

	public PsUICenterClaimFriendRace(UIComponent _parent)
		: base(_parent, string.Empty)
	{
		SetWidth(0.65f, RelativeTo.ScreenWidth);
		SetHeight(0.45f, RelativeTo.ScreenHeight);
		SetVerticalAlign(0.4f);
		SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
		SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
		m_header.SetDrawHandler(PsUIDrawHandlers.ScrollingUIHeader);
		m_header.SetMargins(0.0125f, 0.0125f, 0.0125f, 0f);
		m_versusdata = (PsState.m_activeGameLoop as PsGameLoopVersus).m_versusData;
		m_playerWon = m_versusdata.winner == PlayerPrefsX.GetUserId();
		if ((PsState.m_activeGameLoop as PsGameLoopVersus).m_versusData.GetOpponentInfo() != null)
		{
			m_opponentInfo = m_versusdata.GetOpponentInfo();
			CreateContent(this);
			CreateHeaderContent(m_header);
		}
		else
		{
			Debug.LogError("No opponent, this is bad");
			Destroy();
		}
	}

	public void CreateHeaderContent(UIComponent _parent)
	{
		UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.025f, 0.025f, 0f, 0f);
		uIHorizontalList.SetHorizontalAlign(0.5f);
		string text = "<color=#46fff1>" + m_opponentInfo.score.name + "</color>";
		string text2 = ((!m_playerWon) ? (text + " beat you") : ("You beat " + text));
		UIText uIText = new UIText(uIHorizontalList, false, string.Empty, text2, "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
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
		string text = "<color=#46fff1>" + m_opponentInfo.score.name + "</color>";
		string text2 = ((!m_playerWon) ? ("Your friend " + text + " beat you. Not acceptable!") : ("You beat your friend " + text + ". Way to go!"));
		UITextbox uITextbox = new UITextbox(_parent, false, string.Empty, text2, "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, false, Align.Center, Align.Middle);
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
	}

	public override void Step()
	{
		if (m_accept != null && m_accept.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
		}
		base.Step();
	}
}
