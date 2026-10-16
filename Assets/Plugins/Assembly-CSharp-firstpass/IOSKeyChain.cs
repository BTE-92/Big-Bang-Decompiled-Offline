using UnityEngine;

public class IOSKeyChain
{
	public static bool SetData(string service, string account, string password)
	{
		Debug.Log("IOSKeyChain not available on selected platform!");
		return false;
	}

	public static string GetData(string service, string account)
	{
		Debug.Log("IOSKeyChain not available on selected platform!");
		return string.Empty;
	}
}
