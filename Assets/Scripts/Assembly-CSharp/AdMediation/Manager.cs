using System;
using System.Collections.Generic;
using System.Globalization;

namespace AdMediation
{
	public static class Manager
	{
		private static List<AdNetwork> m_adNetworks;

		public static void Initialize(Dictionary<string, object> _config)
		{
			if (m_adNetworks == null)
			{
				m_adNetworks = new List<AdNetwork>();
			}
			else
			{
				m_adNetworks.Clear();
			}
			AdNetwork item = new AdNetwork("UnityAds", 1, delegate
			{
				UnityAdsManager.Initialize("96752", false);
			}, UnityAdsManager.isReadyToShowAd, delegate(string zone, Action<AdResult> callback)
			{
				UnityAdsManager.ShowAd(callback, zone);
			}, true);
			m_adNetworks.Add(item);
			AdNetwork item2 = new AdNetwork("ChartboostVideo", 2, delegate
			{
				ChartboostManager.Initialize();
			}, ChartboostManager.isReadyToShowAd, delegate(string zone, Action<AdResult> callback)
			{
				ChartboostManager.ShowAd(callback, zone);
			}, true);
			m_adNetworks.Add(item2);
			LoadConfig(_config);
		}

		public static bool adsAvailable()
		{
			if (GetVideoAdCount() > 0 || GetVideoAdTimeOutSeconds() < 0)
			{
				foreach (AdNetwork adNetwork in m_adNetworks)
				{
					if (adNetwork.adsAvailable())
					{
						return true;
					}
				}
			}
			return false;
		}

		public static void LoadConfig(Dictionary<string, object> _config)
		{
			if (_config != null)
			{
				foreach (AdNetwork adNetwork in m_adNetworks)
				{
					object value = null;
					if (_config.TryGetValue(adNetwork.networkName, out value))
					{
						adNetwork.SetConfig((NetworkConfig)value);
					}
				}
			}
			InitializeAdNetworks();
		}

		private static void InitializeAdNetworks()
		{
			if (m_adNetworks == null)
			{
				return;
			}
			foreach (AdNetwork adNetwork in m_adNetworks)
			{
				Debug.Log("Initialize " + adNetwork.networkName);
				adNetwork.initialize();
			}
		}

		public static void ShowAd(Action<AdResult> _callback = null, string _zone = null)
		{
			m_adNetworks.Sort((AdNetwork n1, AdNetwork n2) => n1.priority.CompareTo(n2.priority));
			foreach (AdNetwork adNetwork in m_adNetworks)
			{
				Debug.Log("Checking for ads in " + adNetwork.networkName);
				if (adNetwork.adsAvailable())
				{
					adNetwork.showAd(_zone, _callback);
					VideoAdShown();
					break;
				}
			}
		}

		public static void ShowAd(Action<AdResult> _callback, string _zone, string _adNetwork)
		{
			m_adNetworks.Find((AdNetwork n) => n.networkName == _adNetwork).showAd(_zone, _callback);
		}

		private static int GetVideoAdCount()
		{
			int num = PlayerPrefsX.GetVideoAdCount();
			if (num == -1)
			{
				num = PlayerPrefsX.GetVideoAdStartCount();
				SetVideoAdCount(num);
			}
			return num;
		}

		private static void SetVideoAdCount(int _count)
		{
			PlayerPrefsX.SetVideoAdCount(_count);
		}

		private static void VideoAdShown()
		{
			int videoAdCount = GetVideoAdCount();
			if (GetVideoAdTimeOutSeconds() < 0)
			{
				SetVideoAdCount(PlayerPrefsX.GetVideoAdStartCount() - 1);
			}
			else if (videoAdCount > 0)
			{
				SetVideoAdCount(videoAdCount - 1);
			}
			SetVideoAdTimeOut();
		}

		private static void SetVideoAdTimeOut()
		{
			int videoAdStartTimeOut = PlayerPrefsX.GetVideoAdStartTimeOut();
			PlayerPrefsX.SetVideoAdTimeOut(DateTime.Now.AddSeconds(videoAdStartTimeOut).ToString("O"));
		}

		private static int GetVideoAdTimeOutSeconds()
		{
			string videoAdTimeOut = PlayerPrefsX.GetVideoAdTimeOut();
			int result;
			if (videoAdTimeOut != null)
			{
				result = Convert.ToInt32(DateTime.ParseExact(videoAdTimeOut, "O", CultureInfo.InvariantCulture).Subtract(DateTime.Now).TotalSeconds);
			}
			else
			{
				result = PlayerPrefsX.GetVideoAdStartTimeOut();
				SetVideoAdTimeOut();
			}
			return result;
		}
	}
}
