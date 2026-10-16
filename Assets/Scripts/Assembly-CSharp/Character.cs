using System.Collections.Generic;

public class Character : Unit
{
	public List<Container> m_itemContainers;

	public Shield m_shield;

	public Armor m_armor;

	public Weapon m_weapon;

	public Character(GraphElement _graphElement)
		: base(_graphElement, UnitType.Character)
	{
	}

	public override void CalculateCurrentShield()
	{
		for (int i = 0; i < m_currentShield.Length; i++)
		{
			m_currentShield[i] = m_baseShield[i];
			if (m_shield != null)
			{
				m_currentShield[i] += m_shield.m_shieldModifier[i];
			}
			if (m_armor != null)
			{
				m_currentShield[i] += m_armor.m_shieldModifier[i];
			}
			if (m_weapon != null)
			{
				m_currentShield[i] += m_weapon.m_shieldModifier[i];
			}
			for (int j = 0; j < m_shieldModifiers.Count; j++)
			{
				m_currentShield[i] *= m_shieldModifiers[j].m_modifier.m_multipler;
			}
		}
	}

	public override void CalculateCurrentArmor()
	{
		for (int i = 0; i < m_currentArmor.Length; i++)
		{
			m_currentArmor[i] = m_baseArmor[i];
			if (m_shield != null)
			{
				m_currentShield[i] += m_shield.m_armorModifier[i];
			}
			if (m_armor != null)
			{
				m_currentShield[i] += m_armor.m_armorModifier[i];
			}
			if (m_weapon != null)
			{
				m_currentShield[i] += m_weapon.m_armorModifier[i];
			}
			for (int j = 0; j < m_armorModifiers.Count; j++)
			{
				m_currentArmor[i] *= m_armorModifiers[j].m_modifier.m_multipler;
			}
		}
	}
}
