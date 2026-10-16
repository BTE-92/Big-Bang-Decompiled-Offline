using System.Collections;

public class PsMinigameMetaData
{
	public const string TIME_SPENT_IN_EDIT_MODE = "timeSpentInEditMode";

	public const string EDIT_SESSION_COUNT = "editSessionCount";

	public const string ITEMS_MODIFICATION_COUNT = "itemsModificationCount";

	public const string GROUNDS_MODIFICATION_COUNT = "groundsModificationCount";

	public const string LAST_PLAY_SESSION_START_COUNT = "lastPlaySessionStartCount";

	public string id;

	public string name = "Unnamed Track";

	public string creatorName;

	public string creatorId;

	public string creatorFacebookId;

	public string creatorGameCenterId;

	public bool published;

	public int timesPlayed;

	public int timesLiked;

	public int timesRated;

	public int timeSpentEditing;

	public string description = "No Description";

	public int clientVersion;

	public PsGameDifficulty difficulty;

	public PsGameMode gameMode = PsGameMode.Race;

	public PsRating rating = PsRating.Unrated;

	public bool played;

	public int score;

	public int rewardCoins;

	public int totalCoinsEarned;

	public bool publishReward;

	public bool playReward;

	public int complexity;

	public int levelRequirement;

	public string playerUnit;

	public string[] itemsUsed;

	public string researchIdentifier;

	public int bestTime;

	public int medianTime;

	public int totalWinners;

	public int timeScore;

	public int oneStarWinners;

	public int twoStarWinners;

	public int threeStarWinners;

	public int totalPlayers;

	public Hashtable creatorUpgrades;

	public float quality;

	public bool template;

	public bool hidden;

	public int timeSpentInEditMode;

	public int editSessionCount;

	public int itemsModificationCount;

	public int groundsModificationCount;

	public int lastPlaySessionStartCount;

	public void DebugPrint()
	{
		Debug.Log("MINIGAME METADATA: " + name);
		Debug.Log("ID: " + id);
		Debug.Log("CREATOR NAME: " + creatorName);
		Debug.Log("PUBLISHED: " + published);
		Debug.Log("TIME SPENT EDITING:" + timeSpentEditing);
		Debug.Log("TIMES PLAYED: " + timesPlayed);
		Debug.Log("TIMES LIKED: " + timesLiked);
		Debug.Log("CLIENT VERSION: " + clientVersion);
		Debug.Log("GAME MODE: " + gameMode);
		Debug.Log("COMPLEXITY: " + complexity);
		Debug.Log("LEVEL REQUIREMENT: " + levelRequirement);
		Debug.Log("PLAYER UNIT: " + playerUnit);
		if (itemsUsed != null)
		{
			for (int i = 0; i < itemsUsed.Length; i++)
			{
				Debug.Log("ITEMS USED " + i + ": " + itemsUsed[i]);
			}
		}
	}
}
