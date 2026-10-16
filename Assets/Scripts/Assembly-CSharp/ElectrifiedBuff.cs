using UnityEngine;

public class ElectrifiedBuff : Buff
{
	public ElectrifiedBuff()
	{
		m_identifier = "Electrified";
		m_beganEffect = new Damage(DamageType.Electric, float.MaxValue);
		m_isContagious = true;
		m_isCancellable = true;
	}

	public override void PlayEffects(Unit _unit, BuffState _state, Vector2 _pos)
	{
		switch (_state)
		{
		case BuffState.Began:
			EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject(RESOURCE.ElectricBurst_GameObject), _pos, Vector3.zero, 3f, "GTAG_AUTODESTROY");
			SoundS.PlaySingleShot("/Ingame/ElectricZap", _pos);
			_unit.SetElectrified(true);
			break;
		case BuffState.End:
			_unit.SetElectrified(false);
			break;
		}
	}
}
