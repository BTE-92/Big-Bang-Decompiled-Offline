using System;
using UnityEngine;

public class PsBasicMenuState : BasicState
{
	private PsMenuScene m_parent;

	private Camera m_overlayCamera;

	private UICanvas m_overlayCanvas;

	private UIHorizontalList m_upperLeftArea;

	private PsUIGenericButton m_closeButton;

	private Type m_contentClass;

	private UIComponent m_content;

	public PsBasicMenuState(Type _contentClass)
	{
		m_contentClass = _contentClass;
	}

	public override void Enter(IStatedObject _parent)
	{
		PsMetagameManager.ShowResources();
		m_parent = _parent as PsMenuScene;
		PsMenuScene.CreateBlur();
		if (m_contentClass != null)
		{
			object[] args = new object[1] { this };
			m_content = Activator.CreateInstance(m_contentClass, args) as UIComponent;
		}
		CreateOverlayHeader();
	}

	public virtual void CreateOverlayHeader()
	{
		m_overlayCamera = CameraS.AddCamera("FullscreenPopupCamera", true);
		PsMetagameManager.ShowResources(m_overlayCamera, true);
		m_overlayCanvas = new UICanvas(null, false, "OverlayCanvas", null, string.Empty);
		m_overlayCanvas.RemoveTouchAreas();
		m_overlayCanvas.RemoveDrawHandler();
		m_upperLeftArea = new UIHorizontalList(m_overlayCanvas, "UpperLeft");
		m_upperLeftArea.SetMargins(0.025f, RelativeTo.ScreenShortest);
		m_upperLeftArea.SetAlign(0f, 1f);
		m_upperLeftArea.RemoveDrawHandler();
		m_upperLeftArea.RemoveTouchAreas();
		m_upperLeftArea.SetDepthOffset(-50f);
		m_upperLeftArea.m_camera = m_overlayCamera;
		m_closeButton = new PsUIGenericButton(m_upperLeftArea);
		m_closeButton.SetIcon("hud_icon_back", 0.06f);
		m_overlayCanvas.Update();
	}

	public override void Execute()
	{
		if (m_closeButton.m_hit)
		{
			SoundS.PlaySingleShot("/UI/ButtonBack", Vector3.zero);
			m_parent.m_stateMachine.ChangeState(new PsMainMenuState());
			PsMenuScene.m_lastState = null;
		}
	}

	private void DestroyUI()
	{
		if (m_overlayCanvas != null)
		{
			m_overlayCanvas.Destroy();
			m_overlayCanvas = null;
		}
		if (m_content != null)
		{
			m_content.Destroy();
			m_content = null;
		}
		if (m_overlayCamera != null)
		{
			CameraS.RemoveCamera(m_overlayCamera);
			m_overlayCamera = null;
		}
	}

	public override void Exit()
	{
		DestroyUI();
	}
}
