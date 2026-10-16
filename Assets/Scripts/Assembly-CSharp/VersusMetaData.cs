using System.Linq;

public class VersusMetaData
{
	public string timedEventId;

	public string gameId;

	public int timeLeft;

	public string currentPlayer;

	public string winner;

	public int round;

	public double timeLeftSetTime;

	public VersusParticipant[] participants;

	public PsMinigameMetaData gameMetaData;

	public bool matchBall;

	public string challengedPlayer;

	public string status;

	public VersusParticipant GetOwnInfo()
	{
		if (participants != null)
		{
			for (int i = 0; i < participants.Count(); i++)
			{
				if (participants[i].score != null && participants[i].score.playerId == PlayerPrefsX.GetUserId())
				{
					return participants[i];
				}
			}
		}
		return null;
	}

	public VersusParticipant GetOpponentInfo()
	{
		if (participants != null)
		{
			for (int i = 0; i < participants.Count(); i++)
			{
				if (participants[i].score != null && participants[i].score.playerId != PlayerPrefsX.GetUserId())
				{
					return participants[i];
				}
			}
		}
		return null;
	}
}
