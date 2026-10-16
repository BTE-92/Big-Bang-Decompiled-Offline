using System;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

public class GCMReceiver : MonoBehaviour
{
	public static Action<string> _onError;

	public static Action<Dictionary<string, object>> _onMessage;

	public static Action<string> _onRegistered;

	public static Action<string> _onUnregistered;

	public static Action<int> _onDeleteMessages;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
	}

	private void OnError(string errorId)
	{
		Debug.Log("Error: " + errorId);
		if (_onError != null)
		{
			_onError(errorId);
		}
	}

	private void OnMessage(string message)
	{
		Debug.Log("Message: " + message);
		if (_onMessage != null)
		{
			Dictionary<string, object> obj = Json.Deserialize(message) as Dictionary<string, object>;
			_onMessage(obj);
		}
	}

	private void OnRegistered(string registrationId)
	{
		Debug.Log("Registered: " + registrationId);
		if (_onRegistered != null)
		{
			_onRegistered(registrationId);
		}
	}

	private void OnUnregistered(string registrationId)
	{
		Debug.Log("Unregistered: " + registrationId);
		if (_onUnregistered != null)
		{
			_onUnregistered(registrationId);
		}
	}

	private void OnDeleteMessages(string total)
	{
		Debug.Log("DeleteMessages: " + total);
		if (_onDeleteMessages != null)
		{
			int obj = Convert.ToInt32(total);
			_onDeleteMessages(obj);
		}
	}
}
