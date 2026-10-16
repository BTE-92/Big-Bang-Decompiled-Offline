using System.Collections.Generic;
using UnityEngine;

public class GGData
{
	public List<GGVertex> m_vertices = new List<GGVertex>();

	public GGData()
	{
	}

	public GGData(Vector2[] _vertices)
	{
		for (int i = 0; i < _vertices.Length; i++)
		{
			GGVertex item = new GGVertex
			{
				vertex = _vertices[i]
			};
			m_vertices.Add(item);
		}
	}

	public GGData(Vector3[] _vertices)
	{
		for (int i = 0; i < _vertices.Length; i++)
		{
			GGVertex item = new GGVertex
			{
				vertex = _vertices[i]
			};
			m_vertices.Add(item);
		}
	}

	public GGData Copy()
	{
		GGData gGData = new GGData();
		for (int i = 0; i < m_vertices.Count; i++)
		{
			GGVertex item = m_vertices[i].Copy();
			gGData.m_vertices.Add(item);
		}
		return gGData;
	}

	public Vector2[] ToVector2Array()
	{
		Vector2[] array = new Vector2[m_vertices.Count];
		for (int i = 0; i < m_vertices.Count; i++)
		{
			array[i] = m_vertices[i].vertex;
		}
		return array;
	}

	public void Add(GGData _gg)
	{
		for (int i = 0; i < _gg.m_vertices.Count; i++)
		{
			GGVertex item = _gg.m_vertices[i].Copy();
			m_vertices.Add(item);
		}
	}

	public void EnsureLoop()
	{
		if (!IsLoop())
		{
			m_vertices.Add(m_vertices[0].Copy());
		}
	}

	public void RemoveLoop()
	{
		if (IsLoop())
		{
			m_vertices.Remove(m_vertices[m_vertices.Count - 1]);
		}
	}

	public bool IsLoop()
	{
		if (m_vertices[0].vertex != m_vertices[m_vertices.Count - 1].vertex && !m_vertices[m_vertices.Count - 1].isDuplicate)
		{
			return false;
		}
		return true;
	}

	public void SetOffset(Vector3 _offset)
	{
		for (int i = 0; i < m_vertices.Count; i++)
		{
			m_vertices[i].vertex += _offset;
		}
	}

	public void ReverseVertexOrder()
	{
		m_vertices.Reverse();
	}

	public void Expand(float _amount)
	{
		if (m_vertices.Count > 2)
		{
			for (int i = 0; i < m_vertices.Count; i++)
			{
				Vector3 vertex = m_vertices[i].vertex;
				Vector3 vertex2;
				Vector3 vertex3;
				if (i == 0)
				{
					vertex2 = m_vertices[i].vertex;
					vertex3 = m_vertices[i + 1].vertex;
				}
				else if (i == m_vertices.Count - 1)
				{
					vertex2 = m_vertices[i - 1].vertex;
					vertex3 = m_vertices[i].vertex;
				}
				else
				{
					vertex2 = m_vertices[i - 1].vertex;
					vertex3 = m_vertices[i + 1].vertex;
				}
				Vector2 vector = vertex3 - vertex2;
				float f = Mathf.Atan2(0f - vector.y, vector.x);
				float num = Mathf.Sin(f) * _amount;
				float num2 = Mathf.Cos(f) * _amount;
				m_vertices[i].vertex = new Vector3(vertex.x + num, vertex.y + num2, m_vertices[i].vertex.z);
			}
		}
		else
		{
			Debug.LogError("Cannot expand data under 3 verts.");
		}
	}

	public void RadialShift(float _multiplier)
	{
		Vector3 vector = Vector2.zero;
		for (int i = 0; i < m_vertices.Count; i++)
		{
			vector += m_vertices[i].vertex;
		}
		vector /= (float)m_vertices.Count;
		for (int j = 0; j < m_vertices.Count; j++)
		{
			m_vertices[j].vertex += (m_vertices[j].vertex - vector) * _multiplier;
		}
	}

	public void RadialShiftRandom(float _minMult, float _maxMult)
	{
		Vector3 vector = Vector2.zero;
		for (int i = 0; i < m_vertices.Count; i++)
		{
			vector += m_vertices[i].vertex;
		}
		vector /= (float)m_vertices.Count;
		for (int j = 0; j < m_vertices.Count; j++)
		{
			m_vertices[j].vertex += (m_vertices[j].vertex - vector) * Random.Range(_minMult, _maxMult);
		}
	}

	public void SetUvMapPlanar(float _xScale, float _yScale, Vector2 _offset, UVRect _normalizeRect = null)
	{
		GGDimensions gGDimensions = null;
		if (_normalizeRect != null)
		{
			gGDimensions = GetDimensions();
		}
		for (int i = 0; i < m_vertices.Count; i++)
		{
			Vector2 vector = m_vertices[i].vertex;
			if (_normalizeRect != null)
			{
				float num = (vector.x - gGDimensions.minX) / gGDimensions.width * _normalizeRect.width + _normalizeRect.left;
				float num2 = (vector.y - gGDimensions.minY) / gGDimensions.height * _normalizeRect.height + _normalizeRect.bottom;
				m_vertices[i].uv = new Vector2(num * _xScale, num2 * _yScale) + _offset;
			}
			else
			{
				m_vertices[i].uv = new Vector2(vector.x * _xScale, vector.y * _yScale) + _offset;
			}
		}
	}

