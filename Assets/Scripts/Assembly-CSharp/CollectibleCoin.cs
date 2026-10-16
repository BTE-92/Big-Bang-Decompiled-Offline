using UnityEngine;

public class CollectibleCoin
{
	public PsCoin m_coin;

	public Entity m_entity;

	public ChipmunkBodyC m_body;

	private PrefabC m_prefab;

	private TransformC m_tc;

	public CollectibleCoin(PsCoin _coin)
	{
		m_coin = _coin;
		Vector2 pos = _coin.m_pos;
		m_entity = EntityManager.AddEntity(new string[2] { "GTAG_AUTODESTROY", "GTAG_COIN" });
		m_tc = TransformS.AddComponent(m_entity, "Coin");
		TransformS.SetTransform(m_tc, (Vector3)pos + new Vector3(0f, 0f, 0f), Vector3.zero);
		PsS.AddCustomObject(m_entity, this);
		if (_coin.m_streak.m_tier == -1)
		{
			float num = Mathf.Min(0.4f, (float)PsMetagameManager.m_playerStats.level * 0.01f);
			float num2 = Random.value - num;
			if (num2 < 0.04f - (0.4f - num))
			{
				m_coin.m_streak.m_tier = 3;
			}
			else if (num2 < 0.25f - (0.4f - num))
			{
				m_coin.m_streak.m_tier = 2;
			}
			else
			{
				m_coin.m_streak.m_tier = 1;
			}
			Debug.LogWarning("STREAK TIER: " + m_coin.m_streak.m_tier + ", PROGRESS: " + num + ", RANDOM: " + num2);
		}
		int tier = m_coin.m_streak.m_tier;
		int num3 = 0;
		switch (tier)
		{
		case 1:
			num3 = 0;
			break;
		case 2:
			num3 = 0;
			break;
		case 3:
			num3 = 0;
			if (m_coin.m_index <= 1)
			{
				num3 = 0;
			}
			else if (m_coin.m_streak.m_coins[m_coin.m_index - 1].m_value == 0)
			{
				num3 = 1;
			}
			break;
		}
		if (m_coin.m_index == 0)
		{
			switch (tier)
			{
			case 1:
				num3 = 1;
				break;
			case 2:
				num3 = 1;
				break;
			case 3:
				num3 = ((!(Random.value < 0.25f)) ? 1 : 2);
				break;
			}
		}
		else if (m_coin.m_index == m_coin.m_streak.m_total - 1)
		{
			switch (tier)
			{
			case 1:
				num3 = 1;
				break;
			case 2:
				num3 = ((!(Random.value < 0.25f)) ? 1 : 2);
				break;
			case 3:
				num3 = ((!(Random.value < 0.2f)) ? 3 : 4);
				break;
			}
		}
		m_coin.m_value = num3;
		SelectPrefab(num3);
		if (PsMetagameManager.m_globalCoinValueBoost)
		{
			SetCoinValueBoosterFX();
		}
		ucpShape shape = new ucpCircleShape(20f, Vector2.zero, 16777216u, 50f, 0.5f, 0.9f, (ucpCollisionType)9, true);
		m_body = ChipmunkProS.AddStaticBody(m_tc, shape);
		m_tc.m_identifier = num3;
		ChipmunkProS.AddCollisionHandler(m_body, CoinCollisionHandler, (ucpCollisionType)9, (ucpCollisionType)3, true, false, false);
	}

	private void SelectPrefab(int _value)
	{
		if (m_prefab != null)
		{
			PrefabS.RemoveComponent(m_prefab);
			m_prefab = null;
		}
		if (_value == 0)
		{
			m_prefab = PrefabS.AddComponent(m_tc, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.CollectibleCoin1Prefab_GameObject));
		}
		else if (_value == 1)
		{
			m_prefab = PrefabS.AddComponent(m_tc, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.CollectibleCoin5Prefab_GameObject));
		}
		else if (_value == 2)
		{
			m_prefab = PrefabS.AddComponent(m_tc, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.CollectibleCoin10Prefab_GameObject));
		}
		else if (_value == 3)
		{
			m_prefab = PrefabS.AddComponent(m_tc, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.CollectibleCoin100Prefab_GameObject));
		}
		else if (_value >= 4)
		{
			m_prefab = PrefabS.AddComponent(m_tc, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.CollectibleCoin500Prefab_GameObject));
		}
		PrefabS.SetCamera(m_prefab, CameraS.m_mainCamera);
	}

	public void SetCoinValueBoosterFX()
	{
		TransformS.SetScale(m_tc, new Vector3(1.5f, 1.5f, 1.5f));
	}

	private void CoinCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		if (m_entity != null)
		{
			Entity entity = EntityManager.AddEntity(new string[1] { "GTAG_AUTODESTROY" });
			m_coin.m_collected = true;
			TimerS.AddComponent(entity, "RemoveTimer", 1f, 0f, true);
			TransformC parentTC = TransformS.AddComponent(entity, m_body.TC.transform.position);
			PrefabC c = PrefabS.AddComponent(parentTC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.PickUpSparks_GameObject), string.Empty, false);
			PrefabS.SetCamera(c, CameraS.m_mainCamera);
			PsState.m_debugCoinsValue += PsState.COIN_VALUES[m_body.TC.m_identifier];
			PsState.m_debugCoinsCollected++;
			float num = (float)PsState.m_debugCoinsCollected * (3600f / PsState.m_debugCoinsTime);
			float num2 = (float)PsState.m_debugCoinsValue * (3600f / PsState.m_debugCoinsTime);
			Debug.Log("count: " + PsState.m_debugCoinsCollected + ", value: " + PsState.m_debugCoinsValue + ", coins per hour: " + num + ", value per hour: " + num2);
			PsMetagameManager.m_playerStats.coins += PsState.COIN_VALUES[m_body.TC.m_identifier] * ((!PsMetagameManager.m_globalCoinValueBoost) ? 1 : 2);
			PsMetagameManager.m_playerStats.updated = true;
			Vector2 vector = ChipmunkProWrapper.ucpBodyGetPos(m_body.body);
			SoundS.PlaySingleShotWithParameter("/Ingame/Units/GotCoin", new Vector3(vector.x, vector.y, 0f), "Value", m_body.TC.m_identifier + 1);
			EntityManager.RemoveEntity(m_entity);
			m_entity = null;
		}
	}
}
