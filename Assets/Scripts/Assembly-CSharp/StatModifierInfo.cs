public class StatModifierInfo
{
	public StatModifier m_modifier;

	public float m_endTime;

	public int m_stack;

	public int m_maxStack;

	public StatModifierInfo(StatModifier _modifier)
	{
		m_modifier = _modifier;
		m_endTime = -1f;
	}
}
