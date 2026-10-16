using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OffroadCar : Vehicle, ISpeedBoost
{
	private const float TIRE_ELASTICITY = 0.2f;

	private const float RATE_SCALE = 0.8292683f;

	private const float FRONT_WHEEL_RAD = 34f;

	private const float FRONT_WHEEL_WEIGHT = 5f;

	private const float REAR_WHEEL_RAD = 41f;

	private const float REAR_WHEEL_WEIGHT = 5f;

	private const float CHASSIS_WEIGHT = 100f;

	private const float ALIEN_WEIGHT = 10f;

	private const float TIRE_ANGULAR_DAMP = 0.997f;

	private const float CHASSIS_ANGULAR_DAMP = 0.97f;

	private const float SELF_BALANCE_FORCE = 200f;

	private const int MIN_RPM = 500;

	private const int MAX_RPM = 4000;

	public float[] m_tireForceValues = new float[5] { 0.7f, 1f, 1.2f, 1.275f, 1.3f };

	public float[] m_tireRateValues = new float[5] { 40f, 55f, 62.5f, 67.5f, 70f };

	public float[] m_suspensionValues = new float[5] { -1.2f, -0.9f, -0.8f, -0.75f, -0.7f };

	public float[] m_dampValues = new float[5] { 180f, 210f, 220f, 225f, 230f };

	public float[] m_angleValues = new float[5] { 20f, 27f, 31f, 33.5f, 35f };

	public float[] m_handlingValues = new float[5] { 0.75f, 0.95f, 1.05f, 1.12f, 1.15f };

	public float[] m_gripValues = new float[5] { 0.55f, 1.2f, 1.45f, 1.67f, 1.85f };

	public float[] m_COGValues = new float[5] { -1.8f, 3f, 4.5f, 5.5f, 6f };

	public int[] m_ranges = new int[5] { 0, 5, 10, 15, 20 };

	public int m_powerUpgrade;

	public int m_handlingUpgrade;

	public int m_gripUpgrade;

	public Vector2 UNIT_LINEAR_DAMP = new Vector2(0.995f, 0.997f);

	public Vector2 m_frontTireForces = new Vector2(960000f, 960000f);

	public Vector2 m_rearTireForces = new Vector2(1280000f, 1280000f);

	private float m_tireRate = 60f;

	private float m_frontSpringForce = 10400f;

	private float m_frontSpringDamp = 220f;

	private float m_rearSpringForce = 9600f;

	private float m_rearSpringDamp = 220f;

	private float m_tireGrip = 1.5f;

	private float m_handlingForce = 0.001f;

	private float m_handlingAngle = 30f;

	private float m_tireForceMultiplier = 1f;

	private float m_suspensionMultiplier = -1f;

	private float m_COGAdd;

	public BoneModifier m_boneModifier;

	public uint m_group;

	public ChipmunkBodyC m_frontWheelBody;

	public ChipmunkBodyC m_rearWheelBody;

	public ucpCircleShape m_frontWheelShape;

	public ucpCircleShape m_rearWheelShape;

	public ChipmunkBodyC m_alienBody;

	public ChipmunkConstraintC m_frontWheelSpring;

	public ChipmunkConstraintC m_rearWheelSpring;

	public TransformC m_chassisTC;

	public TransformC m_ftSuspensersTC;

	public TransformC m_rtSuspensersTC;

	public TransformC m_chassisBottomTC;

	public TransformC m_ftBottomTC;

	public TransformC m_rtBottomTC;

	public float m_gasAmount;

	public bool m_gasWasPressed;

	public bool m_brakeWasPressed;

	public GameObject m_mainPrefab;

	public GameObject m_debrisPrefab;

	public AutoGeometryBrush m_tireBrush;

	public AutoGeometryBrush m_tireBrushSmall;

	private int m_headHitTimer;

	private bool m_hasBrokenDown;

	private bool m_tiresBlown;

	public EffectBoost m_boostEffectFrontTire;

	public EffectBoost m_boostEffectRearTire;

	public TransformC m_boostEffectFrontTireTC;

	public TransformC m_boostEffectRearTireTC;

	public int m_airTime;

	public bool m_airTimeSoundTriggered;

	public int m_destructibleGroundContacts;

	public int m_skiddingEndTimer;

	public bool m_inAir;

	private Vector2 m_centerOfGravityOffset;

	private float m_targetFakeRotationX;

	private float m_fakeRotationX;

	private int oldPressedDir;

	private float camTargetYOffset;

	private Vector2 m_chassisOffset;

	private float m_tiltHeldDownCounter;

	private float m_lastGroundContactAngle;

	private float m_boosterReferenceAngle;

	private float m_boosterBaseAngle;

	private float m_trickBoostAngle = (float)Math.PI / 4f;

	private bool m_canGiveBooster = true;

	private Texture m_mainMaterialTexture;

	private PrefabC m_antennaPrefab;

	private ChipmunkBodyC m_antennaRoot;

	private List<ChipmunkBodyC> m_antennaSegments;

	public int m_trickBoostTicks;

	public bool m_trickBoosting;

	public bool m_resourceBoosting;

	public float m_trickBoostForce;

	public OffroadCar()
		: base(new GraphNode(GraphNodeType.Normal))
	{
	}

	public OffroadCar(GraphElement _graphElement)
		: base(_graphElement)
	{
		CalculateUpgrades();
		m_tireBrush = new AutoGeometryBrush(1.5f, false, 0.25f);
		m_tireBrushSmall = new AutoGeometryBrush(1.5f, false, 0.25f);
		string resourceIdentifier = "OffroadCarPrefab_GameObject";
		if (PsState.m_activeGameLoop.m_context == PsMinigameContext.Signal)
		{
			m_mainMaterialTexture = ResourceManager.GetTexture(RESOURCE.OffroadCarDifEarth_Texture2D);
		}
		else if (PsState.m_activeGameLoop.m_selectedVehicle == PsSelectedVehicle.CreatorVehicle)
		{
			m_mainMaterialTexture = ResourceManager.GetTexture(RESOURCE.OffroadCarDifRental_Texture2D);
		}
		else
		{
			m_mainMaterialTexture = GetTierTexture(PsMetagameManager.m_playerStats.GetUpgradeValue(GetType(), "tier", 1));
		}
		m_mainPrefab = ResourceManager.GetGameObject(resourceIdentifier);
		m_debrisPrefab = ResourceManager.GetGameObject(RESOURCE.NutsBoltsSpringsPrefab_GameObject);
		DisableCollidersRenderer(m_mainPrefab.transform);
		float num = 0.45f;
		_graphElement.m_width = 130f * num;
		_graphElement.m_height = 50f * num;
		int num2 = ((!m_graphElement.m_flipped) ? 1 : (-1));
		m_group = GetGroup();
		TransformC transformC = (m_chassisTC = TransformS.AddComponent(m_entity, _graphElement.m_name));
		TransformS.SetTransform(transformC, _graphElement.m_position - (Vector3)m_centerOfGravityOffset, _graphElement.m_rotation);
		m_camTargetTC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(m_camTargetTC, _graphElement.m_position - new Vector3(0f, 16f), _graphElement.m_rotation);
		TransformS.ParentComponent(m_camTargetTC, m_chassisTC);
		m_chassisOffset = m_centerOfGravityOffset + new Vector2(0f, 2f);
		ucpPolyShape[] array = ChipmunkProS.GeneratePolyShapesFromChildren(m_mainPrefab.transform.Find("OffroadCar/OffroadCarCollider").gameObject, m_chassisOffset, 100f, 0.2f, 0.5f, (ucpCollisionType)3, 17895696u);
		ucpPolyShape[] array2 = array;
		foreach (ucpPolyShape ucpPolyShape2 in array2)
		{
			ucpPolyShape2.group = m_group;
		}
		m_chassisBody = ChipmunkProS.AddDynamicBody(transformC, array, m_unitC);
		m_chassisPrefab = PrefabS.AddComponent(transformC, m_chassisOffset, m_mainPrefab.transform.Find("OffroadCar/Chassis").gameObject);
		m_chassisPrefab.p_gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = m_mainMaterialTexture;
		if (PsState.m_activeGameLoop is PsGameLoopEditorPuzzle && PsState.m_activeMinigame != null && PsState.m_activeMinigame.m_editing)
		{
			Renderer[] componentsInChildren = m_chassisPrefab.p_gameObject.GetComponentsInChildren<Renderer>();
			m_materialInstances = new Material[componentsInChildren.Length];
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				m_materialInstances[j] = componentsInChildren[j].material;
				m_materialInstances[j].shader = Shader.Find("WOE/Units/Greyscale");
			}
		}
		m_chassisDebrisParts = new string[6] { "Bumper", "Hood", "Motor", "SteeringWheel", "DoorLeft", "DoorRight" };
		CheckVisualUpgrade("Handling", m_handlingUpgrade);
		CheckVisualUpgrade("Power", m_powerUpgrade);
		m_chassisBottomTC = TransformS.AddComponent(m_entity);
		TransformS.SetPosition(m_chassisBottomTC, transformC.transform.position + new Vector3(1 * num2, -7f, 0f));
		TransformS.ParentComponent(m_chassisBottomTC, m_chassisTC);
		ChipmunkProS.SetBodyLinearDamp(m_chassisBody, new Vector2(UNIT_LINEAR_DAMP.x, UNIT_LINEAR_DAMP.y));
		ChipmunkProS.SetBodyAngularDamp(m_chassisBody, 0.97f);
		float num3 = 34f * num;
		float num4 = 41f * num;
		Vector2 vector = m_centerOfGravityOffset + (Vector2)m_chassisTC.transform.position + new Vector2(_graphElement.m_width * 0.5f * (float)num2, (0f - _graphElement.m_height) * 0.58f - num3 * 0.5f);
		m_frontWheelBody = CreateTire(m_entity, m_chassisBody, vector, num3, 5f, true);
		m_frontWheelBody.customComponent = m_unitC;
		m_ftSuspensersTC = TransformS.AddComponent(m_entity, "FtSuspensers", (Vector3)vector + new Vector3(-7.5f, 17.5f), Vector3.forward * (0f - Vector3.Angle(m_chassisTC.transform.up, new Vector3(7.5f, -17.5f))));
		PrefabC prefabC = PrefabS.AddComponent(m_ftSuspensersTC, new Vector3(0f, 0f, -17.5f), m_mainPrefab.transform.Find("Parts/Suspension").gameObject);
		PrefabC prefabC2 = PrefabS.AddComponent(m_ftSuspensersTC, new Vector3(0f, 0f, 17.5f), m_mainPrefab.transform.Find("Parts/Suspension").gameObject);
		m_ftSuspensersTC.transform.GetChild(0).name = "Suspension";
		m_ftSuspensersTC.transform.GetChild(1).name = "Suspension";
		TransformS.ParentComponent(m_ftSuspensersTC, m_chassisTC);
		TransformC anchor = TransformS.AddComponent(m_entity, "FrontWheelJoint1", m_centerOfGravityOffset + (Vector2)m_chassisTC.transform.position + new Vector2(0f, (0f - _graphElement.m_height) * 0.5f));
		TransformC anchor2 = TransformS.AddComponent(m_entity, "FrontWheelJoint2", vector);
		ChipmunkProS.AddPinJoint(m_chassisBody, m_frontWheelBody, anchor, anchor2);
		anchor = TransformS.AddComponent(m_entity, "FrontWheelSpringAnchor1", vector);
		anchor2 = TransformS.AddComponent(m_entity, "FrontWheelSpringAnchor2", vector);
		m_frontWheelSpring = ChipmunkProS.AddDampedSpring(m_chassisBody, m_frontWheelBody, anchor, anchor2, 0f, m_frontSpringForce, m_frontSpringDamp);
		Vector2 vector2 = m_centerOfGravityOffset + (Vector2)m_chassisTC.transform.position + new Vector2((0f - _graphElement.m_width) * 0.55f * (float)num2, (0f - _graphElement.m_height) * 0.33f - num4 * 0.5f);
		m_rearWheelBody = CreateTire(m_entity, m_chassisBody, vector2, num4, 5f, false);
		m_rearWheelBody.customComponent = m_unitC;
		m_rtSuspensersTC = TransformS.AddComponent(m_entity, "RtSuspensers", (Vector3)vector2 + new Vector3(3f, 22f), Vector3.forward * Vector3.Angle(m_chassisTC.transform.up, new Vector3(-3f, -22f)));
		PrefabC prefabC3 = PrefabS.AddComponent(m_rtSuspensersTC, new Vector3(0f, 0f, -16.5f), m_mainPrefab.transform.Find("Parts/Suspension").gameObject);
		PrefabC prefabC4 = PrefabS.AddComponent(m_rtSuspensersTC, new Vector3(0f, 0f, 16.5f), m_mainPrefab.transform.Find("Parts/Suspension").gameObject);
		m_rtSuspensersTC.transform.GetChild(0).name = "Suspension";
		m_rtSuspensersTC.transform.GetChild(1).name = "Suspension";
		TransformS.ParentComponent(m_rtSuspensersTC, m_chassisTC);
		anchor = TransformS.AddComponent(m_entity, "RearWheelJoint1", m_centerOfGravityOffset + (Vector2)m_chassisTC.transform.position + new Vector2(0f, (0f - _graphElement.m_height) * 0.5f));
		anchor2 = TransformS.AddComponent(m_entity, "RearWheelJoint2", vector2);
		ChipmunkProS.AddPinJoint(m_chassisBody, m_rearWheelBody, anchor, anchor2);
		TransformS.ParentComponent(anchor, m_chassisTC);
		TransformS.ParentComponent(anchor2, m_chassisTC);
		anchor = TransformS.AddComponent(m_entity, "RearWheelSpringAnchor1", vector2);
		anchor2 = TransformS.AddComponent(m_entity, "RearWheelSpringAnchor2", vector2);
		m_rearWheelSpring = ChipmunkProS.AddDampedSpring(m_chassisBody, m_rearWheelBody, anchor, anchor2, 0f, m_rearSpringForce, m_rearSpringDamp);
		TransformS.ParentComponent(anchor, m_chassisTC);
		TransformS.ParentComponent(anchor2, m_chassisTC);
		float num5 = 1f;
		if (m_graphElement.m_flipped)
		{
			num5 = -1f;
		}
		m_ftBottomTC = TransformS.AddComponent(m_entity);
		TransformS.SetPosition(m_ftBottomTC, m_frontWheelBody.TC.transform.position + new Vector3(num5 * num4 * 0.25f, (0f - num3) * 0.33f, 0f));
		TransformS.ParentComponent(m_ftBottomTC, m_chassisTC);
		m_rtBottomTC = TransformS.AddComponent(m_entity);
		TransformS.SetPosition(m_rtBottomTC, m_rearWheelBody.TC.transform.position + new Vector3((0f - num5) * num4 * 0.25f, (0f - num4) * 0.33f, 0f));
		TransformS.ParentComponent(m_rtBottomTC, m_chassisTC);
		TransformS.ParentComponent(m_ftBottomTC, m_chassisTC);
		TransformS.ParentComponent(m_rtBottomTC, m_chassisTC);
		InitMotors(m_rearWheelBody, m_frontWheelBody);
		float num6 = 0.8292683f;
		SetMotorParameters(m_frontTireForces, m_tireRate, m_rearTireForces, m_tireRate * num6);
		if (!m_minigame.m_editing)
		{
			SetEngineSound(m_chassisTC, "/InGame/Vehicles/OffroadCarEngine", 500);
			m_minigame.SetPlayer(this, m_chassisTC, typeof(MotorcycleController));
			Hashtable upgradeValues = GetUpgradeValues();
			PsGameModeBase gameMode = PsState.m_activeGameLoop.m_gameMode;
			gameMode.CreateRecordingGhost(GetType().ToString(), upgradeValues, ParseUpgradeValues(upgradeValues));
			if (gameMode.m_recordingGhost != null)
			{
				gameMode.m_recordingGhost.AddNode("chassis", m_chassisPrefab.p_gameObject.transform);
				gameMode.m_recordingGhost.AddNode("rearWheel", m_rearWheelBody.TC.transform);
				gameMode.m_recordingGhost.AddNode("frontWheel", m_frontWheelBody.TC.transform);
				Debug.LogWarning("CREATED RECORDING GHOST");
			}
			ChipmunkProS.AddCollisionHandler(m_frontWheelBody, TireCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)2, true, false, true);
			ChipmunkProS.AddCollisionHandler(m_frontWheelBody, TireCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)4, true, false, true);
			ChipmunkProS.AddCollisionHandler(m_rearWheelBody, TireCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)2, true, false, true);
			ChipmunkProS.AddCollisionHandler(m_rearWheelBody, TireCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)4, true, false, true);
			ChipmunkProS.AddCollisionHandler(m_chassisBody, ChassisCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)2, true, false, true);
			ChipmunkProS.AddCollisionHandler(m_chassisBody, ChassisCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)4, true, false, true);
		}
		else
		{
			m_minigame.SetPlayerNode(this);
		}
		ProjectorS.AddComponent(m_chassisTC, ResourceManager.GetMaterial(RESOURCE.IngameShadow_Material), 256, new Vector3(0f, 40f));
		CreateEditorTouchArea(_graphElement.m_width, _graphElement.m_height);
		m_hitPoints = 1f;
		m_hitPointType = HitPointType.Lives;
		m_checkForCrushing = true;
		m_crushTolerance = 4000;
		m_canElectrify = true;
		m_canBurn = true;
		m_canFreeze = true;
		DebugDraw.defaultColor = new Color(1f, 1f, 1f, 1f);
		m_airTimeSoundTriggered = true;
		if (m_graphElement.m_flipped)
		{
			TransformS.SetScale(m_chassisTC, new Vector3(-1f, 1f, 1f));
		}
		Vector3 vector3 = m_mainPrefab.transform.Find("OffroadCar/CharacterLocator").localPosition + (Vector3)m_centerOfGravityOffset;
		if (PsState.m_activeGameLoop.m_context == PsMinigameContext.Signal)
		{
			m_alienCharacter = new AlienCharacter(_graphElement, "Drive", "HumanPrefab_GameObject");
		}
		else
		{
			m_alienCharacter = new AlienCharacter(_graphElement, "Drive");
		}
		TransformS.SetGlobalPosition(m_alienCharacter.m_mainTC, m_chassisTC.transform.position + vector3);
		TransformS.ParentComponent(m_alienCharacter.m_mainTC, m_chassisTC);
		if (PsState.m_activeGameLoop.m_context != PsMinigameContext.Signal && !(PsState.m_activeGameLoop is PsGameLoopEditor) && !(PsState.m_activeGameLoop is PsGameLoopEditorTemplate))
		{
			base.health = 2;
			m_alienCharacter.SetRandomHat();
		}
		else if (PsState.m_activeGameLoop.m_context == PsMinigameContext.Signal)
		{
			base.health = -1;
		}
		else
		{
			base.health = 1;
		}
		Vector2 vector4 = (Vector2)m_chassisTC.transform.localPosition + new Vector2(-2f, 28f);
		Vector2 offset = new Vector2(0f, 13f);
		TransformC transformC2 = TransformS.AddComponent(m_entity, "Alien");
		TransformS.SetTransform(transformC2, vector4, Vector2.zero);
		Transform transform = ToolBox.SearchHierarchyForBone(m_alienCharacter.m_mainPC.p_gameObject.transform, "HeadCollider");
		ucpPolyShape ucpPolyShape3 = ChipmunkProS.GeneratePolyShapeFromGameObject(transform.gameObject, offset, 10f, 0.2f, 0.5f, (ucpCollisionType)3, 17895696u, true);
		ucpPolyShape3.group = m_group;
		m_alienBody = ChipmunkProS.AddDynamicBody(transformC2, ucpPolyShape3, m_unitC);
		m_boneModifier = transformC2.transform.gameObject.AddComponent<BoneModifier>();
		m_boneModifier.AddChipmunkModifier(m_alienBody, ToolBox.SearchHierarchyForBone(m_alienCharacter.m_mainPC.p_gameObject.transform, "Spine2"), 0.5f, m_chassisTC.transform);
		m_boneModifier.AddChipmunkModifier(m_alienBody, ToolBox.SearchHierarchyForBone(m_alienCharacter.m_mainPC.p_gameObject.transform, "Neck"), 0.5f, m_chassisTC.transform);
		m_boneModifier.AddChipmunkModifier(m_alienBody, ToolBox.SearchHierarchyForBone(m_alienCharacter.m_mainPC.p_gameObject.transform, "Head"), 0.5f, m_chassisTC.transform);
		anchor = TransformS.AddComponent(m_entity, "AlienJoint1", vector4 + new Vector2(0f, -16f));
		ChipmunkProS.AddPivotJoint(m_chassisBody, m_alienBody, anchor);
		ChipmunkProS.AddRotaryLimitJoint(m_chassisBody, m_alienBody, anchor, -0.43633232f, 0.43633232f);
		anchor = TransformS.AddComponent(m_entity, "AlienSpring1", vector4 + new Vector2(0f, 15f));
		anchor2 = TransformS.AddComponent(m_entity, "AlienSpring2", vector4 + new Vector2(0f, 15f));
		ChipmunkProS.AddDampedSpring(m_chassisBody, m_alienBody, anchor, anchor2, 0f, 2400f, 150f);
		if (!m_minigame.m_editing)
		{
			ChipmunkProS.AddCollisionHandler(m_alienBody, AlienCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)2, true, false, true);
			ChipmunkProS.AddCollisionHandler(m_alienBody, AlienCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)4, true, false, true);
			m_alienCharacter.CreateCamTarget(m_camTargetTC);
			MoveUnitRandomly(0.1f);
		}
		m_graphElement.m_isCopyable = false;
		m_graphElement.m_isRemovable = false;
		m_graphElement.m_isRotateable = false;
	}

	public override void SetAllBaseArmours()
	{
		base.SetAllBaseArmours();
		SetBaseArmor(DamageType.Weapon, 17f);
		SetBaseArmor(DamageType.Electric, 20f);
		SetBaseArmor(DamageType.Fire, 20f);
	}

	public override void Update()
	{
		if (!PsState.m_activeMinigame.m_gameStarted && !PsState.m_activeMinigame.m_gameEnded && !PsState.m_activeMinigame.m_gamePaused && Controller.GetAnyButton())
		{
			PsState.m_activeGameLoop.StartMinigame();
			m_alienCharacter.ResetCameraTarget();
			m_lastGroundContact = Main.m_resettingGameTime;
		}
		if (m_headHitTimer > 0)
		{
			m_headHitTimer--;
		}
		base.Update();
		if (m_minigame.m_editing)
		{
			return;
		}
		if (m_frontWheelBody.m_active && m_rearWheelBody.m_active)
		{
			Vector3 vector = m_frontWheelBody.TC.transform.position - m_ftSuspensersTC.transform.position;
			TransformS.SetGlobalRotation(m_ftSuspensersTC, new Vector3(0f, 0f, Mathf.Atan2(vector.y, vector.x) * 57.29578f - 90f));
			Vector3 vector2 = m_rearWheelBody.TC.transform.position - m_rtSuspensersTC.transform.position;
			TransformS.SetGlobalRotation(m_rtSuspensersTC, new Vector3(0f, 0f, Mathf.Atan2(vector2.y, vector2.x) * 57.29578f - 90f));
		}
		if (!m_isDead && !m_hasBrokenDown)
		{
			ChipmunkProWrapper.ucpBodyResetForces(m_chassisBody.body);
			int num = 0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = GetContactState(m_rearWheelBody) == ContactState.OnContact;
			bool flag8 = GetContactState(m_frontWheelBody) == ContactState.OnContact;
			flag3 = Controller.GetButtonState("Throttle") == ControllerButtonState.ON;
			flag2 = Controller.GetButtonState("Reverse") == ControllerButtonState.ON;
			flag4 = Controller.GetButtonState("BoostButton") == ControllerButtonState.ON;
			flag5 = Controller.GetButtonState("LeftButton1") == ControllerButtonState.ON;
			flag6 = Controller.GetButtonState("LeftButton2") == ControllerButtonState.ON;
			if (m_contactState == ContactState.OnAir)
			{
				float num2 = ChipmunkProWrapper.ucpBodyGetAngle(m_chassisBody.body);
				float num3 = (float)Math.PI * 2f;
				if (m_canGiveBooster)
				{
					float num4 = Mathf.Abs(m_boosterReferenceAngle - num2);
					if (ToolBox.IsBetween(Mathf.Abs(m_boosterReferenceAngle - m_boosterBaseAngle), (float)Math.PI - m_trickBoostAngle, (float)Math.PI + m_trickBoostAngle))
					{
						if (num4 > m_trickBoostAngle)
						{
							TrickBoost(1);
							if (flag5)
							{
								PsAchievementManager.IncrementProgress("hundredBackFlips", 1);
							}
							m_canGiveBooster = false;
						}
					}
					else if (Mathf.Abs(num2 - m_boosterBaseAngle) >= (float)Math.PI)
					{
						TrickBoost(1);
						if (flag5)
						{
							PsAchievementManager.IncrementProgress("hundredBackFlips", 1);
						}
						m_canGiveBooster = false;
					}
				}
				else if (Mathf.Abs(num2 % num3 * num3) < m_trickBoostAngle)
				{
					m_canGiveBooster = true;
					m_boosterReferenceAngle = (float)Mathf.RoundToInt(num2 / num3) * num3;
					m_boosterBaseAngle = m_boosterReferenceAngle;
				}
			}
			if (m_booster != null && flag4)
			{
				m_booster.Use(this);
			}
			if (m_graphElement.m_flipped)
			{
				if (flag3 && !m_gasWasPressed)
				{
					SoundS.PlaySingleShotWithParameter("/InGame/Vehicles/BrakePressed", Vector3.zero, "Load", m_currentLoad);
				}
				if (flag2 && !m_brakeWasPressed)
				{
					SoundS.PlaySingleShotWithParameter("/InGame/Vehicles/GasPressed", Vector3.zero, "Load", m_currentLoad);
				}
			}
			else
			{
				if (flag3 && !m_gasWasPressed)
				{
					SoundS.PlaySingleShotWithParameter("/InGame/Vehicles/GasPressed", Vector3.zero, "Load", m_currentLoad);
				}
				if (flag2 && !m_brakeWasPressed)
				{
					SoundS.PlaySingleShotWithParameter("/InGame/Vehicles/BrakePressed", Vector3.zero, "Load", m_currentLoad);
				}
			}
			ucpSegmentQueryInfo info = default(ucpSegmentQueryInfo);
			ucpShapeFilter filter = default(ucpShapeFilter);
			filter.ucpShapeFilterAll();
			filter.group = m_group;
			ChipmunkProWrapper.ucpSpaceSegmentQueryFirst(m_chassisTC.transform.position, (Vector2)m_chassisTC.transform.position + new Vector2(0f, -200f), 1f, filter, ref info);
			if (m_airTime > 20 && !m_airTimeSoundTriggered && info.alpha > 0.5f)
			{
				if (PsState.m_activeGameLoop.m_context == PsMinigameContext.Signal)
				{
					SoundS.PlaySingleShot("/Cutscenes/RacingAudience_Cheer", Vector3.zero);
				}
				else
				{
					SoundS.PlaySingleShot("/InGame/BigAirJump", Vector3.zero);
				}
				m_alienCharacter.AnimateCharacterRandom("JumpCheer", UnityEngine.Random.Range(0, 2));
				m_airTimeSoundTriggered = true;
				PsState.m_cameraManTakeShot = true;
			}
			if (flag6)
			{
				num = -1;
			}
			else if (flag5)
			{
				num = 1;
			}
			if (num != oldPressedDir)
			{
				flag = true;
			}
			m_brakeWasPressed = flag2;
			m_gasWasPressed = flag3;
			oldPressedDir = num;
			if (m_minigame.m_gameStarted)
			{
				CameraTargetC camTarget = m_alienCharacter.m_camTarget;
				if (info.alpha == 1f)
				{
					camTargetYOffset = -100f;
				}
				else
				{
					camTargetYOffset = -100f + (1f - info.alpha) * 170f;
				}
				camTarget.offset.y += (camTargetYOffset - camTarget.offset.y) * 0.1f;
			}
			m_inAir = m_contactState == ContactState.OnAir || m_groundedChainIndex == -1;
			bool flag9 = GetContactState(m_frontWheelBody) == ContactState.OnAir && GetContactState(m_rearWheelBody) == ContactState.OnAir;
			Vector2 vector3 = (m_lastFrameVelocity = ChipmunkProWrapper.ucpBodyGetVel(m_chassisBody.body));
			if (m_inAir)
			{
				m_airTime++;
				m_targetFakeRotationX = ToolBox.limitBetween(vector3.y / 20f, -10f, 10f);
			}
			else
			{
				m_targetFakeRotationX = 0f;
				m_airTime = 0;
				m_airTimeSoundTriggered = false;
			}
			m_fakeRotationX -= (m_fakeRotationX - m_targetFakeRotationX) * 0.1f;
			m_chassisTC.transform.Rotate(new Vector3(1f, 0f, 0f), m_fakeRotationX);
			float z = m_chassisBody.TC.transform.rotation.eulerAngles.z;
			float num5 = 0f;
			if (flag3 && !flag2)
			{
				m_gasAmount = ToolBox.limitBetween(m_gasAmount + 0.05f, 0f, 1f);
			}
			else if (flag2 && !flag3)
			{
				m_gasAmount = ToolBox.limitBetween(m_gasAmount - 0.05f, -1f, 0f);
			}
			else
			{
				m_gasAmount *= 0.98f;
			}
			if (flag5)
			{
				num5 = m_handlingAngle;
			}
			else if (flag6)
			{
				num5 = 0f - m_handlingAngle;
			}
			float num6 = ToolBox.limitBetween(Mathf.DeltaAngle(z, 0f), -90f, 90f);
			Vector2 velocityRelativeToContacts = GetVelocityRelativeToContacts(m_rearWheelBody);
			bool flag10 = GetRogueContactsCount(m_rearWheelBody, false) > 0;
			float num7 = 1f - ToolBox.limitBetween(Mathf.Abs(velocityRelativeToContacts.x) / 600f, 0f, 0.5f);
			float num8 = 1f;
			bool flag11 = false;
			if (!flag9)
			{
				if (Mathf.Abs(num6) < 60f)
				{
					if (flag2 && velocityRelativeToContacts.x > 200f && !flag9)
					{
						flag11 = true;
					}
					if (flag3 && velocityRelativeToContacts.x < -200f && !flag9)
					{
						flag11 = true;
					}
				}
				if (flag11)
				{
					num8 = 0f;
					num7 = 1.5f;
				}
				if (flag10 && !flag3 && !flag2 && velocityRelativeToContacts.magnitude < 80f)
				{
					float num9 = 1f - ToolBox.getPositionBetween(velocityRelativeToContacts.magnitude, 0f, 80f);
					num8 = 0f;
					num7 = 0.4f * num9;
					flag11 = true;
				}
			}
			SetMotorParameters(m_frontTireForces * num7, m_tireRate * num8, m_rearTireForces * num7, m_tireRate * 0.8292683f * num8);
			m_alienCharacter.AnimateCharacter("DriveDir", (!flag2 || !(velocityRelativeToContacts.x < 10f)) ? 1 : (-1));
			if (!flag11)
			{
				UpdateMotors((!flag3 && !flag2) ? 0f : (m_gasAmount * m_trickBoostForce), !m_graphElement.m_flipped, m_trickBoostForce);
			}
			else
			{
				UpdateMotors(1f, !m_graphElement.m_flipped);
			}
			bool flag12 = false;
			if (m_destructibleGroundContacts > 0 && (!flag3 || !flag2) && (flag3 || flag2))
			{
				flag12 = true;
				m_skiddingEndTimer = 15;
				bool flag13 = false;
				bool flag14 = false;
				if (Main.m_gameTicks % 2 == 0)
				{
					flag13 = SkidTire(m_ftBottomTC);
					flag14 = SkidTire(m_rtBottomTC);
					if (flag13 || flag14)
					{
						Skid(m_tireBrushSmall, m_chassisBottomTC.transform.position);
					}
				}
			}
			if (m_skiddingEndTimer > 0)
			{
				m_skiddingEndTimer--;
			}
			float num10 = 0.0005f;
			float num11 = 1f;
			num10 = m_handlingForce;
			if (flag6 || flag5)
			{
				if (flag9 || m_groundedChainIndex == -1)
				{
					m_tiltHeldDownCounter += 0.0167f;
				}
				else
				{
					m_tiltHeldDownCounter = 0f;
				}
				num10 *= 5f;
			}
			else
			{
				m_tiltHeldDownCounter = 0f;
			}
			if (flag9 || m_groundedChainIndex == -1)
			{
				ToolBox.limitBetween(Mathf.DeltaAngle(z, num5), -90f, 90f);
			}
			else
			{
				num6 = num5;
			}
			float num12 = ChipmunkProWrapper.ucpBodyGetAngVel(m_chassisBody.body);
			float rolledValue = ToolBox.getRolledValue(z, -180f, 180f);
			if (flag5 && num6 < 0f && Mathf.Abs(rolledValue) > 130f)
			{
				num6 = Mathf.Abs(num6);
			}
			else if (flag6 && num6 > 0f && Mathf.Abs(rolledValue) > 90f)
			{
				num6 = 0f - num6;
			}
			if (flag)
			{
				num12 *= 0.75f;
			}
			float num13 = ToolBox.limitBetween(num6 * num10, 0f - num11, num11);
			if (m_contactState == ContactState.OnAir || m_groundedChainIndex == -1)
			{
				float num14 = Mathf.Max(0f, Mathf.Min(1f, info.alpha));
				float b = Mathf.Min(1f, Main.m_resettingGameTime - m_lastGroundContact);
				float num15 = Mathf.Max(0.3f * num14 + 0.2f, Mathf.Max(num14, b) * m_tiltHeldDownCounter);
				float num16 = ToolBox.limitBetween(num5 * num15 * ((float)Math.PI / 180f), -2f, 2f);
				num12 += num16;
				num12 = ToolBox.limitBetween(num12, -5f, 5f);
				if (Contact(CMBIdentifiers.Airflow))
				{
					Vector2 vector4 = ChipmunkProWrapper.ucpBodyGetVel(m_chassisBody.body);
					if ((num12 < 0f && vector4.x < 350f) || (num12 > 0f && vector4.x > -350f))
					{
						ChipmunkProWrapper.ucpBodyApplyImpulseAtWorldPoint(m_chassisBody.body, Vector2.right * 15f * (0f - num12), m_chassisTC.transform.position);
					}
				}
			}
			else
			{
				num12 += num13;
			}
			if (!flag8 && !flag7)
			{
				num12 = ToolBox.limitBetween(num12, -5f, 5f);
			}
			ChipmunkProWrapper.ucpBodySetAngVel(m_chassisBody.body, num12);
			if (flag9 && m_drivingFx != null)
			{
				PauseDriveFx();
			}
			else if (m_drivingFx != null)
			{
				ParticleSystem component = m_drivingFx.p_gameObject.GetComponent<ParticleSystem>();
				if (m_drivingFxName.Equals("MudSplatter_GameObject"))
				{
					if (!flag12 && m_skiddingEndTimer == 0 && component.isPlaying)
					{
						component.Stop();
					}
					else if (flag12 && component.isStopped)
					{
						component.Play();
					}
				}
				else
				{
					float emissionRate = ToolBox.getPositionBetween(m_currentRPM, 500f, 5000f) * 30f;
					component.emissionRate = emissionRate;
				}
			}
			m_boostEffectFrontTireTC.forcedRotation = Quaternion.Euler(Vector3.forward * m_chassisTC.transform.eulerAngles.z);
			m_boostEffectRearTireTC.forcedRotation = Quaternion.Euler(Vector3.forward * m_chassisTC.transform.eulerAngles.z);
			m_boostEffectFrontTireTC.updateRotation = true;
			m_boostEffectRearTireTC.updateRotation = true;
			if (m_engineSound != null)
			{
				float val = Mathf.Abs(ChipmunkProWrapper.ucpBodyGetAngVel(m_rearWheelBody.body));
				float positionBetween = ToolBox.getPositionBetween(val, 0f, 50f);
				float targetRPM = ((!flag3 && !flag2) ? 500f : Mathf.Lerp(500f, 4000f, positionBetween));
				float targetLoad = ((!flag9) ? 1 : 0);
				UpdateEngineSound(targetRPM, targetLoad);
			}
			if (m_tireRollSound != null)
			{
				if (flag9)
				{
					MuteTireRollSound();
				}
				else
				{
					UpdateTireRollSound();
				}
			}
			if (m_trickBoosting || m_resourceBoosting)
			{
				if ((m_gasWasPressed || m_brakeWasPressed) && (flag7 || flag8))
				{
					float num17 = 1f;
					if (m_brakeWasPressed)
					{
						num17 = -1f;
					}
					PsState.m_activeMinigame.m_playerUnit.AddSpeed(m_chassisTC.transform.right * m_trickBoostForce * num17, 1000f);
					m_trickBoostTicks--;
					SetBoostSound(m_chassisTC);
					m_boostEffectFrontTire.ActivateBoost((m_gasWasPressed && m_brakeWasPressed) ? EffectBoost.BoostDirection.Both : (m_gasWasPressed ? EffectBoost.BoostDirection.Right : EffectBoost.BoostDirection.Left));
					m_boostEffectRearTire.ActivateBoost((m_gasWasPressed && m_brakeWasPressed) ? EffectBoost.BoostDirection.Both : (m_gasWasPressed ? EffectBoost.BoostDirection.Right : EffectBoost.BoostDirection.Left));
				}
				else
				{
					m_boostEffectFrontTire.IdleBoost();
					m_boostEffectRearTire.IdleBoost();
				}
				if (m_trickBoostTicks <= 0)
				{
					m_trickBoosting = (m_resourceBoosting = false);
					float num18 = UpgradeLevelToValue(m_handlingUpgrade, m_ranges, m_suspensionValues);
					float num19 = 1.5f * UpgradeLevelToValue(m_gripUpgrade, m_ranges, m_gripValues);
					float value = 10400f * m_suspensionMultiplier * -1f;
					float value2 = 9600f * m_suspensionMultiplier * -1f;
					float value3 = UpgradeLevelToValue(m_handlingUpgrade, m_ranges, m_dampValues);
					float value4 = UpgradeLevelToValue(m_handlingUpgrade, m_ranges, m_dampValues);
					ChipmunkProWrapper.ucpConstraintSetMaxForce(m_frontWheelSpring.constraint, value);
					ChipmunkProWrapper.ucpDampedSpringSetDamping(m_frontWheelSpring.constraint, value3);
					ChipmunkProWrapper.ucpConstraintSetMaxForce(m_rearWheelSpring.constraint, value2);
					ChipmunkProWrapper.ucpDampedSpringSetDamping(m_rearWheelSpring.constraint, value4);
					float value5 = 1.5f * UpgradeLevelToValue(m_gripUpgrade, m_ranges, m_gripValues);
					ChipmunkProWrapper.ucpShapeSetFriction(m_frontWheelShape.shapePtr, value5);
					ChipmunkProWrapper.ucpShapeSetFriction(m_rearWheelShape.shapePtr, value5);
					RemoveBoostSound();
					m_boostEffectFrontTire.DropBoost();
					m_boostEffectRearTire.DropBoost();
				}
			}
			else
			{
				m_trickBoostForce = 1f;
			}
		}
		else if (m_trickBoosting || m_resourceBoosting)
		{
			RemoveBoostSound();
			m_boostEffectFrontTire.DropBoost();
			m_boostEffectRearTire.DropBoost();
			m_trickBoosting = (m_resourceBoosting = false);
		}
	}

	private void UpdateAntenna()
	{
		Vector3 vector = m_chassisBody.TC.transform.TransformPoint(new Vector3(-35f, 43f));
		Vector3 vector2 = vector - m_antennaRoot.TC.transform.position;
		float num = ChipmunkProWrapper.ucpBodyGetAngle(m_chassisBody.body);
		float num2 = num - ChipmunkProWrapper.ucpBodyGetAngle(m_antennaRoot.body);
		ChipmunkProWrapper.ucpBodySetPos(m_antennaRoot.body, vector);
		ChipmunkProWrapper.ucpBodySetAngle(m_antennaRoot.body, num);
		ChipmunkProWrapper.ucpBodySetVel(m_antennaRoot.body, vector2 * (1f / Main.m_gameDeltaTime));
		ChipmunkProWrapper.ucpBodySetAngVel(m_antennaRoot.body, num2 * (1f / Main.m_gameDeltaTime));
		TransformS.SetPosition(m_antennaRoot.TC, vector);
		TransformS.SetRotation(m_antennaRoot.TC, Vector3.forward * num * 57.29578f);
		string text = "Antenna/Bone1";
		for (int i = 1; i < m_antennaSegments.Count; i++)
		{
			text = text + "/Bone" + (i + 1);
			Transform transform = m_antennaPrefab.p_gameObject.transform.FindChild(text);
			if (transform != null)
			{
				transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, m_antennaSegments[i].TC.transform.rotation.eulerAngles.z));
			}
		}
	}

	private void CreateAntenna(GameObject _antennaPrefab, Entity _e, ChipmunkBodyC _chassis, Vector2 _pos)
	{
		float num = 8f;
		TransformC transformC = TransformS.AddComponent(_e, "Antenna");
		TransformS.ParentComponent(transformC, _chassis.TC, new Vector3(-35f, 43f, 0f));
		m_antennaPrefab = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, -20f), _antennaPrefab);
		m_antennaSegments = new List<ChipmunkBodyC>();
		for (int i = 1; i <= 5; i++)
		{
			TransformC transformC2 = TransformS.AddComponent(_e, "AntennaSegment" + i);
			TransformS.SetTransform(transformC2, Vector3.up * num * (i - 1) + transformC.transform.position, Vector2.zero);
			ucpPolyShape ucpPolyShape2 = new ucpPolyShape(4f, num, Vector2.zero, 17895696u, 6f / (float)i, 0f, 0f, (ucpCollisionType)3);
			ucpPolyShape2.group = m_group;
			ChipmunkBodyC chipmunkBodyC = ((m_antennaSegments.Count != 0) ? ChipmunkProS.AddDynamicBody(transformC2, ucpPolyShape2) : (m_antennaRoot = ChipmunkProS.AddKinematicBody(transformC2, ucpPolyShape2)));
			if (m_antennaSegments.Count > 0)
			{
				TransformC transformC3 = TransformS.AddComponent(_e, "AntennaJoint" + i);
				TransformS.SetTransform(transformC3, transformC2.transform.position - Vector3.up * (num / 2f), Vector2.zero);
				ChipmunkProS.AddPivotJoint(m_antennaSegments[m_antennaSegments.Count - 1], chipmunkBodyC, transformC3);
				ChipmunkProS.AddRotaryLimitJoint(m_antennaSegments[m_antennaSegments.Count - 1], chipmunkBodyC, transformC2, -(float)Math.PI / 3f, (float)Math.PI / 3f);
				ChipmunkProS.AddDampedRotarySpring(m_antennaSegments[m_antennaSegments.Count - 1], chipmunkBodyC, transformC2, 0f, 2000000f / (float)i, 10000f);
			}
			m_antennaSegments.Add(chipmunkBodyC);
		}
	}

	public void Skid(AutoGeometryBrush _brush, Vector2 _pos)
	{
		_pos += new Vector2(16f, 16f);
		foreach (AutoGeometryLayer layer in AutoGeometryManager.m_layers)
		{
			if (layer.m_groundC.m_ground.m_skiddable)
			{
				layer.PaintWithBrush(_brush, _pos, AGDrawMode.SUB, true, ref layer.m_bytes);
			}
		}
	}

	public bool SkidTire(TransformC _c)
	{
		Vector2 pos = _c.transform.position;
		pos += new Vector2(16f, 16f);
		foreach (AutoGeometryLayer layer in AutoGeometryManager.m_layers)
		{
			if (layer.m_groundC.m_ground.m_skiddable)
			{
				layer.PaintWithBrush(m_tireBrush, pos, AGDrawMode.SUB, true, ref layer.m_bytes);
			}
		}
		return true;
	}

	public void AddDirt(Vector2 _pos)
	{
		_pos += new Vector2(16f, 16f);
		AutoGeometryBrush autoGeometryBrush = new AutoGeometryBrush(1.5f, false);
		AutoGeometryLayer autoGeometryLayer = AutoGeometryManager.m_layers[0];
		autoGeometryLayer.PaintWithBrush(autoGeometryBrush, _pos, AGDrawMode.ADD, true, ref autoGeometryLayer.m_bytes);
		autoGeometryBrush.Destroy();
	}

	private ChipmunkBodyC CreateTire(Entity _e, ChipmunkBodyC _chassis, Vector2 _pos, float _rad, float _weight, bool _front)
	{
		TransformC transformC = TransformS.AddComponent(_e, "Car Tire");
		TransformC transformC2 = TransformS.AddComponent(_e, "Effect");
		transformC2.forceRotation = true;
		TransformS.ParentComponent(transformC2, transformC, Vector3.zero);
		TransformS.SetTransform(transformC, _pos, Vector2.zero);
		ucpCircleShape ucpCircleShape2 = new ucpCircleShape(_rad, Vector2.zero, 17895696u, _weight, 0.2f, m_tireGrip, (ucpCollisionType)3);
		ucpCircleShape2.group = m_group;
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.AddDynamicBody(transformC, ucpCircleShape2);
		if (_front)
		{
			PrefabC prefabC = PrefabS.AddComponent(transformC2, Vector3.forward * -24f, ResourceManager.GetGameObject(RESOURCE.EffectBoostOffroadCarFront_GameObject), "BoostEffect");
			m_boostEffectFrontTire = prefabC.p_gameObject.GetComponent<EffectBoost>();
			m_boostEffectFrontTireTC = transformC2;
		}
		else
		{
			PrefabC prefabC2 = PrefabS.AddComponent(transformC2, Vector3.forward * -24f, ResourceManager.GetGameObject(RESOURCE.EffectBoostOffroadCarBack_GameObject), "BoostEffect");
			m_boostEffectRearTire = prefabC2.p_gameObject.GetComponent<EffectBoost>();
			m_boostEffectRearTireTC = transformC2;
		}
		PrefabC prefabC3 = null;
		PrefabC prefabC4 = null;
		string text = string.Empty;
		int num = Mathf.FloorToInt((float)m_gripUpgrade / 5f);
		if (num > 0)
		{
			text = "T" + num;
		}
		while (m_mainPrefab.transform.Find("Parts/CarTireFront" + text) == null)
		{
			num--;
			text = ((num <= 0) ? string.Empty : ("T" + num));
		}
		if (_front)
		{
			prefabC4 = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, -23f), m_mainPrefab.transform.Find("Parts/CarTireFront" + text).gameObject);
			prefabC3 = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 23f), m_mainPrefab.transform.Find("Parts/CarTireFront" + text).gameObject);
			prefabC3.m_identifier = 10;
			prefabC4.m_identifier = 10;
			m_frontWheelShape = ucpCircleShape2;
		}
		else
		{
			prefabC4 = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, -23f), m_mainPrefab.transform.Find("Parts/CarTireRear" + text).gameObject);
			prefabC3 = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 23f), m_mainPrefab.transform.Find("Parts/CarTireRear" + text).gameObject);
			prefabC3.m_identifier = 11;
			prefabC4.m_identifier = 11;
			m_rearWheelShape = ucpCircleShape2;
		}
		prefabC3.p_gameObject.transform.localScale = new Vector3(1f, 1f, -1f);
		ChipmunkProS.SetBodyLinearDamp(chipmunkBodyC, new Vector2(UNIT_LINEAR_DAMP.x, UNIT_LINEAR_DAMP.y));
		ChipmunkProS.SetBodyAngularDamp(chipmunkBodyC, 0.997f);
		return chipmunkBodyC;
	}

	private void RemoveTire(ChipmunkBodyC _c, int _identifier = 10)
	{
		List<IComponent> componentsByIdentifier = EntityManager.GetComponentsByIdentifier(m_entity, _identifier);
		for (int i = 0; i < componentsByIdentifier.Count; i++)
		{
			PrefabC c = componentsByIdentifier[i] as PrefabC;
			PrefabS.RemoveComponent(c);
		}
		TransformS.RemoveComponent(_c.TC);
		ChipmunkProS.RemoveBody(_c);
	}

	public void TrickBoost(int _tricks, bool _resourceBoost = false)
	{
		m_trickBoostTicks += 30 + 30 * _tricks;
		if (m_trickBoostTicks > 240)
		{
			m_trickBoostTicks = 240;
		}
		m_trickBoostForce = 1.85f;
		m_resourceBoosting = _resourceBoost;
		m_trickBoosting = m_trickBoosting || !_resourceBoost;
		SoundS.PlaySingleShot("/Ingame/Vehicles/BoosterActivate", Vector2.zero);
		m_boostEffectFrontTire.GainBoost();
		m_boostEffectRearTire.GainBoost();
		if (!m_trickBoosting && !m_resourceBoosting)
		{
			float num = UpgradeLevelToValue(20, m_ranges, m_suspensionValues);
			float value = 10400f * m_suspensionMultiplier * -1f;
			float value2 = 9600f * m_suspensionMultiplier * -1f;
			float value3 = UpgradeLevelToValue(20, m_ranges, m_dampValues);
			float value4 = UpgradeLevelToValue(20, m_ranges, m_dampValues);
			ChipmunkProWrapper.ucpConstraintSetMaxForce(m_frontWheelSpring.constraint, value);
			ChipmunkProWrapper.ucpDampedSpringSetDamping(m_frontWheelSpring.constraint, value3);
			ChipmunkProWrapper.ucpConstraintSetMaxForce(m_rearWheelSpring.constraint, value2);
			ChipmunkProWrapper.ucpDampedSpringSetDamping(m_rearWheelSpring.constraint, value4);
			float value5 = 1.5f * UpgradeLevelToValue(20, m_ranges, m_gripValues);
			ChipmunkProWrapper.ucpShapeSetFriction(m_frontWheelShape.shapePtr, value5);
			ChipmunkProWrapper.ucpShapeSetFriction(m_rearWheelShape.shapePtr, value5);
			m_boostEffectFrontTire.IdleBoost();
			m_boostEffectRearTire.IdleBoost();
		}
	}

	public override void UnitContactEnd(ContactInfo _contactInfo)
	{
		base.UnitContactEnd(_contactInfo);
		if (!m_isDead && !m_hasBrokenDown)
		{
			m_boosterReferenceAngle = ChipmunkProWrapper.ucpBodyGetAngle(m_chassisBody.body);
			m_boosterBaseAngle = (float)Mathf.RoundToInt(m_boosterReferenceAngle / ((float)Math.PI * 2f)) * (float)Math.PI * 2f;
			m_canGiveBooster = true;
		}
	}

	public override void UnitContactStart(ContactInfo _contactInfo)
	{
		base.UnitContactStart(_contactInfo);
		if (!m_isDead && !m_hasBrokenDown)
		{
		}
	}

	public override void GroundContactStart(ContactInfo _contactInfo)
	{
		base.GroundContactStart(_contactInfo);
		if (m_isDead || m_hasBrokenDown)
		{
			return;
		}
		Ground ground = _contactInfo.m_ground;
		if (_contactInfo.m_contactBody.body == m_rearWheelBody.body || _contactInfo.m_contactBody.body == m_frontWheelBody.body)
		{
			SetTireRollSound(m_chassisTC, ground.m_tireRollSound);
			bool flag = true;
			if (ground.m_driveFX != null && m_drivingFx != null && ground.m_driveFX.Equals(m_drivingFxName))
			{
				flag = false;
			}
			if (flag)
			{
				RemoveDriveFx();
			}
			StartDriveFx(m_chassisTC, ground.m_skiddable, ground.m_driveFX);
			if (ground.m_skiddable)
			{
				m_destructibleGroundContacts++;
			}
		}
	}

	public override void GroundContactEnd(ContactInfo _contactInfo)
	{
		base.GroundContactEnd(_contactInfo);
		if (m_isDead || m_hasBrokenDown)
		{
			return;
		}
		m_boosterReferenceAngle = ChipmunkProWrapper.ucpBodyGetAngle(m_chassisBody.body);
		m_boosterBaseAngle = (float)Mathf.RoundToInt(m_boosterReferenceAngle / ((float)Math.PI * 2f)) * (float)Math.PI * 2f;
		m_canGiveBooster = true;
		Ground ground = _contactInfo.m_ground;
		if (!(_contactInfo.m_contactBody.body == m_rearWheelBody.body) && !(_contactInfo.m_contactBody.body == m_frontWheelBody.body))
		{
			return;
		}
		if (ground.m_skiddable)
		{
			m_destructibleGroundContacts--;
		}
		Ground ground2 = null;
		for (int num = m_contacts.Count - 1; num > -1; num--)
		{
			if (m_contacts[num].m_ground != ground)
			{
				ground2 = m_contacts[num].m_ground;
				break;
			}
		}
		MuteTireRollSound();
		PauseDriveFx();
		if (ground2 == null)
		{
			return;
		}
		if (ground2.m_tireRollSound != null)
		{
			SetTireRollSound(m_chassisTC, ground2.m_tireRollSound);
		}
		if (ground2.m_driveFX != null)
		{
			if (!ground2.m_driveFX.Equals(m_drivingFxName))
			{
				RemoveDriveFx();
			}
			StartDriveFx(m_chassisTC, ground2.m_skiddable, ground2.m_driveFX);
		}
	}

	private void TireCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		if (!m_isDead && !m_hasBrokenDown && _phase == ucpCollisionPhase.Begin)
		{
			float positionBetween = ToolBox.getPositionBetween(_pair.impulse.magnitude, 2500f, 10000f);
			if (positionBetween > 0f)
			{
				SoundS.PlaySingleShotWithParameter("/InGame/Vehicles/TireImpact", Vector3.zero, "Force", 0.2f + positionBetween * 0.8f);
			}
		}
	}

	private void ChassisCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		if (m_isDead || m_hasBrokenDown || _phase != ucpCollisionPhase.Begin)
		{
			return;
		}
		float positionBetween = ToolBox.getPositionBetween(_pair.impulse.magnitude, 3500f, 25000f);
		if (positionBetween > 0f)
		{
			SoundS.PlaySingleShotWithParameter("/InGame/Vehicles/ChassisImpact", Vector3.zero, "Force", 0.2f + positionBetween * 0.8f);
			if (positionBetween > 0.9f)
			{
				CreateRandomImpactDebris(m_chassisDebrisParts, m_debrisPrefab);
			}
		}
	}

	private void AlienCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		int identifier = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB].m_identifier;
		if (_phase != ucpCollisionPhase.Begin || m_headHitTimer != 0 || identifier == 2 || identifier == 3)
		{
			return;
		}
		float positionBetween = ToolBox.getPositionBetween(_pair.impulse.magnitude, 1600f, 6000f);
		if (positionBetween > 0f)
		{
			SoundS.PlaySingleShotWithParameter("/InGame/Vehicles/AlienHeadImpact", Vector3.zero, "Force", positionBetween);
			m_alienCharacter.AnimateCharacter("Hit");
		}
		float positionBetween2 = ToolBox.getPositionBetween(_pair.impulse.magnitude, 3600f, 10000f);
		if (!(positionBetween2 > 0f))
		{
			return;
		}
		EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject(RESOURCE.HeadHit_GameObject), new Vector3(_pair.point.x, _pair.point.y, 0f), Vector3.zero, 2f, "GTAG_AUTODESTROY");
		m_headHitTimer = 15;
		if (!ChipmunkProWrapper.ucpShapeGetSensor(_pair.shapeB))
		{
			if (DecreaseHealth())
			{
				Kill(DamageType.Impact, float.MaxValue);
				DetachHeadwear();
			}
			else if (base.health == 1)
			{
				DetachHeadwear();
			}
		}
	}

	public void DetachHeadwear()
	{
		Vector2 force = Vector2.zero;
		if (m_alienBody != null)
		{
			force = ChipmunkProWrapper.ucpBodyGetVel(m_alienBody.body);
		}
		if (m_alienCharacter != null)
		{
			m_alienCharacter.ThrowHelmet(force, 0f, false, 17895696u, false, true);
		}
	}

	public override void EmergencyKill()
	{
		if (m_entity != null)
		{
			ExplosionEffect(m_chassisTC.transform.position);
			Kill(DamageType.Impact, float.MaxValue);
			Debris.CreateDebrisFromChildren(m_chassisPrefab.p_gameObject.transform, m_lastFrameVelocity * 0.7f, new Vector2(2000f, 2000f), 2000f, true, m_group);
			RemoveAssembledEntity(m_entity);
			m_entity = null;
		}
	}

	public override void LaunchRagdoll(bool _explodeBones = false)
	{
		ChipmunkBodyShape bodyShapeByTag = ChipmunkProS.GetBodyShapeByTag(m_chassisBody, "OffroadCarColliderMiddle");
		ChipmunkProWrapper.ucpShapeSetLayers(bodyShapeByTag.shapePtr, 0u);
		TransformS.UnparentComponent(m_alienCharacter.m_mainTC);
		DetachHeadwear();
		Vector2 vector = m_lastFrameVelocity * 0.7f;
		if (vector.y < 0f)
		{
			vector.y *= 0.5f;
		}
		m_alienCharacter.ConstructRagdoll();
		m_alienCharacter.AddLinearVelocityToRagDoll(m_alienCharacter.m_ragdoll, vector + (Vector2)m_chassisTC.transform.up * 200f, 60f);
		DestroyBoneModifier();
		ChipmunkProS.RemoveBody(m_alienBody);
		m_alienBody = null;
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		if (!m_hasBrokenDown)
		{
			if (!m_minigame.m_gameEnded)
			{
				PsState.m_activeMinigame.TweenTimeScale(0.05f, TweenStyle.ExpoIn, 0.1f, delegate
				{
					PsState.m_activeMinigame.TweenTimeScale(1f, TweenStyle.ExpoIn, 0.25f);
				}, 0.5f);
			}
			m_hasBrokenDown = true;
			UpdateMotors(0f);
			if (m_booster != null)
			{
				m_booster = null;
			}
			RemoveEngineSound();
			RemoveTireRollSound();
			RemoveDriveFx();
			ChipmunkProWrapper.ucpBodyResetForces(m_chassisBody.body);
			m_alienCharacter.AnimateCharacter("Death");
			LaunchRagdoll();
			if (_damageType != DamageType.Impact)
			{
				Debris.CreateDebrisFromGO(m_chassisPrefab.p_gameObject.transform.FindChild("Bumper"), GetRandomDebrisVel(), GetRandomDebrisAngVel(), true, m_alienCharacter.m_group, false, null, Vector3.zero, -1f, 17895696u);
				Debris.CreateDebrisFromGO(m_chassisPrefab.p_gameObject.transform.FindChild("Hood"), GetRandomDebrisVel(), GetRandomDebrisAngVel(), true, m_alienCharacter.m_group, false, null, Vector3.zero, -1f, 17895696u);
				Debris.CreateDebrisFromGO(m_chassisPrefab.p_gameObject.transform.FindChild("Motor"), GetRandomDebrisVel(), GetRandomDebrisAngVel(), true, m_alienCharacter.m_group, false, null, Vector3.zero, -1f, 17895696u);
			}
			SoundS.PlaySingleShot("/InGame/Characters/AlienDisappointment", Vector3.zero);
			if (_damageType == DamageType.Weapon)
			{
				SoundS.PlaySingleShot("/Ingame/Characters/AlienDeathScream", Vector3.zero);
			}
			if (_damageType != DamageType.Impact)
			{
				PrefabS.AddComponent(m_chassisTC, new Vector3(33.13f, 30.11f, -2.91f), ResourceManager.GetGameObject(RESOURCE.EngineBreakdown_GameObject));
			}
			if (!m_minigame.m_playerReachedGoal)
			{
				PsState.m_activeGameLoop.LoseMinigame();
			}
		}
		if (m_tiresBlown || _damageType != DamageType.Weapon)
		{
			return;
		}
		SoundS.PlaySingleShot("/InGame/Vehicles/OffroadCarDeath", Vector3.zero);
		if (!m_minigame.m_gameEnded)
		{
			PsState.m_activeMinigame.TweenTimeScale(0.025f, TweenStyle.ExpoIn, 0.1f, delegate
			{
				PsState.m_activeMinigame.TweenTimeScale(1f, TweenStyle.ExpoIn, 0.25f);
			}, 0.5f);
		}
		CreateRandomImpactDebris(m_chassisDebrisParts, m_debrisPrefab);
		CreateTireDebris();
		m_tiresBlown = true;
	}

	public void CreateTireDebris()
	{
		int num = UnityEngine.Random.Range(1, 4);
		for (int i = ((num == 2) ? 1 : num); i <= ((num != 2) ? (num + 1) : 4); i++)
		{
			Entity entity = EntityManager.AddEntity(new string[1] { "GTAG_AUTODESTROY" });
			TransformC transformC = TransformS.AddComponent(entity, "Tire" + i);
			TransformS.SetTransform(transformC, (i >= 3) ? m_rearWheelBody.TC.transform.position : m_frontWheelBody.TC.transform.position, Vector2.zero);
			ucpCircleShape ucpCircleShape2 = new ucpCircleShape(((i >= 3) ? 41f : 34f) * 0.45f, Vector2.zero, 17895696u, (i >= 3) ? 5f : 5f, 0.2f, m_tireGrip, (ucpCollisionType)7);
			ucpCircleShape2.group = (uint)(m_group + ((i < 3) ? 1 : 2));
			ChipmunkBodyC chipmunkBodyC = ChipmunkProS.AddDynamicBody(transformC, ucpCircleShape2);
			PrefabC prefabC = null;
			prefabC = ((i >= 3) ? PrefabS.AddComponent(transformC, new Vector3(0f, 0f, (i % 2 != 1) ? 23 : (-23)), m_mainPrefab.transform.Find("Parts/CarTireRear").gameObject) : PrefabS.AddComponent(transformC, new Vector3(0f, 0f, (i % 2 != 1) ? 23 : (-23)), m_mainPrefab.transform.Find("Parts/CarTireFront").gameObject));
			if (i % 2 == 0)
			{
				prefabC.p_gameObject.transform.localScale = new Vector3(1f, 1f, -1f);
			}
			ChipmunkProWrapper.ucpBodySetVel(chipmunkBodyC.body, GetRandomDebrisVel());
			ChipmunkProWrapper.ucpBodySetAngVel(chipmunkBodyC.body, GetRandomDebrisAngVel());
		}
		if (num <= 2)
		{
			Debris.CreateDebrisFromGO(m_ftSuspensersTC.transform.GetChild(0), GetRandomDebrisVel(), GetRandomDebrisAngVel(), true, m_alienCharacter.m_group, true, null, Vector3.zero, -1f, 17895696u);
			Debris.CreateDebrisFromGO(m_ftSuspensersTC.transform.GetChild(0), GetRandomDebrisVel(), GetRandomDebrisAngVel(), true, m_alienCharacter.m_group, true, null, Vector3.zero, -1f, 17895696u);
			RemoveTire(m_frontWheelBody);
		}
		if (num >= 2)
		{
			Debris.CreateDebrisFromGO(m_rtSuspensersTC.transform.GetChild(0), GetRandomDebrisVel(), GetRandomDebrisAngVel(), true, m_alienCharacter.m_group, true, null, Vector3.zero, -1f, 17895696u);
			Debris.CreateDebrisFromGO(m_rtSuspensersTC.transform.GetChild(0), GetRandomDebrisVel(), GetRandomDebrisAngVel(), true, m_alienCharacter.m_group, true, null, Vector3.zero, -1f, 17895696u);
			RemoveTire(m_rearWheelBody, 11);
		}
	}

	public void DestroyBoneModifier()
	{
		if (m_boneModifier != null)
		{
			UnityEngine.Object.DestroyImmediate(m_boneModifier);
			m_boneModifier = null;
		}
	}

	public override void Destroy()
	{
		if (m_booster != null)
		{
			m_booster = null;
		}
		if (m_minigame != null)
		{
			m_minigame.RemovePlayer();
		}
		DestroyBoneModifier();
		base.Destroy();
		if (m_alienCharacter != null)
		{
			m_alienCharacter.Destroy();
		}
		if (m_tireBrush != null)
		{
			m_tireBrush.Destroy();
		}
		if (m_tireBrushSmall != null)
		{
			m_tireBrushSmall.Destroy();
		}
	}

	public override List<int> ParseUpgradeValues(Hashtable _upgrades)
	{
		ParseMinAndMaxValues();
		float value = ((_upgrades != null) ? Convert.ToSingle(_upgrades["tireForce"]) : 1f);
		float value2 = ((_upgrades != null) ? Convert.ToSingle(_upgrades["tireRate"]) : 60f);
		float value3 = ((_upgrades != null) ? Convert.ToSingle(_upgrades["grip"]) : 1f);
		float value4 = ((_upgrades != null) ? Convert.ToSingle(_upgrades["suspensionForce"]) : (-1f));
		float value5 = ((_upgrades != null) ? Convert.ToSingle(_upgrades["suspensionDamp"]) : 220f);
		float value6 = ((_upgrades != null) ? Convert.ToSingle(_upgrades["COG"]) : 0f);
		float value7 = ((_upgrades != null) ? Convert.ToSingle(_upgrades["handlingForce"]) : 1f) / 1000f;
		float value8 = ((_upgrades != null) ? Convert.ToSingle(_upgrades["handlingAngle"]) : 30f);
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		List<int> list3 = new List<int>();
		List<int> _upgradeValues = new List<int>();
		list.Add(VehicleValueToUpgradeLevel(value, m_ranges, m_tireForceValues));
		list.Add(VehicleValueToUpgradeLevel(value2, m_ranges, m_tireRateValues));
		list2.Add(VehicleValueToUpgradeLevel(value3, m_ranges, m_gripValues));
		list3.Add(VehicleValueToUpgradeLevel(value4, m_ranges, m_suspensionValues));
		list3.Add(VehicleValueToUpgradeLevel(value5, m_ranges, m_dampValues));
		list3.Add(VehicleValueToUpgradeLevel(value6, m_ranges, m_COGValues));
		list3.Add(VehicleValueToUpgradeLevel(value7, m_ranges, m_handlingValues));
		list3.Add(VehicleValueToUpgradeLevel(value8, m_ranges, m_angleValues));
		_upgradeValues.Add(list.Max());
		_upgradeValues.Add(list2.Max());
		_upgradeValues.Add(list3.Max());
		AdjustTiers(ref _upgradeValues);
		_upgradeValues.Add(GetTier(_upgradeValues));
		return _upgradeValues;
	}

	public override Hashtable GetUpgradeValues()
	{
		CalculateUpgrades();
		Debug.LogWarning("GETTING VEHICLE UPGRADE VALUES");
		Hashtable hashtable = new Hashtable();
		hashtable.Add("tireForce", m_tireForceMultiplier);
		hashtable.Add("tireRate", m_tireRate);
		hashtable.Add("grip", m_tireGrip / 1.5f);
		hashtable.Add("suspensionForce", m_suspensionMultiplier);
		hashtable.Add("suspensionDamp", m_rearSpringDamp);
		hashtable.Add("COG", m_COGAdd);
		hashtable.Add("handlingForce", m_handlingForce * 1000f);
		hashtable.Add("handlingAngle", m_handlingAngle);
		return hashtable;
	}

	public override List<string> GetNodeUpgradeNames()
	{
		List<string> list = new List<string>();
		list.Add("RANGES");
		list.Add("tireForceValues");
		list.Add("tireRateValues");
		list.Add("gripValues");
		list.Add("suspensionValues");
		list.Add("dampValues");
		list.Add("COGValues");
		list.Add("handlingValues");
		list.Add("angleValues");
		return list;
	}

	public override List<KeyValuePair<string, int>> GetUpgrades()
	{
		Debug.LogWarning("GETTING PLAYER UPGRADES");
		List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
		KeyValuePair<string, int> item = new KeyValuePair<string, int>("power", PsMetagameManager.m_playerStats.GetUpgradeValue(GetType(), "power"));
		list.Add(item);
		item = new KeyValuePair<string, int>("grip", PsMetagameManager.m_playerStats.GetUpgradeValue(GetType(), "grip"));
		list.Add(item);
		item = new KeyValuePair<string, int>("handling", PsMetagameManager.m_playerStats.GetUpgradeValue(GetType(), "handling"));
		list.Add(item);
		return list;
	}

	public void ParseMinAndMaxValues()
	{
		PsUpgradeableEditorItem psUpgradeableEditorItem = PsMetagameData.GetUnlockableByIdentifier(GetType().ToString()) as PsUpgradeableEditorItem;
		if (psUpgradeableEditorItem != null)
		{
			Hashtable upgradeValues = psUpgradeableEditorItem.m_upgradeValues;
			m_ranges = Array.ConvertAll<object, int>((object[])upgradeValues["RANGES"], base.ToInt);
			m_tireForceValues = Array.ConvertAll<object, float>((object[])upgradeValues["tireForceValues"], base.ToFloat);
			m_tireRateValues = Array.ConvertAll<object, float>((object[])upgradeValues["tireRateValues"], base.ToFloat);
			m_gripValues = Array.ConvertAll<object, float>((object[])upgradeValues["gripValues"], base.ToFloat);
			m_suspensionValues = Array.ConvertAll<object, float>((object[])upgradeValues["suspensionValues"], base.ToFloat);
			m_dampValues = Array.ConvertAll<object, float>((object[])upgradeValues["dampValues"], base.ToFloat);
			m_COGValues = Array.ConvertAll<object, float>((object[])upgradeValues["COGValues"], base.ToFloat);
			m_handlingValues = Array.ConvertAll<object, float>((object[])upgradeValues["handlingValues"], base.ToFloat);
			m_angleValues = Array.ConvertAll<object, float>((object[])upgradeValues["angleValues"], base.ToFloat);
			m_upgradeSteps = psUpgradeableEditorItem.m_upgradeSteps;
		}
	}

	public void CalculateUpgrades()
	{
		ParseMinAndMaxValues();
		if (PsState.m_activeGameLoop.m_selectedVehicle == PsSelectedVehicle.CreatorVehicle)
		{
			m_powerUpgrade = PsState.m_activeGameLoop.m_rentUpgradeValues[0];
			m_gripUpgrade = PsState.m_activeGameLoop.m_rentUpgradeValues[1];
			m_handlingUpgrade = PsState.m_activeGameLoop.m_rentUpgradeValues[2];
		}
		else
		{
			m_powerUpgrade = PsMetagameManager.m_playerStats.GetUpgradeValue(GetType(), "power");
			m_gripUpgrade = PsMetagameManager.m_playerStats.GetUpgradeValue(GetType(), "grip");
			m_handlingUpgrade = PsMetagameManager.m_playerStats.GetUpgradeValue(GetType(), "handling");
		}
		m_tireForceMultiplier = UpgradeLevelToValue(m_powerUpgrade, m_ranges, m_tireForceValues);
		m_frontTireForces = new Vector2(960000f, 960000f) * m_tireForceMultiplier;
		m_rearTireForces = new Vector2(1280000f, 1280000f) * m_tireForceMultiplier;
		m_tireRate = UpgradeLevelToValue(m_powerUpgrade, m_ranges, m_tireRateValues);
		m_tireGrip = 1.5f * UpgradeLevelToValue(m_gripUpgrade, m_ranges, m_gripValues);
		m_suspensionMultiplier = UpgradeLevelToValue(m_handlingUpgrade, m_ranges, m_suspensionValues);
		m_frontSpringForce = 10400f * m_suspensionMultiplier * -1f;
		m_rearSpringForce = 9600f * m_suspensionMultiplier * -1f;
		m_frontSpringDamp = UpgradeLevelToValue(m_handlingUpgrade, m_ranges, m_dampValues);
		m_rearSpringDamp = UpgradeLevelToValue(m_handlingUpgrade, m_ranges, m_dampValues);
		m_COGAdd = UpgradeLevelToValue(m_handlingUpgrade, m_ranges, m_COGValues);
		m_centerOfGravityOffset = new Vector2(0f, 16f + m_COGAdd);
		m_handlingForce = 0.001f;
		m_handlingAngle = UpgradeLevelToValue(m_handlingUpgrade, m_ranges, m_angleValues);
	}
}
