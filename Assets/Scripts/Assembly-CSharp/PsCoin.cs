using UnityEngine;

public class PsCoin
{
	public PsStreak m_streak;

	public Vector2 m_pos;

	public bool m_collected;

	public int m_index;

	public int m_value;

	public PsCoin(Vector2 _pos, PsStreak _streak, int _index)
	{
		m_streak = _streak;
		m_pos = _pos;
		m_index = _index;
		m_collected = false;
	}
}
