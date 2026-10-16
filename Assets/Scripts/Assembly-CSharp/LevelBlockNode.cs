using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class LevelBlockNode : GraphNode
{
	public PsBlockShape m_shape;

	public Entity m_entity;

	public bool m_created = true;

	public static List<int> m_randomOrder = new List<int>();

	public LevelBlockNode(GraphNodeType _nodeType, Type _assembleClassType, string _name, Vector3 _pos, Vector3 _rot, Vector3 _sca)
		: base(_nodeType, _assembleClassType, _name, _pos, _rot, _sca)
	{
	}

	public LevelBlockNode(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		m_shape = (PsBlockShape)(uint)info.GetValue("shape", typeof(uint));
		SetPropertyDefaults();
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
		info.AddValue("shape", (uint)m_shape);
	}

	public override GraphElement DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			LevelBlockNode levelBlockNode = (LevelBlockNode)binaryFormatter.Deserialize(memoryStream);
			CopyAttributes(levelBlockNode, this);
			levelBlockNode.m_created = false;
			levelBlockNode.m_storedRotation = Vector3.zero;
			if (levelBlockNode.m_childElements.Count != 0)
			{
				for (int i = 0; i < levelBlockNode.m_childElements.Count; i++)
				{
					CopyAttributes(levelBlockNode.m_childElements[i], m_childElements[i]);
					levelBlockNode.m_childElements[i].m_storedRotation = Vector3.zero;
				}
			}
			int max = Enum.GetNames(typeof(PsBlockShape)).Length;
			int num;
			for (num = UnityEngine.Random.Range(0, max); num == (int)levelBlockNode.m_shape; num = UnityEngine.Random.Range(0, max))
			{
			}
			levelBlockNode.m_shape = (PsBlockShape)(int)Enum.GetValues(typeof(PsBlockShape)).GetValue(num);
			return levelBlockNode;
		}
	}

	public override void SetPropertyDefaults()
	{
		base.SetPropertyDefaults();
		m_isFlippable = true;
		m_isRemovable = true;
		m_isCopyable = true;
		m_isRotateable = true;
	}
}
