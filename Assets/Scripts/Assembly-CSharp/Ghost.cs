using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Ghost : ISerializable
{
	public string m_name = "Unknown";

	public string m_unitClass;

	public bool m_recording;

	public bool m_playing;

	public Hashtable m_nodes;

	public float m_playbackTick;

	public int m_keyframeCount;

	public Hashtable m_upgradeValues;

	public List<int> m_upgradeSteps;

	private float m_scaledTick;

	public Ghost(string _unitClass, Hashtable _upgradeValues, List<int> _upgradeSteps, bool _record = true)
	{
		m_unitClass = _unitClass;
		m_nodes = new Hashtable();
		m_upgradeValues = _upgradeValues;
		m_upgradeSteps = _upgradeSteps;
		Init(_record);
	}

	public Ghost(SerializationInfo info, StreamingContext ctxt)
	{
		Init(false);
		m_keyframeCount = (int)info.GetValue("keyframeCount", typeof(int));
		m_nodes = (Hashtable)info.GetValue("nodes", typeof(Hashtable));
		try
		{
			m_unitClass = info.GetString("unitClass");
		}
		catch
		{
			m_unitClass = string.Empty;
		}
		try
		{
			m_upgradeValues = (Hashtable)info.GetValue("upgradeValues", typeof(Hashtable));
		}
		catch
		{
			m_upgradeValues = null;
		}
		try
		{
			m_upgradeSteps = (List<int>)info.GetValue("upgradeSteps", typeof(List<int>));
		}
		catch
		{
			m_upgradeSteps = null;
		}
		Debug.Log("Ghost deserialized... keyframes: " + m_keyframeCount);
	}

	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		info.AddValue("keyframeCount", m_keyframeCount);
		info.AddValue("nodes", m_nodes);
		info.AddValue("unitClass", m_unitClass);
		info.AddValue("upgradeValues", m_upgradeValues);
		info.AddValue("upgradeSteps", m_upgradeSteps);
		Debug.Log("Ghost serialized... keyframes: " + m_keyframeCount);
	}

	public Ghost DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			return (Ghost)binaryFormatter.Deserialize(memoryStream);
		}
	}

	public void Init(bool _record)
	{
		m_recording = _record;
		m_playing = !_record;
		m_playbackTick = 0f;
		m_keyframeCount = 0;
		if (_record)
		{
			Debug.Log("New ghost created for recording.");
		}
		else
		{
			Debug.Log("New playback ghost created.");
		}
	}

	public void AddNode(string _name, Transform _TC)
	{
		GhostNode ghostNode = new GhostNode(_name, _TC);
		m_nodes.Add(_name, ghostNode);
		Debug.Log("Node " + _name + " added to ghost.");
		ghostNode.AddKeyFrame();
	}

	public virtual void Update()
	{
		if (m_recording)
		{
			bool flag = Main.m_timeScale >= 1f;
			if (!flag)
			{
				m_scaledTick += Main.m_timeScale;
				if (m_scaledTick >= 1f)
				{
					flag = true;
					m_scaledTick -= 1f;
				}
			}
			else
			{
				m_scaledTick = 0f;
			}
			if (!flag)
			{
				return;
			}
			foreach (GhostNode value in m_nodes.Values)
			{
				value.AddKeyFrame();
			}
			m_keyframeCount++;
		}
		else if (m_playing)
		{
			m_playbackTick = Mathf.Min(m_playbackTick + 1f * Main.m_timeScale, m_keyframeCount - 1);
		}
	}

	public void StopRecord()
	{
		if (m_recording)
		{
			m_recording = false;
		}
	}

	public bool PlaybackEnded()
	{
		return m_playbackTick == (float)(m_keyframeCount - 1);
	}

	public Vector3 GetCurrentPosition(GhostNode _node)
	{
		float num = m_playbackTick - (float)Mathf.FloorToInt(m_playbackTick);
		if (num != 0f)
		{
			Vector3 keyFramePos = _node.GetKeyFramePos(Mathf.FloorToInt(m_playbackTick));
			Vector3 keyFramePos2 = _node.GetKeyFramePos(Mathf.CeilToInt(m_playbackTick));
			Vector3 vector = keyFramePos2 - keyFramePos;
			return keyFramePos + vector * num;
		}
		return _node.GetKeyFramePos(Mathf.FloorToInt(m_playbackTick));
	}

	public float GetCurrentRotation(GhostNode _node)
	{
		float num = m_playbackTick - (float)Mathf.FloorToInt(m_playbackTick);
		if (num != 0f)
		{
			float keyFrameRotation = _node.GetKeyFrameRotation(Mathf.FloorToInt(m_playbackTick));
			float keyFrameRotation2 = _node.GetKeyFrameRotation(Mathf.CeilToInt(m_playbackTick));
			float num2 = keyFrameRotation2 - keyFrameRotation;
			if (num2 > 180f)
			{
				num2 -= 360f;
			}
			else if (num2 < -180f)
			{
				num2 += 360f;
			}
			return keyFrameRotation + num2 * num;
		}
		return _node.GetKeyFrameRotation(Mathf.FloorToInt(m_playbackTick));
	}

	public int GetNextKeyframe(string _nodeName, int _startKeyframe, float _distanceToTravel)
	{
		_distanceToTravel *= _distanceToTravel;
		GhostNode ghostNode = m_nodes[_nodeName] as GhostNode;
		int result = _startKeyframe;
		float num = 0f;
		Vector2 vector = ghostNode.GetKeyFramePos(_startKeyframe);
		for (int i = _startKeyframe + 1; i < m_keyframeCount; i++)
		{
			Vector2 vector2 = ghostNode.GetKeyFramePos(i);
			num = (vector2 - vector).sqrMagnitude;
			if (num >= _distanceToTravel)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public float GetPathWorldDistance(string _nodeName)
	{
		if (m_keyframeCount == 0)
		{
			return 0f;
		}
		GhostNode ghostNode = m_nodes[_nodeName] as GhostNode;
		float num = 0f;
		Vector2 vector = ghostNode.GetKeyFramePos(0);
		for (int i = 1; i < m_keyframeCount; i++)
		{
			Vector2 vector2 = ghostNode.GetKeyFramePos(i);
			num += (vector2 - vector).magnitude;
			vector = vector2;
		}
		return num;
	}

	public virtual void Destroy()
	{
		if (m_recording)
		{
			Debug.Log("Destroying recording ghost");
		}
		else
		{
			Debug.Log("Destroying playback ghost");
		}
		m_nodes.Clear();
		m_nodes = null;
		if (m_upgradeValues != null)
		{
			m_upgradeValues.Clear();
			m_upgradeValues = null;
		}
	}

	public static byte[] SerializeToBytes(Ghost _ghost)
	{
		return SerializeToStream(_ghost).ToArray();
	}

	public static MemoryStream SerializeToStream(Ghost _ghost)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		binaryFormatter.Serialize(memoryStream, _ghost);
		return memoryStream;
	}

	public static Ghost DeSerializeFromBytes(byte[] _bytes)
	{
		Debug.Log("Deserializing ghost...");
		MemoryStream memoryStream = new MemoryStream(_bytes, 0, _bytes.Length, false, true);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		Ghost ghost = null;
		try
		{
			ghost = (Ghost)binaryFormatter.Deserialize(memoryStream);
		}
		catch (InvalidCastException ex)
		{
			Debug.LogWarning(ex.Message);
			ghost = null;
		}
		memoryStream.Close();
		return ghost;
	}
}
