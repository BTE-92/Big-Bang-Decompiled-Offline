using System.Collections.Generic;
using System.Linq;

public class PsTimedEvents
{
	public PsTournaments tournaments;

	public List<PsChallengeMetaData> diamondChallenges;

	public List<VersusMetaData> versusChallenges;

	public List<VersusMetaData> friendlyChallenges;

	public List<PsMinigameMetaData> freshAndFree;

	public PsTimedEvents()
	{
		tournaments = new PsTournaments();
		diamondChallenges = new List<PsChallengeMetaData>();
		versusChallenges = new List<VersusMetaData>();
		friendlyChallenges = new List<VersusMetaData>();
		freshAndFree = new List<PsMinigameMetaData>();
	}

	public bool HasTournaments()
	{
		if (tournaments != null)
		{
			if (tournaments.dailyTournaments != null && tournaments.dailyTournaments.Count > 0)
			{
				return true;
			}
			if (tournaments.activeTournaments != null && tournaments.activeTournaments.Count > 0)
			{
				return true;
			}
			if (tournaments.claimableTournaments != null && tournaments.claimableTournaments.Count > 0)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool HasDiamondChallenge()
	{
		if (diamondChallenges != null && diamondChallenges.Count() > 0)
		{
			return true;
		}
		return false;
	}

	public bool HasVersusChallenge()
	{
		if (versusChallenges != null && versusChallenges.Count() > 0)
		{
			return true;
		}
		return false;
	}

	public bool HasFreshAndFree()
	{
		if (freshAndFree != null && freshAndFree.Count() > 0)
		{
			return true;
		}
		return false;
	}
}
