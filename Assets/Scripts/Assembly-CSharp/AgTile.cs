using System;
using UnityEngine;

public class AgTile
{
	private static int m_instanceCount;

	public IntPtr tilePtr;

	public IntPtr[] shapes;

	public int shapeCount;

	public AgTileEdgeVert[] edgeVerts;

	public TransformC TC;

	public Vector2 pos;

	public cpBB bb;

	public bool dirty;

	public bool regenerateCollisionShapes;

	public AutoGeometryProps props;

	public AutoGeometryLayer agLayer;

	public AgTile(int _shapeSpace, AutoGeometryLayer _layer)
	{
		agLayer = _layer;
		shapes = new IntPtr[_shapeSpace];
		shapeCount = 0;
		dirty = false;
		edgeVerts = null;
		regenerateCollisionShapes = true;
		props = null;
		pos = Vector3.zero;
		m_instanceCount++;
	}

	public void Destroy()
	{
		props.Destroy();
		props = null;
		for (int i = 0; i < shapeCount; i++)
		{
			ChipmunkProWrapper.ucpRemoveShape(shapes[i]);
		}
		shapeCount = 0;
		shapes = null;
		edgeVerts = null;
		EntityManager.RemoveEntity(TC.p_entity);
		TC = null;
	}

	~AgTile()
	{
		m_instanceCount--;
	}
}
