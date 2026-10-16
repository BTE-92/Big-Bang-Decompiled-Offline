using System;
using System.Collections.Generic;
using UnityEngine;

public static class CameraS
{
	public static bool DEBUG = false;

	public static DynamicArray<CameraLayer> m_cameraLayers = new DynamicArray<CameraLayer>(16, 0f, 0f, 1f);

	public static List<Camera> m_cameras;

	private static bool m_debugDraw = false;

	public static Camera m_mainCamera;

	public static float m_mainCameraDistanceMultipler;

	public static float m_mainCameraFov = 40f;

	public static float m_mainCameraDistance = 500f;

	public static Vector3 m_mainCameraRotation = Vector3.zero;

	public static Vector3 m_mainCameraPosition = Vector3.zero;

	public static float m_mainCameraNearClip = 30f;

	public static float m_mainCameraFarClip = 80000f;

	public static cpBB m_mainCameraFrame;

	public static RenderTexture m_mainCameraRenderTexture;

	public static bool m_mainCameraRenderTextureDirty;

	public static int m_mainCameraLayer = 8;

	public static int m_mainCameraCullingMask = 1792;

	public static TransformC m_mainCameraTC;

	public static TransformC m_mainCameraRotateTC;

	public static float m_mainCamShakeTime;

	public static float m_mainCamShakeAmount;

	public static int m_mainCamShakeTickInterval;

	private static Vector3 m_mainCamCurrentShakeAmount;

	public static Camera m_uiCamera;

	public static float m_uiCameraNearClip = 1f;

	public static float m_uiCameraFarClip = 1000f;

	public static int m_uiCameraLayer = 15;

	public static int m_otherCamerasStartLayer = 16;

	public static DynamicArray<CameraTargetC> m_cameraTargetComponents;

	public static Vector3 m_combinedTargetsVelocity;

	public static TransformC m_debugTC;

	public static TransformC m_debugTC2;

	public static int m_screenWidth = 0;

	public static int m_screenHeight = 0;

	public static bool m_updateComponents = true;

	private static Entity cameraRootEntity;

	private static PrefabC m_finalTargetPrefab;

	public static float m_zoomMultipler = 1f;

	public static float m_zoomNeutralizer = 0.98f;

