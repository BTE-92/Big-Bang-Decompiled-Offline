using System;
using AdMediation;
using ChartboostSDK;

public static class ChartboostManager
{
	public static void Initialize()
	{
		Chartboost.setAutoCacheAds(true);
		isReadyToShowAd();
	}

	public static void SetupDelegates(Action<AdResult> _callback)
	{
		Chartboost.didFailToLoadRewardedVideo += delegate(CBLocation arg1, CBImpressionError arg2)
		{
			didFailToLoadRewardedVideo(arg1, arg2, _callback);
		};
		Chartboost.didDismissRewardedVideo += delegate(CBLocation arg1)
		{
			didDismissRewardedVideo(arg1, _callback);
		};
		Chartboost.didCloseRewardedVideo += delegate(CBLocation arg1)
		{
			didCloseRewardedVideo(arg1, _callback);
		};
		Chartboost.didClickRewardedVideo += didClickRewardedVideo;
		Chartboost.didCacheRewardedVideo += didCacheRewardedVideo;
		Chartboost.shouldDisplayRewardedVideo += shouldDisplayRewardedVideo;
		Chartboost.didCompleteRewardedVideo += delegate(CBLocation arg1, int arg2)
		{
			didCompleteRewardedVideo(arg1, arg2, _callback);
		};
		Chartboost.didDisplayRewardedVideo += didDisplayRewardedVideo;
	}

	public static void RemoveDelegates(Action<AdResult> _callback)
	{
		Chartboost.didFailToLoadRewardedVideo -= delegate(CBLocation arg1, CBImpressionError arg2)
		{
			didFailToLoadRewardedVideo(arg1, arg2, _callback);
		};
		Chartboost.didDismissRewardedVideo -= delegate(CBLocation arg1)
		{
			didDismissRewardedVideo(arg1, _callback);
		};
		Chartboost.didCloseRewardedVideo -= delegate(CBLocation arg1)
		{
			didCloseRewardedVideo(arg1, _callback);
		};
		Chartboost.didClickRewardedVideo -= didClickRewardedVideo;
		Chartboost.didCacheRewardedVideo -= didCacheRewardedVideo;
		Chartboost.shouldDisplayRewardedVideo -= shouldDisplayRewardedVideo;
		Chartboost.didCompleteRewardedVideo -= delegate(CBLocation arg1, int arg2)
		{
			didCompleteRewardedVideo(arg1, arg2, _callback);
		};
		Chartboost.didDisplayRewardedVideo -= didDisplayRewardedVideo;
	}

	public static bool isReadyToShowAd()
	{
		bool flag = Chartboost.hasRewardedVideo(CBLocation.Default);
		if (!flag)
		{
			Chartboost.cacheRewardedVideo(CBLocation.Default);
		}
		return flag;
	}

	public static void ShowAd(string _zoneId = null)
	{
		Chartboost.showRewardedVideo(CBLocation.Default);
	}

	public static void ShowAd(Action<AdResult> _callback, string _zoneId = null)
	{
		SetupDelegates(_callback);
		Chartboost.showRewardedVideo(CBLocation.Default);
	}

	private static void didFailToLoadRewardedVideo(CBLocation location, CBImpressionError error, Action<AdResult> _callback)
	{
		AddLog(string.Format("didFailToLoadRewardedVideo: {0} at location {1}", error, location));
		_callback(AdResult.Failed);
		Chartboost.cacheRewardedVideo(CBLocation.Default);
		RemoveDelegates(_callback);
	}

	private static void didDismissRewardedVideo(CBLocation location, Action<AdResult> _callback)
	{
		AddLog("didDismissRewardedVideo: " + location);
		_callback(AdResult.Skipped);
		Chartboost.cacheRewardedVideo(CBLocation.Default);
		RemoveDelegates(_callback);
	}

	private static void didCloseRewardedVideo(CBLocation location, Action<AdResult> _callback)
	{
		AddLog("didCloseRewardedVideo: " + location);
		_callback(AdResult.Skipped);
		Chartboost.cacheRewardedVideo(CBLocation.Default);
		RemoveDelegates(_callback);
	}

	private static void didClickRewardedVideo(CBLocation location)
	{
		AddLog("didClickRewardedVideo: " + location);
	}

	private static void didCacheRewardedVideo(CBLocation location)
	{
		AddLog("didCacheRewardedVideo: " + location);
	}

	private static bool shouldDisplayRewardedVideo(CBLocation location)
	{
		AddLog("shouldDisplayRewardedVideo @" + location);
		return true;
	}

	private static void didCompleteRewardedVideo(CBLocation location, int reward, Action<AdResult> _callback)
	{
		AddLog(string.Format("didCompleteRewardedVideo: reward {0} at location {1}", reward, location));
		_callback(AdResult.Finished);
		RemoveDelegates(_callback);
	}

	private static void didDisplayRewardedVideo(CBLocation location)
	{
		AddLog("didDisplayRewardedVideo: " + location);
	}

	private static void AddLog(string text)
	{
	}
}
