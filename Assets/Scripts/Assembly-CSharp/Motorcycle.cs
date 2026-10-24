using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Motorcycle : Vehicle, ISpeedBoost
{
	private const float TIRE_ELASTICITY = 0.15f;

	private const float FRONT_WHEEL_RAD = 34f;

	private const float FRONT_WHEEL_WEIGHT = 7f;

	private const float REAR_WHEEL_RAD = 38f;

	private const float REAR_WHEEL_WEIGHT = 7f;

	private const float CHASSIS_WEIGHT = 67f;

	private const float ALIEN_HEAD_WEIGHT = 5f;

	private const float TIRE_ANGULAR_DAMP = 0.997f;

	private const float CHASSIS_ANGULAR_DAMP = 0.97f;

	private const float SELF_BALANCE_FORCE = 40f;

	private const int MIN_RPM = 300;

	private const int MAX_RPM = 6000;

	public float[] m_tireForceValues = new float[5] { 0.7f, 1f, 1.2f, 1.275f, 1.3f };

	public float[] m_tireRateValues = new float[5] { 50f, 60f, 65f, 68f, 70f };

	public float[] m_suspensionValues = new float[5] { -1.2f, -1f, -0.9f, -0.87f, -0.85f };

	public float[] m_dampValues = new float[5] { 180f, 200f, 210f, 216f, 220f };

	public float[] m_handlingValues = new float[5] { 0.9f, 1.05f, 1.15f, 1.21f, 1.25f };

	public float[] m_gripValues = new float[5] { 0.7f, 1.01f, 1.15f, 1.25f, 1.3f };

	public float[] m_COGValues = new float[5] { -4.75f, -1.75f, 0f, 1f, 1.25f };

	public int[] m_ranges = new int[5] { 0, 5, 10, 15, 20 };

	public int m_powerUpgrade;

	public int m_handlingUpgrade;

	public int m_gripUpgrade;

	private float m_tireRate = 65f;

	private float m_frontSpringForce = 6500f;

	private float m_frontSpringDamp = 200f;

	private float m_rearSpringForce = 6500f;

	private float m_rearSpringDamp = 200f;

	private float m_tireGrip = 2.25f;

	public Vector2 m_frontTireForces = Vector2.zero;

	public Vector3 m_rearTireForces = new Vector2(1250000f, 600000f);

	private float m_handlingForce = 0.001f;

	private float m_tireForceMultiplier = 1f;

	private float m_suspensionMultiplier = -1f;

	private float m_COGAdd;

	public Vector2 UNIT_LINEAR_DAMP = new Vector2(0.997f, 0.997f);

	public uint m_group;

	public ChipmunkBodyC m_frontWheelBody;

	public ChipmunkBodyC m_rearWheelBody;

	public ucpCircleShape m_frontWheelShape;

	public ucpCircleShape m_rearWheelShape;

	public ChipmunkBodyC m_alienHead;

	public ChipmunkConstraintC m_frontWheelSpring;

	public ChipmunkConstraintC m_rearWheelSpring;

	public ChipmunkConstraintC m_balancerSpring;

	public TransformC m_chassisTC;

	public TransformC m_chassisBottomTC;

	public TransformC m_balancerTC;

	public TransformC m_rtBottomTC;

	public TransformC m_gearTC;

	public TransformC m_ftSuspenserTC;

	public TransformC m_rtSuspenserTC;

	public float m_gasAmount;

	public bool m_gasWasPressed;

	public bool m_brakeWasPressed;

	public int m_lastLeanDir;

	public int m_leanJumpTimer;

	public BoneModifier m_boneModifier;

	private static GameObject m_mainPrefab;

	private static GameObject m_debrisPrefab;

	public AutoGeometryBrush m_tireBrush;

	public string m_tireRollSoundName;

	private int m_headHitTimer;

	public ProjectorC m_playerShadow;

	public Transform m_gearTransform;

	private bool m_hasBrokenDown;

	private bool m_tiresBlown;

	private string m_suspensionUpgradeTier;

	private string m_powerUpgradeTier;

	public EffectBoost m_boostEffectFrontTire;

	public EffectBoost m_boostEffectRearTire;

	public TransformC m_boostEffectFrontTireTC;

	public TransformC m_boostEffectRearTireTC;

	public int m_airTime;

	public bool m_airTimeSoundTriggered;

	public int m_destructibleGroundContacts;

	public int m_skiddingEndTimer;

	public bool m_chassisHasPersistentContact;

	public int m_looseGripTimer;

	private int oldPressedDir;

	private float camTargetYOffset;

	private Vector2 m_centerOfGravityOffset;

	private int flipMult;

	private Vector3 suspenserOriginalRot;

	private float suspenserOriginalAngle;

	private ChipmunkConstraintC m_headJoint;

	private Texture m_mainMaterialTexture;

	private float m_boosterReferenceAngle;

	private float m_boosterBaseAngle;

	private float m_trickBoostAngle = (float)Math.PI / 4f;

	private bool m_leaningForward;

	private bool m_leaningBackward;

	private bool m_leaningCenter;

	private bool m_frontWheelSeparated;

	private bool m_canGiveBooster = true;

	private bool m_turnOver;

	private float m_turnOverDir;

	private float m_turnOverStarted;

	public int m_trickBoostTicks;

	public bool m_trickBoosting;

	public bool m_resourceBoosting;

	public float m_trickBoostForce;

	private Vector2 m_rearTireSurfaceNormal;

	public Motorcycle()
		: base(new GraphNode(GraphNodeType.Normal))
	{
	}

	public Motorcycle(GraphElement _graphElement)
		: base(_graphElement)
	{
		CalculateUpgrades();
		m_minigame = LevelManager.m_currentLevel as Minigame;
		m_tireBrush = new AutoGeometryBrush(1.7f, false, 0.25f);
		if (PsState.m_activeGameLoop.m_context == PsMinigameContext.Signal)
		{
			m_mainMaterialTexture = ResourceManager.GetTexture(RESOURCE.MotorcycleDifEarth_Texture2D);
		}
		else if (PsState.m_activeGameLoop.m_selectedVehicle == PsSelectedVehicle.CreatorVehicle)
		{
			m_mainMaterialTexture = ResourceManager.GetTexture(RESOURCE.MotorcycleDifRental_Texture2D);
		}
		else
		{
			m_mainMaterialTexture = GetTierTexture(PsMetagameManager.m_playerStats.GetUpgradeValue(GetType(), "tier", 1));
		}
		m_mainPrefab = ResourceManager.GetGameObject(RESOURCE.MotorcyclePrefab_GameObject);
		m_debrisPrefab = ResourceManager.GetGameObject(RESOURCE.NutsBoltsSpringsPrefab_GameObject);
		DisableCollidersRenderer(m_mainPrefab.transform);
		m_graphElement.m_isCopyable = false;
		m_graphElement.m_isRemovable = false;
		m_graphElement.m_isRotateable = false;
		float num = 0.45f;
		_graphElement.m_width = 130f * num;
		_graphElement.m_height = 50f * num;
		flipMult = ((!m_graphElement.m_flipped) ? 1 : (-1));
		m_group = GetGroup();
		Vector3 vector = new Vector3(0f, 5f, 0f);
		TransformC transformC = (m_chassisTC = TransformS.AddComponent(m_entity, _graphElement.m_name));
		TransformS.SetTransform(transformC, _graphElement.m_position - (Vector3)m_centerOfGravityOffset + vector, _graphElement.m_rotation);
		m_camTargetTC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(m_camTargetTC, _graphElement.m_position - new Vector3(1f, 17f) + vector, _graphElement.m_rotation);
		TransformS.ParentComponent(m_camTargetTC, m_chassisTC);
		ucpPolyShape[] array = ChipmunkProS.GeneratePolyShapesFromChildren(m_mainPrefab.transform.Find("DirtBike/DirtBikeColliders").gameObject, m_centerOfGravityOffset, 67f, 0.2f, 0.9f, (ucpCollisionType)3, 17895696u);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].group = m_group;
		}
		m_chassisBody = ChipmunkProS.AddDynamicBody(transformC, array, m_unitC);
		m_chassisPrefab = PrefabS.AddComponent(transformC, (Vector3)m_centerOfGravityOffset + new Vector3(0f, -3f, 0f), m_mainPrefab.transform.Find("DirtBike/Chassis").gameObject);
		m_chassisPrefab.p_gameObject.GetComponentInChildren<Renderer>().sharedMaterial.mainTexture = m_mainMaterialTexture;
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
		m_chassisDebrisParts = new string[3] { "FrontFender", "Light", "Feet" };
		CheckVisualUpgrade("Power", m_powerUpgrade);
		m_chassisBottomTC = TransformS.AddComponent(m_entity);
		TransformS.SetPosition(m_chassisBottomTC, transformC.transform.position + new Vector3(1 * flipMult, -7f, 0f));
		TransformS.ParentComponent(m_chassisBottomTC, m_chassisTC);
		ChipmunkProS.SetBodyLinearDamp(m_chassisBody, new Vector2(UNIT_LINEAR_DAMP.x, UNIT_LINEAR_DAMP.y));
		ChipmunkProS.SetBodyAngularDamp(m_chassisBody, 0.97f);
		m_gearTC = TransformS.AddComponent(m_entity, "Gear");
		Vector3 position = (Vector3)m_centerOfGravityOffset + m_chassisTC.transform.position;
		TransformS.SetTransform(m_gearTC, position, _graphElement.m_rotation);
		m_gearTransform = m_chassisPrefab.p_gameObject.transform.Find("Gear");
		float num2 = 34f * num;
		float num3 = 38f * num;
		Vector2 vector2 = m_centerOfGravityOffset + (Vector2)m_chassisTC.transform.position + new Vector2(_graphElement.m_width * 0.54f * (float)flipMult, (0f - _graphElement.m_height) * 0.8f - num2 * 0.5f);
		m_frontWheelBody = createTire(m_entity, m_chassisBody, vector2, num2, 7f, true);
		m_frontWheelBody.customComponent = m_unitC;
		TransformC grooveA = TransformS.AddComponent(m_entity, "FrontWheelJoint1", vector2 + new Vector2(-6.5f, 18.5f));
		TransformC grooveB = TransformS.AddComponent(m_entity, "FrontWheelJoint2", vector2);
		TransformC anchor = TransformS.AddComponent(m_entity, "FrontWheelJoint3", vector2);
		ChipmunkProS.AddGrooveJoint(m_chassisBody, m_frontWheelBody, grooveA, grooveB, anchor);
		grooveA = TransformS.AddComponent(m_entity, "FrontWheelSpringAnchor1", vector2);
		grooveB = TransformS.AddComponent(m_entity, "FrontWheelSpringAnchor2", vector2);
		m_frontWheelSpring = ChipmunkProS.AddDampedSpring(m_chassisBody, m_frontWheelBody, grooveA, grooveB, 0f, m_frontSpringForce, m_frontSpringDamp);
		int num4 = Mathf.FloorToInt((float)m_handlingUpgrade / 5f);
		m_suspensionUpgradeTier = "T" + num4;
		while (m_mainPrefab.transform.Find("Parts/SuspensionFront" + m_suspensionUpgradeTier) == null && num4 != 0)
		{
			num4--;
			m_suspensionUpgradeTier = "T" + num4;
		}
		int num5 = Mathf.FloorToInt((float)m_powerUpgrade / 5f);
		m_powerUpgradeTier = "T" + num5;
		while (m_mainPrefab.transform.Find("DirtBike/Chassis/Power" + m_powerUpgradeTier + "/Exhaust" + m_powerUpgradeTier) == null && num5 != 0)
		{
			num5--;
			m_powerUpgradeTier = "T" + num5;
		}
		m_ftSuspenserTC = TransformS.AddComponent(m_entity, "FtSuspenser");
		TransformS.SetTransform(m_ftSuspenserTC, vector2, Vector3.forward * 22.5f);
		PrefabS.AddComponent(m_ftSuspenserTC, new Vector3(0f, 8.5f, 0f), m_mainPrefab.transform.Find("Parts/SuspensionFront" + m_suspensionUpgradeTier).gameObject);
		m_ftSuspenserTC.transform.GetChild(0).name = "SuspensionFront" + m_suspensionUpgradeTier;
		Vector2 vector3 = m_centerOfGravityOffset + (Vector2)m_chassisTC.transform.position + new Vector2((0f - _graphElement.m_width) * 0.5f * (float)flipMult, (0f - _graphElement.m_height) * 0.7f - num3 * 0.5f);
		m_rearWheelBody = createTire(m_entity, m_chassisBody, vector3, num3, 7f, false);
		m_rearWheelBody.customComponent = m_unitC;
		grooveA = TransformS.AddComponent(m_entity, "RearWheelJoint1", m_gearTC.transform.position);
		grooveB = TransformS.AddComponent(m_entity, "RearWheelJoint2", vector3);
		ChipmunkProS.AddPinJoint(m_chassisBody, m_rearWheelBody, grooveA, grooveB);
		TransformS.ParentComponent(grooveA, m_chassisTC);
		TransformS.ParentComponent(grooveB, m_chassisTC);
		grooveA = TransformS.AddComponent(m_entity, "RearWheelSpringAnchor1", vector3);
		grooveB = TransformS.AddComponent(m_entity, "RearWheelSpringAnchor2", vector3);
		m_rearWheelSpring = ChipmunkProS.AddDampedSpring(m_chassisBody, m_rearWheelBody, grooveA, grooveB, 0f, m_rearSpringForce, m_rearSpringDamp);
		Vector2 vector4 = m_centerOfGravityOffset + (Vector2)m_chassisTC.transform.position + new Vector2(2f, -8f);
		m_rtSuspenserTC = TransformS.AddComponent(m_entity, "RtSuspenser");
		TransformS.SetTransform(m_rtSuspenserTC, vector4, Vector3.forward * Vector3.Angle(m_chassisTC.transform.up, vector3 - vector4));
		PrefabS.AddComponent(m_rtSuspenserTC, Vector3.zero, m_mainPrefab.transform.Find("Parts/SuspensionRear" + m_suspensionUpgradeTier).gameObject);
		m_rtSuspenserTC.transform.GetChild(0).name = "SuspensionRear" + m_suspensionUpgradeTier;
		float num6 = 1f;
		if (m_graphElement.m_flipped)
		{
			num6 = -1f;
		}
		m_rtBottomTC = TransformS.AddComponent(m_entity);
		TransformS.SetPosition(m_rtBottomTC, m_rearWheelBody.TC.transform.position + new Vector3((0f - num6) * num3 * 0.35f, (0f - num3) * 0.33f, 0f));
		TransformS.ParentComponent(m_rtBottomTC, m_chassisTC);
		TransformS.ParentComponent(m_ftSuspenserTC, m_chassisTC);
		TransformS.ParentComponent(m_rtBottomTC, m_chassisTC);
		TransformS.ParentComponent(m_rtSuspenserTC, m_chassisTC);
		InitMotors(m_rearWheelBody, m_frontWheelBody);
		SetMotorParameters(m_frontTireForces, m_tireRate, m_rearTireForces, m_tireRate);
		if (!m_minigame.m_editing)
		{
			SetEngineSound(m_chassisTC, "/InGame/Vehicles/DirtBikeEngine", 300);
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
			ChipmunkProS.AddCollisionHandler(m_frontWheelBody, TireCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)2, true, true, true);
			ChipmunkProS.AddCollisionHandler(m_frontWheelBody, TireCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)4, true, true, true);
			ChipmunkProS.AddCollisionHandler(m_rearWheelBody, TireCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)2, true, true, true);
			ChipmunkProS.AddCollisionHandler(m_rearWheelBody, TireCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)4, true, true, true);
			ChipmunkProS.AddCollisionHandler(m_chassisBody, ChassisCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)2, true, true, true);
			ChipmunkProS.AddCollisionHandler(m_chassisBody, ChassisCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)4, true, true, true);
		}
		else
		{
			m_minigame.SetPlayerNode(this);
		}
		m_playerShadow = ProjectorS.AddComponent(m_chassisTC, ResourceManager.GetMaterial(RESOURCE.IngameShadow_Material), 256, new Vector3(0f, 40f));
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
		Vector3 vector5 = m_mainPrefab.transform.Find("DirtBike/CharacterLocator").localPosition + (Vector3)m_centerOfGravityOffset + new Vector3(0f, -3f, 0f);
		if (PsState.m_activeGameLoop.m_context == PsMinigameContext.Signal)
		{
			m_alienCharacter = new AlienCharacter(_graphElement, "Drive", "HumanPrefab_GameObject");
		}
		else
		{
			m_alienCharacter = new AlienCharacter(_graphElement, "DriveMoto");
		}
		TransformS.SetGlobalPosition(m_alienCharacter.m_mainTC, m_chassisTC.transform.position + vector5);
		TransformS.ParentComponent(m_alienCharacter.m_mainTC, m_chassisTC);
		if (PsState.m_activeGameLoop.m_context != PsMinigameContext.Signal)
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
		Vector2 vector6 = (Vector2)m_chassisTC.transform.position + new Vector2(10f, 38f);
		Vector2 offset = new Vector2(0f, 10f);
		TransformC transformC2 = TransformS.AddComponent(m_entity, "Alien");
		TransformS.SetTransform(transformC2, vector6, Vector2.zero);
		Transform transform = ToolBox.SearchHierarchyForBone(m_alienCharacter.m_mainPC.p_gameObject.transform, "HeadCollider");
		ucpPolyShape ucpPolyShape2 = ChipmunkProS.GeneratePolyShapeFromGameObject(transform.gameObject, offset, 5f, 0.2f, 0.5f, (ucpCollisionType)3, 17895696u, true);
		ucpPolyShape2.group = m_group;
		m_alienHead = ChipmunkProS.AddDynamicBody(transformC2, ucpPolyShape2, m_unitC);
		m_boneModifier = transformC2.transform.gameObject.AddComponent<BoneModifier>();
		m_boneModifier.AddChipmunkModifier(m_alienHead, ToolBox.SearchHierarchyForBone(m_alienCharacter.m_mainPC.p_gameObject.transform, "Spine2"), 0.5f, m_chassisTC.transform);
		m_boneModifier.AddChipmunkModifier(m_alienHead, ToolBox.SearchHierarchyForBone(m_alienCharacter.m_mainPC.p_gameObject.transform, "Neck"), 0.5f, m_chassisTC.transform);
		m_boneModifier.AddChipmunkModifier(m_alienHead, ToolBox.SearchHierarchyForBone(m_alienCharacter.m_mainPC.p_gameObject.transform, "Head"), 0.5f, m_chassisTC.transform);
		grooveA = TransformS.AddComponent(m_entity, "AlienJoint1", vector6 + new Vector2(-13f, -9f));
		ChipmunkProS.AddPivotJoint(m_chassisBody, m_alienHead, grooveA);
		ChipmunkProS.AddRotaryLimitJoint(m_chassisBody, m_alienHead, grooveA, -(float)Math.PI / 12f, 0.6981317f);
		m_headJoint = ChipmunkProS.AddDampedRotarySpring(m_chassisBody, m_alienHead, grooveA, 0f, 2500000f, 45000f);
		if (!m_minigame.m_editing)
		{
			ChipmunkProS.AddCollisionHandler(m_alienHead, AlienCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)2, true, false, true);
			ChipmunkProS.AddCollisionHandler(m_alienHead, AlienCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)4, true, false, true);
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
		if (!m_minigame.m_editing)
		{
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
			{
				Kill(DamageType.Impact, float.MaxValue);
			}
			if (!m_tiresBlown && m_entity != null)
			{
				TransformS.SetGlobalTransform(m_ftSuspenserTC, m_frontWheelBody.TC.transform.position, m_chassisTC.transform.rotation.eulerAngles + Vector3.forward * 22.5f);
				TransformS.SetGlobalRotation(m_rtSuspenserTC, Vector3.forward * (Vector3.Angle(m_chassisTC.transform.up, m_rearWheelBody.TC.transform.position - m_rtSuspenserTC.transform.position) + m_chassisTC.transform.rotation.eulerAngles.z));
				if (m_gearTransform != null)
				{
					m_gearTransform.rotation = m_rearWheelBody.TC.transform.rotation;
				}
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
				int num2 = 0;
				bool flag7 = m_contactState == ContactState.OnAir;
				bool flag8 = GetContactState(m_rearWheelBody) == ContactState.OnContact;
				bool flag9 = GetContactState(m_frontWheelBody) == ContactState.OnContact;
				bool flag10 = !flag9 && !flag8;
				bool flag11 = flag9 && flag8;
				if (flag7)
				{
					m_airTime++;
				}
				else
				{
					m_airTime = 0;
					m_airTimeSoundTriggered = false;
				}
				flag2 = Controller.GetButtonState("Throttle") == ControllerButtonState.ON;
				flag = Controller.GetButtonState("Reverse") == ControllerButtonState.ON;
				flag6 = Controller.GetButtonState("BoostButton") == ControllerButtonState.ON;
				float num3 = 1f;
				if (m_minigame.m_gameStarted)
				{
					flag3 = ((!m_graphElement.m_flipped) ? (Controller.GetButtonState("LeftButton1") == ControllerButtonState.ON) : (Controller.GetButtonState("LeftButton2") == ControllerButtonState.ON));
					flag4 = ((!m_graphElement.m_flipped) ? (Controller.GetButtonState("LeftButton2") == ControllerButtonState.ON) : (Controller.GetButtonState("LeftButton1") == ControllerButtonState.ON));
					if (m_looseGripTimer > 0)
					{
						if (!flag)
						{
							num3 = 0.1f;
						}
						m_looseGripTimer--;
					}
					if (flag3)
					{
						num2 = -1;
					}
					else if (flag4)
					{
						num2 = 1;
					}
				}
				if (m_contactState == ContactState.OnAir)
				{
					float num4 = ChipmunkProWrapper.ucpBodyGetAngle(m_chassisBody.body);
					float num5 = (float)Math.PI * 2f;
					if (m_canGiveBooster)
					{
						float num6 = Mathf.Abs(m_boosterReferenceAngle - num4);
						if (ToolBox.IsBetween(Mathf.Abs(m_boosterReferenceAngle - m_boosterBaseAngle), (float)Math.PI - m_trickBoostAngle, (float)Math.PI + m_trickBoostAngle))
						{
							if (num6 > m_trickBoostAngle)
							{
								TrickBoost(1);
								if (flag3)
								{
									PsAchievementManager.IncrementProgress("hundredBackFlips", 1);
								}
								m_canGiveBooster = false;
							}
						}
						else if (Mathf.Abs(num4 - m_boosterBaseAngle) >= (float)Math.PI)
						{
							TrickBoost(1);
							if (flag3)
							{
								PsAchievementManager.IncrementProgress("hundredBackFlips", 1);
							}
							m_canGiveBooster = false;
						}
					}
					else if (Mathf.Abs(num4 % num5 * num5) < m_trickBoostAngle)
					{
						m_canGiveBooster = true;
						m_boosterReferenceAngle = (float)Mathf.RoundToInt(num4 / num5) * num5;
						m_boosterBaseAngle = m_boosterReferenceAngle;
					}
				}
				if (m_booster != null && flag6)
				{
					m_booster.Use(this);
				}
				if (num2 != m_lastLeanDir && flag11 && m_leanJumpTimer == 0)
				{
					m_leanJumpTimer = 4;
				}
				m_lastLeanDir = num2;
				if (m_graphElement.m_flipped)
				{
					if (flag2 && !m_gasWasPressed)
					{
						SoundS.PlaySingleShotWithParameter("/InGame/Vehicles/BrakePressed", Vector3.zero, "Load", m_currentLoad);
					}
					if (flag && !m_brakeWasPressed)
					{
						SoundS.PlaySingleShotWithParameter("/InGame/Vehicles/GasPressed_DirtBike", Vector3.zero, "Load", m_currentLoad);
					}
				}
				else
				{
					if (flag2 && !m_gasWasPressed)
					{
						SoundS.PlaySingleShotWithParameter("/InGame/Vehicles/GasPressed_DirtBike", Vector3.zero, "Load", m_currentLoad);
					}
					if (flag && !m_brakeWasPressed)
					{
						SoundS.PlaySingleShotWithParameter("/InGame/Vehicles/BrakePressed", Vector3.zero, "Load", m_currentLoad);
					}
				}
				if (flag3)
				{
					num = -1;
				}
				else if (flag4)
				{
					num = 1;
				}
				if (num != oldPressedDir)
				{
					flag5 = true;
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
				m_brakeWasPressed = flag;
				m_gasWasPressed = flag2;
				oldPressedDir = num;
				Vector2 velocityRelativeToContacts = GetVelocityRelativeToContacts(m_rearWheelBody);
				bool flag12 = GetRogueContactsCount(m_rearWheelBody, false) > 0;
				float num7 = 1f - ToolBox.limitBetween(Mathf.Abs(velocityRelativeToContacts.x) / 600f, 0f, 0.6f);
				float num8 = m_tireRate;
				Vector2 frontForces = m_frontTireForces;
				bool flag13 = false;
				if (!flag10)
				{
					if (flag12 && !flag2 && !flag && velocityRelativeToContacts.magnitude < 80f)
					{
						float num9 = 1f - ToolBox.getPositionBetween(velocityRelativeToContacts.magnitude, 0f, 80f);
						num8 = 0f;
						num7 = 0.4f * num9;
						frontForces = m_rearTireForces * num7 * 0.66f;
						flag13 = true;
					}
					if (flag)
					{
						num8 = m_tireRate / 2f;
						if (velocityRelativeToContacts.x > 0f && ChipmunkProWrapper.ucpBodyGetAngVel(m_frontWheelBody.body) < 0f)
						{
							SetFrontBrake(true);
						}
						else
						{
							SetFrontBrake(false);
						}
					}
					else if (flag2)
					{
						if (velocityRelativeToContacts.x < 0f && ChipmunkProWrapper.ucpBodyGetAngVel(m_frontWheelBody.body) > 0f)
						{
							SetFrontBrake(true);
						}
						else
						{
							SetFrontBrake(false);
						}
					}
					else
					{
						SetFrontBrake(false);
					}
				}
				m_boostEffectFrontTireTC.forcedRotation = Quaternion.Euler(Vector3.forward * m_chassisTC.transform.eulerAngles.z);
				m_boostEffectRearTireTC.forcedRotation = Quaternion.Euler(Vector3.forward * m_chassisTC.transform.eulerAngles.z);
				m_boostEffectFrontTireTC.updateRotation = true;
				m_boostEffectRearTireTC.updateRotation = true;
				SetMotorParameters(frontForces, num8, m_rearTireForces * num7, num8);
				m_alienCharacter.AnimateCharacter("DriveDir", (!flag || !(velocityRelativeToContacts.x < 10f)) ? 1 : (-1));
				if (flag2 && !flag)
				{
					if (flag3)
					{
						m_gasAmount = ToolBox.limitBetween(m_gasAmount + 0.2f, 0f, 1f);
					}
					else
					{
						m_gasAmount = ToolBox.limitBetween(m_gasAmount + 0.01f, 0f, 1f);
					}
				}
				else if (flag && !flag2)
				{
					m_gasAmount = ToolBox.limitBetween(m_gasAmount - 0.05f, -1f, 0f);
				}
				else
				{
					m_gasAmount *= 0.9f;
				}
				if (!flag13)
				{
					UpdateMotors((!flag2 && !flag) ? 0f : (m_gasAmount * m_trickBoostForce), !m_graphElement.m_flipped, m_trickBoostForce * num3, true);
				}
				else
				{
					UpdateMotors(1f, !m_graphElement.m_flipped, 1f, true);
				}
				Vector2 vector = ChipmunkProWrapper.ucpBodyGetPos(m_frontWheelBody.body);
				Vector2 vector2 = ChipmunkProWrapper.ucpBodyGetPos(m_rearWheelBody.body);
				if (flag3 || flag4)
				{
					m_alienCharacter.AnimateCharacter("LeanDir", (!flag3) ? 1 : (-1));
					if (m_boneModifier != null)
					{
						m_boneModifier.m_globalWeight = Mathf.Lerp(m_boneModifier.m_globalWeight, 0f, 0.08f);
					}
					if (flag3)
					{
						float value = Mathf.Lerp(ChipmunkProWrapper.ucpDampedRotarySpringGetRestAngle(m_headJoint.constraint), -0.61086524f, 0.08f);
						ChipmunkProWrapper.ucpDampedRotarySpringSetRestAngle(m_headJoint.constraint, value);
					}
					else
					{
						float num10 = Mathf.Lerp(ChipmunkProWrapper.ucpDampedRotarySpringGetRestAngle(m_headJoint.constraint), 0.17453292f, 0.08f);
						ChipmunkProWrapper.ucpDampedRotarySpringSetRestAngle(m_headJoint.constraint, 0.17453292f);
					}
					if (flag3 && !m_leaningBackward && !flag4)
					{
						m_leaningBackward = true;
						m_leaningForward = false;
						m_leaningCenter = false;
						if (flag9)
						{
							ChipmunkProWrapper.ucpBodyApplyImpulseAtWorldPoint(m_chassisBody.body, m_chassisTC.transform.up * 4000f, vector);
							ChipmunkProWrapper.ucpBodyApplyImpulseAtWorldPoint(m_chassisBody.body, m_chassisTC.transform.up * -1000f, vector2);
						}
					}
					else if (flag4 && !m_leaningForward && !flag3)
					{
						m_leaningBackward = false;
						m_leaningForward = true;
						m_leaningCenter = false;
					}
					if (m_contactState == ContactState.OnContact)
					{
						float magnitude = ChipmunkProWrapper.ucpBodyGetVel(m_chassisBody.body).magnitude;
						float num11 = ChipmunkProWrapper.ucpBodyGetAngVel(m_chassisBody.body);
						ucpSegmentQueryInfo info2 = default(ucpSegmentQueryInfo);
						ucpSegmentQueryInfo info3 = default(ucpSegmentQueryInfo);
						filter.ucpShapeFilterAll();
						filter.group = m_group;
						ChipmunkProWrapper.ucpSpaceSegmentQueryFirst(vector, vector + (Vector2)m_chassisTC.transform.up * -100f, 1f, filter, ref info2);
						ChipmunkProWrapper.ucpSpaceSegmentQueryFirst(vector2, vector2 + (Vector2)m_chassisTC.transform.up * -100f, 1f, filter, ref info3);
						float num12 = 1f - info2.alpha * info2.alpha;
						float num13 = 1f - info3.alpha * info3.alpha;
						if (flag4 && flag8 && !flag9)
						{
							if (num11 > 0f)
							{
								num11 *= Math.Min(120f / magnitude, 1f);
							}
							ChipmunkProWrapper.ucpBodySetAngVel(m_chassisBody.body, num11 - Math.Min(200f / magnitude, 0.5f));
							ChipmunkProWrapper.ucpBodyApplyImpulseAtWorldPoint(m_chassisBody.body, m_chassisTC.transform.up * -600f * num12 * Math.Min(120f / magnitude, 1f), vector2);
						}
						else if (flag3 && flag8 && !flag9)
						{
							if (num11 < 0f && !flag)
							{
								num11 *= Math.Min(120f / magnitude, 1f);
							}
							ChipmunkProWrapper.ucpBodySetAngVel(m_chassisBody.body, num11 + Math.Min(magnitude / 3000f, 0.3f));
						}
						else if (flag3 && !flag8 && !flag9)
						{
							ChipmunkProWrapper.ucpBodySetAngVel(m_chassisBody.body, num11 + 0.3f);
						}
						if (m_leaningForward && flag9 && flag8)
						{
							ChipmunkProWrapper.ucpBodyApplyImpulseAtWorldPoint(m_chassisBody.body, m_chassisTC.transform.up * -500f * num13, vector);
							ChipmunkProWrapper.ucpBodyApplyImpulseAtWorldPoint(m_chassisBody.body, m_chassisTC.transform.up * -100f * num12, vector2);
						}
						if (m_leaningBackward && flag9)
						{
							ChipmunkProWrapper.ucpBodyApplyImpulseAtWorldPoint(m_chassisBody.body, m_chassisTC.transform.up * -300f * num12, vector2);
						}
						if (m_chassisHasPersistentContact)
						{
							if (m_leaningForward)
							{
								TurnOver(1f);
							}
							if (m_leaningBackward)
							{
								TurnOver(-1f);
							}
						}
						if (flag8 && !flag9)
						{
							ChipmunkProWrapper.ucpBodySetAngVel(m_chassisBody.body, Mathf.Clamp(ChipmunkProWrapper.ucpBodyGetAngVel(m_chassisBody.body), -1.8f, 1.8f));
						}
						else if (!flag8 && flag9)
						{
							ChipmunkProWrapper.ucpBodySetAngVel(m_chassisBody.body, Mathf.Clamp(ChipmunkProWrapper.ucpBodyGetAngVel(m_chassisBody.body), -1.8f, 4f));
						}
						else if (!flag8 && !flag9)
						{
							ChipmunkProWrapper.ucpBodySetAngVel(m_chassisBody.body, Mathf.Clamp(ChipmunkProWrapper.ucpBodyGetAngVel(m_chassisBody.body), -5.5f, 5.5f));
						}
					}
					if (m_turnOver)
					{
						float num14 = (m_turnOverStarted + 0.5f - Main.m_resettingGameTime) * 2f;
						if (num14 > 0f)
						{
							ChipmunkProWrapper.ucpBodyApplyImpulseAtWorldPoint(m_chassisBody.body, m_chassisTC.transform.up * -500f * m_turnOverDir * num14, vector);
							ChipmunkProWrapper.ucpBodyApplyImpulseAtWorldPoint(m_chassisBody.body, m_chassisTC.transform.up * 500f * m_turnOverDir * num14, vector2);
						}
						else
						{
							m_turnOver = false;
						}
					}
					if (m_contactState == ContactState.OnAir || m_groundedChainIndex == -1)
					{
						float num15 = ChipmunkProWrapper.ucpBodyGetAngVel(m_chassisBody.body);
						if (flag3 && !flag4)
						{
							if (num15 < 0f)
							{
								num15 = 0f;
								ChipmunkProWrapper.ucpBodySetAngVel(m_chassisBody.body, num15);
							}
							num15 += 0.2f;
						}
						else if (flag4 && !flag3)
						{
							if (num15 > 0f)
							{
								num15 = 0f;
								ChipmunkProWrapper.ucpBodySetAngVel(m_chassisBody.body, num15);
							}
							num15 -= 0.2f;
						}
						num15 = ToolBox.limitBetween(num15, -5.5f, 5.5f);
						if (Contact(CMBIdentifiers.Airflow))
						{
							Vector2 vector3 = ChipmunkProWrapper.ucpBodyGetVel(m_chassisBody.body);
							if ((num15 < 0f && vector3.x < 350f) || (num15 > 0f && vector3.x > -350f))
							{
								ChipmunkProWrapper.ucpBodyApplyImpulseAtWorldPoint(m_chassisBody.body, Vector2.right * 15f * (0f - num15), m_chassisTC.transform.position);
							}
						}
						ChipmunkProWrapper.ucpBodySetAngVel(m_chassisBody.body, num15);
					}
				}
				else if (!m_leaningCenter)
				{
					m_leaningBackward = false;
					m_leaningForward = false;
					m_alienCharacter.AnimateCharacter("LeanDir", 0);
					if (m_boneModifier != null)
					{
						m_boneModifier.m_globalWeight = Mathf.Lerp(m_boneModifier.m_globalWeight, 1f, 0.08f);
					}
					float num16 = Mathf.Lerp(ChipmunkProWrapper.ucpDampedRotarySpringGetRestAngle(m_headJoint.constraint), 0f, 0.08f);
					ChipmunkProWrapper.ucpDampedRotarySpringSetRestAngle(m_headJoint.constraint, num16);
					if (num16 == 0f && m_boneModifier != null && m_boneModifier.m_globalWeight == 1f)
					{
						m_leaningCenter = true;
					}
				}
				bool flag14 = false;
				if (m_destructibleGroundContacts > 0 && (!flag2 || !flag) && (flag2 || flag))
				{
					flag14 = true;
					m_skiddingEndTimer = 15;
					if (Main.m_gameTicks % 2 == 0)
					{
						SkidTire(m_rtBottomTC, m_rearWheelBody);
					}
				}
				if (m_skiddingEndTimer > 0)
				{
					m_skiddingEndTimer--;
				}
				if (flag10 && m_drivingFx != null)
				{
					PauseDriveFx();
				}
				else if (m_drivingFx != null)
				{
					ParticleSystem component = m_drivingFx.p_gameObject.GetComponent<ParticleSystem>();
					if (m_drivingFxName.Equals("MudSplatter_GameObject"))
					{
						if (!flag14 && m_skiddingEndTimer == 0 && component.isPlaying)
						{
							component.Stop();
						}
						else if (flag14 && component.isStopped)
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
				if (m_engineSound != null)
				{
					float val = Mathf.Abs(ChipmunkProWrapper.ucpBodyGetAngVel(m_rearWheelBody.body));
					float positionBetween = ToolBox.getPositionBetween(val, 0f, 50f);
					float targetRPM = ((!flag2 && !flag) ? 300f : Mathf.Lerp(300f, 6000f, positionBetween));
					float targetLoad = ((!flag10) ? 1 : 0);
					UpdateEngineSound(targetRPM, targetLoad);
				}
				if (m_tireRollSound != null)
				{
					if (flag10)
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
					if ((m_gasWasPressed || m_brakeWasPressed) && flag8)
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
						float num18 = UpgradeLevelToValue(m_handlingUpgrade, m_ranges, m_suspensionValues);
						float value2 = 10400f * m_suspensionMultiplier * -1f;
						float value3 = 9600f * m_suspensionMultiplier * -1f;
						float value4 = UpgradeLevelToValue(m_handlingUpgrade, m_ranges, m_dampValues);
						float value5 = UpgradeLevelToValue(m_handlingUpgrade, m_ranges, m_dampValues);
						ChipmunkProWrapper.ucpConstraintSetMaxForce(m_frontWheelSpring.constraint, value2);
						ChipmunkProWrapper.ucpDampedSpringSetDamping(m_frontWheelSpring.constraint, value4);
						ChipmunkProWrapper.ucpConstraintSetMaxForce(m_rearWheelSpring.constraint, value3);
						ChipmunkProWrapper.ucpDampedSpringSetDamping(m_rearWheelSpring.constraint, value5);
						float value6 = 2.25f * UpgradeLevelToValue(m_gripUpgrade, m_ranges, m_gripValues);
						ChipmunkProWrapper.ucpShapeSetFriction(m_frontWheelShape.shapePtr, value6);
						ChipmunkProWrapper.ucpShapeSetFriction(m_rearWheelShape.shapePtr, value6);
						RemoveBoostSound();
						m_boostEffectFrontTire.DropBoost();
						m_boostEffectRearTire.DropBoost();
						m_trickBoosting = (m_resourceBoosting = false);
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
		m_frontWheelSeparated = false;
		m_chassisHasPersistentContact = false;
	}

	private void TurnOver(float _dir)
	{
		m_turnOverDir = _dir;
		if (!m_turnOver)
		{
			m_turnOver = true;
			m_turnOverStarted = Main.m_resettingGameTime;
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

	public bool SkidTire(TransformC _tc, ChipmunkBodyC _body)
	{
		float num = ChipmunkProWrapper.ucpBodyGetAngVel(_body.body);
		Vector2 pos = _tc.transform.position;
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

	public ChipmunkBodyC createTire(Entity _e, ChipmunkBodyC _chassis, Vector2 _pos, float _rad, float _weight, bool _front)
	{
		TransformC transformC = TransformS.AddComponent(_e, "Bike Tire");
		TransformC transformC2 = TransformS.AddComponent(_e, "Effect");
		transformC2.forceRotation = true;
		TransformS.ParentComponent(transformC2, transformC, Vector3.zero);
		TransformS.SetTransform(transformC, _pos, Vector2.zero);
		ucpCircleShape ucpCircleShape2 = new ucpCircleShape(_rad, Vector2.zero, 17895696u, _weight, 0.15f, m_tireGrip, (ucpCollisionType)3);
		ucpCircleShape2.group = m_group;
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.AddDynamicBody(transformC, ucpCircleShape2);
		if (_front)
		{
			PrefabC prefabC = PrefabS.AddComponent(transformC2, Vector3.forward * -0.8f, ResourceManager.GetGameObject(RESOURCE.EffectBoostMotorcycleFront_GameObject), "BoostEffect");
			m_boostEffectFrontTire = prefabC.p_gameObject.GetComponent<EffectBoost>();
			m_boostEffectFrontTireTC = transformC2;
		}
		else
		{
			PrefabC prefabC2 = PrefabS.AddComponent(transformC2, Vector3.forward * -0.8f, ResourceManager.GetGameObject(RESOURCE.EffectBoostMotorcycleBack_GameObject), "BoostEffect");
			m_boostEffectRearTire = prefabC2.p_gameObject.GetComponent<EffectBoost>();
			m_boostEffectRearTireTC = transformC2;
		}
		int num = Mathf.FloorToInt((float)m_gripUpgrade / 5f);
		string text = "T" + num;
		while (m_mainPrefab.transform.Find("Parts/BikeTireFront" + text) == null)
		{
			num--;
			text = "T" + num;
			if (num == 0)
			{
				break;
			}
		}
		if (_front)
		{
			PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 0f), m_mainPrefab.transform.Find("Parts/BikeTireFront" + text).gameObject);
			m_frontWheelShape = ucpCircleShape2;
		}
		else
		{
			PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 0f), m_mainPrefab.transform.Find("Parts/BikeTireBack" + text).gameObject);
			m_rearWheelShape = ucpCircleShape2;
		}
		ChipmunkProS.SetBodyLinearDamp(chipmunkBodyC, new Vector2(UNIT_LINEAR_DAMP.x, UNIT_LINEAR_DAMP.y));
		ChipmunkProS.SetBodyAngularDamp(chipmunkBodyC, 0.997f);
		TransformS.ParentComponent(transformC, m_chassisTC);
		return chipmunkBodyC;
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
		Debug.LogWarning("TRICK BOOST x " + _tricks);
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
			float value5 = 2.25f * UpgradeLevelToValue(20, m_ranges, m_gripValues);
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
		if (_contactInfo.m_contactBody.body == m_rearWheelBody.body)
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
		if (!(_contactInfo.m_contactBody.body == m_rearWheelBody.body))
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
		if (m_isDead || m_hasBrokenDown)
		{
			return;
		}
		if (_phase == ucpCollisionPhase.Begin && Mathf.Abs(_pair.impulse.normalized.y) > 0.5f)
		{
			float positionBetween = ToolBox.getPositionBetween(_pair.impulse.magnitude, 1600f, 10000f);
			if (positionBetween > 0f)
			{
				SoundS.PlaySingleShotWithParameter("/InGame/Vehicles/TireImpact_DirtBike", Vector3.zero, "Force", positionBetween);
			}
		}
		if (_phase != ucpCollisionPhase.Separate)
		{
			ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexA];
			if (chipmunkBodyC == m_rearWheelBody)
			{
				m_rearTireSurfaceNormal = _pair.normal;
			}
			else if (chipmunkBodyC == m_frontWheelBody)
			{
				m_frontWheelSeparated = true;
			}
		}
	}

	private void ChassisCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		if (m_isDead || m_hasBrokenDown)
		{
			return;
		}
		if (_phase == ucpCollisionPhase.Begin)
		{
			float positionBetween = ToolBox.getPositionBetween(_pair.impulse.magnitude, 2500f, 10000f);
			if (positionBetween > 0f)
			{
				SoundS.PlaySingleShotWithParameter("/InGame/Vehicles/ChassisImpact", Vector3.zero, "Force", positionBetween);
				if (positionBetween > 0.8f)
				{
					CreateRandomImpactDebris(m_chassisDebrisParts, m_debrisPrefab);
				}
			}
		}
		if (_phase != ucpCollisionPhase.Persist)
		{
			return;
		}
		m_chassisHasPersistentContact = true;
		if (Mathf.Abs(Mathf.DeltaAngle(m_chassisTC.transform.rotation.eulerAngles.z, 0f)) > 60f && !ChipmunkProWrapper.ucpShapeGetSensor(_pair.shapeB))
		{
			ChipmunkBodyShape bodyShapeByTag = ChipmunkProS.GetBodyShapeByTag(m_chassisBody, "DirtBikeColliderRear");
			if (_pair.shapeA == bodyShapeByTag.shapePtr && m_looseGripTimer < 50)
			{
				m_looseGripTimer += 5;
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
		Vector2 zero = Vector2.zero;
		if (m_alienHead != null)
		{
			zero += ChipmunkProWrapper.ucpBodyGetVel(m_alienHead.body);
		}
		if (m_alienCharacter != null)
		{
			m_alienCharacter.ThrowHelmet(zero, 0f, false, 17895696u, false, true);
		}
	}

	public override void EmergencyKill()
	{
		if (m_entity != null)
		{
			Debug.Log("EMERGENCY KILL MOTORCYCLE");
			ExplosionEffect(m_chassisTC.transform.position);
			Kill(DamageType.Impact, float.MaxValue);
			Debris.CreateDebrisFromChildren(m_chassisPrefab.p_gameObject.transform, m_lastFrameVelocity * 0.7f, new Vector2(2000f, 2000f), 2000f, true, m_group);
			RemoveAssembledEntity(m_entity);
			m_entity = null;
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

	public override void LaunchRagdoll(bool _explodeBones = false)
	{
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
		ChipmunkProS.RemoveBody(m_alienHead);
		m_alienHead = null;
		ChipmunkBodyShape bodyShapeByTag = ChipmunkProS.GetBodyShapeByTag(m_chassisBody, "DirtBikeColliderMiddle");
		ChipmunkProWrapper.ucpShapeSetGroup(bodyShapeByTag.shapePtr, m_alienCharacter.m_group);
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
			SoundS.PlaySingleShot("/Ingame/Characters/AlienDisappointment", Vector3.zero);
			switch (_damageType)
			{
			case DamageType.Weapon:
				SoundS.PlaySingleShot("/Ingame/Characters/AlienDeathScream", Vector3.zero);
				break;
			case DamageType.Electric:
				SoundS.PlaySingleShot("/Ingame/Characters/AlienElectrocution", Vector2.zero);
				break;
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
		if (!m_minigame.m_gameEnded)
		{
			PsState.m_activeMinigame.TweenTimeScale(0.05f, TweenStyle.ExpoIn, 0.1f, delegate
			{
				PsState.m_activeMinigame.TweenTimeScale(1f, TweenStyle.ExpoIn, 0.25f);
			}, 0.5f);
		}
		SoundS.PlaySingleShot("/InGame/Vehicles/OffroadCarDeath", Vector3.zero);
		ChipmunkProS.RemoveBody(m_chassisBody);
		m_chassisBody = null;
		ProjectorS.RemoveComponent(m_playerShadow);
		uint num = m_alienCharacter.m_group;
		Debris.CreateDebrisFromGO(m_chassisPrefab.p_gameObject.transform.Find("FrontFender"), GetRandomDebrisVel(), GetRandomDebrisAngVel(), true, num, false, null, Vector3.zero, -1f, 17895696u);
		Debris.CreateDebrisFromGO(m_chassisPrefab.p_gameObject.transform.Find("Light"), GetRandomDebrisVel(), GetRandomDebrisAngVel(), true, num, false, null, Vector3.zero, -1f, 17895696u);
		Debris.CreateDebrisFromGO(m_chassisPrefab.p_gameObject.transform.Find("FrontFork"), GetRandomDebrisVel(), GetRandomDebrisAngVel(), true, num, false, null, Vector3.zero, -1f, 17895696u);
		Debris.CreateDebrisFromGO(m_chassisPrefab.p_gameObject.transform.Find("Gear"), GetRandomDebrisVel(), GetRandomDebrisAngVel(), true, num, false, null, Vector3.zero, -1f, 17895696u);
		Debris.CreateDebrisFromGO(m_chassisPrefab.p_gameObject.transform.Find("GasTank"), GetRandomDebrisVel(), GetRandomDebrisAngVel(), true, num, false, "EngineBreakdown_GameObject", new Vector3(0f, 0f, 10f), -1f, 17895696u);
		Debris.CreateDebrisFromGO(m_chassisPrefab.p_gameObject.transform.Find("Seat"), GetRandomDebrisVel(), GetRandomDebrisAngVel(), true, num, false, null, Vector3.zero, -1f, 17895696u);
		Debris.CreateDebrisFromGO(m_chassisPrefab.p_gameObject.transform.Find("Feet"), GetRandomDebrisVel(), GetRandomDebrisAngVel(), true, num, false, null, Vector3.zero, -1f, 17895696u);
		Debris.CreateDebrisFromGO(m_chassisPrefab.p_gameObject.transform.Find("Power" + m_powerUpgradeTier + "/Exhaust" + m_powerUpgradeTier), GetRandomDebrisVel(), GetRandomDebrisAngVel(), true, num, false, null, Vector3.zero, -1f, 17895696u);
		Debug.LogWarning("Power" + m_powerUpgradeTier + "/Exhaust" + m_powerUpgradeTier);
		Debris.CreateDebrisFromGO(m_ftSuspenserTC.transform.GetChild(0), GetRandomDebrisVel(), GetRandomDebrisAngVel(), true, num, false, null, Vector3.zero, -1f, 17895696u);
		Debris.CreateDebrisFromGO(m_rtSuspenserTC.transform.GetChild(0), GetRandomDebrisVel(), GetRandomDebrisAngVel(), true, num, true, null, Vector3.zero, -1f, 17895696u);
		ChipmunkProWrapper.ucpBodySetVel(m_rearWheelBody.body, GetRandomDebrisVel());
		ChipmunkProWrapper.ucpBodySetVel(m_frontWheelBody.body, GetRandomDebrisVel());
		ChipmunkProWrapper.ucpBodySetAngVel(m_rearWheelBody.body, GetRandomDebrisAngVel());
		ChipmunkProWrapper.ucpBodySetAngVel(m_frontWheelBody.body, GetRandomDebrisAngVel());
		m_tiresBlown = true;
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
	}

	public override List<int> ParseUpgradeValues(Hashtable _upgrades)
	{
		ParseMinAndMaxValues();
		float value = ((_upgrades != null) ? Convert.ToSingle(_upgrades["tireForce"]) : 1f);
		float value2 = ((_upgrades != null) ? Convert.ToSingle(_upgrades["tireRate"]) : 65f);
		float value3 = ((_upgrades != null) ? Convert.ToSingle(_upgrades["grip"]) : 1f);
		float value4 = ((_upgrades != null) ? Convert.ToSingle(_upgrades["suspensionForce"]) : (-1f));
		float value5 = ((_upgrades != null) ? Convert.ToSingle(_upgrades["suspensionDamp"]) : 200f);
		float value6 = ((_upgrades != null) ? Convert.ToSingle(_upgrades["COG"]) : 0f);
		float value7 = ((_upgrades != null) ? Convert.ToSingle(_upgrades["handlingForce"]) : 1f);
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
		hashtable.Add("grip", m_tireGrip / 2.25f);
		hashtable.Add("suspensionForce", m_suspensionMultiplier);
		hashtable.Add("suspensionDamp", m_rearSpringDamp);
		hashtable.Add("COG", m_COGAdd);
		hashtable.Add("handlingForce", m_handlingForce);
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
		return list;
	}

	public override List<KeyValuePair<string, int>> GetUpgrades()
	{
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
			Hashtable upgradeValues = psUpgradeableEditorItem.m_upgradeValues;//Add the <Tinput, TOutput> to the Array.ConvertAll function to make Unity 5.4 happy :).
            m_ranges = Array.ConvertAll<object, int>((object[])upgradeValues["RANGES"], base.ToInt);
			m_tireForceValues = Array.ConvertAll<object, float>((object[])upgradeValues["tireForceValues"], base.ToFloat);
			m_tireRateValues = Array.ConvertAll<object, float>((object[])upgradeValues["tireRateValues"], base.ToFloat);
			m_gripValues = Array.ConvertAll<object, float>((object[])upgradeValues["gripValues"], base.ToFloat);
			m_suspensionValues = Array.ConvertAll<object, float>((object[])upgradeValues["suspensionValues"], base.ToFloat);
			m_dampValues = Array.ConvertAll<object, float>((object[])upgradeValues["dampValues"], base.ToFloat);
			m_COGValues = Array.ConvertAll<object, float>((object[])upgradeValues["COGValues"], base.ToFloat);
			m_handlingValues = Array.ConvertAll<object, float>((object[])upgradeValues["handlingValues"], base.ToFloat);
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
		m_frontTireForces = Vector3.zero * m_tireForceMultiplier;
		m_rearTireForces = new Vector2(1250000f, 600000f) * m_tireForceMultiplier;
		m_tireRate = UpgradeLevelToValue(m_powerUpgrade, m_ranges, m_tireRateValues);
		m_tireGrip = 2.25f * UpgradeLevelToValue(m_gripUpgrade, m_ranges, m_gripValues);
		m_suspensionMultiplier = UpgradeLevelToValue(m_handlingUpgrade, m_ranges, m_suspensionValues);
		m_frontSpringForce = 6500f * m_suspensionMultiplier * -1f;
		m_rearSpringForce = 6500f * m_suspensionMultiplier * -1f;
		m_frontSpringDamp = UpgradeLevelToValue(m_handlingUpgrade, m_ranges, m_dampValues);
		m_rearSpringDamp = UpgradeLevelToValue(m_handlingUpgrade, m_ranges, m_dampValues);
		m_COGAdd = UpgradeLevelToValue(m_handlingUpgrade, m_ranges, m_COGValues);
		m_centerOfGravityOffset = new Vector2(1f, 17f + m_COGAdd);
		m_handlingForce = UpgradeLevelToValue(m_handlingUpgrade, m_ranges, m_handlingValues);
	}
}
