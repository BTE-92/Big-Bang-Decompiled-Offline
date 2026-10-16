using System.Collections.Generic;

public class SelectUndoAction : UndoAction
{
	public uint[] m_oldElements;

	public uint[] m_newElements;

	public SelectUndoAction(List<GraphElement> _oldElements, List<GraphElement> _newElements)
	{
		m_oldElements = new uint[_oldElements.Count];
		m_newElements = new uint[_newElements.Count];
		for (int i = 0; i < _oldElements.Count; i++)
		{
			m_oldElements[i] = _oldElements[i].m_id;
		}
		for (int j = 0; j < _newElements.Count; j++)
		{
			m_newElements[j] = _newElements[j].m_id;
		}
		UndoManager.Add(this);
	}

	public override void ApplyUndo()
	{
		GizmoManager.ClearSelection();
		for (int i = 0; i < m_oldElements.Length; i++)
		{
			GraphElement element = LevelManager.m_currentLevel.m_currentLayer.GetElement(m_oldElements[i]);
			if (element != null)
			{
				element.Select(true);
				GizmoManager.AddToSelection(element);
			}
		}
	}

	public override void ApplyRedo()
	{
		GizmoManager.ClearSelection();
		for (int i = 0; i < m_newElements.Length; i++)
		{
			GraphElement element = LevelManager.m_currentLevel.m_currentLayer.GetElement(m_newElements[i]);
			if (element != null)
			{
				element.Select(true);
				GizmoManager.AddToSelection(element);
			}
		}
	}
}
