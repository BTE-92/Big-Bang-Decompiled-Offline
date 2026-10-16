using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HighScores
{
	public static string TicksToTimeString(int _ticks)
	{
		float seconds = (float)_ticks / 60f;
		return ToolBox.getTimeStringFromSeconds(seconds);
	}

	public static float TicksToTime(int _ticks)
	{
		return (float)_ticks / 60f;
	}

	public static string TimeScoreToTimeString(int _score)
	{
		float seconds = (float)_score / 1000f / 60f;
		return ToolBox.getTimeStringFromSeconds(seconds);
	}

	public static float TimeScoreToTime(int _score)
	{
		return (float)_score / 1000f / 60f;
	}

	public static int TimeToTimeScore(float _time)
	{
		return (int)(_time * 1000f * 60f);
	}

	public static int TicksToTimeScore(int _ticks, bool _generateRandomFractions)
	{
		UnityEngine.Random.seed = (int)(Time.realtimeSinceStartup * 1000f);
		return _ticks * 1000 + (_generateRandomFractions ? UnityEngine.Random.Range(0, 900) : 0);
	}

	public static ScoreTable ParseScoreTableJSON(HttpC _c)
	{
		ScoreTable scoreTable = new ScoreTable();
		Dictionary<string, object> dictionary = ClientTools.ParseServerResponse(_c.www.text);
		if (dictionary.ContainsKey("topScores"))
		{
			List<object> list = dictionary["topScores"] as List<object>;
			HighscoreDataEntry[] array = new HighscoreDataEntry[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				Dictionary<string, object> dict = list[i] as Dictionary<string, object>;
				array[i] = ParseDataEntry(dict);
			}
			scoreTable.m_top = array;
		}
		if (dictionary.ContainsKey("followeeScores"))
		{
			List<object> list2 = dictionary["followeeScores"] as List<object>;
			HighscoreDataEntry[] array2 = new HighscoreDataEntry[list2.Count];
			for (int j = 0; j < list2.Count; j++)
			{
				Dictionary<string, object> dict2 = list2[j] as Dictionary<string, object>;
				array2[j] = ParseDataEntry(dict2);
			}
			scoreTable.m_friends = array2;
		}
		if (dictionary.ContainsKey("topRewards"))
		{
			List<object> list3 = dictionary["topRewards"] as List<object>;
			scoreTable.m_topRewards = new int[list3.Count];
			for (int k = 0; k < list3.Count; k++)
			{
				scoreTable.m_topRewards[k] = Convert.ToInt32(list3[k]);
			}
		}
		return scoreTable;
	}

	public static HighscoreData ParseHighscoreJSON(HttpC _c)
	{
		HighscoreData highscoreData = new HighscoreData();
		Dictionary<string, object> dictionary = ClientTools.ParseServerResponse(_c.www.text);
		List<object> list = dictionary["globalScores"] as List<object>;
		List<object> list2 = dictionary["followeeScores"] as List<object>;
		HighscoreDataEntry[] array = new HighscoreDataEntry[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			Dictionary<string, object> dict = list[i] as Dictionary<string, object>;
			array[i] = ParseDataEntry(dict);
		}
		HighscoreDataEntry[] array2 = new HighscoreDataEntry[list2.Count];
		for (int j = 0; j < list2.Count; j++)
		{
			Dictionary<string, object> dict2 = list2[j] as Dictionary<string, object>;
			array2[j] = ParseDataEntry(dict2);
		}
		highscoreData.m_entries = array;
		highscoreData.m_friends = array2;
		object value;
		if (dictionary.TryGetValue("percentile", out value))
		{
			highscoreData.m_percentile = Convert.ToInt32(value);
		}
		if (dictionary.TryGetValue("nextPercentileTime", out value))
		{
			highscoreData.m_nextPercentileTime = Convert.ToInt32(value);
		}
		if (dictionary.TryGetValue("position", out value))
		{
			highscoreData.m_position = Convert.ToInt32(value);
		}
		return highscoreData;
	}

	public static HighscoreDataEntry ParseDataEntryJSON(HttpC _c)
	{
		Dictionary<string, object> dict = ClientTools.ParseServerResponse(_c.www.text);
		HighscoreDataEntry highscoreDataEntry = ParseDataEntry(dict);
		GhostGetData ghostGetData = (GhostGetData)_c.objectData;
		if (ghostGetData.minigameInfo != null && highscoreDataEntry != null)
		{
			highscoreDataEntry.gameId = ghostGetData.minigameInfo.GetGameId();
		}
		return highscoreDataEntry;
	}

	public static HighscoreDataEntry ParseDataEntry(Dictionary<string, object> _dict)
	{
		if (!_dict.ContainsKey("n"))
		{
			return null;
		}
		HighscoreDataEntry highscoreDataEntry = new HighscoreDataEntry();
		highscoreDataEntry.name = (string)_dict["n"];
		if (_dict.ContainsKey("t"))
		{
			highscoreDataEntry.time = Convert.ToInt32(_dict["t"]);
		}
		if (_dict.ContainsKey("s"))
		{
			highscoreDataEntry.stars = Convert.ToInt32(_dict["s"]);
		}
		if (_dict.ContainsKey("fb"))
		{
			highscoreDataEntry.facebookId = (string)_dict["fb"];
		}
		if (_dict.ContainsKey("gc"))
		{
			highscoreDataEntry.gameCenterId = (string)_dict["gc"];
		}
		if (_dict.ContainsKey("p"))
		{
			highscoreDataEntry.playerId = (string)_dict["p"];
		}
		return highscoreDataEntry;
	}

	public static Hashtable GenerateDataEntryHashtable(HighscoreDataEntry _dataEntry)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("n", _dataEntry.name);
		hashtable.Add("t", _dataEntry.time);
		hashtable.Add("s", _dataEntry.stars);
		hashtable.Add("fb", _dataEntry.facebookId);
		hashtable.Add("gc", _dataEntry.gameCenterId);
		hashtable.Add("p", _dataEntry.playerId);
		return hashtable;
	}
}
