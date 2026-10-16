using System.Linq;

public class PsTournamentMetaData
{
	public bool waitingForRewards;

	public string tournamentId;

	public int timeLeft;

	public int duration;

	public int playerRewardCoins;

	public int playerRewardDiamonds;

	public int participationCoinsCost;

	public int participationDiamondsCost;

	public bool participated;

	public bool daily;

	public string name;

	public int time;

	public double timeLeftSetTime;

	public bool claimed;

	public int participantCount;

	public string gameId;

	public string gameName;

	public PsGameDifficulty difficulty;

	public string creatorName;

	public string creatorId;

	public string playerUnit;

	public PsRating rating = PsRating.Unrated;

	public string creatorFacebookId;

	public string creatorGameCenterId;

	public string gameDescription;

	public PercentileField[] percentiles;

	public int[] topThreeRewards;

	public HighscoreDataEntry[] topScores;

	public int totalPot;

	public int GetReward()
	{
		int position = GetPosition();
		if (position >= 1 && position <= 3)
		{
			return topThreeRewards[position - 1];
		}
		return GetPercentile().reward;
	}

	public int GetPosition()
	{
		for (int i = 0; i < topScores.Length; i++)
		{
			if (topScores[i].time == time)
			{
				return i + 1;
			}
		}
		return 0;
	}

	public PercentileField GetPercentile(int _returnShift = 0)
	{
		if (percentiles.Length == 0)
		{
			PercentileField percentileField = new PercentileField();
			percentileField.reward = 0;
			percentileField.percentile = 0;
			percentileField.index = 1;
			return percentileField;
		}
		if (participantCount > 9)
		{
			for (int i = 0; i < percentiles.Length; i++)
			{
				if (time != 0 && (time <= percentiles[i].percentile || i == 9))
				{
					return percentiles[i + _returnShift];
				}
			}
		}
		else
		{
			for (int j = 0; j < topScores.Length; j++)
			{
				if (time != 0 && time <= topScores[j].time)
				{
					return percentiles[j + _returnShift];
				}
			}
		}
		return percentiles.Last();
	}

	public int GetNextBestPosition()
	{
		int position = GetPosition();
		switch (position)
		{
		case 1:
			return -1;
		case 2:
		case 3:
		case 4:
			return position - 1;
		default:
			if (percentiles[8].percentile == 0 && position != 0)
			{
				return position - 1;
			}
			return -1;
		}
	}

	public int GetNextBestRewardTime()
	{
		int position = GetPosition();
		switch (position)
		{
		case 1:
			return -1;
		case 2:
		case 3:
		case 4:
			return topScores[position - 2].time;
		default:
			if (percentiles[8].percentile == 0 && position != 0)
			{
				return topScores[position - 2].time;
			}
			Debug.Log("REWARD TIME: " + GetPercentile(-1).percentile);
			return GetPercentile(-1).percentile;
		}
	}

	public int GetNextBestReward()
	{
		int position = GetPosition();
		switch (position)
		{
		case 1:
			return -1;
		case 2:
		case 3:
		case 4:
			return topThreeRewards[position - 2];
		default:
			Debug.Log("REWARD: " + GetPercentile(-1).reward);
			return GetPercentile(-1).reward;
		}
	}
}
