using UnityEngine;

public class ExplodingBarrel : Unit
{
	private const int DETONATION_TICKS = 120;

	private ChipmunkBodyC m_cmb;

	private TransformC m_tc;

	private int m_detonationTicks;

	private int m_ticks;

	private bool m_detonated;

	private Entity m_smokeEntity;

	private CameraTargetC m_ctc;

	public ExplodingBarrel(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.ExplodingBarrelPrefab_GameObject);
		m_tc = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(m_tc, _graphElement.m_position, _graphElement.m_rotation);
		PrefabC prefabC = PrefabS.AddComponent(m_tc, new Vector3(0f, 0f, 0f), gameObject.transform.Find("ExplosiveBarrel").gameObject);
		ucpPolyShape[] shapes = ChipmunkProS.GeneratePolyShapesFromChildren(gameObject.transform.Find("Collision1").gameObject, Vector2.zero, 18f, 0.1f, 0.9f, (ucpCollisionType)4, 17895696u);
		m_cmb = ChipmunkProS.AddDynamicBody(m_tc, shapes);
		m_cmb.customComponent = m_unitC;
		if (m_graphElement.m_storedRotation == Vector3.zero)
		{
			m_graphElement.m_storedRotation = new Vector3(0f, Random.Range(0f, 360f), 0f);
		}
		prefabC.p_gameObject.transform.Rotate(m_graphElement.m_storedRotation);
		m_hitPoints = 1f;
		m_hitPointType = HitPointType.Lives;
		m_canElectrify = true;
		m_ticks = 0;
		m_detonated = false;
		m_smokeEntity = null;
		CreateEditorTouchArea(50f, 50f);
		m_graphElement.m_isRotateable = true;
		if (!m_minigame.m_editing)
		{
			CreateCamTarget();
		}
	}

	public override void SetAllBaseArmours()
	{
		base.SetAllBaseArmours();
		SetBaseArmor(DamageType.Impact, 80f);
		SetBaseArmor(DamageType.Electric, 10f);
		SetBaseArmor(DamageType.Weapon, 17f);
	}

	public override void Damage(Damage _damage, float _multiplier, Unit _source)
	{
		if (_source != null && _source.m_graphElement.GetType() == typeof(LevelPlayerNode))
		{
			_multiplier = 1f;
		}
		base.Damage(_damage, _multiplier, _source);
	}

	public override void Update()
	{
		base.Update();
		if (m_detonated)
		{
			m_ticks++;
			if (m_ticks >= m_detonationTicks)
			{
				Kill(DamageType.Impact, float.MaxValue);
			}
		}
	}

	public void CreateCamTarget()
	{
		m_ctc = CameraS.AddTargetComponent(m_tc, 250f, 250f, Vector2.zero);
		m_ctc.activeRadius = 750f;
	}

	public override void EmergencyKill()
	{
		Kill(DamageType.Impact, float.MaxValue);
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		if (m_isDead)
		{
			return;
		}
		if (_damageType == DamageType.Weapon)
		{
			if (!m_detonated)
			{
				m_detonated = true;
				m_detonationTicks = 120 + Random.Range(-15, 15);
				m_smokeEntity = EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject(RESOURCE.EngineBreakdown_GameObject), m_tc.transform.position + new Vector3(0f, 20.11f, -2.91f), Vector3.zero, 999f, "GTAG_AUTODESTROY", m_tc).p_entity;
				m_ctc.activeRadius = 1250f;
				SoundC c = SoundS.AddCombineSoundComponent(m_tc, "BurningSound", "/InGame/Units/BarrelBurn");
				SoundS.PlaySound(c);
			}
			return;
		}
		base.Kill(_damageType, _totalDamage);
		if (m_detonated)
		{
			PrefabS.PauseParticleEmission(EntityManager.GetComponentByEntity(ComponentType.Prefab, m_smokeEntity) as PrefabC, true);
		}
		Vector2 blastCenter = ChipmunkProWrapper.ucpBodyGetPos(m_cmb.body);
		PsS.ApplyBlastWave(blastCenter, new Vector2(50f, 2500f), 600f, 150f, 25f);
		EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject(RESOURCE.ExplodingBarrelExplosion_GameObject), new Vector3(blastCenter.x, blastCenter.y, 0f), Vector3.zero, 2f, "GTAG_AUTODESTROY");
		SoundS.PlaySingleShot("/Ingame/Units/BarrelExplosion", new Vector3(blastCenter.x, blastCenter.y, 0f));
		PsAchievementManager.IncrementProgress("blowTwoHundredBarrels", 1);
		PsAchievementManager.IncrementProgress("blowTwoThousandBarrels", 1);
		Destroy();
	}
}
