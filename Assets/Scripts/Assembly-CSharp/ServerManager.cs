using System;
using System.Collections.Generic;
using Server;
using UnityEngine;

public static class ServerManager
{
	public class PsRetryCallBack
	{
		public Func<HttpC> m_httpcCallBack;

		public Action m_actionCallBack;

		public PsRetryCallBack(Func<HttpC> _httpcCallBack = null, Action _actionCallBack = null)
		{
			m_httpcCallBack = _httpcCallBack;
			m_actionCallBack = _actionCallBack;
		}
	}

	public static bool m_hasAlertViewOnScreen = false;

	public static bool m_playerAuthenticated = false;

	public static bool m_sendDeviceToken = false;

	private static Action m_buttonPressedAction;

	public static PsUIBasePopup m_connectingPopup;

	private static PsUIBasePopup m_errorPopup;

	public static bool m_dontShowLoginPopup = false;

	private static PlayerData[] m_mergePlayersData;

	private static bool touchesWereDisabled = false;

	private static List<PsRetryCallBack> m_retryCallbacks = new List<PsRetryCallBack>();

	public static List<HttpC> m_httpcErrorList = new List<HttpC>();

	public static HttpC Login()
	{
		if (!PsState.m_inLoginFlow && !PsState.m_inIapFlow)
		{
			PsState.m_inLoginFlow = true;
			PsState.m_sessionId = PlayerPrefsX.GetSession();
			m_playerAuthenticated = false;
			AddConnectingPopup();
			Debug.LogWarning("LOGIN");
			return LoginFlow.Start(LoginOk, WoeLoginFailed);
		}
		Debug.LogWarning("Already logging in");
		return null;
	}

	private static void LoginOk(HttpC _c)
	{
		RegisterForNotifications();
		m_sendDeviceToken = true;
		RemoveConnectingPopup();
		m_playerAuthenticated = true;
		PsState.m_inLoginFlow = false;
		PsMetagameManager.InitNewEditorItems();
		if (PsState.m_activeGameLoop != null && PsState.m_activeGameLoop.m_gameMode.m_exitingMinigame && Main.m_currentGame.m_currentScene.m_name == "MenuScene" && Main.m_currentGame.m_currentScene.GetCurrentState() is PsMainMenuState)
		{
			PsState.m_activeGameLoop.BackAtMenu();
		}
	}

