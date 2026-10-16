using System.Collections.Generic;

public static class UndoManager
{
	public static List<UndoAction> m_steps = new List<UndoAction>();

	public static int m_currentStep = 0;

	public static int m_maxSteps = 10;

	public static void Undo()
	{
		GizmoManager.Update();
		if (m_steps.Count > m_currentStep)
		{
			Debug.Log("undo: " + m_currentStep);
			m_steps[m_currentStep].ApplyUndo();
			m_currentStep++;
		}
		else
		{
			Debug.Log("No steps to undo");
		}
		GizmoManager.Update();
	}

	public static void Redo()
	{
		GizmoManager.Update();
		if (m_steps.Count >= m_currentStep && m_currentStep > 0)
		{
			m_currentStep--;
			m_steps[m_currentStep].ApplyRedo();
			Debug.Log("redo: " + m_currentStep);
		}
		else
		{
			Debug.Log("No steps to redo");
		}
		GizmoManager.Update();
	}

	public static void Purge()
	{
		m_steps.Clear();
		m_currentStep = 0;
	}

	public static void Add(UndoAction _step)
	{
		if (m_currentStep > 0)
		{
			m_steps.RemoveRange(0, m_currentStep);
			m_currentStep = 0;
		}
		m_steps.Insert(0, _step);
		if (m_steps.Count > m_maxSteps)
		{
			m_steps.RemoveRange(m_maxSteps, m_steps.Count - m_maxSteps);
		}
	}
}
