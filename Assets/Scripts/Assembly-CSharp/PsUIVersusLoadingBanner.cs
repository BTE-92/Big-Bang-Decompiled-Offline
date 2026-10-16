using UnityEngine;

public class PsUIVersusLoadingBanner : PsUIVersusBannerAlt
{
	public PsUIVersusLoadingBanner(UIComponent _parent, PsGameLoopVersus _loop)
		: base(_parent, _loop)
	{
	}

	public override void CreateOwnInfo(UIComponent _parent)
	{
		base.CreateOwnInfo(_parent);
		if (m_loop.m_versusData != null && m_loop.m_versusData.matchBall)
		{
			UIFittedSprite uIFittedSprite = new UIFittedSprite(this, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_trophy_small_full"));
			uIFittedSprite.SetHeight(0.1f, RelativeTo.ScreenHeight);
			uIFittedSprite.SetHorizontalAlign(0f);
			uIFittedSprite.SetDepthOffset(-5f);
			TransformS.SetRotation(uIFittedSprite.m_TC, new Vector3(0f, 0f, 3.5f));
			UIText uIText = new UIText(m_own, false, string.Empty, "Chance to\nwin a Trophy!", "KGSecondChances_Font", 0.03f, RelativeTo.ScreenHeight, "#fbec5c", "#000000");
			uIText.SetVerticalAlign(0.2f);
			uIText.SetShadowShift(new Vector2(1f, -0.3f), 0.05f);
		}
	}

	public void UpdateOpponentInfo()
	{
		if (m_loop.m_minigameMetaData != null && m_loop.m_versusData.GetOpponentInfo() != null)
		{
			m_opponentImage.m_facebookId = m_loop.m_versusData.GetOpponentInfo().score.facebookId;
			m_opponentImage.LoadPicture();
			m_opponentName.SetText(m_loop.m_versusData.GetOpponentInfo().score.name);
		}
	}
}
