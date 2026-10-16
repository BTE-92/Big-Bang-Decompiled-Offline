using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class GeConnection : GraphConnection, ISerializable, IDeserializationCallback
{
	private bool m_isHighlighted;

	public GeConnection(GraphElement _start, GraphElement _end)
		: base("GeConnection")
	{
		m_disabledAtStart = false;
		_start.AddOutput(this);
		_end.AddInput(this);
	}

	public GeConnection(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
	}

	public override void SetPropertyDefaults()
	{
		base.SetPropertyDefaults();
		m_isRemovable = true;
	}

	public override void Assemble()
	{
		base.Assemble();
		m_TAC = TouchAreaS.AddCircleArea(m_TC, "Select", m_radius, CameraS.m_mainCamera, m_TC);
		TouchAreaS.AddTouchEventListener(m_TAC, SelectionTouchHandler);
		Draw(false);
		GeBaseState.UpdateInfoUI(null);
	}

	public override void Update()
	{
		if (!m_disabled)
		{
			base.Update();
			bool flag = false;
			if (m_start != null && m_start.m_selected)
			{
				flag = true;
			}
			else if (m_end != null && m_end.m_selected)
			{
				flag = true;
			}
			if (m_selected || flag)
			{
				Draw(true);
			}
			else if (!flag && m_isHighlighted)
			{
				Draw(false);
			}
		}
	}

	public override void Select(bool _select)
	{
		base.Select(_select);
		if (!_select)
		{
			Draw(false);
		}
	}

	private void Draw(bool _highlight)
	{
		m_isHighlighted = _highlight;
		if (m_end != null && m_start != null)
		{
			Vector3 vector = m_end.m_position - m_start.m_position;
			Vector3 position = m_start.m_position + vector * 0.5f;
			Vector3 vector2 = vector * 0.5f - vector.normalized * 25f;
			Vector3 vector3 = vector2 - vector.normalized * 10f;
			TransformS.SetGlobalPosition(m_TC, position);
			float f = Mathf.Atan2(0f - vector.y, vector.x);
			float x = Mathf.Sin(f);
			float y = Mathf.Cos(f);
			Vector3 vector4 = new Vector2(x, y).normalized;
			m_points[0] = m_start.m_position;
			m_points[m_points.Count - 1] = m_end.m_position;
			DebugDraw.Clear(CameraS.m_mainCamera, m_TC);
			if (m_selected)
			{
				DebugDraw.defaultColor = Color.yellow;
			}
			else if (!_highlight)
			{
				DebugDraw.defaultColor = new Color(0.3f, 0.3f, 0.3f);
			}
			else
			{
				DebugDraw.defaultColor = new Color(0.8f, 0.8f, 0.8f);
			}
			DebugDraw.CreateLine(CameraS.m_mainCamera, m_TC, -vector2, vector2);
			DebugDraw.CreateLine(CameraS.m_mainCamera, m_TC, vector2, vector3 + vector4 * 10f);
			DebugDraw.CreateLine(CameraS.m_mainCamera, m_TC, vector2, vector3 + vector4 * -10f);
			DebugDraw.CreateCircle(CameraS.m_mainCamera, m_TC, Vector2.zero, 5f, false, 8);
			DebugDraw.defaultColor = Color.white;
		}
	}

	protected void SelectionTouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		if (_touchCount != 1)
		{
			return;
		}
		if (_touchPhase == TouchAreaPhase.ReleaseIn && !_touchIsSecondary)
		{
			List<GraphElement> selection = GizmoManager.GetSelection();
			if (m_selected)
			{
				GizmoManager.RemoveFromSelection(this);
			}
			else
			{
				GizmoManager.AddToSelection(this);
			}
			new SelectUndoAction(selection, GizmoManager.GetSelection());
			GizmoManager.Update();
		}
		List<GraphElement> selection2 = GizmoManager.GetSelection();
		if (selection2.Count == 1)
		{
			if (selection2[0].m_elementType == GraphElementType.Node)
			{
				GeNode node = selection2[0] as GeNode;
				GeBaseState.UpdateInfoUI(node);
			}
			else
			{
				GeBaseState.UpdateInfoUI(null);
			}
		}
		else if (selection2.Count == 0)
		{
			GeBaseState.UpdateInfoUI(null);
		}
	}

	public new GeConnection DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			return (GeConnection)binaryFormatter.Deserialize(memoryStream);
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
	}
}
