using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class GhostNode : ISerializable
{
	public string m_name;

	public Transform m_transform;

	public List<int> m_posX;

	public List<int> m_posY;

	public List<int> m_rot;

	public GhostNode(string _name, Transform _transform)
	{
		m_name = _name;
		m_posX = new List<int>();
		m_posY = new List<int>();
		m_rot = new List<int>();
		m_transform = _transform;
	}

	public GhostNode(SerializationInfo info, StreamingContext ctxt)
	{
		m_transform = null;
		m_name = (string)info.GetValue("name", typeof(string));
		m_posX = new List<int>((int[])info.GetValue("x", typeof(int[])));
		m_posY = new List<int>((int[])info.GetValue("y", typeof(int[])));
		m_rot = new List<int>((int[])info.GetValue("rot", typeof(int[])));
	}

	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		info.AddValue("name", m_name);
		info.AddValue("x", m_posX.ToArray());
		info.AddValue("y", m_posY.ToArray());
		info.AddValue("rot", m_rot.ToArray());
	}

	public void AddKeyFrame()
	{
		m_posX.Add((int)(m_transform.position.x * 100f));
		m_posY.Add((int)(m_transform.position.y * 100f));
		m_rot.Add((int)m_transform.rotation.eulerAngles.z * 100);
	}

	public Vector3 GetKeyFramePos(int _frame)
	{
		if (m_posX == null || m_posY == null || m_posX.Count == 0 || m_posY.Count == 0)
		{
			return Vector3.zero;
		}
		return new Vector3((float)m_posX[_frame] / 100f, (float)m_posY[_frame] / 100f);
	}

	public float GetKeyFrameRotation(int _frame)
	{
		if (m_rot == null || m_rot.Count == 0)
		{
			return 0f;
		}
		return (float)m_rot[_frame] / 100f;
	}
}
