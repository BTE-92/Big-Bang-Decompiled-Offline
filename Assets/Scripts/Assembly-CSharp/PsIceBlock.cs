using UnityEngine;

public class PsIceBlock : PsBlock
{
	private const float m_scaleReduction = 0.0001f;

	private int m_meltTicks;

	private Vector3 m_scale;

	public PsIceBlock(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_materialName = "IceMaterial";
		CreatePhysicsAndModel();
		if (!m_minigame.m_editing)
		{
			ChipmunkProS.AddCollisionHandler(m_cmb, CollisionHandler, (ucpCollisionType)4, (ucpCollisionType)4, false, true, false);
			ChipmunkProS.AddCollisionHandler(m_cmb, CollisionHandler, (ucpCollisionType)4, (ucpCollisionType)3, false, true, false);
			ChipmunkProS.AddCollisionHandler(m_cmb, CollisionHandler, (ucpCollisionType)4, (ucpCollisionType)2, false, true, false);
		}
		m_meltTicks = 0;
		m_scale = m_cmb.TC.transform.localScale;
	}

	public override void SetPhysicsParameters()
	{
		m_massMultiplier = 0.01f;
		m_friction = 0.25f;
		m_elasticity = 0.1f;
	}

	public override void Update()
	{
		base.Update();
		if (!m_minigame.m_editing && m_minigame.m_gameStarted && !m_isDead)
		{
			m_meltTicks++;
			m_scale -= Vector3.one * 0.0001f;
			if (m_scale.x < 0.33f)
			{
				Kill(DamageType.Impact, 5000f);
			}
			else if (m_meltTicks % 10 == m_entity.m_index % 10)
			{
				m_cmb.TC.transform.localScale = new Vector3(m_scale.x, m_scale.y, m_scale.z * 0.95f);
				ChipmunkProWrapper.ucpBodySetMass(m_cmb.body, Mathf.Max(m_area * m_massMultiplier * m_scale.x, 0.2f));
				ChipmunkProWrapper.ucpBodyActivate(m_cmb.body);
			}
		}
	}

	private void CollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		float magnitude = _pair.friction.magnitude;
		m_scale -= Vector3.one * (magnitude / 100000f);
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		Vector2 vector = ChipmunkProWrapper.ucpBodyGetPos(m_cmb.body);
		SoundS.PlaySingleShot("/Ingame/Units/WoodenCrateDestroy", vector);
		EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject(RESOURCE.WoodenCratePuff_GameObject), new Vector3(vector.x, vector.y, 0f), Vector3.zero, 1f, "GTAG_AUTODESTROY");
		Destroy();
	}
}
