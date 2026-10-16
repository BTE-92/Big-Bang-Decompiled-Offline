using System;
using System.Collections.Generic;
using OmniataSDK;
using Prime31;
using UnityEngine;
using com.adjust.sdk;

public static class Metrics
{
	private const string ORG = "traplight";

	private static bool DEBUG = true;

	private static string UID;

	public static void Initialize(string _omniataApiKey, string _flurryIosApiKey, string _flurryAndroidApiKey, string _adjustToken)
	{
		DEBUG = false;
		UID = GetMetricsGUID();
		AdjustConfig adjustConfig = new AdjustConfig(_adjustToken, AdjustEnvironment.Production);
		if (DEBUG)
		{
			Debug.Log("Adjust Initialize");
		}
		Adjust.start(adjustConfig);
		if (DEBUG)
		{
			Debug.Log("Omniata Initialize");
		}
		Omniata.Instance.appDidLaunch(_omniataApiKey, UID, "traplight", (!DEBUG) ? Omniata.LogLevel.Verbose : Omniata.LogLevel.Debug);
		if (DEBUG)
		{
			Debug.Log("FlurryAnalytics Initialize");
		}
		FlurryAnalytics.startSession(_flurryAndroidApiKey, DEBUG);
		FlurryAnalytics.setLogEnabled(true);
		FlurryAnalytics.setUserID(UID);
	}

	public static void FirstTimeLogin()
	{
		if (DEBUG)
		{
			Debug.Log("Omniata Login");
		}
		Omniata.Instance.TrackOmLoad();
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("om_idfa", IDFAGetter.Get());
		Omniata.Instance.TrackOm("om_alias", dictionary);
		if (DEBUG)
		{
			Debug.Log("Flurry Login");
		}
		FlurryAnalytics.logEvent("Login", new Dictionary<string, string> { 
		{
			"platform",
			Application.platform.ToString()
		} });
	}

	public static void Login()
	{
		if (DEBUG)
		{
			Debug.Log("Omniata Login");
		}
		Omniata.Instance.TrackOmLoad();
		if (DEBUG)
		{
			Debug.Log("Flurry Login");
		}
		FlurryAnalytics.logEvent("Login", new Dictionary<string, string> { 
		{
			"platform",
			Application.platform.ToString()
		} });
	}

	public static void SendEvent(string _eventName, Dictionary<string, string> _parameters)
	{
		if (DEBUG)
		{
			Debug.Log("Omniata " + _eventName);
		}
		Omniata.Instance.TrackOm(_eventName, _parameters);
		if (DEBUG)
		{
			Debug.Log("Flurry " + _eventName);
		}
		FlurryAnalytics.logEvent(_eventName, _parameters);
		if (_eventName == "Tutorial")
		{
			AdjustEvent adjustEvent = new AdjustEvent("pfacie");
			Adjust.trackEvent(adjustEvent);
		}
	}

	public static void SendRevenueEvent(string _total, string _currency, string _transactionId)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("total", _total);
		dictionary.Add("currency_code", _currency);
		SendEvent("om_revenue", dictionary);
		FlurryAnalytics.logEvent("revenue", dictionary);
		AdjustEvent adjustEvent = new AdjustEvent("vz7n34");
		adjustEvent.setRevenue(Convert.ToDouble(_total), _currency);
		adjustEvent.setTransactionId(_transactionId);
		Adjust.trackEvent(adjustEvent);
	}

	public static string GetMetricsGUID()
	{
		string text = KeyChain.GetMetrcisGUID();
		if (string.IsNullOrEmpty(text))
		{
			text = PlayerPrefsX.GetMetricsGUID();
			if (string.IsNullOrEmpty(text))
			{
				text = Guid.NewGuid().ToString();
				PlayerPrefsX.SetFirstLogin(true);
				PlayerPrefsX.SetMetricsGUID(text);
				KeyChain.SetMetricsGUID(text);
			}
		}
		return text;
	}
}
