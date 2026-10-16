using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class LevelSpawnPointNode : GraphNode
{
	public LevelSpawnPointNode(Type _assembleClassType, string _name, Vector3 _pos, Vector3 _rot, Vector3 _sca)
		: base(GraphNodeType.Normal, _assembleClassType, _name, _pos, _rot, _sca)
	{
		m_width = 15f;
		m_height = 15f;
		m_isRect = false;
	}

	public LevelSpawnPointNode(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		m_assembleClassType = Type.GetType((string)info.GetValue("assembleClassType", typeof(string)));
		SetPropertyDefaults();
	}

	public override void SetPropertyDefaults()
	{
		base.SetPropertyDefaults();
		m_isRemovable = false;
		m_isCopyable = false;
		m_isRotateable = false;
	}

	public override GraphElement DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			LevelSpawnPointNode levelSpawnPointNode = (LevelSpawnPointNode)binaryFormatter.Deserialize(memoryStream);
			CopyAttributes(levelSpawnPointNode, this);
			return levelSpawnPointNode;
		}
	}
}
