using System;
using BBR249Server.Data;

namespace BBR249Server.Api;

// "BTE": a built-in player who is everybody's friend. It never plays by itself: when somebody challenges it to a friend race and
// has finished their own run, it answers at once with a time close to theirs (a bit faster or slower), so the whole friend race
// (challenge, result bubble on the map, claim) can be tried without a second player.
public static class BotPlayer
{
	// 32 hex characters, like the ids of real players
	public const string Id = "b7ebb7ebb7ebb7ebb7ebb7ebb7ebb7eb";

	public const string Name = "BTE";

	public static void Ensure()
	{
		if (PlayerApi.GetPlayer(Id) == null)
		{
			Db.Exec("INSERT INTO players (id, name, udid, layoutVersion, upgrades, boosters) VALUES ($0,$1,$2,$3,$4,$5)", Id, Name, "bte-bot", 3, PlayerApi.MaxUpgradesJson, PlayerApi.NewPlayerBoostersJson);
			Console.WriteLine("[Bot] " + Name + " created (" + Id + ")");
		}
		// friends with every player, in both directions (new players are linked at their first login)
		Db.Exec("INSERT OR IGNORE INTO follows (followerId, followeeId) SELECT $0, id FROM players WHERE id<>$0", Id);
		Db.Exec("INSERT OR IGNORE INTO follows (followerId, followeeId) SELECT id, $0 FROM players WHERE id<>$0", Id);
	}

	// A versus match against BTE: BTE is the player who "already ran" (it sets the time to beat, with a recorded run as its ghost) and the
	// player races it right away. Its run is the best recorded time of the level (so it is a real run with a real ghost);
	// on a level nobody has finished yet it is the level's best time, or a default one.
	public static string CreateVersusMatch(Row level, string playerId)
	{
		string gameId = level.S("id");
		Row? best = Db.One("SELECT time, stars, ghost FROM scores WHERE gameId=$0 AND time>0 ORDER BY time LIMIT 1", gameId);
		long time;
		long stars;
		byte[]? ghost;
		if (best != null)
		{
			time = best.L("time", 0L);
			stars = Math.Max(1L, best.L("stars", 1L));
			ghost = best.B("ghost");
		}
		else
		{
			time = level.L("bestTime", 0L) > 0 ? level.L("bestTime", 0L) : 15000L;
			stars = 3L;
			ghost = null;
		}
		if (ghost == null || ghost.Length < 2)
		{
			ghost = level.B("creatorGhost");
		}
		string id = PlayerApi.NewId();
		Db.Exec("INSERT INTO challenges (id, kind, gameId, p1, p2, status, currentPlayer, t1, stars1, tries1, ghost1, tries2) VALUES ($0,'versus',$1,$2,$3,'active',$3,$4,$5,1,$6,0)", id, gameId, Id, playerId, time, stars, ghost);
		Console.WriteLine($"[Bot] versus {id}: {PlayerApi.GetPlayer(playerId)?.S("name")} races {Name} on {gameId} (time to beat {time} ms{(ghost != null && ghost.Length > 1 ? ", with a ghost" : ", no ghost")})");
		return id;
	}

	// BTE accepts a friend challenge the moment it is created (it is "online" at all times).
	public static void AcceptIfChallenged(string challengeId)
	{
		Row? ch = Db.One("SELECT * FROM challenges WHERE id=$0", challengeId);
		if (ch == null || ch.S("kind") != "friendly" || ch.S("challenged") != Id || ch.S("p2") != "" || ch.S("status") != "pending")
		{
			return;
		}
		Db.Exec("UPDATE challenges SET p2=$0, status='active', tries2=0, updatedAt=CURRENT_TIMESTAMP WHERE id=$1", Id, challengeId);
	}

	public const int MinHoursBetweenChallenges = 1;

	public const int MaxHoursBetweenChallenges = 48;

	private static int _hoursBetweenChallenges = 2;

	// Hours between two challenges of BTE (1 to 48, 2 by default). Set by the game from its admin settings.
	public static int HoursBetweenChallenges
	{
		get { return _hoursBetweenChallenges; }
		set { _hoursBetweenChallenges = Math.Max(MinHoursBetweenChallenges, Math.Min(MaxHoursBetweenChallenges, value)); }
	}

