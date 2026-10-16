using UnityEngine;

public class PsUIAttentionArrow : UICanvas
{
	private UIFittedSprite m_sprite;

	public PsUIAttentionArrow(UIComponent _parent, bool _onRight = true)
		: base(_parent, false, "AttentionArrow", null, string.Empty)
	{
		SetSize(0.1f, 0.1f, RelativeTo.ScreenShortest);
		RemoveDrawHandler();
		Frame frame = PsState.m_uiSheet.m_atlas.GetFrame("menu_attention_arrow");
		if (!_onRight)
		{
			frame.flipX = true;
		}
		m_sprite = new UIFittedSprite(this, false, string.Empty, PsState.m_uiSheet, frame);
		m_sprite.m_TC.transform.position += new Vector3(0f, 0f, -10f);
		TweenS.SetAdditionalTweenProperties(TweenS.AddTransformTween(m_sprite.m_TC, TweenedProperty.Position, TweenStyle.CubicInOut, m_sprite.m_TC.transform.localPosition, m_sprite.m_TC.transform.localPosition - new Vector3(30f * ((!_onRight) ? (-1f) : 1f), 0f, 0f), 0.5f, 0f, false), -1, true, TweenStyle.CubicInOut);
	}

	public override void UpdateSpecial()
	{
		UpdateAlign();
		base.UpdateSpecial();
	}
}
