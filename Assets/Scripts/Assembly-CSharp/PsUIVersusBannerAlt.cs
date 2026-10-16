using UnityEngine;

public class PsUIVersusBannerAlt : PsUIVersusBanner
{
	public PsUIVersusBannerAlt(UIComponent _parent, PsGameLoopVersus _loop)
		: base(_parent, _loop)
	{
		m_vs.SetHeight(0.15f, RelativeTo.ScreenHeight);
		m_vs.SetVerticalAlign(0.975f);
	}

	public override void CreateLowerInfo(UIComponent _parent)
	{
		string text = "1";
		if (m_loop.m_versusData != null)
		{
			text = m_loop.m_versusData.round.ToString();
		}
		UIText uIText = new UIText(_parent, false, string.Empty, "ROUND\n<color=#ffb009>" + text + "</color>", "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, "#ffffff", "#000000");
		uIText.SetVerticalAlign(0.225f);
		uIText.SetShadowShift(new Vector2(1f, -0.5f), 0.05f);
	}

	public override void CreateRoundInfo(UIComponent _parent)
	{
	}
}
