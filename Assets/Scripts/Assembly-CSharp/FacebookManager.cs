using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using MiniJSON;
using Server;
using UnityEngine;

public static class FacebookManager
{
	private static bool m_initComplete;

	private static Action m_completeCallback;

	private static string[] m_friends;

	private static FBUserData m_userData;

	public static bool m_friendLoadPending;

	public static void Login(Action _completeCallback)
	{
		Debug.Log("FB Login start");
		m_completeCallback = _completeCallback;
		if (m_initComplete)
		{
			OnFBInitComplete();
		}
		else
		{
			FB.Init(OnFBInitComplete);
		}
	}

	public static void Initialize()
	{
		FB.Init(InitComplete);
	}

	private static void InitComplete()
	{
		FB.ActivateApp();
		m_initComplete = true;
	}

	public static void ReturnedFromBackground()
	{
		if (FB.IsInitialized)
		{
			InitComplete();
		}
		else
		{
			Initialize();
		}
	}

	public static void SendTutorialComplete(bool _complete)
	{
		if (_complete)
		{
			FB.LogAppEvent("fb_mobile_tutorial_completion");
		}
	}

	public static void SendToServer(FBUserData _fbUserData)
	{
		Player.CheckSocialLogin("facebook", _fbUserData.id, m_friends, true, FbLoginSendOk, FbLoginSendFail);
	}

	public static void CheckAccountChange()
	{
		Debug.Log("CheckAccountChange");
		string facebookId = PlayerPrefsX.GetFacebookId();
		if (!FB.IsLoggedIn)
		{
			return;
		}
		if (!string.IsNullOrEmpty(facebookId))
		{
			if (m_userData.id != facebookId)
			{
				CheckAccount(FbLoginSendOk, FbLoginSendFail, false);
			}
			else
			{
				LoadFriends(true);
			}
		}
		else
		{
			CheckAccount(FbLoginSendOk, FbLoginSendFail);
		}
	}

	public static void CheckAccount(Action<HttpC> _success, Action<HttpC> _fail, bool autoJoin = true)
	{
		if (FB.IsLoggedIn)
		{
			Player.CheckSocialLogin("facebook", m_userData.id, m_friends, autoJoin, _success, _fail);
		}
		else
		{
			Debug.Log("Facebook info send to server fail: Not authenticated to FB!");
		}
	}

	private static void FbLoginSendOk(HttpC _c)
	{
		Debug.Log("FB login sent to server");
		Dictionary<string, object> dictionary = ClientTools.ParseServerResponse(_c.www.text);
		if (dictionary.ContainsKey("existingId"))
		{
			string existingId = string.Empty;
			existingId = (string)dictionary["existingId"];
			if (existingId != string.Empty)
			{
				PsUIBasePopup psUIBasePopup = new PsUIBasePopup(typeof(PsUIPopupConfirmFacebook), null, null, null, false);
				psUIBasePopup.SetAction("Proceed", delegate
				{
					RestartWithExistingId(existingId);
				});
				psUIBasePopup.SetAction("Cancel", delegate
				{
					AskForCredentialTransfer(existingId);
				});
			}
		}
		else if (dictionary.ContainsKey("shouldJoin"))
		{
			PsUIBasePopup psUIBasePopup2 = new PsUIBasePopup(typeof(PsUIPopupConfirmNewAccountFacebook), null, null, null, false);
			psUIBasePopup2.SetAction("Proceed", RestartWithNewAccount);
			psUIBasePopup2.SetAction("Cancel", delegate
			{
				AskForCredentialTransfer(string.Empty);
			});
		}
		else
		{
			PlayerPrefsX.SetFacebookId(m_userData.id);
			if (m_completeCallback != null)
			{
				m_completeCallback();
			}
		}
	}

	private static void FbLoginSendFail(HttpC _c)
	{
		Debug.Log("FB login send to server fail");
		if (m_completeCallback != null)
		{
			m_completeCallback();
		}
	}

	private static void AskForCredentialTransfer(string _existingId)
	{
		PsUIBasePopup psUIBasePopup = new PsUIBasePopup(typeof(PsUIPopupConfirmTransferFacebook), null, null, null, false);
		psUIBasePopup.SetAction("Proceed", delegate
		{
			TransferFBToCurrentId(_existingId);
		});
		psUIBasePopup.SetAction("Cancel", DisconnectFB);
	}

