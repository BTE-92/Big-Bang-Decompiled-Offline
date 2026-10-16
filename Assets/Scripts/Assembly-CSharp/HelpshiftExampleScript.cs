using System;
using System.Collections.Generic;
using HSMiniJSON;
using Helpshift;
using UnityEngine;
using UnityEngine.UI;

public class HelpshiftExampleScript : MonoBehaviour
{
	private HelpshiftSdk _support;

	public void updateMetaData(string nothing)
	{
		Debug.Log("Update metadata ************************************************************");
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("user-level", "21");
		dictionary.Add("hs-tags", new string[1] { "Tag-1" });
		_support.updateMetaData(dictionary);
	}

	public void helpshiftSessionBegan(string message)
	{
		Debug.Log("Session Began ************************************************************");
	}

	public void helpshiftSessionEnded(string message)
	{
		Debug.Log("Session ended ************************************************************");
	}

	public void alertToRateAppAction(string result)
	{
		Debug.Log("User action on alert :" + result);
	}

	public void didReceiveNotificationCount(string count)
	{
		Debug.Log("Notification async count : " + count);
	}

	public void didReceiveInAppNotificationCount(string count)
	{
		Debug.Log("In-app Notification count : " + count);
	}

	public void newConversationStarted(string message)
	{
	}

	public void userRepliedToConversation(string newMessage)
	{
	}

	public void userCompletedCustomerSatisfactionSurvey(string json)
	{
		Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(json);
		Debug.Log("Customer satisfaction information : " + dictionary);
	}

	private void Start()
	{
		_support = HelpshiftSdk.getInstance();
		_support.install();
		_support.login("identifier", "name", "email");
	}

	public void onShowFAQsClick()
	{
		Debug.Log("Show FAQs clicked !!");
		_support.showFAQs();
	}

	public void onShowConversationClick()
	{
		Debug.Log("Show Conversation clicked !!");
		_support.showConversation();
	}

	public void onShowFAQSectionClick()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("faq_section_id");
		InputField component = gameObject.GetComponent<InputField>();
		try
		{
			Convert.ToInt16(component.text);
			_support.showFAQSection(component.text);
		}
		catch (FormatException ex)
		{
			Debug.Log("Input string is not a sequence of digits : " + ex);
		}
	}

	public void onShowFAQClick()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("faq_id");
		InputField component = gameObject.GetComponent<InputField>();
		try
		{
			Convert.ToInt16(component.text);
			_support.showSingleFAQ(component.text);
		}
		catch (FormatException ex)
		{
			Debug.Log("Input string is not a sequence of digits : " + ex);
		}
	}

	public void onShowReviewReminderClick()
	{
		_support.showAlertToRateAppWithURL("market://details?id=com.RunnerGames.game.YooNinja_Lite");
	}
}
