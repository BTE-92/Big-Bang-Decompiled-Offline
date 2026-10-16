using System;
using UnityEngine;
using com.adjust.sdk;

public class ExampleGUI : MonoBehaviour
{
	private int nr_buttons = 8;

	private static bool isEnabled;

	private bool showPopUp;

	private string txtManualLaunch = "Manual Launch";

	private string txtSetOfflineMode = "Turn Offline Mode ON";

	private string txtSetEnabled = "Disable SDK";

	private void OnGUI()
	{
		if (showPopUp)
		{
			GUI.Window(0, new Rect(Screen.width / 2 - 150, Screen.height / 2 - 65, 300f, 130f), showGUI, "Is SDK enabled?");
		}
		if (GUI.Button(new Rect(0f, Screen.height * 0 / nr_buttons, Screen.width, Screen.height / nr_buttons), txtManualLaunch) && !string.Equals(txtManualLaunch, "SDK Launched", StringComparison.OrdinalIgnoreCase))
		{
			AdjustConfig adjustConfig = new AdjustConfig("{YourAppToken}", AdjustEnvironment.Sandbox);
			adjustConfig.setLogLevel(AdjustLogLevel.Verbose);
			adjustConfig.setAttributionChangedDelegate(attributionChangedDelegate);
			Adjust.start(adjustConfig);
			isEnabled = true;
			txtManualLaunch = "SDK Launched";
		}
		if (GUI.Button(new Rect(0f, Screen.height * 1 / nr_buttons, Screen.width, Screen.height / nr_buttons), "Track Simple Event"))
		{
			AdjustEvent adjustEvent = new AdjustEvent("{YourEventToken}");
			Adjust.trackEvent(adjustEvent);
		}
		if (GUI.Button(new Rect(0f, Screen.height * 2 / nr_buttons, Screen.width, Screen.height / nr_buttons), "Track Revenue Event"))
		{
			AdjustEvent adjustEvent2 = new AdjustEvent("{YourEventToken}");
			adjustEvent2.setRevenue(0.25, "EUR");
			Adjust.trackEvent(adjustEvent2);
		}
		if (GUI.Button(new Rect(0f, Screen.height * 3 / nr_buttons, Screen.width, Screen.height / nr_buttons), "Track Callback Event"))
		{
			AdjustEvent adjustEvent3 = new AdjustEvent("{YourEventToken}");
			adjustEvent3.addCallbackParameter("key", "value");
			adjustEvent3.addCallbackParameter("foo", "bar");
			Adjust.trackEvent(adjustEvent3);
		}
		if (GUI.Button(new Rect(0f, Screen.height * 4 / nr_buttons, Screen.width, Screen.height / nr_buttons), "Track Partner Event"))
		{
			AdjustEvent adjustEvent4 = new AdjustEvent("{YourEventToken}");
			adjustEvent4.addPartnerParameter("key", "value");
			adjustEvent4.addPartnerParameter("foo", "bar");
			Adjust.trackEvent(adjustEvent4);
		}
		if (GUI.Button(new Rect(0f, Screen.height * 5 / nr_buttons, Screen.width, Screen.height / nr_buttons), txtSetOfflineMode))
		{
			if (string.Equals(txtSetOfflineMode, "Turn Offline Mode ON", StringComparison.OrdinalIgnoreCase))
			{
				Adjust.setOfflineMode(true);
				txtSetOfflineMode = "Turn Offline Mode OFF";
			}
			else
			{
				Adjust.setOfflineMode(false);
				txtSetOfflineMode = "Turn Offline Mode ON";
			}
		}
		if (GUI.Button(new Rect(0f, Screen.height * 6 / nr_buttons, Screen.width, Screen.height / nr_buttons), txtSetEnabled))
		{
			if (string.Equals(txtSetEnabled, "Disable SDK", StringComparison.OrdinalIgnoreCase))
			{
				Adjust.setEnabled(false);
				txtSetEnabled = "Enable SDK";
			}
			else
			{
				Adjust.setEnabled(true);
				txtSetEnabled = "Disable SDK";
			}
		}
		if (GUI.Button(new Rect(0f, Screen.height * 7 / nr_buttons, Screen.width, Screen.height / nr_buttons), "Is SDK Enabled?"))
		{
			isEnabled = Adjust.isEnabled();
			showPopUp = true;
		}
	}

	private void showGUI(int windowID)
	{
		if (isEnabled)
		{
			GUI.Label(new Rect(65f, 40f, 200f, 30f), "Adjust SDK is ENABLED!");
		}
		else
		{
			GUI.Label(new Rect(65f, 40f, 200f, 30f), "Adjust SDK is DISABLED!");
		}
		if (GUI.Button(new Rect(90f, 75f, 120f, 40f), "OK"))
		{
			showPopUp = false;
		}
	}

	public void attributionChangedDelegate(AdjustAttribution attribution)
	{
		Debug.Log("Attribution changed");
		if (attribution.trackerName != null)
		{
			Debug.Log("trackerName " + attribution.trackerName);
		}
		if (attribution.trackerToken != null)
		{
			Debug.Log("trackerToken " + attribution.trackerToken);
		}
		if (attribution.network != null)
		{
			Debug.Log("network " + attribution.network);
		}
		if (attribution.campaign != null)
		{
			Debug.Log("campaign " + attribution.campaign);
		}
		if (attribution.adgroup != null)
		{
			Debug.Log("adgroup " + attribution.adgroup);
		}
		if (attribution.creative != null)
		{
			Debug.Log("creative " + attribution.creative);
		}
	}
}
