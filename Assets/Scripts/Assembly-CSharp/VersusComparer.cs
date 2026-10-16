using System.Collections.Generic;

public class VersusComparer : IComparer<VersusMetaData>
{
	public int Compare(VersusMetaData d1, VersusMetaData d2)
	{
		if (d1.currentPlayer != PlayerPrefsX.GetUserId() && d1.GetOpponentInfo() == null)
		{
			if (d2.currentPlayer != PlayerPrefsX.GetUserId() && d2.GetOpponentInfo() == null)
			{
				return 0;
			}
			return 1;
		}
		if (d2.currentPlayer != PlayerPrefsX.GetUserId() && d2.GetOpponentInfo() == null)
		{
			return -1;
		}
		if (d1.currentPlayer == PlayerPrefsX.GetUserId())
		{
			if (d2.currentPlayer == PlayerPrefsX.GetUserId())
			{
				if (d1.timeLeft < d2.timeLeft)
				{
					return -1;
				}
				return 1;
			}
			return -1;
		}
		if (d2.currentPlayer == PlayerPrefsX.GetUserId())
		{
			return 1;
		}
		return 0;
	}
}
