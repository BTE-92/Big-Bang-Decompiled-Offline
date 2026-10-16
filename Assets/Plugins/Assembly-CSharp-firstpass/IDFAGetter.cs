using UnityEngine;

public class IDFAGetter
{
	public static string Get()
	{
		Debug.Log("Unable to get identifier for advertising due to unsupported platform.");
		return string.Empty;
	}
}
