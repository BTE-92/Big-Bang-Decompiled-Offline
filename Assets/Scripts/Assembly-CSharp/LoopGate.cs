using System;
using UnityEngine;

public class LoopGate : Unit
{
	private const float INITIAL_RATE = 140f;

	private ChipmunkBodyC m_concreteBody;

	private ChipmunkBodyC m_cmb;

	private ChipmunkBodyC m_triggerBody;

	private ChipmunkConstraintC m_motor;

	private bool m_active;

	private bool m_separated;

	private float m_separateTimer;

	private float m_currentRate;

	public LoopGate(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_graphElement.m_isFlippable = true;
		m_graphElement.m_isRotateable = true;
		GameObject gameObject = ResourceManager.GetGameObject("Units/LoopGatePrefab");
		TransformC transformC = TransformS.AddComponent(m_entity, m_graphElement.m_name);
		TransformS.SetTransform(transformC, m_graphElement.m_position, m_graphElement.m_rotation);
		GameObject gameObject2 = gameObject.transform.Find("concrete").gameObject;
		PrefabS.AddComponent(transformC, Vector3.zero, gameObject2);
		ucpPolyShape[] shapes = ChipmunkProS.GeneratePolyShapesFromChildren(gameObject.transform.Find("Colliders").gameObject, Vector2.zero, 100f, 0.1f, 0.9f, (ucpCollisionType)4, 1118208u);
		m_concreteBody = ChipmunkProS.AddKinematicBody(transformC, shapes);
		TransformC transformC2 = TransformS.AddComponent(m_entity, "LoopGatePlank");
		TransformS.SetTransform(transformC2, m_graphElement.m_position, m_graphElement.m_rotation);
		GameObject gameObject3 = gameObject.transform.Find("plank").gameObject;
		PrefabS.AddComponent(transformC2, Vector3.up * 5f, gameObject3);
		ucpPolyShape shape = ChipmunkProS.GeneratePolyShapeFromGameObject(gameObject3.transform.Find("plankCollider").gameObject, Vector2.zero, 100f, 0.1f, 0.9f, (ucpCollisionType)4, 17895696u);
		m_cmb = ChipmunkProS.AddDynamicBody(transformC2, shape);
		TransformC transformC3 = TransformS.AddComponent(m_entity, "anchor");
		TransformS.SetTransform(transformC3, m_graphElement.m_position, m_graphElement.m_rotation);
		ChipmunkProS.AddPivotJoint(m_concreteBody, m_cmb, transformC3);
		ChipmunkProS.AddRotaryLimitJoint(m_concreteBody, m_cmb, transformC3, -0.61086524f, 0.61086524f);
		SetBodyRot();
		if (!m_minigame.m_editing)
		{
			ChipmunkProS.AddCollisionHandler(m_cmb, PlankCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)3, true, true, true);
		}
		m_motor = ChipmunkProS.AddSimpleMotor(m_concreteBody, m_cmb, transformC3, 140f * ((!m_graphElement.m_flipped) ? (-1f) : 1f), 7500000f);
		m_currentRate = 0f;
		m_active = false;
		m_separated = false;
		m_separateTimer = 0f;
		CreateEditorTouchArea();
	}

	public void SetBodyRot()
	{
		float num = m_graphElement.m_rotation.z + 35f;
		if (m_graphElement.m_flipped)
		{
			num -= 70f;
		}
		ChipmunkProWrapper.ucpBodySetAngle(m_cmb.body, num * ((float)Math.PI / 180f));
		TransformS.SetGlobalRotation(m_cmb.TC, Vector3.forward * num);
	}

	public override void Update()
	{
		base.Update();
		if (m_minigame.m_editing)
		{
			SetBodyRot();
		}
		else
		{
			if (m_minigame.m_editing || !m_minigame.m_gameStarted)
			{
				return;
			}
			if (m_separated)
			{
				m_separateTimer -= Main.m_gameDeltaTime;
				if (m_separateTimer <= 0f)
				{
					m_separateTimer = 0f;
					m_active = true;
					m_separated = false;
					if (m_cmb.TC.transform.rotation.z - m_concreteBody.TC.transform.rotation.z < 0f)
					{
						m_currentRate = -140f;
					}
					else
					{
						m_currentRate = 140f;
					}
					ChipmunkProWrapper.ucpSimpleMotorSetRate(m_motor.constraint, m_currentRate);
				}
			}
			if (m_active)
			{
				if (m_currentRate < 0f && m_cmb.TC.transform.rotation.z - m_concreteBody.TC.transform.rotation.z > 32.5f)
				{
					m_active = false;
				}
				else if (m_currentRate > 0f && m_cmb.TC.transform.rotation.z - m_concreteBody.TC.transform.rotation.z < -32.5f)
				{
					m_active = false;
				}
			}
		}
	}

	private void PlankCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB];
		UnitC unitC = chipmunkBodyC.customComponent as UnitC;
		if (unitC != null && unitC.m_unit != null)
		{
			Vector3 vector = m_cmb.TC.transform.InverseTransformPoint(_pair.point);
			if ((_phase == ucpCollisionPhase.Begin || _phase == ucpCollisionPhase.Persist) && Mathf.Abs(vector.x) <= 140f)
			{
				m_separated = false;
			}
			else if (_phase == ucpCollisionPhase.Separate)
			{
				m_separated = true;
				m_separateTimer = 0.5f;
			}
		}
	}
}
