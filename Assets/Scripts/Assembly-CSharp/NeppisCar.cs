using System;
using UnityEngine;

public class NeppisCar : Vehicle
{
	private const float TIRE_FRICTION = 1.5f;

	private const float TIRE_ELASTICITY = 0.2f;

	private const float TIRE_ANGULAR_DAMP = 0.997f;

	private const float TIRE_ANGULAR_DAMP_ONGROUND = 0.3f;

	private const float FRONT_WHEEL_RAD = 14f;

	private const float FRONT_WHEEL_WEIGHT = 5f;

	private const float REAR_WHEEL_RAD = 18f;

	private const float REAR_WHEEL_WEIGHT = 5f;

	private const float CHASSIS_WEIGHT = 100f;

	private const float FRONT_SPRING_FORCE = 35000f;

	private const float FRONT_SPRING_DAMP = 600f;

	private const float REAR_SPRING_FORCE = 35000f;

	private const float REAR_SPRING_DAMP = 600f;

	private const float CHASSIS_ANGULAR_DAMP = 0.97f;

	private const float CAR_LINEAR_DAMP = 0.997f;

	private const float SELF_BALANCE_FORCE = 70f;

	private static GameObject m_mainPrefab;

	public uint m_group;

	public ChipmunkBodyC m_frontWheelBody;

	public ChipmunkBodyC m_rearWheelBody;

	public new ChipmunkBodyC m_chassisBody;

	public TransformC m_chassisTC;

	public TransformC m_alienHeadTC;

	public TransformC m_assBallTC;

	public TransformC m_assBallRubberTC;

	public float m_tireAngularDamp;

	public int m_ticksOnGround;

	private Vector2 m_centerOfGravityOffset = new Vector2(-5f, 15f);

	private Vector2 m_assBallIdlePos;

	private Vector2 m_assBallDrivePos;

	private bool m_stopped;

	private bool m_dragStartedAfterStop;

