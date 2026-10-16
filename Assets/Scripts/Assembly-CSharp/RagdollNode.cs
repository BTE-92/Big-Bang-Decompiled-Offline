using System;
using System.Collections.Generic;
using UnityEngine;

public class RagdollNode
{
	public string m_name;

	public RagdollNode m_parent;

	public List<RagdollNode> m_childs;

	public Transform m_transform;

	public Quaternion m_originalRot;

	public Vector3 m_centeroid;

	public Vector3 m_offset;

	public bool m_flipped;

	public TransformC m_tc;

	public ChipmunkBodyC m_cmb;

	public IntPtr m_pivotJoint;

	public IntPtr m_rotaryLimitJoint;

	public RagdollJointBootstrap m_ragdollJointBootstrap;

	public RagdollNode(Transform _transform)
	{
		m_transform = _transform;
		m_name = _transform.name;
		m_childs = new List<RagdollNode>();
		m_ragdollJointBootstrap = m_transform.gameObject.AddComponent<RagdollJointBootstrap>();
		m_ragdollJointBootstrap.m_ragdollNode = this;
	}

	public RagdollNode FindChild(string _name)
	{
		for (int i = 0; i < m_childs.Count; i++)
		{
			if (m_childs[i].m_name == _name)
			{
				return m_childs[i];
			}
			m_childs[i].FindChild(_name);
		}
		return null;
	}
}
