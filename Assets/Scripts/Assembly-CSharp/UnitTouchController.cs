using UnityEngine;

public class UnitTouchController : Controller
{
	private Camera m_overlayCamera;

	public UnitTouchController(Camera _overlayCamera = null)
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
	}

	public override void Close()
	{
		base.Close();
		if (m_open)
		{
			m_open = false;
			Controller.RemoveAllButtons();
		}
	}
}
