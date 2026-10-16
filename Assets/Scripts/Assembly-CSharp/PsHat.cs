using UnityEngine;

public class PsHat : Unit
{
	public TransformC m_tc;

	public PrefabC m_prefabC;

	public ChipmunkBodyC m_cmb;

	protected float m_massMultiplier = 0.007f;

	protected float m_friction = 0.4f;

	protected float m_elasticity = 0.25f;

	public PsHat(Transform _hatTransform, Vector2 _linVel, float _angVel)
		: base(new GraphNode(GraphNodeType.Normal, "Hat"), UnitType.Basic)
	{
		EntityManager.AddTagForEntity(m_entity, "GTAG_AUTODESTROY");
		m_tc = TransformS.AddComponent(m_entity, "Hat");
		TransformS.SetTransform(m_tc, _hatTransform.position, _hatTransform.rotation.eulerAngles);
		m_checkForCrushing = true;
		Vector2 vector = new Vector2(_hatTransform.localPosition.x - _hatTransform.parent.transform.localPosition.x, _hatTransform.transform.localPosition.y - _hatTransform.parent.transform.localPosition.y);
		ucpPolyShape ucpPolyShape2 = ChipmunkProS.GeneratePolyShapeFromGameObject(_hatTransform.gameObject, -vector, 1f, m_elasticity, m_friction, (ucpCollisionType)4, 17895696u);
		ucpPolyShape2.mass = ucpPolyShape2.area * m_massMultiplier;
		m_cmb = ChipmunkProS.AddDynamicBody(m_tc, ucpPolyShape2);
		m_cmb.customComponent = m_unitC;
		m_cmb.m_identifier = 3;
		m_prefabC = PrefabS.AddComponent(m_tc, Vector3.zero, _hatTransform.gameObject);
		ChipmunkProWrapper.ucpBodySetVel(m_cmb.body, _linVel);
		ChipmunkProWrapper.ucpBodySetAngVel(m_cmb.body, _angVel);
		Object.Destroy(_hatTransform.gameObject);
		m_hitPoints = 1f;
		m_hitPointType = HitPointType.Lives;
		m_canBurn = true;
	}

	public override void SetAllBaseArmours()
	{
		base.SetAllBaseArmours();
		SetBaseArmor(DamageType.Impact, 145f);
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
		Vector2 vector = ChipmunkProWrapper.ucpBodyGetPos(m_cmb.body);
		SoundS.PlaySingleShot("/Ingame/Units/WoodenCrateDestroy", vector);
		EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject(RESOURCE.CardboardBoxPuff_GameObject), new Vector3(vector.x, vector.y, 0f), Vector3.zero, 2f, "GTAG_AUTODESTROY");
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
