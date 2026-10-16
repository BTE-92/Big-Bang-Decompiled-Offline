using System;
using System.Collections.Generic;
using UnityEngine;

public static class AutogeometryVisuals
{
	private static CVertex[] m_frontVerts = new CVertex[200];

	private static List<CVertex> m_newVerts = new List<CVertex>(200);

	private static int m_frontVertCount = 0;

	public static Mesh CreateBeltMeshFromVertexArray(AgPolygon _poly, float _width, Vector3 _offset, bool _loop, Vector2 _tileCenter, float _smoothingAngle)
	{
		bool flag = false;
		Mesh mesh = new Mesh();
		int num = _poly.vertices.Count - ((!_loop) ? 1 : 0);
		int num2 = num * 6;
		int[] array = new int[num2 * ((!flag) ? 1 : 2)];
		Vector3[] array2 = new Vector3[_poly.vertices.Count - (_loop ? 1 : 0)];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = new Vector3(_poly.vertices[i].x + _offset.x, _poly.vertices[i].y + _offset.y, _offset.z) - (Vector3)_tileCenter;
		}
		Color black = Color.black;
		m_frontVertCount = 0;
		if (_smoothingAngle < 180f)
		{
			m_newVerts.Clear();
			for (int j = 0; j < array2.Length; j++)
			{
				Vector3 vector;
				Vector3 vector2;
				if (_loop)
				{
					int rolledValue = ToolBox.getRolledValue(j - 1, 0, array2.Length - 1);
					int rolledValue2 = ToolBox.getRolledValue(j + 1, 0, array2.Length - 1);
					vector = array2[rolledValue] - array2[j];
					vector2 = array2[j] - array2[rolledValue2];
				}
				else
				{
					int rolledValue = Mathf.Max(j - 1, 0);
					int rolledValue2 = Mathf.Min(j + 1, array2.Length - 1);
					vector = ((j <= 0) ? (array2[rolledValue] - array2[j + 1]) : (array2[rolledValue] - array2[j]));
					vector2 = ((j >= array2.Length - 1) ? (array2[j - 1] - array2[rolledValue2]) : (array2[j] - array2[rolledValue2]));
				}
				float num3 = Vector2.Angle(vector, vector2);
				if (num3 >= _smoothingAngle)
				{
					m_newVerts.Add(new CVertex(array2[j], Vector2.zero, black, new Vector2(vector.y, 0f - vector.x).normalized));
					m_newVerts.Add(new CVertex(array2[j], Vector2.zero, black, new Vector2(vector2.y, 0f - vector2.x).normalized, true));
				}
				else
				{
					m_newVerts.Add(new CVertex(array2[j], Vector2.zero, black, _poly.extraData[j]));
				}
			}
			m_frontVertCount = m_newVerts.Count;
			for (int k = 0; k < m_newVerts.Count; k++)
			{
				m_frontVerts[k] = m_newVerts[k];
			}
		}
		else
		{
			for (int l = 0; l < array2.Length; l++)
			{
				m_frontVerts[l] = new CVertex(array2[l], Vector2.zero, black, _poly.extraData[l]);
			}
			m_frontVertCount = array2.Length;
		}
		int frontVertCount = m_frontVertCount;
		int num4 = 0;
		int num5 = 0;
		for (int m = 0; m < frontVertCount; m++)
		{
			int num6 = (num5 + 1) % frontVertCount;
			if (!m_frontVerts[num6].isDuplicate && num4 < num2)
			{
				array[num4] = num5;
				array[num4 + 1] = num5 + frontVertCount;
				array[num4 + 2] = num6;
				array[num4 + 3] = num6;
				array[num4 + 4] = num5 + frontVertCount;
				array[num4 + 5] = num6 + frontVertCount;
				num4 += 6;
			}
			num5++;
		}
		Vector3[] array3 = new Vector3[m_frontVertCount * 2 * ((!flag) ? 1 : 2)];
		Vector2[] array4 = new Vector2[m_frontVertCount * 2 * ((!flag) ? 1 : 2)];
		Vector3[] array5 = new Vector3[m_frontVertCount * 2 * ((!flag) ? 1 : 2)];
		Vector3 vector3 = new Vector3(0f, 0f, _width);
		for (int n = 0; n < m_frontVertCount; n++)
		{
			Vector3 vert = m_frontVerts[n].vert;
			Vector2 uv = m_frontVerts[n].uv;
			Vector3 normal = m_frontVerts[n].normal;
			array3[n] = vert;
			array4[n] = uv;
			array5[n] = normal;
			array3[n + m_frontVertCount] = vert + vector3;
			array4[n + m_frontVertCount] = uv;
			array5[n + m_frontVertCount] = normal;
		}
		mesh.vertices = array3;
		mesh.triangles = array;
		mesh.uv = array4;
		mesh.normals = array5;
		return mesh;
	}

	public static Mesh CreateFrontFaceMeshFromPolygons(AgPolygon[] _polygons, Vector3 _offset, Vector3 _tileCenter)
	{
		IntPtr tess = LibTessWrapper.tessNewTess(IntPtr.Zero);
		Vector2 vector = -(Vector2)_tileCenter;
		for (int i = 0; i < _polygons.Length; i++)
		{
			float[] array = new float[_polygons[i].vertices.Count * 3];
			for (int j = 0; j < _polygons[i].vertices.Count; j++)
			{
				int num = (_polygons[i].vertices.Count - 1 - j) * 3;
				array[num] = _polygons[i].vertices[j].x + vector.x;
				array[num + 1] = _polygons[i].vertices[j].y + vector.y;
				array[num + 2] = 0f;
			}
			LibTessWrapper.tessAddContour(tess, 3, array, 12, _polygons[i].vertices.Count);
		}
		LibTessWrapper.tessTesselate(tess, 0, 0, 3, 3, null);
		int num2 = LibTessWrapper.tessGetVertexCount(tess);
		int num3 = LibTessWrapper.tessGetElementCount(tess);
		float[] array2 = new float[num2 * 3];
		LibTessWrapper.wTessGetVertices(tess, array2, 3);
		int[] array3 = new int[num3 * 3];
		LibTessWrapper.wTessGetElements(tess, array3, 3);
		LibTessWrapper.tessDeleteTess(tess);
		Mesh mesh = new Mesh();
		if (num2 > 0)
		{
			int[] array4 = new int[num3 * 3];
			for (int k = 0; k < num3; k++)
			{
				int num4 = k * 3;
				for (int l = 0; l < 3; l++)
				{
					int num5 = array3[k * 3 + l];
					if (num5 != -1)
					{
						array4[num4 + 2 - l] = num5;
					}
				}
			}
			int num6 = num2;
			Vector3[] array5 = new Vector3[num6];
			Vector2[] array6 = new Vector2[num6];
			Vector2[] array7 = new Vector2[num6];
			Vector2 vector2 = _tileCenter / 200f;
			vector2.x = ToolBox.getRolledValue(vector2.x, -1f, 1f);
			vector2.y = ToolBox.getRolledValue(vector2.y, -1f, 1f);
			for (int m = 0; m < num6; m++)
			{
				int num7 = m * 3;
				Vector2 vector3 = new Vector2(array2[num7], array2[num7 + 1]);
				array5[m] = new Vector3(vector3.x, vector3.y, 0f) + _offset;
				Vector2 vector4 = new Vector2(vector3.x, vector3.y) / 200f;
				array7[m] = vector2 + vector4;
				float positionBetween = ToolBox.getPositionBetween(vector3.x + _tileCenter.x + 8f, (float)(-AutoGeometryManager.m_width) * 0.5f, (float)AutoGeometryManager.m_width * 0.5f);
				float positionBetween2 = ToolBox.getPositionBetween(vector3.y + _tileCenter.y + 8f, (float)(-AutoGeometryManager.m_height) * 0.5f, (float)AutoGeometryManager.m_height * 0.5f);
				array6[m] = new Vector2(positionBetween, positionBetween2);
			}
			mesh.vertices = array5;
			mesh.uv = array6;
			mesh.uv2 = array7;
			mesh.triangles = array4;
		}
		return mesh;
	}
}
