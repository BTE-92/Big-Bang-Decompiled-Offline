using System;
using System.Collections.Generic;
using System.Linq;
using MiniJSON;

public static class AchievementManager
{
	private static List<Achievement> m_achievements;

	private static bool m_initialized;

	public static List<Achievement> All
	{
		get
		{
			return m_achievements;
		}
	}

	public static List<Achievement> Completed
	{
		get
		{
			return m_achievements.FindAll((Achievement obj) => obj.completed);
		}
	}

	public static List<Achievement> Locked
	{
		get
		{
			return m_achievements.FindAll((Achievement obj) => !obj.completed);
		}
	}

	public static string AllJson
	{
		get
		{
			return ListToJson(All);
		}
	}

	public static string CompletedJson
	{
		get
		{
			return ListToJson(Completed);
		}
	}

	public static string LockedJson
	{
		get
		{
			return ListToJson(Locked);
		}
	}

	public static bool IsInitialized
	{
		get
		{
			return m_initialized;
		}
	}

	public static void Initialize(List<Achievement> _achievements)
	{
		m_achievements = _achievements;
		m_initialized = true;
	}

	public static bool IsCompleted(string _identifier)
	{
		if (m_initialized)
		{
			bool result = false;
			if (m_achievements.Exists((Achievement obj) => obj.id == _identifier))
			{
				result = m_achievements.Single((Achievement obj) => obj.id == _identifier).completed;
			}
			return result;
		}
		Debug.LogError("AchievementManager is not initialized!");
		return false;
	}

	public static List<string> IncrementProgress(string _identifier, int _value)
	{
		List<string> list = new List<string>();
		if (m_initialized)
		{
			foreach (Achievement item in m_achievements.Where((Achievement obj) => obj.id == _identifier))
			{
				if (item.percentCompleted < 100)
				{
					item.IncrementProgress(_value);
					list.Add(_identifier);
				}
			}
		}
		else
		{
			Debug.LogError("AchievementManager is not initialized!");
		}
		return list;
	}

	public static List<string> Complete(string _identifier)
	{
		List<string> list = new List<string>();
		if (m_initialized)
		{
			foreach (Achievement item in m_achievements.Where((Achievement obj) => obj.id == _identifier))
			{
				if (!item.completed)
				{
					item.Complete();
					list.Add(_identifier);
				}
			}
		}
		else
		{
			Debug.LogError("AchievementManager is not initialized!");
		}
		return list;
	}

	private static void UpdateProgress(string _identifier, int _value)
	{
		Debug.Log("Achievement update: " + _identifier + " Value: " + _value);
		foreach (Achievement item in m_achievements.Where((Achievement obj) => obj.id == _identifier))
		{
			item.UpdateProgress(_value);
		}
	}

	private static string ListToJson(List<Achievement> _list)
	{
		List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
		foreach (Achievement item in _list)
		{
			list.Add(item.ToDict());
		}
		return Json.Serialize(list);
	}

	public static void UpdateFromList(List<object> _list)
	{
		if (m_initialized)
		{
			foreach (Dictionary<string, object> item in _list)
			{
				if (!item.ContainsKey("id") && !item.ContainsKey("value"))
				{
					Debug.LogError("Invalid achievement dictionary");
				}
				else
				{
					Debug.Log((string)item["id"]);
					UpdateProgress((string)item["id"], Convert.ToInt32(item["value"]));
				}
			}
			return;
		}
		Debug.LogError("AchievementManager is not initialized!");
	}

	public static void UpdateFromJson(string _json)
	{
		UpdateFromList(Json.Deserialize(_json) as List<object>);
	}
}