	private static void RestartWithExistingId(string _id)
	{
		Debug.Log("Restarting with existing ID");
		string userId = PlayerPrefsX.GetUserId();
		PlayerPrefsX.SetFacebookId(m_userData.id);
		Player.ResolveSocialLogin("facebook", m_userData.id, _id, userId, m_friends, ResolveOkDontLoadFriends, ResolveFail);
		m_friendLoadPending = true;
		PsState.m_restarting = true;
		PlayerPrefsX.SetUserId(_id);
		KeyChain.SetUserId(_id);
		if (PsMetagameManager.m_serverQueueItems != null)
		{
			for (int i = 0; i < PsMetagameManager.m_serverQueueItems.Count; i++)
			{
				PsMetagameManager.m_serverQueueItems[i] = null;
			}
			PsMetagameManager.m_serverQueueItems.Clear();
		}
		m_completeCallback = null;
		Main.m_currentGame.m_sceneManager.ChangeScene(new StartupScene("StartupScene"), new FadeLoadingScene(Color.black));
	}

	private static void RestartWithNewAccount()
	{
		Debug.Log("Restart with new account");
		m_friendLoadPending = true;
		PlayerPrefsX.SetFacebookId(m_userData.id);
		ResetFlow.Start(false);
	}

	private static void TransferFBToCurrentId(string _oldId)
	{
		Debug.Log("Transfer FB to current user");
		m_friendLoadPending = true;
		PlayerPrefsX.SetFacebookId(m_userData.id);
		Player.ResolveSocialLogin("facebook", m_userData.id, PlayerPrefsX.GetUserId(), _oldId, m_friends, ResolveOk, ResolveFail);
	}

	private static void ResolveOkDontLoadFriends(HttpC _c)
	{
		Debug.Log("Social resolve ok.");
		m_friendLoadPending = true;
	}

	private static void ResolveOk(HttpC _c)
	{
		Debug.Log("ResolveOK");
		LoadFriends(true);
	}

	private static void ResolveFail(HttpC _c)
	{
		Debug.Log("Social resolve failed.");
		if (m_completeCallback != null)
		{
			m_completeCallback();
		}
	}

	private static void DisconnectFB()
	{
		Logout(null);
	}

	private static void LoginComplete(FBUserData? _fbUserData)
	{
		Debug.Log("FB Login complete");
		if (_fbUserData.HasValue)
		{
			m_userData = _fbUserData.Value;
			PlayerPrefsX.SetFacebookName(m_userData.username);
			LoadFriends();
		}
		else if (m_completeCallback != null)
		{
			m_completeCallback();
		}
	}

	public static void Logout(Action _completeCallback, bool remove = false)
	{
		if (_completeCallback != null)
		{
			m_completeCallback = _completeCallback;
		}
		if (remove)
		{
			Debug.Log("LOGGING OUT OF FACEBOOK");
			string facebookId = PlayerPrefsX.GetFacebookId();
			FB.LogOut();
			PlayerPrefsX.DeleteKey("FacebookId");
			PlayerPrefsX.DeleteKey("FacebookName");
			Player.RemoveSocialLogin("facebook", facebookId, RemoveOk, RemoveFail);
		}
		else if (m_completeCallback != null)
		{
			m_completeCallback();
		}
	}

	private static void RemoveOk(HttpC _c)
	{
		Debug.Log("Social remove succesful");
		if (m_completeCallback != null)
		{
			m_completeCallback();
		}
	}

	private static void RemoveFail(HttpC _c)
	{
		Debug.Log("Social remove from user failed on server");
		if (m_completeCallback != null)
		{
			m_completeCallback();
		}
	}

	private static void FriendSendOk(HttpC _c)
	{
		Debug.Log("FB friends sent to server");
		if (m_completeCallback != null)
		{
			m_completeCallback();
		}
		m_completeCallback = null;
	}

	private static void FriendSendFail(HttpC _c = null)
	{
		Debug.Log("FB friend send to server fail");
		if (m_completeCallback != null)
		{
			m_completeCallback();
		}
	}

	public static void LoadFriends(bool _pending = false)
	{
		Debug.Log("Load FB Friends");
		FB.API("/me/friends?fields=id,name,installed", HttpMethod.GET, delegate(IGraphResult fbResult)
		{
			if (!string.IsNullOrEmpty(fbResult.Error))
			{
				Debug.LogError("Facebook friend load error " + fbResult.Error);
				m_friends = null;
				FriendSendFail();
			}
			else
			{
				m_friends = ClientTools.ParseFBFriendIds(fbResult.RawResult);
				if (m_friendLoadPending || _pending)
				{
					m_friendLoadPending = false;
					Player.SendSocialFriends("facebook", m_userData.id, m_friends, FriendSendOk, FriendSendFail);
				}
				else
				{
					CheckAccountChange();
				}
			}
		});
	}

