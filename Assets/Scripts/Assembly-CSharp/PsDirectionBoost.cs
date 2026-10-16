using System;
using System.Collections.Generic;
using UnityEngine;

public class PsDirectionBoost : Unit
{
	public const float COLLIDER_RADIUS = 20f;

	public const float TOTAL_VELOCITY = 1000f;

	public const float EXIT_OFFSET = 50f;

	public const int COOL_DOWN_TIME = 240;

	public const int TWEEN_TIME = 20;

	private PrefabC m_prefab;

	private ChipmunkBodyC m_body;

	private TransformC m_tc;

	private bool m_startBoost;

	private int m_coolDownTimer;

	private TransformC m_transformHelper;

	private Vector2 m_velocityAtStart;

	private Vector2 m_unitCenterPosition;

	private Unit m_unit;

	public PsDirectionBoost(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_graphElement.m_name = "DirectionBoost";
		m_graphElement.m_isCopyable = true;
		m_graphElement.m_isRemovable = true;
		m_graphElement.m_isRotateable = true;
		m_graphElement.m_isFlippable = false;
		m_tc = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(m_tc, _graphElement.m_position + new Vector3(0f, 0f, 0f), _graphElement.m_rotation);
		ucpShape shape = new ucpCircleShape(20f, Vector2.zero, 17895696u, 0f, 0f, 0f, (ucpCollisionType)10, true);
		m_body = ChipmunkProS.AddStaticBody(m_tc, shape);
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.DirectionBoostArrowPrefab_GameObject);
		m_prefab = PrefabS.AddComponent(m_tc, new Vector3(0f, 0f, 45f), gameObject);
		if (!m_minigame.m_editing)
		{
			ChipmunkProS.AddCollisionHandler(m_body, CollisionHandler, (ucpCollisionType)10, (ucpCollisionType)4, true, false, false);
			ChipmunkProS.AddCollisionHandler(m_body, CollisionHandler, (ucpCollisionType)10, (ucpCollisionType)3, true, false, false);
		}
		CreateEditorTouchArea(20f, 20f);
	}

	public override void CreateEditorTouchArea(float _width, float _height, TransformC _parentTC = null)
	{
		if (m_minigame.m_editing)
		{
			CreateGraphElementTouchArea(_width, _parentTC);
		}
	}

	protected void CollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB];
		UnitC unitC = chipmunkBodyC.customComponent as UnitC;
		ucpBodyType ucpBodyType2 = ChipmunkProWrapper.ucpBodyGetType(chipmunkBodyC.body);
		if (unitC == null || unitC.m_unit == null || ucpBodyType2 != ucpBodyType.DYNAMIC)
		{
			return;
		}
		switch (_phase)
		{
		case ucpCollisionPhase.Begin:
			if (m_coolDownTimer <= 0)
			{
				m_unit = unitC.m_unit;
				m_startBoost = true;
			}
			break;
		}
	}

	public override void Update()
	{
		if (m_minigame.m_editing || m_unit == null)
		{
			return;
		}
		if (m_startBoost && m_coolDownTimer <= 0)
		{
			SetupUnit(m_unit);
		}
		else
		{
			if (m_coolDownTimer <= 0)
			{
				return;
			}
			m_coolDownTimer--;
			int num = 240 - m_coolDownTimer;
			if (num > 20)
			{
				return;
			}
			float t = (float)num / 20f;
			Vector2 bezierPoint = ToolBox.GetBezierPoint(t, m_unitCenterPosition, m_tc.transform.position, m_tc.transform.position, m_tc.transform.position + m_tc.transform.right * 50f);
			TransformS.SetGlobalPosition(m_transformHelper, bezierPoint);
			if (num == 20)
			{
				EntityManager.SetActivityOfEntity(m_unit.m_entity, true);
				TransformS.RemoveComponent(m_transformHelper);
				m_transformHelper = null;
				List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.ChipmunkBody, m_unit.m_entity);
				Vector3 vector = new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), 0f);
				for (int i = 0; i < componentsByEntity.Count; i++)
				{
					ChipmunkBodyC chipmunkBodyC = componentsByEntity[i] as ChipmunkBodyC;
					ChipmunkProWrapper.ucpBodySetPos(chipmunkBodyC.body, chipmunkBodyC.TC.transform.position + vector);
					Vector2 vector2 = ChipmunkProWrapper.ucpBodyGetVel(chipmunkBodyC.body);
					ChipmunkProWrapper.ucpBodySetVel(chipmunkBodyC.body, (Vector2)m_tc.transform.right * 1000f);
				}
			}
		}
	}

	private void SetupUnit(Unit _unit)
	{
		m_startBoost = false;
		m_coolDownTimer = 240;
		m_unitCenterPosition = Vector2.zero;
		m_velocityAtStart = Vector2.zero;
		List<IComponent> unitBodyList = _unit.GetUnitBodyList();
		for (int i = 0; i < unitBodyList.Count; i++)
		{
			IntPtr body = (unitBodyList[i] as ChipmunkBodyC).body;
			m_unitCenterPosition += ChipmunkProWrapper.ucpBodyGetPos(body);
			m_velocityAtStart += ChipmunkProWrapper.ucpBodyGetVel(body);
			ChipmunkProWrapper.ucpBodyResetForces(body);
			ChipmunkProWrapper.ucpBodySetVel(body, Vector2.zero);
		}
		m_unitCenterPosition /= (float)unitBodyList.Count;
		m_velocityAtStart /= (float)unitBodyList.Count;
		EntityManager.SetActivityOfEntity(_unit.m_entity, false, false, true, false);
		m_transformHelper = TransformS.AddComponent(m_entity, m_unitCenterPosition);
		for (int j = 0; j < _unit.m_entity.m_components.Count; j++)
		{
			if (_unit.m_entity.m_components[j].m_componentType == ComponentType.Transform)
			{
				TransformC transformC = _unit.m_entity.m_components[j] as TransformC;
				if (transformC.parent == null)
				{
					TransformS.ParentComponent(transformC, m_transformHelper);
				}
			}
			else if (_unit.m_entity.m_components[j].m_componentType == ComponentType.CameraTarget)
			{
				CameraTargetC cameraTargetC = _unit.m_entity.m_components[j] as CameraTargetC;
				cameraTargetC.m_active = true;
				cameraTargetC.frameTC.m_active = true;
			}
		}
	}
}
