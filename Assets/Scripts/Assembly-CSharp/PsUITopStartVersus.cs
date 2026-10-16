using UnityEngine;

public class PsUITopStartVersus : UICanvas
{
	private PsUIGenericButton m_exitButton;

	public PsUITopStartVersus(UIComponent _parent)
		: base(_parent, false, "TopContent", null, string.Empty)
	{
		RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, "UpperLeft");
		uIHorizontalList.SetMargins(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetSpacing(0.13f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetAlign(0f, 1f);
		uIHorizontalList.RemoveDrawHandler();
		m_exitButton = new PsUIGenericButton(uIHorizontalList);
		m_exitButton.SetIcon("hud_icon_menu_exit", 0.06f);
		m_exitButton.SetOrangeColors();
		UIHorizontalList uIHorizontalList2 = new UIHorizontalList(uIHorizontalList, "challengeInfo");
		uIHorizontalList2.SetVerticalAlign(1f);
		uIHorizontalList2.SetSpacing(0.025f, RelativeTo.ScreenHeight);
		uIHorizontalList2.RemoveDrawHandler();
		UICanvas uICanvas = new UICanvas(uIHorizontalList2, false, string.Empty, null, string.Empty);
		uICanvas.SetWidth(0.3f, RelativeTo.ScreenHeight);
		uICanvas.SetHeight(0.5f, RelativeTo.ScreenHeight);
		uICanvas.SetVerticalAlign(0f);
		uICanvas.SetMargins(0f, 0f, 0.065f, -0.065f);
		uICanvas.SetDepthOffset(1f);
		uICanvas.SetRogue();
		uICanvas.RemoveDrawHandler();
		UIFittedSprite uIFittedSprite = new UIFittedSprite(uICanvas, false, "ShieldImage", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("challenge_profile_shield"));
		uIFittedSprite.SetWidth(0.3f, RelativeTo.ScreenHeight);
		uIFittedSprite.SetVerticalAlign(0f);
		PsUIProfileImage psUIProfileImage = new PsUIProfileImage(uIHorizontalList2, false, string.Empty, PlayerPrefsX.GetFacebookId(), PlayerPrefsX.GetGameCenterId(), PsMetagameManager.m_playerStats.rank);
		psUIProfileImage.SetSize(0.1f, 0.1f, RelativeTo.ScreenHeight);
		TransformS.SetRotation(psUIProfileImage.m_TC, new Vector3(0f, 0f, 5f));
		UIFittedSprite uIFittedSprite2 = new UIFittedSprite(uIHorizontalList2, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("challenge_VS_logo"));
		uIFittedSprite2.SetHeight(0.1f, RelativeTo.ScreenHeight);
		string facebookId = null;
		string gameCenterId = null;
		int rank = -1;
		if (PsState.m_activeGameLoop.m_versusData.GetOpponentInfo() != null)
		{
			facebookId = PsState.m_activeGameLoop.m_versusData.GetOpponentInfo().score.facebookId;
			gameCenterId = PsState.m_activeGameLoop.m_versusData.GetOpponentInfo().score.gameCenterId;
			rank = PsState.m_activeGameLoop.m_versusData.GetOpponentInfo().rank;
		}
		PsUIProfileImage psUIProfileImage2 = new PsUIProfileImage(uIHorizontalList2, false, string.Empty, facebookId, gameCenterId, rank);
		psUIProfileImage2.SetSize(0.1f, 0.1f, RelativeTo.ScreenHeight);
		TransformS.SetRotation(psUIProfileImage2.m_TC, new Vector3(0f, 0f, -5f));
		if (PsState.m_activeGameLoop.m_versusData.matchBall && PsState.m_activeGameLoop.m_versusData.currentPlayer == PlayerPrefsX.GetUserId())
		{
			UICanvas uICanvas2 = new UICanvas(uIHorizontalList2, false, string.Empty, null, string.Empty);
			uICanvas2.SetRogue();
			uICanvas2.SetSize(0.175f, 0.175f, RelativeTo.ScreenHeight);
			uICanvas2.SetAlign(0f, 1f);
			uICanvas2.SetMargins(-0.1f, 0.1f, -0.025f, 0.025f);
			uICanvas2.RemoveDrawHandler();
			UIFittedSprite uIFittedSprite3 = new UIFittedSprite(uICanvas2, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_icon_trophy_full"));
			uIFittedSprite3.SetHeight(1f, RelativeTo.ParentHeight);
			uIFittedSprite3.SetAlign(0f, 1f);
			uIFittedSprite3.SetDepthOffset(2f);
			TransformS.SetRotation(uIFittedSprite3.m_TC, new Vector3(0f, 0f, 3.5f));
		}
	}

	public override void Step()
	{
		if (m_exitButton.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
		}
		base.Step();
	}
}
