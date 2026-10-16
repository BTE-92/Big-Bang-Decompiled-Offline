using System;
using System.Collections;

public class PsUIBaseState : BasicState
{
	private PsUIBasePopup m_baseCanvas;

	private Type m_mainType;

	private Type m_leftType;

	private Type m_rightType;

	private Type m_topType;

	private InitialPage m_page;

	private bool m_addToOpenPopups;

	private Hashtable m_queuedActions = new Hashtable();

	public PsUIBaseState(Type _main, Type _top = null, Type _left = null, Type _right = null, bool _addToOpenPopups = false, InitialPage _page = InitialPage.Center)
	{
		m_mainType = _main;
		m_leftType = _left;
		m_rightType = _right;
		m_topType = _top;
		m_page = _page;
		m_addToOpenPopups = _addToOpenPopups;
	}

	public override void Enter(IStatedObject _parent)
	{
		EntityManager.SetActivityOfEntitiesWithTag("PlanetUI", false, false);
		CreateUI();
	}

	public virtual void CreateUI()
	{
		m_baseCanvas = new PsUIBasePopup(m_mainType, m_topType, m_leftType, m_rightType, true, m_addToOpenPopups, m_page);
		if (m_queuedActions.Count <= 0)
		{
			return;
		}
		foreach (string key in m_queuedActions.Keys)
		{
			SetAction(key, m_queuedActions[key] as Action);
		}
	}

	public void SetAction(string _key, Action _action)
	{
		if (m_baseCanvas != null)
		{
			m_baseCanvas.SetAction(_key, _action);
			return;
		}
		Debug.LogWarning("Base UI is null! Setting action to queue to be set up after UI creation");
		if (!m_queuedActions.ContainsKey(_key))
		{
			m_queuedActions.Add(_key, _action);
			return;
		}
		Debug.LogWarning("DUPLICATE KEY! Replacing action set to key: " + _key);
		m_queuedActions[_key] = _action;
	}

	public void BringToFront()
	{
		if (m_baseCanvas != null)
		{
			m_baseCanvas.BringToFront();
		}
	}

	public void CallAction(string _key)
	{
		if (m_baseCanvas != null)
		{
			m_baseCanvas.CallAction(_key);
		}
		else
		{
			Debug.LogWarning("Base UI is null!");
		}
	}

	public void DestroyCanvas()
	{
		if (m_baseCanvas != null)
		{
			m_baseCanvas.Destroy();
			m_baseCanvas = null;
		}
	}

	public override void Exit()
	{
		DestroyCanvas();
		EntityManager.SetActivityOfEntitiesWithTag("PlanetUI", true);
	}
}
