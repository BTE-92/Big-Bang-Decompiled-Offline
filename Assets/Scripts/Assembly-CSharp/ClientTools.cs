using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MiniJSON;
using Server.Utility;
using UnityEngine;

public static class ClientTools
{
	public static string gpw(string s)
	{
		UnityEngine.Random.seed = 97139634;
		char[] array = s.ToCharArray();
		char[] value = ToolBox.shuffleArray(array);
		return new string(value);
	}

	public static string GenerateHash(string _string)
	{
		string strToEncrypt = _string + gpw(ServerConfig.GetHPW());
		return ToolBox.SHA256Sum(strToEncrypt);
	}

	public static string GenerateHash(string _string, byte[] _data)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		byte[] bytes = uTF8Encoding.GetBytes(_string + gpw(ServerConfig.GetHPW()));
		byte[] array = new byte[_data.Length + bytes.Length];
		_data.CopyTo(array, 0);
		bytes.CopyTo(array, _data.Length);
		return ToolBox.SHA256Sum(array);
	}

	public static string GenerateHash(byte[] _data)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		byte[] bytes = uTF8Encoding.GetBytes(gpw(ServerConfig.GetHPW()));
		byte[] array = new byte[_data.Length + bytes.Length];
		_data.CopyTo(array, 0);
		bytes.CopyTo(array, _data.Length);
		return ToolBox.SHA256Sum(array);
	}

	public static string GenerateHash(PlayerData _data)
	{
		string empty = string.Empty;
		empty += _data.playerId;
		empty += _data.coins;
		empty += _data.diamonds;
		empty += _data.bolts;
		empty += _data.cups;
		empty += _data.keys;
		empty += _data.maxKeys;
		empty += _data.rank;
		empty += gpw(ServerConfig.GetHPW());
		return ToolBox.SHA256Sum(empty);
	}

	public static Dictionary<string, object> ParseServerResponse(string _response)
	{
		if (!_response.StartsWith("{"))
		{
			Debug.LogError("Server response not valid JSON: " + _response);
			return null;
		}
		Debug.Log("<color=cyan>RESPONSE</color>: " + _response);
		Dictionary<string, object> dictionary = Json.Deserialize(_response) as Dictionary<string, object>;
		if (ServerResponseOk(dictionary))
		{
			return dictionary;
		}
		Debug.LogWarning("ERROR RESPONSE: " + dictionary["error"]);
		return dictionary;
	}

	public static bool ServerResponseOk(Dictionary<string, object> dict)
	{
		if (dict == null)
		{
			Debug.LogError("NULL DICTIONARY!!!");
			return false;
		}
		return true;
	}

	public static GhostData[] ParseGhostData(HttpC _c)
	{
		GhostData ghostData = default(GhostData);
		Dictionary<string, string> responseHeaders = _c.www.responseHeaders;
		if (responseHeaders.ContainsKey("GHOST_TIME"))
		{
			ghostData.time = Convert.ToInt32(responseHeaders["GHOST_TIME"]);
		}
		if (responseHeaders.ContainsKey("GHOST_NAME"))
		{
			ghostData.name = WWW.UnEscapeURL(responseHeaders["GHOST_NAME"], Encoding.UTF8);
		}
		byte[] bytes = _c.www.bytes;
		ghostData.ghost = bytes;
		return new GhostData[1] { ghostData };
	}

	public static GhostData[] ParseGhostDatas(HttpC _c)
	{
		Dictionary<string, string> responseHeaders = _c.www.responseHeaders;
		string text = string.Empty;
		if (responseHeaders.ContainsKey("FILE_SIZES"))
		{
			text = responseHeaders["FILE_SIZES"];
		}
		int[] array = (from x in text.Split(',')
			select Convert.ToInt32(x)).ToArray();
		byte[] bytes = _c.www.bytes;
		GhostData[] array2 = new GhostData[array.Length / 2];
		for (int num = 0; num < array.Length; num += 2)
		{
			byte[] source = bytes.Skip(array.Take(num).Sum()).ToArray();
			GhostData ghostData = default(GhostData);
			byte[] bytes2 = source.Take(array[num]).ToArray();
			byte[] source2 = source.Skip(array[num]).ToArray();
			ghostData.ghost = source2.Take(array[num + 1]).ToArray();
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(uTF8Encoding.GetString(bytes2));
			if (dictionary.ContainsKey("name"))
			{
				ghostData.name = (string)dictionary["name"];
			}
			if (dictionary.ContainsKey("time"))
			{
				ghostData.time = Convert.ToInt32(dictionary["time"]);
			}
			array2[num / 2] = ghostData;
		}
		return array2;
	}

	public static byte[] ParseBytes(HttpC _c)
	{
		return _c.www.bytes;
	}

	public static Texture2D ParseTexture(HttpC _c)
	{
		Texture2D texture2D = new Texture2D(10, 10, TextureFormat.RGB24, false);
		_c.www.LoadImageIntoTexture(texture2D);
		return texture2D;
	}

	public static ClientConfig ParsePlayerConfig(Dictionary<string, object> _dictionary, ClientConfig _c)
	{
		Debug.Log("Parse initial player config");
		if (_dictionary.ContainsKey("coins"))
		{
			_c.coinsAtStart = Convert.ToInt32(_dictionary["coins"]);
		}
		else
		{
			_c.coinsAtStart = 1000;
		}
		if (_dictionary.ContainsKey("diamonds"))
		{
			_c.diamondsAtStart = Convert.ToInt32(_dictionary["diamonds"]);
		}
		else
		{
			_c.diamondsAtStart = 50;
		}
		if (_dictionary.ContainsKey("bolts"))
		{
			_c.boltsAtStart = Convert.ToInt32(_dictionary["bolts"]);
		}
		else
		{
			_c.boltsAtStart = 0;
		}
		if (_dictionary.ContainsKey("keys"))
		{
			_c.keysAtStart = Convert.ToInt32(_dictionary["keys"]);
		}
		else
		{
			_c.keysAtStart = 5;
		}
		return _c;
	}

	public static PlayerStatistics ParsePlayerStatistics(HttpC _c)
	{
		Dictionary<string, object> dictionary = ParseServerResponse(_c.www.text);
		return ParsePlayerStatistics(dictionary);
	}

	public static PlayerStatistics ParsePlayerStatistics(Dictionary<string, object> _dictionary)
	{
		PlayerStatistics result = default(PlayerStatistics);
		if (_dictionary.ContainsKey("versusPlays"))
		{
			result.versusPlays = Convert.ToInt32(_dictionary["versusPlays"]);
		}
		if (_dictionary.ContainsKey("versusWins"))
		{
			result.versusWins = Convert.ToInt32(_dictionary["versusWins"]);
		}
		if (_dictionary.ContainsKey("versusLosses"))
		{
			result.versusLosses = Convert.ToInt32(_dictionary["versusLosses"]);
		}
		if (_dictionary.ContainsKey("mcVersusStats"))
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)_dictionary["mcVersusStats"];
			if (dictionary.ContainsKey("totalPlays"))
			{
				result.motorcycleVersusPlays = Convert.ToInt32(dictionary["totalPlays"]);
			}
			if (dictionary.ContainsKey("wins"))
			{
				result.motorcycleVersusWins = Convert.ToInt32(dictionary["wins"]);
			}
			if (dictionary.ContainsKey("losses"))
			{
				result.motorcycleVersusLosses = Convert.ToInt32(dictionary["losses"]);
			}
		}
		if (_dictionary.ContainsKey("carVersusStats"))
		{
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)_dictionary["carVersusStats"];
			if (dictionary2.ContainsKey("totalPlays"))
			{
				result.offroadCarVersusPlays = Convert.ToInt32(dictionary2["totalPlays"]);
			}
			if (dictionary2.ContainsKey("wins"))
			{
				result.offroadCarVersusWins = Convert.ToInt32(dictionary2["wins"]);
			}
			if (dictionary2.ContainsKey("losses"))
			{
				result.offroadCarVersusLosses = Convert.ToInt32(dictionary2["losses"]);
			}
		}
		return result;
	}

	public static List<string> ParseAvailableEditorItemsList(HttpC _c)
	{
		Dictionary<string, object> dictionary = ParseServerResponse(_c.www.text);
		List<object> data = dictionary["data"] as List<object>;
		return ParseAvailableEditorItems(data);
	}

	public static List<string> ParseAvailableEditorItems(List<object> _data)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < _data.Count; i++)
		{
			Dictionary<string, object> dictionary = _data[i] as Dictionary<string, object>;
			list.Add((string)dictionary["className"]);
		}
		return list;
	}

	public static CommentData[] ParseComments(HttpC _c)
	{
		Dictionary<string, object> dictionary = ParseServerResponse(_c.www.text);
		List<object> list = dictionary["data"] as List<object>;
		CommentData[] array = new CommentData[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			Dictionary<string, object> dictionary2 = list[i] as Dictionary<string, object>;
			array[i].playerId = (string)dictionary2["playerId"];
			array[i].comment = (string)dictionary2["comment"];
			array[i].name = (string)dictionary2["name"];
			if (dictionary2.ContainsKey("tag"))
			{
				array[i].tag = (string)dictionary2["tag"];
			}
			if (dictionary2.ContainsKey("facebookId"))
			{
				array[i].facebookId = (string)dictionary2["facebookId"];
			}
			if (dictionary2.ContainsKey("gameCenterId"))
			{
				array[i].gameCenterId = (string)dictionary2["gameCenterId"];
			}
		}
		return array;
	}

	public static List<string> ParseLikes(HttpC _c)
	{
		Dictionary<string, object> dictionary = ParseServerResponse(_c.www.text);
		object[] array = (dictionary["data"] as List<object>).ToArray();
		List<string> list = new List<string>();
		for (int i = 0; i < array.Length; i++)
		{
			list.Add((string)array[i]);
		}
		return list;
	}

	public static FeedData[] ParseGameFeedList(HttpC _c)
	{
		Dictionary<string, object> dictionary = ParseServerResponse(_c.www.text);
		List<object> list = dictionary["data"] as List<object>;
		FeedData[] array = new FeedData[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			Dictionary<string, object> dictionary2 = list[i] as Dictionary<string, object>;
			string text = (string)dictionary2["type"];
			array[i].targetId = (string)dictionary2["targetId"];
			array[i].type = text;
			Dictionary<string, object> dictionary3 = (Dictionary<string, object>)dictionary2["ts"];
			array[i].timestamp = new DateTime((long)dictionary3["$date"]);
			array[i].message = (string)dictionary2["message"];
			if (!text.Equals("game"))
			{
				continue;
			}
			if (dictionary2.ContainsKey("friendCreator"))
			{
				array[i].friendCreator = (string)dictionary2["friendCreator"];
			}
			if (dictionary2.ContainsKey("friendPlayers"))
			{
				object[] array2 = (dictionary2["friendPlayers"] as List<object>).ToArray();
				array[i].friendPlayers = new string[array2.Length];
				for (int j = 0; j < array2.Length; j++)
				{
					array[i].friendPlayers[j] = (string)array2[j];
				}
			}
		}
		return array;
	}

	public static PsMinigameMetaData ParseTemplate(HttpC _c)
	{
		PsMinigameMetaData psMinigameMetaData = ParseMinigameMetaData(_c);
		psMinigameMetaData.template = true;
		return psMinigameMetaData;
	}

	public static PsMinigameMetaData ParseMinigameMetaData(HttpC _c)
	{
		Dictionary<string, object> dict = ParseServerResponse(_c.www.text);
		return ParseMinigameMetaData(dict);
	}

	public static PsMinigameMetaData ParseTemplate(Dictionary<string, object> _dict)
	{
		if (_dict == null || _dict.Count <= 1)
		{
			return null;
		}
		PsMinigameMetaData psMinigameMetaData = ParseMinigameMetaData(_dict);
		psMinigameMetaData.template = true;
		return psMinigameMetaData;
	}

	public static PsMinigameMetaData ParseMinigameMetaData(Dictionary<string, object> _dict)
	{
		if (_dict == null || _dict.Count <= 1)
		{
			return null;
		}
		if (!_dict.ContainsKey("error"))
		{
			PsMinigameMetaData psMinigameMetaData = new PsMinigameMetaData();
			psMinigameMetaData.name = (string)_dict["name"];
			psMinigameMetaData.id = (string)_dict["id"];
			if (_dict.ContainsKey("creatorId"))
			{
				psMinigameMetaData.creatorId = (string)_dict["creatorId"];
			}
			else
			{
				psMinigameMetaData.creatorId = "??";
			}
			if (_dict.ContainsKey("timeSpentEditing"))
			{
				psMinigameMetaData.timeSpentEditing = Convert.ToInt32(_dict["timeSpentEditing"]);
			}
			if (_dict.ContainsKey("timeSpentInEditMode"))
			{
				psMinigameMetaData.timeSpentInEditMode = Convert.ToInt32(_dict["timeSpentInEditMode"]);
			}
			if (_dict.ContainsKey("editSessionCount"))
			{
				psMinigameMetaData.editSessionCount = Convert.ToInt32(_dict["editSessionCount"]);
			}
			if (_dict.ContainsKey("groundsModificationCount"))
			{
				psMinigameMetaData.groundsModificationCount = Convert.ToInt32(_dict["groundsModificationCount"]);
			}
			if (_dict.ContainsKey("itemsModificationCount"))
			{
				psMinigameMetaData.itemsModificationCount = Convert.ToInt32(_dict["itemsModificationCount"]);
			}
			if (_dict.ContainsKey("lastPlaySessionStartCount"))
			{
				psMinigameMetaData.lastPlaySessionStartCount = Convert.ToInt32(_dict["lastPlaySessionStartCount"]);
			}
			if (_dict.ContainsKey("creatorFacebookId"))
			{
				psMinigameMetaData.creatorFacebookId = (string)_dict["creatorFacebookId"];
			}
			if (_dict.ContainsKey("creatorGameCenterId"))
			{
				psMinigameMetaData.creatorGameCenterId = (string)_dict["creatorGameCenterId"];
			}
			if (_dict.ContainsKey("creatorName"))
			{
				psMinigameMetaData.creatorName = (string)_dict["creatorName"];
			}
			else
			{
				psMinigameMetaData.creatorName = "??";
			}
			if (_dict.ContainsKey("publishTime"))
			{
				psMinigameMetaData.published = true;
			}
			else
			{
				psMinigameMetaData.published = false;
			}
			if (_dict.ContainsKey("description"))
			{
				psMinigameMetaData.description = (string)_dict["description"];
			}
			if (_dict.ContainsKey("timesLiked"))
			{
				psMinigameMetaData.timesLiked = Convert.ToInt32(_dict["timesLiked"]);
			}
			if (_dict.ContainsKey("timesRated"))
			{
				psMinigameMetaData.timesRated = Convert.ToInt32(_dict["timesRated"]);
			}
			if (_dict.ContainsKey("timesPlayed"))
			{
				psMinigameMetaData.timesPlayed = Convert.ToInt32(_dict["timesPlayed"]);
			}
			else
			{
				psMinigameMetaData.timesPlayed = 0;
			}
			if (_dict.ContainsKey("clientVersion"))
			{
				double num = Convert.ToDouble(_dict["clientVersion"]);
				psMinigameMetaData.clientVersion = (int)num;
			}
			if (_dict.ContainsKey("played"))
			{
				psMinigameMetaData.played = (bool)_dict["played"];
			}
			if (_dict.ContainsKey("score"))
			{
				psMinigameMetaData.score = Convert.ToInt32(_dict["score"]);
			}
			if (_dict.ContainsKey("researchIdentifier"))
			{
				psMinigameMetaData.researchIdentifier = (string)_dict["researchIdentifier"];
			}
			if (_dict.ContainsKey("median"))
			{
				psMinigameMetaData.medianTime = Convert.ToInt32(_dict["median"]);
			}
			if (_dict.ContainsKey("totalWinners"))
			{
				psMinigameMetaData.totalWinners = Convert.ToInt32(_dict["totalWinners"]);
			}
			if (_dict.ContainsKey("oneStarWinners"))
			{
				psMinigameMetaData.oneStarWinners = Convert.ToInt32(_dict["oneStarWinners"]);
			}
			if (_dict.ContainsKey("twoStarWinners"))
			{
				psMinigameMetaData.twoStarWinners = Convert.ToInt32(_dict["twoStarWinners"]);
			}
			if (_dict.ContainsKey("threeStarWinners"))
			{
				psMinigameMetaData.threeStarWinners = Convert.ToInt32(_dict["threeStarWinners"]);
			}
			if (_dict.ContainsKey("participantCount"))
			{
				psMinigameMetaData.totalPlayers = Convert.ToInt32(_dict["participantCount"]);
			}
			if (_dict.ContainsKey("complexity"))
			{
				Convert.ToInt32(_dict["complexity"]);
			}
			if (_dict.ContainsKey("levelRequirement"))
			{
				double num2 = Convert.ToDouble(_dict["levelRequirement"]);
				psMinigameMetaData.levelRequirement = (int)num2;
			}
			if (_dict.ContainsKey("playerUnit"))
			{
				psMinigameMetaData.playerUnit = (string)_dict["playerUnit"];
			}
			if (_dict.ContainsKey("rating"))
			{
				string rating = (string)_dict["rating"];
				psMinigameMetaData.rating = ParseRating(rating);
			}
			if (_dict.ContainsKey("gameQuality"))
			{
				psMinigameMetaData.quality = (float)Convert.ToDouble(_dict["gameQuality"]);
			}
			else
			{
				Debug.LogError("NO GAME QUALITY");
			}
			if (_dict.ContainsKey("itemsUsed"))
			{
				object[] array = (_dict["itemsUsed"] as List<object>).ToArray();
				psMinigameMetaData.itemsUsed = new string[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					psMinigameMetaData.itemsUsed[i] = (string)array[i];
				}
			}
			if (_dict.ContainsKey("difficulty"))
			{
				string difficulty = (string)_dict["difficulty"];
				psMinigameMetaData.difficulty = ParseDifficulty(difficulty);
			}
			else
			{
				psMinigameMetaData.difficulty = PsGameDifficulty.New;
			}
			if (_dict.ContainsKey("gameMode"))
			{
				switch ((string)_dict["gameMode"])
				{
				case "Race":
					psMinigameMetaData.gameMode = PsGameMode.Race;
					break;
				case "StarCollect":
					psMinigameMetaData.gameMode = PsGameMode.StarCollect;
					break;
				}
			}
			else
			{
				psMinigameMetaData.gameMode = PsGameMode.Race;
			}
			if (psMinigameMetaData.creatorId == PlayerPrefsX.GetUserId())
			{
				if (_dict.ContainsKey("rewardCoins"))
				{
					psMinigameMetaData.rewardCoins = Convert.ToInt32(_dict["rewardCoins"]);
				}
				if (_dict.ContainsKey("totalCoinsEarned"))
				{
					psMinigameMetaData.totalCoinsEarned = Convert.ToInt32(_dict["totalCoinsEarned"]);
				}
				if (_dict.ContainsKey("publishReward"))
				{
					psMinigameMetaData.publishReward = (bool)_dict["publishReward"];
				}
				if (_dict.ContainsKey("playReward"))
				{
					psMinigameMetaData.playReward = (bool)_dict["playReward"];
				}
			}
			if (_dict.ContainsKey("t"))
			{
				psMinigameMetaData.timeScore = Convert.ToInt32(_dict["t"]);
			}
			if (_dict.ContainsKey("s"))
			{
				psMinigameMetaData.score = Convert.ToInt32(_dict["s"]);
			}
			if (_dict.ContainsKey("bestTime"))
			{
				psMinigameMetaData.bestTime = Convert.ToInt32(_dict["bestTime"]);
			}
			if (_dict.ContainsKey("creatorUpgrades") && _dict["creatorUpgrades"] != null)
			{
				psMinigameMetaData.creatorUpgrades = new Hashtable(_dict["creatorUpgrades"] as Dictionary<string, object>);
			}
			if (_dict.ContainsKey("state"))
			{
				string text = (string)_dict["state"];
				if (text.ToLower() == "hidden")
				{
					psMinigameMetaData.hidden = true;
				}
			}
			return psMinigameMetaData;
		}
		return null;
	}

	public static PsGameDifficulty ParseDifficulty(string _difficulty)
	{
		switch (_difficulty)
		{
		case "Easy":
			return PsGameDifficulty.Easy;
		case "Tricky":
			return PsGameDifficulty.Tricky;
		case "Insane":
			return PsGameDifficulty.Insane;
		case "Impossible":
			return PsGameDifficulty.Impossible;
		default:
			return PsGameDifficulty.New;
		}
	}

	public static PsRating ParseRating(string _rating)
	{
		switch (_rating)
		{
		case "Positive":
			return PsRating.Positive;
		case "Neutral":
			return PsRating.Neutral;
		case "Negative":
			return PsRating.Negative;
		case "Unrated":
			return PsRating.Unrated;
		default:
			return PsRating.Unrated;
		}
	}

	public static PsTournamentMetaData ParseTournamentMetaData(Dictionary<string, object> _dict)
	{
		foreach (KeyValuePair<string, object> item in _dict)
		{
			Debug.Log(item.Key + ": " + item.Value);
		}
		PsTournamentMetaData psTournamentMetaData = new PsTournamentMetaData();
		if (_dict.ContainsKey("id"))
		{
			psTournamentMetaData.tournamentId = (string)_dict["id"];
		}
		if (_dict.ContainsKey("timeLeft"))
		{
			psTournamentMetaData.timeLeft = Convert.ToInt32(_dict["timeLeft"]);
		}
		if (_dict.ContainsKey("duration"))
		{
			psTournamentMetaData.duration = Convert.ToInt32(_dict["duration"]);
		}
		if (_dict.ContainsKey("rewardCoins"))
		{
			psTournamentMetaData.playerRewardCoins = Convert.ToInt32(_dict["rewardCoins"]);
		}
		if (_dict.ContainsKey("rewardDiamonds"))
		{
			psTournamentMetaData.playerRewardDiamonds = Convert.ToInt32(_dict["rewardDiamonds"]);
		}
		if (_dict.ContainsKey("participationCoinsCost"))
		{
			psTournamentMetaData.participationCoinsCost = Convert.ToInt32(_dict["participationCoinsCost"]);
		}
		if (_dict.ContainsKey("participationDiamondsCost"))
		{
			psTournamentMetaData.participationDiamondsCost = Convert.ToInt32(_dict["participationDiamondsCost"]);
		}
		if (_dict.ContainsKey("participated"))
		{
			psTournamentMetaData.participated = (bool)_dict["participated"];
		}
		if (_dict.ContainsKey("participantCount"))
		{
			psTournamentMetaData.participantCount = Convert.ToInt32(_dict["participantCount"]);
		}
		if (_dict.ContainsKey("state"))
		{
			psTournamentMetaData.claimed = (string)_dict["state"] == "claimed";
		}
		if (_dict.ContainsKey("daily"))
		{
			psTournamentMetaData.daily = (bool)_dict["daily"];
		}
		if (_dict.ContainsKey("name"))
		{
			psTournamentMetaData.name = (string)_dict["name"];
		}
		if (_dict.ContainsKey("time"))
		{
			psTournamentMetaData.time = Convert.ToInt32(_dict["time"]);
		}
		if (_dict.ContainsKey("gameId"))
		{
			psTournamentMetaData.gameId = (string)_dict["gameId"];
		}
		if (_dict.ContainsKey("gameName"))
		{
			psTournamentMetaData.gameName = (string)_dict["gameName"];
		}
		if (_dict.ContainsKey("difficulty"))
		{
			psTournamentMetaData.difficulty = ParseDifficulty((string)_dict["difficulty"]);
		}
		if (_dict.ContainsKey("creatorName"))
		{
			psTournamentMetaData.creatorName = (string)_dict["creatorName"];
		}
		if (_dict.ContainsKey("creatorId"))
		{
			psTournamentMetaData.creatorId = (string)_dict["creatorId"];
		}
		if (_dict.ContainsKey("playerUnit"))
		{
			psTournamentMetaData.playerUnit = (string)_dict["playerUnit"];
		}
		if (_dict.ContainsKey("rating"))
		{
			psTournamentMetaData.rating = ParseRating((string)_dict["rating"]);
		}
		if (_dict.ContainsKey("facebookId"))
		{
			psTournamentMetaData.creatorFacebookId = (string)_dict["facebookId"];
		}
		if (_dict.ContainsKey("gameCenterId"))
		{
			psTournamentMetaData.creatorGameCenterId = (string)_dict["gameCenterId"];
		}
		if (_dict.ContainsKey("gameDescription"))
		{
			psTournamentMetaData.gameDescription = (string)_dict["gameDescription"];
		}
		if (_dict.ContainsKey("percentileField"))
		{
			psTournamentMetaData.percentiles = ParsePercentileList(_dict["percentileField"] as List<object>);
		}
		if (_dict.ContainsKey("topRewards"))
		{
			List<object> list = _dict["topRewards"] as List<object>;
			psTournamentMetaData.topThreeRewards = new int[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				psTournamentMetaData.topThreeRewards[i] = Convert.ToInt32(list[i]);
			}
		}
		if (_dict.ContainsKey("topScores"))
		{
			List<object> list2 = _dict["topScores"] as List<object>;
			psTournamentMetaData.topScores = new HighscoreDataEntry[list2.Count];
			for (int j = 0; j < list2.Count; j++)
			{
				Dictionary<string, object> dict = list2[j] as Dictionary<string, object>;
				psTournamentMetaData.topScores[j] = HighScores.ParseDataEntry(dict);
			}
		}
		if (_dict.ContainsKey("totalPot"))
		{
			psTournamentMetaData.totalPot = Convert.ToInt32(_dict["totalPot"]);
		}
		psTournamentMetaData.timeLeftSetTime = Main.m_EPOCHSeconds;
		return psTournamentMetaData;
	}

	public static PercentileField[] ParsePercentileList(List<object> percentileList)
	{
		PercentileField[] array = new PercentileField[percentileList.Count];
		for (int i = 0; i < percentileList.Count; i++)
		{
			Dictionary<string, object> dict = percentileList[i] as Dictionary<string, object>;
			array[i] = ParsePercentileField(dict);
			array[i].index = i + 1;
		}
		return array;
	}

	public static PercentileField ParsePercentileField(Dictionary<string, object> _dict)
	{
		PercentileField percentileField = new PercentileField();
		if (_dict.ContainsKey("time"))
		{
			percentileField.percentile = Convert.ToInt32(_dict["time"]);
		}
		if (_dict.ContainsKey("reward"))
		{
			percentileField.reward = Convert.ToInt32(_dict["reward"]);
		}
		return percentileField;
	}

	public static PsChallengeMetaData ParseChallengeMetaData(Dictionary<string, object> _dict)
	{
		PsChallengeMetaData psChallengeMetaData = new PsChallengeMetaData();
		foreach (KeyValuePair<string, object> item in _dict)
		{
			Debug.Log(item.Key);
		}
		if (_dict != null)
		{
			if (_dict.ContainsKey("meta"))
			{
				psChallengeMetaData.minigameMetaData = ParseMinigameMetaData((Dictionary<string, object>)_dict["meta"]);
			}
			if (_dict.ContainsKey("timeLeft"))
			{
				psChallengeMetaData.timeLeft = Convert.ToInt32(_dict["timeLeft"]);
			}
			if (_dict.ContainsKey("duration"))
			{
				psChallengeMetaData.duration = Convert.ToInt32(_dict["duration"]);
			}
			if (_dict.ContainsKey("timedEventId"))
			{
				psChallengeMetaData.timedEventId = (string)_dict["timedEventId"];
			}
			psChallengeMetaData.timeLeftSetTime = Main.m_EPOCHSeconds;
		}
		return psChallengeMetaData;
	}

	public static VersusMetaData ParseVersusMetaData(HttpC _c)
	{
		Dictionary<string, object> dict = ParseServerResponse(_c.www.text);
		return ParseVersusMetaData(dict);
	}

	public static VersusMetaData ParseVersusMetaData(Dictionary<string, object> _dict)
	{
		VersusMetaData versusMetaData = new VersusMetaData();
		versusMetaData.timedEventId = (string)_dict["timedEventId"];
		versusMetaData.gameId = (string)_dict["gameId"];
		if (_dict.ContainsKey("meta"))
		{
			versusMetaData.gameMetaData = ParseMinigameMetaData((Dictionary<string, object>)_dict["meta"]);
		}
		if (_dict.ContainsKey("participants"))
		{
			versusMetaData.participants = ParseVersusParticipants((List<object>)_dict["participants"]);
		}
		if (_dict.ContainsKey("timeLeft"))
		{
			versusMetaData.timeLeft = Convert.ToInt32(_dict["timeLeft"]);
		}
		versusMetaData.timeLeftSetTime = Main.m_EPOCHSeconds;
		if (_dict.ContainsKey("currentPlayer"))
		{
			versusMetaData.currentPlayer = (string)_dict["currentPlayer"];
		}
		if (_dict.ContainsKey("winner"))
		{
			versusMetaData.winner = (string)_dict["winner"];
		}
		if (_dict.ContainsKey("round"))
		{
			versusMetaData.round = Convert.ToInt32(_dict["round"]);
		}
		if (_dict.ContainsKey("matchBall"))
		{
			versusMetaData.matchBall = (bool)_dict["matchBall"];
		}
		if (_dict.ContainsKey("challengedPlayer"))
		{
			versusMetaData.challengedPlayer = (string)_dict["challengedPlayer"];
		}
		if (_dict.ContainsKey("status"))
		{
			versusMetaData.status = (string)_dict["status"];
		}
		return versusMetaData;
	}

	public static VersusParticipant[] ParseVersusParticipants(List<object> _participants)
	{
		VersusParticipant[] array = new VersusParticipant[_participants.Count];
		for (int i = 0; i < _participants.Count; i++)
		{
			Dictionary<string, object> dict = _participants[i] as Dictionary<string, object>;
			array[i] = ParseVersusParticipant(dict);
		}
		return array;
	}

	public static VersusParticipant ParseVersusParticipant(Dictionary<string, object> _dict)
	{
		VersusParticipant versusParticipant = new VersusParticipant();
		versusParticipant.score = HighScores.ParseDataEntry(_dict["score"] as Dictionary<string, object>);
		versusParticipant.rank = Convert.ToInt32(_dict["rank"]);
		versusParticipant.stars = Convert.ToInt32(_dict["stars"]);
		versusParticipant.tries = Convert.ToInt32(_dict["tries"]);
		return versusParticipant;
	}

	public static PsMinigameMetaData[] ParseTemplateList(HttpC _c)
	{
		Dictionary<string, object> dictionary = ParseServerResponse(_c.www.text);
		if (dictionary.ContainsKey("data"))
		{
			List<object> list = dictionary["data"] as List<object>;
			PsMinigameMetaData[] array = new PsMinigameMetaData[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				Dictionary<string, object> dict = list[i] as Dictionary<string, object>;
				array[i] = ParseTemplate(dict);
			}
			return array;
		}
		return null;
	}

	public static PsMinigameMetaData[] ParseMinigameList(HttpC _c)
	{
		Dictionary<string, object> dictionary = ParseServerResponse(_c.www.text);
		if (dictionary.ContainsKey("data"))
		{
			List<object> list = dictionary["data"] as List<object>;
			PsMinigameMetaData[] array = new PsMinigameMetaData[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				Dictionary<string, object> dict = list[i] as Dictionary<string, object>;
				array[i] = ParseMinigameMetaData(dict);
			}
			return array;
		}
		return null;
	}

	public static PsTimedEvents ParseTimedEvents(HttpC _c)
	{
		Dictionary<string, object> dict = ParseServerResponse(_c.www.text);
		return ParseTimedEvents(dict);
	}

	public static PsTimedEvents ParseTimedEvents(Dictionary<string, object> _dict)
	{
		PsTimedEvents psTimedEvents = new PsTimedEvents();
		if (_dict.ContainsKey("dailyChallenges"))
		{
			List<object> dict = _dict["dailyChallenges"] as List<object>;
			psTimedEvents.diamondChallenges = ParseChallengeList(dict);
		}
		if (_dict.ContainsKey("versusChallenges"))
		{
			List<object> dict2 = _dict["versusChallenges"] as List<object>;
			psTimedEvents.versusChallenges = ParseVersusList(dict2);
		}
		if (_dict.ContainsKey("freshAndFree"))
		{
			Dictionary<string, object> dict3 = _dict["freshAndFree"] as Dictionary<string, object>;
			psTimedEvents.freshAndFree.Clear();
			psTimedEvents.freshAndFree.Add(ParseMinigameMetaData(dict3));
		}
		if (_dict.ContainsKey("friendlyChallenges"))
		{
			List<object> dict4 = _dict["friendlyChallenges"] as List<object>;
			psTimedEvents.friendlyChallenges = ParseVersusList(dict4);
		}
		psTimedEvents.tournaments = ParseTournaments(_dict);
		return psTimedEvents;
	}

	public static PsTournaments ParseTournaments(HttpC _c)
	{
		Dictionary<string, object> dictionary = ParseServerResponse(_c.www.text);
		return ParseTournaments(dictionary);
	}

	public static PsTournaments ParseTournaments(Dictionary<string, object> _dictionary)
	{
		PsTournaments psTournaments = new PsTournaments();
		if (_dictionary.ContainsKey("daily"))
		{
			List<object> dict = _dictionary["daily"] as List<object>;
			psTournaments.dailyTournaments = ParseTournamentList(dict);
		}
		if (_dictionary.ContainsKey("active"))
		{
			List<object> dict2 = _dictionary["active"] as List<object>;
			psTournaments.activeTournaments = ParseTournamentList(dict2);
			for (int i = 0; i < psTournaments.activeTournaments.Count; i++)
			{
				if (psTournaments.activeTournaments[i].timeLeft <= 0)
				{
					psTournaments.activeTournaments[i].waitingForRewards = true;
				}
			}
		}
		if (_dictionary.ContainsKey("claimable"))
		{
			List<object> dict3 = _dictionary["claimable"] as List<object>;
			psTournaments.claimableTournaments = ParseTournamentList(dict3);
		}
		if (_dictionary.ContainsKey("claimed"))
		{
			List<object> dict4 = _dictionary["claimed"] as List<object>;
			psTournaments.claimedTournaments = ParseTournamentList(dict4);
		}
		return psTournaments;
	}

	public static PsTournamentMetaData ParseTournament(HttpC _c)
	{
		Dictionary<string, object> dict = ParseServerResponse(_c.www.text);
		return ParseTournamentMetaData(dict);
	}

	public static List<PsTournamentMetaData> ParseTournamentList(HttpC _c)
	{
		return ParseTournamentList(ParseServerResponse(_c.www.text)["data"] as List<object>);
	}

	public static List<PsTournamentMetaData> ParseTournamentList(List<object> _dict)
	{
		List<PsTournamentMetaData> list = new List<PsTournamentMetaData>();
		for (int i = 0; i < _dict.Count; i++)
		{
			Dictionary<string, object> dict = _dict[i] as Dictionary<string, object>;
			list.Add(ParseTournamentMetaData(dict));
		}
		return list;
	}

	public static List<PsChallengeMetaData> ParseChallengeList(HttpC _c)
	{
		return ParseChallengeList(ParseServerResponse(_c.www.text)["data"] as List<object>);
	}

	public static List<PsChallengeMetaData> ParseChallengeList(List<object> _dict)
	{
		List<PsChallengeMetaData> list = new List<PsChallengeMetaData>();
		for (int i = 0; i < _dict.Count; i++)
		{
			Dictionary<string, object> dict = _dict[i] as Dictionary<string, object>;
			list.Add(ParseChallengeMetaData(dict));
		}
		return list;
	}

	public static List<VersusMetaData> ParseVersusList(HttpC _c)
	{
		return ParseVersusList(ParseServerResponse(_c.www.text)["data"] as List<object>);
	}

	public static List<VersusMetaData> ParseVersusList(List<object> _dict)
	{
		List<VersusMetaData> list = new List<VersusMetaData>();
		for (int i = 0; i < _dict.Count; i++)
		{
			Dictionary<string, object> dict = _dict[i] as Dictionary<string, object>;
			list.Add(ParseVersusMetaData(dict));
		}
		return list;
	}

	public static string[] ParseFBFriendIds(string _FBFriendString)
	{
		Debug.Log(_FBFriendString);
		Dictionary<string, object> dictionary = FacebookManager.ParseFBResponse(_FBFriendString);
		List<string> list = new List<string>();
		if (dictionary.ContainsKey("data"))
		{
			List<object> list2 = dictionary["data"] as List<object>;
			for (int i = 0; i < list2.Count; i++)
			{
				Dictionary<string, object> dictionary2 = list2[i] as Dictionary<string, object>;
				if (dictionary2.ContainsKey("installed"))
				{
					list.Add((string)dictionary2["id"]);
				}
			}
		}
		else
		{
			Debug.LogError("Wrong data from server when parsing FB friends!");
		}
		return list.ToArray();
	}

	public static Friends ParseFriends(HttpC _c)
	{
		Dictionary<string, object> dictionary = ParseServerResponse(_c.www.text);
		Friends friends = new Friends();
		friends.followees = new List<PlayerData>(ParseFriendList(dictionary["followees"] as List<object>));
		friends.followers = new List<PlayerData>(ParseFriendList(dictionary["followers"] as List<object>));
		friends.friends = friends.followees.Intersect(friends.followers).ToList();
		if (friends.friends != null && friends.friends.Count > 0)
		{
			friends.followees = friends.followees.Except(friends.friends).ToList();
			friends.followers = friends.followers.Except(friends.friends).ToList();
		}
		Debug.Log("Followees: " + friends.followees.Count + " Followers: " + friends.followers.Count + " Friends: " + ((friends.friends != null) ? friends.friends.Count : 0));
		return friends;
	}

	private static PlayerData[] ParseFriendList(List<object> _friendJson)
	{
		PlayerData[] array = new PlayerData[_friendJson.Count];
		for (int i = 0; i < _friendJson.Count; i++)
		{
			array[i] = ParseFollowPlayerData(_friendJson[i] as Dictionary<string, object>);
		}
		return array;
	}

	public static PlayerData[] ParseFollowPlayers(HttpC _c)
	{
		Dictionary<string, object> dictionary = ParseServerResponse(_c.www.text);
		return ParseFollowPlayers(dictionary);
	}

	public static PlayerData[] ParseFollowPlayers(Dictionary<string, object> _dictionary)
	{
		List<object> friendJson = _dictionary["data"] as List<object>;
		return ParseFriendList(friendJson);
	}

	public static PlayerData ParsePlayerData(Dictionary<string, object> _dictionary)
	{
		PlayerData result = new PlayerData
		{
			playerId = (string)_dictionary["id"],
			name = (string)_dictionary["name"]
		};
		if (_dictionary.ContainsKey("coins"))
		{
			result.coins = Convert.ToInt32(_dictionary["coins"]);
		}
		if (_dictionary.ContainsKey("diamonds"))
		{
			result.diamonds = Convert.ToInt32(_dictionary["diamonds"]);
		}
		if (_dictionary.ContainsKey("stars"))
		{
			result.stars = Convert.ToInt32(_dictionary["stars"]);
		}
		if (_dictionary.ContainsKey("bolts"))
		{
			result.bolts = Convert.ToInt32(_dictionary["bolts"]);
		}
		if (_dictionary.ContainsKey("keys"))
		{
			result.keys = Convert.ToInt32(_dictionary["keys"]);
		}
		if (_dictionary.ContainsKey("level"))
		{
			result.level = Convert.ToInt32(_dictionary["level"]);
		}
		if (_dictionary.ContainsKey("maxKeys"))
		{
			result.maxKeys = Convert.ToInt32(_dictionary["maxKeys"]);
		}
		if (_dictionary.ContainsKey("itemLevel"))
		{
			result.itemLevel = Convert.ToInt32(_dictionary["itemLevel"]);
		}
		if (_dictionary.ContainsKey("cups"))
		{
			result.cups = Convert.ToInt32(_dictionary["cups"]);
		}
		if (_dictionary.ContainsKey("rank"))
		{
			result.rank = Convert.ToInt32(_dictionary["rank"]);
		}
		if (_dictionary.ContainsKey("upgrades"))
		{
			result.upgrades = ParsePlayerUpgrades(_dictionary["upgrades"] as Dictionary<string, object>);
		}
		if (_dictionary.ContainsKey("boosters"))
		{
			PsMetagameManager.m_playerStats.boosters = new Hashtable(_dictionary["boosters"] as Dictionary<string, object>);
		}
		if (_dictionary.ContainsKey("acceptNotifications"))
		{
			result.acceptNotifications = (bool)_dictionary["acceptNotifications"];
		}
		if (_dictionary.ContainsKey("facebookId"))
		{
			result.facebookId = (string)_dictionary["facebookId"];
		}
		if (_dictionary.ContainsKey("gameCenterId"))
		{
			result.gameCenterId = (string)_dictionary["gameCenterId"];
		}
		if (_dictionary.ContainsKey("ninjaCreationTimestamp"))
		{
			result.ninjaCreationTimestamp = (string)_dictionary["ninjaCreationTimestamp"];
		}
		if (_dictionary.ContainsKey("countryCode"))
		{
			result.countryCode = (string)_dictionary["countryCode"];
		}
		if (_dictionary.ContainsKey("itemDbVersion"))
		{
			result.itemDbVersion = Convert.ToInt32(_dictionary["itemDbVersion"]);
		}
		if (_dictionary.ContainsKey("data"))
		{
			Dictionary<string, object> dictionary = _dictionary["data"] as Dictionary<string, object>;
			if (dictionary != null)
			{
				PsMetagamePlayerData.m_playerData = new Hashtable(dictionary);
			}
		}
		if (_dictionary.ContainsKey("clientConfig"))
		{
			result.clientConfig = new ClientConfig(_dictionary["clientConfig"] as Dictionary<string, object>);
		}
		if (_dictionary.ContainsKey("playerConfig"))
		{
			result.clientConfig = ParsePlayerConfig(_dictionary["playerConfig"] as Dictionary<string, object>, result.clientConfig);
		}
		if (_dictionary.ContainsKey("keyRefreshTimeLeft"))
		{
			result.keyTimeLeft = (int)(long)_dictionary["keyRefreshTimeLeft"];
		}
		result.claimedTutorials = new List<string>();
		if (_dictionary.ContainsKey("claimedTutorials"))
		{
			object[] array = (_dictionary["claimedTutorials"] as List<object>).ToArray();
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					result.claimedTutorials.Add((string)array[i]);
				}
			}
		}
		if (_dictionary.ContainsKey("OffroadCarVisual"))
		{
			result.offroadCarVisual = ParseList(_dictionary["OffroadCarVisual"] as List<object>);
		}
		if (_dictionary.ContainsKey("OffroadCarUpgrade"))
		{
			result.offroadCarUpgrade = ParseList(_dictionary["OffroadCarUpgrade"] as List<object>);
		}
		if (_dictionary.ContainsKey("MotorcycleVisual"))
		{
			result.motorcycleVisual = ParseList(_dictionary["MotorcycleVisual"] as List<object>);
		}
		if (_dictionary.ContainsKey("MotorcycleUpgrade"))
		{
			result.motorcycleUpgrade = ParseList(_dictionary["MotorcycleUpgrade"] as List<object>);
		}
		if (_dictionary.ContainsKey("hash"))
		{
			result.hash = (string)_dictionary["hash"];
		}
		return result;
	}

	private static List<string> ParseList(List<object> _list)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < _list.Count(); i++)
		{
			list.Add((string)_list[i]);
		}
		return list;
	}

	public static Hashtable ParsePlayerUpgrades(Dictionary<string, object> _dictionary)
	{
		Hashtable hashtable = new Hashtable();
		foreach (string key in _dictionary.Keys)
		{
			Dictionary<string, object> dictionary = _dictionary[key] as Dictionary<string, object>;
			if (dictionary != null)
			{
				hashtable.Add(key, new Hashtable(dictionary));
			}
		}
		return hashtable;
	}

	public static PsPlayerProfilerData ParsePlayerProfilerData(HttpC _c)
	{
		Dictionary<string, object> dictionary = ParseServerResponse(_c.www.text);
		return ParsePlayerProfilerData(dictionary);
	}

	public static PsPlayerProfilerData ParsePlayerProfilerData(Dictionary<string, object> _dictionary)
	{
		PsPlayerProfilerData psPlayerProfilerData = new PsPlayerProfilerData();
		if (_dictionary.ContainsKey("itemLevel"))
		{
			psPlayerProfilerData.m_itemLevel = Convert.ToInt32(_dictionary["itemLevel"]);
		}
		if (_dictionary.ContainsKey("skill"))
		{
			psPlayerProfilerData.m_skills.Add(new PsPlayerProfilerData.ProfilerVehicleData("OffroadCar", Convert.ToInt32(_dictionary["skill"]), Convert.ToInt32(_dictionary["offroadCarPreference"])));
		}
		if (_dictionary.ContainsKey("motorcycleSkill"))
		{
			psPlayerProfilerData.m_skills.Add(new PsPlayerProfilerData.ProfilerVehicleData("Motorcycle", Convert.ToInt32(_dictionary["motorcycleSkill"]), Convert.ToInt32(_dictionary["motorcyclePreference"])));
		}
		if (_dictionary.ContainsKey("searchPrefs"))
		{
			List<object> list = _dictionary["searchPrefs"] as List<object>;
			if (list != null)
			{
				foreach (object item in list)
				{
					Dictionary<string, object> dictionary = item as Dictionary<string, object>;
					if (dictionary == null)
					{
						continue;
					}
					foreach (KeyValuePair<string, object> item2 in dictionary)
					{
						string key = item2.Key;
						double num = Convert.ToDouble(item2.Value);
						psPlayerProfilerData.m_subgenrePrefs.Add(key, (float)num);
					}
				}
			}
		}
		return psPlayerProfilerData;
	}

	public static PlayerData ParseFollowPlayerData(Dictionary<string, object> _dictionary)
	{
		PlayerData result = new PlayerData
		{
			playerId = (string)_dictionary["id"],
			name = (string)_dictionary["name"]
		};
		if (_dictionary.ContainsKey("acceptNotifications"))
		{
			result.acceptNotifications = (bool)_dictionary["acceptNotifications"];
		}
		if (_dictionary.ContainsKey("facebookId"))
		{
			result.facebookId = (string)_dictionary["facebookId"];
		}
		if (_dictionary.ContainsKey("gameCenterId"))
		{
			result.gameCenterId = (string)_dictionary["gameCenterId"];
		}
		if (_dictionary.ContainsKey("ninjaCreationTimestamp"))
		{
			result.ninjaCreationTimestamp = (string)_dictionary["ninjaCreationTimestamp"];
		}
		if (_dictionary.ContainsKey("countryCode"))
		{
			result.countryCode = (string)_dictionary["countryCode"];
		}
		if (_dictionary.ContainsKey("itemDbVersion"))
		{
			result.itemDbVersion = Convert.ToInt32(_dictionary["itemDbVersion"]);
		}
		return result;
	}

	public static Hashtable CreateMetagameNodeDataJSON(MetagameNodeData _data)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("nodeType", _data.m_nodeDataType.ToString());
		if (_data.m_nodeDataType == MetagameNodeDataType.Unlock)
		{
			hashtable.Add("itemName", _data.m_unlockableName);
			hashtable.Add("itemDescription", _data.m_unlockableDescription);
			hashtable.Add("itemIcon", _data.m_unlockableIcon);
			hashtable.Add("itemClass", _data.m_dataIdentifier);
		}
		if (_data.m_nodeDataType == MetagameNodeDataType.UnlockableCategory)
		{
			hashtable.Add("itemType", _data.m_unlockableType);
			hashtable.Add("itemName", _data.m_unlockableName);
			hashtable.Add("itemDescription", _data.m_unlockableDescription);
			hashtable.Add("itemIcon", _data.m_unlockableIcon);
		}
		if (_data.m_nodeDataType == MetagameNodeDataType.Unlockable)
		{
			hashtable.Add("itemName", _data.m_unlockableName);
			hashtable.Add("itemDescription", _data.m_unlockableDescription);
			hashtable.Add("itemIcon", _data.m_unlockableIcon);
			hashtable.Add("itemNodeType", _data.m_unlockableGraphNodeClass);
			hashtable.Add("itemClass", _data.m_dataIdentifier);
			hashtable.Add("itemMaxCount", _data.m_unlockableMaxCount);
			hashtable.Add("itemComplexity", _data.m_unlockableComplexity);
			hashtable.Add("itemLevel", _data.m_unlockableItemLevel);
		}
		if (_data.m_nodeDataType == MetagameNodeDataType.UnlockableSimple)
		{
			hashtable.Add("itemName", _data.m_unlockableName);
			hashtable.Add("itemDescription", _data.m_unlockableDescription);
			hashtable.Add("itemIcon", _data.m_unlockableIcon);
			hashtable.Add("itemClass", _data.m_dataIdentifier);
			hashtable.Add("itemLevel", _data.m_unlockableItemLevel);
		}
		if (_data.m_nodeDataType == MetagameNodeDataType.UnlockableUpgradeable)
		{
			hashtable.Add("itemName", _data.m_unlockableName);
			hashtable.Add("itemDescription", _data.m_unlockableDescription);
			hashtable.Add("itemIcon", _data.m_unlockableIcon);
			hashtable.Add("itemNodeType", _data.m_unlockableGraphNodeClass);
			hashtable.Add("itemClass", _data.m_dataIdentifier);
			hashtable.Add("itemMaxCount", _data.m_unlockableMaxCount);
			hashtable.Add("itemComplexity", _data.m_unlockableComplexity);
			hashtable.Add("itemLevel", _data.m_unlockableItemLevel);
			hashtable.Add("vehicleUpgradeValues", _data.m_unlockableUpgradeValues);
			hashtable.Add("vehicleUpgradeSteps", _data.m_unlockableUpgradeSteps);
			hashtable.Add("vehicleUpgradePrices", _data.m_unlockableUpgradePrices);
			hashtable.Add("rentName", _data.m_unlockableRentName);
			hashtable.Add("rentButton", _data.m_unlockableRentButton);
		}
		if (_data.m_nodeDataType == MetagameNodeDataType.Level)
		{
			if (_data.m_levelMinigameId != string.Empty)
			{
				hashtable.Add("levelMinigameId", _data.m_levelMinigameId);
			}
			hashtable.Add("levelDifficulty", _data.m_levelDifficulty);
			hashtable.Add("levelPlayerUnit", _data.m_levelPlayerUnit);
			hashtable.Add("levelSubgenre", _data.m_levelSubgenre.ToString());
			hashtable.Add("levelGameMode", _data.m_levelGameMode);
			hashtable.Add("levelItems", _data.m_levelItems.ToArray());
			if (_data.m_levelMedalTimes != null && _data.m_levelMedalTimes.Length > 0 && !string.IsNullOrEmpty(_data.m_levelMedalTimes[0]))
			{
				hashtable.Add("levelMedalTimes", _data.m_levelMedalTimes);
			}
		}
		if (_data.m_nodeDataType == MetagameNodeDataType.EditorPuzzle)
		{
			if (_data.m_levelMinigameId != string.Empty)
			{
				hashtable.Add("levelMinigameId", _data.m_levelMinigameId);
			}
			hashtable.Add("puzzleLimitedItems", _data.m_levelItems.ToArray());
		}
		if (_data.m_nodeDataType == MetagameNodeDataType.Signal)
		{
			hashtable.Add("levelMinigameId", _data.m_levelMinigameId);
			hashtable.Add("blockUnlocked", _data.m_blockUnlocked);
			hashtable.Add("blockWillUnlock", _data.m_blockWillUnlock);
		}
		if (_data.m_nodeDataType == MetagameNodeDataType.VersusRace)
		{
			hashtable.Add("levelMinigameId", _data.m_levelMinigameId);
		}
		if (_data.m_nodeDataType == MetagameNodeDataType.Block)
		{
			hashtable.Add("levelMinigameId", _data.m_levelMinigameId);
			hashtable.Add("blockRequiredStars", _data.m_blockRequiredStars);
			hashtable.Add("blockType", _data.m_blockType);
			hashtable.Add("blockBolts", _data.m_blockBolts);
			hashtable.Add("blockCoins", _data.m_blockCoins);
			hashtable.Add("blockDiamonds", _data.m_blockDiamonds);
			hashtable.Add("blockKeys", _data.m_blockKeys);
			hashtable.Add("blockUnlocked", _data.m_blockUnlocked);
			hashtable.Add("blockWillUnlock", _data.m_blockWillUnlock);
			hashtable.Add("blockRewardCue", _data.m_dataIdentifier);
		}
		if (_data.m_nodeDataType == MetagameNodeDataType.Dialogue)
		{
			hashtable.Add("dialogueIdentifier", _data.m_dataIdentifier);
			hashtable.Add("dialogueTrigger", _data.m_dialogueTrigger.ToString());
			hashtable.Add("dialogueCharacter", _data.m_dialogueCharacter.ToString());
			hashtable.Add("dialogueCharacterPosition", _data.m_dialogueCharacterPosition.ToString());
			hashtable.Add("dialogueText", _data.m_dialogueText);
			hashtable.Add("dialogueProceed", _data.m_dialogueProceed);
			hashtable.Add("dialogueCancel", _data.m_dialogueCancel);
		}
		if (_data.m_nodeDataType == MetagameNodeDataType.LevelTemplate)
		{
			hashtable.Add("levelTemplateIdentifier", _data.m_dataIdentifier);
			hashtable.Add("levelMinigameId", _data.m_levelMinigameId);
			hashtable.Add("levelPlayerUnit", _data.m_levelPlayerUnit);
			hashtable.Add("levelGameMode", _data.m_levelGameMode.ToString());
			hashtable.Add("levelTemplateDomeSize", _data.m_levelTemplateDomeSize.ToString());
			hashtable.Add("levelTemplateItems", _data.m_levelItems.ToArray());
			hashtable.Add("levelTemplateMinigames", _data.m_levelTemplateMinigames.ToArray());
		}
		return hashtable;
	}

	public static MetagameNodeData ParseMetagameNodeDataJSON(Dictionary<string, object> _dataDict)
	{
		MetagameNodeData metagameNodeData = new MetagameNodeData(string.Empty);
		try
		{
			metagameNodeData.m_nodeDataType = (MetagameNodeDataType)(int)Enum.Parse(typeof(MetagameNodeDataType), (string)_dataDict["nodeType"], true);
		}
		catch
		{
			switch ((string)_dataDict["nodeType"])
			{
			case "ItemCategory":
				metagameNodeData.m_nodeDataType = MetagameNodeDataType.UnlockableCategory;
				break;
			case "Item":
				metagameNodeData.m_nodeDataType = MetagameNodeDataType.Unlockable;
				break;
			case "ItemPlayer":
				metagameNodeData.m_nodeDataType = MetagameNodeDataType.UnlockableUpgradeable;
				break;
			default:
				metagameNodeData.m_nodeDataType = MetagameNodeDataType.Undefined;
				break;
			}
		}
		if (metagameNodeData.m_nodeDataType == MetagameNodeDataType.Unlock)
		{
			metagameNodeData.m_unlockableName = (string)_dataDict["itemName"];
			metagameNodeData.m_unlockableIcon = (string)_dataDict["itemIcon"];
			metagameNodeData.m_unlockableDescription = (string)_dataDict["itemDescription"];
			try
			{
				metagameNodeData.m_dataIdentifier = (string)_dataDict["itemClass"];
			}
			catch
			{
				metagameNodeData.m_dataIdentifier = string.Empty;
			}
		}
		if (metagameNodeData.m_nodeDataType == MetagameNodeDataType.UnlockableCategory)
		{
			metagameNodeData.m_unlockableType = (PsUnlockableType)(int)Enum.Parse(typeof(PsUnlockableType), (string)_dataDict["itemType"]);
			metagameNodeData.m_unlockableName = (string)_dataDict["itemName"];
			metagameNodeData.m_unlockableDescription = (string)_dataDict["itemDescription"];
			metagameNodeData.m_unlockableIcon = (string)_dataDict["itemIcon"];
		}
		if (metagameNodeData.m_nodeDataType == MetagameNodeDataType.Unlockable)
		{
			metagameNodeData.m_unlockableName = (string)_dataDict["itemName"];
			metagameNodeData.m_unlockableDescription = (string)_dataDict["itemDescription"];
			metagameNodeData.m_unlockableIcon = (string)_dataDict["itemIcon"];
			try
			{
				metagameNodeData.m_unlockableGraphNodeClass = (string)_dataDict["itemNodeType"];
			}
			catch
			{
				metagameNodeData.m_unlockableGraphNodeClass = typeof(GraphNode).ToString();
			}
			try
			{
				metagameNodeData.m_dataIdentifier = (string)_dataDict["itemClass"];
			}
			catch
			{
				metagameNodeData.m_dataIdentifier = typeof(BasicAssembledClass).ToString();
			}
			metagameNodeData.m_unlockableMaxCount = Convert.ToInt32(_dataDict["itemMaxCount"]);
			metagameNodeData.m_unlockableComplexity = Convert.ToInt32(_dataDict["itemComplexity"]);
			try
			{
				metagameNodeData.m_unlockableItemLevel = Convert.ToInt32(_dataDict["itemLevel"]);
			}
			catch
			{
				metagameNodeData.m_unlockableItemLevel = 0;
			}
		}
		if (metagameNodeData.m_nodeDataType == MetagameNodeDataType.UnlockableSimple)
		{
			metagameNodeData.m_unlockableName = (string)_dataDict["itemName"];
			metagameNodeData.m_unlockableDescription = (string)_dataDict["itemDescription"];
			metagameNodeData.m_unlockableIcon = (string)_dataDict["itemIcon"];
			try
			{
				metagameNodeData.m_dataIdentifier = (string)_dataDict["itemClass"];
			}
			catch
			{
				metagameNodeData.m_dataIdentifier = typeof(BasicAssembledClass).ToString();
			}
			try
			{
				metagameNodeData.m_unlockableItemLevel = Convert.ToInt32(_dataDict["itemLevel"]);
			}
			catch
			{
				metagameNodeData.m_unlockableItemLevel = 0;
			}
		}
		if (metagameNodeData.m_nodeDataType == MetagameNodeDataType.UnlockableUpgradeable)
		{
			metagameNodeData.m_unlockableName = (string)_dataDict["itemName"];
			metagameNodeData.m_unlockableDescription = (string)_dataDict["itemDescription"];
			metagameNodeData.m_unlockableIcon = (string)_dataDict["itemIcon"];
			metagameNodeData.m_unlockableGraphNodeClass = (string)_dataDict["itemNodeType"];
			metagameNodeData.m_dataIdentifier = (string)_dataDict["itemClass"];
			metagameNodeData.m_unlockableMaxCount = Convert.ToInt32(_dataDict["itemMaxCount"]);
			metagameNodeData.m_unlockableComplexity = Convert.ToInt32(_dataDict["itemComplexity"]);
			try
			{
				metagameNodeData.m_unlockableItemLevel = Convert.ToInt32(_dataDict["itemLevel"]);
			}
			catch
			{
				metagameNodeData.m_unlockableItemLevel = 0;
			}
			try
			{
				metagameNodeData.m_unlockableRentName = (string)_dataDict["rentName"];
			}
			catch
			{
				metagameNodeData.m_unlockableRentName = "Rentzal";
			}
			try
			{
				metagameNodeData.m_unlockableRentButton = (string)_dataDict["rentButton"];
			}
			catch
			{
				metagameNodeData.m_unlockableRentButton = "Rentz this";
			}
			if (_dataDict.ContainsKey("vehicleUpgradeValues"))
			{
				metagameNodeData.m_unlockableUpgradeValues = new Hashtable(_dataDict["vehicleUpgradeValues"] as Dictionary<string, object>);
				Hashtable hashtable = new Hashtable();
				foreach (string key in metagameNodeData.m_unlockableUpgradeValues.Keys)
				{
					List<object> list = metagameNodeData.m_unlockableUpgradeValues[key] as List<object>;
					if (list != null)
					{
						object[] array = list.Cast<object>().ToArray();
						if (array != null)
						{
							hashtable.Add(key, array);
						}
					}
				}
				metagameNodeData.m_unlockableUpgradeValues = hashtable;
			}
			else
			{
				Debug.Log("UPGRADE VALUES NOT FOUND");
			}
			metagameNodeData.m_unlockableUpgradeSteps = Convert.ToInt32(_dataDict["vehicleUpgradeSteps"]);
			List<object> list2 = (List<object>)_dataDict["vehicleUpgradePrices"];
			metagameNodeData.m_unlockableUpgradePrices = new string[list2.Count];
			for (int i = 0; i < list2.Count; i++)
			{
				metagameNodeData.m_unlockableUpgradePrices[i] = (string)list2[i];
			}
		}
		if (metagameNodeData.m_nodeDataType == MetagameNodeDataType.Level)
		{
			if (_dataDict.ContainsKey("levelMinigameId"))
			{
				metagameNodeData.m_levelMinigameId = (string)_dataDict["levelMinigameId"];
			}
			if (_dataDict.ContainsKey("levelDifficulty"))
			{
				metagameNodeData.m_levelDifficulty = (PsGameDifficulty)(int)Enum.Parse(typeof(PsGameDifficulty), (string)_dataDict["levelDifficulty"], true);
			}
			if (_dataDict.ContainsKey("levelPlayerUnit"))
			{
				metagameNodeData.m_levelPlayerUnit = (string)_dataDict["levelPlayerUnit"];
			}
			if (_dataDict.ContainsKey("levelSubgenre"))
			{
				metagameNodeData.m_levelSubgenre = (PsSubgenre)(int)Enum.Parse(typeof(PsSubgenre), (string)_dataDict["levelSubgenre"], true);
			}
			if (_dataDict.ContainsKey("levelGameMode"))
			{
				metagameNodeData.m_levelGameMode = (PsGameMode)(int)Enum.Parse(typeof(PsGameMode), (string)_dataDict["levelGameMode"], true);
			}
			List<object> list3 = null;
			if (_dataDict.ContainsKey("levelItems"))
			{
				list3 = (List<object>)_dataDict["levelItems"];
			}
			metagameNodeData.m_levelItems = new List<string>();
			if (list3 != null)
			{
				for (int j = 0; j < list3.Count; j++)
				{
					metagameNodeData.m_levelItems.Add((string)list3[j]);
				}
			}
			if (_dataDict.ContainsKey("levelMedalTimes"))
			{
				List<object> list4 = (List<object>)_dataDict["levelMedalTimes"];
				metagameNodeData.m_levelMedalTimes = new string[3];
				for (int k = 0; k < list4.Count; k++)
				{
					metagameNodeData.m_levelMedalTimes[k] = (string)list4[k];
				}
			}
		}
		if (metagameNodeData.m_nodeDataType == MetagameNodeDataType.EditorPuzzle)
		{
			if (_dataDict.ContainsKey("levelMinigameId"))
			{
				metagameNodeData.m_levelMinigameId = (string)_dataDict["levelMinigameId"];
			}
			metagameNodeData.m_levelItems = new List<string>();
			if (_dataDict.ContainsKey("puzzleLimitedItems"))
			{
				List<object> list5 = (List<object>)_dataDict["puzzleLimitedItems"];
				for (int l = 0; l < list5.Count; l++)
				{
					metagameNodeData.m_levelItems.Add((string)list5[l]);
				}
			}
		}
		if (metagameNodeData.m_nodeDataType == MetagameNodeDataType.Signal)
		{
			metagameNodeData.m_levelMinigameId = (string)_dataDict["levelMinigameId"];
			metagameNodeData.m_blockUnlocked = (string)_dataDict["blockUnlocked"];
			try
			{
				metagameNodeData.m_blockWillUnlock = (string)_dataDict["blockWillUnlock"];
			}
			catch
			{
				metagameNodeData.m_blockWillUnlock = string.Empty;
			}
		}
		if (metagameNodeData.m_nodeDataType == MetagameNodeDataType.VersusRace)
		{
			try
			{
				metagameNodeData.m_levelMinigameId = (string)_dataDict["levelMinigameId"];
			}
			catch
			{
				metagameNodeData.m_levelMinigameId = string.Empty;
			}
		}
		if (metagameNodeData.m_nodeDataType == MetagameNodeDataType.Block)
		{
			try
			{
				metagameNodeData.m_levelMinigameId = (string)_dataDict["levelMinigameId"];
			}
			catch
			{
				metagameNodeData.m_levelMinigameId = string.Empty;
			}
			metagameNodeData.m_blockRequiredStars = Convert.ToInt32(_dataDict["blockRequiredStars"]);
			metagameNodeData.m_blockType = (PsPathBlockType)(int)Enum.Parse(typeof(PsPathBlockType), (string)_dataDict["blockType"]);
			metagameNodeData.m_blockBolts = Convert.ToInt32(_dataDict["blockBolts"]);
			metagameNodeData.m_blockCoins = Convert.ToInt32(_dataDict["blockCoins"]);
			metagameNodeData.m_blockDiamonds = Convert.ToInt32(_dataDict["blockDiamonds"]);
			try
			{
				metagameNodeData.m_blockKeys = Convert.ToInt32(_dataDict["blockKeys"]);
			}
			catch
			{
				metagameNodeData.m_blockKeys = 0;
			}
			metagameNodeData.m_blockUnlocked = (string)_dataDict["blockUnlocked"];
			try
			{
				metagameNodeData.m_blockWillUnlock = (string)_dataDict["blockWillUnlock"];
			}
			catch
			{
				metagameNodeData.m_blockWillUnlock = string.Empty;
			}
			try
			{
				metagameNodeData.m_dataIdentifier = (string)_dataDict["blockRewardCue"];
			}
			catch
			{
				metagameNodeData.m_dataIdentifier = string.Empty;
			}
		}
		if (metagameNodeData.m_nodeDataType == MetagameNodeDataType.Dialogue)
		{
			metagameNodeData.m_dataIdentifier = (string)_dataDict["dialogueIdentifier"];
			try
			{
				metagameNodeData.m_dialogueTrigger = (PsNodeEventTrigger)(int)Enum.Parse(typeof(PsNodeEventTrigger), (string)_dataDict["dialogueTrigger"]);
			}
			catch
			{
				metagameNodeData.m_dialogueTrigger = PsNodeEventTrigger.MinigameLose;
			}
			metagameNodeData.m_dialogueCharacterPosition = (PsDialogueCharacterPosition)(int)Enum.Parse(typeof(PsDialogueCharacterPosition), (string)_dataDict["dialogueCharacterPosition"]);
			metagameNodeData.m_dialogueCharacter = (PsDialogueCharacter)(int)Enum.Parse(typeof(PsDialogueCharacter), (string)_dataDict["dialogueCharacter"]);
			metagameNodeData.m_dialogueText = (string)_dataDict["dialogueText"];
			metagameNodeData.m_dialogueProceed = (string)_dataDict["dialogueProceed"];
			metagameNodeData.m_dialogueCancel = (string)_dataDict["dialogueCancel"];
		}
		if (metagameNodeData.m_nodeDataType == MetagameNodeDataType.LevelTemplate)
		{
			metagameNodeData.m_dataIdentifier = (string)_dataDict["levelTemplateIdentifier"];
			try
			{
				metagameNodeData.m_levelMinigameId = (string)_dataDict["levelMinigameId"];
			}
			catch
			{
				metagameNodeData.m_levelMinigameId = string.Empty;
			}
			metagameNodeData.m_levelPlayerUnit = (string)_dataDict["levelPlayerUnit"];
			metagameNodeData.m_levelGameMode = (PsGameMode)(int)Enum.Parse(typeof(PsGameMode), (string)_dataDict["levelGameMode"]);
			metagameNodeData.m_levelTemplateDomeSize = (PsGameArea)(int)Enum.Parse(typeof(PsGameArea), (string)_dataDict["levelTemplateDomeSize"]);
			List<object> list6 = (List<object>)_dataDict["levelTemplateItems"];
			metagameNodeData.m_levelItems = new List<string>();
			for (int m = 0; m < list6.Count; m++)
			{
				metagameNodeData.m_levelItems.Add((string)list6[m]);
			}
			if (_dataDict.ContainsKey("levelTemplateMinigames"))
			{
				List<object> list7 = (List<object>)_dataDict["levelTemplateMinigames"];
				metagameNodeData.m_levelTemplateMinigames = new List<string>();
				for (int n = 0; n < list7.Count; n++)
				{
					metagameNodeData.m_levelTemplateMinigames.Add((string)list7[n]);
				}
			}
		}
		return metagameNodeData;
	}

	public static Dictionary<int, PsMetagameDataHelper> ParseInitialGraphNodes(Dictionary<string, object> _dataDict)
	{
		Dictionary<int, PsMetagameDataHelper> dictionary = new Dictionary<int, PsMetagameDataHelper>();
		if (_dataDict.ContainsKey("nodes"))
		{
			List<object> list = _dataDict["nodes"] as List<object>;
			for (int i = 0; i < list.Count; i++)
			{
				Dictionary<string, object> dictionary2 = list[i] as Dictionary<string, object>;
				int num = Convert.ToInt32(dictionary2["id"]);
				string name = (string)dictionary2["name"];
				float x = Convert.ToSingle(dictionary2["x"]);
				float y = Convert.ToSingle(dictionary2["y"]);
				Dictionary<string, object> dataDict = dictionary2["data"] as Dictionary<string, object>;
				MetagameNodeData metagameNodeData = ParseMetagameNodeDataJSON(dataDict);
				PsMetagameDataHelper psMetagameDataHelper = new PsMetagameDataHelper();
				psMetagameDataHelper.m_id = num;
				psMetagameDataHelper.m_name = name;
				psMetagameDataHelper.m_x = x;
				psMetagameDataHelper.m_y = y;
				psMetagameDataHelper.m_metagameNodeData = metagameNodeData;
				dictionary.Add(num, psMetagameDataHelper);
			}
		}
		return dictionary;
	}

	public static void LinkInitialGraphNodes(Dictionary<int, PsMetagameDataHelper> _nodes, Dictionary<string, object> _dataDict)
	{
		if (_dataDict.ContainsKey("edges"))
		{
			List<object> list = _dataDict["edges"] as List<object>;
			for (int i = 0; i < list.Count; i++)
			{
				Dictionary<string, object> dictionary = list[i] as Dictionary<string, object>;
				int key = Convert.ToInt32(dictionary["source"]);
				int key2 = Convert.ToInt32(dictionary["target"]);
				PsMetagameDataHelper psMetagameDataHelper = _nodes[key];
				PsMetagameDataHelper psMetagameDataHelper2 = _nodes[key2];
				psMetagameDataHelper.m_outputs.Add(psMetagameDataHelper2);
				psMetagameDataHelper2.m_inputs.Add(psMetagameDataHelper);
			}
		}
	}

	public static void ParseInitialGraphData(HttpC _c, int _pathVersion = 0)
	{
		string response = Encoding.UTF8.GetString(FilePacker.UnZipBytes(_c.www.bytes));
		Dictionary<string, object> dictionary = ParseServerResponse(response);
		ParseInitialGraphData(dictionary, (_pathVersion == 0) ? PlayerPrefsX.GetPathVersion() : _pathVersion);
		SaveLocalPathData(Json.Serialize(dictionary));
	}

	public static void ParseInitialGraphData(Dictionary<string, object> _dataDict, int _pathVersion)
	{
		PsMetagameData.m_playerUnits.Clear();
		PsMetagameData.m_units.Clear();
		PsMetagameData.m_gameModes.Clear();
		PsMetagameData.m_gameAreas.Clear();
		PsMetagameData.m_gameEnvironments.Clear();
		PsMetagameData.m_gameMaterials.Clear();
		PsMetagameData.m_menus.Clear();
		PsMetagameData.m_unlocks.Clear();
		PsMetagameData.m_levelTemplates.Clear();
		Dictionary<int, PsMetagameDataHelper> dictionary = ParseInitialGraphNodes(_dataDict);
		LinkInitialGraphNodes(dictionary, _dataDict);
		foreach (KeyValuePair<int, PsMetagameDataHelper> item in dictionary)
		{
			PsMetagameDataHelper value = item.Value;
			if (value.m_inputs.Count == 0)
			{
				if (value.m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.UnlockableCategory)
				{
					PsMetagameData.AddCategory(value);
				}
				else if (value.m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Dialogue)
				{
					PsMetagameData.AddDialogue(value);
				}
			}
			if (value.m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.LevelTemplate)
			{
				PsMetagameData.AddLevelTemplate(value);
			}
		}
		foreach (KeyValuePair<int, PsMetagameDataHelper> item2 in dictionary)
		{
			PsMetagameDataHelper value2 = item2.Value;
			if (value2.m_metagameNodeData.m_nodeDataType != MetagameNodeDataType.Unlock)
			{
				continue;
			}
			bool flag = false;
			for (int i = 0; i < value2.m_inputs.Count; i++)
			{
				if (value2.m_inputs[i].m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Unlock)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				PsMetagameData.AddUnlocks(value2);
			}
		}
		Debug.Log("UNITS: " + PsMetagameData.m_units.Count);
		Debug.Log("PLAYER UNITS: " + PsMetagameData.m_playerUnits.Count);
		Debug.Log("GAME MODES: " + PsMetagameData.m_gameModes.Count);
		Debug.Log("GAME MATERIALS: " + PsMetagameData.m_gameMaterials.Count);
		Debug.Log("GAME AREAS: " + PsMetagameData.m_gameAreas.Count);
		Debug.Log("GAME ENVIRONMENTS: " + PsMetagameData.m_gameEnvironments.Count);
		Debug.Log("MENUS: " + PsMetagameData.m_menus.Count);
		Debug.Log("UNLOCKS: " + PsMetagameData.m_unlocks.Count);
		dictionary = null;
		PlayerPrefsX.SetPathVersion(_pathVersion);
	}

	public static List<PsPlanetPath> ParseProgressionPathData(List<object> _paths, bool _local = true)
	{
		List<PsPlanetPath> list = new List<PsPlanetPath>();
		PsPlanetPath psPlanetPath = null;
		List<PsPlanetPath> list2 = new List<PsPlanetPath>();
		for (int i = 0; i < _paths.Count; i++)
		{
			Dictionary<string, object> dictionary = _paths[i] as Dictionary<string, object>;
			string text = (string)dictionary["name"];
			int currentNodeId = Convert.ToInt32(dictionary["currentNode"]);
			int lane = 0;
			if (dictionary.ContainsKey("lane"))
			{
				lane = Convert.ToInt32(dictionary["lane"]);
			}
			int startNodeId = 0;
			if (dictionary.ContainsKey("startNode"))
			{
				startNodeId = Convert.ToInt32(dictionary["startNode"]);
			}
			bool overwrite = false;
			if (dictionary.ContainsKey("overwrite"))
			{
				overwrite = (bool)dictionary["overwrite"];
			}
			PsPlanetPath psPlanetPath2 = null;
			if (text == "MainPath")
			{
				psPlanetPath = new PsPlanetPath(text);
				psPlanetPath2 = psPlanetPath;
			}
			else
			{
				psPlanetPath2 = new PsPlanetPath(text);
				list2.Add(psPlanetPath2);
			}
			psPlanetPath2.m_currentNodeId = currentNodeId;
			psPlanetPath2.m_startNodeId = startNodeId;
			psPlanetPath2.m_lane = lane;
			psPlanetPath2.m_overwrite = overwrite;
			list.Add(psPlanetPath2);
			List<object> list3 = dictionary["nodes"] as List<object>;
			for (int j = 0; j < list3.Count; j++)
			{
				Dictionary<string, object> dictionary2 = list3[j] as Dictionary<string, object>;
				int id = Convert.ToInt32(dictionary2["id"]);
				int levelNumber = Convert.ToInt32(dictionary2["levelNumber"]);
				int score = Convert.ToInt32(dictionary2["score"]);
				bool rentedVehicle = false;
				if (dictionary2.ContainsKey("rented"))
				{
					rentedVehicle = Convert.ToBoolean(dictionary2["rented"]);
				}
				string text2 = string.Empty;
				PsMinigameContext psMinigameContext = PsMinigameContext.Undefined;
				PsPathBlockType psPathBlockType = PsPathBlockType.Chest;
				int requiredStars = 0;
				int blockCoins = 0;
				int blockDiamonds = 0;
				int blockBolts = 0;
				int blockKeys = 0;
				string unlockedProgression = string.Empty;
				string willUnlockProgression = string.Empty;
				MinigameSearchParametres minigameSearchParametres = new MinigameSearchParametres();
				Hashtable hashtable = new Hashtable();
				bool unlocked = true;
				int num = 0;
				int duration = 0;
				string text3 = string.Empty;
				string text4 = string.Empty;
				bool eventOver = false;
				bool domeDestroyed = false;
				List<string> list4 = null;
				bool removedFromPath = false;
				string templateIdentifier = string.Empty;
				string[] array = null;
				string rewardCue = string.Empty;
				VersusMetaData versusData = null;
				PsGameMode gameModeFromProgressionData = PsGameMode.Any;
				if (dictionary2.ContainsKey("data"))
				{
					Dictionary<string, object> dictionary3 = dictionary2["data"] as Dictionary<string, object>;
					try
					{
						psMinigameContext = (PsMinigameContext)(int)Enum.Parse(typeof(PsMinigameContext), (string)dictionary3["context"]);
					}
					catch
					{
					}
					text2 = (string)dictionary3["minigameId"];
					try
					{
						gameModeFromProgressionData = (PsGameMode)(int)Enum.Parse(typeof(PsGameMode), (string)dictionary3["gameMode"]);
					}
					catch
					{
					}
					switch (psMinigameContext)
					{
					case PsMinigameContext.Block:
					case PsMinigameContext.Signal:
						requiredStars = Convert.ToInt32(dictionary3["requiredStars"]);
						psPathBlockType = (PsPathBlockType)(int)Enum.Parse(typeof(PsPathBlockType), (string)dictionary3["blockType"]);
						unlockedProgression = (string)dictionary3["unlockedProgression"];
						if (dictionary3.ContainsKey("willUnlockProgression"))
						{
							willUnlockProgression = (string)dictionary3["willUnlockProgression"];
						}
						if (dictionary3.ContainsKey("blockBolts"))
						{
							blockBolts = Convert.ToInt32(dictionary3["blockBolts"]);
						}
						if (dictionary3.ContainsKey("blockCoins"))
						{
							blockCoins = Convert.ToInt32(dictionary3["blockCoins"]);
						}
						if (dictionary3.ContainsKey("blockDiamonds"))
						{
							blockDiamonds = Convert.ToInt32(dictionary3["blockDiamonds"]);
						}
						if (dictionary3.ContainsKey("blockKeys"))
						{
							blockKeys = Convert.ToInt32(dictionary3["blockKeys"]);
						}
						if (dictionary3.ContainsKey("blockRewardCue"))
						{
							rewardCue = (string)dictionary3["blockRewardCue"];
						}
						break;
					case PsMinigameContext.Level:
						if (dictionary3.ContainsKey("levelDifficulty"))
						{
							minigameSearchParametres.m_difficulty = (PsGameDifficulty)(int)Enum.Parse(typeof(PsGameDifficulty), (string)dictionary3["levelDifficulty"]);
						}
						if (dictionary3.ContainsKey("levelFeatures"))
						{
							minigameSearchParametres.m_features = dictionary3["levelFeatures"] as string[];
						}
						if (dictionary3.ContainsKey("levelGameMode"))
						{
							minigameSearchParametres.m_gameMode = (PsGameMode)(int)Enum.Parse(typeof(PsGameMode), (string)dictionary3["levelGameMode"]);
						}
						if (dictionary3.ContainsKey("levelItems"))
						{
							minigameSearchParametres.m_items = dictionary3["levelItems"] as string[];
						}
						if (dictionary3.ContainsKey("levelPlayerUnit"))
						{
							minigameSearchParametres.m_playerUnitFilter = (string)dictionary3["levelPlayerUnit"];
						}
						if (dictionary3.ContainsKey("levelMedalTimes"))
						{
							List<object> list5 = (List<object>)dictionary3["levelMedalTimes"];
							array = new string[3];
							for (int k = 0; k < list5.Count; k++)
							{
								array[k] = (string)list5[k];
							}
						}
						break;
					case PsMinigameContext.EditorPuzzle:
						if (dictionary3.ContainsKey("puzzleLimitedItems"))
						{
							List<object> list6 = (List<object>)dictionary3["puzzleLimitedItems"];
							list4 = new List<string>();
							for (int l = 0; l < list6.Count; l++)
							{
								list4.Add((string)list6[l]);
							}
						}
						else
						{
							list4 = new List<string>();
						}
						break;
					case PsMinigameContext.EditorTemplate:
						if (dictionary3.ContainsKey("dataIdentifier"))
						{
							templateIdentifier = (string)dictionary3["dataIdentifier"];
						}
						if (dictionary3.ContainsKey("removedFromPath"))
						{
							removedFromPath = (bool)dictionary3["removedFromPath"];
						}
						break;
					case PsMinigameContext.Challenge:
						if (dictionary3.ContainsKey("timeleft"))
						{
							num = Convert.ToInt32(dictionary3["timeleft"]);
						}
						if (dictionary3.ContainsKey("duration"))
						{
							duration = Convert.ToInt32(dictionary3["duration"]);
						}
						if (dictionary3.ContainsKey("endTime"))
						{
							text3 = (string)dictionary3["endTime"];
						}
						if (dictionary3.ContainsKey("eventOver"))
						{
							eventOver = (bool)dictionary3["eventOver"];
						}
						if (dictionary3.ContainsKey("domeDestroyed"))
						{
							domeDestroyed = (bool)dictionary3["domeDestroyed"];
						}
						if (dictionary3.ContainsKey("timedEventId"))
						{
							text4 = (string)dictionary3["timedEventId"];
						}
						break;
					case PsMinigameContext.Fresh:
						if (dictionary3.ContainsKey("timeleft"))
						{
							num = Convert.ToInt32(dictionary3["timeleft"]);
						}
						if (dictionary3.ContainsKey("duration"))
						{
							duration = Convert.ToInt32(dictionary3["duration"]);
						}
						if (dictionary3.ContainsKey("endTime"))
						{
							text3 = (string)dictionary3["endTime"];
						}
						if (dictionary3.ContainsKey("eventOver"))
						{
							eventOver = (bool)dictionary3["eventOver"];
						}
						if (dictionary3.ContainsKey("domeDestroyed"))
						{
							domeDestroyed = (bool)dictionary3["domeDestroyed"];
						}
						break;
					case PsMinigameContext.Versus:
					case PsMinigameContext.VersusPath:
					case PsMinigameContext.Friendly:
						if (dictionary3.ContainsKey("removedFromPath"))
						{
							removedFromPath = (bool)dictionary3["removedFromPath"];
						}
						if (dictionary3.ContainsKey("timeleft"))
						{
							num = Convert.ToInt32(dictionary3["timeleft"]);
						}
						if (dictionary3.ContainsKey("duration"))
						{
							duration = Convert.ToInt32(dictionary3["duration"]);
						}
						if (dictionary3.ContainsKey("endTime"))
						{
							text3 = (string)dictionary3["endTime"];
						}
						if (dictionary3.ContainsKey("eventOver"))
						{
							eventOver = (bool)dictionary3["eventOver"];
						}
						if (dictionary3.ContainsKey("domeDestroyed"))
						{
							domeDestroyed = (bool)dictionary3["domeDestroyed"];
						}
						if (dictionary3.ContainsKey("timedEventId"))
						{
							text4 = (string)dictionary3["timedEventId"];
						}
						if ((psMinigameContext == PsMinigameContext.Versus || psMinigameContext == PsMinigameContext.Friendly) && text == "FloatingPath" && dictionary3.ContainsKey("versusData"))
						{
							Dictionary<string, object> dictionary4 = dictionary3["versusData"] as Dictionary<string, object>;
							dictionary4.Add("timedEventId", text4);
							dictionary4.Add("gameId", text2);
							dictionary4.Add("timeLeft", num);
							versusData = ParseVersusMetaData(dictionary3["versusData"] as Dictionary<string, object>);
						}
						break;
					}
					if (dictionary3.ContainsKey("dialogueIdentifiers"))
					{
						List<object> list7 = dictionary3["dialogueIdentifiers"] as List<object>;
						List<object> list8 = dictionary3["dialogueTriggers"] as List<object>;
						if (list7 != null)
						{
							for (int m = 0; m < list7.Count; m++)
							{
								hashtable.Add((string)list8[m], (string)list7[m]);
							}
						}
					}
					if (dictionary3.ContainsKey("unlocked"))
					{
						unlocked = (bool)dictionary3["unlocked"];
					}
				}
				PsGameLoop psGameLoop = null;
				switch (psMinigameContext)
				{
				case PsMinigameContext.Level:
					psGameLoop = new PsGameLoopLevel(text2, minigameSearchParametres, psPlanetPath2, id, levelNumber, score, unlocked, array);
					break;
				case PsMinigameContext.EditorPuzzle:
					psGameLoop = new PsGameLoopEditorPuzzle(list4, text2, psPlanetPath2, id, levelNumber, score, unlocked);
					break;
				case PsMinigameContext.EditorTemplate:
					psGameLoop = new PsGameLoopEditorTemplate(templateIdentifier, text2, psPlanetPath2, id, levelNumber, score, unlocked, removedFromPath);
					break;
				case PsMinigameContext.Versus:
				case PsMinigameContext.VersusPath:
				{
					string endTime = text3;
					psGameLoop = new PsGameLoopVersus(PsMinigameContext.Versus, versusData, text4, text2, psPlanetPath2, id, levelNumber, score, -1, removedFromPath, -1, endTime);
					break;
				}
				case PsMinigameContext.Friendly:
				{
					string endTime = text3;
					psGameLoop = new PsGameLoopFriendly(PsMinigameContext.Friendly, versusData, text4, text2, psPlanetPath2, id, levelNumber, score, -1, removedFromPath, -1, endTime);
					break;
				}
				case PsMinigameContext.Fresh:
					psGameLoop = new PsGameLoopFresh(PsMinigameContext.Fresh, text2, psPlanetPath2, id, levelNumber, score, num, duration, text3, eventOver, domeDestroyed);
					break;
				case PsMinigameContext.Challenge:
					psGameLoop = new PsGameLoopChallenge(PsMinigameContext.Challenge, text4, text2, psPlanetPath2, id, levelNumber, score, num, duration, text3, eventOver, domeDestroyed);
					break;
				case PsMinigameContext.Block:
				case PsMinigameContext.Signal:
					psGameLoop = ((psPathBlockType != PsPathBlockType.Antenna) ? new PsGameLoopBlockLevel(PsMinigameContext.Block, text2, psPathBlockType, requiredStars, unlockedProgression, willUnlockProgression, psPlanetPath2, id, score, blockCoins, blockDiamonds, blockBolts, blockKeys, unlocked, rewardCue) : (string.IsNullOrEmpty(text2) ? ((PsGameLoop)new PsGameLoopBlockSignalWithoutLevel(PsMinigameContext.Signal, PsPathBlockType.Antenna, requiredStars, unlockedProgression, willUnlockProgression, psPlanetPath2, id, score, blockCoins, blockDiamonds, blockBolts, blockKeys, unlocked, rewardCue)) : ((PsGameLoop)new PsGameLoopBlockSignal(PsMinigameContext.Signal, text2, psPathBlockType, requiredStars, unlockedProgression, willUnlockProgression, psPlanetPath2, id, score, blockCoins, blockDiamonds, blockBolts, blockKeys, unlocked, rewardCue))));
					break;
				}
				if (psGameLoop != null)
				{
					psGameLoop.m_gameModeFromProgressionData = gameModeFromProgressionData;
					psGameLoop.m_rentedVehicle = rentedVehicle;
					if (hashtable.Count > 0)
					{
						psGameLoop.m_dialogues = hashtable;
					}
					psPlanetPath2.m_nodeInfos.Add(psGameLoop);
				}
			}
		}
		if (_local)
		{
			for (int n = 0; n < list2.Count; n++)
			{
				if (list2[n].m_name.Contains("SidePath") && list2[n].m_startNodeId > 0)
				{
					psPlanetPath.GetNodeInfoFromPartialPath(list2[n].m_startNodeId).m_sidePath = list2[n];
				}
			}
		}
		return list;
	}

	public static Hashtable GenerateProgressionFloatingPathJson(PsPlanetPath _floatingPath)
	{
		for (int i = 0; i < _floatingPath.m_nodeInfos.Count; i++)
		{
			_floatingPath.m_nodeInfos[i].m_nodeId = i + 1;
		}
		List<PsPlanetPath> list = new List<PsPlanetPath>();
		list.Add(_floatingPath);
		return GenerateProgressionPathJson(null, -1, false, false, false, list);
	}

	public static Hashtable GenerateProgressionPathJson(PsPlanetPath _path)
	{
		List<PsPlanetPath> list = new List<PsPlanetPath>();
		list.Add(_path);
		return GenerateProgressionPathJson(null, -1, false, false, false, list);
	}

	public static Hashtable GenerateProgressionPathJson(PsGameLoop _mainPathNode, int _currentNode, bool _includeMainPath = true, bool _includeMainPathNodes = true, bool _includeSidePathNodes = true)
	{
		List<PsGameLoop> list = new List<PsGameLoop>();
		list.Add(_mainPathNode);
		return GenerateProgressionPathJson(list, _currentNode, _includeMainPath, _includeMainPathNodes, _includeSidePathNodes);
	}

	public static Hashtable GenerateProgressionPathJson(List<PsGameLoop> _mainPathNodes, int _currentNode, bool _includeMainPath = true, bool _includeMainPathNodes = true, bool _includeSidePathNodes = true, List<PsPlanetPath> _additionalPaths = null)
	{
		Hashtable hashtable = new Hashtable();
		List<Hashtable> list = new List<Hashtable>();
		hashtable.Add("paths", list);
		Hashtable hashtable2 = new Hashtable();
		List<Hashtable> list2 = new List<Hashtable>();
		if (_includeMainPath)
		{
			if (_currentNode < PsPlanetManager.GetCurrentPlanet().m_mainPath.m_currentNodeId)
			{
				Debug.LogError("SETTING MAINPATH CURRENT NODE TO: " + _currentNode + ". THIS IS A BUG!");
			}
			list.Add(hashtable2);
			hashtable2.Add("name", "MainPath");
			hashtable2.Add("currentNode", _currentNode);
			if (_includeMainPathNodes)
			{
				hashtable2.Add("nodes", list2);
			}
		}
		if (_additionalPaths == null)
		{
			_additionalPaths = new List<PsPlanetPath>();
		}
		if (_mainPathNodes != null)
		{
			for (int i = 0; i < _mainPathNodes.Count; i++)
			{
				if (_mainPathNodes[i].m_sidePath != null && _includeSidePathNodes)
				{
					_additionalPaths.Add(_mainPathNodes[i].m_sidePath);
				}
				AddPathNode(_mainPathNodes[i], list2, _includeMainPathNodes);
			}
		}
		for (int j = 0; j < _additionalPaths.Count; j++)
		{
			if (_additionalPaths[j] != null)
			{
				Hashtable hashtable3 = new Hashtable();
				list.Add(hashtable3);
				List<Hashtable> list3 = new List<Hashtable>();
				hashtable3.Add("nodes", list3);
				if (_additionalPaths[j].m_overwrite)
				{
					hashtable3.Add("overwrite", true);
				}
				hashtable3.Add("lane", _additionalPaths[j].m_lane);
				hashtable3.Add("name", _additionalPaths[j].m_name);
				hashtable3.Add("currentNode", _additionalPaths[j].m_currentNodeId);
				if (_additionalPaths[j].m_startNodeId > 0)
				{
					hashtable3.Add("startNode", _additionalPaths[j].m_startNodeId);
				}
				if (_additionalPaths[j].m_parentPath != null)
				{
					hashtable3.Add("parent", _additionalPaths[j].m_parentPath.m_name);
				}
				for (int k = 0; k < _additionalPaths[j].m_nodeInfos.Count; k++)
				{
					AddPathNode(_additionalPaths[j].m_nodeInfos[k], list3, true);
				}
			}
		}
		return hashtable;
	}

	private static void AddPathNode(PsGameLoop _node, List<Hashtable> _pathNodes, bool _includeRootNode)
	{
		if (!_includeRootNode)
		{
			return;
		}
		Hashtable hashtable = new Hashtable();
		_pathNodes.Add(hashtable);
		hashtable.Add("id", _node.m_nodeId);
		hashtable.Add("levelNumber", _node.m_levelNumber);
		hashtable.Add("score", _node.m_scoreBest);
		hashtable.Add("rented", _node.m_rentedVehicle);
		Hashtable hashtable2 = new Hashtable();
		hashtable.Add("data", hashtable2);
		hashtable2.Add("context", _node.m_context);
		hashtable2.Add("minigameId", _node.m_minigameId);
		hashtable2.Add("unlocked", _node.m_unlocked);
		if (_node.m_gameModeFromProgressionData != PsGameMode.Any)
		{
			hashtable2.Add("gameMode", _node.m_gameModeFromProgressionData);
		}
		if (_node.m_context == PsMinigameContext.Block || _node.m_context == PsMinigameContext.Signal)
		{
			if (_node is PsGameLoopBlockSignal)
			{
				PsGameLoopBlockSignal psGameLoopBlockSignal = _node as PsGameLoopBlockSignal;
				hashtable2.Add("blockType", psGameLoopBlockSignal.m_blockType);
				hashtable2.Add("requiredStars", psGameLoopBlockSignal.m_requiredStars);
				hashtable2.Add("unlockedProgression", psGameLoopBlockSignal.m_unlockedProgression);
				hashtable2.Add("willUnlockProgression", psGameLoopBlockSignal.m_willUnlockProgression);
				hashtable2.Add("blockCoins", psGameLoopBlockSignal.m_blockCoins);
				hashtable2.Add("blockDiamonds", psGameLoopBlockSignal.m_blockDiamonds);
				hashtable2.Add("blockKeys", psGameLoopBlockSignal.m_blockKeys);
				hashtable2.Add("blockBolts", psGameLoopBlockSignal.m_blockBolts);
				if (!string.IsNullOrEmpty(psGameLoopBlockSignal.m_rewardCue))
				{
					hashtable2.Add("blockRewardCue", psGameLoopBlockSignal.m_rewardCue);
				}
			}
			else if (_node is PsGameLoopBlockSignalWithoutLevel)
			{
				PsGameLoopBlockSignalWithoutLevel psGameLoopBlockSignalWithoutLevel = _node as PsGameLoopBlockSignalWithoutLevel;
				hashtable2.Add("blockType", psGameLoopBlockSignalWithoutLevel.m_blockType);
				hashtable2.Add("requiredStars", psGameLoopBlockSignalWithoutLevel.m_requiredStars);
				hashtable2.Add("unlockedProgression", psGameLoopBlockSignalWithoutLevel.m_unlockedProgression);
				hashtable2.Add("willUnlockProgression", psGameLoopBlockSignalWithoutLevel.m_willUnlockProgression);
				hashtable2.Add("blockCoins", psGameLoopBlockSignalWithoutLevel.m_blockCoins);
				hashtable2.Add("blockDiamonds", psGameLoopBlockSignalWithoutLevel.m_blockDiamonds);
				hashtable2.Add("blockKeys", psGameLoopBlockSignalWithoutLevel.m_blockKeys);
				hashtable2.Add("blockBolts", psGameLoopBlockSignalWithoutLevel.m_blockBolts);
				if (!string.IsNullOrEmpty(psGameLoopBlockSignalWithoutLevel.m_rewardCue))
				{
					hashtable2.Add("blockRewardCue", psGameLoopBlockSignalWithoutLevel.m_rewardCue);
				}
			}
			else
			{
				PsGameLoopBlockLevel psGameLoopBlockLevel = _node as PsGameLoopBlockLevel;
				hashtable2.Add("blockType", psGameLoopBlockLevel.m_blockType);
				hashtable2.Add("requiredStars", psGameLoopBlockLevel.m_requiredStars);
				hashtable2.Add("unlockedProgression", psGameLoopBlockLevel.m_unlockedProgression);
				hashtable2.Add("willUnlockProgression", psGameLoopBlockLevel.m_willUnlockProgression);
				hashtable2.Add("blockCoins", psGameLoopBlockLevel.m_blockCoins);
				hashtable2.Add("blockDiamonds", psGameLoopBlockLevel.m_blockDiamonds);
				hashtable2.Add("blockKeys", psGameLoopBlockLevel.m_blockKeys);
				hashtable2.Add("blockBolts", psGameLoopBlockLevel.m_blockBolts);
				if (!string.IsNullOrEmpty(psGameLoopBlockLevel.m_rewardCue))
				{
					hashtable2.Add("blockRewardCue", psGameLoopBlockLevel.m_rewardCue);
				}
			}
		}
		else if (_node.m_context == PsMinigameContext.Level)
		{
			PsGameLoopLevel psGameLoopLevel = _node as PsGameLoopLevel;
			hashtable2.Add("levelDifficulty", psGameLoopLevel.m_minigameSearchParametres.m_difficulty);
			hashtable2.Add("levelFeatures", psGameLoopLevel.m_minigameSearchParametres.m_features);
			hashtable2.Add("levelGameMode", psGameLoopLevel.m_minigameSearchParametres.m_gameMode);
			hashtable2.Add("levelItems", psGameLoopLevel.m_minigameSearchParametres.m_items);
			hashtable2.Add("levelPlayerUnit", psGameLoopLevel.m_minigameSearchParametres.m_playerUnitFilter);
			if (psGameLoopLevel.m_forcedMedalTimes != null && psGameLoopLevel.m_forcedMedalTimes.Length > 0 && !string.IsNullOrEmpty(psGameLoopLevel.m_forcedMedalTimes[0]))
			{
				hashtable2.Add("levelMedalTimes", psGameLoopLevel.m_forcedMedalTimes);
			}
		}
		else if (_node.m_context == PsMinigameContext.EditorPuzzle)
		{
			PsGameLoopEditorPuzzle psGameLoopEditorPuzzle = _node as PsGameLoopEditorPuzzle;
			if (psGameLoopEditorPuzzle.m_limitedItems != null)
			{
				hashtable2.Add("puzzleLimitedItems", psGameLoopEditorPuzzle.m_limitedItems.ToArray());
			}
		}
		else if (_node.m_context == PsMinigameContext.EditorTemplate)
		{
			PsGameLoopEditorTemplate psGameLoopEditorTemplate = _node as PsGameLoopEditorTemplate;
			hashtable2.Add("dataIdentifier", psGameLoopEditorTemplate.m_templateIdentifier);
			hashtable2.Add("removedFromPath", psGameLoopEditorTemplate.m_removedFromPath);
		}
		else if (_node.m_context == PsMinigameContext.Fresh)
		{
			PsGameLoopFresh psGameLoopFresh = _node as PsGameLoopFresh;
			hashtable2.Add("duration", psGameLoopFresh.m_eventDuration);
			hashtable2.Add("timeleft", psGameLoopFresh.m_eventTimeLeft);
			hashtable2.Add("endTime", psGameLoopFresh.m_endTime.ToString("O"));
			hashtable2.Add("eventOver", psGameLoopFresh.m_eventOver);
			hashtable2.Add("domeDestroyed", psGameLoopFresh.m_domeDestroyed);
		}
		else if (_node.m_context == PsMinigameContext.Challenge)
		{
			PsGameLoopChallenge psGameLoopChallenge = _node as PsGameLoopChallenge;
			hashtable2.Add("duration", psGameLoopChallenge.m_eventDuration);
			hashtable2.Add("timeleft", psGameLoopChallenge.m_eventTimeLeft);
			hashtable2.Add("endTime", psGameLoopChallenge.m_endTime.ToString("O"));
			hashtable2.Add("timedEventId", psGameLoopChallenge.m_timedEventId);
			hashtable2.Add("eventOver", psGameLoopChallenge.m_eventOver);
			hashtable2.Add("domeDestroyed", psGameLoopChallenge.m_domeDestroyed);
		}
		else if (_node.m_context == PsMinigameContext.Versus || _node.m_context == PsMinigameContext.VersusPath || _node.m_context == PsMinigameContext.Friendly)
		{
			PsGameLoopVersus psGameLoopVersus = _node as PsGameLoopVersus;
			hashtable2.Add("removedFromPath", psGameLoopVersus.m_removedFromPath);
			hashtable2.Add("duration", psGameLoopVersus.m_eventDuration);
			hashtable2.Add("timeleft", psGameLoopVersus.m_eventTimeLeft);
			hashtable2.Add("endTime", psGameLoopVersus.m_endTime.ToString("O"));
			hashtable2.Add("timedEventId", psGameLoopVersus.m_timedEventId);
			hashtable2.Add("eventOver", psGameLoopVersus.m_eventOver);
			hashtable2.Add("domeDestroyed", psGameLoopVersus.m_domeDestroyed);
			if ((_node.m_context == PsMinigameContext.Versus || _node.m_context == PsMinigameContext.Friendly) && _node.m_path.m_name == "FloatingPath")
			{
				Hashtable value = GenerateVersusDataHash(psGameLoopVersus);
				hashtable2.Add("versusData", value);
			}
		}
		if (_node.m_dialogues == null || _node.m_dialogues.Count <= 0)
		{
			return;
		}
		string[] array = new string[_node.m_dialogues.Count];
		string[] array2 = new string[_node.m_dialogues.Count];
		int num = 0;
		foreach (DictionaryEntry dialogue in _node.m_dialogues)
		{
			array2[num] = (string)dialogue.Key;
			array[num] = (string)dialogue.Value;
			num++;
		}
		hashtable2.Add("dialogueIdentifiers", array);
		hashtable2.Add("dialogueTriggers", array2);
	}

	public static Hashtable GenerateVersusDataHash(PsGameLoopVersus _versusInfo)
	{
		Hashtable hashtable = new Hashtable();
		if (_versusInfo.m_versusData != null)
		{
			hashtable.Add("currentPlayer", _versusInfo.m_versusData.currentPlayer);
			hashtable.Add("winner", _versusInfo.m_versusData.winner);
			hashtable.Add("round", _versusInfo.m_versusData.round);
			hashtable.Add("matchball", _versusInfo.m_versusData.matchBall);
			hashtable.Add("challengedPlayer", _versusInfo.m_versusData.challengedPlayer);
			List<object> list = new List<object>();
			for (int i = 0; i < _versusInfo.m_versusData.participants.Count(); i++)
			{
				list.Add(_versusInfo.m_versusData.participants[i].GenerateParticipantHashTable());
			}
			hashtable.Add("participants", list);
		}
		else
		{
			Debug.LogWarning("VersusData was null (THIS IS A PROBLEM (maybe) )");
		}
		return hashtable;
	}

	public static string GetResourceHash(Hashtable _resources)
	{
		string text = string.Empty;
		if (_resources.ContainsKey("coins"))
		{
			text += _resources["coins"].ToString();
		}
		if (_resources.ContainsKey("diamonds"))
		{
			text += _resources["diamonds"].ToString();
		}
		if (_resources.ContainsKey("keys"))
		{
			text += _resources["keys"].ToString();
		}
		if (_resources.ContainsKey("maxKeys"))
		{
			text += _resources["maxKeys"].ToString();
		}
		if (_resources.ContainsKey("level"))
		{
			text += _resources["level"].ToString();
		}
		if (_resources.ContainsKey("bolts"))
		{
			text += _resources["bolts"].ToString();
		}
		if (_resources.ContainsKey("stars"))
		{
			text += _resources["stars"].ToString();
		}
		if (_resources.ContainsKey("itemLevel"))
		{
			text += _resources["itemLevel"].ToString();
		}
		if (_resources.ContainsKey("cups"))
		{
			text += _resources["cups"].ToString();
		}
		if (_resources.ContainsKey("rank"))
		{
			text += _resources["rank"].ToString();
		}
		if (_resources.ContainsKey("tokens"))
		{
			text += _resources["tokens"].ToString();
		}
		return GenerateHash(text);
	}

	public static string GenerateJson(Hashtable _table)
	{
		return Json.Serialize(_table);
	}

	public static List<Hashtable> GetUpgradeSums(Hashtable upgrades)
	{
		List<Hashtable> list = new List<Hashtable>();
		foreach (DictionaryEntry upgrade in upgrades)
		{
			int num = 0;
			Hashtable hashtable = upgrade.Value as Hashtable;
			foreach (DictionaryEntry item in hashtable)
			{
				if ((string)item.Key != "tier")
				{
					num += Convert.ToInt32(item.Value);
				}
			}
			Hashtable hashtable2 = new Hashtable();
			hashtable2.Add("vehicle", upgrade.Key.ToString());
			hashtable2.Add("sum", num);
			list.Add(hashtable2);
		}
		return list;
	}

	public static string GeneratePlayerSetDataJson(Hashtable _setData, ref List<string> _tags)
	{
		return Json.Serialize(GeneratePlayerSetData(_setData, ref _tags));
	}

	public static Hashtable GeneratePlayerSetData(Hashtable _setData, ref List<string> _tags)
	{
		Hashtable hashtable;
		if (_setData.ContainsKey("Resources"))
		{
			hashtable = (Hashtable)_setData["Resources"];
			if (!hashtable.ContainsKey("coins"))
			{
				hashtable.Add("coins", PsMetagameManager.m_playerStats.coins);
			}
			else
			{
				hashtable["coins"] = PsMetagameManager.m_playerStats.coins;
			}
			if (!hashtable.ContainsKey("diamonds"))
			{
				hashtable.Add("diamonds", PsMetagameManager.m_playerStats.diamonds);
			}
			else
			{
				hashtable["diamonds"] = PsMetagameManager.m_playerStats.diamonds;
			}
			if (!hashtable.ContainsKey("stars"))
			{
				hashtable.Add("stars", PsMetagameManager.m_playerStats.stars);
			}
			else
			{
				hashtable["stars"] = PsMetagameManager.m_playerStats.stars;
			}
			if (!hashtable.ContainsKey("bolts"))
			{
				hashtable.Add("bolts", PsMetagameManager.m_playerStats.bolts);
			}
			else
			{
				hashtable["bolts"] = PsMetagameManager.m_playerStats.bolts;
			}
			if (!hashtable.ContainsKey("level"))
			{
				hashtable.Add("level", PsMetagameManager.m_playerStats.level);
			}
			else
			{
				hashtable["level"] = PsMetagameManager.m_playerStats.level;
			}
			if (!hashtable.ContainsKey("keys"))
			{
				hashtable.Add("keys", PsMetagameManager.m_playerStats.keys);
			}
			else
			{
				hashtable["keys"] = PsMetagameManager.m_playerStats.keys;
			}
			if (!hashtable.ContainsKey("maxKeys"))
			{
				hashtable.Add("maxKeys", PsMetagameManager.m_playerStats.maxKeys);
			}
			else
			{
				hashtable["maxKeys"] = PsMetagameManager.m_playerStats.maxKeys;
			}
			if (!hashtable.ContainsKey("cups"))
			{
				hashtable.Add("cups", PsMetagameManager.m_playerStats.cups);
			}
			else
			{
				hashtable["cups"] = PsMetagameManager.m_playerStats.cups;
			}
			if (!hashtable.ContainsKey("rank"))
			{
				hashtable.Add("rank", PsMetagameManager.m_playerStats.rank);
			}
			else
			{
				hashtable["rank"] = PsMetagameManager.m_playerStats.rank;
			}
			if (!hashtable.ContainsKey("upgrades"))
			{
				hashtable.Add("upgrades", PsMetagameManager.m_playerStats.upgrades);
			}
			else
			{
				hashtable["upgrades"] = PsMetagameManager.m_playerStats.upgrades;
			}
			if (!hashtable.ContainsKey("upgradeSums"))
			{
				hashtable.Add("upgradeSums", GetUpgradeSums(PsMetagameManager.m_playerStats.upgrades));
			}
			else
			{
				hashtable["upgradeSums"] = GetUpgradeSums(PsMetagameManager.m_playerStats.upgrades);
			}
			if (!hashtable.ContainsKey("boosters"))
			{
				hashtable.Add("boosters", PsMetagameManager.m_playerStats.boosters);
			}
			else
			{
				hashtable["boosters"] = PsMetagameManager.m_playerStats.boosters;
			}
			if (hashtable.ContainsKey("keyRefresh"))
			{
				_tags.Add("KeyRefresh");
				hashtable["keyRefresh"] = PsMetagameManager.m_keyReloadTimeLeft;
			}
			else
			{
				_tags.Add("KeyRefresh");
				hashtable.Add("keyRefresh", PsMetagameManager.m_keyReloadTimeLeft);
			}
			if (hashtable.ContainsKey("itemLevel"))
			{
				hashtable["itemLevel"] = PsMetagameManager.m_playerStats.itemLevel;
			}
			_setData["Resources"] = hashtable;
		}
		else
		{
			hashtable = new Hashtable();
			hashtable.Add("coins", PsMetagameManager.m_playerStats.coins);
			hashtable.Add("diamonds", PsMetagameManager.m_playerStats.diamonds);
			hashtable.Add("stars", PsMetagameManager.m_playerStats.stars);
			hashtable.Add("bolts", PsMetagameManager.m_playerStats.bolts);
			hashtable.Add("level", PsMetagameManager.m_playerStats.level);
			hashtable.Add("keys", PsMetagameManager.m_playerStats.keys);
			hashtable.Add("maxKeys", PsMetagameManager.m_playerStats.maxKeys);
			hashtable.Add("upgrades", PsMetagameManager.m_playerStats.upgrades);
			hashtable.Add("upgradeSums", GetUpgradeSums(PsMetagameManager.m_playerStats.upgrades));
			hashtable.Add("boosters", PsMetagameManager.m_playerStats.boosters);
			hashtable.Add("itemLevel", PsMetagameManager.m_playerStats.itemLevel);
			hashtable.Add("cups", PsMetagameManager.m_playerStats.cups);
			hashtable.Add("rank", PsMetagameManager.m_playerStats.rank);
			hashtable.Add("keyRefresh", PsMetagameManager.m_keyReloadTimeLeft);
			_setData.Add("Resources", hashtable);
		}
		_tags.Add("Coins");
		_tags.Add("Diamonds");
		_tags.Add("Stars");
		_tags.Add("Bolts");
		_tags.Add("Keys");
		_tags.Add("MaxKeys");
		_tags.Add("Upgrades");
		_tags.Add("Boosters");
		if (_setData.ContainsKey("hash"))
		{
			_setData["hash"] = GetResourceHash(hashtable);
		}
		else
		{
			_setData.Add("hash", GetResourceHash(hashtable));
		}
		return _setData;
	}

	public static HttpC ParsePathSync(HttpC _c)
	{
		Dictionary<string, object> dictionary = ParseServerResponse(_c.www.text);
		if (dictionary.ContainsKey("lastPathSync"))
		{
			PlayerPrefsX.SetLastSync((string)dictionary["lastPathSync"]);
		}
		return _c;
	}

	public static RewardData ParseRewardData(HttpC _c)
	{
		Dictionary<string, object> dictionary = ParseServerResponse(_c.www.text);
		RewardData rewardData = new RewardData();
		Dictionary<string, object> dictionary2 = null;
		if (dictionary.ContainsKey("reward"))
		{
			dictionary2 = dictionary["reward"] as Dictionary<string, object>;
			if (dictionary2.ContainsKey("rewardCoins"))
			{
				rewardData.m_coins = Convert.ToInt32(dictionary2["rewardCoins"]);
			}
			if (dictionary2.ContainsKey("rewardDiamonds"))
			{
				rewardData.m_diamonds = Convert.ToInt32(dictionary2["rewardDiamonds"]);
			}
			PsMetagameManager.m_playerStats.updated = true;
		}
		return rewardData;
	}

	public static NotificationData[] ParseNotifications(Dictionary<string, object> _dictionary)
	{
		List<object> list = _dictionary["data"] as List<object>;
		NotificationData[] array = new NotificationData[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			Dictionary<string, object> dictionary = list[i] as Dictionary<string, object>;
			array[i].playerId = (string)dictionary["playerId"];
			array[i].message = (string)dictionary["message"];
			array[i].read = (bool)dictionary["read"];
			array[i].type = (string)dictionary["type"];
			if (dictionary.ContainsKey("data"))
			{
				array[i].data = (string)dictionary["data"];
			}
		}
		return array;
	}

	public static Hashtable CreateMinigameMetaDataHashtable(PsMinigameMetaData _minigameMetaData, bool _playerIsCreator = false)
	{
		Hashtable hashtable = new Hashtable();
		if (_playerIsCreator)
		{
			if (!string.IsNullOrEmpty(_minigameMetaData.id))
			{
				hashtable.Add("id", _minigameMetaData.id);
			}
			hashtable.Add("creatorName", PlayerPrefsX.GetUserName());
			hashtable.Add("creatorId", PlayerPrefsX.GetUserId());
			if (!string.IsNullOrEmpty(PlayerPrefsX.GetFacebookId()))
			{
				hashtable.Add("creatorFacebookId", PlayerPrefsX.GetFacebookId());
			}
			if (!string.IsNullOrEmpty(PlayerPrefsX.GetGameCenterId()))
			{
				hashtable.Add("creatorGameCenterId", PlayerPrefsX.GetGameCenterId());
			}
		}
		else
		{
			hashtable.Add("id", _minigameMetaData.id);
			hashtable.Add("creatorName", _minigameMetaData.creatorName);
			hashtable.Add("creatorId", _minigameMetaData.creatorId);
			hashtable.Add("creatorFacebookId", _minigameMetaData.creatorFacebookId);
			hashtable.Add("creatorGameCenterId", _minigameMetaData.creatorGameCenterId);
		}
		hashtable.Add("editSessionCount", _minigameMetaData.editSessionCount);
		hashtable.Add("groundsModificationCount", _minigameMetaData.groundsModificationCount);
		hashtable.Add("itemsModificationCount", _minigameMetaData.itemsModificationCount);
		hashtable.Add("lastPlaySessionStartCount", _minigameMetaData.lastPlaySessionStartCount);
		hashtable.Add("timeSpentInEditMode", _minigameMetaData.timeSpentInEditMode);
		hashtable.Add("name", _minigameMetaData.name);
		hashtable.Add("description", _minigameMetaData.description);
		hashtable.Add("clientVersion", Main.CLIENT_VERSION());
		hashtable.Add("gameMode", _minigameMetaData.gameMode);
		hashtable.Add("complexity", _minigameMetaData.complexity);
		hashtable.Add("playerUnit", _minigameMetaData.playerUnit);
		hashtable.Add("itemsUsed", _minigameMetaData.itemsUsed);
		hashtable.Add("levelRequirement", _minigameMetaData.levelRequirement);
		hashtable.Add("timeSpentEditing", _minigameMetaData.timeSpentEditing);
		hashtable.Add("creatorUpgrades", _minigameMetaData.creatorUpgrades);
		return hashtable;
	}

	public static DataBlob CreateLevelDataBlob(byte[] _screenshotData)
	{
		MemoryStream inputStream = LevelSerializer.SerializeLevelToStream(PsState.m_activeMinigame);
		MemoryStream memoryStream = FilePacker.ZipStream(inputStream, 5);
		Hashtable obj = CreateMinigameMetaDataHashtable(PsState.m_activeGameLoop.m_minigameMetaData, true);
		string text = Json.Serialize(obj);
		Debug.Log(text);
		Hashtable header = new Hashtable();
		byte[] array = null;
		array = ((_screenshotData == null) ? FilePacker.CombineByteArrays(new byte[2][]
		{
			FilePacker.StringToByteArray(text),
			memoryStream.ToArray()
		}, header) : FilePacker.CombineByteArrays(new byte[3][]
		{
			FilePacker.StringToByteArray(text),
			memoryStream.ToArray(),
			_screenshotData
		}, header));
		return new DataBlob
		{
			header = header,
			data = array
		};
	}

	public static DataBlob CreatePublishLevelDataBlob(byte[] _screenshotData, string _progressionJson, DataBlob _ghostData)
	{
		MemoryStream memoryStream = FilePacker.ZipStream(LevelSerializer.SerializeLevelToStream(PsState.m_activeMinigame), 5);
		Hashtable hashtable = CreateMinigameMetaDataHashtable(PsState.m_activeGameLoop.m_minigameMetaData, true);
		hashtable["bestTime"] = PsState.m_activeMinigame.m_lastSentTimeScore;
		hashtable["median"] = PsState.m_activeMinigame.m_lastSentTimeScore;
		string text = Json.Serialize(hashtable);
		Debug.Log(text);
		if (!string.IsNullOrEmpty(_progressionJson))
		{
			return GenerateLevelDataBlob(new byte[5][]
			{
				FilePacker.StringToByteArray(text),
				memoryStream.ToArray(),
				_screenshotData,
				_ghostData.data,
				FilePacker.StringToByteArray(_progressionJson)
			});
		}
		return GenerateLevelDataBlob(new byte[4][]
		{
			FilePacker.StringToByteArray(text),
			memoryStream.ToArray(),
			_screenshotData,
			_ghostData.data
		});
	}

	public static DataBlob GenerateLevelDataBlob(byte[][] byteArray)
	{
		Hashtable header = new Hashtable();
		byte[] array = null;
		array = FilePacker.CombineByteArrays(byteArray, header);
		return new DataBlob
		{
			header = header,
			data = array
		};
	}

	public static byte[] CreateLevelZipWithoutMetadataAndScreenshot()
	{
		MemoryStream inputStream = LevelSerializer.SerializeLevelToStream(PsState.m_activeMinigame);
		MemoryStream memoryStream = FilePacker.ZipStream(inputStream, 5);
		return memoryStream.ToArray();
	}

	public static DataBlob CreatePathDataBlob(string _itemLevelJson, string _pathJson)
	{
		Hashtable header = new Hashtable();
		byte[] array = FilePacker.ZipBytes(Encoding.UTF8.GetBytes(_pathJson));
		byte[] array2 = null;
		array2 = FilePacker.CombineByteArrays(new byte[2][]
		{
			FilePacker.StringToByteArray(_itemLevelJson),
			array
		}, header);
		return new DataBlob
		{
			header = header,
			data = array2
		};
	}

	public static DataBlob CreateGhostDataBlob(Ghost _ghost)
	{
		MemoryStream inputStream = Ghost.SerializeToStream(_ghost);
		MemoryStream memoryStream = FilePacker.ZipStream(inputStream, 5);
		return new DataBlob
		{
			header = null,
			data = memoryStream.ToArray()
		};
	}

	public static string GenerateUserJSON()
	{
		Hashtable hashtable = new Hashtable();
		if (!string.IsNullOrEmpty(KeyChain.GetUserId()))
		{
			hashtable.Add("id", KeyChain.GetUserId());
		}
		else if (!string.IsNullOrEmpty(PlayerPrefsX.GetUserId()))
		{
			hashtable.Add("id", PlayerPrefsX.GetUserId());
		}
		if (!string.IsNullOrEmpty(KeyChain.GetUserName()))
		{
			hashtable.Add("name", KeyChain.GetUserName());
		}
		else
		{
			hashtable.Add("name", PlayerPrefsX.GetUserName());
		}
		if (PlayerPrefsX.GetPathVersion() != 0)
		{
			hashtable.Add("pathVersion", PlayerPrefsX.GetPathVersion());
		}
		hashtable.Add("clientVersion", Main.CLIENT_VERSION() + " " + Application.platform.ToString());
		hashtable.Add("locale", "EN");
		hashtable.Add("udid", Metrics.GetMetricsGUID());
		hashtable.Add("majorPath", 2);
		string text = Json.Serialize(hashtable);
		Debug.Log("USER JSON GENERATED: " + text);
		return text;
	}

	public static string GenerateIdListJson(List<PsMinigameMetaData> _games)
	{
		Hashtable hashtable = new Hashtable();
		string[] array = new string[_games.Count];
		for (int i = 0; i < _games.Count; i++)
		{
			array[i] = _games[i].id;
		}
		hashtable.Add("games", array);
		string text = Json.Serialize(hashtable);
		Debug.Log("Id list json generated: " + text);
		return text;
	}

	public static Dictionary<string, object> LoadLocalPathData()
	{
		string text = Main.GetPersistentDataPath() + "/LocalInitialData.txt";
		if (File.Exists(text))
		{
			Debug.Log("LOADING LOCAL INITIAL PATH FILE: " + text);
			StreamReader streamReader = File.OpenText(text);
			string text2 = string.Empty;
			for (string text3 = streamReader.ReadLine(); text3 != null; text3 = streamReader.ReadLine())
			{
				text2 += text3;
			}
			return Json.Deserialize(text2) as Dictionary<string, object>;
		}
		Debug.LogWarning("NO LOCAL INITIAL PATH FILE: " + text);
		return null;
	}

	public static void SaveLocalPathData(string _json)
	{
		string path = Main.GetPersistentDataPath() + "/LocalInitialData.txt";
		StreamWriter streamWriter = File.CreateText(path);
		streamWriter.WriteLine(_json);
		streamWriter.Close();
	}

	public static void SaveFile(string _name, string _extension, byte[] _data)
	{
		string text = Main.GetPersistentDataPath() + "/" + _name;
		FileStream fileStream = File.Create(text);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream, Encoding.UTF8);
		binaryWriter.Write(_data);
		binaryWriter.Close();
		File.Move(text, Path.ChangeExtension(text, _extension));
		fileStream.Close();
	}

	private static string GetSoundFileName(int _index)
	{
		if (_index == 0)
		{
			return "FMOD_bank_list";
		}
		return "Master Bank";
	}

	private static string GetSoundFileExtension(int _index)
	{
		switch (_index)
		{
		case 0:
			return ".txt";
		case 1:
			return ".strings.bank";
		case 2:
			return ".bank";
		default:
			return ".txt";
		}
	}

	public static RewardData ParseUnclaimedRewards(HttpC _c)
	{
		Dictionary<string, object> dictionary = ParseServerResponse(_c.www.text);
		RewardData rewardData = new RewardData();
		if (dictionary.ContainsKey("coins"))
		{
			rewardData.m_coins = Convert.ToInt32(dictionary["coins"]);
		}
		return rewardData;
	}
}
