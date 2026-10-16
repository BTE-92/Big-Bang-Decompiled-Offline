using System;
using System.Collections;
using UnityEngine;

public class PlayerStats
{
	private bool m_isDirty;

	private int m_coins;

	private int m_diamonds;

	private int m_keys;

	private int m_bolts;

	private int m_stars;

	private int m_level;

	private bool m_updated;

	private bool m_upgraded;

	private int m_itemLevel;

	private int m_maxKeys;

	private int m_cups;

	private int m_rank = 1;

	private Hashtable m_upgrades = new Hashtable();

	private Hashtable m_boosters = new Hashtable();

	public bool isDirty
	{
		get
		{
			return m_isDirty;
		}
	}

	public int coins
	{
		get
		{
			return m_coins;
		}
		set
		{
			if (m_coins != value)
			{
				m_coins = value;
				m_isDirty = true;
			}
		}
	}

	public int diamonds
	{
		get
		{
			return m_diamonds;
		}
		set
		{
			if (m_diamonds != value)
			{
				m_diamonds = value;
				m_isDirty = true;
			}
		}
	}

	public int keys
	{
		get
		{
			return m_keys;
		}
		set
		{
			if (m_keys != value)
			{
				m_keys = value;
				m_isDirty = true;
			}
		}
	}

	public int bolts
	{
		get
		{
			return m_bolts;
		}
		set
		{
			if (m_bolts != value)
			{
				m_bolts = value;
				m_isDirty = true;
			}
		}
	}

	public int stars
	{
		get
		{
			return m_stars;
		}
		set
		{
			if (m_stars != value)
			{
				m_stars = value;
				m_isDirty = true;
			}
		}
	}

	public int level
	{
		get
		{
			return m_level;
		}
		set
		{
			if (m_level != value)
			{
				m_level = value;
				m_isDirty = true;
			}
		}
	}

	public bool updated
	{
		get
		{
			return m_updated;
		}
		set
		{
			if (m_updated != value)
			{
				m_updated = value;
				m_isDirty = true;
			}
		}
	}

	public bool upgraded
	{
		get
		{
			return m_upgraded;
		}
		set
		{
			if (m_upgraded != value)
			{
				m_upgraded = value;
				m_isDirty = true;
			}
		}
	}

	public int itemLevel
	{
		get
		{
			return m_itemLevel;
		}
		set
		{
			if (m_itemLevel != value)
			{
				m_itemLevel = value;
				m_isDirty = true;
			}
		}
	}

	public int maxKeys
	{
		get
		{
			return m_maxKeys;
		}
		set
		{
			if (m_maxKeys != value)
			{
				m_maxKeys = value;
				m_isDirty = true;
			}
		}
	}

	public int cups
	{
		get
		{
			return m_cups;
		}
		set
		{
			if (m_cups != value)
			{
				m_cups = value;
				m_isDirty = true;
			}
		}
	}

	public int rank
	{
		get
		{
			return m_rank;
		}
		set
		{
			if (m_rank != value)
			{
				m_rank = value;
				m_isDirty = true;
			}
		}
	}

	public Hashtable upgrades
	{
		get
		{
			return m_upgrades;
		}
		set
		{
			m_upgrades = value;
			m_isDirty = true;
		}
	}

	public Hashtable boosters
	{
		get
		{
			return m_boosters;
		}
		set
		{
			m_boosters = value;
			m_isDirty = true;
		}
	}

	public void SetDirty(bool value = true)
	{
		m_isDirty = value;
	}

	public int GetUpgradeValue(Type _vehicleType, string _key, int _defaultValue = 0)
	{
		if (m_upgrades.ContainsKey(_vehicleType.ToString()))
		{
			Hashtable hashtable = m_upgrades[_vehicleType.ToString()] as Hashtable;
			if (hashtable != null && hashtable.ContainsKey(_key))
			{
				return Convert.ToInt32(hashtable[_key]);
			}
			if (hashtable == null)
			{
				hashtable = new Hashtable();
			}
			hashtable.Add(_key, _defaultValue);
			m_upgrades[_vehicleType.ToString()] = hashtable;
			SetDirty();
		}
		else
		{
			Hashtable hashtable2 = new Hashtable();
			hashtable2.Add(_key, _defaultValue);
			m_upgrades.Add(_vehicleType.ToString(), hashtable2);
			SetDirty();
		}
		return _defaultValue;
	}

	public bool TierChanged(Type _vehicleType)
	{
		if (m_upgrades.ContainsKey(_vehicleType.ToString()))
		{
			Hashtable hashtable = m_upgrades[_vehicleType.ToString()] as Hashtable;
			if (hashtable != null && hashtable.Count >= 3)
			{
				int upgradeValue = GetUpgradeValue(_vehicleType, "tier", 1);
				int num = 999999;
				foreach (string key in hashtable.Keys)
				{
					if (!key.Equals("tier") && Convert.ToInt32(hashtable[key]) < num)
					{
						num = Convert.ToInt32(hashtable[key]);
					}
				}
				if (Mathf.FloorToInt((float)num / 5f) + 1 > upgradeValue)
				{
					hashtable["tier"] = upgradeValue + 1;
					m_upgrades[_vehicleType.ToString()] = hashtable;
					SetDirty();
					return true;
				}
				return false;
			}
			return false;
		}
		return false;
	}

