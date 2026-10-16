using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class LevelRollingBoulderNode : GraphNode
{
	public PsRollingBoulderRadius m_boulderRadius;

	public Entity m_entity;

	public bool m_created = true;

	public static List<int> m_randomOrder = new List<int>();

	public LevelRollingBoulderNode(GraphNodeType _nodeType, Type _assembleClassType, string _name, Vector3 _pos, Vector3 _rot, Vector3 _sca)
		: base(_nodeType, _assembleClassType, _name, _pos, _rot, _sca)
	{
	}

	public LevelRollingBoulderNode(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		m_boulderRadius = (PsRollingBoulderRadius)(uint)info.GetValue("boulderRadius", typeof(uint));
		SetPropertyDefaults();
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
		info.AddValue("boulderRadius", (uint)m_boulderRadius);
	}

	public override GraphElement DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			LevelRollingBoulderNode levelRollingBoulderNode = (LevelRollingBoulderNode)binaryFormatter.Deserialize(memoryStream);
			CopyAttributes(levelRollingBoulderNode, this);
			levelRollingBoulderNode.m_created = false;
			levelRollingBoulderNode.m_storedRotation = Vector3.zero;
			if (levelRollingBoulderNode.m_childElements.Count != 0)
			{
				for (int i = 0; i < levelRollingBoulderNode.m_childElements.Count; i++)
				{
					CopyAttributes(levelRollingBoulderNode.m_childElements[i], m_childElements[i]);
					levelRollingBoulderNode.m_childElements[i].m_storedRotation = Vector3.zero;
				}
			}
			int max = Enum.GetNames(typeof(PsRollingBoulderRadius)).Length;
			int num;
			for (num = UnityEngine.Random.Range(0, max); num == (int)levelRollingBoulderNode.m_boulderRadius; num = UnityEngine.Random.Range(0, max))
			{
			}
			levelRollingBoulderNode.m_boulderRadius = (PsRollingBoulderRadius)(int)Enum.GetValues(typeof(PsRollingBoulderRadius)).GetValue(num);
			return levelRollingBoulderNode;
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
