using System.Collections.Generic;
using UnityEngine;

public static class GizmoManager
{
	private static List<GraphElement> m_selection = new List<GraphElement>();

	public static List<GraphElement> m_parents = new List<GraphElement>();

	public static List<GraphElement> m_childs = new List<GraphElement>();

	public static MultiGizmo m_multiGizmo;

	public static List<ParentGizmo> m_parentGizmos = new List<ParentGizmo>();

	public static List<BasicGizmo> m_childGizmos = new List<BasicGizmo>();

	public static void Update()
	{
		if (m_multiGizmo != null)
		{
			m_multiGizmo.Destroy();
			m_multiGizmo = null;
		}
		while (m_parentGizmos.Count > 0)
		{
			int index = m_parentGizmos.Count - 1;
			m_parentGizmos[index].Destroy();
			m_parentGizmos.RemoveAt(index);
		}
		while (m_childGizmos.Count > 0)
		{
			int index2 = m_childGizmos.Count - 1;
			m_childGizmos[index2].Destroy();
			m_childGizmos.RemoveAt(index2);
		}
		m_parents.Clear();
		m_childs.Clear();
		if (m_selection.Count == 0)
		{
			return;
		}
		for (int i = 0; i < m_selection.Count; i++)
		{
			GraphElement graphElement = m_selection[i];
			if (graphElement.m_elementType == GraphElementType.Node)
			{
				GraphNode graphNode = graphElement as GraphNode;
				if (graphNode.m_childElements.Count > 0 || graphNode.m_parentElement.m_elementType == GraphElementType.Graph)
				{
					m_parents.Add(graphNode);
				}
				else if (graphNode.m_parentElement.m_elementType == GraphElementType.Node)
				{
					m_childs.Add(graphNode);
				}
			}
			else if (graphElement.m_elementType == GraphElementType.Connection)
			{
				m_parents.Add(graphElement);
			}
		}
		if (m_parents.Count > 1)
		{
			m_multiGizmo = new MultiGizmo(m_selection);
		}
		else
		{
			for (int j = 0; j < m_parents.Count; j++)
			{
				List<GraphElement> list = new List<GraphElement>();
				list.Add(m_parents[j]);
				if (m_parents[j].m_elementType == GraphElementType.Node)
				{
					if ((m_parents[j] as GraphNode).m_nodeType == GraphNodeType.Normal)
					{
						m_parentGizmos.Add(new ParentGizmo(list));
					}
				}
				else if (m_parents[j].m_elementType == GraphElementType.Connection)
				{
					m_parentGizmos.Add(new ParentGizmo(list));
				}
			}
			for (int k = 0; k < m_childs.Count; k++)
			{
				List<GraphElement> list2 = new List<GraphElement>();
				list2.Add(m_childs[k]);
				if ((m_childs[k] as GraphNode).m_nodeType == GraphNodeType.Child)
				{
					m_childGizmos.Add(new ChildGizmo(list2));
				}
				else if ((m_childs[k] as GraphNode).m_nodeType == GraphNodeType.IndependentChild)
				{
					m_childGizmos.Add(new IndependentChildGizmo(list2));
				}
			}
		}
		SoundS.PlaySingleShot("/UI/ButtonNormal", Vector3.zero);
	}

	public static List<GraphElement> GetParents(List<GraphElement> _graphElements)
	{
		List<GraphElement> list = new List<GraphElement>();
		for (int i = 0; i < _graphElements.Count; i++)
		{
			if (_graphElements[i].m_parentElement.m_elementType == GraphElementType.Node)
			{
				if (!list.Contains(_graphElements[i].m_parentElement))
				{
					list.Add(_graphElements[i].m_parentElement);
				}
			}
			else if (!list.Contains(_graphElements[i]))
			{
				list.Add(_graphElements[i]);
			}
		}
		return list;
	}

	public static void AddToSelection(List<GraphElement> _graphElements)
	{
		for (int i = 0; i < _graphElements.Count; i++)
		{
			AddToSelection(_graphElements[i]);
		}
	}

	public static void AddToSelection(GraphElement _graphElement)
	{
		if (!_graphElement.m_isSelectable)
		{
			return;
		}
		if (_graphElement.m_parentElement.m_elementType == GraphElementType.Node)
		{
			AddToSelection(_graphElement.m_parentElement);
			return;
		}
		if (!m_selection.Contains(_graphElement))
		{
			m_selection.Add(_graphElement);
			_graphElement.Select(true);
		}
		if (_graphElement.m_elementType != GraphElementType.Node)
		{
			return;
		}
		GraphNode graphNode = _graphElement as GraphNode;
		for (int i = 0; i < graphNode.m_childElements.Count; i++)
		{
			if (!m_selection.Contains(graphNode.m_childElements[i]))
			{
				m_selection.Add(graphNode.m_childElements[i]);
				graphNode.m_childElements[i].Select(true);
			}
		}
	}

	public static void RemoveFromSelection(GraphElement _graphElement)
	{
		if (!_graphElement.m_isSelectable)
		{
			return;
		}
		if (_graphElement.m_parentElement.m_elementType == GraphElementType.Node)
		{
			RemoveFromSelection(_graphElement.m_parentElement);
			return;
		}
		if (_graphElement.m_elementType == GraphElementType.Node)
		{
			GraphNode graphNode = _graphElement as GraphNode;
			for (int i = 0; i < graphNode.m_childElements.Count; i++)
			{
				if (graphNode.m_childElements[i].m_selected)
				{
					m_selection.Remove(graphNode.m_childElements[i]);
					graphNode.m_childElements[i].Select(false);
				}
			}
		}
		if (_graphElement.m_selected)
		{
			m_selection.Remove(_graphElement);
			_graphElement.Select(false);
		}
	}

	public static void SetSelection(List<GraphElement> _newSelection)
	{
		ClearSelection();
		AddToSelection(_newSelection);
	}

	public static List<GraphElement> GetSelection()
	{
		return m_selection;
	}

	public static void ClearSelection()
	{
		for (int i = 0; i < m_selection.Count; i++)
		{
			m_selection[i].Select(false);
		}
		m_selection.Clear();
	}

	public static void UpdatePosition()
	{
		if (m_multiGizmo != null)
		{
			m_multiGizmo.UpdatePosition();
		}
		for (int i = 0; i < m_parentGizmos.Count; i++)
		{
			m_parentGizmos[i].UpdatePosition();
		}
		for (int j = 0; j < m_childGizmos.Count; j++)
		{
			m_childGizmos[j].UpdatePosition();
		}
	}
}
