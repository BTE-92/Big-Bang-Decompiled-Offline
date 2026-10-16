using System.Collections;
using System.Collections.Generic;
using Server.Utility;

public struct PlayerData
{
	public string playerId;

	public string name;

	public string gameCenterId;

	public string facebookId;

	public string ninjaCreationTimestamp;

	public string countryCode;

	public int itemDbVersion;

	public int coins;

	public int diamonds;

	public int bolts;

	public int stars;

	public int keys;

	public int maxKeys;

	public int itemLevel;

	public int level;

	public int cups;

	public int rank;

	public Hashtable upgrades;

	public Hashtable boosters;

	public Hashtable data;

	public ClientConfig clientConfig;

	public bool acceptNotifications;

	public Hashtable setData;

	public int keyTimeLeft;

	public List<string> claimedTutorials;

	public string hash;

	public List<string> offroadCarVisual;

	public List<string> offroadCarUpgrade;

	public List<string> motorcycleVisual;

	public List<string> motorcycleUpgrade;
}
