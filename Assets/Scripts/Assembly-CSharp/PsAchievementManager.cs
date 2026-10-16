using System.Collections.Generic;

public static class PsAchievementManager
{
	private static List<string> m_updated;

	public static bool IsInitialized
	{
		get
		{
			return AchievementManager.IsInitialized;
		}
	}

	public static List<Achievement> All
	{
		get
		{
			return AchievementManager.All;
		}
	}

	public static List<Achievement> Completed
	{
		get
		{
			return AchievementManager.Completed;
		}
	}

	public static List<Achievement> Locked
	{
		get
		{
			return AchievementManager.Locked;
		}
	}

	public static void Initialize()
	{
		m_updated = new List<string>();
		List<Achievement> list = new List<Achievement>();
		list.Add(new Achievement("unlockBike", "New vehicle!", "Unlocked motorbike."));
		list.Add(new Achievement("upgradeMax", "Grease monkey", "Upgrade any vehicle to the max."));
		list.Add(new Achievement("publishLevel", "Very constructive", "Published a level."));
		list.Add(new Achievement("challengeFriend", "A Challenger Appears", "Challenged a friend to a friend race."));
		list.Add(new Achievement("challengeAccepted", "Challenge accepted", "Accept a friend challenge."));
		list.Add(new Achievement("testFriendLevels", "Buddies", "Test a friend level from the friend list."));
		list.Add(new Achievement("triggerFortyChickens", "Birdie!", "Triggered 40 chicken cannons", 40));
		list.Add(new Achievement("blowTwoHundredBarrels", "This blows", "Blow up 200 barrels.", 200));
		list.Add(new Achievement("blowTwoThousandBarrels", "The Big Banger", "Blow up 2000 barrels.", 2000));
		list.Add(new Achievement("breakTwoHundredPlanks", "Chop it up!", "Break 200 wooden planks.", 200));
		list.Add(new Achievement("breakTwoThousandPlanks", "Professional logger", "Break 2000 wooden planks", 2000));
		list.Add(new Achievement("breakTwoThousandBoxes", "Logistics expert", "Break 2000 boxes", 2000));
		list.Add(new Achievement("hundredBackFlips", "Flippin' mad", "Do 100 backflips.", 100));
		list.Add(new Achievement("finishHardLevel", "It's tricky!", "Finish tricky level"));
		list.Add(new Achievement("impossibru", "Utterly insane", "Finish insane level."));
		list.Add(new Achievement("dealWithIt", "Hard as a... hard thing!", "Finish hard level."));
		list.Add(new Achievement("epicDealWithIt", "Deal with it.", "Finish ten hard levels.", 20));
		list.Add(new Achievement("gainFiftyGoldMedals", "Star by star", "Finish 50 times with 3 star rating", 50));
		list.Add(new Achievement("reachHundredHPCar", "Strong as a horse", "Upgrade car to 100hp", 100));
		list.Add(new Achievement("reachHundredHPBike", "Biker", "Upgrade dirt bike to 100hp", 100));
		list.Add(new Achievement("longTimeInEditor", "Real art takes time", "Spend 180 active minutes in the editor", 180));
		AchievementManager.Initialize(list);
	}

	public static bool IsCompleted(string _identifier)
	{
		if (AchievementManager.IsInitialized)
		{
			return AchievementManager.IsCompleted(_identifier);
		}
		return false;
	}

	public static void IncrementProgress(string _identifier, int _value)
	{
		if (!AchievementManager.IsInitialized)
		{
			return;
		}
		List<string> list = AchievementManager.IncrementProgress(_identifier, _value);
		foreach (string item in list)
		{
			if (m_updated != null && !m_updated.Contains(item))
			{
				m_updated.Add(item);
			}
		}
	}

	public static void Complete(string _identifier)
	{
		if (!AchievementManager.IsInitialized)
		{
			return;
		}
		List<string> list = AchievementManager.Complete(_identifier);
		foreach (string item in list)
		{
			if (m_updated != null && !m_updated.Contains(item))
			{
				m_updated.Add(item);
			}
		}
	}

	public static void SendToServer()
	{
	}

	public static List<Dictionary<string, object>> GetUpdatedAchievements()
	{
		List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
		if (AchievementManager.IsInitialized)
		{
			List<Achievement> list2 = All.FindAll((Achievement obj) => m_updated.Contains(obj.id));
			foreach (Achievement item in list2)
			{
				list.Add(item.ToDict());
			}
			m_updated.Clear();
		}
		return list;
	}

	public static void UpdateFromList(List<object> _list)
	{
		AchievementManager.UpdateFromList(_list);
	}

	private static void SendToServer(List<Achievement> _update)
	{
		foreach (Achievement item in _update)
		{
			PsMetagameManager.UpsertAchievement(item.ToJson());
		}
	}
}
