using System;
using UnityEngine;

public class PsBlock : Unit
{
	public TransformC m_tc;

	public PrefabC m_pc;

	public ChipmunkBodyC m_cmb;

	public LevelBlockNode m_blockGraphNode;

	public GameObject m_mainPrefab;

	public GameObject m_blockGameobject;

	public float m_area;

	private float m_width;

	private float m_height;

	protected float m_massMultiplier;

	protected float m_friction;

	protected float m_elasticity;

	protected string m_materialName;

	protected bool randomizeRotation;

	public PsBlock(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		SetPhysicsParameters();
		m_mainPrefab = ResourceManager.GetGameObject(RESOURCE.BlocksPrefab_GameObject);
		m_blockGraphNode = _graphElement as LevelBlockNode;
		m_tc = TransformS.AddComponent(m_entity, "Block");
		TransformS.SetTransform(m_tc, m_blockGraphNode.m_position, m_blockGraphNode.m_rotation);
		if (m_minigame.m_editing)
		{
			int num = Enum.GetNames(typeof(PsBlockShape)).Length;
			if (m_blockGraphNode.m_flipped || !m_blockGraphNode.m_created)
			{
				if (!m_blockGraphNode.m_created)
				{
					randomizeRotation = true;
				}
				if (m_blockGraphNode.m_flipped)
				{
					randomizeRotation = true;
					int num2 = (int)(m_blockGraphNode.m_shape + 1);
					if (num2 >= num)
					{
						num2 = 0;
					}
					m_blockGraphNode.m_shape = (PsBlockShape)(int)Enum.GetValues(typeof(PsBlockShape)).GetValue(num2);
				}
				m_blockGraphNode.m_created = true;
				m_blockGraphNode.m_flipped = false;
			}
		}
		LoadObject();
		m_area = m_width * m_height;
		m_checkForCrushing = true;
		CreateEditorTouchArea();
	}

	public virtual void SetPhysicsParameters()
	{
		m_massMultiplier = 0.015f;
		m_friction = 0.9f;
		m_elasticity = 0.1f;
	}

	public override void EmergencyKill()
	{
		if (!m_isDead)
		{
			Kill(DamageType.Impact, float.MaxValue);
		}
	}

	public virtual void LoadObject()
	{
		int num = 0;
		int num2 = 0;
		switch (m_blockGraphNode.m_shape)
		{
		case PsBlockShape.Rect5050:
			m_blockGameobject = m_mainPrefab.transform.Find("WoodenBox50x50").gameObject;
			m_width = 50f;
			m_height = 50f;
			if (randomizeRotation)
			{
				num = UnityEngine.Random.Range(-1, 2);
				num2 = UnityEngine.Random.Range(0, 2);
			}
			break;
		case PsBlockShape.Rect10050:
			m_blockGameobject = m_mainPrefab.transform.Find("WoodenBox100x50").gameObject;
			m_width = 100f;
			m_height = 50f;
			if (randomizeRotation)
			{
				num2 = UnityEngine.Random.Range(0, 2);
			}
			break;
		case PsBlockShape.Rect10025:
			m_blockGameobject = m_mainPrefab.transform.Find("WoodenBox100x25").gameObject;
			m_width = 100f;
			m_height = 25f;
			if (randomizeRotation)
			{
				num2 = UnityEngine.Random.Range(0, 2);
			}
			break;
		case PsBlockShape.Rect100100:
			m_blockGameobject = m_mainPrefab.transform.Find("WoodenBox100x100").gameObject;
			m_width = 100f;
			m_height = 100f;
			if (randomizeRotation)
			{
				Debug.Log("Randomize values");
				num = UnityEngine.Random.Range(-1, 2);
				num2 = UnityEngine.Random.Range(0, 2);
			}
			break;
		case PsBlockShape.HalfRect5050:
			m_blockGameobject = m_mainPrefab.transform.Find("WoodenBox50x50T").gameObject;
			m_width = 50f;
			m_height = 25f;
			break;
		}
		if (randomizeRotation)
		{
			m_graphElement.m_storedRotation = new Vector3(0f, (float)num2 * 180f, (float)num * 90f);
		}
	}

	public virtual void CreatePhysicsAndModel()
	{
		ucpPolyShape shape = ChipmunkProS.GeneratePolyShapeFromGameObject(m_blockGameobject.transform.GetChild(0).gameObject, Vector2.zero, m_area * m_massMultiplier, m_elasticity, m_friction, (ucpCollisionType)4, 17895696u);
		m_blockGameobject.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
		m_cmb = ChipmunkProS.AddDynamicBody(m_tc, shape);
		m_cmb.customComponent = m_unitC;
		m_pc = PrefabS.AddComponent(m_tc, Vector3.zero, m_blockGameobject);
		m_pc.p_gameObject.transform.localRotation = Quaternion.identity;
		m_pc.p_gameObject.GetComponent<Renderer>().material = ResourceManager.GetMaterial(m_materialName + "_Material");
		m_pc.p_gameObject.transform.Rotate(m_graphElement.m_storedRotation);
	}
}
