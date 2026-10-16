using UnityEngine;

public class TestNotification : Notification
{
	private string m_text;

	private UICanvas m_canvas;

	private TweenC m_tween;

	private bool m_turned;

	public TestNotification(string _text)
	{
		m_text = _text;
	}

	public override void Start()
	{
		base.Start();
		m_duration = 2.0;
		m_turned = false;
		SetSize(0.5f, 0.1f, RelativeTo.ScreenHeight);
		RemoveDrawHandler();
		SetMargins(0f);
		SetAlign(0.5f, 0f);
		RemoveTouchAreas();
		m_camera.depth = 2f;
		m_canvas = new UICanvas(this, true, "Notification", null, string.Empty);
		m_canvas.SetSize(0.5f, 0.1f, RelativeTo.ScreenHeight);
		m_canvas.SetDrawHandler(PsUIPopup.PopupDrawHandler);
		new UIFittedText(m_canvas, false, string.Empty, m_text, "HurmeRegular_Font", true);
		m_tween = TweenS.AddTransformTween(m_canvas.m_TC, TweenedProperty.Position, TweenStyle.CubicInOut, m_canvas.m_TC.transform.localPosition - new Vector3(0f, 0.1f * (float)Screen.height, 0f), m_canvas.m_TC.transform.localPosition, 0.5f, 0f, false);
		Update();
	}

	public override void End()
	{
		Destroy();
		base.End();
	}

	public override void Step()
	{
		if (m_started != 0.0 && m_tween != null && m_tween.hasFinished && m_started + m_duration - Main.m_gameTimeSinceAppStarted <= 0.5 && !m_turned)
		{
			m_turned = true;
			TweenS.RemoveComponent(m_tween);
			m_tween = TweenS.AddTransformTween(m_canvas.m_TC, TweenedProperty.Position, TweenStyle.CubicInOut, m_canvas.m_TC.transform.localPosition, m_canvas.m_TC.transform.localPosition - new Vector3(0f, 0.1f * (float)Screen.height, 0f), 0.5f, 0f, false);
		}
		base.Step();
	}

	public override void Destroy()
	{
		if (m_tween != null)
		{
			TweenS.RemoveComponent(m_tween);
			m_tween = null;
		}
		base.Destroy();
	}
}
