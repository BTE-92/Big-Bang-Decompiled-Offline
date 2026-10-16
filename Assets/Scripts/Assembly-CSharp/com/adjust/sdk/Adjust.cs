using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.adjust.sdk
{
	public class Adjust : MonoBehaviour
	{
		private const string errorMessage = "adjust: SDK not started. Start it manually using the 'start' method.";

		private static IAdjust instance;

		private static Action<AdjustAttribution> attributionChangedDelegate;

		public bool startManually = true;

		public bool eventBuffering;

		public bool printAttribution = true;

		public string appToken = "{Your App Token}";

		public AdjustLogLevel logLevel = AdjustLogLevel.Info;

		public AdjustEnvironment environment;

		private void Awake()
		{
			if (instance != null)
			{
				return;
			}
			UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
			if (!startManually)
			{
				AdjustConfig adjustConfig = new AdjustConfig(appToken, environment);
				adjustConfig.setLogLevel(logLevel);
				adjustConfig.setEventBufferingEnabled(eventBuffering);
				if (printAttribution)
				{
					adjustConfig.setAttributionChangedDelegate(responseDelegate);
				}
				start(adjustConfig);
			}
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (instance != null)
			{
				if (pauseStatus)
				{
					instance.onPause();
				}
				else
				{
					instance.onResume();
				}
			}
		}

		public static void start(AdjustConfig adjustConfig)
		{
			if (instance != null)
			{
				Debug.Log("adjust: Error, SDK already started.");
				return;
			}
			if (adjustConfig == null)
			{
				Debug.Log("adjust: Missing config to start.");
				return;
			}
			instance = new AdjustAndroid();
			if (instance == null)
			{
				Debug.Log("adjust: SDK can only be used in Android, iOS, Windows Phone 8 or Windows Store apps.");
				return;
			}
			attributionChangedDelegate = adjustConfig.getAttributionChangedDelegate();
			instance.start(adjustConfig);
		}

		public static void trackEvent(AdjustEvent adjustEvent)
		{
			if (instance == null)
			{
				Debug.Log("adjust: SDK not started. Start it manually using the 'start' method.");
			}
			else if (adjustEvent == null)
			{
				Debug.Log("adjust: Missing event to track.");
			}
			else
			{
				instance.trackEvent(adjustEvent);
			}
		}

		public static void setEnabled(bool enabled)
		{
			if (instance == null)
			{
				Debug.Log("adjust: SDK not started. Start it manually using the 'start' method.");
			}
			else
			{
				instance.setEnabled(enabled);
			}
		}

		public static bool isEnabled()
		{
			if (instance == null)
			{
				Debug.Log("adjust: SDK not started. Start it manually using the 'start' method.");
				return false;
			}
			return instance.isEnabled();
		}

		public static void setOfflineMode(bool enabled)
		{
			if (instance == null)
			{
				Debug.Log("adjust: SDK not started. Start it manually using the 'start' method.");
			}
			else
			{
				instance.setOfflineMode(enabled);
			}
		}

		public static void setDeviceToken(string deviceToken)
		{
			if (instance == null)
			{
				Debug.Log("adjust: SDK not started. Start it manually using the 'start' method.");
			}
			else
			{
				instance.setDeviceToken(deviceToken);
			}
		}

		public static void setReferrer(string referrer)
		{
			if (instance == null)
			{
				Debug.Log("adjust: SDK not started. Start it manually using the 'start' method.");
			}
			else
			{
				instance.setReferrer(referrer);
			}
		}

		public void getNativeMessage(string sAttributionData)
		{
			runAttributionChangedString(sAttributionData);
		}

		public static void runAttributionChangedString(string stringAttributionData)
		{
			if (instance == null)
			{
				Debug.Log("adjust: SDK not started. Start it manually using the 'start' method.");
				return;
			}
			if (attributionChangedDelegate == null)
			{
				Debug.Log("adjust: Attribution changed delegate was not set.");
				return;
			}
			AdjustAttribution obj = new AdjustAttribution(stringAttributionData);
			attributionChangedDelegate(obj);
		}

		public static void runAttributionChangedDictionary(Dictionary<string, string> dicAttributionData)
		{
			if (instance == null)
			{
				Debug.Log("adjust: SDK not started. Start it manually using the 'start' method.");
				return;
			}
			if (attributionChangedDelegate == null)
			{
				Debug.Log("adjust: Attribution changed delegate was not set.");
				return;
			}
			AdjustAttribution obj = new AdjustAttribution(dicAttributionData);
			attributionChangedDelegate(obj);
		}

		private void responseDelegate(AdjustAttribution responseData)
		{
			Debug.Log("Attribution changed!");
			if (responseData.trackerName != null)
			{
				Debug.Log("trackerName " + responseData.trackerName);
			}
			if (responseData.trackerToken != null)
			{
				Debug.Log("trackerToken " + responseData.trackerToken);
			}
			if (responseData.network != null)
			{
				Debug.Log("network " + responseData.network);
			}
			if (responseData.campaign != null)
			{
				Debug.Log("campaign " + responseData.campaign);
			}
			if (responseData.adgroup != null)
			{
				Debug.Log("adgroup " + responseData.adgroup);
			}
			if (responseData.creative != null)
			{
				Debug.Log("creative " + responseData.creative);
			}
			if (responseData.clickLabel != null)
			{
				Debug.Log("clickLabel" + responseData.clickLabel);
			}
		}
	}
}
