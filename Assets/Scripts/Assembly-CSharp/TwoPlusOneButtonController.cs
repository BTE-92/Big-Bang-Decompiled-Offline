using UnityEngine;

public class TwoPlusOneButtonController : Controller
{
	private UIHorizontalList m_leftArea;

	private UIHorizontalList m_rightArea;

	private Camera m_overlayCamera;

	public TwoPlusOneButtonController(Camera _overlayCamera = null)
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
		UIRectSpriteSensor uIRectSpriteSensor = new UIRectSpriteSensor(m_leftArea, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("controller_arrow_left"), false);
		uIRectSpriteSensor.SetHeight(0.175f, RelativeTo.ScreenShortest);
		uIRectSpriteSensor.SetTouchAreaSizeMultipler(1.5f);
		AddButton("LeftButton1", uIRectSpriteSensor, ControllerButtonType.BUTTON, KeyCode.LeftArrow);
		UIRectSpriteSensor uIRectSpriteSensor2 = new UIRectSpriteSensor(m_leftArea, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("controller_arrow_right"), false);
		uIRectSpriteSensor2.SetHeight(0.175f, RelativeTo.ScreenShortest);
		uIRectSpriteSensor2.SetTouchAreaSizeMultipler(1.5f);
		AddButton("LeftButton2", uIRectSpriteSensor2, ControllerButtonType.BUTTON, KeyCode.RightArrow);
		m_leftArea.Update();
		m_rightArea = new UIHorizontalList(null, "buttonArea");
		m_rightArea.SetAlign(1f, 0f);
		m_rightArea.SetMargins(0f, 0.04f, 0f, 0.04f, RelativeTo.ScreenShortest);
		m_rightArea.SetSpacing(0.01f, RelativeTo.ScreenShortest);
		m_rightArea.RemoveDrawHandler();
		UIRectSpriteSensor uIRectSpriteSensor3 = new UIRectSpriteSensor(m_rightArea, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("controller_button_green"), false);
		uIRectSpriteSensor3.SetHeight(0.175f, RelativeTo.ScreenShortest);
		uIRectSpriteSensor3.SetTouchAreaSizeMultipler(1.5f);
		AddButton("RightButton", uIRectSpriteSensor3, ControllerButtonType.BUTTON, KeyCode.UpArrow);
		m_rightArea.Update();
		if (m_overlayCamera != null)
		{
			m_leftArea.SetCamera(m_overlayCamera, true, false);
			m_rightArea.SetCamera(m_overlayCamera, true, false);
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
			Controller.RemoveAllButtons();
		}
	}
}
