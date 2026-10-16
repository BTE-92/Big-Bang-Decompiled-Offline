using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable]
public class Level : ISerializable
{
	public Hashtable m_settings;

	public List<LevelLayer> m_layers;

	public LevelLayer m_currentLayer;

	public int m_currentLayerIndex;

	public bool m_changed;

	public Level(int _width, int _height)
	{
		m_settings = new Hashtable();
		m_layers = new List<LevelLayer>();
		m_currentLayer = new LevelLayer(LevelLayerType.BaseLayer, _width, _height);
		m_layers.Add(m_currentLayer);
		m_currentLayerIndex = 0;
	}

	public Level(SerializationInfo info, StreamingContext ctxt)
	{
		m_settings = (Hashtable)info.GetValue("settings", typeof(Hashtable));
		m_layers = new List<LevelLayer>((LevelLayer[])info.GetValue("layers", typeof(LevelLayer[])));
	}

	public virtual void ItemChanged()
	{
		m_changed = true;
	}

	public virtual void AddItem(string _key)
	{
	}

	public virtual void RemoveItem(string _key)
	{
	}

	public virtual void ClearItems()
	{
	}

	public virtual void SetLayerItems()
	{
	}

	public virtual void Destroy()
	{
		while (m_layers.Count > 0)
		{
			int index = m_layers.Count - 1;
			m_layers[index].Dispose();
			m_layers.RemoveAt(index);
		}
		m_layers = null;
		m_currentLayer = null;
		m_currentLayerIndex = -1;
		m_settings.Clear();
		m_settings = null;
	}

	public virtual void Update()
	{
		for (int i = 0; i < m_layers.Count; i++)
		{
			if (m_layers[i].m_assembled && !m_layers[i].m_disabled)
			{
				m_layers[i].Update();
			}
		}
	}

	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		info.AddValue("settings", m_settings);
		info.AddValue("layers", m_layers.ToArray());
	}
}
