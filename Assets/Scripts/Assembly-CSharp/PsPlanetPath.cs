using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class PsPlanetPath : ISerializable
{
	public string m_name;

	public List<PsGameLoop> m_nodeInfos;

	public int m_currentNodeId;

	public int m_startNodeId;

	public int m_lane;

	public PsPlanetPath m_parentPath;

	public bool m_overwrite;

	public PsPlanetPath(string _name)
	{
		m_name = _name;
		m_nodeInfos = new List<PsGameLoop>();
		m_currentNodeId = 1;
		m_startNodeId = 0;
		m_lane = 0;
	}

	public PsPlanetPath(SerializationInfo info, StreamingContext ctxt)
	{
		m_name = (string)info.GetValue("name", typeof(string));
		m_currentNodeId = (int)info.GetValue("currentNodeId", typeof(int));
		m_startNodeId = (int)info.GetValue("startNodeId", typeof(int));
		m_lane = (int)info.GetValue("lane", typeof(int));
		m_nodeInfos = new List<PsGameLoop>((PsGameLoop[])info.GetValue("nodeInfos", typeof(PsGameLoop[])));
		for (int i = 0; i < m_nodeInfos.Count; i++)
		{
			m_nodeInfos[i].m_path = this;
		}
	}

	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		info.AddValue("name", m_name);
		info.AddValue("currentNodeId", m_currentNodeId);
		info.AddValue("nodeInfos", m_nodeInfos.ToArray());
		info.AddValue("startNodeId", m_startNodeId);
		info.AddValue("lane", m_lane);
	}

	public PsPlanetPath DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			return (PsPlanetPath)binaryFormatter.Deserialize(memoryStream);
		}
	}

	public PsGameLoop GetNodeInfo(int _nodeId)
	{
		if (_nodeId > 0 && m_nodeInfos.Count >= _nodeId)
		{
			return m_nodeInfos[_nodeId - 1];
		}
		return null;
	}

	public PsGameLoop GetNodeInfoFromPartialPath(int _nodeId)
	{
		for (int i = 0; i < m_nodeInfos.Count; i++)
		{
			if (m_nodeInfos[i].m_nodeId == _nodeId)
			{
				return m_nodeInfos[i];
			}
		}
		return null;
	}

	public PsGameLoop GetCurrentNodeInfo()
	{
		if (m_nodeInfos.Count >= m_currentNodeId && m_currentNodeId > 0)
		{
			return m_nodeInfos[m_currentNodeId - 1];
		}
		return null;
	}

	public int GetCurrentBlockId()
	{
		for (PsGameLoop nodeInfo = GetNodeInfo(m_currentNodeId); nodeInfo != null; nodeInfo = GetNodeInfo(nodeInfo.m_nodeId + 1))
		{
			if (nodeInfo.m_context == PsMinigameContext.Block || nodeInfo.m_context == PsMinigameContext.Signal)
			{
				return nodeInfo.m_nodeId;
			}
		}
		return -1;
	}

	public int GetNextBlockId(int _nextFromId)
	{
		if (m_nodeInfos.Count < _nextFromId)
		{
			return -1;
		}
		for (PsGameLoop nodeInfo = GetNodeInfo(_nextFromId + 1); nodeInfo != null; nodeInfo = GetNodeInfo(nodeInfo.m_nodeId + 1))
		{
			if (nodeInfo.m_context == PsMinigameContext.Block || nodeInfo.m_context == PsMinigameContext.Signal)
			{
				return nodeInfo.m_nodeId;
			}
		}
		return -1;
	}

	public int GetLastBlockId()
	{
		int num = m_nodeInfos.Count - 1;
		for (int num2 = num; num2 > -1; num2--)
		{
			PsGameLoop psGameLoop = m_nodeInfos[num2];
			if (psGameLoop.m_context == PsMinigameContext.Block || psGameLoop.m_context == PsMinigameContext.Signal)
			{
				return psGameLoop.m_nodeId;
			}
		}
		return 0;
	}

	public int GetLastLevelId()
	{
		int num = m_nodeInfos.Count - 1;
		for (int num2 = num; num2 > -1; num2--)
		{
			PsGameLoop psGameLoop = m_nodeInfos[num2];
			if (psGameLoop.m_context == PsMinigameContext.Level || psGameLoop.m_context == PsMinigameContext.EditorPuzzle || psGameLoop.m_context == PsMinigameContext.Versus || psGameLoop.m_context == PsMinigameContext.EditorTemplate)
			{
				return psGameLoop.m_nodeId;
			}
		}
		return 0;
	}

	public int GetPrevBlockId(int _prevFromId)
	{
		if (m_nodeInfos.Count < _prevFromId)
		{
			return -1;
		}
		for (PsGameLoop nodeInfo = GetNodeInfo(_prevFromId - 1); nodeInfo != null; nodeInfo = GetNodeInfo(nodeInfo.m_nodeId - 1))
		{
			if (nodeInfo.m_context == PsMinigameContext.Block || nodeInfo.m_context == PsMinigameContext.Signal)
			{
				return nodeInfo.m_nodeId;
			}
		}
		return 0;
	}

	public PsGameLoop GetCurrentSidePathNodeInfo()
	{
		int num = m_currentNodeId;
		PsGameLoop psGameLoop = null;
		while (num > 0)
		{
			PsGameLoop nodeInfo = GetNodeInfo(num);
			if (nodeInfo.m_sidePath != null)
			{
				psGameLoop = nodeInfo.m_sidePath.GetCurrentNodeInfo();
			}
			if (psGameLoop != null)
			{
				return psGameLoop;
			}
			num--;
		}
		return psGameLoop;
	}
}
