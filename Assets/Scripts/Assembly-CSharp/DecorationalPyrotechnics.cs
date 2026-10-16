using UnityEngine;

public class DecorationalPyrotechnics : Unit
{
	public GraphNode m_pyroNode;

	public GameObject m_mainPrefab;

	public PrefabC m_confetti;

	public TransformC m_tc;

	private bool m_triggered;

	public DecorationalPyrotechnics(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_pyroNode = m_graphElement as GraphNode;
		m_tc = TransformS.AddComponent(m_entity, "Confetti");
		TransformS.SetTransform(m_tc, m_pyroNode.m_position + new Vector3(0f, 0f, 64f) + GetZBufferBias(), m_pyroNode.m_rotation);
		m_mainPrefab = ResourceManager.GetGameObject(RESOURCE.DecorationPyrotechnicsConfettiWorkPrefab_GameObject);
		m_confetti = PrefabS.AddComponent(m_tc, Vector3.zero, m_mainPrefab, string.Empty);
		m_pyroNode.m_isFlippable = true;
		if (m_pyroNode.m_flipped)
		{
			TransformS.SetScale(m_tc, new Vector3(-1f, 1f, 1f));
		}
		CreateEditorTouchArea(50f, 50f, m_tc);
		if (!m_minigame.m_editing)
		{
			ucpCircleShape shape = new ucpCircleShape(250f, Vector2.zero, 16777216u, 1f, 1f, 1f, (ucpCollisionType)10, true);
			ChipmunkBodyC c = ChipmunkProS.AddStaticBody(m_tc, shape);
			ChipmunkProS.AddCollisionHandler(c, CollisionHandler, (ucpCollisionType)10, (ucpCollisionType)3, true, false, false);
		}
		m_triggered = false;
	}

	private void CollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		if (!m_triggered)
		{
			ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB];
			UnitC unitC = chipmunkBodyC.customComponent as UnitC;
			if (unitC != null && unitC.m_unit != null)
			{
				m_confetti.p_gameObject.transform.GetChild(0).gameObject.SendMessage("PlayEffect");
				m_confetti.p_gameObject.transform.GetChild(1).gameObject.SendMessage("PlayEffect");
				m_triggered = true;
			}
		}
	}
}
