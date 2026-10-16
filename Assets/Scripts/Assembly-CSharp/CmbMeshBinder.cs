using System.Collections.Generic;
using UnityEngine;

public class CmbMeshBinder
{
	public class CmbMeshBindPoint
	{
		public ChipmunkBodyC m_cmb;

		public Vector2 m_localPoint;

		public Vector2 m_originalWorldPoint;

		public Vector2 m_pointInPrefabSpace;

		public List<int> m_vertexIndices;

		public CmbMeshBindPoint()
		{
			m_vertexIndices = new List<int>();
		}
	}

	public List<CmbMeshBindPoint> m_bindPoints;

	public PrefabC m_prefab;

	private Vector3[] m_originalVertPositions;

	public CmbMeshBinder()
	{
		m_bindPoints = new List<CmbMeshBindPoint>();
	}

	public void AddBindPoint(ChipmunkBodyC _cmb, Vector2 _localPoint)
	{
		CmbMeshBindPoint cmbMeshBindPoint = new CmbMeshBindPoint();
		cmbMeshBindPoint.m_cmb = _cmb;
		cmbMeshBindPoint.m_localPoint = _localPoint;
		cmbMeshBindPoint.m_originalWorldPoint = ChipmunkProWrapper.ucpBodyLocal2World(cmbMeshBindPoint.m_cmb.body, cmbMeshBindPoint.m_localPoint);
		m_bindPoints.Add(cmbMeshBindPoint);
	}

	public void Bind(PrefabC _prefab)
	{
		m_prefab = _prefab;
		m_originalVertPositions = m_prefab.p_mesh.vertices;
		for (int i = 0; i < m_bindPoints.Count; i++)
		{
			CmbMeshBindPoint cmbMeshBindPoint = m_bindPoints[i];
			cmbMeshBindPoint.m_pointInPrefabSpace = m_prefab.p_parentTC.transform.InverseTransformPoint(cmbMeshBindPoint.m_originalWorldPoint);
		}
		for (int j = 0; j < m_originalVertPositions.Length; j++)
		{
			Vector3 vertexWorldCoord = m_prefab.p_gameObject.transform.TransformPoint(m_originalVertPositions[j]);
			CmbMeshBindPoint cmbMeshBindPoint2 = FindBindPointForVertex(vertexWorldCoord);
			cmbMeshBindPoint2.m_vertexIndices.Add(j);
		}
	}

	public void UpdateMesh()
	{
		Vector3[] array = new Vector3[m_originalVertPositions.Length];
		for (int i = 0; i < m_bindPoints.Count; i++)
		{
			CmbMeshBindPoint cmbMeshBindPoint = m_bindPoints[i];
			Vector2 pointInPrefabSpace = cmbMeshBindPoint.m_pointInPrefabSpace;
			Vector2 vector = m_prefab.p_parentTC.transform.InverseTransformPoint(ChipmunkProWrapper.ucpBodyLocal2World(cmbMeshBindPoint.m_cmb.body, cmbMeshBindPoint.m_localPoint));
			Vector3 vector2 = pointInPrefabSpace - vector;
			for (int j = 0; j < cmbMeshBindPoint.m_vertexIndices.Count; j++)
			{
				int num = cmbMeshBindPoint.m_vertexIndices[j];
				array[num] = m_originalVertPositions[num] - vector2;
			}
		}
		m_prefab.p_mesh.vertices = array;
		m_prefab.p_mesh.RecalculateBounds();
		m_prefab.p_mesh.RecalculateNormals();
	}

	private CmbMeshBindPoint FindBindPointForVertex(Vector3 _vertexWorldCoord)
	{
		float num = float.MaxValue;
		int num2 = -1;
		for (int i = 0; i < m_bindPoints.Count; i++)
		{
			float sqrMagnitude = (_vertexWorldCoord - (Vector3)m_bindPoints[i].m_originalWorldPoint).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				num2 = i;
			}
		}
		if (num2 >= 0)
		{
			return m_bindPoints[num2];
		}
		return null;
	}
}
