using UnityEngine;

public class UIRectSpriteButton : UIFittedSprite
{
	private bool m_noSound;

	public UIRectSpriteButton(UIComponent _parent, string _tag, SpriteSheet _spriteSheet, Frame _frame, bool _convertToPrefab = true, bool _noSound = false)
		: base(_parent, true, _tag, _spriteSheet, _frame, _convertToPrefab)
	{
		m_noSound = _noSound;
	}

	public override void DrawHandler(UIComponent _c)
	{
		base.DrawHandler(_c);
	}

	protected override void OnTouchRollIn(TLTouch _touch, bool _secondary)
	{
		base.OnTouchRollIn(_touch, _secondary);
		if (m_scaleOnTouch)
		{
			if (m_touchScaleTween != null)
			{
				TweenS.RemoveComponent(m_touchScaleTween);
				m_touchScaleTween = null;
			}
			m_touchScaleTween = TweenS.AddTransformTween(m_TC, TweenedProperty.Scale, TweenStyle.BackOut, new Vector3(1.05f, 1.05f, 1.05f), 0.1f, 0f, false);
		}
	}

	protected override void OnTouchBegan(TLTouch _touch)
	{
		base.OnTouchBegan(_touch);
		if (m_scaleOnTouch)
		{
			if (m_touchScaleTween != null)
			{
				TweenS.RemoveComponent(m_touchScaleTween);
				m_touchScaleTween = null;
			}
			m_touchScaleTween = TweenS.AddTransformTween(m_TC, TweenedProperty.Scale, TweenStyle.BackOut, new Vector3(1.05f, 1.05f, 1.05f), 0.1f, 0f, false);
		}
	}

	protected override void OnTouchRollOut(TLTouch _touch, bool _secondary)
	{
		base.OnTouchRollOut(_touch, _secondary);
		if (m_scaleOnTouch)
		{
			if (m_touchScaleTween != null)
			{
				TweenS.RemoveComponent(m_touchScaleTween);
				m_touchScaleTween = null;
			}
			m_touchScaleTween = TweenS.AddTransformTween(m_TC, TweenedProperty.Scale, TweenStyle.BackOut, new Vector3(1f, 1f, 1f), 0.1f, 0f, false);
		}
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		base.OnTouchRelease(_touch, _inside);
		if (m_scaleOnTouch)
		{
			if (m_touchScaleTween != null)
			{
				TweenS.RemoveComponent(m_touchScaleTween);
				m_touchScaleTween = null;
			}
			m_touchScaleTween = TweenS.AddTransformTween(m_TC, TweenedProperty.Scale, TweenStyle.BackOut, new Vector3(1f, 1f, 1f), 0.1f, 0f, false);
			if (!m_noSound && _inside)
			{
				SoundS.PlaySingleShot("/UI/ButtonNormal", Vector3.zero);
			}
		}
	}
}
