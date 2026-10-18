using System;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : Unit
{
	protected GameObject m_mainPrefab;

	protected GraphNode m_node1;

	protected GraphNode m_node2;

	protected GraphNode m_trigger;

	protected TransformC m_node1TC;

	protected TransformC m_node2TC;

	protected TransformC m_triggerTC;

	protected Transform m_base;

	protected Transform m_top;

	protected Transform m_bottom;

	protected float m_radius;

	protected float m_depth;

	protected ChipmunkBodyC m_triggerCMB;

	protected ChipmunkBodyC m_fieldCMB;

	protected float m_length;

	protected bool m_enabled;

	private EffectForceField m_effect;

	private static List<ForceFieldColor> m_reserverdColors = new List<ForceFieldColor>();

	private ForceFieldColor m_color;

	private int m_alienContactCount;

	private float m_cooldownOver;

	public ForceField(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_alienContactCount = 0;
		m_enabled = true;
		m_depth = 75f;
		GraphNode graphNode = m_graphElement as GraphNode;
		graphNode.m_moveChilds = false;
		graphNode.m_nodeType = GraphNodeType.Invisible;
		if (graphNode.m_childElements.Count == 0)
		{
			m_node1 = new GraphNode(GraphNodeType.Child, typeof(TouchableAssembledClass), "ForceNode1", _graphElement.m_position + Vector3.right * 50f + Vector3.forward * m_depth + Vector3.up * 100f, Vector3.zero, Vector3.one);
			m_node2 = new GraphNode(GraphNodeType.Child, typeof(TouchableAssembledClass), "ForceNode2", _graphElement.m_position + Vector3.right * -50f + Vector3.forward * m_depth + Vector3.up * 100f, Vector3.zero, Vector3.one);
			m_trigger = new GraphNode(GraphNodeType.IndependentChild, typeof(TouchableAssembledClass), "Trigger", _graphElement.m_position + Vector3.forward * m_depth, Vector3.zero, Vector3.one);
			graphNode.AddElement(m_node1);
			graphNode.AddElement(m_node2);
			graphNode.AddElement(m_trigger);
		}
		else
		{
			m_node1 = graphNode.m_childElements[0] as GraphNode;
			m_node2 = graphNode.m_childElements[1] as GraphNode;
			m_trigger = graphNode.m_childElements[2] as GraphNode;
			m_node1.m_position.z = m_depth;
			m_node2.m_position.z = m_depth;
			m_trigger.m_position.z = m_depth;
		}
		m_graphElement.m_position = (m_node1.m_position + m_node2.m_position) / 2f;
		m_node1.m_minDistanceFromParent = 50f;
		m_node1.m_maxDistanceFromParent = 1750f;
		m_node2.m_minDistanceFromParent = 50f;
		m_node2.m_maxDistanceFromParent = 1750f;
		m_trigger.m_minDistanceFromParent = 0f;
		m_trigger.m_maxDistanceFromParent = 1750f;
		m_node1.m_isRemovable = false;
		m_node2.m_isRemovable = false;
		m_trigger.m_isRemovable = true;
		m_node1.m_isCopyable = false;
		m_node2.m_isCopyable = false;
		m_trigger.m_isCopyable = true;
		m_trigger.m_isFlippable = true;
		m_trigger.m_isRotateable = false;
		m_trigger.m_isForegroundable = true;
		if (m_graphElement.m_storedRotation == Vector3.zero)
		{
			int i;
			for (i = 1; m_reserverdColors.Contains((ForceFieldColor)i); i++)
			{
			}
			m_color = (ForceFieldColor)i;
			m_graphElement.m_storedRotation = new Vector3(i, i, i);
		}
		else
		{
			if (m_graphElement.m_flipped)
			{
				int num = Enum.GetNames(typeof(ForceFieldColor)).Length;
				m_graphElement.m_flipped = false;
				if (m_reserverdColors.Count < num - 1)
				{
					int num2 = (int)m_graphElement.m_storedRotation.x + 1;
					if (num2 > num)
					{
						num2 = 1;
					}
					while (m_reserverdColors.Contains((ForceFieldColor)num2))
					{
						num2++;
						if (num2 > num)
						{
							num2 = 1;
						}
					}
					m_graphElement.m_storedRotation = new Vector3(num2, num2, num2);
				}
			}
			m_color = (ForceFieldColor)m_graphElement.m_storedRotation.x;
		}
		if (m_graphElement.m_inFront)
		{
			m_enabled = false;
		}
		m_reserverdColors.Add(m_color);
		m_radius = 50f;
		m_mainPrefab = ResourceManager.GetGameObject(RESOURCE.ForceFieldPrefab_GameObject);
		m_node1TC = TransformS.AddComponent(m_entity, "ForceNode1");
		TransformS.SetGlobalTransform(m_node1TC, m_node1.m_position + Vector3.forward * m_depth, m_node1.m_rotation);
		m_node2TC = TransformS.AddComponent(m_entity, "ForceNode2");
		TransformS.SetGlobalTransform(m_node2TC, m_node2.m_position + Vector3.forward * m_depth, m_node2.m_rotation);
		m_triggerTC = TransformS.AddComponent(m_entity, "Trigger");
		TransformS.SetGlobalTransform(m_triggerTC, m_trigger.m_position + Vector3.forward * m_depth, m_trigger.m_rotation);
		TransformC transformC = TransformS.AddComponent(m_entity, "Base");
		TransformS.SetGlobalTransform(transformC, m_trigger.m_position, Vector3.zero);
		PrefabC prefabC = PrefabS.AddComponent(transformC, Vector3.zero, m_mainPrefab);
		m_effect = prefabC.p_gameObject.GetComponent<EffectForceField>();
		m_effect.Initialize(m_color, m_enabled);
		m_base = prefabC.p_gameObject.transform.Find("ForceFieldButton");
		m_base.position = m_triggerTC.transform.position + Vector3.forward * m_depth * 0.5f;
		m_base.rotation = Quaternion.Euler(m_trigger.m_rotation);
		m_top = prefabC.p_gameObject.transform.Find("ForceFieldStart");
		m_top.position = m_node1TC.transform.position;
		m_bottom = prefabC.p_gameObject.transform.Find("ForceFieldEnd");
		m_bottom.position = m_node2TC.transform.position;
		TransformS.SetGlobalRotation(m_node1TC, m_top.transform.rotation.eulerAngles);
		TransformS.SetGlobalRotation(m_node2TC, m_bottom.transform.rotation.eulerAngles);
		float width = (m_length = ((Vector2)(m_node2.m_position - m_node1.m_position)).magnitude);
		if (m_minigame.m_editing)
		{
			return;
		}
		ucpPolyShape shape = new ucpPolyShape(50f, 50f, Vector2.zero, 257u, 1f, 0.5f, 0.5f, (ucpCollisionType)4, true);
		m_triggerCMB = ChipmunkProS.AddStaticBody(m_triggerTC, shape);
		ChipmunkProS.AddCollisionHandler(m_triggerCMB, TriggerCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)3, true, false, true);
		ucpPolyShape shape2 = new ucpPolyShape(width, 2f, Vector2.zero, 257u, 1f, 0.5f, 1f, (ucpCollisionType)4);
		m_fieldCMB = ChipmunkProS.AddStaticBody(m_graphElement.m_TC, shape2);
		m_fieldCMB.customComponent = m_unitC;
		if (!m_enabled)
		{
			for (int j = 0; j < m_fieldCMB.shapes.Count; j++)
			{
				ChipmunkProWrapper.ucpShapeSetLayers(m_fieldCMB.shapes[j].shapePtr, 0u);
			}
		}
	}

	public override void Update()
	{
		base.Update();
		if (m_minigame.m_editing)
		{
			TransformS.SetGlobalTransform(m_node1TC, m_node1.m_position, m_top.rotation.eulerAngles);
			TransformS.SetGlobalTransform(m_node2TC, m_node2.m_position, m_bottom.rotation.eulerAngles);
			TransformS.SetGlobalTransform(m_triggerTC, m_trigger.m_position, m_trigger.m_rotation);
			m_top.position = m_node1TC.transform.position;
			m_bottom.position = m_node2TC.transform.position;
			m_base.position = m_triggerTC.transform.position + Vector3.forward * m_depth * 0.5f;
			m_base.rotation = Quaternion.Euler(m_trigger.m_rotation);
			Vector2 vector = m_node1.m_position - m_node2.m_position;
			float magnitude = vector.magnitude;
			if (magnitude != m_length)
			{
				m_length = magnitude;
				float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
				m_graphElement.m_position = m_node2.m_position + (Vector3)vector * 0.5f;
				m_graphElement.m_rotation = Vector3.forward * num;
				TransformS.SetGlobalTransform(m_graphElement.m_TC, m_graphElement.m_position, m_graphElement.m_rotation);
			}
		}
	}

	public void InteractWithIntersectingObjects(bool _kill)
	{
		if (_kill)
		{
			ucpPointQueryInfo[] array = new ucpPointQueryInfo[100];
			ChipmunkProWrapper.ucpSpaceShapeQuery(m_fieldCMB.shapes[0].shapePtr, array, 100);
			for (int i = 0; i < array.Length && !(array[i].shape == IntPtr.Zero); i++)
			{
				IntPtr intPtr = ChipmunkProWrapper.ucpShapeGetBody(array[i].shape);
				for (int num = ChipmunkProS.m_bodies.m_array.Length - 1; num >= 0; num--)
				{
					if (intPtr == ChipmunkProS.m_bodies.m_array[num].body)
					{
						ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[num];
						UnitC unitC = chipmunkBodyC.customComponent as UnitC;
						if (unitC != null && unitC.m_unit != null)
						{
							unitC.m_unit.EmergencyKill();
						}
						break;
					}
				}
			}
		}
		else
		{
			for (int j = 0; j < m_contacts.Count; j++)
			{
				ChipmunkProWrapper.ucpBodyActivate(m_contacts[j].m_cmb.body);
			}
		}
	}

	public virtual void TriggerCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB];
		UnitC unitC = chipmunkBodyC.customComponent as UnitC;
		if (unitC == null || unitC.m_unit == null)
		{
			return;
		}
		switch (_phase)
		{
		case ucpCollisionPhase.Begin:
			if (m_alienContactCount == 0 && m_cooldownOver < Main.m_resettingGameTime)
			{
				m_cooldownOver = Main.m_resettingGameTime + 0.5f;
				m_enabled = !m_enabled;
				m_effect.ToggleForceField(m_enabled);
				for (int i = 0; i < m_fieldCMB.shapes.Count; i++)
				{
					ChipmunkProWrapper.ucpShapeSetLayers(m_fieldCMB.shapes[i].shapePtr, m_enabled ? m_fieldCMB.shapes[i].layerMask : 0u);
				}
				InteractWithIntersectingObjects(m_enabled);
			}
			m_alienContactCount++;
			break;
		case ucpCollisionPhase.Separate:
			m_alienContactCount--;
			break;
		}
	}

	public override void Destroy()
	{
		m_reserverdColors.Remove(m_color);
		base.Destroy();
	}
}
