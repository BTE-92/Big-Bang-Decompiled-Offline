using System.Collections.Generic;
using UnityEngine;

public static class AutoGeometryManager
{
	public struct lookupData
	{
		public byte b;

		public int layerId;
	}

	public const int MAX_LAYERS = 10;

	public const int TEXEL_SCALE = 16;

	public static bool m_useDome = false;

	public static int m_width;

	public static int m_height;

	public static Vector2 m_tileCacheOffset;

	public static List<AutoGeometryLayer> m_layers = new List<AutoGeometryLayer>();

	public static int m_layerWidth;

	public static int m_layerHeight;

	public static lookupData[] m_maxValueLookupData;

	public static byte[] m_domeValueLookupBytes;

	public static Vector2 m_domeCenter;

	public static Vector2 m_domeDiameter;

	public static float m_domeDiameterBias;

	private static int lastUpdatedLookupLayer = -1;

	public static void Initialize(int _width, int _height, bool _useDome)
	{
		m_useDome = _useDome;
		m_width = _width;
		m_height = _height;
		m_layerWidth = m_width / 16;
		m_layerHeight = m_height / 16;
		SetDomeSize(m_layerWidth, m_layerHeight, 0f);
		m_domeValueLookupBytes = new byte[m_layerWidth * m_layerHeight];
		m_maxValueLookupData = new lookupData[m_layerWidth * m_layerHeight];
		Debug.Log("Autogeometry manager initialize for world: " + m_width + "/" + m_height);
		m_tileCacheOffset = -new Vector2(m_width, m_height) * 0.5f;
	}

	public static void Free()
	{
		m_domeValueLookupBytes = null;
		m_maxValueLookupData = null;
		m_layers.Clear();
	}

	public static void SetDomeSize(int _width, int _height, float domeYOffset)
	{
		float num = 8f;
		m_domeCenter = new Vector2((float)m_layerWidth * 0.5f, domeYOffset / 16f);
		m_domeDiameter = new Vector2((float)_width - num, ((float)_height - num * ((float)_height / (float)_width)) * 2f);
		m_domeDiameterBias = 25f / (float)_width;
	}

	public static void Update()
	{
		for (int i = 0; i < m_layers.Count; i++)
		{
			m_layers[i].Update();
		}
	}

	public static AutoGeometryLayer AddLayer(Ground _groundClass, int _tileSize, int _samplesPerTile, float _simplifyTreshold = 1f)
	{
		Debug.Log(m_layers.Count);
		_tileSize *= 16;
		if (m_layers.Count < 10)
		{
			AutoGeometryLayer autoGeometryLayer = new AutoGeometryLayer(_groundClass, _tileSize, _samplesPerTile, 16, _simplifyTreshold);
			m_layers.Add(autoGeometryLayer);
			return autoGeometryLayer;
		}
		Debug.LogError("No more layers allowed!");
		return null;
	}

	public static void DestroyAllLayers()
	{
		while (m_layers.Count > 0)
		{
			int index = m_layers.Count - 1;
			m_layers[index].Destroy();
			m_layers.RemoveAt(index);
		}
	}

	public static void ClearTileDirtyFlags()
	{
		for (int i = 0; i < m_layers.Count; i++)
		{
			m_layers[i].ClearAgTileDirtyFlags();
		}
	}

	public static void SetDirtyTileTracking(bool _track)
	{
		for (int i = 0; i < m_layers.Count; i++)
		{
			m_layers[i].TrackDirtyTiles(_track);
		}
	}

	private static int GetLayerByteOffset(float _x, float _y)
	{
		if (_x >= 1f && _y >= 1f && _x <= (float)(m_layerWidth - 1) && _y <= (float)(m_layerHeight - 1))
		{
			return (int)_y * m_layerWidth + (int)_x;
		}
		return -1;
	}

	public static lookupData ReadMaxLayerValueFromWorldPos(Vector2 _worldPos)
	{
		_worldPos -= m_tileCacheOffset;
		Vector2 vector = _worldPos / 16f;
		if (vector.x >= 0f && vector.y >= 0f && vector.x <= (float)(m_layerWidth - 1) && vector.y <= (float)(m_layerHeight - 1))
		{
			int num = (int)vector.y * m_layerWidth + (int)vector.x;
			lookupData result = m_maxValueLookupData[num];
			if (m_useDome)
			{
				result.b = (byte)Mathf.Max(result.b, m_domeValueLookupBytes[num]);
			}
			return result;
		}
		return default(lookupData);
	}

	private static lookupData GetBiggestValueFromLayers(AutoGeometryLayer _selectedLayer, int _byteOffset)
	{
		lookupData result = new lookupData
		{
			b = 0,
			layerId = -1
		};
		for (int i = 0; i < m_layers.Count; i++)
		{
			if (m_layers[i] != _selectedLayer)
			{
				byte b = m_layers[i].m_bytes[_byteOffset];
				if (b > 0 && b > result.b)
				{
					result.b = b;
					result.layerId = i;
				}
			}
		}
		return result;
	}

