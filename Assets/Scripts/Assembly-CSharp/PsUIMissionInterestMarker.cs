using UnityEngine;

public class PsUIMissionInterestMarker : UICanvas
{
	private UIFittedSprite m_sprite;

	public PsUIMissionInterestMarker(UIComponent _parent)
		: base(_parent, false, "MissionInterestMarker", null, string.Empty)
	{
		SetMargins(-0.5f, 0.5f, -0.5f, 0.5f, RelativeTo.OwnHeight);
		SetSize(0.05f, 0.05f, RelativeTo.ScreenShortest);
		SetAlign(0f, 1f);
		RemoveDrawHandler();
		m_rogue = true;
		m_sprite = new UIFittedSprite(this, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_mission_icon"));
		TweenC c = TweenS.AddTransformTween(m_sprite.m_TC, TweenedProperty.Rotation, TweenStyle.CubicInOut, Vector3.forward * -20f, Vector3.forward * 20f, 0.5f, 0f, false);
		TweenS.SetAdditionalTweenProperties(c, -1, true, TweenStyle.CubicInOut);
	}

	public override void UpdateSpecial()
	{
		UpdateAlign();
		base.UpdateSpecial();
	}
}
