using System;
using UnityEngine;

public static class KeyChain
{
	public const string USER_ID = "UserID";

	public const string USER_NAME = "UserName";

	public const string GAMECENTER_ID = "GcID";

	public const string SOUNDBANK_VERSION = "SoundBank";

	public const string METRICS_GUID = "MetricsGuid";

	public const string RATING_STATUS = "RatingStatus";

	private const string SERVICE_NAME = "WhatOnEarth";

	public static void SetMetricsGUID(string _guid)
	{
		SaveString("MetricsGuid", _guid);
	}

	public static string GetMetrcisGUID()
	{
		return GetString("MetricsGuid");
	}

	public static void SetUserId(string _userId)
	{
		SaveString("UserID", _userId);
	}

	public static string GetUserId()
	{
		return GetString("UserID");
	}

	public static string GetUserName()
	{
		return GetString("UserName");
	}

	public static void SetUserName(string _name)
	{
		SaveString("UserName", _name);
	}

	public static void SetGameCenterId(string _GCId)
	{
		SaveString("GcID", _GCId);
	}

	public static string GetGameCenterId()
	{
		return GetString("GcID");
	}

	public static void SetSoundbankVersion(int _version)
	{
		SaveString("SoundBank", _version.ToString());
	}

	public static int GetSoundbankVersion()
	{
		string value = GetString("SoundBank");
		int result = 0;
		if (!string.IsNullOrEmpty(value))
		{
			result = Convert.ToInt32(value);
		}
		return result;
	}

	public static void SetFileVersion(string _name, int _version)
	{
		SaveString(_name, _version.ToString());
	}

	public static int GetFileVersion(string _name)
	{
		string value = GetString(_name);
		int result = 0;
		if (!string.IsNullOrEmpty(value))
		{
			result = Convert.ToInt32(value);
		}
		return result;
	}

	private static void SaveString(string name, string value)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (!IOSKeyChain.SetData("WhatOnEarth", name, value))
			{
				Debug.LogError("Saving string to keychain failed!");
			}
		}
		else
		{
			PlayerPrefs.SetString(name, value);
			PlayerPrefs.Save();
		}
	}

	private static string GetString(string key)
	{
		string empty = string.Empty;
		empty = ((Application.platform != RuntimePlatform.IPhonePlayer) ? PlayerPrefs.GetString(key) : IOSKeyChain.GetData("WhatOnEarth", key));
		if (empty != null && empty.Equals(string.Empty))
		{
			return null;
		}
		return empty;
	}

	public static int GetRatingStatus()
	{
		string value = GetString("RatingStatus");
		int result = 0;
		if (!string.IsNullOrEmpty(value))
		{
			result = Convert.ToInt32(value);
		}
		return result;
	}

	public static void SetRatingStatus(int _status)
	{
		SaveString("RatingStatus", _status.ToString());
	}
}
