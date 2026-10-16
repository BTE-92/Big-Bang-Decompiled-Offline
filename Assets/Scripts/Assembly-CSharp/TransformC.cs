using System.Collections.Generic;
using UnityEngine;

public class TransformC : BasicComponent
{
	public static int m_componentCount;

	public bool updatePosition;

	public bool updateRotation;

	public bool updateScale;

	public bool updatedPosition;

	public bool updatedRotation;

	public bool updatedScale;

	public TransformC parent;

	public List<TransformC> childs;

	public int level;

	public bool parentedToPhysics;

	public Vector3 lastLocalPos;

	public Vector3 delta;

	public bool forceRotation;

	public Quaternion forcedRotation = Quaternion.identity;

	public bool forceScale;

	public Vector3 forcedScale = Vector3.one;

	public Transform transform;

	public TransformC()
		: base(ComponentType.Transform)
	{
		transform = Object.Instantiate(TransformS.m_transformHelper).transform;
		childs = new List<TransformC>();
		m_componentCount++;
	}

	public override void Reset()
	{
		base.Reset();
		transform.name = "TransformComponent";
		transform.gameObject.SetActive(false);
		childs.Clear();
		forceRotation = false;
		forceScale = false;
		parent = null;
		m_active = false;
		updatedPosition = false;
		updatedRotation = false;
		updatedScale = false;
		updatePosition = true;
		updateRotation = true;
		updateScale = true;
		transform.localScale = Vector3.one;
		transform.position = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		forcedRotation = Quaternion.identity;
		forcedScale = Vector3.one;
		level = 0;
		parentedToPhysics = false;
		delta = Vector3.zero;
		lastLocalPos = Vector3.zero;
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	~TransformC()
	{
		m_componentCount--;
	}

	public TransformC GetRoot()
	{
		TransformC transformC = this;
		while (transformC.parent != null)
		{
			transformC = transformC.parent;
		}
		return transformC;
	}
}
