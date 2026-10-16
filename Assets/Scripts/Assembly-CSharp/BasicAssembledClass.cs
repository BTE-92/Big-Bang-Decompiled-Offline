using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BasicAssembledClass : IAssembledClass
{
	private GraphElement _graphElement;

	private List<Entity> _assembledEntities;

	public Minigame m_minigame;

	public GraphElement m_graphElement
	{
		get
		{
			return _graphElement;
		}
		set
		{
			_graphElement = value;
		}
	}

	public List<Entity> m_assembledEntities
	{
		get
		{
			return _assembledEntities;
		}
		set
		{
			_assembledEntities = value;
		}
	}

	public BasicAssembledClass(GraphElement _graphElement)
	{
		m_assembledEntities = new List<Entity>();
		m_graphElement = _graphElement;
		m_minigame = LevelManager.m_currentLevel as Minigame;
	}

	public virtual void CreateGraphElementTouchArea(Mesh _collisionMesh, TransformC _parentTC = null)
	{
		if (m_minigame.m_editing)
		{
			if (m_graphElement.m_TAC != null)
			{
				Debug.LogError("Trying to add multiple touch areas to GraphElement.");
				return;
			}
			m_graphElement.m_isRect = true;
			m_graphElement.m_width = _collisionMesh.bounds.extents.x;
			m_graphElement.m_width = _collisionMesh.bounds.extents.y;
			m_graphElement.m_TAC = TouchAreaS.AddMeshArea((_parentTC != null) ? _parentTC : m_graphElement.m_TC, "Select", _collisionMesh, CameraS.m_mainCamera, m_graphElement.m_TC);
			TouchAreaS.AddTouchEventListener(m_graphElement.m_TAC, SelectionTouchHandler);
		}
	}

	public virtual void CreateGraphElementTouchArea(float _graphElementRadius, TransformC _parentTC = null)
	{
		if (m_minigame.m_editing)
		{
			if (m_graphElement.m_TAC != null)
			{
				Debug.LogError("Trying to add multiple touch areas to GraphElement.");
				return;
			}
			m_graphElement.m_isRect = false;
			m_graphElement.m_radius = _graphElementRadius;
			m_graphElement.m_TAC = TouchAreaS.AddCircleArea((_parentTC != null) ? _parentTC : m_graphElement.m_TC, "Select", _graphElementRadius, CameraS.m_mainCamera, m_graphElement.m_TC);
			TouchAreaS.AddTouchEventListener(m_graphElement.m_TAC, SelectionTouchHandler);
		}
	}

	public virtual void CreateGraphElementTouchArea(float _graphElementWidth, float _graphElementHeight, TransformC _parentTC = null)
	{
		if (m_minigame.m_editing)
		{
			if (m_graphElement.m_TAC != null)
			{
				Debug.LogError("Trying to add multiple touch areas to GraphElement.");
				return;
			}
			m_graphElement.m_isRect = true;
			m_graphElement.m_width = _graphElementWidth;
			m_graphElement.m_height = _graphElementHeight;
			m_graphElement.m_TAC = TouchAreaS.AddRectArea((_parentTC != null) ? _parentTC : m_graphElement.m_TC, "Select", _graphElementWidth, _graphElementHeight, CameraS.m_mainCamera, m_graphElement.m_TC);
			TouchAreaS.AddTouchEventListener(m_graphElement.m_TAC, SelectionTouchHandler);
		}
	}

	public void RemoveAssembledEntity(Entity _e)
	{
		if (m_assembledEntities.Contains(_e))
		{
			EntityManager.RemoveEntity(_e);
			m_assembledEntities.Remove(_e);
		}
	}

	public virtual void Destroy()
	{
		while (m_assembledEntities.Count > 0)
		{
			int index = m_assembledEntities.Count - 1;
			if (m_assembledEntities[index].m_index > -1)
			{
				EntityManager.RemoveEntity(m_assembledEntities[index]);
			}
			m_assembledEntities.RemoveAt(index);
		}
		m_graphElement.m_assembledClasses.Remove(this);
	}

	public virtual void SyncPositionToGraphElementPosition()
	{
		for (int i = 0; i < m_assembledEntities.Count; i++)
		{
			List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.ChipmunkBody, m_assembledEntities[i]);
			for (int j = 0; j < componentsByEntity.Count; j++)
			{
				ChipmunkBodyC chipmunkBodyC = componentsByEntity[j] as ChipmunkBodyC;
				ChipmunkProWrapper.ucpBodySetPos(chipmunkBodyC.body, chipmunkBodyC.TC.transform.position);
			}
		}
	}

	protected void SelectionTouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		if (_touchCount == 1 && _touchPhase == TouchAreaPhase.ReleaseIn && !_touchIsSecondary)
		{
			List<GraphElement> selection = GizmoManager.GetSelection();
			if (m_graphElement.m_selected)
			{
				GizmoManager.RemoveFromSelection(m_graphElement);
			}
			else
			{
				GizmoManager.AddToSelection(m_graphElement);
			}
			new SelectUndoAction(selection, GizmoManager.GetSelection());
			GizmoManager.Update();
		}
	}
}
