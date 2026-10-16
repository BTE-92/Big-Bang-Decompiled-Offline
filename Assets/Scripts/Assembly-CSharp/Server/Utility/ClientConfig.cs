using System;
using System.Collections.Generic;

namespace Server.Utility
{
	public class ClientConfig
	{
		public int carRefreshMinutes;

		public int freshFreeInterval;

		public int keysAtStart;

		public int diamondsAtStart;

		public int coinsAtStart;

		public int boltsAtStart;

		public int fbConnectReward;

		public int dailyGemAmount;

		public int videoAdCount;

		public int videoAdCoolDown;

		public int freshFreeCount;

		public int freshFreeCoolDown;

		public int inRaceDiamondSpawnProbability;

		public ClientConfig()
		{
			carRefreshMinutes = 6;
			freshFreeInterval = 15;
			fbConnectReward = 20;
			dailyGemAmount = 10;
			videoAdCount = 2;
			videoAdCoolDown = 3600;
			freshFreeCount = 3;
			freshFreeCoolDown = 1800;
			inRaceDiamondSpawnProbability = 25;
			boltsAtStart = 0;
			coinsAtStart = 1000;
			diamondsAtStart = 50;
			keysAtStart = 5;
		}

		public ClientConfig(Dictionary<string, object> _dictionary)
		{
			ParseFromDictionary(_dictionary);
		}

		public void ParseFromDictionary(Dictionary<string, object> _dictionary)
		{
			if (_dictionary.ContainsKey("carRefreshMinutes"))
			{
				carRefreshMinutes = Convert.ToInt32(_dictionary["carRefreshMinutes"]);
			}
			else
			{
				carRefreshMinutes = 6;
			}
			if (_dictionary.ContainsKey("freshFreeInterval"))
			{
				freshFreeInterval = Convert.ToInt32(_dictionary["freshFreeInterval"]);
			}
			else
			{
				freshFreeInterval = 15;
			}
			if (_dictionary.ContainsKey("fbConnectRewardAmount"))
			{
				fbConnectReward = Convert.ToInt32(_dictionary["fbConnectRewardAmount"]);
			}
			else
			{
				fbConnectReward = 20;
			}
			if (_dictionary.ContainsKey("dailyGemAmount"))
			{
				dailyGemAmount = Convert.ToInt32(_dictionary["dailyGemAmount"]);
			}
			else
			{
				dailyGemAmount = 10;
			}
			if (_dictionary.ContainsKey("videoAdCount"))
			{
				videoAdCount = Convert.ToInt32(_dictionary["videoAdCount"]);
			}
			else
			{
				videoAdCount = 2;
			}
			if (_dictionary.ContainsKey("videoAdCoolDown"))
			{
				videoAdCoolDown = Convert.ToInt32(_dictionary["videoAdCoolDown"]);
			}
			else
			{
				videoAdCoolDown = 3600;
			}
			if (_dictionary.ContainsKey("freshFreeCount"))
			{
				freshFreeCount = Convert.ToInt32(_dictionary["freshFreeCount"]);
			}
			else
			{
				freshFreeCount = 3;
			}
			if (_dictionary.ContainsKey("freshFreeCoolDown"))
			{
				freshFreeCoolDown = Convert.ToInt32(_dictionary["freshFreeCoolDown"]);
			}
			else
			{
				freshFreeCoolDown = 1800;
			}
			if (_dictionary.ContainsKey("inRaceDiamondSpawnProbability"))
			{
				inRaceDiamondSpawnProbability = Convert.ToInt32(_dictionary["inRaceDiamondSpawnProbability"]);
			}
			else
			{
				inRaceDiamondSpawnProbability = 25;
			}
			boltsAtStart = 0;
			coinsAtStart = 1000;
			diamondsAtStart = 50;
			keysAtStart = 5;
			if (_dictionary.ContainsKey("minimumRentPrice"))
			{
				PsState.m_minimumRentPrice = Convert.ToInt32(_dictionary["minimumRentPrice"]);
			}
			if (_dictionary.ContainsKey("rentDiamondMultiplier"))
			{
				PsState.m_rentPriceMultiplier = Convert.ToSingle(_dictionary["rentDiamondMultiplier"]);
			}
			if (_dictionary.ContainsKey("versusChallengeTryAmount"))
			{
				PsState.m_versusChallengeTryAmount = Convert.ToInt32(_dictionary["versusChallengeTryAmount"]);
			}
			if (_dictionary.ContainsKey("versusTokenMaxCount"))
			{
				PsState.m_versusTokenMaxCount = Convert.ToInt32(_dictionary["versusTokenMaxCount"]);
			}
			if (_dictionary.ContainsKey("versusRankCap"))
			{
				PsState.m_versusRankCap = Convert.ToInt32(_dictionary["versusRankCap"]);
			}
			if (_dictionary.ContainsKey("fixedFreshMinigameId"))
			{
				PsState.m_fixedFreshMinigameId = Convert.ToString(_dictionary["fixedFreshMinigameId"]);
			}
		}
	}
}
