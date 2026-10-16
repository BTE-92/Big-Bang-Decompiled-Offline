using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class AlienCharacter : Unit
{
	private const int BEAMING_OUT_DELAY = 40;

	public uint m_group;

	public CameraTargetC m_camTarget;

	public TransformC m_mainTC;

	public PrefabC m_mainPC;

	public Animator m_animator;

	private RuntimeAnimatorController m_animatorController;

	private int m_bindPoseState;

	private int m_standPoseState;

	private int m_drivePoseState;

	public RagdollNode m_ragdoll;

	public bool m_crushed;

	private float m_blinkTimer;

	private float m_blinkFrequency = 2.5f;

	public int m_beamOutTimer;

	private float m_targetWidth = 600f;

	private float m_targetHeight = 400f;

	private Vector2 m_targetOffset = new Vector2(0f, 35f);

	private bool m_dontBlink;

	private Material[] m_materialInstances;

	public AlienEffects m_alienEffects;

	public List<Transform> m_bones;

	private bool m_beamInitialized;

	private TransformC m_beamFxTC;

	private bool m_hasHelmet = true;

	private float m_nextElectrifiedEffect;

	public AlienCharacter(GraphElement _graphElement, string driveState, string _assetName = "AlienNewPrefab_GameObject")
		: base(_graphElement, UnitType.Character)
	{
		m_group = GetGroup();
		m_bones = new List<Transform>();
		m_mainTC = TransformS.AddComponent(m_entity, "AlienCharacter");
		TransformS.SetTransform(m_mainTC, _graphElement.m_position, Vector3.zero);
		m_mainPC = PrefabS.AddComponent(m_mainTC, Vector3.zero, ResourceManager.GetGameObject(_assetName));
		PrefabS.SetCamera(m_mainPC, CameraS.m_mainCamera);
		DisableColliderRenderers(m_mainPC.p_gameObject.transform);
		if (PsState.m_activeGameLoop is PsGameLoopEditorPuzzle && PsState.m_activeMinigame != null && PsState.m_activeMinigame.m_editing)
		{
			Renderer[] componentsInChildren = m_mainPC.p_gameObject.GetComponentsInChildren<Renderer>();
			m_materialInstances = new Material[componentsInChildren.Length];
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				m_materialInstances[i] = componentsInChildren[i].material;
				m_materialInstances[i].shader = Shader.Find("WOE/Units/Greyscale");
			}
			m_dontBlink = true;
		}
		m_animator = m_mainPC.p_gameObject.GetComponent<Animator>();
		m_animatorController = ResourceManager.GetResourceClass("AlienAnimatorController_AnimatorController").ResourceObject as RuntimeAnimatorController;
		m_animator.runtimeAnimatorController = m_animatorController;
		if (_graphElement.m_flipped)
		{
			TransformS.SetGlobalRotation(m_mainTC, Vector3.up * 270f);
		}
		else
		{
			TransformS.SetGlobalRotation(m_mainTC, Vector3.up * 90f);
		}
		m_bindPoseState = Animator.StringToHash("Base.Bind");
		m_standPoseState = Animator.StringToHash("Base.Stand");
		m_drivePoseState = Animator.StringToHash("Base." + driveState);
		m_animator.Play(m_drivePoseState);
		m_hitPoints = 1f;
		m_hitPointType = HitPointType.Lives;
		m_canElectrify = true;
		m_checkForCrushing = true;
		m_crushTolerance = 2000;
		m_beamOutTimer = 40;
		m_alienEffects = m_mainPC.p_gameObject.GetComponent<AlienEffects>();
		if (m_alienEffects != null)
		{
			m_alienEffects.Initialize();
		}
	}

	public override void Destroy()
	{
		if (m_materialInstances != null)
		{
			for (int i = 0; i < m_materialInstances.Length; i++)
			{
				UnityEngine.Object.Destroy(m_materialInstances[i]);
				m_materialInstances[i] = null;
			}
			m_materialInstances = null;
		}
		while (m_bones.Count > 0)
		{
			int index = m_bones.Count - 1;
			UnityEngine.Object.Destroy(m_bones[index].gameObject);
			m_bones.RemoveAt(index);
		}
		base.Destroy();
		m_ragdoll = null;
	}

	public void DisableColliderRenderers(Transform _tc)
	{
		if (_tc.GetComponent<Renderer>() != null && _tc.name.Contains("Collider"))
		{
			_tc.GetComponent<Renderer>().enabled = false;
		}
		for (int i = 0; i < _tc.childCount; i++)
		{
			DisableColliderRenderers(_tc.GetChild(i));
		}
	}

	public void SetPropToLocator(RESOURCE _resourceObject, string _locatorPathName)
	{
		SetPropToLocator(ResourceManager.GetGameObject(_resourceObject), _locatorPathName);
	}

	public void SetPropToLocator(GameObject _resourceGameObject, string _locatorPathName)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(_resourceGameObject);
		PrefabS.SetCamera(gameObject, CameraS.m_mainCamera);
		if (PsState.m_activeGameLoop is PsGameLoopEditorPuzzle && PsState.m_activeMinigame != null && PsState.m_activeMinigame.m_editing)
		{
			int num = m_materialInstances.Length;
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
			Material[] array = new Material[num + componentsInChildren.Length];
			Array.Copy(m_materialInstances, array, num);
			m_materialInstances = array;
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				m_materialInstances[num + i] = componentsInChildren[i].material;
				m_materialInstances[num + i].shader = Shader.Find("WOE/Units/Greyscale");
			}
		}
		Transform parent = m_mainPC.p_gameObject.transform.Find(_locatorPathName);
		gameObject.transform.parent = parent;
		gameObject.transform.localPosition = Vector3.zero;
	}

	public string SetRandomHat()
	{
		bool flag = true;
		string empty = string.Empty;
		UnityEngine.Random.seed = (int)(Main.m_gameTimeSinceAppStarted * 60.0);
		int num = 1;
		if (!flag)
		{
			num = UnityEngine.Random.Range(0, 5);
		}
		RESOURCE resourceObject;
		switch (num)
		{
		case 0:
			resourceObject = RESOURCE.AlienBaseballHatPrefab_GameObject;
			empty = "BaseballHat";
			break;
		case 1:
			resourceObject = RESOURCE.AlienHelmetPrefab_GameObject;
			empty = "Helmet";
			break;
		case 2:
			resourceObject = RESOURCE.BarbarianHelmetPrefab_GameObject;
			empty = "BarbarianHelmet";
			break;
		case 3:
			resourceObject = RESOURCE.CowboyHatPrefab_GameObject;
			empty = "CowboyHat";
			break;
		case 4:
			resourceObject = RESOURCE.HorseHeadMaskPrefab_GameObject;
			empty = "HorseHead";
			break;
		default:
			resourceObject = RESOURCE.AlienHelmetPrefab_GameObject;
			empty = "Helmet";
			break;
		}
		SetPropToLocator(resourceObject, "Hips/Spine1/Spine2/Neck/Head/HeadCollider/HeadGearLocator");
		return empty;
	}

	public override void SetAllBaseArmours()
	{
		base.SetAllBaseArmours();
		SetBaseArmor(DamageType.Weapon, 0f);
		SetBaseArmor(DamageType.Electric, 0f);
	}

	public void AnimateCharacterRandom(string _trigger, int _value)
	{
		if (m_animator != null)
		{
			m_animator.SetInteger("Random", _value);
			m_animator.SetTrigger(_trigger);
		}
	}

	public void AnimateCharacter(string _trigger)
	{
		if (m_animator != null)
		{
			m_animator.SetTrigger(_trigger);
		}
	}

	public void AnimateCharacter(string _param, int _value)
	{
		if (m_animator != null)
		{
			m_animator.SetInteger(_param, _value);
		}
	}

	public override void InactiveUpdate()
	{
		base.InactiveUpdate();
		BeamOutUpdate();
	}

	public override void Update()
	{
		base.Update();
		if (m_electrificationTime > 1f && m_canElectrify)
		{
			ExplodeRagdoll(m_ragdoll);
		}
		Blink();
		BeamOutUpdate();
	}

	private void Blink()
	{
		if (!(m_animator != null) || m_dontBlink)
		{
			return;
		}
		if (m_blinkTimer <= 0f)
		{
			if (!m_animator.GetCurrentAnimatorStateInfo(1).IsName("Blink") && m_animator.GetCurrentAnimatorStateInfo(2).IsName("Default"))
			{
				AnimateCharacter("Blink");
			}
			m_blinkTimer = UnityEngine.Random.Range(1f, m_blinkFrequency);
		}
		else
		{
			m_blinkTimer -= Main.m_gameDeltaTime;
		}
	}

	public void BeamOutUpdate()
	{
		if (!m_minigame.m_playerBeamingOut)
		{
			return;
		}
		bool flag = !m_minigame.m_playerReachedGoal && !m_crushed;
		if (!m_beamInitialized)
		{
			m_beamOutTimer = 40;
			m_beamInitialized = true;
		}
		if (flag && ((!m_minigame.m_gameEnded && m_beamOutTimer == 40) || (m_minigame.m_gameEnded && m_beamOutTimer == 40)))
		{
			SoundS.PlaySingleShot("/InGame/Events/RestartBeam", Vector3.zero);
			m_beamFxTC = TransformS.AddComponent(m_entity, m_mainTC.transform.position);
			PrefabS.AddComponent(m_beamFxTC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.BeamUp_GameObject));
			m_beamFxTC.transform.localRotation = Quaternion.Euler(new Vector3(-90f, 0f, 0f));
		}
		if (m_beamOutTimer <= 0)
		{
			m_minigame.m_playerBeamingOut = true;
		}
		if (flag)
		{
			float value = 1f - ToolBox.getPositionBetween(m_beamOutTimer, 10f, 40f);
			TransformS.SetPosition(m_beamFxTC, m_mainTC.transform.position);
			Component[] componentsInChildren = m_mainPC.p_gameObject.GetComponentsInChildren(typeof(Renderer), false);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				(componentsInChildren[i] as Renderer).material.SetFloat("_Cutoff", value);
			}
			if (m_beamOutTimer == 0)
			{
				m_animator = null;
				EntityManager.SetActivityOfEntity(m_entity, false, true, true, false);
			}
		}
		m_beamOutTimer--;
	}

	public void AddLinearVelocityToRagDoll(RagdollNode _node, Vector2 _addition, float _random)
	{
		if (_node != null)
		{
			Vector2 vector = ChipmunkProWrapper.ucpBodyGetVel(_node.m_cmb.body);
			float x = UnityEngine.Random.value * _random - _random * 0.5f;
			float y = UnityEngine.Random.value * _random - _random * 0.5f;
			Vector2 vector2 = vector + _addition + new Vector2(x, y);
			ChipmunkProWrapper.ucpBodySetVel(_node.m_cmb.body, vector + _addition + new Vector2(x, y));
			for (int i = 0; i < _node.m_childs.Count; i++)
			{
				AddLinearVelocityToRagDoll(_node.m_childs[i], _addition, _random);
			}
		}
	}

	public void ConstructRagdoll(bool _skipJoints = false)
	{
		if (m_ragdoll != null)
		{
			return;
		}
		if (m_mainTC.parent != null)
		{
			TransformS.UnparentComponent(m_mainTC);
		}
		Vector3 eulerAngles = m_mainTC.transform.eulerAngles;
		if (eulerAngles.x > 180f)
		{
			eulerAngles.x -= 360f;
		}
		if (eulerAngles.z == 180f)
		{
			eulerAngles.x = 0f - eulerAngles.x + 180f;
			if (m_graphElement.m_flipped)
			{
				eulerAngles.y = 270f;
			}
			else
			{
				eulerAngles.y = 90f;
			}
			eulerAngles.z = 0f;
		}
		m_mainTC.transform.rotation = Quaternion.Euler(eulerAngles);
		m_ragdoll = CreateRagdoll(m_mainPC.p_gameObject.transform, _skipJoints);
		m_ragdoll.m_ragdollJointBootstrap.m_mainTC = m_mainTC;
		CreateCamTarget(m_mainTC);
	}

	public void ThrowHelmet([Optional] Vector2 _force, float _angVel = 0f, bool _toBackground = false, uint _layer = 17895696u, bool _tween = false, bool flyUp = false)
	{
		if (!m_hasHelmet)
		{
			return;
		}
		Transform transform = m_mainPC.p_gameObject.transform.Find("Hips/Spine1/Spine2/Neck/Head/HeadCollider/HeadGearLocator");
		if (transform != null && transform.childCount > 0 && transform.GetChild(0).childCount > 0)
		{
			if (_force == Vector2.zero || flyUp)
			{
				_force += new Vector2(m_mainTC.transform.up.x, m_mainTC.transform.up.y) * 400f;
			}
			new PsHat(transform.GetChild(0).GetChild(0), _force, _angVel);
			m_hasHelmet = false;
		}
	}

	public override void EmergencyKill()
	{
		if (!m_crushed)
		{
			Debug.Log("EMERGENCY KILL CHARACTER");
			if (m_ragdoll != null)
			{
				ExplodeRagdoll(m_ragdoll);
			}
			m_animator = null;
			m_crushed = true;
			CameraS.RemoveAllTargetComponents();
		}
	}

	public void CreateCamTarget(TransformC _tc)
	{
		if (m_camTarget != null)
		{
			CameraS.RemoveTargetComponent(m_camTarget);
		}
		m_camTarget = CameraS.AddTargetComponent(_tc, m_targetWidth, m_targetHeight, m_targetOffset);
		m_camTarget.interpolateSpeed = 1f / 30f;
		m_camTarget.frameGrowVelocityMultiplier = new Vector2(10f, 10f);
		m_camTarget.framePosVelocityMultiplier = new Vector2(41f, 30f);
		m_camTarget.frameScaleVelocityMultiplier = 0.02f;
		m_camTarget.frameSlopRadiusMinMax = new Vector2(0f, 200f);
		m_camTarget.velAngleChangeMult = new Vector2(2f, 3.5f);
		m_camTarget.angleLimits = new Vector2(8f, 16f);
		m_camTarget.framePeekShiftMax = new Vector2(100f, 0f);
		m_camTarget.framePeekShiftMultiplier = 8f;
		m_camTarget.frameWorldBounds.b = (float)(-LevelManager.m_currentLevel.m_currentLayer.m_layerHeight) * 0.5f - 100f;
		SoundS.SetListener(_tc.transform.gameObject, true);
	}

	public void SetVehicleSelectTarget(float _width, float _height, Vector2 _offset)
	{
		m_camTarget.offset = _offset;
		CameraS.SetTargetBB(m_camTarget, _width, _height);
	}

	public void ResetCameraTarget()
	{
		m_camTarget.offset = m_targetOffset;
		CameraS.SetTargetBB(m_camTarget, m_targetWidth, m_targetHeight);
	}

	private void ExplodeRagdoll(RagdollNode _node)
	{
		Transform transform = null;
		for (int i = 0; i < _node.m_transform.childCount; i++)
		{
			Transform child = _node.m_transform.GetChild(i);
			if (!child.name.Contains("AlienBones"))
			{
				continue;
			}
			transform = child;
			if (child.name == "AlienBones_LegUpperRight" || child.name == "AlienBones_LegUpperLeft" || child.name == "AlienBones_Skull" || child.name == "AlienBones_ArmUpperRight" || child.name == "AlienBones_ArmUpperLeft")
			{
				TransformS.UnparentComponent(_node.m_tc);
				if (_node.m_pivotJoint != IntPtr.Zero)
				{
					ChipmunkProWrapper.ucpRemoveConstraint(_node.m_pivotJoint);
					_node.m_pivotJoint = IntPtr.Zero;
				}
				if (_node.m_rotaryLimitJoint != IntPtr.Zero)
				{
					ChipmunkProWrapper.ucpRemoveConstraint(_node.m_rotaryLimitJoint);
					_node.m_rotaryLimitJoint = IntPtr.Zero;
				}
				break;
			}
		}
		if (transform != null)
		{
			transform.parent = _node.m_tc.transform;
			m_bones.Add(transform);
		}
		for (int j = 0; j < _node.m_childs.Count; j++)
		{
			ExplodeRagdoll(_node.m_childs[j]);
		}
		if (_node == m_ragdoll)
		{
			SetElectrified(false, true, Vector2.zero);
			m_canElectrify = false;
			m_crushed = true;
			m_checkForCrushing = false;
			if (m_alienEffects != null)
			{
				m_alienEffects.Explode();
			}
			Vector2 vector = m_lastFrameVelocity * 0.7f;
			if (vector.y < 0f)
			{
				vector.y *= 0.5f;
			}
			AddLinearVelocityToRagDoll(m_ragdoll, Vector2.up * 200f, 300f);
		}
	}

	private RagdollNode CreateRagdoll(Transform _transform, bool _skipJoints = false, RagdollNode _parentNode = null)
	{
		if (_parentNode == null)
		{
			m_animator.Play(m_drivePoseState);
			AnimateCharacter("DriveDir", 1);
			AnimateCharacter("LeanDir", 0);
			m_animator.Update(10f);
			if (_skipJoints)
			{
				m_crushed = true;
				m_checkForCrushing = false;
			}
		}
		RagdollNode ragdollNode = null;
		bool flag = false;
		Transform transform = null;
		Transform transform2 = null;
		for (int i = 0; i < _transform.childCount; i++)
		{
			Transform child = _transform.GetChild(i);
			if (child.name.Contains("Collider"))
			{
				transform = child;
			}
			if (_skipJoints && child.name.Contains("AlienBones"))
			{
				transform2 = child;
				if (child.name == "AlienBones_LegUpperRight" || child.name == "AlienBones_LegUpperLeft" || child.name == "AlienBones_Skull" || child.name == "AlienBones_ArmUpperRight" || child.name == "AlienBones_ArmUpperLeft")
				{
					flag = true;
				}
			}
		}
		if (transform != null)
		{
			ragdollNode = new RagdollNode(transform.parent);
			ragdollNode.m_flipped = m_graphElement.m_flipped;
			Vector2[] array = ChipmunkProS.GenerateCollisionVertexArrayFromGameObject(transform.gameObject, true, ragdollNode.m_flipped);
			ragdollNode.m_centeroid = ChipmunkProWrapper.ucpCenteroidForPoly(array.Length, array);
			float num = ChipmunkProWrapper.ucpAreaForPoly(array.Length, array, 0f);
			num /= 1100f;
			float num2 = (2f + 4f * num) * 2.5f;
			ragdollNode.m_tc = TransformS.AddComponent(m_entity, transform.position + ragdollNode.m_centeroid);
			ragdollNode.m_tc.transform.name = ragdollNode.m_name;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			for (int j = 0; j < transform.childCount; j++)
			{
				Transform child2 = transform.GetChild(j);
				if (child2.name.Contains("Angle"))
				{
					Vector2 vector = child2.position - transform.position;
					num3 = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
					if (ragdollNode.m_flipped)
					{
						num3 += 180f;
					}
				}
				else if (child2.name.Contains("Min"))
				{
					num4 = child2.localEulerAngles.x;
					if (num4 >= 180f)
					{
						num4 -= 360f;
					}
				}
				else if (child2.name.Contains("Max"))
				{
					num5 = child2.localEulerAngles.x;
					if (num5 >= 180f)
					{
						num5 -= 360f;
					}
				}
			}
			ragdollNode.m_tc.transform.RotateAround(transform.position, Vector3.forward, num3);
			ucpPolyShape ucpPolyShape2 = new ucpPolyShape(array, -ragdollNode.m_centeroid, 17895696u, num2, 0.5f, 1f, (ucpCollisionType)4);
			ucpPolyShape2.group = m_group;
			if (transform.name == "HeadCollider")
			{
				ucpPolyShape2.group++;
			}
			ragdollNode.m_cmb = ChipmunkProS.AddDynamicBody(ragdollNode.m_tc, ucpPolyShape2);
			ChipmunkProS.SetBodyAngularDamp(ragdollNode.m_cmb, 0.9f);
			ChipmunkProS.SetBodyLinearDamp(ragdollNode.m_cmb, new Vector2(0.997f, 0.997f));
			ChipmunkProS.AddCollisionHandler(ragdollNode.m_cmb, RagdollCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)2, true, false, false);
			ChipmunkProS.AddCollisionHandler(ragdollNode.m_cmb, RagdollCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)4, true, false, false);
			if (m_unitC != null)
			{
				ragdollNode.m_cmb.customComponent = m_unitC;
			}
			if (transform2 != null)
			{
				transform2.parent = ragdollNode.m_tc.transform;
			}
			if (_parentNode != null)
			{
				_parentNode.m_childs.Add(ragdollNode);
				ragdollNode.m_parent = _parentNode;
				if (!flag)
				{
					TransformS.ParentComponent(ragdollNode.m_tc, ragdollNode.m_parent.m_tc);
					ragdollNode.m_offset = Vector3.zero;
					if (ragdollNode.m_parent != null)
					{
						ragdollNode.m_offset = ragdollNode.m_transform.eulerAngles - ragdollNode.m_parent.m_transform.eulerAngles;
					}
					ragdollNode.m_pivotJoint = ChipmunkProWrapper.ucpPivotJointNew(_parentNode.m_cmb.body, ragdollNode.m_cmb.body, transform.position);
					ChipmunkProWrapper.ucpAddConstraint(ragdollNode.m_pivotJoint, -1);
					ragdollNode.m_rotaryLimitJoint = ChipmunkProWrapper.ucpRotaryLimitJointNew(_parentNode.m_cmb.body, ragdollNode.m_cmb.body, num4 * ((float)Math.PI / 180f), num5 * ((float)Math.PI / 180f));
					ChipmunkProWrapper.ucpConstraintSetMaxForce(ragdollNode.m_rotaryLimitJoint, 50000f * num2);
					ChipmunkProWrapper.ucpAddConstraint(ragdollNode.m_rotaryLimitJoint, -1);
				}
			}
		}
		if (ragdollNode == null && _parentNode != null)
		{
			ragdollNode = _parentNode;
		}
		for (int k = 0; k < _transform.childCount; k++)
		{
			Transform child3 = _transform.GetChild(k);
			if (_skipJoints && transform == null && transform2 != null)
			{
				transform2.parent = ragdollNode.m_tc.transform;
			}
			RagdollNode ragdollNode2 = CreateRagdoll(child3, _skipJoints, ragdollNode);
			if (ragdollNode == null)
			{
				ragdollNode = ragdollNode2;
			}
		}
		return ragdollNode;
	}

	private void RagdollCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		if (_phase == ucpCollisionPhase.Begin)
		{
			float positionBetween = ToolBox.getPositionBetween(_pair.impulse.magnitude, 3000f, 10000f);
			if (positionBetween > 0f)
			{
				SoundS.PlaySingleShotWithParameter("/InGame/Characters/AlienBodyImpact", Vector3.zero, "Force", 0.2f + positionBetween * 0.8f);
			}
		}
	}

	public override void SetElectrified(bool _electrify, bool _affectContacts, Vector2 _contactPoint)
	{
		base.SetElectrified(_electrify, _affectContacts, _contactPoint);
		if (m_isElectrified && m_nextElectrifiedEffect < Main.m_resettingGameTime)
		{
			m_nextElectrifiedEffect = Main.m_resettingGameTime + 1f;
			SoundS.PlaySingleShot("/Ingame/Characters/AlienElectrocution", Vector2.zero);
		}
		if (m_alienEffects != null && !m_crushed)
		{
			if (_electrify)
			{
				m_alienEffects.PlayElectrocute();
			}
			else
			{
				m_alienEffects.StopElectrocute();
			}
		}
	}
}
