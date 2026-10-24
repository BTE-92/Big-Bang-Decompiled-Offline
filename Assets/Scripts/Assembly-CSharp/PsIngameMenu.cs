using System;
using UnityEngine;

public static class PsIngameMenu
{
	private static PsUIBasePopup _popupMenu;

	public static IPlayMenu m_playMenu;

	public static Controller m_controller;

	private static Camera m_overlayCamera;

	public static PsUIBasePopup m_popupMenu
	{
		get
		{
			return _popupMenu;
		}
		set
		{
			_popupMenu = value;
		}
	}

	public static void CloseAll()
	{
		if (m_popupMenu != null)
		{
			m_popupMenu.Destroy();
			m_popupMenu = null;
		}
		if (m_playMenu != null)
		{
			m_playMenu.Destroy();
			m_playMenu = null;
		}
		CloseController();
	}

	public static void OpenController(bool _overlayCamera = false)
	{
		if (PsState.m_activeMinigame == null)
		{
			return;
		}
		if (m_controller != null)
		{
			CloseController();
		}
		if (_overlayCamera)
		{
			m_overlayCamera = CameraS.AddCamera("ControllerCamera", true);
		}
		if (PsState.m_activeMinigame.m_playerControllerType != null)
		{
			object[] args = new object[1] { m_overlayCamera };
			m_controller = Activator.CreateInstance(PsState.m_activeMinigame.m_playerControllerType, args) as Controller;
			m_controller.Open();
		}
	}

	public static void CloseController()
	{
		if (m_controller != null)
		{
			m_controller.Close();
			m_controller = null;
			if (m_overlayCamera != null)
			{
				CameraS.RemoveCamera(m_overlayCamera);
				m_overlayCamera = null;
			}
		}
	}
}
