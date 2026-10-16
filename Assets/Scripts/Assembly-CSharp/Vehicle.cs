using System;
using UnityEngine;

public abstract class Vehicle : Unit
{
	public const int UPGRADE_TIER_1 = 5;

	public const int UPGRADE_TIER_2 = 10;

	public const int UPGRADE_TIER_3 = 15;

	public const int MAX_HEALTH = 3;

	protected string[] m_chassisDebrisParts;

	protected PrefabC m_chassisPrefab;

	public Shield m_shield;

	public Armor m_armor;

	public Weapon m_weapon;

	public ChipmunkBodyC m_chassisBody;

	public ChipmunkConstraintC m_rearMotor;

	public ChipmunkConstraintC m_frontMotor;

	private Vector2 m_frontMotorForces;

	private Vector2 m_rearMotorForces;

	private float m_frontMaxRate;

	private float m_rearMaxRate;

	public int m_upgradeSteps;

	public AlienCharacter m_alienCharacter;

	public TransformC m_camTargetTC;

	public SoundC m_engineSound;

	public float m_currentRPM;

	public float m_currentLoad;

	public SoundC m_tireRollSound;

	public SoundC m_boostSound;

	protected Material[] m_materialInstances;

	public PrefabC m_drivingFx;

	public string m_drivingFxName;

	public Booster m_booster;

	private int m_health = 2;

	public int health
	{
		get
		{
			return m_health;
		}
		set
		{
			m_health = Mathf.Clamp(value, -1, 3);
		}
	}

	public Vehicle(GraphElement _graphElement)
		: base(_graphElement, UnitType.Vehicle)
	{
		m_tireRollSound = null;
		m_engineSound = null;
		m_boostSound = null;
	}

	public virtual void InitMotors(ChipmunkBodyC _rearWheel, ChipmunkBodyC _frontWheel)
	{
		TransformC anchor = TransformS.AddComponent(m_entity, _rearWheel.TC.transform.position);
		m_rearMotor = ChipmunkProS.AddSimpleMotor(m_chassisBody, _rearWheel, anchor, 0f, 0f);
		ChipmunkProWrapper.ucpSimpleMotorSetRate(m_rearMotor.constraint, 0f);
		ChipmunkProWrapper.ucpConstraintSetMaxForce(m_rearMotor.constraint, 0f);
		anchor = TransformS.AddComponent(m_entity, _frontWheel.TC.transform.position);
		m_frontMotor = ChipmunkProS.AddSimpleMotor(m_chassisBody, _frontWheel, anchor, 0f, 0f);
		ChipmunkProWrapper.ucpSimpleMotorSetRate(m_frontMotor.constraint, 0f);
		ChipmunkProWrapper.ucpConstraintSetMaxForce(m_frontMotor.constraint, 0f);
		m_frontMotorForces = new Vector2(750000f, 1250000f);
		m_rearMotorForces = new Vector2(750000f, 1250000f);
		m_frontMaxRate = 50f;
		m_rearMaxRate = 50f;
	}

	public void ReachedGoal()
	{
		if (m_booster != null)
		{
			m_booster = null;
		}
	}

	public override void Update()
	{
		base.Update();
		if (m_chassisBody != null)
		{
			m_lastFrameVelocity = ChipmunkProWrapper.ucpBodyGetVel(m_chassisBody.body);
		}
		else
		{
			m_lastFrameVelocity = Vector2.zero;
		}
		UpdateBoostSound();
	}

