using System;
using Helpshift;
using UnityEngine;

public class HelpshiftManager : MonoBehaviour
{
	private static HelpshiftSdk m_sdk;

	private static Action m_rateSuccessCallback;

	private static Action m_rateFeedbackCallback;

	private static Action m_rateCloceCallback;

	public static void SetUserMetaData(string _userId, string _userName)
	{
		if (m_sdk != null)
		{
			m_sdk.setUserIdentifier(_userId);
			m_sdk.setNameAndEmail(_userName, string.Empty);
		}
	}

	public static void RegisterDeviceToken(string _token)
	{
		if (m_sdk != null)
		{
			m_sdk.registerDeviceToken(_token);
		}
	}

	public static void ShowFAQ()
	{
		if (m_sdk != null)
		{
			m_sdk.showFAQs();
		}
		else
		{
			Debug.LogWarning("HelpShift not initialized");
		}
	}

	public static void ShowAlertToRate(string _appID, Action _successCallback = null, Action _feedbackCallback = null, Action _closeCallback = null)
	{
		if (m_sdk != null)
		{
			m_rateSuccessCallback = _successCallback;
			m_rateFeedbackCallback = _feedbackCallback;
			m_rateCloceCallback = _closeCallback;
			m_sdk.showAlertToRateAppWithURL("market://details?id=" + _appID);
		}
	}

	public void alertToRateAppAction(string message)
	{
		if (message == "HS_RATE_ALERT_SUCCESS" && m_rateSuccessCallback != null)
		{
			m_rateSuccessCallback();
		}
		else if (message == "HS_RATE_ALERT_FEEDBACK" && m_rateFeedbackCallback != null)
		{
			m_rateFeedbackCallback();
		}
		else if (message == "HS_RATE_ALERT_CLOSE" && m_rateCloceCallback != null)
		{
			m_rateCloceCallback();
		}
	}

	private void Start()
	{
		m_sdk = HelpshiftSdk.getInstance();
		m_sdk.install();
	}
}
