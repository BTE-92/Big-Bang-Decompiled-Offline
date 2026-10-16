using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class GraphNode : GraphElement, ISerializable, IDeserializationCallback
{
	public GraphNodeType m_nodeType;

	public Type m_assembleClassType;

	public List<GraphElement> m_childElements;

	public GraphElement[] m_tempElements;

	public bool m_rotateChilds;

	public bool m_moveChilds;

	public float m_minDistanceFromParent;

	public float m_maxDistanceFromParent;

	public PullMode m_pullMode;

	public TriggerMode m_triggerMode;

	public bool m_triggerFlag;

	public double m_triggerInterval;

	public float m_resourceCount;

	public float m_maxResourceCount;

	public GraphNode(GraphNodeType _nodeType)
		: base("GraphNode")
	{
		m_childElements = new List<GraphElement>();
		m_nodeType = _nodeType;
		m_elementType = GraphElementType.Node;
		m_pullMode = PullMode.PullAny;
		m_triggerMode = TriggerMode.Passive;
		m_triggerInterval = 0.0;
		m_resourceCount = 0f;
		m_maxResourceCount = -1f;
	}

	public GraphNode(GraphNodeType _nodeType, Type _assembleClassType, string _name, Vector3 _pos, Vector3 _rot, Vector3 _sca)
		: base(_name)
	{
		Debug.Log("GraphNode Constructor");
		m_childElements = new List<GraphElement>();
		m_nodeType = _nodeType;
		m_elementType = GraphElementType.Node;
		m_assembleClassType = _assembleClassType;
		m_position = _pos;
		m_rotation = _rot;
		m_scale = _sca;
		m_width = 15f;
		m_height = 15f;
		m_isRect = false;
	}

	public GraphNode(GraphNodeType _nodeType, string _name)
		: base(_name)
	{
		m_childElements = new List<GraphElement>();
		m_nodeType = _nodeType;
		m_elementType = GraphElementType.Node;
	}

	public GraphNode(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		m_nodeType = (GraphNodeType)(uint)info.GetValue("nodeType", typeof(uint));
		m_pullMode = (PullMode)(uint)info.GetValue("pullMode", typeof(uint));
		m_triggerMode = (TriggerMode)(uint)info.GetValue("triggerMode", typeof(uint));
		try
		{
			m_triggerInterval = (float)info.GetValue("triggerInterval", typeof(float));
		}
		catch
		{
			m_triggerInterval = 0.0;
		}
		m_tempElements = (GraphElement[])info.GetValue("elements", typeof(GraphElement[]));
		m_assembleClassType = Type.GetType((string)info.GetValue("assembleClassType", typeof(string)));
	}

	public override void SetPropertyDefaults()
	{
		base.SetPropertyDefaults();
		m_moveChilds = true;
		m_rotateChilds = true;
	}

	public override void Initialize()
	{
		base.Initialize();
		for (int i = 0; i < m_childElements.Count; i++)
		{
			m_childElements[i].Initialize();
		}
	}

	public override void Assemble()
	{
		base.Assemble();
		if (m_assembleClassType != null)
		{
			object[] args = new object[1] { this };
			IAssembledClass item = Activator.CreateInstance(m_assembleClassType, args) as IAssembledClass;
			m_assembledClasses.Add(item);
			LevelManager.m_currentLevel.AddItem(m_assembleClassType.ToString());
		}
		for (int i = 0; i < m_childElements.Count; i++)
		{
			m_childElements[i].Assemble();
		}
	}

	public GraphElement GetElement(uint _id)
	{
		for (int i = 0; i < m_childElements.Count; i++)
		{
			if (m_childElements[i].m_id == _id)
			{
				return m_childElements[i];
			}
		}
		return null;
	}

	public GraphElement GetElement(string _name)
	{
		for (int i = 0; i < m_childElements.Count; i++)
		{
			if (m_childElements[i].m_name == _name)
			{
				return m_childElements[i];
			}
		}
		return null;
	}

	public GraphElement[] GetElements(string _name)
	{
		List<GraphElement> list = new List<GraphElement>();
		for (int i = 0; i < m_childElements.Count; i++)
		{
			if (m_childElements[i].m_name == _name)
			{
				list.Add(m_childElements[i]);
			}
		}
		return list.ToArray();
	}

	public void AddElement(GraphElement _element)
	{
		_element.m_parentElement = this;
		if (!m_childElements.Contains(_element))
		{
			m_childElements.Add(_element);
		}
	}

	public bool RemoveElement(GraphElement _element)
	{
		return m_childElements.Remove(_element);
	}

	public override void Dispose()
	{
		if (m_assembleClassType != null)
		{
			LevelManager.m_currentLevel.RemoveItem(m_assembleClassType.ToString());
		}
		while (m_childElements.Count > 0)
		{
			int index = m_childElements.Count - 1;
			m_childElements[index].Dispose();
		}
		m_childElements = null;
		base.Clear(false);
		base.Dispose();
	}

	public override void Clear(bool _isReset)
	{
		if (m_childElements != null)
		{
			for (int i = 0; i < m_childElements.Count; i++)
			{
				m_childElements[i].Clear(_isReset);
			}
		}
		base.Clear(_isReset);
	}

	public override void Reset()
	{
		if (m_assembleClassType != null)
		{
			LevelManager.m_currentLevel.RemoveItem(m_assembleClassType.ToString());
		}
		for (int i = 0; i < m_childElements.Count; i++)
		{
			m_childElements[i].Reset();
		}
		base.Reset();
	}

	public override void Update()
	{
		if (!m_disabled)
		{
			for (int i = 0; i < m_childElements.Count; i++)
			{
				m_childElements[i].Update();
			}
		}
	}

	public override void Trigger()
	{
		if (m_disabled)
		{
			return;
		}
		if (m_pullMode == PullMode.TriggerAny)
		{
			for (int i = 0; i < m_outputs.Count; i++)
			{
				m_outputs[i].Trigger();
			}
		}
		else if (m_pullMode == PullMode.TriggerAllOrNone)
		{
			if (ResourceDemandSatisfied(m_stateOutputCount))
			{
				for (int j = 0; j < m_outputs.Count; j++)
				{
					m_outputs[j].Trigger();
				}
			}
		}
		else if (m_pullMode == PullMode.PullAny)
		{
			for (int k = 0; k < m_inputs.Count; k++)
			{
				m_inputs[k].Pull();
			}
		}
		else if (m_pullMode == PullMode.PullAndTriggerAny)
		{
			for (int l = 0; l < m_inputs.Count; l++)
			{
				m_inputs[l].Pull();
			}
			for (int m = 0; m < m_outputs.Count; m++)
			{
				m_outputs[m].Trigger();
			}
		}
		else if (m_pullMode == PullMode.PullAllOrNone)
		{
			if (EnoughResourcesToPull(m_stateInputCount))
			{
				for (int n = 0; n < m_inputs.Count; n++)
				{
					m_inputs[n].Pull();
				}
			}
		}
		else
		{
			if (m_pullMode != PullMode.PullAndTriggerAllOrNone || !EnoughResourcesToPull(m_stateInputCount))
			{
				return;
			}
			for (int num = 0; num < m_inputs.Count; num++)
			{
				m_inputs[num].Pull();
			}
			if (ResourceDemandSatisfied(m_stateOutputCount))
			{
				for (int num2 = 0; num2 < m_outputs.Count; num2++)
				{
					m_outputs[num2].Trigger();
				}
			}
		}
	}

	public virtual bool ResourceDemandSatisfied(int _index)
	{
		if (_index == m_outputs.Count)
		{
			return true;
		}
		float num = 0f;
		for (int i = _index; i < m_outputs.Count; i++)
		{
			num += m_outputs[i].m_label.m_value;
		}
		if (m_resourceCount >= num)
		{
			return true;
		}
		return false;
	}

	public virtual bool EnoughResourcesToPull(int _index)
	{
		if (_index == m_inputs.Count)
		{
			return true;
		}
		for (int i = _index; i < m_inputs.Count; i++)
		{
			GraphConnection graphConnection = m_inputs[i];
			if (graphConnection.m_start != null && graphConnection.m_label.m_value > (graphConnection.m_start as GraphNode).m_resourceCount)
			{
				return false;
			}
		}
		return true;
	}

	public override GraphElement DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			GraphNode graphNode = (GraphNode)binaryFormatter.Deserialize(memoryStream);
			CopyAttributes(graphNode, this);
			graphNode.m_storedRotation = Vector3.zero;
			if (graphNode.m_childElements.Count != 0)
			{
				for (int i = 0; i < graphNode.m_childElements.Count; i++)
				{
					CopyAttributes(graphNode.m_childElements[i], m_childElements[i]);
					graphNode.m_childElements[i].m_storedRotation = Vector3.zero;
				}
			}
			return graphNode;
		}
	}

	public override void CopyAttributes(GraphElement _newElement, GraphElement _oldElement)
	{
		base.CopyAttributes(_newElement, _oldElement);
		GraphNode graphNode = _newElement as GraphNode;
		GraphNode graphNode2 = _oldElement as GraphNode;
		if (graphNode != null && graphNode2 != null)
		{
			graphNode.m_minDistanceFromParent = graphNode2.m_minDistanceFromParent;
			graphNode.m_maxDistanceFromParent = graphNode2.m_maxDistanceFromParent;
		}
	}

	public override void OnDeserialization(object sender)
	{
		base.OnDeserialization(sender);
		m_childElements = new List<GraphElement>(m_tempElements);
		m_tempElements = null;
		for (int i = 0; i < m_childElements.Count; i++)
		{
			m_childElements[i].m_parentElement = this;
			if (m_childElements[i].m_elementType == GraphElementType.Connection)
			{
				GraphConnection graphConnection = m_childElements[i] as GraphConnection;
				GraphElement element = GetElement(graphConnection.m_startId);
				GraphElement element2 = GetElement(graphConnection.m_endId);
				element.AddOutput(graphConnection);
				element2.AddInput(graphConnection);
			}
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
		info.AddValue("nodeType", (uint)m_nodeType);
		info.AddValue("pullMode", (uint)m_pullMode);
		info.AddValue("triggerMode", (uint)m_triggerMode);
		info.AddValue("triggerInterval", m_triggerInterval);
		info.AddValue("elements", m_childElements.ToArray());
		if (m_assembleClassType != null)
		{
			info.AddValue("assembleClassType", m_assembleClassType.ToString());
		}
		else
		{
			info.AddValue("assembleClassType", string.Empty);
		}
	}
}
