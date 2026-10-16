using UnityEngine;

public class TLTouch : IPoolable
{
	private int _index;

	public int m_fingerId;

	public Vector2 m_startPosition;

	public Vector2 m_currentPosition;

	public Vector2 m_deltaPosition;

	public TouchPhase m_phase;

	public int m_tapCount;

	public bool m_consumed;

	public Camera m_consumingCamera;

	public bool m_cancelled;

	public bool m_dragged;

	public TouchAreaC m_primaryArea;

	public TouchAreaPhase m_primaryPhase;

	public TouchAreaC m_secondaryArea;

	public TouchAreaPhase m_secondaryPhase;

	public TouchAreaC m_lockedSecondary;

	public int m_index
	{
		get
		{
			return _index;
		}
		set
		{
			_index = value;
		}
	}

	public void Reset()
	{
		m_consumed = false;
		m_consumingCamera = null;
		m_cancelled = false;
		m_dragged = false;
		m_primaryArea = null;
		m_secondaryArea = null;
		m_secondaryPhase = TouchAreaPhase.RollOut;
		m_primaryPhase = TouchAreaPhase.ReleaseOut;
		m_lockedSecondary = null;
	}

	public void Destroy()
	{
	}
}
