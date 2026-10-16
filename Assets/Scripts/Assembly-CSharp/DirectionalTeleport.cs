using UnityEngine;

public class DirectionalTeleport : TeleportBase
{
	public DirectionalTeleport(GraphElement _graphElement)
		: base(_graphElement, 75f, string.Empty, 0f)
	{
		ucpPolyShape shape = new ucpPolyShape(m_radius * 2f, 10f, new Vector2(0f, 0f - (m_radius + 5f)), 257u, 1f, 0.1f, 0.5f, (ucpCollisionType)4);
		ChipmunkProS.AddKinematicBody(m_node1TC, shape);
		ChipmunkProS.AddKinematicBody(m_node2TC, shape);
		shape = new ucpPolyShape(10f, m_radius * 2f, new Vector2(0f - (m_radius + 5f), 0f), 257u, 1f, 0.1f, 0.5f, (ucpCollisionType)4);
		ChipmunkProS.AddKinematicBody(m_node1TC, shape);
		ChipmunkProS.AddKinematicBody(m_node2TC, shape);
		shape = new ucpPolyShape(10f, m_radius * 2f, new Vector2(m_radius + 5f, 0f), 257u, 1f, 0.1f, 0.5f, (ucpCollisionType)4);
		ChipmunkProS.AddKinematicBody(m_node1TC, shape);
		ChipmunkProS.AddKinematicBody(m_node2TC, shape);
		m_node1.m_isRotateable = true;
		m_node2.m_isRotateable = true;
	}

	public override void Update()
	{
		base.Update();
		DebugDraw.CreateLine(CameraS.m_mainCamera, m_node1TC, Vector3.zero, Vector3.up * (m_radius + 25f));
		DebugDraw.CreateBox(CameraS.m_mainCamera, m_node1TC, new Vector3(0f, 0f - (m_radius + 5f)), m_radius * 2f, 10f);
		DebugDraw.CreateBox(CameraS.m_mainCamera, m_node1TC, new Vector3(m_radius + 5f, 0f), 10f, m_radius * 2f);
		DebugDraw.CreateBox(CameraS.m_mainCamera, m_node1TC, new Vector3(0f - (m_radius + 5f), 0f), 10f, m_radius * 2f);
		DebugDraw.Clear(CameraS.m_mainCamera, m_node2TC);
		DebugDraw.CreateLine(CameraS.m_mainCamera, m_node2TC, Vector3.zero, Vector3.up * (m_radius + 25f));
		DebugDraw.CreateBox(CameraS.m_mainCamera, m_node2TC, new Vector3(0f, 0f - (m_radius + 5f)), m_radius * 2f, 10f);
		DebugDraw.CreateBox(CameraS.m_mainCamera, m_node2TC, new Vector3(m_radius + 5f, 0f), 10f, m_radius * 2f);
		DebugDraw.CreateBox(CameraS.m_mainCamera, m_node2TC, new Vector3(0f - (m_radius + 5f), 0f), 10f, m_radius * 2f);
	}

	public override void TeleportCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB];
		UnitC unitC = chipmunkBodyC.customComponent as UnitC;
		if (unitC != null && unitC.m_unit != null && _phase == ucpCollisionPhase.Begin && !unitC.m_unit.m_teleported)
		{
			if (_pair.shapeA == m_node1Shape && Vector3.Angle((Vector3)_pair.point - m_node1.m_position, m_node1.m_TC.transform.up) <= 45f)
			{
				unitC.m_unit.SetAsTeleporting(15, m_node1.m_TC.transform, m_node2.m_TC.transform, false, true);
			}
			else if (_pair.shapeA == m_node2Shape && Vector3.Angle((Vector3)_pair.point - m_node2.m_position, m_node2.m_TC.transform.up) <= 45f)
			{
				unitC.m_unit.SetAsTeleporting(15, m_node2.m_TC.transform, m_node1.m_TC.transform, false, true);
			}
		}
	}
}
