using System;
using UnityEngine;

public class CrushingPlatform : MovingPlatformBase
{
	private const int SHAKE_TIME = 80;

	private bool m_shaking;

	private float m_shakeMultiplier;

	private Vector3 m_shakeStartRot;

	private float m_shakeTicks;

	private float m_shakeAmount;

	private float m_shakeAdd;

	private TransformC m_stopperTC;

	private ChipmunkBodyC m_stopperCmb;

	private IntPtr m_spikeShape;

	public CrushingPlatform(GraphElement _graphElement)
		: base(_graphElement, "CrushingPlatformPrefab")
	{
		if (!m_minigame.m_editing)
		{
			ChipmunkProS.AddCollisionHandler(m_cmb, PlatformCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)4, true, false, false);
			ChipmunkProS.AddCollisionHandler(m_cmb, PlatformCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)3, true, false, false);
			m_startPos = m_cmb.TC.transform.position;
			m_shakeStartRot = m_cmb.TC.transform.rotation.eulerAngles;
		}
		m_duration = 0.0007142857f * m_moveMagnitude;
		m_shaking = true;
		m_shakeMultiplier = 1f;
		m_shakeTicks = 80f;
		m_shakeAmount = 0.025f;
		m_shakeAdd = 0f;
	}

	public override void CreateObject()
	{
		GameObject gameObject = m_mainPrefab.transform.Find("Crusher").gameObject;
		m_platformPrefabC = PrefabS.AddComponent(m_platformTC, new Vector3(0f, 0f, 10f) + GetZBufferBias(), gameObject);
		GameObject gameObject2 = m_mainPrefab.transform.Find("Rail").gameObject;
		PrefabS.AddComponent(m_pipeTC, new Vector3(0f, 0f, 60f) + GetZBufferBias(), gameObject2);
		m_pipeTC.transform.localScale = new Vector3((m_moveMagnitude + 100f) / 100f, 1f, 1f);
		GameObject gameObject3 = m_mainPrefab.transform.Find("RailNode").gameObject;
		PrefabS.AddComponent(m_startTC, new Vector3(0f, 0f, 60f) + GetZBufferBias(), gameObject3);
		m_startTC.transform.localScale = new Vector3(-1f, 1f, 1f);
		GameObject gameObject4 = m_mainPrefab.transform.Find("RailNode").gameObject;
		PrefabS.AddComponent(m_endTC, new Vector3(0f, 0f, 60f) + GetZBufferBias(), gameObject4);
		m_stopperTC = TransformS.AddComponent(m_entity, "Stopper");
		TransformS.SetTransform(m_stopperTC, m_graphElement.m_position, m_graphElement.m_rotation);
		GameObject gameObject5 = m_mainPrefab.transform.Find("CrusherPlatform").gameObject;
		PrefabS.AddComponent(m_stopperTC, new Vector3(0f, 0f, 10f) + GetZBufferBias(), gameObject5);
		TransformS.SetGlobalTransform(m_stopperTC, m_graphElement.m_position + (Vector3)m_moveDir * (m_moveMagnitude + 25f), m_graphElement.m_TC.transform.rotation.eulerAngles);
	}

	public override void CreateColliders()
	{
		ucpPolyShape[] array = new ucpPolyShape[2]
		{
			ChipmunkProS.GeneratePolyShapeFromGameObject(m_mainPrefab.transform.Find("Collision1").gameObject, Vector2.zero, 1f, 0.25f, 0.9f, (ucpCollisionType)4, 257u),
			null
		};
		array[0].group = GetGroup();
		array[1] = ChipmunkProS.GeneratePolyShapeFromGameObject(m_mainPrefab.transform.Find("Collision2").gameObject, Vector2.zero, 1f, 0.25f, 0.9f, (ucpCollisionType)4, 257u);
		array[1].group = GetGroup();
		m_cmb = ChipmunkProS.AddKinematicBody(m_platformTC, array, m_unitC);
		m_spikeShape = array[0].shapePtr;
		m_stopperCmb = ChipmunkProS.AddStaticBody(_shapes: new ucpPolyShape[2]
		{
			ChipmunkProS.GeneratePolyShapeFromGameObject(m_mainPrefab.transform.Find("Collision3").gameObject, Vector2.up * 56f, 1f, 0.25f, 0.9f, (ucpCollisionType)4, 257u),
			ChipmunkProS.GeneratePolyShapeFromGameObject(m_mainPrefab.transform.Find("Collision4").gameObject, Vector2.up * 56f, 1f, 0.25f, 0.9f, (ucpCollisionType)4, 257u)
		}, _transformComponent: m_stopperTC, _customComponent: m_unitC);
	}

	public override GraphNode CreateChild()
	{
		m_childNode = new GraphNode(GraphNodeType.Child, typeof(TouchableChildNode), "ChildNode", (Vector2)m_graphElement.m_position - Vector2.up * 100f, Vector3.zero, Vector3.one);
		m_childNode.m_isRemovable = false;
		m_childNode.m_isCopyable = false;
		m_childNode.m_minDistanceFromParent = 100f;
		m_childNode.m_maxDistanceFromParent = 1000f;
		return m_childNode;
	}

	public override void CreateGhostObject()
	{
		PrefabS.AddComponent(m_ghostPlatformTC, Vector3.forward * 10f, m_mainPrefab.transform.Find("Crusher").gameObject);
	}

	public override void EditUpdate()
	{
		base.EditUpdate();
		if (m_minigame.m_editing)
		{
			TransformS.SetGlobalRotation(m_platformTC, m_graphElement.m_rotation);
			TransformS.SetGlobalTransform(m_stopperTC, m_graphElement.m_position + (Vector3)m_moveDir * (m_moveMagnitude + 25f), m_graphElement.m_TC.transform.rotation.eulerAngles);
		}
	}

	public override void Update()
	{
		m_graphElement.m_rotation = m_pipeTC.transform.rotation.eulerAngles + new Vector3(0f, 0f, 90f);
		TransformS.SetGlobalTransform(m_graphElement.m_TC, m_graphElement.m_position, m_graphElement.m_rotation);
		base.Update();
		if (m_minigame.m_editing || !m_minigame.m_gameStarted)
		{
			return;
		}
		if (m_shaking)
		{
			if (m_shakeTicks == 80f)
			{
				SoundS.PlaySingleShot("/Ingame/Units/CrusherTremble", m_cmb.TC.transform.position);
			}
			ChipmunkProWrapper.ucpBodySetAngle(m_cmb.body, (m_shakeStartRot.z + m_shakeAdd * m_shakeMultiplier) * ((float)Math.PI / 180f));
			m_shakeAdd += 0.05f * Main.m_timeScale;
			if (m_shakeAdd >= 2f)
			{
				m_shakeAdd = 2f;
			}
			m_shakeAmount += m_shakeAdd;
			m_shakeTicks -= 1f * Main.m_timeScale;
			if (m_shakeTicks % 4f == 0f)
			{
				m_shakeMultiplier *= -1f;
			}
			if (m_shakeTicks <= 0f)
			{
				m_shaking = false;
				ChipmunkProWrapper.ucpBodySetAngle(m_cmb.body, m_graphElement.m_rotation.z * ((float)Math.PI / 180f));
				TransformS.SetGlobalRotation(m_cmb.TC, m_graphElement.m_rotation);
			}
			return;
		}
		m_shakeTicks -= 1f;
		if (m_currentTime == -1f && m_shakeTicks <= 0f)
		{
			m_startPos = m_cmb.TC.transform.position;
			m_currentTime = 0f;
		}
		else if (m_currentValue == m_moveMagnitude * m_rotMultiplier)
		{
			m_currentTime = -1f;
			m_rotMultiplier *= -1f;
			if (m_rotMultiplier == 1f)
			{
				m_duration = 0.0007142857f * m_moveMagnitude;
				m_shaking = true;
				m_shakeTicks = 80f;
				m_shakeStartRot = m_cmb.TC.transform.rotation.eulerAngles;
				m_shakeAdd = 0f;
				m_shakeAmount = UnityEngine.Random.Range(0.025f, 0.035f);
			}
			else
			{
				m_duration = 0.0092857145f * m_moveMagnitude;
				m_shakeTicks = 30f;
				SoundS.PlaySingleShot("/Ingame/Units/CrusherCrush", m_cmb.TC.transform.position);
			}
		}
		if (m_currentTime != -1f)
		{
			m_currentTime += Main.m_gameDeltaTime;
			if (m_currentTime >= m_duration)
			{
				m_currentTime = m_duration;
			}
			m_currentValue = TweenS.tween((m_rotMultiplier > 0f) ? TweenStyle.QuadIn : TweenStyle.QuadInOut, m_currentTime, m_duration, 0f, m_moveMagnitude * m_rotMultiplier);
		}
		ChipmunkProWrapper.ucpBodySetPos(m_cmb.body, m_startPos + m_moveDir * m_currentValue);
		TransformS.SetGlobalRotation(m_gearTC, m_gearTC.transform.rotation.eulerAngles + new Vector3(0f, 0f, ChipmunkProWrapper.ucpBodyGetVel(m_cmb.body).magnitude * Main.m_gameDeltaTime) * m_rotMultiplier);
	}

	private void PlatformCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB];
		UnitC unitC = chipmunkBodyC.customComponent as UnitC;
		if (unitC != null && unitC.m_unit != null && _phase == ucpCollisionPhase.Begin && _pair.shapeA == m_spikeShape)
		{
			unitC.m_unit.EmergencyKill();
		}
	}
}
