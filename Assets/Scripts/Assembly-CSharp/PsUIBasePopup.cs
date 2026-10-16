using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsUIBasePopup : UICanvas
{
	public Entity m_utilityEntity;

	private List<Entity> m_hiddenEntities;

	private bool m_hideUIComponents;

	public UIComponent m_mainContent;

	private UIComponent m_leftContent;

	private UIComponent m_rightContent;

	public UIComponent m_topContent;

	public UIComponent m_pageButtonLeft;

	public UIComponent m_pageButtonRight;

	private Type m_main;

	private Type m_top;

	private Type m_left;

	private Type m_right;

	private Hashtable m_actions = new Hashtable();

	public UIScrollableSnappingCanvas m_scrollableCanvas;

	private UIHorizontalList m_scrollableCanvasContent;

	public Camera m_overlayCamera;

	private InitialPage m_page;

	public PsUIBasePopup(Type _main, Type _top = null, Type _left = null, Type _right = null, bool _hideUIComponents = true, bool _addPopup = true, InitialPage _page = InitialPage.Center)
		: base(null, false, "UIBase", null, string.Empty)
	{
		m_hideUIComponents = _hideUIComponents;
		m_main = _main;
		m_top = _top;
		m_left = _left;
		m_right = _right;
		m_page = _page;
		Initialize(m_main, m_top, m_left, m_right, _addPopup);
	}

	private void Initialize(Type _main, Type _top, Type _left = null, Type _right = null, bool _addPopup = true)
	{
		if (m_hideUIComponents)
		{
			m_hiddenEntities = EntityManager.SetVisibilityOfEntitiesWithTag("UIComponent", false);
			PsState.m_UIComponentsHidden = true;
			UnityUIManager.HideVisibleUI(this);
		}
		SetWidth(1f, RelativeTo.ScreenWidth);
		SetHeight(1f, RelativeTo.ScreenHeight);
		SetDepthOffset(20f);
		RemoveDrawHandler();
		object[] array = null;
		m_utilityEntity = EntityManager.AddEntity();
		m_utilityEntity.m_persistent = true;
		int num = 1;
		if (_left != null)
		{
			num++;
		}
		if (_right != null)
		{
			num++;
		}
		m_scrollableCanvas = new UIScrollableSnappingCanvas(this, "ScrollableCanvas", num);
		m_scrollableCanvas.RemoveDrawHandler();
		m_scrollableCanvas.SetWidth(1f, RelativeTo.ParentWidth);
		m_scrollableCanvas.SetHeight(1f, RelativeTo.ParentHeight);
		m_scrollableCanvas.SetVerticalAlign(0f);
		m_scrollableCanvas.m_maxScrollInertialX = 200f * (float.Parse(Screen.width.ToString()) / 1024f);
		if (num == 1)
		{
			m_scrollableCanvas.m_maxScrollInertialX = 0f;
		}
		m_scrollableCanvas.m_maxScrollInertialY = 0f;
		float horizontalAlign = 0f;
		if (m_page == InitialPage.Center)
		{
			if (_left != null && _right == null)
			{
				m_scrollableCanvas.SetScrollPosition(1f, 0f);
				m_scrollableCanvas.m_currentPageX = 1;
				horizontalAlign = 1f;
			}
			else if (_left == null && _right != null)
			{
				m_scrollableCanvas.SetScrollPosition(0f, 0f);
				m_scrollableCanvas.m_currentPageX = 0;
			}
			else
			{
				m_scrollableCanvas.SetScrollPosition(0.5f, 0f);
				horizontalAlign = 0.5f;
				m_scrollableCanvas.m_currentPageX = 1;
			}
		}
		else if (m_page == InitialPage.Left)
		{
			horizontalAlign = 0f;
			m_scrollableCanvas.SetScrollPosition(0f, 0f);
			m_scrollableCanvas.m_currentPageX = 0;
		}
		else if (m_page == InitialPage.Right)
		{
			horizontalAlign = 1f;
			m_scrollableCanvas.SetScrollPosition(1f, 0f);
			if (_left == null)
			{
				m_scrollableCanvas.m_currentPageX = 1;
			}
			else
			{
				m_scrollableCanvas.m_currentPageX = 2;
			}
		}
		m_scrollableCanvasContent = new UIHorizontalList(m_scrollableCanvas, "hlist");
		m_scrollableCanvasContent.SetHeight(1f, RelativeTo.ParentHeight);
		m_scrollableCanvasContent.SetHorizontalAlign(horizontalAlign);
		m_scrollableCanvasContent.RemoveDrawHandler();
		array = new object[1] { m_scrollableCanvasContent };
		if (_left != null)
		{
			m_leftContent = Activator.CreateInstance(_left, array) as UIComponent;
		}
		if (_main != null)
		{
			m_mainContent = Activator.CreateInstance(_main, array) as UIComponent;
		}
		if (_right != null)
		{
			m_rightContent = Activator.CreateInstance(_right, array) as UIComponent;
		}
		m_overlayCamera = CameraS.AddCamera("OverlayCamera", true);
		if (_top != null)
		{
			array = new object[1] { this };
			m_topContent = Activator.CreateInstance(_top, array) as UIComponent;
			m_topContent.SetCamera(m_overlayCamera, true, false);
		}
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
		if (m_topContent != null)
		{
			m_topContent.UpdateUniqueCamera();
		}
		if (_addPopup)
		{
			PsState.m_openPopups.Add(this);
		}
		Update();
	}

	public override void Step()
	{
		if (m_pageButtonLeft != null && m_pageButtonLeft.m_hit)
		{
			m_scrollableCanvas.m_changePage = -1;
		}
		else if (m_pageButtonRight != null && m_pageButtonRight.m_hit)
		{
			m_scrollableCanvas.m_changePage = 1;
		}
		base.Step();
	}

	public void CreatePageButtonLeft(string _text)
	{
		m_pageButtonLeft = new UICanvas(null, true, "PageButtonLeft", null, string.Empty);
		m_pageButtonLeft.SetSize(0.15f, 0.1f, RelativeTo.ScreenHeight);
		m_pageButtonLeft.SetAlign(0.05f, 0.5f);
		UIFittedText uIFittedText = new UIFittedText(m_pageButtonLeft, false, string.Empty, _text, "KGSecondChances_Font", true, "#FFFFFF", "#000000");
		uIFittedText.m_shadowDistance = 0.05f;
		m_pageButtonLeft.SetCamera(m_overlayCamera, true, false);
		m_pageButtonLeft.Update();
	}

	public void CreatePageButtonRight(string _text)
	{
		m_pageButtonRight = new UICanvas(null, true, "PageButtonRight", null, string.Empty);
		m_pageButtonRight.SetSize(0.15f, 0.1f, RelativeTo.ScreenHeight);
		m_pageButtonRight.SetAlign(0.95f, 0.5f);
		UIFittedText uIFittedText = new UIFittedText(m_pageButtonRight, false, string.Empty, _text, "KGSecondChances_Font", true, "#FFFFFF", "#000000");
		uIFittedText.m_shadowDistance = 0.05f;
		m_pageButtonRight.SetCamera(m_overlayCamera, true, false);
		m_pageButtonRight.Update();
	}

	public void RemovePageButtons()
	{
		if (m_pageButtonLeft != null)
		{
			m_pageButtonLeft.Destroy();
			m_pageButtonLeft = null;
		}
		if (m_pageButtonRight != null)
		{
			m_pageButtonRight.Destroy();
			m_pageButtonRight = null;
		}
	}

	public void SetAction(string _key, Action _action)
	{
		if (!m_actions.ContainsKey(_key))
		{
			m_actions.Add(_key, _action);
			return;
		}
		Debug.LogWarning("OVERRIDING EXISTING ACTION, KEY: " + _key);
		m_actions[_key] = _action;
	}

	public void BringToFront()
	{
		if (m_scrollableCanvasContent != null)
		{
			CameraS.BringToFront(m_scrollableCanvasContent.m_camera);
		}
		if (m_topContent != null)
		{
			CameraS.BringToFront(m_topContent.m_camera);
		}
	}

	public void CallAction(string _key)
	{
		if (m_actions.ContainsKey(_key))
		{
			Action action = m_actions[_key] as Action;
			if (action != null)
			{
				action();
			}
		}
		else
		{
			Debug.LogWarning("KEY NOT FOUND: " + _key);
		}
	}

	public override void Destroy()
	{
		RemovePageButtons();
		if (m_hideUIComponents)
		{
			for (int i = 0; i < m_hiddenEntities.Count; i++)
			{
				if (m_hiddenEntities[i] != null)
				{
					EntityManager.SetVisibilityOfEntity(m_hiddenEntities[i], true);
				}
			}
			UnityUIManager.ShowHiddenUI(this);
		}
		if (m_overlayCamera != null)
		{
			CameraS.RemoveCamera(m_overlayCamera);
		}
		EntityManager.RemoveEntity(m_utilityEntity);
		m_utilityEntity = null;
		PsState.m_openPopups.Remove(this);
		base.Destroy();
	}
}