	public NeppisCar(GraphElement _graphElement)
		: base(_graphElement)
	{
		if (m_mainPrefab == null)
		{
			m_mainPrefab = ResourceManager.GetGameObject("Units/NeppisCarPrefab");
		}
		m_graphElement.m_isCopyable = false;
		m_graphElement.m_isRemovable = false;
		m_graphElement.m_isRotateable = false;
		_graphElement.m_width = 70f;
		_graphElement.m_height = 25f;
		m_group = (uint)(m_entity.m_index + 1000);
		m_chassisTC = TransformS.AddComponent(m_entity, _graphElement.m_name, _graphElement.m_position, _graphElement.m_rotation);
		TransformS.SetTransform(m_chassisTC, _graphElement.m_position - (Vector3)m_centerOfGravityOffset, _graphElement.m_rotation);
		ucpPolyShape ucpPolyShape2 = ChipmunkProS.GeneratePolyShapeFromGameObject(m_mainPrefab.transform.Find("Collision").gameObject, m_centerOfGravityOffset, 100f, 0.2f, 0.5f, (ucpCollisionType)3, 17895696u);
		ucpPolyShape2.group = m_group;
		m_chassisBody = ChipmunkProS.AddDynamicBody(m_chassisTC, ucpPolyShape2);
		m_chassisBody.customComponent = m_unitC;
		PrefabS.AddComponent(m_chassisTC, m_centerOfGravityOffset, m_mainPrefab.transform.Find("NeppisBody").gameObject);
		ChipmunkProS.SetBodyLinearDamp(m_chassisBody, new Vector2(0.997f, 0.997f));
		ChipmunkProS.SetBodyAngularDamp(m_chassisBody, 0.97f);
		m_alienHeadTC = TransformS.AddComponent(m_entity, "Alien head");
		Vector3 vector = (Vector3)m_centerOfGravityOffset + m_chassisTC.transform.position + new Vector3(0f, 5f, 0f);
		TransformS.SetTransform(m_alienHeadTC, vector, _graphElement.m_rotation);
		Vector2 offset = new Vector2(0f, 12f);
		ucpCircleShape shape = new ucpCircleShape(16f, offset, 17895696u, 6f, 0.2f, 0.5f, (ucpCollisionType)3)
		{
			group = m_group
		};
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.AddDynamicBody(m_alienHeadTC, shape);
		chipmunkBodyC.customComponent = m_unitC;
		PrefabS.AddComponent(m_alienHeadTC, Vector2.zero, m_mainPrefab.transform.Find("NeppisChar").gameObject);
		TransformC anchor = TransformS.AddComponent(m_entity, "AlienJoint1", vector + new Vector3(0f, -7f, 0f));
		ChipmunkProS.AddPivotJoint(m_chassisBody, chipmunkBodyC, anchor);
		ChipmunkProS.AddRotaryLimitJoint(m_chassisBody, chipmunkBodyC, anchor, -0.17453292f, 0.17453292f);
		anchor = TransformS.AddComponent(m_entity, "AlienSpring1", vector);
		TransformC anchor2 = TransformS.AddComponent(m_entity, "AlienSpring2", vector);
		ChipmunkProS.AddDampedSpring(m_chassisBody, chipmunkBodyC, anchor, anchor2, 0f, 2000f, 100f);
		float num = 14f;
		float num2 = 18f;
		Vector2 vector2 = m_centerOfGravityOffset + (Vector2)m_chassisTC.transform.position + new Vector2(_graphElement.m_width * 0.55f, (0f - _graphElement.m_height) * 0.48f - num * 0.5f);
		m_frontWheelBody = createTire(m_entity, m_chassisBody, vector2, num, 5f, true);
		m_frontWheelBody.customComponent = m_unitC;
		anchor = TransformS.AddComponent(m_entity, "FrontWheelJoint1", m_centerOfGravityOffset + (Vector2)m_chassisTC.transform.position + new Vector2(0f, (0f - _graphElement.m_height) * 0.5f));
		anchor2 = TransformS.AddComponent(m_entity, "FrontWheelJoint2", vector2);
		float magnitude = (anchor.transform.position - anchor2.transform.position).magnitude;
		ChipmunkProS.AddSlideJoint(m_chassisBody, m_frontWheelBody, anchor, anchor2, magnitude, magnitude);
		anchor = TransformS.AddComponent(m_entity, "FrontWheelSpringAnchor1", vector2);
		anchor2 = TransformS.AddComponent(m_entity, "FrontWheelSpringAnchor2", vector2);
		ChipmunkProS.AddDampedSpring(m_chassisBody, m_frontWheelBody, anchor, anchor2, 0f, 35000f, 600f);
		Vector2 vector3 = m_centerOfGravityOffset + (Vector2)m_chassisTC.transform.position + new Vector2((0f - _graphElement.m_width) * 0.35f, (0f - _graphElement.m_height) * 0.34f - num2 * 0.5f);
		m_rearWheelBody = createTire(m_entity, m_chassisBody, vector3, num2, 5f, false);
		m_rearWheelBody.customComponent = m_unitC;
		anchor = TransformS.AddComponent(m_entity, "RearWheelJoint1", m_centerOfGravityOffset + (Vector2)m_chassisTC.transform.position + new Vector2(0f, (0f - _graphElement.m_height) * 0.5f));
		anchor2 = TransformS.AddComponent(m_entity, "RearWheelJoint2", vector3);
		ChipmunkProS.AddPinJoint(m_chassisBody, m_rearWheelBody, anchor, anchor2);
		anchor = TransformS.AddComponent(m_entity, "RearWheelSpringAnchor1", vector3);
		anchor2 = TransformS.AddComponent(m_entity, "RearWheelSpringAnchor2", vector3);
		ChipmunkProS.AddDampedSpring(m_chassisBody, m_rearWheelBody, anchor, anchor2, 0f, 35000f, 600f);
		m_assBallTC = TransformS.AddComponent(m_entity, "Ass Ball");
		m_assBallIdlePos = m_centerOfGravityOffset + new Vector2(-48f, 0f);
		m_assBallDrivePos = m_centerOfGravityOffset + new Vector2(-35f, 0f);
		PrefabS.AddComponent(m_assBallTC, Vector2.zero, m_mainPrefab.transform.Find("AssBall").gameObject);
		TransformS.ParentComponent(m_assBallTC, m_chassisTC);
		TransformS.SetPosition(m_assBallTC, m_assBallIdlePos);
		m_assBallRubberTC = TransformS.AddComponent(m_entity, "Ass Ball Rubber");
		PrefabS.AddComponent(m_assBallRubberTC, Vector2.zero, m_mainPrefab.transform.Find("AssBallRubber").gameObject);
		TransformS.SetPosition(m_assBallRubberTC, m_chassisTC.transform.position);
		TransformS.ParentComponent(m_assBallRubberTC, m_chassisTC);
		TransformS.SetPosition(m_assBallRubberTC, m_assBallIdlePos);
		InitMotors(m_rearWheelBody, m_frontWheelBody);
		if (!m_minigame.m_editing)
		{
			CreateCamera();
			m_minigame.SetPlayer(this, m_chassisTC, typeof(UnitTouchController));
			TouchAreaC c = TouchAreaS.AddCircleArea(m_chassisTC, "Sling", 100f, CameraS.m_mainCamera);
			TouchAreaS.AddTouchEventListener(c, TouchHandler);
		}
		ProjectorS.AddComponent(m_chassisTC, ResourceManager.GetMaterial(RESOURCE.ShadowMaterial_Material), 256, new Vector3(5f, 40f));
		CreateEditorTouchArea(_graphElement.m_width, _graphElement.m_height);
		m_hitPoints = 1f;
		m_hitPointType = HitPointType.Lives;
		m_stopped = true;
		SetHandBrake(true);
	}

