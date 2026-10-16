using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class LevelRockNode : GraphNode
{
	public Vector2[] m_rockVertices;

	public int m_totalVertexCount;

	public float m_rockRadius;

	public Entity m_entity;

	public LevelRockNode(GraphNodeType _nodeType, Type _assembleClassType, string _name, Vector3 _pos, Vector3 _rot, Vector3 _sca)
		: base(_nodeType, _assembleClassType, _name, _pos, _rot, _sca)
	{
	}

	public LevelRockNode(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		try
		{
			m_rockRadius = (float)info.GetValue("rockRadius", typeof(float));
		}
		catch
		{
			m_rockRadius = (float)info.GetValue("radius", typeof(float));
		}
		m_totalVertexCount = (int)info.GetValue("vertexCount", typeof(int));
		m_rockVertices = new Vector2[m_totalVertexCount];
		for (int i = 0; i < m_totalVertexCount; i++)
		{
			m_rockVertices[i] = new Vector2((float)info.GetValue("pos" + i + "x", typeof(float)), (float)info.GetValue("pos" + i + "y", typeof(float)));
		}
		SetPropertyDefaults();
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
		info.AddValue("rockRadius", m_rockRadius);
		info.AddValue("vertexCount", m_totalVertexCount);
		for (int i = 0; i < m_totalVertexCount; i++)
		{
			info.AddValue("pos" + i + "x", m_rockVertices[i].x);
			info.AddValue("pos" + i + "y", m_rockVertices[i].y);
		}
	}

	public override GraphElement DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			LevelRockNode levelRockNode = (LevelRockNode)binaryFormatter.Deserialize(memoryStream);
			CopyAttributes(levelRockNode, this);
			levelRockNode.m_rockVertices = null;
			levelRockNode.m_totalVertexCount = 0;
			return levelRockNode;
		}
	}

	public override void SetPropertyDefaults()
	{
		base.SetPropertyDefaults();
		m_isRemovable = true;
		m_isCopyable = true;
		m_isRotateable = true;
	}
}
