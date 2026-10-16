using UnityEngine;

public class MotorcycleController : Controller
{
	private UIHorizontalList m_leftArea;

	private UIHorizontalList m_rightArea;

	public UIRectSpriteSensor m_rightButton;

	private Camera m_overlayCamera;

	public MotorcycleController(Camera _overlayCamera = null)
	{
		m_overlayCamera = _overlayCamera;
	}

	public override void Open()
	{
		if (m_open)
		{
			Close();
		}
		m_open = true;
		m_leftArea = new UIHorizontalList(null, "buttonArea");
		m_leftArea.SetAlign(0f, 0f);
		m_leftArea.SetMargins(0.04f, 0f, 0f, 0.04f, RelativeTo.ScreenShortest);
		m_leftArea.SetSpacing(0.01f, RelativeTo.ScreenShortest);
		m_leftArea.RemoveDrawHandler();
		UIRectSpriteSensor uIRectSpriteSensor = new UIRectSpriteSensor(m_leftArea, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("controller_arrow_tilt_left"), false);
		uIRectSpriteSensor.SetHeight(0.125f, RelativeTo.ScreenLongest);
		UIRectSpriteSensor uIRectSpriteSensor2 = new UIRectSpriteSensor(m_leftArea, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("controller_arrow_tilt_right"), false);
		uIRectSpriteSensor2.SetHeight(0.125f, RelativeTo.ScreenLongest);
		AddButton("LeftButton1", uIRectSpriteSensor, ControllerButtonType.BUTTON, KeyCode.LeftArrow);
		AddButton("LeftButton2", uIRectSpriteSensor2, ControllerButtonType.BUTTON, KeyCode.RightArrow);
		m_leftArea.Update();
		if (Booster.IsBoosterAllowed())
		{
			m_boostButton = new PsUIBoosterButton(null);
			m_boostButton.SetAlign((float)Screen.height / (float)Screen.width * 0.04f, m_leftArea.m_height * 1.3f);
			m_boostButton.Update();
			AddButton("BoostButton", m_boostButton.m_button, ControllerButtonType.BUTTON, KeyCode.Space);
		}
		if (m_boostButton != null && PsMetagameManager.m_playerStats.GetBoosterValue() < 1)
		{
			m_boostButton.GreyScaleOn();
		}
		m_rightArea = new UIHorizontalList(null, "buttonArea");
		m_rightArea.SetAlign(1f, 0f);
		m_rightArea.SetMargins(0f, 0.04f, 0f, 0.04f, RelativeTo.ScreenShortest);
		m_rightArea.SetSpacing(0.01f, RelativeTo.ScreenShortest);
		m_rightArea.RemoveDrawHandler();
		UIRectSpriteSensor uIRectSpriteSensor3 = new UIRectSpriteSensor(m_rightArea, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("controller_arrow_left"), false);
		uIRectSpriteSensor3.SetHeight(0.125f, RelativeTo.ScreenLongest);
		m_rightButton = new UIRectSpriteSensor(m_rightArea, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("controller_arrow_right"), false);
		m_rightButton.SetHeight(0.125f, RelativeTo.ScreenLongest);
		AddButton("Reverse", uIRectSpriteSensor3, ControllerButtonType.BUTTON, KeyCode.DownArrow);
		AddButton("Throttle", m_rightButton, ControllerButtonType.BUTTON, KeyCode.UpArrow);
		m_rightArea.Update();
		if (m_overlayCamera != null)
		{
			m_leftArea.SetCamera(m_overlayCamera, true, false);
			m_rightArea.SetCamera(m_overlayCamera, true, false);
			if (m_boostButton != null)
			{
				m_boostButton.SetCamera(m_overlayCamera, true, false);
			}
		}
	}

	public override void Close()
	{
		base.Close();
		if (m_open)
		{
			m_open = false;
			m_leftArea.Destroy();
			m_rightArea.Destroy();
			if (m_boostButton != null)
			{
				m_boostButton.Destroy();
			}
			Controller.RemoveAllButtons();
		}
	}

	public override void EnableController()
	{
		foreach (UIRectSpriteSensor child in m_leftArea.m_childs)
		{
			child.EnableTouchAreas();
			child.m_rect.p_gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Framework/VertexColorUnlitDouble");
		}
		foreach (UIRectSpriteSensor child2 in m_rightArea.m_childs)
		{
			child2.EnableTouchAreas();
			child2.m_rect.p_gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Framework/VertexColorUnlitDouble");
		}
		if (m_boostButton != null && PsMetagameManager.m_playerStats.GetBoosterValue() > 0)
		{
			m_boostButton.EnableTouchAreas();
			m_boostButton.GreyScaleOff();
		}
		base.EnableController();
	}

	public override void DisableController()
	{
		foreach (UIRectSpriteSensor child in m_leftArea.m_childs)
		{
			child.DisableTouchAreas();
			child.m_rect.p_gameObject.GetComponent<Renderer>().material.shader = Shader.Find("WOE/Fx/GreyscaleUnlitAlpha");
		}
		foreach (UIRectSpriteSensor child2 in m_rightArea.m_childs)
		{
			child2.DisableTouchAreas();
			child2.m_rect.p_gameObject.GetComponent<Renderer>().material.shader = Shader.Find("WOE/Fx/GreyscaleUnlitAlpha");
		}
		if (m_boostButton != null)
		{
			m_boostButton.DisableTouchAreas();
			m_boostButton.GreyScaleOn();
		}
		base.DisableController();
	}

	public override void DisabletouchAreas()
	{
		base.DisabletouchAreas();
		foreach (UIRectSpriteSensor child in m_leftArea.m_childs)
		{
			child.DisableTouchAreas();
		}
		foreach (UIRectSpriteSensor child2 in m_rightArea.m_childs)
		{
			child2.DisableTouchAreas();
		}
		if (m_boostButton != null)
		{
			m_boostButton.DisableTouchAreas();
		}
	}
}
