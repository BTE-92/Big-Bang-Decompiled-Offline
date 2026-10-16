using System;
using UnityEngine;

public class PsGameCardState : BasicState
{
	private Entity m_utilityEntity;

	private UIScrollableCanvas m_baseCanvas;

	private UICanvas m_overlayCanvas;

	private UICanvas m_upperArea;

	private UIHorizontalList m_upperLeftArea;

	private UIHorizontalList m_upperRightArea;

	private UICanvas m_lowerArea;

	private UIHorizontalList m_lowerLeftArea;

	private UIHorizontalList m_lowerRightArea;

	public PsUIGenericButton m_closeButton;

	private PsUIProfileImage m_profile;

	private Action m_exitAction;

	private Type m_mainType;

	private Type m_leftType;

	private Type m_rightType;

	public UIComponent m_mainContent;

	public UIComponent m_leftContent;

	public UIComponent m_rightContent;

	public UIScrollableSnappingCanvas m_scrollableCanvas;

	public UIHorizontalList m_scrollableCanvasContent;

	private Camera m_overlayCamera;

	public PsGameCardState(Action _exitAction, Type _main, Type _left = null, Type _right = null)
	{
		m_exitAction = _exitAction;
		m_mainType = _main;
		m_leftType = _left;
		m_rightType = _right;
	}

	public override void Enter(IStatedObject _parent)
	{
		m_utilityEntity = EntityManager.AddEntity();
		CreateUI();
	}

	public virtual void CreateUI()
	{
		m_baseCanvas = new UIScrollableCanvas(null, "GameCard");
		m_baseCanvas.SetWidth(1f, RelativeTo.ScreenWidth);
		m_baseCanvas.SetHeight(1f, RelativeTo.ScreenHeight);
		m_baseCanvas.SetDepthOffset(20f);
		m_baseCanvas.RemoveDrawHandler();
		m_baseCanvas.m_maxScrollInertialX = 0f;
		m_baseCanvas.m_maxScrollInertialY = 0f;
		int num = 1;
		if (m_leftType != null)
		{
			num++;
		}
		if (m_rightType != null)
		{
			num++;
		}
		m_scrollableCanvas = new UIScrollableSnappingCanvas(m_baseCanvas, "ScrollableCanvas", num);
		m_scrollableCanvas.RemoveDrawHandler();
		m_scrollableCanvas.SetWidth(1f, RelativeTo.ParentWidth);
		m_scrollableCanvas.SetHeight(1f, RelativeTo.ParentHeight);
		m_scrollableCanvas.SetMargins(0.025f, 0.025f, 0.025f, 0.025f, RelativeTo.ScreenWidth);
		m_scrollableCanvas.SetVerticalAlign(0f);
		m_scrollableCanvas.m_maxScrollInertialX = 200f * (float.Parse(Screen.width.ToString()) / 1024f);
		if (num == 1)
		{
			m_scrollableCanvas.m_maxScrollInertialX = 0f;
		}
		m_scrollableCanvas.m_maxScrollInertialY = 0f;
		float horizontalAlign = 0f;
		if (m_leftType != null && m_rightType == null)
		{
			m_scrollableCanvas.SetScrollPosition(1f, 0f);
			horizontalAlign = 1f;
		}
		else if (m_leftType == null && m_rightType != null)
		{
			m_scrollableCanvas.SetScrollPosition(0f, 0f);
		}
		else
		{
			m_scrollableCanvas.SetScrollPosition(0.5f, 0f);
			horizontalAlign = 0.5f;
		}
		m_scrollableCanvasContent = new UIHorizontalList(m_scrollableCanvas, "hlist");
		m_scrollableCanvasContent.SetSpacing(0.125f, RelativeTo.ScreenHeight);
		m_scrollableCanvasContent.SetMargins(0.125f, 0.125f, 0f, 0f, RelativeTo.ScreenWidth);
		m_scrollableCanvasContent.SetVerticalAlign(0f);
		m_scrollableCanvasContent.SetHorizontalAlign(horizontalAlign);
		m_scrollableCanvasContent.RemoveDrawHandler();
		object[] args = new object[1] { m_scrollableCanvasContent };
		if (m_leftType != null)
		{
			m_leftContent = Activator.CreateInstance(m_leftType, args) as UIComponent;
		}
		if (m_mainType != null)
		{
			m_mainContent = Activator.CreateInstance(m_mainType, args) as UIComponent;
		}
		if (m_rightType != null)
		{
			m_rightContent = Activator.CreateInstance(m_rightType, args) as UIComponent;
		}
		CreateOverlayHeader();
		if (m_leftContent != null)
		{
			m_leftContent.UpdateUniqueCamera();
		}
		if (m_mainContent != null)
		{
			m_mainContent.UpdateUniqueCamera();
		}
		if (m_rightContent != null)
		{
			m_rightContent.UpdateUniqueCamera();
		}
		m_baseCanvas.Update();
	}

	public virtual void CreateOverlayHeader()
	{
		m_overlayCamera = CameraS.AddCamera("FullscreenPopupCamera", true);
		PsMetagameManager.ShowResources(m_overlayCamera, true);
		m_overlayCanvas = new UICanvas(m_baseCanvas, false, "OverlayCanvas", null, string.Empty);
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
	}

	public override void Execute()
	{
		if (m_closeButton.m_hit)
		{
			SoundS.PlaySingleShot("/UI/ButtonBack", Vector3.zero);
			m_exitAction();
		}
	}

	public override void Exit()
	{
		m_baseCanvas.Destroy();
		PsMetagameManager.HideResources();
		CameraS.RemoveCamera(m_overlayCamera);
		m_overlayCamera = null;
	}
}
