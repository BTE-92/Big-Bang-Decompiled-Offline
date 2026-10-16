using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class DebugDraw
{
	public static float m_lineWidth = 1f;

	public static Color defaultColor;

	public static SpriteSheet p_spriteSheet;

	public static SpriteSheet p_uiSpriteSheet;

	public static Frame m_debugFrame;

	public static TransformC m_debugTC;

	public static void Initialize()
	{
		if (CameraS.m_mainCamera == null || CameraS.m_uiCamera == null)
		{
			Debug.LogError("Initialize CameraS first");
			return;
		}
		p_spriteSheet = SpriteS.AddSpriteSheet(CameraS.m_mainCamera, ResourceManager.GetMaterial("Framework/SolidMat"), 1f);
		p_uiSpriteSheet = SpriteS.AddSpriteSheet(CameraS.m_uiCamera, ResourceManager.GetMaterial("Framework/SolidMat"), 1f);
		defaultColor = new Color(1f, 1f, 1f, 1f);
		m_debugFrame = new Frame(0f, 0f, 128f, 128f);
		Entity entity = EntityManager.AddEntity("DebugDraw");
		m_debugTC = TransformS.AddComponent(entity, "DebugDraw");
		entity.m_persistent = true;
	}

	public static void Clear(Camera _camera)
	{
		SpriteSheet spriteSheet = p_spriteSheet;
		if (p_uiSpriteSheet.m_camera == _camera)
		{
			spriteSheet = p_uiSpriteSheet;
		}
		while (p_spriteSheet.m_components.m_aliveCount > 0)
		{
			SpriteS.RemoveComponent(spriteSheet.m_components.m_array[spriteSheet.m_components.m_aliveIndices[0]]);
		}
	}

	public static void Clear(Camera _camera, TransformC _tc)
	{
		List<SpriteC> spritesByTransform = SpriteS.GetSpritesByTransform(_tc);
		while (spritesByTransform.Count > 0)
		{
			SpriteS.RemoveComponent(spritesByTransform[0]);
			spritesByTransform.RemoveAt(0);
		}
	}

	public static SpriteC CreateLine(Camera _camera, TransformC _transformComponent, float _length, Vector3 _offset, float _offsetAngle)
	{
		SpriteSheet sheet = p_spriteSheet;
		if (p_uiSpriteSheet.m_camera == _camera)
		{
			sheet = p_uiSpriteSheet;
		}
		SpriteC spriteC = SpriteS.AddComponent(_transformComponent, m_debugFrame, sheet);
		SpriteS.SetOffset(spriteC, _offset, _offsetAngle);
		SpriteS.SetDimensions(spriteC, _length, m_lineWidth);
		SpriteS.SetColor(spriteC, defaultColor);
		return spriteC;
	}

	public static SpriteC CreateLine(Camera _camera, TransformC _transformComponent, Vector3 _start, Vector3 _end)
	{
		SpriteSheet sheet = p_spriteSheet;
		if (p_uiSpriteSheet.m_camera == _camera)
		{
			sheet = p_uiSpriteSheet;
		}
		SpriteC spriteC = SpriteS.AddComponent(_transformComponent, m_debugFrame, sheet);
		Vector3 pos = (_start + _end) * 0.5f;
		Vector3 vector = _end - _start;
		float rot = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		SpriteS.SetOffset(spriteC, pos, rot);
		SpriteS.SetDimensions(spriteC, vector.magnitude, m_lineWidth);
		SpriteS.SetColor(spriteC, defaultColor);
		return spriteC;
	}

	public static void CreateBox(Camera _camera, TransformC _transformComponent, Vector3 _pos, float _width, float _height, bool _createAngleMarker = false)
	{
		Vector3 vector = new Vector3(_width * 0.5f, 0f, 0f);
		Vector3 vector2 = new Vector3(0f, _height * 0.5f, 0f);
		CreateLine(_camera, _transformComponent, _width, vector2 + _pos, 0f);
		CreateLine(_camera, _transformComponent, _width, -vector2 + _pos, 0f);
		CreateLine(_camera, _transformComponent, _height, vector + _pos, 90f);
		CreateLine(_camera, _transformComponent, _height, -vector + _pos, 90f);
		if (_createAngleMarker)
		{
			float num = _width * 0.5f * 0.5f;
			float num2 = _height * 0.5f * 0.5f;
			CreateLine(_camera, _transformComponent, num, _pos + new Vector3(num * 0.5f, 0f, 0f), 0f);
			CreateLine(_camera, _transformComponent, num2, _pos + new Vector3(0f, num2 * 0.5f, 0f), 90f);
		}
	}

	public static void CreateCircle(Camera _camera, TransformC _transformComponent, Vector3 pos, float radius, bool createAngleMarker = false, int segments = 16)
	{
		float num = 360f / (float)segments;
		float num2 = (float)Math.PI * radius;
		num2 /= (float)segments;
		float num3 = 90f;
		for (int i = 0; i < segments + 1; i++)
		{
			Vector3 offset = pos + new Vector3(Mathf.Cos(num3 * ((float)Math.PI / 180f)), Mathf.Sin(num3 * ((float)Math.PI / 180f)), 0f) * radius;
			if (i > 0)
			{
				CreateLine(_camera, _transformComponent, num2 * 2f, offset, num * (float)i);
			}
			num3 = ToolBox.getCappedAngle(num3 + num);
		}
		if (createAngleMarker)
		{
			CreateLine(_camera, _transformComponent, radius, pos + new Vector3(0f, radius * 0.5f, 0f), 90f);
		}
	}

	public static void DrawVectorArray(Camera _camera, TransformC _tc, Vector2[] _points)
	{
		for (int i = 0; i < _points.Length - 1; i++)
		{
			Vector2 vector = _points[i];
			Vector2 vector2 = _points[i + 1];
			Vector2 vector3 = vector2 - vector;
			Vector2 vector4 = vector + vector3 * 0.5f;
			float offsetAngle = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
			CreateLine(_camera, _tc, vector3.magnitude, vector4, offsetAngle);
		}
	}

	public static void AddRandom(Vector2[] _array, float _amount)
	{
		for (int i = 0; i < _array.Length; i++)
		{
			_array[i] += new Vector2(UnityEngine.Random.Range(_amount * -0.5f, _amount * 0.5f), UnityEngine.Random.Range(_amount * -0.5f, _amount * 0.5f));
		}
	}

	public static void AddRadialRandom(Vector2[] _array, float _amount)
	{
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < _array.Length; i++)
		{
			zero += _array[i];
		}
		zero /= (float)_array.Length;
		for (int j = 0; j < _array.Length; j++)
		{
			_array[j] = (_array[j] - zero).normalized * (UnityEngine.Random.Range(0f, _amount) - _amount) + _array[j];
		}
	}

	public static void AddRadialRandomToOutside(Vector2[] _array, float _amount)
	{
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < _array.Length; i++)
		{
			zero += _array[i];
		}
		zero /= (float)_array.Length;
		for (int j = 0; j < _array.Length; j++)
		{
			_array[j] = (_array[j] - zero).normalized * UnityEngine.Random.Range(0f, _amount) + _array[j];
		}
	}

	public static void AddRadialStarShift(Vector2[] _array, float _amount)
	{
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < _array.Length; i++)
		{
			zero += _array[i];
		}
		zero /= (float)_array.Length;
		for (int j = 0; j < _array.Length; j++)
		{
			if (j % 2 == 0)
			{
				_array[j] = (_array[j] - zero).normalized * _amount + _array[j];
			}
		}
	}

	public static void ScaleVectorArray(Vector2[] _vectorArray, Vector2 _amount)
	{
		for (int i = 0; i < _vectorArray.Length; i++)
		{
			_vectorArray[i].x *= _amount.x;
			_vectorArray[i].y *= _amount.y;
		}
	}

	public static Vector2[] ExpandVectorArray(Vector2[] _vectorArray, float _amount)
	{
		Vector2[] array = new Vector2[_vectorArray.Length];
		_vectorArray.CopyTo(array, 0);
		if (array.Length > 2)
		{
			for (int i = 0; i < array.Length; i++)
			{
				Vector2 vector = array[i];
				Vector2 vector2;
				Vector2 vector3;
				if (i == 0)
				{
					vector2 = array[array.Length - 1];
					vector3 = array[i + 1];
				}
				else if (i == array.Length - 1)
				{
					vector2 = array[i - 1];
					vector3 = array[0];
				}
				else
				{
					vector2 = array[i - 1];
					vector3 = array[i + 1];
				}
				Vector2 vector4 = vector3 - vector2;
				float f = Mathf.Atan2(0f - vector4.y, vector4.x);
				float num = Mathf.Sin(f) * _amount;
				float num2 = Mathf.Cos(f) * _amount;
				array[i] = new Vector2(vector.x + num, vector.y + num2);
			}
		}
		return array;
	}

	public static Vector2[] ExtrudePath(Vector2[] _path, float _width)
	{
		Vector2[] array = new Vector2[_path.Length * 2];
		if (_path.Length > 1)
		{
			for (int i = 0; i < _path.Length; i++)
			{
				Vector2 vector = _path[i];
				Vector2 vector2 = ((i != 0) ? ((i != _path.Length - 1) ? _path[i + 1] : (_path[i] + (_path[i] - _path[i - 1]))) : _path[i + 1]);
				Vector2 vector3 = vector2 - vector;
				float f = Mathf.Atan2(0f - vector3.y, vector3.x);
				float num = Mathf.Sin(f) * _width * 0.5f;
				float num2 = Mathf.Cos(f) * _width * 0.5f;
				array[i] = new Vector2(vector.x - num, vector.y - num2);
				array[array.Length - i - 1] = new Vector2(vector.x + num, vector.y + num2);
			}
		}
		return array;
	}

	public static Vector2[] GetCircle(float _radius, int _segments, Vector2 _offset)
	{
		return GetCircle(_radius, _segments, _offset, false);
	}

	public static Vector2[] GetCircle(float _radius, int _segments, Vector2 _offset, bool _closed)
	{
		if (_closed)
		{
			return GetArc(_radius, _segments, 360f, 0f, _offset);
		}
		return GetArc(_radius, _segments, 360f - 360f / (float)_segments, 0f, _offset);
	}

	public static Vector2[] GetRect(float _width, float _height, Vector2 _offset)
	{
		return GetRect(_width, _height, _offset, false);
	}

	public static Vector2[] GetRect(float _width, float _height, Vector2 _offset, bool _closed)
	{
		return GetRoundedRect(_width, _height, 0f, 1, _offset, _closed);
	}

	public static Vector2[] GetLine(Vector2 _start, Vector2 _end, int _middlePointCount)
	{
		Vector2[] array = new Vector2[_middlePointCount + 2];
		Vector2 vector = _end - _start;
		int num = _middlePointCount + 1;
		Vector2 vector2 = vector / num;
		array[0] = _start;
		array[array.Length - 1] = _end;
		for (int i = 1; i < array.Length - 1; i++)
		{
			array[i] = _start + vector2 * i;
		}
		return array;
	}

	public static Vector2[] GetRoundedRect(float _width, float _height, float _radius, int _segments, Vector2 _offset)
	{
		return GetRoundedRect(_width, _height, _radius, _segments, _offset, true);
	}

	public static Vector2[] GetRoundedRect(float _width, float _height, float _radius, int _segments, Vector2 _offset, bool _closed)
	{
		Vector2[] array = null;
		int num = 0;
		if (_closed)
		{
			num = 1;
		}
		if (_height > _radius * 2f && _width > _radius * 2f)
		{
			array = new Vector2[_segments * 4 + num];
			Vector2[] arc = GetArc(_radius, _segments, 90f, 270f, new Vector2(_width * 0.5f - _radius, _height * -0.5f + _radius) + _offset);
			Vector2[] arc2 = GetArc(_radius, _segments, 90f, 180f, new Vector2(_width * -0.5f + _radius, _height * -0.5f + _radius) + _offset);
			Vector2[] arc3 = GetArc(_radius, _segments, 90f, 90f, new Vector2(_width * -0.5f + _radius, _height * 0.5f - _radius) + _offset);
			Vector2[] arc4 = GetArc(_radius, _segments, 90f, 0f, new Vector2(_width * 0.5f - _radius, _height * 0.5f - _radius) + _offset);
			arc.CopyTo(array, 0);
			arc2.CopyTo(array, _segments * 1);
			arc3.CopyTo(array, _segments * 2);
			arc4.CopyTo(array, _segments * 3);
			if (_closed)
			{
				array[array.Length - 1] = arc[0];
			}
		}
		else if (_height > _radius * 2f)
		{
			_radius = _width * 0.5f;
			array = new Vector2[_segments * 4 + num];
			Vector2[] arc5 = GetArc(_radius, _segments * 2, 180f, 180f, new Vector2(0f, _height * -0.5f + _radius) + _offset);
			Vector2[] arc6 = GetArc(_radius, _segments * 2, 180f, 0f, new Vector2(0f, _height * 0.5f - _radius) + _offset);
			arc5.CopyTo(array, 0);
			arc6.CopyTo(array, _segments * 2);
			if (_closed)
			{
				array[array.Length - 1] = arc5[0];
			}
		}
		else if (_width > _radius * 2f)
		{
			_radius = _height * 0.5f;
			array = new Vector2[_segments * 4 + num];
			Vector2[] arc7 = GetArc(_radius, _segments * 2, 180f, 90f, new Vector2(_width * -0.5f + _radius, 0f) + _offset);
			Vector2[] arc8 = GetArc(_radius, _segments * 2, 180f, 270f, new Vector2(_width * 0.5f - _radius, 0f) + _offset);
			arc7.CopyTo(array, 0);
			arc8.CopyTo(array, _segments * 2);
			if (_closed)
			{
				array[array.Length - 1] = arc7[0];
			}
		}
		else
		{
			array = new Vector2[_segments * 4 + num];
			Vector2[] arc9 = GetArc(_radius, _segments * 4, 360f - 360f / (float)_segments * 4f, 0f, _offset);
			arc9.CopyTo(array, 0);
			if (_closed)
			{
				array[array.Length - 1] = arc9[0];
			}
		}
		return array;
	}

	public static Vector2[] GetArc(float radius, int _segments, float _arcAngle, float _startAngle, Vector2 _offset)
	{
		Vector2[] array = new Vector2[_segments];
		float num = _arcAngle / (float)(_segments - 1);
		float num2 = (float)Math.PI * radius;
		num2 /= (float)_segments;
		float num3 = _startAngle;
		for (int num4 = _segments - 1; num4 > -1; num4--)
		{
			array[num4] = _offset + new Vector2(Mathf.Cos(num3 * ((float)Math.PI / 180f)), Mathf.Sin(num3 * ((float)Math.PI / 180f))) * radius;
			num3 = ToolBox.getCappedAngle(num3 + num);
		}
		return array;
	}

	public static Vector2[] GetBezierRect(float _width, float _height, float _cornerSize, int _segments, Vector2 _offset)
	{
		if (_width < 0f || _height < 0f)
		{
			Debug.LogWarning("Negative width or height!");
			return null;
		}
		Vector2[] array = new Vector2[_segments * 4];
		Vector2 vector = new Vector2(_width * -0.5f, _height * 0.5f);
		Vector2 vector2 = new Vector2(_width * 0.5f, _height * 0.5f);
		Vector2 vector3 = new Vector2(_width * 0.5f, _height * -0.5f);
		Vector2 vector4 = new Vector2(_width * -0.5f, _height * -0.5f);
		float num = 0.5f;
		Vector2 p = vector + new Vector2(_cornerSize * num, _cornerSize);
		Vector2 p2 = vector2 + new Vector2((0f - _cornerSize) * num, _cornerSize);
		Vector2 p3 = vector2 + new Vector2(_cornerSize, (0f - _cornerSize) * num);
		Vector2 p4 = vector3 + new Vector2(_cornerSize, _cornerSize * num);
		Vector2 p5 = vector3 + new Vector2((0f - _cornerSize) * num, 0f - _cornerSize);
		Vector2 p6 = vector4 + new Vector2(_cornerSize * num, 0f - _cornerSize);
		Vector2 p7 = vector4 + new Vector2(0f - _cornerSize, _cornerSize * num);
		Vector2 p8 = vector + new Vector2(0f - _cornerSize, (0f - _cornerSize) * num);
		for (int i = 0; i < _segments; i++)
		{
			array[i] = ToolBox.GetBezierPoint((float)i / (float)_segments, vector, p, p2, vector2);
			array[i + _segments] = ToolBox.GetBezierPoint((float)i / (float)_segments, vector2, p3, p4, vector3);
			array[i + _segments * 2] = ToolBox.GetBezierPoint((float)i / (float)_segments, vector3, p5, p6, vector4);
			array[i + _segments * 3] = ToolBox.GetBezierPoint((float)i / (float)_segments, vector4, p7, p8, vector);
		}
		return array;
	}

	public static Vector2[] AddSpeechHandleToVectorArray(Vector2[] _array, SpeechBubbleHandlePosition _handlePosition, SpeechBubbleHandleType _handleType)
	{
		int num = Mathf.RoundToInt((float)_array.Length * 0.25f);
		int num2 = num * 2 + Mathf.RoundToInt((float)num * 0.5f);
		switch (_handlePosition)
		{
		case SpeechBubbleHandlePosition.BottomLeft:
			num2 = num * 2 + Mathf.RoundToInt((float)num * 0.75f);
			break;
		case SpeechBubbleHandlePosition.BottomRight:
			num2 = num * 2 + Mathf.RoundToInt((float)num * 0.25f);
			break;
		case SpeechBubbleHandlePosition.Left:
			num2 = num * 3 + Mathf.RoundToInt((float)num * 0.382f);
			break;
		case SpeechBubbleHandlePosition.Right:
			num2 = num * 1 + Mathf.RoundToInt((float)num * 0.618f);
			break;
		case SpeechBubbleHandlePosition.TopLeft:
			num2 = num * 0 + Mathf.RoundToInt((float)num * 0.25f);
			break;
		case SpeechBubbleHandlePosition.TopRight:
			num2 = num * 0 + Mathf.RoundToInt((float)num * 0.75f);
			break;
		}
		float num3 = Vector2.Distance(_array[0], _array[num * 3]);
		float num4 = num3 * 0.3f;
		float num5 = num4 * 0.25f;
		Vector2 vector = Vector2.zero;
		switch (_handleType)
		{
		case SpeechBubbleHandleType.SmallToLeft:
			num4 = (float)Screen.height * 0.05f;
			num5 = num4 * 0.25f;
			vector = Vector2.right * (0f - num5);
			break;
		case SpeechBubbleHandleType.SmallToRight:
			num4 = (float)Screen.height * 0.05f;
			num5 = num4 * 0.25f;
			vector = Vector2.right * num5;
			break;
		}
		List<Vector2> list = new List<Vector2>(_array);
		Vector2 vector2 = list[num2 - 1];
		Vector2 vector3 = list[num2];
		Vector2 vector4 = list[num2 + 1];
		Vector2 item = vector3 + (vector2 - vector3).normalized * num5;
		Vector2 item2 = vector3 + (vector4 - vector3).normalized * num5;
		Vector2 vector5 = Vector2.up * (0f - num4) + vector;
		switch (_handlePosition)
		{
		case SpeechBubbleHandlePosition.Left:
			vector5 = Vector2.right * (0f - num4) + new Vector2(vector.y, 0f - vector.x);
			break;
		case SpeechBubbleHandlePosition.Right:
			vector5 = Vector2.right * num4 + new Vector2(vector.y, vector.x);
			break;
		case SpeechBubbleHandlePosition.TopLeft:
		case SpeechBubbleHandlePosition.TopRight:
			vector5 = Vector2.up * num4 + -vector;
			break;
		}
		List<Vector2> list3;
		List<Vector2> list2 = (list3 = list);
		int index2;
		int index = (index2 = num2);
		Vector2 vector6 = list3[index2];
		list2[index] = vector6 + vector5;
		list.Insert(num2 + 1, item2);
		list.Insert(num2, item);
		list.Insert(num2 + 1, list[num2 + 1]);
		return list.ToArray();
	}

	public static Vector2[] GetRoundedBezierRect(float _width, float _height, float _cornerSize = -1f, float _cornerRoundness = -1f, float _edgeRoundness = -1f, float _cornerRandom = 0.2f, bool _topLeftCorner = true, bool _topRightCorner = true, bool _bottomLeftCorner = true, bool _bottomRightCorner = true)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		if (_topLeftCorner)
		{
			num++;
			num2 = 1;
		}
		if (_topRightCorner)
		{
			num++;
			num3 = 1;
		}
		if (_bottomLeftCorner)
		{
			num++;
			num4 = 1;
		}
		if (_bottomRightCorner)
		{
			num++;
			num5 = 1;
		}
		float num6 = (float)Screen.height * 0.05f;
		if (_cornerSize != -1f)
		{
			num6 = _cornerSize;
		}
		float num7 = num6 * 0.618f;
		if (_cornerRoundness != -1f)
		{
			num7 = _cornerRoundness;
		}
		float num8 = (float)Screen.height * 0.005f;
		if (_edgeRoundness != -1f)
		{
			num8 = _edgeRoundness;
		}
		int num9 = 7;
		int num10 = 10;
		float num11 = 1f / (float)(num9 - 1);
		float num12 = 1f / (float)(num10 - 1);
		Vector2[] array = new Vector2[num * (num9 - 1) + 4 * (num10 - 1) + 1];
		Vector2 vector = Vector2.zero;
		Vector2 zero = Vector2.zero;
		Vector2 vector2 = Vector2.zero;
		Vector2 p = Vector2.zero;
		Vector2 p2 = Vector2.zero;
		Vector2 p3 = Vector2.zero;
		int num13 = 1;
		int num14 = 1;
		for (int i = 0; i < 8; i++)
		{
			if (num14 == 1)
			{
				vector = new Vector2(num6 * UnityEngine.Random.Range(0f - _cornerRandom, _cornerRandom), 0f);
				vector = new Vector2(num6 * UnityEngine.Random.Range(0f - _cornerRandom, _cornerRandom), 0f);
				switch (i)
				{
				case 0:
					vector2 = new Vector2(_width * -0.5f + num6 * (float)num2, _height * 0.5f) + vector * num2;
					p3 = new Vector2(_width * 0.5f - num6 * (float)num3, _height * 0.5f) + vector * num3;
					p = new Vector2(vector2.x + num7, vector2.y + num8) + vector * num2;
					p2 = new Vector2(p3.x - num7, p3.y + num8) + vector * num3;
					array[0] = vector2;
					break;
				case 2:
					vector2 = array[num13 - 1];
					p3 = new Vector2(_width * 0.5f, _height * -0.5f + num6 * (float)num5) + zero * num5;
					p = new Vector2(vector2.x + num8, vector2.y - num7);
					p2 = new Vector2(p3.x + num8, p3.y + num7);
					break;
				case 4:
					vector2 = array[num13 - 1];
					p3 = new Vector2(_width * -0.5f + num6 * (float)num4, _height * -0.5f) + vector * num4;
					p = new Vector2(vector2.x - num7, vector2.y - num8);
					p2 = new Vector2(p3.x + num7, p3.y - num8);
					break;
				case 6:
					vector2 = array[num13 - 1];
					p3 = new Vector2(_width * -0.5f, _height * 0.5f - num6 * (float)num2) + zero * num2;
					p = new Vector2(vector2.x - num8, vector2.y + num7);
					p2 = new Vector2(p3.x - num8, p3.y - num7);
					break;
				}
				for (int j = 1; j < num10; j++)
				{
					array[num13] = ToolBox.GetBezierPoint((float)j * num12, vector2, p, p2, p3);
					num13++;
				}
			}
			else
			{
				switch (i)
				{
				case 1:
					vector2 = array[num13 - 1];
					p3 = new Vector2(_width * 0.5f, _height * 0.5f - num6) + zero;
					p = new Vector2(vector2.x + num7, vector2.y - num8);
					p2 = new Vector2(p3.x - num8, p3.y + num7);
					if (_topRightCorner)
					{
						for (int m = 1; m < num9; m++)
						{
							array[num13] = ToolBox.GetBezierPoint((float)m * num11, vector2, p, p2, p3);
							num13++;
						}
					}
					break;
				case 3:
					vector2 = array[num13 - 1];
					p3 = new Vector2(_width * 0.5f - num6, _height * -0.5f) + vector;
					p = new Vector2(vector2.x - num8, vector2.y - num7);
					p2 = new Vector2(p3.x + num7, p3.y + num8);
					if (_bottomRightCorner)
					{
						for (int l = 1; l < num9; l++)
						{
							array[num13] = ToolBox.GetBezierPoint((float)l * num11, vector2, p, p2, p3);
							num13++;
						}
					}
					break;
				case 5:
					vector2 = array[num13 - 1];
					p3 = new Vector2(_width * -0.5f, _height * -0.5f + num6) + zero;
					p = new Vector2(vector2.x - num7, vector2.y + num8);
					p2 = new Vector2(p3.x + num8, p3.y - num7);
					if (_bottomLeftCorner)
					{
						for (int n = 1; n < num9; n++)
						{
							array[num13] = ToolBox.GetBezierPoint((float)n * num11, vector2, p, p2, p3);
							num13++;
						}
					}
					break;
				case 7:
					vector2 = array[num13 - 1];
					p3 = array[0];
					p = new Vector2(vector2.x + num8, vector2.y + num7);
					p2 = new Vector2(p3.x - num7, p3.y - num8);
					if (_topLeftCorner)
					{
						for (int k = 1; k < num9; k++)
						{
							array[num13] = ToolBox.GetBezierPoint((float)k * num11, vector2, p, p2, p3);
							num13++;
						}
					}
					break;
				}
			}
			num14 *= -1;
		}
		return array;
	}

	public static Color GetColor(float _r, float _g, float _b)
	{
		return new Color(_r / 255f, _g / 255f, _b / 255f);
	}

	public static Color GetColor(float _r, float _g, float _b, float _a)
	{
		return new Color(_r / 255f, _g / 255f, _b / 255f, _a / 255f);
	}

	public static uint ColorToUInt(Color _color)
	{
		return (uint)((Mathf.RoundToInt(_color.a * 255f) << 24) | (Mathf.RoundToInt(_color.r * 255f) << 16) | (Mathf.RoundToInt(_color.g * 255f) << 8) | Mathf.RoundToInt(_color.b * 255f));
	}

	public static Color UIntToColor(uint _uint)
	{
		return GetColor((_uint >> 16) & 0xFF, (_uint >> 8) & 0xFF, _uint & 0xFF, (_uint >> 24) & 0xFF);
	}

	public static string ColorToHex(Color32 _color)
	{
		return _color.r.ToString("X2") + _color.g.ToString("X2") + _color.b.ToString("X2");
	}

	public static Color HexToColor(string _hex)
	{
		_hex = _hex.Replace("#", string.Empty);
		byte r = byte.Parse(_hex.Substring(0, 2), NumberStyles.HexNumber);
		byte g = byte.Parse(_hex.Substring(2, 2), NumberStyles.HexNumber);
		byte b = byte.Parse(_hex.Substring(4, 2), NumberStyles.HexNumber);
		return new Color32(r, g, b, byte.MaxValue);
	}

	public static uint HexToUint(string _hex)
	{
		return ColorToUInt(HexToColor(_hex));
	}

	public static Vector2[] VerticalSkew(Vector2[] _points, float _skewCenterX, float _skewAmount, float _skewAmountRelativeTo)
	{
		Vector2[] array = new Vector2[_points.Length];
		_points.CopyTo(array, 0);
		for (int i = 0; i < array.Length; i++)
		{
			float num = (array[i].x - _skewCenterX) / _skewAmountRelativeTo * _skewAmount;
			array[i].y += num;
		}
		return array;
	}
}
