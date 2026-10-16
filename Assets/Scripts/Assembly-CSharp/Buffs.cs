using System.Collections;

public static class Buffs
{
	public static Hashtable m_buffs = new Hashtable();

	public static Buff GetBuff(string _identifier)
	{
		if (m_buffs.ContainsKey(_identifier))
		{
			return m_buffs[_identifier] as Buff;
		}
		return null;
	}

	public static void AddBuff(Buff _buff)
	{
		if (!m_buffs.ContainsKey(_buff.m_identifier))
		{
			m_buffs.Add(_buff.m_identifier, _buff);
		}
		else
		{
			Debug.LogError("trying to add dublicate buff " + _buff.m_identifier);
		}
	}

	public static void Initialize()
	{
		AddBuff(new BurningBuff());
		AddBuff(new ElectrifiedBuff());
	}
}
