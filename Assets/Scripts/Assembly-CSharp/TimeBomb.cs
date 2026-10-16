using UnityEngine;

public class TimeBomb : Unit
{
	private ChipmunkBodyC m_cmb;

	private PrefabC m_bombPrefabC;

	private TweenC m_tween;

	private Material m_lightMaterial;

	private float m_emissionVal;

	private bool m_dim;

	private bool m_updater;

	private TransformC m_tc;

	public TimeBomb(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_updater = false;
		m_emissionVal = 0f;
		m_dim = false;
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.FloatingBombPrefab_GameObject);
		m_lightMaterial = gameObject.transform.Find("Lights").GetComponent<Renderer>().sharedMaterial;
		m_tc = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(m_tc, _graphElement.m_position, _graphElement.m_rotation);
		m_bombPrefabC = PrefabS.AddComponent(m_tc, new Vector3(0f, 0f, 0f), gameObject);
		ucpCircleShape shape = new ucpCircleShape(35f, Vector2.zero, 17895696u, 15f, 0.1f, 0.9f, (ucpCollisionType)4);
		m_cmb = ChipmunkProS.AddDynamicBody(m_tc, shape);
		m_cmb.customComponent = m_unitC;
		if (!m_minigame.m_editing)
		{
			ChipmunkProS.SetBodyGravity(m_cmb, Vector2.zero);
			ChipmunkProS.SetBodyLinearDamp(m_cmb, new Vector2(0.993f, 0.993f));
			TransformC anchor = TransformS.AddComponent(m_entity, "RotaryLimit", _graphElement.m_position);
			ChipmunkProS.AddRotaryLimitJoint(m_cmb, ChipmunkProS.m_staticBody, anchor, 0f, 0f);
			m_tween = TweenS.AddTween(m_entity, TweenStyle.QuadInOut, -2.5f, 2.5f, 1f, 0f);
			TweenS.SetAdditionalTweenProperties(m_tween, -1, true, TweenStyle.QuadInOut);
		}
		CreateEditorTouchArea(50f, 50f);
		m_hitPoints = 1f;
		m_hitPointType = HitPointType.Lives;
		m_graphElement.m_isRotateable = false;
		m_checkForCrushing = true;
		m_reactToBlastWaves = true;
		int num = 0;
		for (int i = 0; i < PsS.m_units.m_aliveCount; i++)
		{
			if (num > 0)
			{
				break;
			}
			UnitC unitC = PsS.m_units.m_array[PsS.m_units.m_aliveIndices[i]];
			TimeBomb timeBomb = unitC.m_unit as TimeBomb;
			if (timeBomb != null && timeBomb != this && !timeBomb.m_isDead)
			{
				num++;
			}
			else if (timeBomb != null && timeBomb == this)
			{
				break;
			}
		}
		if (num == 0)
		{
			m_updater = true;
		}
		if (!m_minigame.m_editing)
		{
			CreateCamTarget();
		}
	}

	public override void SetAllBaseArmours()
	{
		base.SetAllBaseArmours();
		SetBaseArmor(DamageType.Impact, 3f);
		SetBaseArmor(DamageType.Electric, 10f);
	}

	public override void Damage(Damage _damage, float _multiplier, Unit _source)
	{
		if (_source != null && _source.m_graphElement.GetType() == typeof(LevelPlayerNode))
		{
			_multiplier = 0.2f;
		}
		base.Damage(_damage, _multiplier, _source);
	}

	public override void Destroy()
	{
		if (m_updater)
		{
			for (int i = 0; i < PsS.m_units.m_aliveCount; i++)
			{
				UnitC unitC = PsS.m_units.m_array[PsS.m_units.m_aliveIndices[i]];
				TimeBomb timeBomb = unitC.m_unit as TimeBomb;
				if (timeBomb != null && timeBomb != this)
				{
					timeBomb.m_updater = true;
					break;
				}
			}
		}
		base.Destroy();
	}

	public void CreateCamTarget()
	{
		CameraTargetC cameraTargetC = CameraS.AddTargetComponent(m_tc, 150f, 150f, new Vector2(0f, 0f));
		cameraTargetC.activeRadius = 500f;
	}

	public override void Update()
	{
		base.Update();
		if (m_isDead)
		{
			return;
		}
		if (m_updater)
		{
			m_lightMaterial.SetFloat("_Emission", m_emissionVal);
			m_emissionVal += ((!m_dim) ? 0.01f : (-0.01f));
			if ((m_dim && m_emissionVal <= 0f) || (!m_dim && m_emissionVal >= 0.5f))
			{
				m_dim = !m_dim;
			}
		}
		if (!m_minigame.m_editing)
		{
			m_bombPrefabC.p_gameObject.transform.Rotate(0f, -0.5f, 0f);
			m_bombPrefabC.p_gameObject.transform.localPosition = new Vector3(0f, m_tween.currentValue.x, 0f);
		}
	}

	public override void EmergencyKill()
	{
		if (!m_isDead)
		{
			Kill(DamageType.Impact, float.MaxValue);
		}
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		TweenS.RemoveComponent(m_tween);
		m_tween = null;
		Vector2 blastCenter = ChipmunkProWrapper.ucpBodyGetPos(m_cmb.body);
		PsS.ApplyBlastWave(blastCenter, new Vector2(50f, 4000f), 600f, 180f, 30f);
		EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject(RESOURCE.FloatingBombExplosion_GameObject), new Vector3(blastCenter.x, blastCenter.y, 0f), Vector3.zero, 2f, "GTAG_AUTODESTROY");
		SoundS.PlaySingleShot("/Ingame/Units/BarrelExplosion", new Vector3(blastCenter.x, blastCenter.y, 0f));
		Destroy();
	}
}
