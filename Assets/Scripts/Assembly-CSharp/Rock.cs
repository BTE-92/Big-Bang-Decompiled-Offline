using UnityEngine;

public class Rock : Unit
{
	public ChipmunkBodyC m_cmb;

	private LevelRockNode m_rockGraphNode;

	public Vector3 m_centeroid;

	public float m_area;

	public Rock(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_rockGraphNode = _graphElement as LevelRockNode;
		TransformC transformC = TransformS.AddComponent(m_entity, "Rock");
		TransformS.SetTransform(transformC, m_rockGraphNode.m_position, m_rockGraphNode.m_rotation);
		if (m_rockGraphNode.m_rockVertices == null)
		{
			CreateVertices();
		}
		else
		{
			Vector2[] rockVertices = m_rockGraphNode.m_rockVertices;
			m_rockGraphNode.m_totalVertexCount = ChipmunkProWrapper.ucpConvexHull(rockVertices.Length, rockVertices, 0f);
			m_rockGraphNode.m_rockVertices = new Vector2[m_rockGraphNode.m_totalVertexCount];
			for (int i = 0; i < m_rockGraphNode.m_totalVertexCount; i++)
			{
				m_rockGraphNode.m_rockVertices[i] = rockVertices[i];
			}
		}
		m_centeroid = ChipmunkProWrapper.ucpCenteroidForPoly(m_rockGraphNode.m_totalVertexCount, m_rockGraphNode.m_rockVertices);
		m_area = ChipmunkProWrapper.ucpAreaForPoly(m_rockGraphNode.m_totalVertexCount, m_rockGraphNode.m_rockVertices, 0f);
		ucpPolyShape shape = new ucpPolyShape(m_rockGraphNode.m_rockVertices, -m_centeroid, 17895696u, m_area * 0.03f, 0.1f, 0.9f, (ucpCollisionType)4);
		m_cmb = ChipmunkProS.AddDynamicBody(transformC, shape);
		m_cmb.customComponent = m_unitC;
		GGData gGData = new GGData(m_rockGraphNode.m_rockVertices);
		gGData.ReverseVertexOrder();
		gGData.SetOffset(-m_centeroid + Vector3.forward * (-15f - m_rockGraphNode.m_rockRadius * 0.25f));
		gGData.RadialShift(-0.5f);
		gGData.SetUvMapPlanar(0.012f, 0.012f, Vector2.zero);
		Mesh mesh = GeometryGenerator.GenerateFlatMesh(gGData);
		GGData gGData2 = new GGData(m_rockGraphNode.m_rockVertices);
		gGData2.ReverseVertexOrder();
		gGData2.SetOffset(-m_centeroid + Vector3.forward * -10f);
		gGData2.SetUvMapPlanar(0.012f, 0.012f, Vector2.zero);
		gGData2.SplitVertices(45f);
		gGData.SplitVertices(45f);
		Mesh mesh2 = GeometryGenerator.GenerateBeltMesh(0f, gGData2, gGData);
		GGData gGData3 = gGData2.Copy();
		gGData2.SetUvMapBelt(0.012f, 0f, true);
		gGData3.SetUvMapBelt(0.012f, 0.5f, true);
		Mesh mesh3 = GeometryGenerator.GenerateBeltMesh(m_rockGraphNode.m_rockRadius * 0.8f, gGData2, gGData3);
		PrefabS.CreatePrefabFromMeshArray(transformC, new Mesh[3] { mesh, mesh2, mesh3 }, CameraS.m_mainCamera, ResourceManager.GetMaterial(RESOURCE.RockMaterial_Material), true, true, true);
		m_checkForCrushing = true;
		CreateEditorTouchArea();
	}

	public override void EmergencyKill()
	{
		if (!m_isDead)
		{
			Vector2 vector = ChipmunkProWrapper.ucpBodyGetPos(m_cmb.body);
			SoundS.PlaySingleShot("/Ingame/Units/WoodenCrateDestroy", vector);
			EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject(RESOURCE.WoodenCratePuff_GameObject), new Vector3(vector.x, vector.y, 0f), Vector3.zero, 1f, "GTAG_AUTODESTROY");
			Destroy();
		}
	}

	public void CreateVertices()
	{
		int max = 10;
		m_rockGraphNode.m_rockRadius = Random.Range(30f, 60f);
		Vector2[] circle = DebugDraw.GetCircle(m_rockGraphNode.m_rockRadius, Random.Range(3, max), Vector2.zero, false);
		DebugDraw.AddRadialRandom(circle, m_rockGraphNode.m_rockRadius * 0.5f);
		GGData gGData = new GGData(circle);
		gGData.ReverseVertexOrder();
		circle = gGData.ToVector2Array();
		m_rockGraphNode.m_totalVertexCount = ChipmunkProWrapper.ucpConvexHull(circle.Length, circle, 0f);
		m_rockGraphNode.m_rockVertices = new Vector2[m_rockGraphNode.m_totalVertexCount];
		for (int i = 0; i < m_rockGraphNode.m_totalVertexCount; i++)
		{
			m_rockGraphNode.m_rockVertices[i] = circle[i];
		}
	}
}
