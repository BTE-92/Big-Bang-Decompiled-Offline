using UnityEngine;

public class PsStoneBlock : PsBlock
{
	public PsStoneBlock(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_materialName = "MetalMaterial";
		CreatePhysicsAndModel();
		m_hitPoints = 1f;
		m_hitPointType = HitPointType.Lives;
		m_canElectrify = true;
	}

	public override void SetPhysicsParameters()
	{
		m_massMultiplier = 0.015f;
		m_friction = 0.9f;
		m_elasticity = 0.1f;
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		Vector2 vector = ChipmunkProWrapper.ucpBodyGetPos(m_cmb.body);
		SoundS.PlaySingleShot("/Ingame/Units/WoodenCrateDestroy", vector);
		EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject(RESOURCE.MetalBoxCrush_GameObject), new Vector3(vector.x, vector.y, 0f), Vector3.zero, 1f, "GTAG_AUTODESTROY");
		Destroy();
	}

	public override void SetElectrified(bool _electrify, bool _affectContacts, Vector2 _contactPoint)
	{
		base.SetElectrified(_electrify, _affectContacts, _contactPoint);
		if (!(m_pc.p_gameObject == null))
		{
			if (_electrify)
			{
				PrefabS.SetShaderColor(m_pc, Color.red);
			}
			else
			{
				PrefabS.SetShaderColor(m_pc, Color.white);
			}
		}
	}
}
