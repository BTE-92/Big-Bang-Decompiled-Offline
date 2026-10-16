using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class GraphSlotNode : GraphNode
{
	public SlotType m_slotType;

	public List<GraphNode> m_externalNodes;

	public int[] m_externalGraphIds;

	public int[] m_externalNodeIds;

	public GraphSlotNode(GraphNodeType _nodeType, string _name, Vector3 _pos, Vector3 _rot)
		: base(_nodeType, _name)
	{
		m_position = _pos;
		m_rotation = _rot;
		m_width = 15f;
		m_height = 15f;
		m_isRect = false;
		m_externalNodes = new List<GraphNode>();
	}

	public GraphSlotNode(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		m_slotType = (SlotType)(int)info.GetValue("slotType", typeof(SlotType));
		m_externalNodeIds = (int[])info.GetValue("externalNodeIds", typeof(int[]));
	}

	public override void Initialize()
	{
		base.Initialize();
	}

	public override void Assemble()
	{
		m_assembled = true;
	}

	public override GraphElement DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			GraphSlotNode graphSlotNode = (GraphSlotNode)binaryFormatter.Deserialize(memoryStream);
			CopyAttributes(graphSlotNode, this);
			return graphSlotNode;
		}
	}

	public override void OnDeserialization(object sender)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
		info.AddValue("slotType", m_slotType);
		info.AddValue("externalNodes", m_externalNodeIds);
	}
}
