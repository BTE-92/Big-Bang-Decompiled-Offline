using System.Collections.Generic;

public class Friends
{
	public List<PlayerData> followees;

	public List<PlayerData> followers;

	public List<PlayerData> friends;

	public bool HasContacts()
	{
		if ((friends != null && friends.Count > 0) || (followees != null && followees.Count > 0) || (followers != null && followers.Count > 0))
		{
			return true;
		}
		return false;
	}
}
