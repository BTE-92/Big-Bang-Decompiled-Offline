using System;
using System.Collections.Generic;
using UnityEngine;

public class TeleportBase : Unit
{
	protected GameObject m_mainPrefab;

	protected GraphNode m_node1;

	protected GraphNode m_node2;

	protected TransformC m_node1TC;

	protected TransformC m_node2TC;

	protected List<Renderer> m_renderers;

	protected int[] m_renderIndices1;

	protected int[] m_renderIndices2;

	protected PrefabC m_portal1;

	protected PrefabC m_portal2;

	protected IntPtr m_node1Shape;

	protected IntPtr m_node2Shape;

	protected float m_radius;

	protected float m_depth;

	public TeleportBase(GraphElement _graphElement, float _radius, string _mainPrefabName, float _depth = 0f)
		: base(_graphElement, UnitType.Basic)
	{
		m_depth = _depth;
		GraphNode graphNode = m_graphElement as GraphNode;
		graphNode.m_moveChilds = false;
		graphNode.m_nodeType = GraphNodeType.Invisible;
		if (graphNode.m_childElements.Count == 0)
		{
			m_node1 = new GraphNode(GraphNodeType.IndependentChild, typeof(TouchableAssembledClass), "TeleportChildNode1", _graphElement.m_position + Vector3.right * 100f + Vector3.forward * _depth, Vector3.zero, Vector3.one);
			m_node2 = new GraphNode(GraphNodeType.IndependentChild, typeof(TouchableAssembledClass), "TeleportChildNode2", _graphElement.m_position + Vector3.right * -100f + Vector3.forward * _depth, Vector3.zero, Vector3.one);
			graphNode.AddElement(m_node1);
			graphNode.AddElement(m_node2);
		}
		else
		{
			m_node1 = graphNode.m_childElements[0] as GraphNode;
			m_node2 = graphNode.m_childElements[1] as GraphNode;
			m_node1.m_position.z = _depth;
			m_node2.m_position.z = _depth;
		}
		m_graphElement.m_position = (m_node1.m_position + m_node2.m_position) / 2f;
		m_node1.m_minDistanceFromParent = 100f;
		m_node1.m_maxDistanceFromParent = 1500f;
		m_node2.m_minDistanceFromParent = 100f;
		m_node2.m_maxDistanceFromParent = 1500f;
		m_radius = _radius;
		m_mainPrefab = ResourceManager.GetGameObject(_mainPrefabName + "_GameObject");
		m_renderers = new List<Renderer>();
		m_node1TC = TransformS.AddComponent(m_entity, "TeleportNode1");
		TransformS.SetGlobalTransform(m_node1TC, m_node1.m_position, m_node1.m_rotation);
		GameObject gameObject = m_mainPrefab.transform.Find("Teleport1").gameObject;
		m_portal1 = PrefabS.AddComponent(m_node1TC, Vector3.zero, gameObject);
		Renderer[] componentsInChildren = m_portal1.p_gameObject.GetComponentsInChildren<Renderer>();
		m_renderIndices1 = new int[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			m_renderers.Add(componentsInChildren[i]);
			m_renderIndices1[i] = i;
		}
		m_node2TC = TransformS.AddComponent(m_entity, "TeleportNode2");
		TransformS.SetGlobalTransform(m_node2TC, m_node2.m_position, m_node2.m_rotation);
		GameObject gameObject2 = m_mainPrefab.transform.Find("Teleport2").gameObject;
		m_portal2 = PrefabS.AddComponent(m_node2TC, Vector3.zero, gameObject2);
		componentsInChildren = m_portal2.p_gameObject.GetComponentsInChildren<Renderer>();
		m_renderIndices2 = new int[componentsInChildren.Length];
		int count = m_renderers.Count;
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			m_renderers.Add(componentsInChildren[j]);
			m_renderIndices2[j] = count + j;
		}
		if (!m_minigame.m_editing)
		{
			ucpCircleShape ucpCircleShape2 = new ucpCircleShape(m_radius, Vector2.zero, 257u, 1f, 0.1f, 0.1f, (ucpCollisionType)4, true);
			ChipmunkBodyC c = ChipmunkProS.AddStaticBody(m_node1TC, ucpCircleShape2);
			m_node1Shape = ucpCircleShape2.shapePtr;
			ucpCircleShape ucpCircleShape3 = new ucpCircleShape(m_radius, Vector2.zero, 257u, 1f, 0.1f, 0.1f, (ucpCollisionType)4, true);
			ChipmunkBodyC c2 = ChipmunkProS.AddStaticBody(m_node2TC, ucpCircleShape3);
			m_node2Shape = ucpCircleShape3.shapePtr;
			ChipmunkProS.AddCollisionHandler(c, TeleportCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)3, true, false, true);
			ChipmunkProS.AddCollisionHandler(c2, TeleportCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)3, true, false, true);
			ChipmunkProS.AddCollisionHandler(c, TeleportCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)4, true, false, true);
			ChipmunkProS.AddCollisionHandler(c2, TeleportCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)4, true, false, true);
		}
	}

	public override void Update()
	{
		base.Update();
		if (m_minigame.m_editing)
		{
			TransformS.SetGlobalTransform(m_node1TC, m_node1.m_position, m_node1.m_rotation);
			TransformS.SetGlobalTransform(m_node2TC, m_node2.m_position, m_node2.m_rotation);
			m_graphElement.m_position = (m_node1.m_position + m_node2.m_position) / 2f;
			TransformS.SetGlobalTransform(m_graphElement.m_TC, m_graphElement.m_position, m_graphElement.m_rotation);
		}
	}

	public virtual void TeleportCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
	}
}
