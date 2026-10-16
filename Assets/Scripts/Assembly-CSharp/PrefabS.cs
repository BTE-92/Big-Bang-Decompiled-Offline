using System.Collections.Generic;
using UnityEngine;

public static class PrefabS
{
	public static DynamicArray<PrefabC> m_components;

	public static GameObject m_emptyGameObject;

	public static void Initialize()
	{
		m_components = new DynamicArray<PrefabC>();
		m_emptyGameObject = new GameObject("PrefabSystem: InstantiateHelper");
		MeshFilter meshFilter = m_emptyGameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = new Mesh();
		MeshRenderer meshRenderer = m_emptyGameObject.AddComponent<MeshRenderer>();
		meshRenderer.enabled = false;
	}

	public static PrefabC AddComponent(TransformC _parentTC, Vector3 _offset)
	{
		return AddComponent(_parentTC, _offset, string.Empty);
	}

	public static PrefabC AddComponent(TransformC _parentTC, Vector3 _offset, string _name)
	{
		PrefabC prefabC = m_components.AddItem();
		prefabC.p_gameObject = Object.Instantiate(m_emptyGameObject);
		prefabC.p_gameObject.GetComponent<Renderer>().enabled = true;
		prefabC.p_mesh = (prefabC.p_gameObject.GetComponent("MeshFilter") as MeshFilter).mesh;
		prefabC.p_gameObject.transform.parent = _parentTC.transform;
		prefabC.p_gameObject.transform.localPosition = _offset;
		prefabC.p_gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
		prefabC.p_parentTC = _parentTC;
		prefabC.m_name = _name;
		prefabC.m_wasVisible = true;
		prefabC.p_gameObject.GetComponent<Renderer>().castShadows = false;
		prefabC.p_gameObject.GetComponent<Renderer>().receiveShadows = false;
		EntityManager.AddComponentToEntity(_parentTC.p_entity, prefabC);
		return prefabC;
	}

	public static PrefabC AddComponent(TransformC _parentTC, Vector3 _offset, GameObject _gameObject)
	{
		return AddComponent(_parentTC, _offset, _gameObject, string.Empty);
	}

	public static PrefabC AddComponent(TransformC _parentTC, Vector3 _offset, GameObject _gameObject, string _identifier, bool _resetLocalRotation = true, bool _resetLocalPosition = true)
	{
		PrefabC prefabC = m_components.AddItem();
		Vector3 position = _gameObject.transform.position;
		prefabC.p_gameObject = Object.Instantiate(_gameObject, _parentTC.transform.position, _gameObject.transform.rotation) as GameObject;
		MeshFilter meshFilter = prefabC.p_gameObject.GetComponent("MeshFilter") as MeshFilter;
		if (meshFilter != null)
		{
			prefabC.p_mesh = meshFilter.mesh;
		}
		else
		{
			prefabC.p_mesh = null;
		}
		prefabC.p_gameObject.transform.parent = _parentTC.transform;
		if (_resetLocalPosition)
		{
			prefabC.p_gameObject.transform.localPosition = _offset;
		}
		else
		{
			prefabC.p_gameObject.transform.localPosition = position;
		}
		if (_resetLocalRotation)
		{
			prefabC.p_gameObject.transform.localRotation = Quaternion.identity;
		}
		prefabC.p_parentTC = _parentTC;
		prefabC.m_name = _identifier;
		prefabC.m_wasVisible = true;
		EntityManager.AddComponentToEntity(_parentTC.p_entity, prefabC);
		return prefabC;
	}

	public static PrefabC AddComponent(TransformC _parentTC, GameObject _gameObject, string _identifier = "")
	{
		PrefabC prefabC = m_components.AddItem();
		prefabC.p_gameObject = Object.Instantiate(_gameObject);
		MeshFilter meshFilter = prefabC.p_gameObject.GetComponent("MeshFilter") as MeshFilter;
		if (meshFilter != null)
		{
			prefabC.p_mesh = meshFilter.mesh;
		}
		else
		{
			prefabC.p_mesh = null;
		}
		prefabC.p_gameObject.transform.parent = _parentTC.transform;
		prefabC.p_parentTC = _parentTC;
		prefabC.m_name = _identifier;
		prefabC.m_wasVisible = true;
		EntityManager.AddComponentToEntity(_parentTC.p_entity, prefabC);
		return prefabC;
	}