	public static void UpdateMaxValueLookupTable(AutoGeometryLayer _selectedLayer, LookupUpdateMode _lookupUpdateMode, bool _updateDome = false)
	{
		Debug.Log(string.Concat("Lookup updated: ", _lookupUpdateMode, "/ Dome Updated: ", _updateDome));
		switch (_lookupUpdateMode)
		{
		case LookupUpdateMode.LEVEL_LOADED:
		{
			for (int j = 0; j < m_layerWidth * m_layerHeight; j++)
			{
				m_maxValueLookupData[j] = GetBiggestValueFromLayers(_selectedLayer, j);
			}
			break;
		}
		case LookupUpdateMode.IN_EDITOR:
		{
			int num = lastUpdatedLookupLayer;
			int index = _selectedLayer.m_index;
			if (num == index)
			{
				return;
			}
			for (int k = 0; k < m_layerWidth * m_layerHeight; k++)
			{
				lookupData lookupData2 = m_maxValueLookupData[k];
				if (lookupData2.layerId == index)
				{
					m_maxValueLookupData[k] = GetBiggestValueFromLayers(_selectedLayer, k);
					continue;
				}
				byte b = m_layers[num].m_bytes[k];
				if (b > lookupData2.b)
				{
					m_maxValueLookupData[k].b = b;
					m_maxValueLookupData[k].layerId = num;
				}
			}
			break;
		}
		case LookupUpdateMode.NEW_LEVEL:
		{
			for (int i = 0; i < m_layerWidth * m_layerHeight; i++)
			{
				m_maxValueLookupData[i].b = 0;
				m_maxValueLookupData[i].layerId = -1;
			}
			break;
		}
		}
		lastUpdatedLookupLayer = _selectedLayer.m_index;
		if (!m_useDome || !_updateDome)
		{
			return;
		}
		float num2 = (float)m_layerWidth * 0.5f - m_domeDiameter.x * 0.25f;
		float num3 = (float)m_layerWidth * 0.5f + m_domeDiameter.x * 0.25f;
		float num4 = m_domeDiameter.y * 0.25f;
		for (int l = 0; l < m_layerHeight; l++)
		{
			for (int m = 0; m < m_layerWidth; m++)
			{
				int layerByteOffset = GetLayerByteOffset(m, l);
				if (layerByteOffset < 0)
				{
					continue;
				}
				bool flag = false;
				if ((float)m > num2 && (float)m < num3 && (float)l < num4)
				{
					flag = true;
				}
				if (!flag)
				{
					float num5 = ToolBox.PointInsideEllipse(new Vector2(m, l), m_domeCenter, m_domeDiameter * 0.5f, 0f);
					if (num5 > 1f)
					{
						float positionBetween = ToolBox.getPositionBetween(num5 - 1f, 0f, m_domeDiameterBias);
						byte b2 = (byte)ToolBox.limitBetween(positionBetween * 255f, 0f, 255f);
						m_domeValueLookupBytes[layerByteOffset] = b2;
					}
					else
					{
						m_domeValueLookupBytes[layerByteOffset] = 0;
					}
				}
				else
				{
					m_domeValueLookupBytes[layerByteOffset] = 0;
				}
			}
		}
	}

	public static int GetLayerAtWorldPos(Vector2 _worldPosition, byte _tolerance = 0)
	{
		byte b = 127;
		int result = -1;
		for (int i = 0; i < m_layers.Count; i++)
		{
			byte b2 = m_layers[i].ReadDataFromWorldPos(_worldPosition);
			if (b2 > _tolerance && b2 > b)
			{
				result = i;
				b = b2;
			}
		}
		return result;
	}

	public static void HighlightLayer(AutoGeometryLayer _layer, float _amount, float _speed = 0.1f)
	{
		for (int i = 0; i < m_layers.Count; i++)
		{
			float emissionAmount = 0f;
			if (_layer == m_layers[i])
			{
				emissionAmount = _amount;
			}
			m_layers[i].SetHighlight(emissionAmount, _speed);
		}
	}

	public static void SetLayerShaders(bool _inGame)
	{
		Debug.Log("Setting Ground shaders to " + ((!_inGame) ? "EDITOR" : "INGAME"));
		for (int i = 0; i < m_layers.Count; i++)
		{
			m_layers[i].SetLayerShaders(_inGame);
		}
	}

	public static void PaintWithBrush(AutoGeometryLayer _layer, AutoGeometryBrush _brush, Vector2 _pos, bool _additive, bool _soften, bool _updateAgMask = true)
	{
		int index = m_layers.IndexOf(_layer);
		m_layers[index].PaintWithBrush(_brush, _pos, (!_additive) ? AGDrawMode.SUB : AGDrawMode.ADD, _soften, ref m_layers[index].m_bytes, true, _updateAgMask);
	}
}
