using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class LevelCustomNode : GraphNode
{
	public uint m_uint;

	public int m_int;

	public bool m_bool;

	public float m_float;

	public string m_string;

	public LevelCustomNode(GraphNodeType _nodeType, Type _assembleClassType, string _name, Vector3 _pos, Vector3 _rot, Vector3 _sca)
		: base(_nodeType, _assembleClassType, _name, _pos, _rot, _sca)
	{
		m_uint = 0u;
		m_int = 0;
		m_bool = false;
		m_float = 0f;
		m_string = string.Empty;
	}

	public LevelCustomNode(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		m_uint = (uint)info.GetValue("uint", typeof(uint));
		m_int = (int)info.GetValue("int", typeof(int));
		m_bool = (bool)info.GetValue("bool", typeof(bool));
		m_float = (float)info.GetValue("float", typeof(float));
		m_string = (string)info.GetValue("string", typeof(string));
		SetPropertyDefaults();
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
		info.AddValue("uint", m_uint);
		info.AddValue("int", m_int);
		info.AddValue("bool", m_bool);
		info.AddValue("float", m_float);
		info.AddValue("string", m_string);
	}

	public override GraphElement DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			LevelCustomNode levelCustomNode = (LevelCustomNode)binaryFormatter.Deserialize(memoryStream);
			CopyAttributes(levelCustomNode, this);
			levelCustomNode.m_storedRotation = Vector3.zero;
			if (levelCustomNode.m_childElements.Count != 0)
			{
				for (int i = 0; i < levelCustomNode.m_childElements.Count; i++)
				{
					CopyAttributes(levelCustomNode.m_childElements[i], m_childElements[i]);
					levelCustomNode.m_childElements[i].m_storedRotation = Vector3.zero;
				}
			}
			levelCustomNode.m_uint = m_uint;
			levelCustomNode.m_int = m_int;
			levelCustomNode.m_bool = m_bool;
			levelCustomNode.m_float = m_float;
			levelCustomNode.m_string = m_string;
			return levelCustomNode;
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