	private static void WoeLoginFailed(HttpC _c)
	{
		ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), delegate
		{
			PsState.m_inLoginFlow = false;
			m_playerAuthenticated = false;
			return Login();
		});
	}

	public static void ThrowServerErrorException(string _title, WWW _www, Func<HttpC> _httpcCallBack = null, Action _actionCallBack = null)
	{
		ThrowServerErrorException(_title, ServerErrors.GetNetworkError(_www.error), _httpcCallBack, _actionCallBack);
	}

	public static void ThrowServerErrorException(string _title, string _message, Func<HttpC> _httpcCallBack = null, Action _actionCallBack = null)
	{
		RemoveConnectingPopup();
		PsMetagameManager.m_queueWaitTicks = 5;
		m_retryCallbacks.Add(new PsRetryCallBack(_httpcCallBack, _actionCallBack));
		CreateServerErrorPopup(_title, _message, "Retry", delegate
		{
			ServerErrorButtonClicked(null);
		});
		Debug.LogStackTrace();
	}

	public static void CreateServerErrorPopup(string _title, string _message, string _proceedText, Action _proceedAction)
	{
		Debug.LogWarning(_title + ": " + _message);
		if (!m_hasAlertViewOnScreen)
		{
			HttpS.m_gatesFreeze = true;
			TouchAreaS.CancelAllTouches();
			if (TouchAreaS.m_disabled)
			{
				touchesWereDisabled = true;
				TouchAreaS.Enable();
			}
			if (PsState.m_canAutoPause)
			{
				PsState.m_activeGameLoop.PauseMinigame();
				Main.m_currentGame.m_currentScene.m_stateMachine.Update();
				Debug.Log("Game autopaused.");
			}
			PsUIBasePopup psUIBasePopup = new PsUIBasePopup(typeof(PsUIErrorPopup), null, null, null, false);
			psUIBasePopup.SetAction("Proceed", _proceedAction);
			(psUIBasePopup.m_mainContent as PsUIErrorPopup).CreateHeader(_title);
			(psUIBasePopup.m_mainContent as PsUIErrorPopup).CreateContent(_message, _proceedText);
			psUIBasePopup.Update();
			m_errorPopup = psUIBasePopup;
			m_hasAlertViewOnScreen = true;
		}
	}

	private static void RemoveServerErrorPopup()
	{
		if (m_hasAlertViewOnScreen)
		{
			Debug.Log("Removing server error popup");
			m_errorPopup.Destroy();
			m_errorPopup = null;
			m_hasAlertViewOnScreen = false;
			HttpS.m_gatesFreeze = false;
			if (touchesWereDisabled && Main.m_currentGame.m_sceneManager.m_loadingScene != null)
			{
				TouchAreaS.Disable();
				touchesWereDisabled = false;
			}
			else
			{
				TouchAreaS.Enable();
				touchesWereDisabled = false;
			}
		}
	}

	private static void ServerErrorButtonClicked(string _string)
	{
		ExecuteRetryCallBacks();
		RemoveServerErrorPopup();
		AddConnectingPopup();
	}

	public static void ClearRetryCallBacks()
	{
		RemoveServerErrorPopup();
		m_retryCallbacks.Clear();
	}

	public static void ExecuteRetryCallBacks()
	{
		if (m_retryCallbacks.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < m_retryCallbacks.Count; i++)
		{
			if (m_retryCallbacks[i].m_httpcCallBack != null)
			{
				Debug.Log("Calling retry function: " + m_retryCallbacks[i].m_httpcCallBack.Method.Name);
				HttpC item = m_retryCallbacks[i].m_httpcCallBack();
				m_httpcErrorList.Add(item);
			}
			if (m_retryCallbacks[i].m_actionCallBack != null)
			{
				m_retryCallbacks[i].m_actionCallBack();
			}
			m_retryCallbacks[i] = null;
		}
		m_retryCallbacks.Clear();
	}

	public static void AddConnectingPopup()
	{
		if (!m_dontShowLoginPopup)
		{
			Debug.Log("Adding connecting popup");
			if (m_connectingPopup == null)
			{
				TouchAreaS.CancelAllTouches();
				TouchAreaS.Disable();
				m_connectingPopup = new PsUIBasePopup(typeof(PsUIBlockingMessagePopup), null, null, null, false);
				(m_connectingPopup.m_mainContent as PsUIBlockingMessagePopup).SetText(PsStrings.Get(StringID.PROMPT_CONNECTING));
			}
			else
			{
				Debug.Log("Already has connecting popup.");
			}
			if (PsState.m_canAutoPause)
			{
				PsState.m_activeGameLoop.PauseMinigame();
			}
		}
	}

	public static void RemoveConnectingPopup()
	{
		if (m_connectingPopup != null)
		{
			Debug.Log("Removing connecting popup");
			TouchAreaS.Enable();
			m_connectingPopup.Destroy();
			m_connectingPopup = null;
		}
	}

	public static void ThrowOldVersionMessage(string _message)
	{
		RemoveConnectingPopup();
		CreateServerErrorPopup("Please Update", _message, "Get it!", UpdateButtonClicked);
	}

	private static void UpdateButtonClicked()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Application.OpenURL("https://itunes.apple.com/app/id1048896882");
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			Application.OpenURL("market://details?id=" + Application.bundleIdentifier);
		}
		else
		{
			Debug.LogError("RAISE YOUR VERSION NUMBER FROM THE TL/BuildEditor menu, for example: 2.0.0");
		}
	}

	public static void RegisterForNotifications()
	{
	}

	public static void Update()
	{
	}

	private static string ByteArrayToString(byte[] ba)
	{
		string text = BitConverter.ToString(ba).ToLower();
		return text.Replace("-", string.Empty);
	}
}
