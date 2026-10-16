using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class LevelTextNode : GraphNode
{
	public string m_text;

	public Entity m_entity;

	public LevelTextNode(GraphNodeType _nodeType, Type _assembleClassType, string _name, Vector3 _pos, Vector3 _rot, Vector3 _sca)
		: base(_nodeType, _assembleClassType, _name, _pos, _rot, _sca)
	{
		m_text = "Lorem ipsum dolor sit amet.";
	}

	public LevelTextNode(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		m_text = (string)info.GetValue("text", typeof(string));
		SetPropertyDefaults();
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
		info.AddValue("text", m_text);
	}

	public override GraphElement DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			LevelTextNode levelTextNode = (LevelTextNode)binaryFormatter.Deserialize(memoryStream);
			CopyAttributes(levelTextNode, this);
			return levelTextNode;
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
