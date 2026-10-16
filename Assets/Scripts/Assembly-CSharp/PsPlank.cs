using System.Collections.Generic;
using UnityEngine;

public class PsPlank : Unit
{
	private const float PLANK_HEIGHT = 10f;

	private const float PLANK_DEPTH = 50f;

	private List<TransformC> m_tc;

	private List<ChipmunkBodyC> m_cmb;

	private PrefabC m_pc;

	private CmbMeshBinder m_meshBinder;

	private GraphNode m_node1;

	private GraphNode m_node2;

	private float m_length;

	private float m_density;

	private float m_elasticity;

	private float m_friction;

	public PsPlank(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_tc = new List<TransformC>();
		m_cmb = new List<ChipmunkBodyC>();
		m_density = 0.01f;
		m_elasticity = 0.1f;
		m_friction = 0.9f;
		m_hitPoints = 1f;
		m_hitPointType = HitPointType.Lives;
		m_canBurn = true;
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
		m_node1.m_minDistanceFromParent = 75f;
		m_node1.m_maxDistanceFromParent = 200f;
		m_node2.m_minDistanceFromParent = 75f;
		m_node2.m_maxDistanceFromParent = 200f;
		CreatePlank();
		CreateEditorTouchArea(m_length, 10f);
	}

	public override void SetAllBaseArmours()
	{
		base.SetAllBaseArmours();
		SetBaseArmor(DamageType.Weapon, 20f);
		SetBaseArmor(DamageType.Fire, 20f);
	}

	public void CreatePlankMesh(TransformC _tc, Vector2[] verts)
	{
		if (m_pc != null)
		{
			PrefabS.RemoveComponent(m_pc);
		}
		GGData gGData = new GGData(verts);
		gGData.SetOffset(new Vector3(0f, 0f, -25f));
		for (int i = 0; i < gGData.m_vertices.Count; i++)
		{
			GGVertex gGVertex = gGData.m_vertices[i];
			gGVertex.uv = new Vector2(gGVertex.vertex.x / 70f, 0.8f + 0.2f * (gGVertex.vertex.y + 5f) / 10f);
		}
		Mesh mesh = GeometryGenerator.GenerateFlatMesh(gGData);
		GGData gGData2 = new GGData(verts);
		GGData gGData3 = new GGData(verts);
		gGData2.SetOffset(new Vector3(0f, 0f, -25f));
		gGData3.SetOffset(new Vector3(0f, 0f, -25f));
		for (int j = 0; j < gGData2.m_vertices.Count; j++)
		{
			GGVertex gGVertex2 = gGData2.m_vertices[j];
			GGVertex gGVertex3 = gGData3.m_vertices[j];
			gGVertex2.uv = new Vector2(gGVertex2.vertex.x / 70f, 0.79f);
			gGVertex3.uv = new Vector2(gGVertex3.vertex.x / 70f, 0.19f);
		}
		Mesh mesh2 = GeometryGenerator.GenerateBeltMesh(50f, gGData2, gGData3);
		m_pc = PrefabS.CreatePrefabFromMeshArray(_tc, new Mesh[2] { mesh, mesh2 }, CameraS.m_mainCamera, ResourceManager.GetMaterial(RESOURCE.BambooMaterial_Material), true, true, true);
		m_pc.p_mesh.MarkDynamic();
	}

	public override void SyncPositionToGraphElementPosition()
	{
		Vector2 vector = m_node2.m_position - m_node1.m_position;
		if (m_length != vector.magnitude)
		{
			CreatePlank();
		}
	}