	private static void JoinOk(HttpC _c)
	{
		Debug.Log("FB join success");
		Dictionary<string, object> dictionary = ClientTools.ParseServerResponse(_c.www.text);
		object value = null;
		dictionary.TryGetValue("firstJoin", out value);
		if (value != null && (bool)value)
		{
			PsMetagameManager.m_playerStats.diamonds += PlayerPrefsX.GetClientConfig().fbConnectReward;
			PsMetagameManager.SetPlayerData(new Hashtable(), false, PsMetagameManager.PlayerDataSetSUCCEED, PsMetagameManager.PlayerDataSetFAILED);
		}
		if (m_completeCallback != null)
		{
			m_completeCallback();
		}
	}

	private static void JoinFail(HttpC _c)
	{
		Debug.Log("FB join fail");
		Logout(null);
	}

	public static bool IsLoggedIn()
	{
		return PlayerPrefsX.GetFacebookId() != null && FB.IsLoggedIn;
	}

	private static void OnFBInitComplete()
	{
		m_initComplete = true;
		Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
		if (FB.IsLoggedIn)
		{
			FB.API("/me?fields=id,first_name,last_name", HttpMethod.GET, FBUserDataCallback);
		}
		else
		{
			List<string> list = new List<string>();
			list.Add("user_friends");
			FB.LogInWithReadPermissions(list, FBLoginCallback);
		}
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			FB.GetAppLink(DeepLinkCallback);
		}
	}

	private static void FBLoginCallback(ILoginResult result)
	{
		if (result.Error != null)
		{
			Debug.LogError("Error Response from FB Login:\n" + result.Error);
			LoginComplete(null);
		}
		else if (FB.IsLoggedIn)
		{
			Debug.Log("FB Login was successful!");
			FB.API("/me?fields=id,first_name,last_name", HttpMethod.GET, FBUserDataCallback);
		}
		else
		{
			Debug.Log("FB Login cancelled by Player");
			LoginComplete(null);
		}
	}

	private static void FBUserDataCallback(IGraphResult result)
	{
		if (result.Error != null)
		{
			Debug.LogError("Error Response:\n" + result.Error);
			LoginComplete(null);
			return;
		}
		Debug.Log("FB Me call was successful!");
		Debug.Log(result.RawResult);
		FBUserData value = ParseFBUserData(result.RawResult);
		LoginComplete(value);
	}

	private static FBUserData ParseFBUserData(string _FBUserDataString)
	{
		Dictionary<string, object> dictionary = ParseFBResponse(_FBUserDataString);
		FBUserData result = new FBUserData
		{
			id = (string)dictionary["id"]
		};
		string text = (string)dictionary["first_name"];
		string text2 = (string)dictionary["last_name"];
		result.username = text + " " + ((text2.Length <= 0) ? string.Empty : text2.Substring(0, 1));
		return result;
	}

	public static Dictionary<string, object> ParseFBResponse(string _response)
	{
		if (!_response.StartsWith("{"))
		{
			Debug.LogError("Server response not valid JSON: " + _response);
			return null;
		}
		return Json.Deserialize(_response) as Dictionary<string, object>;
	}

	public static HttpC GetPicture(string _facebookId, Action<Texture2D> _okCallback, Action<HttpC> _failureCallback, bool _largePicture)
	{
		ResponseHandler<Texture2D> responseHandler = new ResponseHandler<Texture2D>(_okCallback, ClientTools.ParseTexture);
		string text = "https://graph.facebook.com/" + _facebookId + "/picture";
		text = ((!_largePicture) ? (text + "?width=50&height=50") : (text + "?width=200&height=200"));
		HttpC httpC = HttpS.AddGetComponent(text, null);
		httpC.requestComplete += responseHandler.RequestOk;
		httpC.requestFailed += _failureCallback;
		return httpC;
	}

	private static void DeepLinkCallback(IAppLinkResult result)
	{
		if (!string.IsNullOrEmpty(result.Url))
		{
			int num = new Uri(result.Url).Query.IndexOf("request_ids");
			if (num != -1)
			{
				Debug.Log("Deep link received");
			}
		}
	}
}
