using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class Debris
{
	public Entity m_entity;

	public ChipmunkBodyC m_body;

	public TransformC m_tc;

	public Debris(Vector3 _pos, GameObject _prefab, ucpShape _shape, string _particleEffectPath, Vector3 _particleOffSet, bool _calculateCenteroid, float _destroyAfterSecs = -1f, bool tween = false)
	{
		m_entity = EntityManager.AddEntity(new string[2] { "GTAG_AUTODESTROY", "GTAG_DEBRIS" });
		m_tc = TransformS.AddComponent(m_entity, "Debris-" + _prefab.name);
		Vector3 eulerAngles = _prefab.transform.eulerAngles;
		Vector2 offset = _shape.offset;
		if (!_calculateCenteroid)
		{
			_shape.offset = Vector2.zero;
		}
		TransformC transformC = TransformS.AddComponent(m_entity);
		TransformS.ParentComponent(transformC, m_tc, Vector3.zero);
		TransformS.SetTransform(m_tc, _pos, eulerAngles);
		PrefabC prefabC = null;
		if (_prefab != null)
		{
			prefabC = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, _prefab.transform.position.z) + (Vector3)offset * ((!_calculateCenteroid) ? (-1f) : 1f), _prefab, string.Empty);
		}
		if (_particleEffectPath != null)
		{
			PrefabS.AddComponent(transformC, _particleOffSet, ResourceManager.GetGameObject(_particleEffectPath));
		}
		if (_destroyAfterSecs > 0f)
		{
			TimerS.AddComponent(m_entity, "DebrisTimer", _destroyAfterSecs, 0f, true);
		}
		m_body = ChipmunkProS.AddDynamicBody(m_tc, _shape);
		if (tween)
		{
			TweenS.AddTransformTween(transformC, TweenedProperty.Position, TweenStyle.Linear, new Vector3(0f, 0f, 90f), 1f, 0f, true);
		}
	}

	public static bool CreateDebrisFromGO(Transform _parentTransform, Vector2 _linVel, float _angVel, bool _deleteGO = true, uint _group = 0, bool _calculateCenteroid = false, string _particleEffectPath = null, Vector3 _particleOffSet = default(Vector3), float _destroyTime = -1f, uint _layer = 1u)
	{
		return CreateDebrisFromGO(_parentTransform, Vector3.zero, _linVel, _angVel, _deleteGO, _group, _calculateCenteroid, _particleEffectPath, _particleOffSet, _destroyTime, _layer);
	}

	public static bool CreateDebrisFromGO(Transform _parentTransform, Vector3 _position, Vector2 _linVel, float _angVel, bool _deleteGO = true, uint _group = 0, bool _calculateCenteroid = false, string _particleEffectPath = null, Vector3 _particleOffSet = default(Vector3), float _destroyTime = -1f, uint _layer = 1u)
	{
		if (_parentTransform == null || _parentTransform.childCount == 0)
		{
			return false;
		}
		Transform transform = _parentTransform.Find(_parentTransform.name + "Collider");
		if (transform != null)
		{
			Vector2 vector = Vector2.zero;
			if (_calculateCenteroid)
			{
				Mesh mesh = transform.GetComponent<MeshFilter>().mesh;
				Vector2[] array = new Vector2[mesh.vertexCount];
				for (int i = 0; i < mesh.vertexCount; i++)
				{
					array[i] = new Vector2(mesh.vertices[i].x, mesh.vertices[i].y);
				}
				vector = ChipmunkProWrapper.ucpCenteroidForPoly(array.Length, array);
			}
			ucpPolyShape ucpPolyShape2 = ChipmunkProS.GeneratePolyShapeFromGameObject(transform.gameObject, -vector, 1f, 0.5f, 0.5f, (ucpCollisionType)7, _layer);
			ucpPolyShape2.group = _group;
			ucpPolyShape2.mass = 1f + ucpPolyShape2.area * 0.033f;
			Vector3 pos = ((!(_position == Vector3.zero)) ? _position : new Vector3(transform.transform.position.x, transform.transform.position.y));
			Debris debris = new Debris(pos, _parentTransform.gameObject, ucpPolyShape2, _particleEffectPath, _particleOffSet, _calculateCenteroid, _destroyTime);
			ChipmunkProWrapper.ucpBodySetVel(debris.m_body.body, _linVel);
			ChipmunkProWrapper.ucpBodySetAngVel(debris.m_body.body, _angVel);
			if (_deleteGO)
			{
				UnityEngine.Object.DestroyImmediate(_parentTransform.gameObject);
			}
			return true;
		}
		return false;
	}

	public static Debris ConvertGameObjectToDebrisWithZTween(Transform _objectTransform, Vector3 _position, Vector2 _linVel, float _angVel, [Optional][DefaultParameterValue(true)] bool _deleteGO, uint _group = 0, bool _calculateCenteroid = false, string _particleEffectPath = null, Vector3 _particleOffSet = default(Vector3), float _destroyTime = -1f, uint _layer = 1u, bool _tween = true)
	{
		if (_objectTransform == null || _objectTransform.gameObject.GetComponent<MeshFilter>() == null)
		{
			if (_objectTransform.childCount == 0 || _objectTransform.GetChild(0).gameObject.GetComponent<MeshFilter>() == null)
			{
				return null;
			}
			_objectTransform = _objectTransform.GetChild(0);
		}
		Vector2 vector = Vector2.zero;
		if (_calculateCenteroid)
		{
			Mesh mesh = _objectTransform.GetComponent<MeshFilter>().mesh;
			Vector2[] array = new Vector2[mesh.vertexCount];
			for (int i = 0; i < mesh.vertexCount; i++)
			{
				array[i] = new Vector2(mesh.vertices[i].x, mesh.vertices[i].y);
			}
			vector = ChipmunkProWrapper.ucpCenteroidForPoly(array.Length, array);
		}
		vector = new Vector2(_objectTransform.transform.localPosition.x - _objectTransform.parent.transform.localPosition.x, _objectTransform.transform.localPosition.y - _objectTransform.parent.transform.localPosition.y);
		ucpPolyShape ucpPolyShape2 = ChipmunkProS.GeneratePolyShapeFromGameObject(_objectTransform.gameObject, -vector, 1f, 0.5f, 0.5f, (ucpCollisionType)7, _layer);
		ucpPolyShape2.group = _group;
		ucpPolyShape2.mass = 1f + ucpPolyShape2.area * 0.033f;
		Vector3 pos = ((!(_position == Vector3.zero)) ? _position : new Vector3(_objectTransform.transform.position.x, _objectTransform.transform.position.y));
		Debris debris = new Debris(pos, _objectTransform.gameObject, ucpPolyShape2, _particleEffectPath, _particleOffSet, true, _destroyTime, _tween);
		ChipmunkProWrapper.ucpBodySetVel(debris.m_body.body, _linVel);
		ChipmunkProWrapper.ucpBodySetAngVel(debris.m_body.body, _angVel);
		if (_deleteGO)
		{
			UnityEngine.Object.DestroyImmediate(_objectTransform.gameObject);
		}
		return debris;
	}

	public static void CreateDebrisFromChildren(Transform _parentTransform, Vector2 _linVel, Vector2 _linVelRandom, float _angVelRandom, bool _deleteGO = true, uint _group = 0)
	{
		for (int i = 0; i < _parentTransform.childCount; i++)
		{
			Vector2 linVel = _linVel + new Vector2(UnityEngine.Random.Range(0f - _linVelRandom.x, _linVelRandom.x), UnityEngine.Random.Range(0f - _linVelRandom.y, _linVelRandom.y));
			float angVel = UnityEngine.Random.Range(0f - _angVelRandom, _angVelRandom) * ((float)Math.PI / 180f);
			CreateDebrisFromGO(_parentTransform.transform.GetChild(i), linVel, angVel, false, _group, false, null, default(Vector3), -1f);
		}
		if (_deleteGO)
		{
			while (_parentTransform.transform.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(_parentTransform.transform.GetChild(0).gameObject);
			}
		}
	}
}