	public static void Initialize()
	{
		m_screenWidth = Screen.width;
		m_screenHeight = Screen.height;
		Entity entity = EntityManager.AddEntity();
		entity.m_persistent = true;
		cameraRootEntity = entity;
		if (m_debugDraw)
		{
			m_debugTC = TransformS.AddComponent(entity, "Main Camera Debug Transform");
			m_debugTC2 = TransformS.AddComponent(entity, "Main Camera Debug Transform");
		}
		m_mainCameraTC = TransformS.AddComponent(entity, "Main Camera Transform");
		m_mainCameraRotateTC = TransformS.AddComponent(entity, "Main Camera Rotate Transform");
		m_mainCamera = Camera.main;
		m_mainCamera.targetTexture = null;
		m_mainCamera.cullingMask = m_mainCameraCullingMask;
		m_mainCamera.gameObject.layer = m_mainCameraLayer;
		m_mainCamera.fieldOfView = m_mainCameraFov;
		m_mainCamera.farClipPlane = m_mainCameraFarClip;
		m_mainCamera.nearClipPlane = m_mainCameraNearClip;
		m_mainCamera.fieldOfView = m_mainCameraFov;
		m_mainCamera.backgroundColor = Color.black;
		for (int i = 0; i < m_mainCamera.transform.childCount; i++)
		{
			Transform child = m_mainCamera.transform.GetChild(i);
			child.gameObject.layer = m_mainCamera.gameObject.layer;
		}
		m_mainCamera.transform.parent = m_mainCameraTC.transform;
		m_mainCameraTC.transform.parent = m_mainCameraRotateTC.transform;
		m_mainCamera.transform.localPosition = Vector3.forward * (0f - m_mainCameraDistance);
		m_mainCamera.transform.localRotation = Quaternion.identity;
		m_mainCameraRotateTC.transform.localPosition = Vector3.zero;
		GameObject gameObject = new GameObject("UI Camera");
		m_uiCamera = gameObject.AddComponent<Camera>();
		m_uiCamera.orthographic = true;
		m_uiCamera.orthographicSize = (float)Screen.height * 0.5f;
		m_uiCamera.depth = 1f;
		m_uiCamera.cullingMask = 1 << m_uiCameraLayer;
		m_uiCamera.gameObject.layer = m_uiCameraLayer;
		m_uiCamera.nearClipPlane = m_uiCameraNearClip;
		m_uiCamera.farClipPlane = m_uiCameraFarClip;
		m_uiCamera.gameObject.transform.position = new Vector3(0f, 0f, -500f);
		m_uiCamera.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		m_uiCamera.clearFlags = CameraClearFlags.Depth;
		m_cameraTargetComponents = new DynamicArray<CameraTargetC>();
		m_mainCameraFrame = ChipmunkProWrapper.ucpBBNew((float)Screen.width * -0.5f, (float)Screen.height * -0.5f, (float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
		m_cameras = new List<Camera>();
		m_cameras.Add(m_mainCamera);
		m_cameras.Add(m_uiCamera);
		m_mainCameraRenderTextureDirty = false;
	}

	public static void ShakeMainCamera(float _amount, float _duration, int _tickInterval = 2)
	{
		m_mainCamShakeAmount = _amount;
		m_mainCamShakeTime = _duration;
		m_mainCamShakeTickInterval = _tickInterval;
	}

	public static void SnapMainCameraFrame()
	{
		cpBB mainCameraFrame = CalculateMergedTargetBB();
		if (!mainCameraFrame.isNull())
		{
			m_mainCameraFrame = mainCameraFrame;
			m_mainCameraRotation = Vector3.zero;
		}
		m_mainCamShakeTime = 0f;
	}

	public static void SnapMainCameraPos(Vector2 _position)
	{
		m_mainCameraPosition = _position;
		m_mainCameraRotateTC.transform.position = m_mainCameraPosition;
		m_mainCameraTC.transform.localPosition = Vector3.zero;
		m_mainCameraFrame = CenterBB(_position, m_mainCameraFrame);
		m_mainCameraRotation = Vector3.zero;
	}

	public static void RenderMainCameraToTexture(Shader _replacementShader)
	{
		Color backgroundColor = m_mainCamera.backgroundColor;
		m_mainCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
		m_mainCamera.targetTexture = m_mainCameraRenderTexture;
		m_mainCamera.RenderWithShader(_replacementShader, "RenderType");
		m_mainCamera.targetTexture = null;
		m_mainCamera.backgroundColor = backgroundColor;
		m_mainCameraRenderTextureDirty = true;
	}

	public static void ClearMainCameraRenderTexture()
	{
		if (m_mainCameraRenderTextureDirty)
		{
			m_mainCameraRenderTextureDirty = false;
			Color backgroundColor = m_mainCamera.backgroundColor;
			int cullingMask = m_mainCamera.cullingMask;
			m_mainCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
			m_mainCamera.cullingMask = 0;
			m_mainCamera.targetTexture = m_mainCameraRenderTexture;
			m_mainCamera.Render();
			m_mainCamera.targetTexture = null;
			m_mainCamera.backgroundColor = backgroundColor;
			m_mainCamera.cullingMask = cullingMask;
		}
	}

	public static Camera AddCamera(string _name, bool _ortographic, CameraClearFlags _clearFlags = CameraClearFlags.Depth)
	{
		GameObject gameObject = new GameObject(_name);
		Camera camera = gameObject.AddComponent<Camera>();
		camera.depth = 1f;
		camera.gameObject.transform.position = new Vector3(0f, 0f, -500f);
		camera.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		camera.clearFlags = _clearFlags;
		CameraLayer cameraLayer = m_cameraLayers.AddItem();
		int num = m_otherCamerasStartLayer + cameraLayer.m_index;
		if (_ortographic)
		{
			camera.orthographic = _ortographic;
			camera.orthographicSize = (float)Screen.height * 0.5f;
			camera.cullingMask = 1 << num;
			camera.gameObject.layer = num;
			camera.nearClipPlane = m_uiCameraNearClip;
			camera.farClipPlane = m_uiCameraFarClip;
			m_cameras.Add(camera);
		}
		else
		{
			camera.orthographic = false;
			camera.cullingMask = 1 << num;
			camera.gameObject.layer = num;
			camera.nearClipPlane = m_mainCameraNearClip;
			camera.farClipPlane = m_mainCameraFarClip;
			for (int i = 0; i < m_cameras.Count; i++)
			{
				if (m_cameras[i] == m_uiCamera)
				{
					m_cameras.Insert(i, camera);
					break;
				}
			}
		}
		for (int j = 0; j < m_cameras.Count; j++)
		{
			m_cameras[j].depth = j;
		}
		return camera;
	}

	public static void BringToFront(Camera _camera, bool _updateCameraDepth = true)
	{
		if (!m_cameras.Remove(_camera))
		{
			return;
		}
		m_cameras.Add(_camera);
		if (_updateCameraDepth)
		{
			for (int i = 0; i < m_cameras.Count; i++)
			{
				m_cameras[i].depth = i;
			}
		}
	}

	public static bool MoveToBehindOther(Camera _camera, Camera _otherCamera)
	{
		if (m_cameras.Contains(_otherCamera) && m_cameras.Contains(_camera))
		{
			m_cameras.Remove(_camera);
			m_cameras.Insert(m_cameras.IndexOf(_otherCamera), _camera);
			for (int i = 0; i < m_cameras.Count; i++)
			{
				m_cameras[i].depth = i;
			}
			return true;
		}
		return false;
	}

	public static void RemoveCamera(Camera _camera)
	{
		if (_camera != null && _camera != m_uiCamera && _camera != m_mainCamera)
		{
			m_cameraLayers.RemoveItem(_camera.gameObject.layer - m_otherCamerasStartLayer);
			m_cameras.Remove(_camera);
			UnityEngine.Object.DestroyImmediate(_camera.gameObject);
		}
	}

	public static CameraTargetC AddTargetComponent(TransformC _tc, float _width, float _height, Vector2 _offset)
	{
		CameraTargetC cameraTargetC = m_cameraTargetComponents.AddItem();
		cameraTargetC.targetTC = _tc;
		cameraTargetC.frameTC = TransformS.AddComponent(cameraRootEntity, "Camera Target Transform", _tc.transform.position);
		cameraTargetC.frameTC.forceRotation = true;
		cameraTargetC.frameTC.forceScale = true;
		cameraTargetC.offset = _offset;
		cameraTargetC.lastFrameTCPos = _tc.transform.position;
		cameraTargetC.lastTargetTCPos = _tc.transform.position;
		cameraTargetC.framePrefab = null;
		cameraTargetC.translatedCenter = Vector2.zero;
		cameraTargetC.framePeek = Vector2.zero;
		SetTargetBB(cameraTargetC, _width, _height);
		EntityManager.AddComponentToEntity(_tc.p_entity, cameraTargetC);
		return cameraTargetC;
	}

	public static void ResetFramePosition(CameraTargetC _ct)
	{
		_ct.frameTC.transform.position = _ct.targetTC.transform.position;
		_ct.lastFrameTCPos = _ct.frameTC.transform.position;
		_ct.lastTargetTCPos = _ct.targetTC.transform.position;
		_ct.translatedCenter = Vector2.zero;
	}

	public static void SetTargetBB(CameraTargetC _ct, float _width, float _height)
	{
		if (DEBUG)
		{
			if (_ct.framePrefab != null)
			{
				PrefabS.RemoveComponent(_ct.framePrefab);
			}
			_ct.framePrefab = PrefabS.CreateRect(_ct.frameTC, (Vector3)_ct.offset + new Vector3(0f, 0f, 2.5f), _width, _height, new Color(0f, 1f, 0f, 0.2f), ResourceManager.GetMaterial(RESOURCE.DebugMaterial_Material), m_mainCamera);
		}
		_ct.targetFrame = ChipmunkProWrapper.ucpBBNew(_width * -0.5f, _height * -0.5f, _width * 0.5f, _height * 0.5f);
	}

	public static void RemoveAllTargetComponents()
	{
		while (m_cameraTargetComponents.m_aliveCount > 0)
		{
			CameraTargetC c = m_cameraTargetComponents.m_array[m_cameraTargetComponents.m_aliveIndices[0]];
			RemoveTargetComponent(c);
		}
	}

	public static void RemoveTargetComponent(CameraTargetC _c)
	{
		if (_c.framePrefab != null)
		{
			PrefabS.RemoveComponent(_c.framePrefab);
			_c.framePrefab = null;
		}
		if (_c.frameTC != null)
		{
			TransformS.RemoveComponent(_c.frameTC);
			_c.frameTC = null;
		}
		EntityManager.RemoveComponentFromEntity(_c);
		m_cameraTargetComponents.RemoveItem(_c);
	}

	public static cpBB TransformBB(cpBB _bb, Vector2 _pos, float _scale)
	{
		_bb.l = _pos.x + _bb.l * _scale;
		_bb.r = _pos.x + _bb.r * _scale;
		_bb.b = _pos.y + _bb.b * _scale;
		_bb.t = _pos.y + _bb.t * _scale;
		return _bb;
	}

	public static cpBB CenterBB(Vector2 _pos, cpBB _bb)
	{
		cpBB result = default(cpBB);
		float num = (_bb.r - _bb.l) * 0.5f;
		float num2 = (_bb.t - _bb.b) * 0.5f;
		result.l = _pos.x - num;
		result.r = _pos.x + num;
		result.t = _pos.y + num2;
		result.b = _pos.y - num2;
		return result;
	}

	private static cpBB ExpandBBToDirection(cpBB _bb, float _x, float _y)
	{
		if (_x < 0f)
		{
			_bb.l += _x;
		}
		else if (_x > 0f)
		{
			_bb.r += _x;
		}
		if (_y < 0f)
		{
			_bb.b += _y;
		}
		else if (_y > 0f)
		{
			_bb.t += _y;
		}
		return _bb;
	}

	private static cpBB ScaleBB(cpBB _bb, float _scale)
	{
		float num = (_bb.r - _bb.l) * 0.5f;
		float num2 = (_bb.t - _bb.b) * 0.5f;
		Vector2 vector = new Vector2(_bb.l + num, _bb.b + num2);
		_bb.l = vector.x - num * _scale;
		_bb.r = vector.x + num * _scale;
		_bb.b = vector.y - num2 * _scale;
		_bb.t = vector.y + num2 * _scale;
		return _bb;
	}

	private static float GetBBWidth(cpBB _bb)
	{
		return _bb.r - _bb.l;
	}

	private static float GetBBHeight(cpBB _bb)
	{
		return _bb.t - _bb.b;
	}

	private static Vector2 GetBBCenter(cpBB _bb)
	{
		float num = (_bb.r - _bb.l) * 0.5f;
		float num2 = (_bb.t - _bb.b) * 0.5f;
		return new Vector2(_bb.l + num, _bb.b + num2);
	}

	private static cpBB CalculateMergedTargetBB()
	{
		int num = 0;
		int num2 = 0;
		cpBB cpBB2 = default(cpBB);
		m_combinedTargetsVelocity = Vector2.zero;
		Vector2 primaryTargetsCenter = GetPrimaryTargetsCenter();
		int aliveCount = m_cameraTargetComponents.m_aliveCount;
		for (int i = 0; i < aliveCount; i++)
		{
			CameraTargetC cameraTargetC = m_cameraTargetComponents.m_array[m_cameraTargetComponents.m_aliveIndices[i]];
			bool flag = true;
			if (cameraTargetC.activeRadius < float.MaxValue)
			{
				float num3 = 0f;
				float magnitude = (primaryTargetsCenter - (Vector2)cameraTargetC.frameTC.transform.position).magnitude;
				if (magnitude > cameraTargetC.activeRadius)
				{
					cameraTargetC.secondaryTargetTransMult += 1f / 60f;
				}
				else
				{
					cameraTargetC.secondaryTargetTransMult -= 1f / 60f;
					num3 = ToolBox.getPositionBetween(magnitude, cameraTargetC.activeRadius * 0.75f, cameraTargetC.activeRadius);
					cameraTargetC.secondaryTargetTransMult = Mathf.Min(cameraTargetC.secondaryTargetTransMult, num3);
				}
				cameraTargetC.secondaryTargetTransMult = ToolBox.limitBetween(cameraTargetC.secondaryTargetTransMult, 0f, 1f);
				if (cameraTargetC.secondaryTargetTransMult == 1f)
				{
					flag = false;
					cameraTargetC.frameTC.transform.position = cameraTargetC.targetTC.transform.position;
					cameraTargetC.lastTargetTCPos = cameraTargetC.targetTC.transform.position;
					cameraTargetC.lastFrameTCPos = cameraTargetC.targetTC.transform.position;
				}
			}
			if (!cameraTargetC.m_active || !flag)
			{
				continue;
			}
			if (cameraTargetC.frameSlopRadiusMinMax != Vector2.zero)
			{
				float x = cameraTargetC.frameSlopRadiusMinMax.x;
				float y = cameraTargetC.frameSlopRadiusMinMax.y;
				Vector2 vector = cameraTargetC.frameTC.transform.position - cameraTargetC.targetTC.transform.position;
				float magnitude2 = vector.magnitude;
				if (magnitude2 > x)
				{
					float positionBetween = ToolBox.getPositionBetween(magnitude2, x, y);
					TransformS.Move(cameraTargetC.frameTC, -vector.normalized * positionBetween * (magnitude2 - x));
				}
			}
			else
			{
				TransformS.SetPosition(cameraTargetC.frameTC, cameraTargetC.targetTC.transform.position);
			}
			Vector3 vector2 = Vector3.zero;
			if (cameraTargetC.activeRadius < float.MaxValue && primaryTargetsCenter != Vector2.zero)
			{
				vector2 = (primaryTargetsCenter - (Vector2)cameraTargetC.frameTC.transform.position) * cameraTargetC.secondaryTargetTransMult;
			}
			cpBB cpBB3 = TransformBB(cameraTargetC.targetFrame, cameraTargetC.frameTC.transform.position + (Vector3)cameraTargetC.offset + vector2, 1f);
			Vector3 vector3 = cameraTargetC.frameTC.transform.position - cameraTargetC.lastFrameTCPos;
			Vector3 vector4 = cameraTargetC.targetTC.transform.position - cameraTargetC.lastTargetTCPos;
			if (cameraTargetC.frameGrowVelocityMultiplier != Vector2.zero)
			{
				Vector2 vector5 = new Vector2(vector3.x * cameraTargetC.frameGrowVelocityMultiplier.x, vector3.y * cameraTargetC.frameGrowVelocityMultiplier.y);
				cpBB3 = ExpandBBToDirection(cpBB3, vector5.x, vector5.y);
			}
			if (cameraTargetC.framePosVelocityMultiplier != Vector2.zero)
			{
				Vector2 vector6 = new Vector2(vector3.x * cameraTargetC.framePosVelocityMultiplier.x, vector3.y * cameraTargetC.framePosVelocityMultiplier.y);
				cpBB3 = TransformBB(cpBB3, new Vector3(vector6.x, vector6.y, 0f), 1f);
			}
			if (cameraTargetC.framePeekShiftMultiplier > 0f)
			{
				cameraTargetC.framePeek += (Vector2)vector4 * cameraTargetC.framePeekShiftMultiplier;
				if (Mathf.Abs(cameraTargetC.framePeek.x) > cameraTargetC.framePeekShiftMax.x)
				{
					cameraTargetC.framePeek.x = cameraTargetC.framePeekShiftMax.x * Mathf.Sign(cameraTargetC.framePeek.x);
				}
				if (Mathf.Abs(cameraTargetC.framePeek.y) > cameraTargetC.framePeekShiftMax.y)
				{
					cameraTargetC.framePeek.y = cameraTargetC.framePeekShiftMax.y * Mathf.Sign(cameraTargetC.framePeek.y);
				}
				cpBB3 = TransformBB(cpBB3, cameraTargetC.framePeek, 1f);
			}
			if (cameraTargetC.frameScaleVelocityMultiplier > 0f)
			{
				cpBB3 = ScaleBB(cpBB3, 1f + vector3.magnitude * cameraTargetC.frameScaleVelocityMultiplier);
			}
			cpBB3.b = Mathf.Max(cpBB3.b, cameraTargetC.frameWorldBounds.b);
			cpBB3.t = Mathf.Min(cpBB3.t, cameraTargetC.frameWorldBounds.t);
			cpBB3.l = Mathf.Max(cpBB3.l, cameraTargetC.frameWorldBounds.l);
			cpBB3.r = Mathf.Min(cpBB3.r, cameraTargetC.frameWorldBounds.r);
			cameraTargetC.translatedCenter = GetBBCenter(cpBB3);
			cpBB2 = ((num != 0) ? ChipmunkProWrapper.ucpBBMerge(cpBB2, cpBB3) : cpBB3);
			if (cameraTargetC.activeRadius == float.MaxValue)
			{
				m_combinedTargetsVelocity += vector3;
				num2++;
			}
			cameraTargetC.lastFrameTCPos = cameraTargetC.frameTC.transform.position;
			cameraTargetC.lastTargetTCPos = cameraTargetC.targetTC.transform.position;
			num++;
		}
		if (num2 > 0)
		{
			m_combinedTargetsVelocity /= (float)num2;
		}
		return cpBB2;
	}

	public static bool isPrimaryTarget(CameraTargetC _target)
	{
		return _target.activeRadius == float.MaxValue;
	}

	public static Vector2 GetPrimaryTargetsCenter()
	{
		Vector2 zero = Vector2.zero;
		int num = 0;
		int aliveCount = m_cameraTargetComponents.m_aliveCount;
		for (int i = 0; i < aliveCount; i++)
		{
			CameraTargetC cameraTargetC = m_cameraTargetComponents.m_array[m_cameraTargetComponents.m_aliveIndices[i]];
			if (cameraTargetC.m_active && cameraTargetC.activeRadius == float.MaxValue)
			{
				if (cameraTargetC.translatedCenter != Vector2.zero)
				{
					zero += cameraTargetC.translatedCenter;
				}
				else
				{
					zero += (Vector2)cameraTargetC.frameTC.transform.position;
				}
				num++;
			}
		}
		if (num > 0)
		{
			zero /= (float)num;
		}
		return zero;
	}

	private static float GetCameraInterpolationSpeed()
	{
		float num = 0f;
		int aliveCount = m_cameraTargetComponents.m_aliveCount;
		for (int i = 0; i < aliveCount; i++)
		{
			CameraTargetC cameraTargetC = m_cameraTargetComponents.m_array[m_cameraTargetComponents.m_aliveIndices[i]];
			if (cameraTargetC.m_active && cameraTargetC.interpolateSpeed > num)
			{
				num = cameraTargetC.interpolateSpeed;
			}
		}
		if (num == 0f)
		{
			num = 1f;
		}
		return num;
	}

	private static Vector2 GetCameraVelocityAngleChange()
	{
		Vector2 result = default(Vector2);
		int aliveCount = m_cameraTargetComponents.m_aliveCount;
		for (int i = 0; i < aliveCount; i++)
		{
			CameraTargetC cameraTargetC = m_cameraTargetComponents.m_array[m_cameraTargetComponents.m_aliveIndices[i]];
			if (cameraTargetC.m_active)
			{
				if (cameraTargetC.velAngleChangeMult.x > result.x)
				{
					result.x = cameraTargetC.velAngleChangeMult.x;
				}
				if (cameraTargetC.velAngleChangeMult.y > result.y)
				{
					result.y = cameraTargetC.velAngleChangeMult.y;
				}
			}
		}
		return result;
	}

	private static Vector2 GetCameraAngleLimit()
	{
		Vector2 result = new Vector2(999f, 999f);
		int aliveCount = m_cameraTargetComponents.m_aliveCount;
		for (int i = 0; i < aliveCount; i++)
		{
			CameraTargetC cameraTargetC = m_cameraTargetComponents.m_array[m_cameraTargetComponents.m_aliveIndices[i]];
			if (cameraTargetC.m_active)
			{
				if (cameraTargetC.angleLimits.x < result.x)
				{
					result.x = cameraTargetC.angleLimits.x;
				}
				if (cameraTargetC.angleLimits.y < result.y)
				{
					result.y = cameraTargetC.angleLimits.y;
				}
			}
		}
		return result;
	}

	private static cpBB InterpolateBBs(cpBB _from, cpBB _to, float _delta)
	{
		return new cpBB
		{
			b = _from.b + (_to.b - _from.b) * _delta,
			l = _from.l + (_to.l - _from.l) * _delta,
			r = _from.r + (_to.r - _from.r) * _delta,
			t = _from.t + (_to.t - _from.t) * _delta
		};
	}

	public static void Update()
	{
		if (Mathf.Abs(m_zoomMultipler) > 0.01f)
		{
			m_zoomMultipler *= m_zoomNeutralizer;
		}
		else
		{
			m_zoomMultipler = 0f;
		}
		Vector3 vector = -m_mainCamera.ScreenToWorldPoint(new Vector3((float)Screen.width * 0.5f + 1f, (float)Screen.height * 0.5f, m_mainCamera.transform.position.z));
		m_mainCameraDistanceMultipler = 1f / (vector + m_mainCamera.transform.position).x;
		if (m_finalTargetPrefab != null)
		{
			PrefabS.RemoveComponent(m_finalTargetPrefab);
			m_finalTargetPrefab = null;
		}
		if (m_updateComponents)
		{
			cpBB cpBB2 = CalculateMergedTargetBB();
			if (!cpBB2.isNull())
			{
				if (DEBUG)
				{
					m_finalTargetPrefab = PrefabS.CreateRect(m_mainCameraRotateTC, Vector3.zero, GetBBWidth(cpBB2), GetBBHeight(cpBB2), new Color(1f, 0f, 0f, 0.2f), ResourceManager.GetMaterial(RESOURCE.DebugMaterial_Material), m_mainCamera);
				}
				float cameraInterpolationSpeed = GetCameraInterpolationSpeed();
				m_mainCameraFrame = InterpolateBBs(m_mainCameraFrame, cpBB2, cameraInterpolationSpeed);
				float num = (m_mainCameraFrame.r - m_mainCameraFrame.l) * 0.5f;
				float num2 = (m_mainCameraFrame.t - m_mainCameraFrame.b) * 0.5f;
				m_mainCameraPosition = new Vector3(m_mainCameraFrame.l + num, m_mainCameraFrame.b + num2, 0f);
				float num3 = (float)Screen.width / (float)Screen.height;
				float a = (0f - 1f / Mathf.Tan(m_mainCamera.fieldOfView * 0.5f * ((float)Math.PI / 180f))) * num2;
				float b = (0f - 1f / Mathf.Tan(m_mainCamera.fieldOfView * num3 * 0.5f * ((float)Math.PI / 180f))) * num;
				TransformS.SetPosition(m_mainCameraRotateTC, m_mainCameraPosition);
				m_mainCamera.transform.localPosition = Vector3.forward * Mathf.Min(a, b) * (m_zoomMultipler + 1f);
				Vector2 cameraVelocityAngleChange = GetCameraVelocityAngleChange();
				if (cameraVelocityAngleChange != Vector2.zero)
				{
					Vector2 cameraAngleLimit = GetCameraAngleLimit();
					Vector3 zero = Vector3.zero;
					zero.y = cameraVelocityAngleChange.y * m_combinedTargetsVelocity.x;
					zero.y = ToolBox.limitBetween(zero.y, 0f - cameraAngleLimit.y, cameraAngleLimit.y);
					zero.x = (0f - cameraVelocityAngleChange.x) * m_combinedTargetsVelocity.y;
					zero.x = ToolBox.limitBetween(zero.x, 0f - cameraAngleLimit.x, cameraAngleLimit.x);
					m_mainCameraRotation += (zero - m_mainCameraRotation) * cameraInterpolationSpeed;
					TransformS.SetRotation(m_mainCameraRotateTC, m_mainCameraRotation);
				}
				else
				{
					m_mainCameraRotation += (Vector3.zero - m_mainCameraRotation) * cameraInterpolationSpeed;
					TransformS.SetRotation(m_mainCameraRotateTC, m_mainCameraRotation);
				}
				if (m_mainCamShakeTime > 0f)
				{
					if (Main.m_gameTicks % m_mainCamShakeTickInterval == 0)
					{
						m_mainCamCurrentShakeAmount = new Vector3(UnityEngine.Random.Range(0f - m_mainCamShakeAmount, m_mainCamShakeAmount), UnityEngine.Random.Range(0f - m_mainCamShakeAmount, m_mainCamShakeAmount), UnityEngine.Random.Range(0f - m_mainCamShakeAmount, 0f));
					}
					else
					{
						m_mainCamShakeAmount *= 0.8f;
					}
					m_mainCamera.transform.localPosition += m_mainCamCurrentShakeAmount;
					m_mainCamShakeTime -= Main.m_gameDeltaTime;
				}
			}
		}
		m_cameraTargetComponents.Update();
	}
}
