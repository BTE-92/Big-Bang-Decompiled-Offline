using UnityEngine;

public class BuffInfo
{
	public Buff m_buff;

	public bool m_new;

	public IAssembledClass m_source;

	public IAssembledClass m_target;

	public Vector2 m_contactPoint;

	public float m_nextTick;

	public float m_lastTick;

	public BuffInfo(Buff _buff, IAssembledClass _source, IAssembledClass _target, Vector2 _contactPoint)
	{
		m_buff = _buff;
		m_new = true;
		m_source = _source;
		m_target = _target;
		m_contactPoint = _contactPoint;
		m_nextTick = -1f;
		m_lastTick = -1f;
	}
}