	public void SetUvMapBelt(float _scale, float v, bool _ensureLoop)
	{
		if (_ensureLoop)
		{
			EnsureLoop();
		}
		float num = 0f;
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < m_vertices.Count; i++)
		{
			if (IsLoop())
			{
				if (i > 0 && i < m_vertices.Count - 1)
				{
					num += ((Vector2)(m_vertices[i].vertex - m_vertices[i - 1].vertex)).magnitude;
				}
				else if (i == m_vertices.Count - 1)
				{
					num += ((Vector2)(m_vertices[i].vertex - m_vertices[i - 2].vertex)).magnitude;
				}
			}
			else if (i > 0)
			{
				num += ((Vector2)(m_vertices[i].vertex - m_vertices[i - 1].vertex)).magnitude;
			}
			m_vertices[i].uv = new Vector2(num * _scale, v);
		}
	}

	public void SetUvMapBeltV(float _scale, float _u, bool _ensureLoop)
	{
		if (_ensureLoop)
		{
			EnsureLoop();
		}
		float num = 0f;
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < m_vertices.Count; i++)
		{
			if (IsLoop())
			{
				if (i > 0 && i < m_vertices.Count - 1)
				{
					num += ((Vector2)(m_vertices[i].vertex - m_vertices[i - 1].vertex)).magnitude;
				}
				else if (i == m_vertices.Count - 1)
				{
					num += ((Vector2)(m_vertices[i].vertex - m_vertices[i - 2].vertex)).magnitude;
				}
			}
			else if (i > 0)
			{
				num += ((Vector2)(m_vertices[i].vertex - m_vertices[i - 1].vertex)).magnitude;
			}
			m_vertices[i].uv = new Vector2(_u, num * _scale);
		}
	}

	public GGDimensions GetDimensions()
	{
		GGDimensions gGDimensions = new GGDimensions();
		gGDimensions.minY = float.MaxValue;
		gGDimensions.maxY = float.MinValue;
		gGDimensions.minX = float.MaxValue;
		gGDimensions.maxX = float.MinValue;
		for (int i = 0; i < m_vertices.Count; i++)
		{
			if (m_vertices[i].vertex.y < gGDimensions.minY)
			{
				gGDimensions.minY = m_vertices[i].vertex.y;
			}
			if (m_vertices[i].vertex.y > gGDimensions.maxY)
			{
				gGDimensions.maxY = m_vertices[i].vertex.y;
			}
			if (m_vertices[i].vertex.x < gGDimensions.minX)
			{
				gGDimensions.minX = m_vertices[i].vertex.x;
			}
			if (m_vertices[i].vertex.x > gGDimensions.maxX)
			{
				gGDimensions.maxX = m_vertices[i].vertex.x;
			}
		}
		gGDimensions.width = gGDimensions.maxX - gGDimensions.minX;
		gGDimensions.height = gGDimensions.maxY - gGDimensions.minY;
		return gGDimensions;
	}

	public void SetColorGradientVertical(uint _top, uint _bottom)
	{
		Color color = DebugDraw.UIntToColor(_bottom);
		Color color2 = DebugDraw.UIntToColor(_top);
		GGDimensions dimensions = GetDimensions();
		for (int i = 0; i < m_vertices.Count; i++)
		{
			float num = (m_vertices[i].vertex.y - dimensions.minY) / dimensions.height;
			Color color3 = color2 * num + color * (1f - num);
			m_vertices[i].color = color3;
		}
	}

	public void SplitVertices(float _smoothingAngle = 180f, bool _ensureLoop = true)
	{
		if (!(_smoothingAngle < 180f))
		{
			return;
		}
		if (_ensureLoop)
		{
			EnsureLoop();
		}
		bool flag = IsLoop();
		List<GGVertex> list = new List<GGVertex>();
		for (int i = 0; i < m_vertices.Count; i++)
		{
			Vector3 vector;
			Vector3 vector2;
			if (flag)
			{
				int rolledValue = ToolBox.getRolledValue(i - 1, 0, m_vertices.Count - 1);
				int rolledValue2 = ToolBox.getRolledValue(i + 1, 0, m_vertices.Count - 1);
				vector = m_vertices[rolledValue].vertex - m_vertices[i].vertex;
				vector2 = m_vertices[i].vertex - m_vertices[rolledValue2].vertex;
			}
			else
			{
				int rolledValue = ToolBox.limitBetween(i - 1, 0, m_vertices.Count - 1);
				int rolledValue2 = ToolBox.limitBetween(i + 1, 0, m_vertices.Count - 1);
				vector = ((i <= 0) ? (m_vertices[rolledValue].vertex - m_vertices[i + 1].vertex) : (m_vertices[rolledValue].vertex - m_vertices[i].vertex));
				vector2 = ((i >= m_vertices.Count - 1) ? (m_vertices[i - 1].vertex - m_vertices[rolledValue2].vertex) : (m_vertices[i].vertex - m_vertices[rolledValue2].vertex));
			}
			float num = Vector2.Angle(vector, vector2);
			if (num >= _smoothingAngle)
			{
				list.Add(m_vertices[i]);
				list.Add(m_vertices[i].Copy(true));
			}
			else
			{
				list.Add(m_vertices[i]);
			}
		}
		m_vertices = list;
	}
}
