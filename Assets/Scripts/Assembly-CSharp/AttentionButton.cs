using UnityEngine;

public class AttentionButton : PsUIGenericButton
{
	private TweenC m_attentionTween;

	public AttentionButton(UIComponent _parent)
		: base(_parent)
	{
		m_attentionTween = TweenS.AddTransformTween(m_TC, TweenedProperty.Scale, TweenStyle.QuadInOut, Vector3.one, new Vector3(0.95f, 0.95f, 1f), 0.5f, 0f, false);
		TweenS.SetAdditionalTweenProperties(m_attentionTween, -1, true, TweenStyle.QuadInOut);
	}

	protected override void OnTouchRollIn(TLTouch _touch, bool _secondary)
	{
		if (m_attentionTween != null)
		{
			TweenS.RemoveComponent(m_attentionTween);
			m_attentionTween = null;
		}
		base.OnTouchRollIn(_touch, _secondary);
	}

	protected override void OnTouchBegan(TLTouch _touch)
	{
		if (m_attentionTween != null)
		{
			TweenS.RemoveComponent(m_attentionTween);
			m_attentionTween = null;
		}
		base.OnTouchBegan(_touch);
	}

	private void MiddleTweener(TweenC _c)
	{
		m_attentionTween = TweenS.AddTransformTween(m_TC, TweenedProperty.Scale, TweenStyle.QuadInOut, Vector3.one, new Vector3(0.95f, 0.95f, 1f), 0.5f, 0f, false);
		TweenS.SetAdditionalTweenProperties(m_attentionTween, -1, true, TweenStyle.QuadInOut);
	}

	protected override void OnTouchRollOut(TLTouch _touch, bool _secondary)
	{
		if (m_scaledUp)
		{
			if (m_touchScaleTween != null)
			{
				TweenS.RemoveComponent(m_touchScaleTween);
				m_touchScaleTween = null;
			}
			Hide();
			m_touchScaleTween = TweenS.AddTransformTween(m_TC, TweenedProperty.Scale, TweenStyle.CubicInOut, m_currentScale - new Vector3(0.05f, 0.05f, 0.05f), 0.1f, 0f, false);
			TweenS.AddTweenEndEventListener(m_touchScaleTween, MiddleTweener);
			m_currentScale -= new Vector3(0.05f, 0.05f, 0.05f);
			m_scaledUp = false;
		}
		if (!m_end)
		{
			if (_secondary)
			{
				m_end = true;
				HighlightSecondary(false);
			}
			else
			{
				m_end = true;
				Highlight(false);
			}
		}
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		base.OnTouchRelease(_touch, _inside);
		if (!_inside && m_attentionTween == null)
		{
			m_attentionTween = TweenS.AddTransformTween(m_TC, TweenedProperty.Scale, TweenStyle.QuadInOut, Vector3.one, new Vector3(0.95f, 0.95f, 1f), 0.5f, 0f, false);
			TweenS.SetAdditionalTweenProperties(m_attentionTween, -1, true, TweenStyle.QuadInOut);
		}
	}
}
