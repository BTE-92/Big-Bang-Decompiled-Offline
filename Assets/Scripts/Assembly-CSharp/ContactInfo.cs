using UnityEngine;

public class ContactInfo
{
	public ChipmunkBodyC m_contactBody;

	public ChipmunkBodyC m_cmb;

	public Vector2 m_contactPoint;

	public Ground m_ground;

	public Unit m_unit;

	public int m_contactCount;

	public bool m_began;

	public float m_beganTime;

	public bool m_end;

	public float m_endTime;

	public ContactInfo(ChipmunkBodyC _contactBody, ChipmunkBodyC _cmb, Ground _ground, Vector2 _contactPoint)
	{
		m_contactBody = _contactBody;
		m_cmb = _cmb;
		m_contactPoint = _contactPoint;
		m_ground = _ground;
		m_unit = null;
		m_beganTime = Main.m_resettingGameTime;
		m_endTime = -1f;
		m_contactCount = 1;
		m_began = true;
		m_end = false;
	}

	public ContactInfo(ChipmunkBodyC _contactBody, ChipmunkBodyC _cmb, Unit _unit, Vector2 _contactPoint)
	{
		m_contactBody = _contactBody;
		m_cmb = _cmb;
		m_contactPoint = _contactPoint;
		m_ground = null;
		m_unit = _unit;
		m_beganTime = Main.m_resettingGameTime;
		m_endTime = -1f;
		m_contactCount = 1;
		m_began = true;
		m_end = false;
	}
}
