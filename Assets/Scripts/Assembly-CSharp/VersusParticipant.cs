using System.Collections;

public class VersusParticipant
{
	public int rank;

	public HighscoreDataEntry score;

	public int stars;

	public int tries;

	public Hashtable GenerateParticipantHashTable()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("score", HighScores.GenerateDataEntryHashtable(score));
		hashtable.Add("rank", rank);
		hashtable.Add("stars", stars);
		hashtable.Add("tries", tries);
		return hashtable;
	}
}
