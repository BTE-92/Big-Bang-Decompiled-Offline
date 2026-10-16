using UnityEngine;

public class PsUIVersusOpponentBanner : PsUIVersusBanner
{
	public PsUIVersusOpponentBanner(UIComponent _parent, PsGameLoopVersus _loop)
		: base(_parent, _loop)
	{
	}

	public override void Initialize()
	{
		base.Initialize();
		m_bgFrame.flipX = true;
	}

	public override void CreateRoundInfo(UIComponent _parent)
	{
		string text = "OPPONENT'S\nTURN";
		string color = "ffffff";
		if (m_loop.m_versusData.matchBall)
		{
			text = "CHANCE\nTO LOSE!";
			color = "ec6a3b";
		}
		UICanvas uICanvas = new UICanvas(_parent, false, string.Empty, null, string.Empty);
		uICanvas.SetHeight(0.08f, RelativeTo.ScreenHeight);
		uICanvas.SetVerticalAlign(1f);
		uICanvas.SetMargins(0f, 0f, -0.01f, 0.01f);
		uICanvas.RemoveDrawHandler();
		UIText uIText = new UIText(uICanvas, false, string.Empty, text, "KGSecondChances_Font", 0.04f, RelativeTo.ScreenHeight, color, "000000");
		uIText.SetVerticalAlign(0f);
		uIText.SetShadowShift(new Vector2(1f, -0.5f), 0.05f);
	}

	public override void CreateOpponentInfo(UIComponent _parent)
	{
		base.CreateOpponentInfo(_parent);
		if (m_loop.m_versusData.matchBall)
		{
			UIFittedSprite uIFittedSprite = new UIFittedSprite(this, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_trophy_small_full"));
			uIFittedSprite.SetHeight(0.1f, RelativeTo.ScreenHeight);
			uIFittedSprite.SetHorizontalAlign(1f);
			uIFittedSprite.SetDepthOffset(-5f);
			TransformS.SetRotation(uIFittedSprite.m_TC, new Vector3(0f, 0f, -3.5f));
		}
	}

	public override void CreateLowerInfo(UIComponent _parent)
	{
		m_timeLeft = new UIText(_parent, false, string.Empty, "Time left: " + PsMetagameManager.GetTimeStringFromSeconds(m_loop.SecondsLeft()), "KGSecondChances_Font", 0.025f, RelativeTo.ScreenHeight);
		m_timeLeft.SetVerticalAlign(0.2f);
	}

	public override void Step()
	{
		base.Step();
	}
}
