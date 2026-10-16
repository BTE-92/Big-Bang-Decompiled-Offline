using UnityEngine;

public class PsUIVersusClaimBanner : PsUIVersusBanner
{
	public PsUIVersusClaimBanner(UIComponent _parent, PsGameLoopVersus _loop)
		: base(_parent, _loop)
	{
	}

	public override void CreateRoundInfo(UIComponent _parent)
	{
		string text = "YOU LOST";
		string color = "ffffff";
		if (m_loop.m_versusData.winner == PlayerPrefsX.GetUserId())
		{
			text = "YOU WON!";
			color = "a1ff1b";
		}
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
}
