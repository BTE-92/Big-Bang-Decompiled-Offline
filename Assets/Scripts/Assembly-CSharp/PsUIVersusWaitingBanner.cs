using UnityEngine;

public class PsUIVersusWaitingBanner : PsUIVersusBanner
{
	private UIText m_waiting;

	public PsUIVersusWaitingBanner(UIComponent _parent, PsGameLoopVersus _loop)
		: base(_parent, _loop)
	{
		m_bg.SetOverrideShader(Shader.Find("WOE/Fx/GreyscaleUnlitAlpha"));
	}

	public override void Initialize()
	{
		base.Initialize();
		m_bgFrame.flipX = true;
	}

	public override void CreateLowerInfo(UIComponent _parent)
	{
	}

	public override void Step()
	{
		base.Step();
	}
}