	public override void SetAllBaseArmours()
	{
		base.SetAllBaseArmours();
		SetBaseArmor(DamageType.Weapon, 20f);
		SetBaseArmor(DamageType.Electric, 20f);
	}

	public void CreateCamera()
	{
	}

	public override void Update()
	{
		base.Update();
		if (m_minigame.m_editing)
		{
			return;
		}
		m_ticksOnGround++;
		if (!m_isDead)
		{
			bool flag = m_contactState == ContactState.OnAir;
			if (!flag)
			{
				m_ticksOnGround++;
			}
			else
			{
				m_ticksOnGround = 0;
			}
			float num = ChipmunkProWrapper.ucpBodyGetAngVel(m_frontWheelBody.body);
			float num2 = ChipmunkProWrapper.ucpBodyGetAngVel(m_rearWheelBody.body);
			float num3 = Mathf.Abs((num + num2) * 0.5f);
			if (num3 < 5f && m_ticksOnGround > 60 && !m_stopped && !flag)
			{
				m_minigame.m_gameTicksFreezed = true;
				SetHandBrake(true);
				TransformS.SetGlobalPosition(m_assBallTC, m_chassisTC.transform.position);
				TransformS.SetPosition(m_assBallTC, m_assBallIdlePos);
				m_stopped = true;
			}
			float target = 0f;
			float z = m_chassisBody.TC.transform.rotation.eulerAngles.z;
			float num4 = ChipmunkProWrapper.ucpBodyGetAngularDamp(m_frontWheelBody.body);
			float num5 = 0.997f;
			if (flag)
			{
				float num6 = 70f;
				float f = m_chassisBody.TC.transform.rotation.eulerAngles.z * ((float)Math.PI / 180f);
				Vector2 normalized = new Vector2(Mathf.Sin(f), 0f - Mathf.Cos(f)).normalized;
				float num7 = ToolBox.limitBetween(Mathf.DeltaAngle(z, target), -90f, 90f);
			}
			else
			{
				num5 = 0.3f;
			}
			m_tireAngularDamp += (num5 - num4) * 0.02f;
		}
	}

