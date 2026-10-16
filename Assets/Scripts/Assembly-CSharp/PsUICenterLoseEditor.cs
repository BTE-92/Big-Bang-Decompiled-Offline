using UnityEngine;

public class PsUICenterLoseEditor : UICanvas
{
	public PsUICenterLoseEditor(UIComponent _parent)
		: base(_parent, true, string.Empty, null, string.Empty)
	{
		SetWidth(1f, RelativeTo.ScreenWidth);
		SetHeight(1f, RelativeTo.ScreenHeight);
		RemoveDrawHandler();
		UIText uIText = new UIText(this, false, string.Empty, "<color=#d1f0d8>Tap to</color> <i><color=#2EFE9A>retry</color></i>", "HurmeBold_Font", 0.08f, RelativeTo.ScreenHeight);
		uIText.SetVerticalAlign(0.95f);
		TweenC c = TweenS.AddTransformTween(uIText.m_TC, TweenedProperty.Scale, TweenStyle.QuadInOut, new Vector3(1.05f, 1.05f, 1f), 0.6f, 0f, false);
		TweenS.SetAdditionalTweenProperties(c, -1, true, TweenStyle.QuadInOut);
	}

	public override void Step()
	{
		if (m_hit || Input.GetKeyDown(KeyCode.RightShift))
		{
			(GetRoot() as PsUIBasePopup).CallAction("Restart");
		}
		base.Step();
	}
}