	// BTE challenges the player: now and then (at most every few hours, and never while an earlier challenge of BTE is still open) it
	// sends a friend challenge on a level the player has already finished, with a time close to the player's best.
	// The player sees it as a bubble on the map and accepts or declines it, like a challenge from a real friend.
	public static void MaybeChallenge(string playerId)
	{
		if (string.IsNullOrEmpty(playerId) || playerId == Id)
		{
			return;
		}
		if (Db.One("SELECT id FROM challenges WHERE kind='friendly' AND p1=$0 AND challenged=$1 AND claimed2=0 AND status<>'declined'", Id, playerId) != null)
		{
			return;
		}
		if (Db.One("SELECT id FROM challenges WHERE kind='friendly' AND p1=$0 AND challenged=$1 AND createdAt>datetime('now', $2)", Id, playerId, "-" + _hoursBetweenChallenges + " hours") != null)
		{
			return;
		}
		// a level the player has finished (BTE needs a realistic time and a ghost to run against)
		Row? score = Db.One("SELECT s.gameId, s.time, s.stars, s.ghost FROM scores s JOIN minigames m ON m.id=s.gameId WHERE s.playerId=$0 AND s.time>0 AND m.state='public' AND m.gameMode='Race' AND m.levelData IS NOT NULL ORDER BY RANDOM() LIMIT 1", playerId);
		if (score == null)
		{
			return;
		}
		string id = PlayerApi.NewId();
		long time = Math.Max(1000L, (long)(score.L("time", 0L) * 1.03));
		Db.Exec("INSERT INTO challenges (id, kind, gameId, p1, challenged, status, currentPlayer, t1, stars1, tries1, ghost1) VALUES ($0,'friendly',$1,$2,$3,'pending','',$4,$5,1,$6)", id, score.S("gameId"), Id, playerId, time, Math.Max(1L, score.L("stars", 1L)), score.B("ghost"));
		Console.WriteLine($"[Bot] {Name} challenges {PlayerApi.GetPlayer(playerId)?.S("name")} on {score.S("gameId")} (time to beat {time} ms)");
	}

	// Called after a time was saved on a friend race. If the challenged player is BTE and it has not answered yet, it plays now.
	public static void RespondIfChallenged(Row? ch)
	{
		if (ch == null || ch.S("kind") != "friendly" || ch.S("challenged") != Id || (ch.S("p2") != "" && ch.S("p2") != Id) || ch.L("t2", 0L) > 0)
		{
			return;
		}
		if (ch.S("status") != "pending" && ch.S("status") != "active")
		{
			return;
		}
		long t1 = ch.L("t1", 0L);
		if (t1 <= 0)
		{
			return;
		}
		// BTE's time: between 92 % and 112 % of the challenger's (it wins about four races out of ten), always the same for a given challenge
		int seed = 0;
		foreach (char x in ch.S("id"))
		{
			seed = (seed * 31 + x) & 0x7FFFFFFF;
		}
		double factor = 0.92 + (seed % 21) / 100.0;
		long t2 = Math.Max(1000L, (long)(t1 * factor));
		long stars2 = Math.Max(1L, ch.L("stars1", 1L));
		string challenger = ch.S("p1");
		string winner = t2 < t1 ? Id : challenger;
		// BTE's ghost: the best recorded run of the level, else the level creator's, else the challenger's own
		byte[]? ghost = null;
		Row? score = Db.One("SELECT ghost FROM scores WHERE gameId=$0 AND time>0 AND playerId<>$1 ORDER BY time LIMIT 1", ch.S("gameId"), challenger);
		ghost = score?.B("ghost");
		if (ghost == null || ghost.Length < 2)
		{
			ghost = MinigameApi.GetMinigame(ch.S("gameId"))?.B("creatorGhost");
		}
		if (ghost == null || ghost.Length < 2)
		{
			ghost = ch.B("ghost1");
		}
		Db.Exec("UPDATE challenges SET p2=$0, status='finished', winner=$1, currentPlayer='', t2=$2, stars2=$3, tries2=1, ghost2=$4, claimed2=1, updatedAt=CURRENT_TIMESTAMP WHERE id=$5", Id, winner, t2, stars2, ghost, ch.S("id"));
		Console.WriteLine($"[Bot] {Name} answers challenge {ch.S("id")}: {t2} ms against {t1} ms -> {(winner == Id ? Name : "the challenger")} wins");
	}
}