	public Vector2[] GetSegmentChainPoints(Vector2 _start, Vector2 _end, float _segmentLength)
	{
		Vector2 vector = _end - _start;
		float magnitude = vector.magnitude;
		int num = Mathf.CeilToInt(magnitude / _segmentLength);
		float num2 = magnitude / (float)num;
		Vector2[] array = new Vector2[num + 1];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = _start + vector.normalized * num2 * i;
		}
		return array;
	}

	public Vector2[] GetSegmentChainVerts(Vector2 _offset, Vector2[] _chainPoints, float _plankHeight)
	{
		int num = _chainPoints.Length * 2;
		Vector2[] array = new Vector2[num];
		float magnitude = (_chainPoints[1] - _chainPoints[0]).magnitude;
		Vector2 vector = new Vector2((0f - magnitude) * 0.5f, 0f);
		for (int i = 0; i < _chainPoints.Length; i++)
		{
			Vector2 vector2 = new Vector2(0f, 1f) * _plankHeight * 0.5f;
			if (i > 0)
			{
				vector += new Vector2(magnitude, 0f);
			}
			array[i] = vector + vector2 + _offset;
			array[num - 1 - i] = vector - vector2 + _offset;
		}
		return array;
	}

	private void CreatePlank()
	{
		for (int i = 0; i < m_tc.Count; i++)
		{
			if (m_cmb.Count == m_tc.Count)
			{
				ChipmunkProS.RemoveBody(m_cmb[i]);
			}
			TransformS.RemoveComponent(m_tc[i]);
		}
		m_cmb.Clear();
		m_tc.Clear();
		Vector2[] segmentChainPoints = GetSegmentChainPoints(m_node1.m_position, m_node2.m_position, 70f);
		m_length = (m_node2.m_position - m_node1.m_position).magnitude;
		float num = m_length * 10f * m_density;
		int num2 = segmentChainPoints.Length - 1;
		m_meshBinder = null;
		if (!m_minigame.m_editing)
		{
			m_meshBinder = new CmbMeshBinder();
		}
		for (int j = 0; j < segmentChainPoints.Length; j++)
		{
			if (j <= 0)
			{
				continue;
			}
			Vector2 vector = segmentChainPoints[j] - segmentChainPoints[j - 1];
			Vector2 vector2 = segmentChainPoints[j - 1] + vector * 0.5f;
			float num3 = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			TransformC transformC = TransformS.AddComponent(m_entity, m_graphElement.m_name + "_" + j);
			TransformS.SetPosition(transformC, vector2);
			TransformS.SetRotation(transformC, Vector3.forward * num3);
			if (m_minigame.m_editing)
			{
				TransformS.ParentComponent(transformC, m_graphElement.m_TC);
			}
			else
			{
				Vector2 vector3 = new Vector2((0f - vector.magnitude) * 0.5f, 0f);
				Vector2 vector4 = new Vector2(vector.magnitude * 0.5f, 0f);
				ucpSegmentShape ucpSegmentShape2 = new ucpSegmentShape(vector3, vector4, 5f, Vector2.zero, 17895696u, num / (float)num2, m_elasticity, m_friction, (ucpCollisionType)4);
				ucpSegmentShape2.group = GetGroup();
				ChipmunkBodyC chipmunkBodyC = ChipmunkProS.AddDynamicBody(transformC, ucpSegmentShape2, m_unitC);
				if (j == 1)
				{
					m_meshBinder.AddBindPoint(chipmunkBodyC, vector3 + new Vector2(0f, 5f));
					m_meshBinder.AddBindPoint(chipmunkBodyC, vector3 + new Vector2(0f, -5f));
				}
				m_meshBinder.AddBindPoint(chipmunkBodyC, vector4 + new Vector2(0f, 5f));
				m_meshBinder.AddBindPoint(chipmunkBodyC, vector4 + new Vector2(0f, -5f));
				if (j > 1)
				{
					ChipmunkProWrapper.ucpAddConstraint(ChipmunkProWrapper.ucpPivotJointNew(m_cmb[m_cmb.Count - 1].body, chipmunkBodyC.body, segmentChainPoints[j - 1]), -1);
					TransformC anchor = TransformS.AddComponent(m_entity);
					ChipmunkProS.AddDampedRotarySpring(chipmunkBodyC, m_cmb[m_cmb.Count - 1], anchor, 0f, 8000000 * num2, 150000f);
				}
				m_cmb.Add(chipmunkBodyC);
			}
			m_tc.Add(transformC);
		}
		Vector2[] segmentChainVerts = GetSegmentChainVerts(Vector2.zero, segmentChainPoints, 10f);
		CreatePlankMesh(m_tc[0], segmentChainVerts);
		if (m_meshBinder != null)
		{
			m_meshBinder.Bind(m_pc);
		}
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
				m_graphElement.m_position = m_node2.m_position + (Vector3)vector * 0.5f;
				TransformS.SetGlobalTransform(m_graphElement.m_TC, m_graphElement.m_position, m_graphElement.m_rotation);
			}
			return;
		}
		if (m_pc.p_gameObject.GetComponent<Renderer>().isVisible)
		{
			m_meshBinder.UpdateMesh();
		}
		for (int i = 1; i < m_cmb.Count; i++)
		{
			float z = m_tc[i - 1].transform.eulerAngles.z;
			float z2 = m_tc[i].transform.eulerAngles.z;
			float f = z - z2;
			float f2 = z - 360f - z2;
			float f3 = z - (z2 - 360f);
			if (Mathf.Abs(f) > 25f && Mathf.Abs(f2) > 25f && Mathf.Abs(f3) > 25f)
			{
				EmergencyKill();
				break;
			}
		}
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		if (!m_isDead)
		{
			base.Kill(_damageType, _totalDamage);
			Vector2 vector = ChipmunkProWrapper.ucpBodyGetPos(m_cmb[Mathf.RoundToInt((float)m_cmb.Count * 0.5f)].body);
			SoundS.PlaySingleShot("/Ingame/Units/WoodenCrateDestroy", vector);
			EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject(RESOURCE.WoodenCratePuff_GameObject), new Vector3(vector.x, vector.y, 0f), Vector3.zero, 1f, "GTAG_AUTODESTROY");
			PsAchievementManager.IncrementProgress("breakTwoHundredPlanks", 1);
			PsAchievementManager.IncrementProgress("breakTwoThousandPlanks", 1);
			Destroy();
		}
	}

	public override void EmergencyKill()
	{
		if (!m_isDead)
		{
			Vector2 vector = ChipmunkProWrapper.ucpBodyGetPos(m_cmb[Mathf.RoundToInt((float)m_cmb.Count * 0.5f)].body);
			SoundS.PlaySingleShot("/Ingame/Units/WoodenCrateDestroy", vector);
			EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject(RESOURCE.WoodenCratePuff_GameObject), new Vector3(vector.x, vector.y, 0f), Vector3.zero, 1f, "GTAG_AUTODESTROY");
			PsAchievementManager.IncrementProgress("breakTwoHundredPlanks", 1);
			PsAchievementManager.IncrementProgress("breakTwoThousandPlanks", 1);
			Destroy();
		}
	}

	public override void SetBurning(bool _burn)
	{
		if (_burn)
		{
			PrefabS.SetShaderColor(m_pc, Color.red);
		}
		else
		{
			PrefabS.SetShaderColor(m_pc, Color.white);
		}
	}
}