	private void TouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		if (_touchIsSecondary)
		{
			return;
		}
		if (_touchPhase == TouchAreaPhase.Began && m_stopped)
		{
			m_dragStartedAfterStop = true;
		}
		if (m_dragStartedAfterStop)
		{
			Vector2 vector = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, _touches[0].m_currentPosition);
			float num = ChipmunkProWrapper.ucpBodyGetAngle(m_chassisBody.body);
			TransformS.SetGlobalPosition(m_assBallRubberTC, (Vector2)m_chassisTC.transform.position);
			TransformS.SetPosition(m_assBallRubberTC, m_assBallDrivePos);
			Vector2 vector2 = m_assBallRubberTC.transform.position;
			Vector2 vector3 = vector - vector2;
			float angleFromVector = ToolBox.getAngleFromVector2(-vector3.normalized);
			Vector2 vector4 = new Vector2(Mathf.Cos(num), Mathf.Sin(num));
			float num2 = Vector2.Dot(vector4, vector3.normalized);
			float num3 = vector3.magnitude * num2;
			if (num3 > 0f)
			{
				num3 = 0f;
			}
			else if (num3 < -150f)
			{
				num3 = -150f;
			}
			Vector2 vector5 = vector2 + vector4 * num3;
			switch (_touchPhase)
			{
			case TouchAreaPhase.MoveIn:
			case TouchAreaPhase.MoveOut:
				TransformS.SetGlobalPosition(m_assBallTC, vector5);
				TransformS.SetScale(m_assBallRubberTC, new Vector3((0f - num3) * 0.095f, 1.6f, 1f));
				TransformS.SetGlobalRotation(m_assBallRubberTC, new Vector3(0f, 0f, num * 57.29578f));
				break;
			case TouchAreaPhase.ReleaseIn:
			case TouchAreaPhase.ReleaseOut:
				m_ticksOnGround = 0;
				m_minigame.m_gameTicksFreezed = false;
				m_tireAngularDamp = 0.997f;
				SetHandBrake(false);
				TransformS.SetGlobalPosition(m_assBallTC, m_chassisTC.transform.position);
				TransformS.SetPosition(m_assBallTC, m_assBallDrivePos);
				TransformS.SetScale(m_assBallRubberTC, new Vector3(0f, 1.6f, 1f));
				m_dragStartedAfterStop = false;
				m_stopped = false;
				break;
			}
		}
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public ChipmunkBodyC createTire(Entity _e, ChipmunkBodyC _chassis, Vector2 _pos, float _rad, float _weight, bool _front)
	{
		TransformC transformC = TransformS.AddComponent(_e, "Car Tire");
		TransformS.SetTransform(transformC, _pos, Vector2.zero);
		ucpCircleShape ucpCircleShape2 = new ucpCircleShape(_rad, Vector2.zero, 17895696u, _weight, 0.2f, 1.5f, (ucpCollisionType)3);
		ucpCircleShape2.group = m_group;
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.AddDynamicBody(transformC, ucpCircleShape2);
		if (_front)
		{
			PrefabS.AddComponent(transformC, new Vector3(0f, 0f, -18f), m_mainPrefab.transform.Find("FrontWheel").gameObject);
			PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 18f), m_mainPrefab.transform.Find("FrontWheel2").gameObject);
		}
		else
		{
			PrefabS.AddComponent(transformC, new Vector3(0f, 0f, -25f), m_mainPrefab.transform.Find("BackWheel").gameObject);
			PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 25f), m_mainPrefab.transform.Find("BackWheel2").gameObject);
		}
		ChipmunkProS.SetBodyLinearDamp(chipmunkBodyC, new Vector2(0.997f, 0.997f));
		ChipmunkProS.SetBodyAngularDamp(chipmunkBodyC, 0.997f);
		return chipmunkBodyC;
	}
}
