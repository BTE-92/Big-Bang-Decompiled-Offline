using System;
using System.Collections.Generic;
using AdMediation;
using Server;

public static class PsAdMediation
{
	private static bool m_initialized;

	public static void Initialize()
	{
		Manager.Initialize(null);
		m_initialized = true;
	}

	public static void Initialize(List<object> _config)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		NetworkConfig networkConfig = default(NetworkConfig);
		foreach (Dictionary<string, object> item in _config)
		{
			networkConfig.priority = Convert.ToInt32(item["priority"]);
			networkConfig.enabled = true;
			dictionary.Add((string)item["networkName"], networkConfig);
		}
		Manager.Initialize(dictionary);
		m_initialized = true;
	}

	public static void Initialize(Dictionary<string, object> _config)
	{
		Manager.Initialize(_config);
		m_initialized = true;
	}

	public static void ShowAd(string _location = null)
	{
		if (m_initialized)
		{
			SoundS.PauseMixer(true);
			Action<AdResult> callback = delegate
			{
				SoundS.PauseMixer(false);
			};
			PsState.m_adWatched = true;
			Manager.ShowAd(callback, _location);
		}
		else
		{
			Debug.Log("Ad mediator is not initialized yet!");
		}
	}

	public static void ShowAd(Action<AdResult> _callback, string _location = null)
	{
		if (m_initialized)
		{
			SoundS.PauseMixer(true);
			_callback = ((_callback != null) ? ((Action<AdResult>)Delegate.Combine(_callback, (Action<AdResult>)delegate
			{
				SoundS.PauseMixer(false);
			})) : ((Action<AdResult>)delegate
			{
				SoundS.PauseMixer(false);
			}));
			PsState.m_adWatched = true;
			Manager.ShowAd(_callback, _location);
		}
		else
		{
			Debug.Log("Ad mediator is not initialized yet!");
		}
	}

	public static bool adsAvailable()
	{
		if (m_initialized)
		{
			return Manager.adsAvailable();
		}
		Debug.Log("Ad mediator is not initialized yet!");
		return false;
	}

	private static void GetConfigAndInitialize()
	{
		Ads.GetAdNetworkConfig(GetConfigOkAndInit, GetConfigFailedAndInit);
	}

	public static void UseServerConfig()
	{
		Ads.GetAdNetworkConfig(GetConfigOk, GetConfigFailed);
	}

	private static void GetConfigOk(HttpC _request)
	{
		Dictionary<string, object> config = ClientTools.ParseServerResponse(_request.www.text);
		Manager.LoadConfig(config);
		m_initialized = true;
	}

	private static void GetConfigOkAndInit(HttpC _request)
	{
		Dictionary<string, object> config = ClientTools.ParseServerResponse(_request.www.text);
		Manager.Initialize(config);
		m_initialized = true;
	}

	private static void GetConfigFailedAndInit(HttpC _request)
	{
		Ads.GetAdNetworkConfig(GetConfigOkAndInit, GetConfigFailedAndInit);
	}

	private static void GetConfigFailed(HttpC _request)
	{
		Ads.GetAdNetworkConfig(GetConfigOk, GetConfigFailed);
	}
}
