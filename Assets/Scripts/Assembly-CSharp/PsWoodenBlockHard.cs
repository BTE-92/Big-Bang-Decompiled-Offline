using UnityEngine;

public class PsWoodenBlockHard : PsBlock
{
	public PsWoodenBlockHard(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_materialName = "WoodMaterial";
		CreatePhysicsAndModel();
		m_hitPoints = 1f;
		m_hitPointType = HitPointType.Lives;
		m_canBurn = true;
		float armorValue = m_area * 0.5f;
		SetBaseArmor(DamageType.Impact, armorValue);
	}

	public override void SetPhysicsParameters()
	{
		m_massMultiplier = 0.007f;
		m_friction = 0.9f;
		m_elasticity = 0.1f;
	}

	public override void SetAllBaseArmours()
	{
		base.SetAllBaseArmours();
		SetBaseArmor(DamageType.Weapon, 20f);
		SetBaseArmor(DamageType.Fire, 20f);
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		Vector2 vector = ChipmunkProWrapper.ucpBodyGetPos(m_cmb.body);
		SoundS.PlaySingleShot("/Ingame/Units/WoodenCrateDestroy", vector);
		EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject(RESOURCE.WoodenCratePuff_GameObject), new Vector3(vector.x, vector.y, 0f), Vector3.zero, 1f, "GTAG_AUTODESTROY");
		Destroy();
	}

	public override void SetBurning(bool _burn)
	{
		if (_burn)
		{
			PrefabS.SetShaderColor(m_pc, Color.red);
		}
		else
		{
			PrefabS.SetShaderColor(m_pc, Color.white);
		}
	}
}