	public static void RemoveComponent(PrefabC _c, bool _removeInstancedSharedMaterial = true)
	{
		if (_c.p_entity == null)
		{
			Debug.LogError("Trying to remove component that has already been removed");
			return;
		}
		if (_removeInstancedSharedMaterial && _c.p_gameObject != null && _c.p_gameObject.GetComponent<Renderer>() != null && _c.p_gameObject.GetComponent<Renderer>().sharedMaterial != null && (_c.p_gameObject.GetComponent<Renderer>().sharedMaterial.name.Contains("Instance") || _c.p_gameObject.GetComponent<Renderer>().sharedMaterial.name.Contains("Clone")))
		{
			Object.DestroyImmediate(_c.p_gameObject.GetComponent<Renderer>().sharedMaterial);
		}
		if (_c.p_mesh != null)
		{
			Object.Destroy(_c.p_mesh);
		}
		if (_c.p_gameObject != null)
		{
			Object.Destroy(_c.p_gameObject);
		}
		_c.p_mesh = null;
		_c.p_gameObject = null;
		_c.p_parentTC = null;
		_c.m_name = string.Empty;
		_c.m_wasVisible = false;
		EntityManager.RemoveComponentFromEntity(_c);
		m_components.RemoveItem(_c);
	}

	public static void RemoveComponentsByEntity(Entity _e, bool _removeInstancedSharedMaterial = true)
	{
		List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.Prefab, _e);
		while (componentsByEntity.Count > 0)
		{
			int index = componentsByEntity.Count - 1;
			RemoveComponent(componentsByEntity[index] as PrefabC, _removeInstancedSharedMaterial);
			componentsByEntity.RemoveAt(index);
		}
	}

	public static void Update()
	{
		m_components.Update();
	}

	public static void SetCamera(PrefabC _c, Camera _camera)
	{
		SetCameraLayer(_c, _camera.gameObject.layer);
	}

	public static void SetCameraLayer(PrefabC _c, int _cameraLayer)
	{
		_c.p_gameObject.layer = _cameraLayer;
		for (int i = 0; i < _c.p_gameObject.transform.childCount; i++)
		{
			SetCameraLayer(_c.p_gameObject.transform.GetChild(i).gameObject, _cameraLayer);
		}
	}

	public static void SetCamera(GameObject _go, Camera _camera)
	{
		SetCameraLayer(_go, _camera.gameObject.layer);
	}

	public static void SetCameraLayer(GameObject _go, int _cameraLayer)
	{
		_go.layer = _cameraLayer;
		for (int i = 0; i < _go.transform.childCount; i++)
		{
			SetCameraLayer(_go.transform.GetChild(i).gameObject, _cameraLayer);
		}
	}

	public static void PauseParticleEmission(PrefabC _c, bool _pause)
	{
		Component[] componentsInChildren = _c.p_gameObject.GetComponentsInChildren(typeof(ParticleSystem));
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			ParticleSystem particleSystem = componentsInChildren[i] as ParticleSystem;
			if (_pause)
			{
				particleSystem.enableEmission = false;
			}
			else
			{
				particleSystem.enableEmission = true;
			}
		}
	}

	public static void PauseParticleSystems(PrefabC _c, bool _pause)
	{
		Component[] componentsInChildren = _c.p_gameObject.GetComponentsInChildren(typeof(ParticleSystem));
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			ParticleSystem particleSystem = componentsInChildren[i] as ParticleSystem;
			if (_pause && particleSystem.IsAlive())
			{
				particleSystem.Pause(true);
			}
			else if (particleSystem.isPaused)
			{
				particleSystem.Play(true);
			}
		}
	}

	public static void PauseAnimations(PrefabC _c, bool _pause)
	{
		Component[] componentsInChildren = _c.p_gameObject.GetComponentsInChildren(typeof(Animator));
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Animator animator = componentsInChildren[i] as Animator;
			if (_pause)
			{
				animator.speed = 0f;
			}
			else
			{
				animator.speed = 1f;
			}
		}
	}

	public static void SetVertexColors(PrefabC _c, Color _color)
	{
		MeshFilter meshFilter = _c.p_gameObject.GetComponent("MeshFilter") as MeshFilter;
		Mesh mesh = meshFilter.mesh;
		SetVertexColors(mesh, _color);
	}

	public static void SetVertexColors(GameObject _gameObject, Color _color)
	{
		MeshFilter meshFilter = _gameObject.GetComponent("MeshFilter") as MeshFilter;
		Mesh mesh = meshFilter.mesh;
		SetVertexColors(mesh, _color);
	}

	public static void SetVertexColors(Mesh _mesh, Color _color)
	{
		Color[] array = new Color[_mesh.colors.Length];
		for (int i = 0; i < _mesh.colors.Length; i++)
		{
			array[i] = _color;
		}
		_mesh.colors = array;
	}

	public static void SetVisibilityByTransformComponent(TransformC _tc, bool _visible, bool _affectChildren = false, bool _affectWholeHierarchy = false)
	{
		if (_affectWholeHierarchy)
		{
			_tc = TransformS.GetRootTransformComponent(_tc);
		}
		if (_affectChildren || _affectWholeHierarchy)
		{
			for (int i = 0; i < _tc.childs.Count; i++)
			{
				SetVisibilityByTransformComponent(_tc.childs[i], _visible, true);
			}
		}
		int aliveCount = m_components.m_aliveCount;
		for (int j = 0; j < aliveCount; j++)
		{
			PrefabC prefabC = m_components.m_array[m_components.m_aliveIndices[j]];
			if (prefabC.p_parentTC == _tc)
			{
				SetVisibility(prefabC, _visible);
			}
		}
	}

	public static void SetVisibility(PrefabC _c, bool _visible, bool _markVisibility = true)
	{
		_c.p_gameObject.SetActive(_visible);
		if (_markVisibility)
		{
			_c.m_wasVisible = _visible;
		}
	}

	public static void ColorizeByTransformComponent(TransformC _tc, Color _color, bool _affectChildren, bool _affectWholeHierarchy)
	{
		if (_affectWholeHierarchy)
		{
			_tc = TransformS.GetRootTransformComponent(_tc);
		}
		if (_affectChildren || _affectWholeHierarchy)
		{
			for (int i = 0; i < _tc.childs.Count; i++)
			{
				ColorizeByTransformComponent(_tc.childs[i], _color, true, false);
			}
		}
		int aliveCount = m_components.m_aliveCount;
		for (int j = 0; j < aliveCount; j++)
		{
			PrefabC prefabC = m_components.m_array[m_components.m_aliveIndices[j]];
			if (prefabC.p_parentTC == _tc && prefabC.p_mesh != null)
			{
				SetVertexColors(prefabC.p_mesh, _color);
			}
		}
	}

	public static Color GetShaderColor(PrefabC _c)
	{
		return _c.p_gameObject.GetComponent<Renderer>().material.GetColor("_Color");
	}

	public static void SetShaderColor(PrefabC _c, Color _color)
	{
		if (_c.p_gameObject.GetComponent<Renderer>() != null)
		{
			_c.p_gameObject.GetComponent<Renderer>().material.SetColor("_Color", _color);
		}
		for (int i = 0; i < _c.p_gameObject.transform.childCount; i++)
		{
			SetShaderColorToGameObjectHierarchy(_c.p_gameObject.transform.GetChild(i), _color);
		}
	}

	public static void SetShaderColorToGameObjectHierarchy(Transform _t, Color _color)
	{
		if (_t.gameObject.GetComponent<Renderer>() != null)
		{
			_t.gameObject.GetComponent<Renderer>().material.SetColor("_Color", _color);
		}
		for (int i = 0; i < _t.gameObject.transform.childCount; i++)
		{
			SetShaderColorToGameObjectHierarchy(_t.gameObject.transform.GetChild(i), _color);
		}
	}

	public static PrefabC CreateRect(TransformC _tc, Vector3 _offset, float _width, float _height, Color _color, Material _material, Camera _camera)
	{
		PrefabC prefabC = AddComponent(_tc, Vector3.zero);
		prefabC.p_gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
		prefabC.p_gameObject.layer = _camera.gameObject.layer;
		prefabC.p_gameObject.GetComponent<Renderer>().material = _material;
		Vector3[] vertices = new Vector3[4]
		{
			new Vector3(_width * -0.5f, _height * 0.5f, 0f) + _offset,
			new Vector3(_width * 0.5f, _height * 0.5f, 0f) + _offset,
			new Vector3(_width * 0.5f, _height * -0.5f, 0f) + _offset,
			new Vector3(_width * -0.5f, _height * -0.5f, 0f) + _offset
		};
		Vector2[] uv = new Vector2[4]
		{
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f),
			new Vector2(0f, 0f)
		};
		Color[] colors = new Color[4] { _color, _color, _color, _color };
		int[] triangles = new int[6] { 0, 1, 2, 2, 3, 0 };
		prefabC.p_mesh.triangles = null;
		prefabC.p_mesh.vertices = null;
		prefabC.p_mesh.vertices = vertices;
		prefabC.p_mesh.triangles = triangles;
		prefabC.p_mesh.uv = uv;
		prefabC.p_mesh.colors = colors;
		prefabC.p_mesh.RecalculateBounds();
		prefabC.p_mesh.RecalculateNormals();
		return prefabC;
	}

	public static PrefabC CreatePathPrefabComponentFromVectorArray(TransformC _tc, Vector3 _offset, Vector2[] _points, float _width, Color _color, Material _material, Camera _camera, Position _align, bool _closed)
	{
		return CreatePathPrefabComponentFromVectorArray(_tc, _offset, _points, _width, _color, _color, _material, _camera, _align, _closed);
	}

	public static PrefabC CreatePathPrefabComponentFromVectorArray(TransformC _tc, Vector3 _offset, Vector2[] _points, float _width, Color _bottomColor, Color _topColor, Material _material, Camera _camera, Position _align, bool _closed)
	{
		float num = 99999f;
		float num2 = -99999f;
		float num3 = 99999f;
		float num4 = -99999f;
		bool flag = _topColor != _bottomColor;
		if (flag)
		{
			for (int i = 0; i < _points.Length; i++)
			{
				Vector2 vector = _points[i];
				if (vector.y < num)
				{
					num = vector.y;
				}
				if (vector.y > num2)
				{
					num2 = vector.y;
				}
				if (vector.x < num3)
				{
					num3 = vector.x;
				}
				if (vector.x > num4)
				{
					num4 = vector.x;
				}
			}
		}
		float num5 = num2 - num;
		float num6 = num4 - num3;
		PrefabC prefabC = AddComponent(_tc, Vector3.zero);
		prefabC.p_gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
		prefabC.p_gameObject.layer = _camera.gameObject.layer;
		prefabC.p_gameObject.GetComponent<Renderer>().material = _material;
		Vector2[] array;
		if (_points[0] - _points[_points.Length - 1] == Vector2.zero || !_closed)
		{
			array = _points;
		}
		else
		{
			array = new Vector2[_points.Length + 1];
			_points.CopyTo(array, 0);
			array[array.Length - 1] = array[0];
		}
		Vector3[] array2 = new Vector3[array.Length * 2];
		Vector3[] array3 = new Vector3[array.Length * 2];
		Vector2[] array4 = new Vector2[array.Length * 2];
		Color[] array5 = new Color[array.Length * 2];
		int[] array6 = new int[array.Length * 6];
		float num7 = 0f;
		for (int j = 0; j < array.Length; j++)
		{
			Vector2 vector2 = array[j];
			Vector2 vector3;
			Vector2 vector4;
			if (!_closed)
			{
				if (j == 0)
				{
					vector3 = array[j] - (array[j + 1] - array[j]);
					vector4 = array[j + 1];
				}
				else if (j == array.Length - 1)
				{
					vector3 = array[j - 1];
					vector4 = array[j] + (array[j] - array[j - 1]);
				}
				else
				{
					vector3 = array[j - 1];
					vector4 = array[j + 1];
				}
			}
			else if (j == 0)
			{
				vector3 = array[array.Length - 2];
				vector4 = array[j + 1];
			}
			else if (j == array.Length - 1)
			{
				vector3 = array[j - 1];
				vector4 = array[1];
			}
			else
			{
				vector3 = array[j - 1];
				vector4 = array[j + 1];
			}
			Vector2 normalized = (vector3 - vector2).normalized;
			Vector2 normalized2 = (vector2 - vector4).normalized;
			float f = Mathf.Atan2(0f - normalized.y, normalized.x);
			float x = Mathf.Sin(f);
			float y = Mathf.Cos(f);
			Vector2 vector5 = new Vector2(x, y);
			float f2 = Mathf.Atan2(0f - normalized2.y, normalized2.x);
			float x2 = Mathf.Sin(f2);
			float y2 = Mathf.Cos(f2);
			Vector2 vector6 = new Vector2(x2, y2);
			Vector2 normalized3 = ((vector5 + vector6) * 0.5f).normalized;
			Vector3 vector7 = new Vector3(normalized3.x, normalized3.y, 0f);
			Vector3 vector8 = new Vector3(vector2.x, vector2.y, 0f);
			Vector3 vector9 = vector8;
			Vector3 vector10 = vector8;
			switch (_align)
			{
			case Position.Center:
				vector9 = vector8 + vector7 * _width * 0.5f;
				vector10 = vector8 - vector7 * _width * 0.5f;
				break;
			case Position.Inside:
				vector9 = vector8 + vector7 * _width;
				vector10 = vector8;
				break;
			case Position.Outside:
				vector9 = vector8;
				vector10 = vector8 - vector7 * _width;
				break;
			}
			array2[j * 2] = vector9 + _offset;
			array2[j * 2 + 1] = vector10 + _offset;
			num7 += (vector2 - vector3).magnitude;
			array4[j * 2] = Vector2.up * num7;
			array4[j * 2 + 1] = Vector2.up * num7 + Vector2.right;
			array3[j * 2] = Vector3.forward;
			array3[j * 2 + 1] = Vector3.forward;
			if (flag)
			{
				float num8 = (vector2.y - num) / num5;
				array5[j * 2] = _topColor * num8 + _bottomColor * (1f - num8);
				array5[j * 2 + 1] = array5[j * 2];
			}
			else
			{
				array5[j * 2] = _topColor;
				array5[j * 2 + 1] = _topColor;
			}
			if (j < array.Length - 1)
			{
				array6[j * 6] = j * 2;
				array6[j * 6 + 1] = j * 2 + 1;
				array6[j * 6 + 2] = j * 2 + 2;
				array6[j * 6 + 3] = j * 2 + 2;
				array6[j * 6 + 4] = j * 2 + 1;
				array6[j * 6 + 5] = j * 2 + 3;
			}
		}
		prefabC.p_mesh.triangles = null;
		prefabC.p_mesh.vertices = null;
		prefabC.p_mesh.vertices = array2;
		prefabC.p_mesh.triangles = array6;
		prefabC.p_mesh.uv = array4;
		prefabC.p_mesh.colors = array5;
		prefabC.p_mesh.normals = array3;
		prefabC.p_mesh.RecalculateBounds();
		prefabC.p_mesh.RecalculateNormals();
		return prefabC;
	}

	public static PrefabC CreateSphericalPathFromVectorArray(TransformC _tc, Vector3 _offset, Vector3[] _points, float _width, Color _color, Material _material, Camera _camera, Position _align, bool _closed)
	{
		PrefabC prefabC = AddComponent(_tc, Vector3.zero);
		prefabC.p_gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
		prefabC.p_gameObject.layer = _camera.gameObject.layer;
		prefabC.p_gameObject.GetComponent<Renderer>().material = _material;
		Vector3[] array;
		if (_points[0] - _points[_points.Length - 1] == Vector3.zero || !_closed)
		{
			array = _points;
		}
		else
		{
			array = new Vector3[_points.Length + 1];
			_points.CopyTo(array, 0);
			array[array.Length - 1] = array[0];
		}
		Vector3[] array2 = new Vector3[array.Length * 2];
		Vector3[] array3 = new Vector3[array.Length * 2];
		Vector2[] array4 = new Vector2[array.Length * 2];
		Color[] array5 = new Color[array.Length * 2];
		int[] array6 = new int[array.Length * 6];
		float num = 0f;
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 vector = array[i];
			Vector3 vector2;
			Vector3 vector3;
			if (!_closed)
			{
				if (i == 0)
				{
					vector2 = array[i] - (array[i + 1] - array[i]);
					vector3 = array[i + 1];
				}
				else if (i == array.Length - 1)
				{
					vector2 = array[i - 1];
					vector3 = array[i] + (array[i] - array[i - 1]);
				}
				else
				{
					vector2 = array[i - 1];
					vector3 = array[i + 1];
				}
			}
			else if (i == 0)
			{
				vector2 = array[array.Length - 2];
				vector3 = array[i + 1];
			}
			else if (i == array.Length - 1)
			{
				vector2 = array[i - 1];
				vector3 = array[1];
			}
			else
			{
				vector2 = array[i - 1];
				vector3 = array[i + 1];
			}
			Vector3 normalized = (vector2 - vector).normalized;
			Vector3 normalized2 = (vector - vector3).normalized;
			Vector3 tangent = (normalized + normalized2) * 0.5f;
			Vector3 normal = vector.normalized;
			Vector3 binormal = Vector3.zero;
			Vector3.OrthoNormalize(ref normal, ref tangent, ref binormal);
			Vector3 vector4 = vector;
			Vector3 vector5 = vector;
			switch (_align)
			{
			case Position.Center:
				vector4 = vector - binormal * _width * 0.5f;
				vector5 = vector + binormal * _width * 0.5f;
				break;
			case Position.Inside:
				vector4 = vector - binormal * _width;
				vector5 = vector;
				break;
			case Position.Outside:
				vector4 = vector;
				vector5 = vector + binormal * _width;
				break;
			}
			array2[i * 2] = vector4 + _offset;
			array2[i * 2 + 1] = vector5 + _offset;
			num += (vector - vector2).magnitude;
			array4[i * 2] = Vector2.up * num;
			array4[i * 2 + 1] = Vector2.up * num + Vector2.right;
			array3[i * 2] = Vector3.forward;
			array3[i * 2 + 1] = Vector3.forward;
			array5[i * 2] = _color;
			array5[i * 2 + 1] = _color;
			if (i < array.Length - 1)
			{
				array6[i * 6] = i * 2;
				array6[i * 6 + 1] = i * 2 + 1;
				array6[i * 6 + 2] = i * 2 + 2;
				array6[i * 6 + 3] = i * 2 + 2;
				array6[i * 6 + 4] = i * 2 + 1;
				array6[i * 6 + 5] = i * 2 + 3;
			}
		}
		prefabC.p_mesh.triangles = null;
		prefabC.p_mesh.vertices = null;
		prefabC.p_mesh.vertices = array2;
		prefabC.p_mesh.triangles = array6;
		prefabC.p_mesh.uv = array4;
		prefabC.p_mesh.colors = array5;
		prefabC.p_mesh.normals = array3;
		prefabC.p_mesh.RecalculateBounds();
		prefabC.p_mesh.RecalculateNormals();
		return prefabC;
	}

	public static PrefabC CreateFlatPrefabComponentsFromPolygon(TransformC _tc, Vector3 _offset, GGData _polygon, Color _color, Material _material, Camera _camera)
	{
		uint num = DebugDraw.ColorToUInt(_color);
		return CreateFlatPrefabComponentsFromPolygon(_tc, _offset, _polygon, num, num, _material, _camera, string.Empty, null);
	}

	public static PrefabC CreateFlatPrefabComponentsFromPolygon(TransformC _tc, Vector3 _offset, GGData _polygon, Color _bottomColor, Color _topColor, Material _material, Camera _camera)
	{
		uint topColor = DebugDraw.ColorToUInt(_topColor);
		uint bottomColor = DebugDraw.ColorToUInt(_bottomColor);
		return CreateFlatPrefabComponentsFromPolygon(_tc, _offset, _polygon, bottomColor, topColor, _material, _camera, string.Empty, null);
	}

	public static PrefabC CreateFlatPrefabComponentsFromVectorArray(TransformC _tc, Vector3 _offset, Vector2[] _points, uint _bottomColor, uint _topColor, Material _material, Camera _camera, string _identifier, UVRect _normalizeRect = null)
	{
		GGData polygon = new GGData(_points);
		return CreateFlatPrefabComponentsFromPolygon(_tc, _offset, polygon, _bottomColor, _topColor, _material, _camera, _identifier, _normalizeRect);
	}

	public static PrefabC CreateFlatPrefabComponentsFromPolygon(TransformC _tc, Vector3 _offset, GGData _polygon, uint _bottomColor, uint _topColor, Material _material, Camera _camera, string _identifier, UVRect _normalizeRect)
	{
		if (_polygon.m_vertices.Count > 0)
		{
			_polygon.SetUvMapPlanar(1f, 1f, Vector2.zero, _normalizeRect);
			_polygon.SetColorGradientVertical(_topColor, _bottomColor);
			_polygon.SetOffset(_offset);
			Mesh mesh = GeometryGenerator.GenerateFlatMesh(_polygon);
			if (mesh != null)
			{
				return CreatePrefabFromMesh(_tc, mesh, _camera.gameObject.layer, _material, true, true);
			}
		}
		else
		{
			Debug.LogError("NO VERTICES IN POLYGON!");
		}
		return null;
	}

	public static PrefabC CreateBeltPrefabFromVectorArray(TransformC _tc, Vector3 _offset, float _depth, Vector2[] _points, Color _color, Material _material, Camera _camera, string _identifier)
	{
		PrefabC prefabC = AddComponent(_tc, _offset);
		prefabC.p_gameObject.layer = _camera.gameObject.layer;
		prefabC.p_gameObject.GetComponent<Renderer>().material = _material;
		if (_points[0] != _points[_points.Length - 1])
		{
			Vector2[] array = new Vector2[_points.Length + 1];
			_points.CopyTo(array, 0);
			array[array.Length - 1] = array[0];
			_points = array;
		}
		List<Vector3> list = new List<Vector3>(_points.Length * 2);
		List<Vector2> list2 = new List<Vector2>(_points.Length * 2);
		List<Color> list3 = new List<Color>(_points.Length * 2);
		List<int> list4 = new List<int>((_points.Length * 2 - 2) * 3);
		float num = 0f;
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < _points.Length; i++)
		{
			Vector2 vector = _points[i];
			if (i > 0)
			{
				num += (vector - _points[i - 1]).magnitude;
			}
			list.AddRange(new Vector3[2]);
			list2.AddRange(new Vector2[2]);
			list3.AddRange(new Color[2]);
			list4.AddRange(new int[6]);
			list[i * 2] = new Vector3(vector.x, vector.y, _depth);
			list[i * 2 + 1] = new Vector3(vector.x, vector.y, 0f);
			list2[i * 2] = new Vector2(num / 100f, 1f);
			list2[i * 2 + 1] = new Vector2(num / 100f, 0.5f);
			list3[i * 2] = _color;
			list3[i * 2 + 1] = _color;
			if (i > 0)
			{
				list4[(i * 2 - 2) * 3] = i * 2 - 2;
				list4[(i * 2 - 2) * 3 + 1] = i * 2 - 2 + 2;
				list4[(i * 2 - 2) * 3 + 2] = i * 2 - 2 + 1;
				list4[(i * 2 - 1) * 3] = i * 2 - 1;
				list4[(i * 2 - 1) * 3 + 1] = i * 2 - 1 + 1;
				list4[(i * 2 - 1) * 3 + 2] = i * 2 - 1 + 2;
			}
		}
		prefabC.p_mesh.vertices = list.ToArray();
		prefabC.p_mesh.triangles = list4.ToArray();
		prefabC.p_mesh.uv = list2.ToArray();
		prefabC.p_mesh.colors = list3.ToArray();
		prefabC.p_mesh.RecalculateBounds();
		prefabC.p_mesh.RecalculateNormals();
		return prefabC;
	}

	public static PrefabC CreateBeltPrefabFromVectorArray(TransformC _tc, Vector3 _offset, float _depth, Vector2[] _pointsFar, Vector2[] _pointsNear, Color _color, Material _material, Camera _camera, string _identifier)
	{
		if (_pointsFar.Length != _pointsNear.Length)
		{
			Debug.LogError("Vertex arrays has to be of same length");
			return null;
		}
		PrefabC prefabC = AddComponent(_tc, _offset);
		prefabC.p_gameObject.layer = _camera.gameObject.layer;
		prefabC.p_gameObject.GetComponent<Renderer>().material = _material;
		if (_pointsFar[0] != _pointsFar[_pointsFar.Length - 1])
		{
			Vector2[] array = new Vector2[_pointsFar.Length + 1];
			_pointsFar.CopyTo(array, 0);
			array[array.Length - 1] = array[0];
			_pointsFar = array;
			array = new Vector2[_pointsNear.Length + 1];
			_pointsNear.CopyTo(array, 0);
			array[array.Length - 1] = array[0];
			_pointsNear = array;
		}
		List<Vector3> list = new List<Vector3>(_pointsFar.Length * 2);
		List<Vector2> list2 = new List<Vector2>(_pointsFar.Length * 2);
		List<Color> list3 = new List<Color>(_pointsFar.Length * 2);
		List<int> list4 = new List<int>((_pointsFar.Length * 2 - 2) * 3);
		float num = 0f;
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < _pointsFar.Length; i++)
		{
			Vector2 vector = _pointsFar[i];
			Vector2 vector2 = _pointsNear[i];
			if (i > 0)
			{
				num += (vector - _pointsFar[i - 1]).magnitude;
			}
			list.AddRange(new Vector3[2]);
			list2.AddRange(new Vector2[2]);
			list3.AddRange(new Color[2]);
			list4.AddRange(new int[6]);
			list[i * 2] = new Vector3(vector.x, vector.y, _depth);
			list[i * 2 + 1] = new Vector3(vector2.x, vector2.y, 0f);
			list2[i * 2] = new Vector2(num / 100f, 1f);
			list2[i * 2 + 1] = new Vector2(num / 100f, 0.5f);
			list3[i * 2] = _color;
			list3[i * 2 + 1] = _color;
			if (i > 0)
			{
				list4[(i * 2 - 2) * 3] = i * 2 - 2;
				list4[(i * 2 - 2) * 3 + 1] = i * 2 - 2 + 2;
				list4[(i * 2 - 2) * 3 + 2] = i * 2 - 2 + 1;
				list4[(i * 2 - 1) * 3] = i * 2 - 1;
				list4[(i * 2 - 1) * 3 + 1] = i * 2 - 1 + 1;
				list4[(i * 2 - 1) * 3 + 2] = i * 2 - 1 + 2;
			}
		}
		prefabC.p_mesh.vertices = list.ToArray();
		prefabC.p_mesh.triangles = list4.ToArray();
		prefabC.p_mesh.uv = list2.ToArray();
		prefabC.p_mesh.colors = list3.ToArray();
		prefabC.p_mesh.RecalculateBounds();
		prefabC.p_mesh.RecalculateNormals();
		return prefabC;
	}

	public static PrefabC CreatePrefabFromMesh(TransformC _tc, Mesh _mesh, Camera _camera, Material _material, bool _destroyMesh, bool _recalcNormals = false, bool _optimize = false)
	{
		return CreatePrefabFromMesh(_tc, _mesh, _camera.gameObject.layer, _material, _destroyMesh, _recalcNormals, _optimize);
	}

	public static PrefabC CreatePrefabFromMesh(TransformC _tc, Mesh _mesh, int _cameraLayer, Material _material, bool _destroyMesh, bool _recalcNormals = false, bool _optimize = false)
	{
		if (_mesh != null)
		{
			PrefabC prefabC = AddComponent(_tc, Vector3.zero);
			CombineInstance[] array = new CombineInstance[1];
			array[0].mesh = _mesh;
			prefabC.p_mesh.CombineMeshes(array, true, false);
			prefabC.p_mesh.RecalculateBounds();
			if (_recalcNormals)
			{
				prefabC.p_mesh.RecalculateNormals();
			}
			if (_optimize)
			{
				prefabC.p_mesh.Optimize();
			}
			prefabC.p_gameObject.GetComponent<Renderer>().material = _material;
			prefabC.p_gameObject.layer = _cameraLayer;
			if (_destroyMesh)
			{
				Object.DestroyImmediate(_mesh);
			}
			return prefabC;
		}
		return null;
	}

	public static PrefabC CreatePrefabFromMeshArray(TransformC _tc, Mesh[] _meshes, Camera _camera, Material _material, bool _destroyMeshes, bool _recalcNormals = false, bool _optimize = false)
	{
		return CreatePrefabFromMeshArray(_tc, _meshes, _camera.gameObject.layer, _material, _destroyMeshes, _recalcNormals, _optimize);
	}

	public static PrefabC CreatePrefabFromMeshArray(TransformC _tc, Mesh[] _meshes, int _cameraLayer, Material _material, bool _destroyMeshes, bool _recalcNormals = false, bool _optimize = false)
	{
		CombineInstance[] array = new CombineInstance[_meshes.Length];
		for (int i = 0; i < _meshes.Length; i++)
		{
			CombineInstance combineInstance = new CombineInstance
			{
				mesh = _meshes[i]
			};
			array[i] = combineInstance;
		}
		PrefabC prefabC = AddComponent(_tc, Vector3.zero);
		prefabC.p_mesh.CombineMeshes(array, true, false);
		prefabC.p_mesh.RecalculateBounds();
		if (_recalcNormals)
		{
			prefabC.p_mesh.RecalculateNormals();
		}
		if (_optimize)
		{
			prefabC.p_mesh.Optimize();
		}
		prefabC.p_gameObject.GetComponent<Renderer>().material = _material;
		prefabC.p_gameObject.layer = _cameraLayer;
		if (_destroyMeshes)
		{
			for (int j = 0; j < _meshes.Length; j++)
			{
				if (_meshes[j] != null)
				{
					Object.DestroyImmediate(_meshes[j]);
				}
			}
		}
		return prefabC;
	}

	public static UVRect GetLargestUVRect(float _texWidth, float _texHeight, float _targetWidth, float _targetHeight)
	{
		if (_texWidth == _targetWidth && _texHeight == _targetHeight)
		{
			return UVRect.Normal();
		}
		if (_texWidth <= _targetWidth && _texHeight <= _targetHeight)
		{
			float num = _texWidth / _targetWidth;
			float num2 = _texHeight / _targetHeight;
			if (num > num2)
			{
				float num3 = _targetHeight / _targetWidth / (_texHeight / _texWidth);
				return new UVRect(0f, (1f - num3) * 0.5f, 1f, num3);
			}
			float num4 = _texHeight / _texWidth / (_targetHeight / _targetWidth);
			return new UVRect((1f - num4) * 0.5f, 0f, num4, 1f);
		}
		if (_texWidth > _targetWidth && _texHeight > _targetHeight)
		{
			float num5 = _texWidth / _targetWidth;
			float num6 = _texHeight / _targetHeight;
			if (num5 > num6)
			{
				float num7 = _texHeight / _texWidth / (_targetHeight / _targetWidth);
				return new UVRect((1f - num7) * 0.5f, 0f, num7, 1f);
			}
			float num8 = _targetHeight / _targetWidth / (_texHeight / _texWidth);
			return new UVRect(0f, (1f - num8) * 0.5f, 1f, num8);
		}
		if (_texWidth <= _targetWidth && _texHeight > _targetHeight)
		{
			float num9 = _targetHeight / _targetWidth / (_texHeight / _texWidth);
			return new UVRect(0f, (1f - num9) * 0.5f, 1f, num9);
		}
		if (_texWidth > _targetWidth && _texHeight <= _targetHeight)
		{
			float num10 = _texHeight / _texWidth / (_targetHeight / _targetWidth);
			return new UVRect((1f - num10) * 0.5f, 0f, num10, 1f);
		}
		return UVRect.Normal();
	}
}
