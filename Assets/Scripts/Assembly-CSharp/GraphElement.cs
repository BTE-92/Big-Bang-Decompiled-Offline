using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public abstract class GraphElement : ISerializable, IDeserializationCallback
{
	public TransformC m_TC;

	public TouchAreaC m_TAC;

	public GraphNode m_parentElement;

	public GraphElementType m_elementType;

	public string m_name;

	public uint m_id;

	public float m_radius;

	public float m_width;

	public float m_height;

	public bool m_isRect;

	public Vector3 m_position;

	public Vector3 m_rotation;

	public Vector3 m_scale;

	public Vector3 m_storedRotation;

	public bool m_flipped;

	public bool m_inFront;

	public bool m_disabled;

	public bool m_disabledAtStart;

	public bool m_selected;

	public List<GraphConnection> m_inputs;

	public List<GraphConnection> m_outputs;

	public int m_stateInputCount;

	public int m_stateOutputCount;

	public bool m_assembled;

	public List<IAssembledClass> m_assembledClasses;

	public bool m_isMoveable;

	public bool m_isRotateable;

	public bool m_isScaleable;

	public bool m_isScaleableUniform;

	public bool m_isCopyable;

	public bool m_isRemovable;

	public bool m_isModifiable;

	public bool m_isReplaceable;

	public bool m_isFlippable;

	public bool m_isLinkable;

	public bool m_isSelectable;

	public bool m_isForegroundable;

	public GraphElement(string _name)
	{
		m_name = _name;
		m_id = LevelManager.GetUniqueId();
		m_inputs = new List<GraphConnection>();
		m_outputs = new List<GraphConnection>();
		m_assembledClasses = new List<IAssembledClass>();
		m_disabled = true;
		m_disabledAtStart = true;
		m_radius = 25f;
		m_width = 50f;
		m_height = 50f;
		m_isRect = false;
		m_stateInputCount = 0;
		m_stateOutputCount = 0;
		m_position = Vector3.zero;
		m_rotation = Vector3.zero;
		m_scale = Vector3.one;
		m_storedRotation = Vector3.zero;
		m_flipped = false;
		m_assembled = false;
		m_inFront = false;
		SetPropertyDefaults();
	}

	public GraphElement(SerializationInfo info, StreamingContext ctxt)
	{
		m_elementType = (GraphElementType)(uint)info.GetValue("elementType", typeof(uint));
		m_name = (string)info.GetValue("name", typeof(string));
		m_id = (uint)info.GetValue("id", typeof(uint));
		float x = (float)info.GetValue("pX", typeof(float));
		float y = (float)info.GetValue("pY", typeof(float));
		float z = (float)info.GetValue("pZ", typeof(float));
		float x2 = (float)info.GetValue("rX", typeof(float));
		float y2 = (float)info.GetValue("rY", typeof(float));
		float z2 = (float)info.GetValue("rZ", typeof(float));
		float x3 = (float)info.GetValue("sX", typeof(float));
		float y3 = (float)info.GetValue("sY", typeof(float));
		float z3 = (float)info.GetValue("sZ", typeof(float));
		try
		{
			float x4 = (float)info.GetValue("r2X", typeof(float));
			float y4 = (float)info.GetValue("r2Y", typeof(float));
			float z4 = (float)info.GetValue("r2Z", typeof(float));
			m_storedRotation = new Vector3(x4, y4, z4);
		}
		catch
		{
			m_storedRotation = Vector3.zero;
		}
		m_position = new Vector3(x, y, z);
		m_rotation = new Vector3(x2, y2, z2);
		m_scale = new Vector3(x3, y3, z3);
		m_flipped = (bool)info.GetValue("flipped", typeof(bool));
		m_width = (float)info.GetValue("width", typeof(float));
		m_height = (float)info.GetValue("height", typeof(float));
		try
		{
			m_radius = (float)info.GetValue("radius", typeof(float));
		}
		catch
		{
			m_radius = (m_width + m_height) * 0.25f;
		}
		m_isRect = (bool)info.GetValue("isRect", typeof(bool));
		m_disabledAtStart = (bool)info.GetValue("disabled", typeof(bool));
		try
		{
			m_inFront = info.GetBoolean("inFront");
		}
		catch
		{
			m_inFront = false;
		}
		SetPropertyDefaults();
		m_inputs = new List<GraphConnection>();
		m_outputs = new List<GraphConnection>();
		m_assembledClasses = new List<IAssembledClass>();
		m_disabled = true;
		m_stateInputCount = 0;
		m_stateOutputCount = 0;
		if (m_id > LevelManager.m_uniqueID)
		{
			LevelManager.m_uniqueID = m_id + 1;
		}
	}

	public virtual void SetPropertyDefaults()
	{
		m_isMoveable = true;
		m_isRotateable = false;
		m_isScaleable = false;
		m_isScaleableUniform = false;
		m_isCopyable = true;
		m_isRemovable = true;
		m_isModifiable = false;
		m_isReplaceable = false;
		m_isFlippable = false;
		m_isLinkable = false;
		m_isSelectable = true;
		m_isForegroundable = false;
	}

	public virtual void Select(bool _select)
	{
		if (m_isSelectable)
		{
			m_selected = _select;
		}
	}

	public virtual Vector3 GetConnectionPosition(Vector3 _point)
	{
		Vector3 vector = _point - m_position;
		return m_position + vector.normalized * m_width;
	}

	public virtual Vector3 GetPosition()
	{
		return m_position;
	}

	public virtual void AddInput(GraphConnection _connection)
	{
		if (_connection.m_end != null)
		{
			_connection.m_end.RemoveInput(_connection);
		}
		if (_connection.m_connectionType == ConnectionType.State)
		{
			m_inputs.Insert(m_stateInputCount, _connection);
			m_stateInputCount++;
		}
		else
		{
			m_inputs.Add(_connection);
		}
		_connection.m_end = this;
		_connection.m_points[_connection.m_points.Count - 1] = GetPosition();
		_connection.CalculateStartPosition(false);
		_connection.CalculateEndPosition(false);
		_connection.Reset();
	}

	public virtual void AddOutput(GraphConnection _connection)
	{
		if (_connection.m_start != null)
		{
			_connection.m_start.RemoveOutput(_connection);
		}
		if (_connection.m_connectionType == ConnectionType.State)
		{
			m_outputs.Insert(m_stateOutputCount, _connection);
			m_stateOutputCount++;
		}
		else
		{
			m_outputs.Add(_connection);
		}
		_connection.m_start = this;
		_connection.m_points[0] = GetPosition();
		_connection.CalculateEndPosition(false);
		_connection.CalculateStartPosition(false);
		_connection.Reset();
	}

	public virtual bool RemoveInput(GraphConnection _connection)
	{
		if (m_inputs.Contains(_connection))
		{
			_connection.m_end = null;
			if (_connection.m_connectionType == ConnectionType.State)
			{
				m_stateInputCount--;
			}
			m_inputs.Remove(_connection);
			return true;
		}
		return false;
	}

	public virtual bool RemoveOutput(GraphConnection _connection)
	{
		if (m_outputs.Contains(_connection))
		{
			_connection.m_start = null;
			if (_connection.m_connectionType == ConnectionType.State)
			{
				m_stateOutputCount--;
			}
			m_outputs.Remove(_connection);
			return true;
		}
		return false;
	}

	public virtual void Dispose()
	{
		Clear(false);
		while (m_inputs.Count > 0)
		{
			int index = m_inputs.Count - 1;
			if (m_inputs[index] != null)
			{
				m_inputs[index].Dispose();
			}
		}
		m_inputs = null;
		while (m_outputs.Count > 0)
		{
			int index2 = m_outputs.Count - 1;
			if (m_outputs[index2] != null)
			{
				m_outputs[index2].Dispose();
			}
		}
		m_outputs = null;
		if (m_parentElement != null)
		{
			m_parentElement.RemoveElement(this);
		}
	}

	public virtual void Initialize()
	{
	}

	public virtual void Assemble()
	{
		if (!m_disabledAtStart)
		{
			m_disabled = false;
		}
		if (m_TC == null)
		{
			m_TC = EntityManager.AddEntityWithTC();
			m_TC.transform.name = m_name;
			TransformS.SetGlobalPosition(m_TC, m_position);
			TransformS.SetGlobalRotation(m_TC, m_rotation);
		}
		m_assembled = true;
	}

	public virtual void Copied()
	{
	}

	public virtual void Flipped()
	{
	}

	public virtual void Foregrounded()
	{
	}

	public virtual void Linked()
	{
	}

	public virtual void Removed()
	{
	}

	public virtual void Clear(bool _isReset)
	{
		while (m_assembledClasses.Count > 0)
		{
			int index = m_assembledClasses.Count - 1;
			m_assembledClasses[index].Destroy();
		}
		if (m_TC != null)
		{
			EntityManager.RemoveEntity(m_TC.p_entity);
			m_TC = null;
			m_TAC = null;
		}
		m_assembled = false;
	}

	public virtual void Reset()
	{
		Clear(true);
		Assemble();
	}

	public virtual void Update()
	{
	}

	public virtual void Trigger()
	{
	}

	public virtual void Start()
	{
	}

	public virtual void End()
	{
	}

	public virtual void CopyAttributes(GraphElement _newElement, GraphElement _oldElement)
	{
		_newElement.m_isMoveable = _oldElement.m_isMoveable;
		_newElement.m_isRotateable = _oldElement.m_isRotateable;
		_newElement.m_isScaleable = m_isScaleable;
		_newElement.m_isScaleableUniform = _oldElement.m_isScaleableUniform;
		_newElement.m_isCopyable = _oldElement.m_isCopyable;
		_newElement.m_isRemovable = _oldElement.m_isRemovable;
		_newElement.m_isModifiable = _oldElement.m_isModifiable;
		_newElement.m_isReplaceable = _oldElement.m_isReplaceable;
		_newElement.m_isFlippable = _oldElement.m_isFlippable;
		_newElement.m_isLinkable = _oldElement.m_isLinkable;
		_newElement.m_isForegroundable = _oldElement.m_isForegroundable;
	}

	public virtual GraphElement DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			GraphElement graphElement = (GraphElement)binaryFormatter.Deserialize(memoryStream);
			CopyAttributes(graphElement, this);
			return graphElement;
		}
	}

	public virtual void OnDeserialization(object sender)
	{
	}

	public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		info.AddValue("elementType", (uint)m_elementType);
		info.AddValue("name", m_name);
		info.AddValue("id", m_id);
		info.AddValue("pX", m_position.x);
		info.AddValue("pY", m_position.y);
		info.AddValue("pZ", m_position.z);
		info.AddValue("rX", m_rotation.x);
		info.AddValue("rY", m_rotation.y);
		info.AddValue("rZ", m_rotation.z);
		info.AddValue("sX", m_scale.x);
		info.AddValue("sY", m_scale.y);
		info.AddValue("sZ", m_scale.z);
		info.AddValue("r2X", m_storedRotation.x);
		info.AddValue("r2Y", m_storedRotation.y);
		info.AddValue("r2Z", m_storedRotation.z);
		info.AddValue("flipped", m_flipped);
		info.AddValue("width", m_width);
		info.AddValue("height", m_height);
		info.AddValue("radius", m_radius);
		info.AddValue("isRect", m_isRect);
		info.AddValue("disabled", m_disabledAtStart);
		info.AddValue("inFront", m_inFront);
	}
}
