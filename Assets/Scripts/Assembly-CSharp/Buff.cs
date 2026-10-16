using UnityEngine;

public class Buff
{
	public string m_identifier;

	public bool m_isDebuff;

	public bool m_isContagious;

	public bool m_isCancellable;

	public float m_interval;

	public float m_duration;

	public Damage m_beganEffect;

	public Damage m_tickEffect;

	public Damage m_endEffect;

	public StatModifier m_shieldModifier;

	public StatModifier m_armorModifier;

	public Buff()
	{
		m_identifier = string.Empty;
		m_isDebuff = true;
		m_isContagious = false;
		m_isCancellable = false;
		m_interval = -1f;
		m_duration = -1f;
		m_beganEffect = null;
		m_tickEffect = null;
		m_endEffect = null;
		m_shieldModifier = null;
		m_armorModifier = null;
	}

	public virtual void PlayEffects(Unit _unit, BuffState _state, Vector2 _pos)
	{
	}
}
