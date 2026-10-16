using UnityEngine;

public class BurningBuff : Buff
{
	public BurningBuff()
	{
		m_identifier = "Burning";
		m_tickEffect = new Damage(DamageType.Fire, 1f);
		m_endEffect = new Damage(DamageType.Fire, float.MaxValue);
		m_interval = 0.5f;
		m_duration = 1f;
		m_isContagious = true;
	}

	public override void PlayEffects(Unit _unit, BuffState _state, Vector2 _pos)
	{
		switch (_state)
		{
		case BuffState.Tick:
			EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject(RESOURCE.ElectricBurst_GameObject), _pos, Vector3.zero, 3f, "GTAG_AUTODESTROY");
			SoundS.PlaySingleShot("/Ingame/ElectricZap", _pos);
			_unit.SetBurning(true);
			break;
		case BuffState.End:
			_unit.SetBurning(false);
			break;
		}
	}
}