	public void SetUpgradeValue(Type _vehicleType, string _key, int _value)
	{
		if (m_upgrades.ContainsKey(_vehicleType.ToString()))
		{
			Hashtable hashtable = m_upgrades[_vehicleType.ToString()] as Hashtable;
			if (hashtable == null || !hashtable.ContainsKey(_key))
			{
				if (hashtable == null)
				{
					hashtable = new Hashtable();
				}
				hashtable.Add(_key, _value);
			}
			else
			{
				hashtable[_key] = _value;
			}
			m_upgrades[_vehicleType.ToString()] = hashtable;
		}
		else
		{
			Hashtable hashtable2 = new Hashtable();
			hashtable2.Add(_key, _value);
			m_upgrades.Add(_vehicleType.ToString(), hashtable2);
		}
		PsMetrics.VehicleUpgrade(_vehicleType.ToString(), _key, _value.ToString());
		SetDirty();
	}

	private void SetBooster(int _value)
	{
		string key = "booster";
		if (m_boosters.ContainsKey(key))
		{
			m_boosters[key] = _value;
		}
		else
		{
			m_boosters.Add(key, _value);
		}
		SetDirty();
	}

	public void CumulateBoosters(int _amount)
	{
		SetBooster(GetBoosterValue() + _amount);
	}

	public int GetBoosterValue()
	{
		string key = "booster";
		if (!m_boosters.ContainsKey(key))
		{
			m_boosters[key] = 0;
		}
		return Convert.ToInt32(m_boosters[key]);
	}

	public void CumulateUpgradeValue(Type _vehicleType, string _key)
	{
		if (m_upgrades.ContainsKey(_vehicleType.ToString()))
		{
			Hashtable hashtable = m_upgrades[_vehicleType.ToString()] as Hashtable;
			if (hashtable == null || !hashtable.ContainsKey(_key))
			{
				if (hashtable == null)
				{
					hashtable = new Hashtable();
				}
				hashtable.Add(_key, 1);
			}
			else
			{
				hashtable[_key] = Convert.ToInt32(hashtable[_key]) + 1;
			}
			m_upgrades[_vehicleType.ToString()] = hashtable;
		}
		else
		{
			Hashtable hashtable2 = new Hashtable();
			hashtable2.Add(_key, 1);
			m_upgrades.Add(_vehicleType.ToString(), hashtable2);
		}
		SetDirty();
	}

	public void CumulateCoins(int _amount)
	{
		m_coins += _amount;
		if (PsMetagameManager.m_menuResourceView != null && PsMetagameManager.m_menuResourceView.m_showCoins)
		{
			PsMetagameManager.m_menuResourceView.CreateAddedResources(ResourceType.Coins, _amount, 0f);
		}
		SetDirty();
	}

	public void CumulateCoinsWithFlyingResources(int _amount, Vector2 _startPos, float _delay = 0f)
	{
		m_coins += _amount;
		if (PsMetagameManager.m_menuResourceView != null)
		{
			PsMetagameManager.m_menuResourceView.CreateFlyingResources(_amount, _startPos, ResourceType.Coins, _delay);
		}
		SetDirty();
	}

	public void CumulateDiamonds(int _amount)
	{
		m_diamonds += _amount;
		if (PsMetagameManager.m_menuResourceView != null && PsMetagameManager.m_menuResourceView.m_showDiamonds)
		{
			PsMetagameManager.m_menuResourceView.CreateAddedResources(ResourceType.Diamonds, _amount, 0f);
		}
		SetDirty();
	}

	public void CumulateDiamondsWithFlyingResources(int _amount, Vector2 _startPos, float _delay = 0f)
	{
		m_diamonds += _amount;
		if (PsMetagameManager.m_menuResourceView != null)
		{
			PsMetagameManager.m_menuResourceView.CreateFlyingResources(_amount, _startPos, ResourceType.Diamonds, _delay);
		}
		SetDirty();
	}

	public void CumulateKeysWithFlyingResources(int _amount, Vector2 _startPos, float _delay = 0f)
	{
		m_keys = Math.Min(m_keys + _amount, PsMetagameManager.m_playerStats.maxKeys);
		if (PsMetagameManager.m_menuResourceView != null)
		{
			PsMetagameManager.m_menuResourceView.CreateFlyingResources(_amount, _startPos, ResourceType.Keys, _delay);
		}
		SetDirty();
	}

	public void CreateFlyingTemplate(int _amount, Vector2 _startPos, Vector2 _endPos, float _delay = 0f)
	{
		if (PsMetagameManager.m_menuResourceView != null)
		{
			PsMetagameManager.m_menuResourceView.CreateFlyingResources(_amount, _startPos, ResourceType.Templates, _delay, null, null, null, null, _endPos);
		}
	}

	public void CumulateKeys(int _amount)
	{
		m_keys = Math.Min(m_keys + _amount, PsMetagameManager.m_playerStats.maxKeys);
		if (PsMetagameManager.m_menuResourceView != null && PsMetagameManager.m_menuResourceView.m_showKeys)
		{
			PsMetagameManager.m_menuResourceView.CreateAddedResources(ResourceType.Keys, _amount, 0f);
		}
		SetDirty();
	}

	public void CumulateStars(int _amount)
	{
		m_stars += _amount;
		SetDirty();
	}

	public void CumulateBolts(int _amount)
	{
		m_bolts += _amount;
		if (PsMetagameManager.m_menuResourceView != null && PsMetagameManager.m_menuResourceView.m_showBolts)
		{
			PsMetagameManager.m_menuResourceView.CreateAddedResources(ResourceType.Bolts, _amount, 0f);
		}
		SetDirty();
	}

	public void CumulateBoltsWithFlyingResources(int _amount, Vector2 _startPos, float _delay = 0f)
	{
		if (PsMetagameManager.m_menuResourceView.m_boltRow == null)
		{
			PsMetagameManager.m_menuResourceView.CreateBoltRow(true);
		}
		m_bolts += _amount;
		if (PsMetagameManager.m_menuResourceView != null)
		{
			PsMetagameManager.m_menuResourceView.CreateFlyingResources(_amount, _startPos, ResourceType.Bolts, _delay);
		}
		SetDirty();
	}
}
