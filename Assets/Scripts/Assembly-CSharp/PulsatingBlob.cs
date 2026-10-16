using UnityEngine;

public class PulsatingBlob : Unit
{
	private const float MAX_DIST = 60f;

	private const float ACCEL_START = 0f;

	private const float ACCEL_INCREMENT = 0.75f;

	public uint m_group;

	public bool m_wasHit;

	public bool m_onCooldown;

	public float m_accel;

	private ChipmunkBodyC m_bumperBottomBody;

	private ChipmunkBodyC m_bumperTopBody;

	private ChipmunkConstraintC m_spring1;

	private ChipmunkConstraintC m_spring2;

	private float m_restLength;

	private float m_coolDownTimer = 1f;

	private TransformC m_TC;

	public PulsatingBlob(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.BouncePlatformPrefab_GameObject);
		m_group = GetGroup();
		TransformC transformC = TransformS.AddComponent(m_entity, "BumperBottom");
		TransformS.SetTransform(transformC, _graphElement.m_position, _graphElement.m_rotation);
		GameObject gameObject2 = gameObject.transform.Find("BouncePlatformBody").gameObject;
		PrefabS.AddComponent(transformC, new Vector3(0f, -17f, 10f) + GetZBufferBias(), gameObject2);
		ucpPolyShape ucpPolyShape2 = ChipmunkProS.GeneratePolyShapeFromGameObject(gameObject.transform.Find("Collision2").gameObject, Vector2.zero, 1f, 0f, 0.9f, (ucpCollisionType)4, 257u);
		ucpPolyShape2.group = m_group;
		m_bumperBottomBody = ChipmunkProS.AddStaticBody(transformC, ucpPolyShape2);
		m_bumperBottomBody.customComponent = m_unitC;
		transformC = TransformS.AddComponent(m_entity, "BumperTop");
		TransformS.SetTransform(transformC, _graphElement.m_position, _graphElement.m_rotation);
		m_TC = transformC;
		gameObject2 = gameObject.transform.Find("BouncePlatform").gameObject;
		PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 10f) + GetZBufferBias(), gameObject2);
		ucpPolyShape ucpPolyShape3 = ChipmunkProS.GeneratePolyShapeFromGameObject(gameObject.transform.Find("Collision1").gameObject, Vector2.zero, 100f, 0.1f, 0.9f, (ucpCollisionType)4, 1118208u);
		ucpPolyShape3.group = m_group;
		m_bumperTopBody = ChipmunkProS.AddDynamicBody(transformC, ucpPolyShape3);
		m_bumperTopBody.customComponent = m_unitC;
		TransformC grooveA = TransformS.AddComponent(m_entity, "Groove1", _graphElement.m_position + new Vector3(0f, 120f, 0f));
		TransformC grooveB = TransformS.AddComponent(m_entity, "Groove2", _graphElement.m_position + new Vector3(0f, 0f, 0f));
		TransformC anchor = TransformS.AddComponent(m_entity, "GrooveAnchor", _graphElement.m_position + new Vector3(0f, 0f, 0f));
		ChipmunkProS.AddGrooveJoint(m_bumperBottomBody, m_bumperTopBody, grooveA, grooveB, anchor);
		m_restLength = 20f;
		grooveA = TransformS.AddComponent(m_entity, "LeftSpringAnchor1", _graphElement.m_position + new Vector3(-100f, 0f - m_restLength));
		grooveB = TransformS.AddComponent(m_entity, "LeftSpringAnchor2", _graphElement.m_position + new Vector3(-100f, 0f));
		m_spring1 = ChipmunkProS.AddDampedSpring(m_bumperBottomBody, m_bumperTopBody, grooveA, grooveB, m_restLength, 60000f, 500f);
		grooveA = TransformS.AddComponent(m_entity, "RightSpringAnchor1", _graphElement.m_position + new Vector3(100f, 0f - m_restLength));
		grooveB = TransformS.AddComponent(m_entity, "RightSpringAnchor2", _graphElement.m_position + new Vector3(100f, 0f));
		m_spring2 = ChipmunkProS.AddDampedSpring(m_bumperBottomBody, m_bumperTopBody, grooveA, grooveB, m_restLength, 60000f, 500f);
		if (!m_minigame.m_editing)
		{
			ChipmunkProS.AddCollisionHandler(m_bumperTopBody, BlobCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)4, true, true, false);
			ChipmunkProS.AddCollisionHandler(m_bumperTopBody, BlobCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)3, true, true, false);
		}
		CreateEditorTouchArea(gameObject2);
		m_graphElement.m_isRotateable = true;
	}

	public override void Update()
	{
		base.Update();
		if (m_wasHit)
		{
			float num = 80f;
			m_restLength = ToolBox.limitBetween(m_restLength + m_accel, 20f, num);
			m_accel += 0.75f;
			if (m_restLength >= num)
			{
				m_wasHit = false;
			}
		}
		else
		{
			m_accel = 0f;
			float max = 80f;
			m_restLength -= 1.8f;
			m_restLength = ToolBox.limitBetween(m_restLength, 20f, max);
			m_coolDownTimer -= Main.m_gameDeltaTime;
			if (m_coolDownTimer <= 0f)
			{
				m_onCooldown = false;
			}
		}
		ChipmunkProWrapper.ucpDampedSpringSetRestLength(m_spring1.constraint, m_restLength);
		ChipmunkProWrapper.ucpDampedSpringSetRestLength(m_spring2.constraint, m_restLength);
	}

	private void BlobCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB];
		UnitC unitC = chipmunkBodyC.customComponent as UnitC;
		if (unitC != null && unitC.m_unit != null)
		{
			Vector3 vector = m_bumperTopBody.TC.transform.InverseTransformPoint(_pair.point);
			if ((_phase == ucpCollisionPhase.Begin || _phase == ucpCollisionPhase.Persist) && Mathf.Abs(vector.x) <= 70f && !m_onCooldown)
			{
				m_onCooldown = true;
				m_wasHit = true;
				m_coolDownTimer = 1f;
				SoundS.PlaySingleShot("/InGame/Units/BouncyPlatform", m_TC.transform.position);
			}
		}
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		Destroy();
	}
}
