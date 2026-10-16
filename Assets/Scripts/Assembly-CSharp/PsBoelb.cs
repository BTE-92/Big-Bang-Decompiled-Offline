using UnityEngine;

public class PsBoelb : Unit
{
	public TransformC m_tc;

	public PrefabC m_prefabC;

	public ChipmunkBodyC m_cmb;

	protected float m_massMultiplier = 0.007f;

	protected float m_friction = 0.4f;

	protected float m_elasticity = 0.25f;

	private BoelbAI m_boelbEffects;

	private bool m_playerInitialized;

	public PsBoelb(LevelCustomNode _graphNode)
		: base(_graphNode, UnitType.Basic)
	{
		m_tc = TransformS.AddComponent(m_entity, "Boelb");
		TransformS.SetTransform(m_tc, _graphNode.m_position, _graphNode.m_rotation);
		m_checkForCrushing = true;
		ucpPolyShape ucpPolyShape2 = new ucpPolyShape(new Vector2[5]
		{
			new Vector2(-20f, -20f),
			new Vector2(20f, -20f),
			new Vector2(20f, 15f),
			new Vector2(0f, 20f),
			new Vector2(-20f, 15f)
		}, Vector2.zero, 17895696u, 10f, 0f, 1f, (ucpCollisionType)4);
		ucpPolyShape2.mass = ucpPolyShape2.area * m_massMultiplier;
		m_cmb = ChipmunkProS.AddDynamicBody(m_tc, ucpPolyShape2);
		m_cmb.customComponent = m_unitC;
		m_cmb.m_identifier = 3;
		m_prefabC = PrefabS.AddComponent(m_tc, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.Boelb_GameObject));
		PrefabS.SetCamera(m_prefabC, CameraS.m_mainCamera);
		m_boelbEffects = m_prefabC.p_gameObject.GetComponent<BoelbAI>();
		m_hitPoints = 1f;
		m_hitPointType = HitPointType.Lives;
		m_canBurn = true;
		m_canElectrify = true;
		m_canFreeze = true;
		SetBaseArmor(DamageType.Impact, 145f);
		if (!m_minigame.m_editing)
		{
			CameraTargetC cameraTargetC = CameraS.AddTargetComponent(m_tc, 400f, 400f, Vector2.zero);
			cameraTargetC.activeRadius = 1250f;
		}
		else
		{
			CreateGraphElementTouchArea(20f, m_tc);
		}
		m_playerInitialized = false;
	}

	public override void Update()
	{
		base.Update();
		if (!m_playerInitialized && m_minigame.m_gameStarted && m_boelbEffects.lookAtTarget == null)
		{
			m_boelbEffects.lookAtTarget = m_minigame.m_playerTC.transform;
			m_playerInitialized = true;
		}
		if (m_contactState != ContactState.OnContact)
		{
		}
	}

	public override void SetAllBaseArmours()
	{
		base.SetAllBaseArmours();
		SetBaseArmor(DamageType.Weapon, 20f);
		SetBaseArmor(DamageType.Fire, 20f);
	}

	public override void Damage(Damage _damage, float _multiplier, Unit _source)
	{
		if (_source != null && _source.m_graphElement.GetType() == typeof(LevelPlayerNode))
		{
			_multiplier = 5f;
		}
		base.Damage(_damage, _multiplier, _source);
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		m_boelbEffects.Kill();
		Destroy();
	}

	public override void EmergencyKill()
	{
		if (!m_isDead)
		{
			Kill(DamageType.Impact, float.MaxValue);
		}
	}

	public override void SetBurning(bool _burn)
	{
		if (_burn)
		{
			PrefabS.SetShaderColor(m_prefabC, Color.red);
		}
		else
		{
			PrefabS.SetShaderColor(m_prefabC, Color.white);
		}
	}
}
