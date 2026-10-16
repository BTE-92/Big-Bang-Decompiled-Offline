using UnityEngine;

public class PsWoodenBlock : PsBlock
{
	public PsWoodenBlock(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_materialName = "CardboardMaterial";
		CreatePhysicsAndModel();
		m_cmb.m_identifier = 3;
		m_hitPoints = 1f;
		m_hitPointType = HitPointType.Lives;
		m_canBurn = true;
		float armorValue = m_area * 0.05f;
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
		PsAchievementManager.IncrementProgress("breakTwoThousandBoxes", 1);
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