	protected override bool IsImmuneToGroundEffects(ContactInfo _ci)
	{
		ucpPointQueryInfo[] array = new ucpPointQueryInfo[5];
		ucpShapeFilter filter = default(ucpShapeFilter);
		filter.ucpShapeFilterAll();
		filter.group = GetGroup();
		int num = ChipmunkProWrapper.ucpSpacePointQuery(_ci.m_contactPoint, 5f, filter, array, 5);
		bool result = false;
		for (int i = 0; i < num; i++)
		{
			IntPtr intPtr = ChipmunkProWrapper.ucpShapeGetBody(array[i].shape);
			if (intPtr != _ci.m_contactBody.body && intPtr != _ci.m_cmb.body && ChipmunkProWrapper.ucpBodyGetType(intPtr) == ucpBodyType.DYNAMIC)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public virtual void SetMotorParameters(Vector2 _frontForces, float _frontRate, Vector2 _rearForces, float _rearRate)
	{
		m_frontMotorForces = _frontForces;
		m_rearMotorForces = _rearForces;
		m_frontMaxRate = _frontRate;
		m_rearMaxRate = _rearRate;
	}

	public virtual void UpdateMotors(float _gasAmount, bool _rightIsForward = true, float _forceMultipler = 1f, bool _rearOnly = false)
	{
		Vector2 vector = ChipmunkProWrapper.ucpBodyGetVel(m_chassisBody.body);
		if (_gasAmount != 0f)
		{
			bool flag = ((!_rightIsForward) ? (_gasAmount < 0f) : (_gasAmount > 0f));
			float num = ((!flag) ? m_rearMotorForces.y : m_rearMotorForces.x) * _forceMultipler;
			float value = Mathf.Abs(ChipmunkProWrapper.ucpBodyGetAngVel(m_rearMotor.bodyB.body));
			float num2 = Mathf.Clamp(value, m_rearMaxRate * Mathf.Abs(_gasAmount), m_rearMaxRate);
			if (num > 0f)
			{
				ChipmunkProWrapper.ucpConstraintSetMaxForce(m_rearMotor.constraint, num);
			}
			ChipmunkProWrapper.ucpSimpleMotorSetRate(m_rearMotor.constraint, num2 * (_gasAmount / Mathf.Abs(_gasAmount)));
			if (!_rearOnly)
			{
				float num3 = ((!flag) ? m_frontMotorForces.y : m_frontMotorForces.x) * _forceMultipler;
				value = Mathf.Abs(ChipmunkProWrapper.ucpBodyGetAngVel(m_frontMotor.bodyB.body));
				num2 = Mathf.Clamp(value, m_frontMaxRate * Mathf.Abs(_gasAmount), m_frontMaxRate);
				if (num3 > 0f)
				{
					ChipmunkProWrapper.ucpConstraintSetMaxForce(m_frontMotor.constraint, num3);
				}
				ChipmunkProWrapper.ucpSimpleMotorSetRate(m_frontMotor.constraint, num2 * (_gasAmount / Mathf.Abs(_gasAmount)));
			}
		}
		else
		{
			ChipmunkProWrapper.ucpSimpleMotorSetRate(m_rearMotor.constraint, 0f);
			ChipmunkProWrapper.ucpConstraintSetMaxForce(m_rearMotor.constraint, 0f);
			if (!_rearOnly)
			{
				ChipmunkProWrapper.ucpSimpleMotorSetRate(m_frontMotor.constraint, 0f);
				ChipmunkProWrapper.ucpConstraintSetMaxForce(m_frontMotor.constraint, 0f);
			}
		}
	}

	public virtual void SetHandBrake(bool _brake, float _force = 1000000f)
	{
		if (!m_isDead)
		{
			ChipmunkProWrapper.ucpSimpleMotorSetRate(m_rearMotor.constraint, 0f);
			ChipmunkProWrapper.ucpConstraintSetMaxForce(m_rearMotor.constraint, (!_brake) ? 0f : _force);
			ChipmunkProWrapper.ucpSimpleMotorSetRate(m_frontMotor.constraint, 0f);
			ChipmunkProWrapper.ucpConstraintSetMaxForce(m_frontMotor.constraint, (!_brake) ? 0f : _force);
		}
	}

	public virtual void SetFrontBrake(bool _brake, float _force = 1000000f)
	{
		if (!m_isDead)
		{
			ChipmunkProWrapper.ucpSimpleMotorSetRate(m_frontMotor.constraint, 0f);
			ChipmunkProWrapper.ucpConstraintSetMaxForce(m_frontMotor.constraint, (!_brake) ? 0f : _force);
		}
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		UpdateMotors(0f);
	}

	public override void Damage(Damage _damage, float _multiplier, Unit _source)
	{
		if (!PsState.m_activeMinigame.m_playerReachedGoal)
		{
			base.Damage(_damage, _multiplier, _source);
		}
	}

	public virtual void ExplosionEffect(Vector3 _position)
	{
		Entity entity = EntityManager.AddEntity(new string[1] { "GTAG_AUTODESTROY" });
		TransformC transformC = TransformS.AddComponent(entity);
		transformC.transform.position = _position;
		PrefabC c = PrefabS.AddComponent(transformC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.VehicleCrushEffect_GameObject));
		PrefabS.SetCamera(c, CameraS.m_mainCamera);
	}

	public void SetBoostSound(TransformC _tc)
	{
		if (m_boostSound == null)
		{
			m_boostSound = SoundS.AddComponent(_tc, "/Ingame/Vehicles/BoosterLoop");
			SoundS.PlaySound(m_boostSound);
			SoundS.SetSoundParameter(m_boostSound, "RPM", m_currentRPM);
		}
	}

	public void RemoveBoostSound()
	{
		if (m_boostSound != null)
		{
			SoundS.RemoveComponent(m_boostSound);
			m_boostSound = null;
		}
	}

	public void UpdateBoostSound()
	{
		if (m_boostSound != null)
		{
			SoundS.SetSoundParameter(m_boostSound, "RPM", m_currentRPM * m_currentLoad);
		}
	}

	public void SetEngineSound(TransformC _tc, string _sound, int _minRPM)
	{
		if (m_engineSound == null)
		{
			m_engineSound = SoundS.AddComponent(_tc, _sound);
			SoundS.PlaySound(m_engineSound);
			m_currentRPM = _minRPM;
			m_currentLoad = 0f;
			SoundS.SetSoundParameter(m_engineSound, "RPM", m_currentRPM);
			SoundS.SetVolume(m_engineSound, 0f);
		}
	}

	public void RemoveEngineSound()
	{
		if (m_engineSound != null)
		{
			SoundS.RemoveComponent(m_engineSound);
			m_engineSound = null;
		}
	}

	public void UpdateEngineSound(float _targetRPM, float _targetLoad)
	{
		if (m_engineSound != null)
		{
			m_currentRPM += (_targetRPM - m_currentRPM) * 0.1f;
			m_currentLoad += (_targetLoad - m_currentLoad) * 0.1f;
			SoundS.SetSoundParameter(m_engineSound, "RPM", m_currentRPM);
			SoundS.SetSoundParameter(m_engineSound, "Load", m_currentLoad);
			if (GameSceneEffectManager.m_signalNoiseMusicPlaying)
			{
				SoundS.SetVolume(m_engineSound, 0f);
			}
			else
			{
				SoundS.SetVolume(m_engineSound, 1f);
			}
		}
		else
		{
			m_currentRPM = 0f;
			m_currentLoad = 0f;
		}
	}

	public void SetTireRollSound(TransformC _tc, string _sound)
	{
		if (_sound != null && m_tireRollSound != null && _sound.Equals(m_tireRollSound.name))
		{
			SoundS.SetVolume(m_tireRollSound, 1f);
			return;
		}
		RemoveTireRollSound();
		if (m_tireRollSound == null && _sound != null)
		{
			m_tireRollSound = SoundS.AddComponent(_tc, _sound);
			SoundS.SetSoundParameter(m_tireRollSound, "RPM", m_currentRPM);
			SoundS.PlaySound(m_tireRollSound);
		}
	}

	public void UpdateTireRollSound()
	{
		if (m_tireRollSound != null)
		{
			SoundS.SetSoundParameter(m_tireRollSound, "RPM", m_currentRPM);
		}
	}

	public void MuteTireRollSound()
	{
		if (m_tireRollSound != null)
		{
			SoundS.SetVolume(m_tireRollSound, 0f);
		}
	}

	public void RemoveTireRollSound()
	{
		if (m_tireRollSound != null)
		{
			SoundS.RemoveComponent(m_tireRollSound);
			m_tireRollSound = null;
		}
	}

	public virtual void LaunchRagdoll(bool _explodeBones = false)
	{
	}

	public void StartDriveFx(TransformC _tc, bool _destructibleGround, string _fx)
	{
		if (m_drivingFx == null && !m_isDead && _fx != null)
		{
			Vector3 zero = Vector3.zero;
			zero = (_destructibleGround ? new Vector3(0f, 0f, -2.91f) : new Vector3(-33.13f, 0f, -2.91f));
			GameObject gameObject = ResourceManager.GetGameObject(_fx);
			m_drivingFx = PrefabS.AddComponent(_tc, zero, gameObject, string.Empty, false);
			m_drivingFxName = _fx;
		}
		else if (m_drivingFx != null && !_destructibleGround)
		{
			m_drivingFx.p_gameObject.GetComponent<ParticleSystem>().Play();
		}
	}

	public void PauseDriveFx()
	{
		if (m_drivingFx != null)
		{
			m_drivingFx.p_gameObject.GetComponent<ParticleSystem>().Stop();
		}
	}

	public void RemoveDriveFx()
	{
		if (m_drivingFx != null)
		{
			TimerC timerC = TimerS.AddComponent(m_entity, "RemoveTimerForDriveFxEmitter", 2f, 0f, false, RemoveDriveFxTimerDelegate);
			timerC.customComponent = m_drivingFx;
			m_drivingFx.p_gameObject.GetComponent<ParticleSystem>().Stop();
			m_drivingFx = null;
			m_drivingFxName = string.Empty;
		}
	}

	public void RemoveDriveFxTimerDelegate(TimerC _c)
	{
		PrefabS.RemoveComponent(_c.customComponent as PrefabC);
		TimerS.RemoveComponent(_c);
	}

	protected Vector2 GetRandomDebrisVel()
	{
		return m_lastFrameVelocity * 0.7f + new Vector2(UnityEngine.Random.Range(-300f, 300f), UnityEngine.Random.Range(200f, 400f));
	}

	protected float GetRandomDebrisAngVel()
	{
		return UnityEngine.Random.Range(-600f, 600f) * ((float)Math.PI / 180f);
	}

	protected void CreateRandomImpactDebris(string[] chassisPartNames, GameObject debrisPrefab)
	{
		Vector2 randomDebrisVel = GetRandomDebrisVel();
		float randomDebrisAngVel = GetRandomDebrisAngVel();
		if (chassisPartNames.Length > 0)
		{
			int num = UnityEngine.Random.Range(0, chassisPartNames.Length);
			Transform transform = m_chassisPrefab.p_gameObject.transform.FindChild(chassisPartNames[num]);
			if (transform != null && transform.gameObject.activeInHierarchy)
			{
				Debris.CreateDebrisFromGO(transform, randomDebrisVel, randomDebrisAngVel, true, GetGroup(), false, null, default(Vector3), -1f);
			}
		}
		if (debrisPrefab.transform.childCount > 0)
		{
			Vector3 vector = Vector3.forward * UnityEngine.Random.Range(-5f, 5f);
			int num = UnityEngine.Random.Range(0, debrisPrefab.transform.childCount);
			Debris.CreateDebrisFromGO(debrisPrefab.transform.GetChild(num), m_chassisBody.TC.transform.position + vector, randomDebrisVel, randomDebrisAngVel, false, GetGroup(), false, null, Vector3.zero, 5f);
		}
	}

	public void CheckVisualUpgrade(string type, int level)
	{
		Transform transform = null;
		for (int i = 0; i <= 3; i++)
		{
			transform = m_chassisPrefab.p_gameObject.transform.FindChild(type + "T" + i);
			if (transform != null)
			{
				transform.gameObject.SetActive(false);
			}
		}
		int num = Mathf.FloorToInt((float)level / 5f);
		transform = m_chassisPrefab.p_gameObject.transform.FindChild(type + "T" + num);
		while (transform == null && num != 0)
		{
			num--;
			transform = m_chassisPrefab.p_gameObject.transform.FindChild(type + "T" + num);
		}
		if (transform != null)
		{
			transform.gameObject.SetActive(true);
		}
	}

	public Texture GetTierTexture(int _tier)
	{
		int num = Mathf.Min(_tier - 1, 4);
		string resourceIdentifier = GetType().Name + "DifT" + num + "_Texture2D";
		return ResourceManager.GetTexture(resourceIdentifier);
	}

	public void DisableCollidersRenderer(Transform _transform)
	{
		foreach (Transform item in _transform.transform)
		{
			if (item.name.ToLower().Contains("collider"))
			{
				MeshRenderer component = item.gameObject.GetComponent<MeshRenderer>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
			DisableCollidersRenderer(item);
		}
	}

	public static string GetUpgradeName(string _key)
	{
		switch (_key)
		{
		case "power":
			return "Power";
		case "grip":
			return "Grip";
		case "handling":
			return "Suspension";
		default:
			return null;
		}
	}

	public override void Destroy()
	{
		base.Destroy();
		if (m_materialInstances != null)
		{
			for (int i = 0; i < m_materialInstances.Length; i++)
			{
				UnityEngine.Object.Destroy(m_materialInstances[i]);
				m_materialInstances[i] = null;
			}
			m_materialInstances = null;
		}
	}

	public bool DecreaseHealth(int damage = 1)
	{
		if (health > 0)
		{
			m_health = Mathf.Max(m_health - damage, 0);
		}
		if (m_health == 0)
		{
			return true;
		}
		return false;
	}
}
