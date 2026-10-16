using UnityEngine;

public class UIRectSpriteSensor : UIFittedSprite
{
	public UIRectSpriteSensor(UIComponent _parent, string _tag, SpriteSheet _spriteSheet, Frame _frame, bool _convertToPrefab = true)
		: base(_parent, true, _tag, _spriteSheet, _frame, _convertToPrefab)
	{
		m_TAC.m_allowSecondary = true;
		m_TAC.m_allowPrimary = false;
		m_TAC.m_cancelOtherTouches = false;
	}

	public override void DrawHandler(UIComponent _c)
	{
		TweenS.RemoveAllTweensFromEntity(m_spriteTC.p_entity);
		if (m_highlight || m_highlightSecondary)
		{
			TweenS.AddTransformTween(m_spriteTC, TweenedProperty.Scale, TweenStyle.BackOut, new Vector3(0.8f, 0.8f, 1f), 0.1f, 0f, true);
		}
		else
		{
			TweenS.AddTransformTween(m_spriteTC, TweenedProperty.Scale, TweenStyle.BackOut, new Vector3(1f, 1f, 1f), 0.1f, 0f, true);
		}
	}

	protected override void Highlight(bool _value)
	{
		base.Highlight(_value);
		d_Draw(this);
	}

	protected override void HighlightSecondary(bool _value)
	{
		base.HighlightSecondary(_value);
		d_Draw(this);
	}
}
