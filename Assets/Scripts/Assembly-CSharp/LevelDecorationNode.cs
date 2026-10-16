using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class LevelDecorationNode : GraphNode
{
	public uint m_index;

	public LevelDecorationNode(GraphNodeType _nodeType, Type _assembleClassType, string _name, Vector3 _pos, Vector3 _rot, Vector3 _sca)
		: base(_nodeType, _assembleClassType, _name, _pos, _rot, _sca)
	{
		m_index = 0u;
	}

	public LevelDecorationNode(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		m_index = (uint)info.GetValue("index", typeof(uint));
		SetPropertyDefaults();
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
		info.AddValue("index", m_index);
	}

	public override GraphElement DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			LevelDecorationNode levelDecorationNode = (LevelDecorationNode)binaryFormatter.Deserialize(memoryStream);
			CopyAttributes(levelDecorationNode, this);
			levelDecorationNode.m_flipped = false;
			levelDecorationNode.m_inFront = false;
			levelDecorationNode.m_storedRotation = Vector3.zero;
			if (levelDecorationNode.m_childElements.Count != 0)
			{
				for (int i = 0; i < levelDecorationNode.m_childElements.Count; i++)
				{
					CopyAttributes(levelDecorationNode.m_childElements[i], m_childElements[i]);
					levelDecorationNode.m_childElements[i].m_storedRotation = Vector3.zero;
				}
			}
			levelDecorationNode.m_index = m_index;
			return levelDecorationNode;
		}
	}

	public override void SetPropertyDefaults()
	{
		base.SetPropertyDefaults();
		m_isFlippable = true;
		m_isRemovable = true;
		m_isCopyable = true;
		m_isRotateable = true;
		m_isForegroundable = false;
	}
}
