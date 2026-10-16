using System.Collections.Generic;

public class PsStreak
{
	public List<PsCoin> m_coins;

	public int m_total;

	public int m_collected;

	public int m_tier;

	public PsStreak()
	{
		m_tier = -1;
	}

	public void SetCoins(List<PsCoin> _coins)
	{
		m_coins = _coins;
		m_total = _coins.Count;
		m_collected = 0;
	}
}
