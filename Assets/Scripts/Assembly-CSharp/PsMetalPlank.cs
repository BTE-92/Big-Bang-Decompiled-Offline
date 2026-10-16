using System;
using System.Collections.Generic;
using UnityEngine;

public class PsMetalPlank : Unit
{
	private TransformC m_tc;

	private ChipmunkBodyC m_cmb;

	private PrefabC m_pc;

	private Mesh m_plankMesh;

	private GraphNode m_node1;

	private GraphNode m_node2;

	private float m_length;

	private float m_density;

	private float m_elasticity;

	private float m_friction;

	public PsMetalPlank(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_density = 0.01f;
		m_elasticity = 0.1f;
		m_friction = 0.9f;
		m_canElectrify = true;
		m_checkForCrushing = true;
		GraphNode graphNode = m_graphElement as GraphNode;
		if (graphNode.m_childElements.Count == 0)
		{
			m_node1 = new GraphNode(GraphNodeType.Child, typeof(TouchableAssembledClass), "PlankChildNode1", _graphElement.m_position + Vector3.right * 100f + Vector3.forward * -5f, Vector3.zero, Vector3.one);
			m_node2 = new GraphNode(GraphNodeType.Child, typeof(TouchableAssembledClass), "PlankChildNode2", _graphElement.m_position + Vector3.right * -100f + Vector3.forward * -5f, Vector3.zero, Vector3.one);
			graphNode.AddElement(m_node1);
			graphNode.AddElement(m_node2);
		}
		else
		{
			m_node1 = graphNode.m_childElements[0] as GraphNode;
			m_node2 = graphNode.m_childElements[1] as GraphNode;
		}
		m_node1.m_minDistanceFromParent = 50f;
		m_node1.m_maxDistanceFromParent = 200f;
		m_node2.m_minDistanceFromParent = 50f;
		m_node2.m_maxDistanceFromParent = 200f;
		Vector2 vector = m_node2.m_position - m_node1.m_position;
		float num = (m_length = vector.magnitude);
		float num2 = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		float mass = num * 10f * m_density;
		m_tc = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetGlobalTransform(m_tc, _graphElement.m_position, _graphElement.m_rotation);
		ucpPolyShape shape = new ucpPolyShape(num, 10f, Vector2.zero, 17895696u, mass, m_elasticity, m_friction, (ucpCollisionType)4);
		m_cmb = ChipmunkProS.AddDynamicBody(m_tc, shape);
		m_cmb.customComponent = m_unitC;
		m_pc = PrefabS.AddComponent(m_tc, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.PlankPrefab_GameObject));
		m_pc.p_gameObject.transform.localScale = new Vector3(num / 100f, 1f, 1f);
		m_plankMesh = m_pc.p_gameObject.transform.Find("plank").gameObject.GetComponent<MeshFilter>().mesh;
		CalculateUVs(num);
		CreateEditorTouchArea(num, 10f);
	}

	public override void SyncPositionToGraphElementPosition()
	{
		for (int i = 0; i < m_assembledEntities.Count; i++)
		{
			List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.ChipmunkBody, m_assembledEntities[i]);
			for (int j = 0; j < componentsByEntity.Count; j++)
			{
				ChipmunkBodyC chipmunkBodyC = componentsByEntity[j] as ChipmunkBodyC;
				ChipmunkProWrapper.ucpBodySetPos(chipmunkBodyC.body, m_graphElement.m_position);
				ChipmunkProWrapper.ucpBodySetAngle(chipmunkBodyC.body, m_graphElement.m_rotation.z * ((float)Math.PI / 180f));
				TransformS.SetGlobalPosition(chipmunkBodyC.TC, m_graphElement.m_position);
				TransformS.SetGlobalRotation(chipmunkBodyC.TC, m_graphElement.m_rotation);
			}
		}
	}

	public void CalculateUVs(float _len)
	{
		Vector2[] uv = m_plankMesh.uv;
		for (int i = 0; i < uv.Length; i++)
		{
			if (uv[i].x > 0.05f)
			{
				uv[i].x = _len / 100f;
			}
		}
		m_plankMesh.uv = uv;
	}

	public override void Update()
	{
		base.Update();
		if (m_minigame.m_editing)
		{
			Vector2 vector = m_node1.m_position - m_node2.m_position;
			float magnitude = vector.magnitude;
			if (magnitude != m_length)
			{
				m_length = magnitude;
				float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
				float num2 = magnitude * 10f * m_density;
				m_graphElement.m_position = m_node2.m_position + (Vector3)vector * 0.5f;
				m_graphElement.m_rotation = Vector3.forward * num;
				TransformS.SetGlobalTransform(m_graphElement.m_TC, m_graphElement.m_position, m_graphElement.m_rotation);
				m_pc.p_gameObject.transform.localScale = new Vector3(magnitude / 100f, 1f, 1f);
				CalculateUVs(magnitude);
			}
		}
	}

	public override void EmergencyKill()
	{
		if (!m_isDead)
		{
			Vector2 vector = ChipmunkProWrapper.ucpBodyGetPos(m_cmb.body);
			SoundS.PlaySingleShot("/Ingame/Units/WoodenCrateDestroy", vector);
			EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject(RESOURCE.MetalBoxCrush_GameObject), new Vector3(vector.x, vector.y, 0f), Vector3.zero, 1f, "GTAG_AUTODESTROY");
			Destroy();
		}
	}

	public override void SetElectrified(bool _electrify, bool _affectContacts, Vector2 _contactPoint)
	{
		base.SetElectrified(_electrify, _affectContacts, _contactPoint);
		if (!(m_pc.p_gameObject == null))
		{
			if (_electrify)
			{
				PrefabS.SetShaderColor(m_pc, Color.red);
			}
			else
			{
				PrefabS.SetShaderColor(m_pc, Color.white);
			}
		}
	}
}
