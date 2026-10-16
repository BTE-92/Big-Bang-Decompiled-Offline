using System;
using AdMediation;
using UnityEngine.Advertisements;

public static class UnityAdsManager
{
	public static void Initialize(string _gameId, bool _testMode)
	{
		if (Advertisement.isSupported)
		{
			Advertisement.Initialize(_gameId, _testMode);
		}
		else
		{
			Debug.Log("Platform not supported for UnityAds");
		}
	}

	public static bool isReadyToShowAd()
	{
		if (!Advertisement.isSupported)
		{
			return false;
		}
		return Advertisement.IsReady();
	}

	public static void ShowAd(string _zoneId = null)
	{
		Advertisement.Show(_zoneId, new ShowOptions
		{
			resultCallback = delegate(ShowResult result)
			{
				Debug.Log(result.ToString());
			}
		});
	}

	public static void ShowAd(Action<AdResult> _callback, string _zoneId = null)
	{
		Advertisement.Show(_zoneId, new ShowOptions
		{
			resultCallback = delegate(ShowResult result)
			{
				_callback((AdResult)(int)Enum.Parse(typeof(AdResult), result.ToString()));
			}
		});
	}
}
