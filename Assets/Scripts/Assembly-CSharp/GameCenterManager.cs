using System;
using System.Collections.Generic;
using Server;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public static class GameCenterManager
{
	public class GameCenterPictureDownloader
	{
		public Action<Texture2D> m_callback;

		public GameCenterPictureDownloader(string _gameCenterId)
		{
			Social.LoadUsers(new string[1] { _gameCenterId }, ImageLoaded);
		}

		private void ImageLoaded(IUserProfile[] users)
		{
			m_callback(users[0].image);
		}
	}

	public static bool m_gcCallbackHandled;

	public static void Logout(Action _GCLogoutComplete)
	{
		Debug.Log("LOGGING OUT OF GAME CENTER");
		PlayerPrefsX.DeleteKey("GameCenterId");
		PlayerPrefsX.DeleteKey("GameCenterName");
		KeyChain.SetGameCenterId(string.Empty);
		m_gcCallbackHandled = false;
	}

	public static void CheckAccountChange()
	{
		string gameCenterId = PlayerPrefsX.GetGameCenterId();
		if (!Social.localUser.authenticated)
		{
			return;
		}
		if (!string.IsNullOrEmpty(gameCenterId))
		{
			if (Social.localUser.id != PlayerPrefsX.GetGameCenterId())
			{
				CheckAccount(GcLoginSendOk, GcLoginSendFail, false);
			}
			else
			{
				LoadFriends();
			}
		}
		else
		{
			CheckAccount(GcLoginSendOk, GcLoginSendFail);
		}
	}

	public static void GCLoginCallback(bool _success)
	{
		Debug.Log("GC Login returned to callback.");
		if (m_gcCallbackHandled)
		{
			Debug.LogWarning("GC CALLBACK CANCELLED, LOGIN SUCCESS: " + _success);
			return;
		}
		if (_success)
		{
			Debug.Log("GC Login successful");
			CheckAccountChange();
		}
		else
		{
			Debug.Log("GC Login failed");
		}
		m_gcCallbackHandled = true;
	}

	public static void Login()
	{
		Debug.Log("CALLED GC LOGIN.");
		if (!Social.localUser.authenticated)
		{
			Debug.Log("GC Starting to authenticate...");
			Social.localUser.Authenticate(GCLoginCallback);
		}
		else
		{
			Debug.Log("GC already logged in.");
			m_gcCallbackHandled = true;
			CheckAccountChange();
		}
	}

	public static void LoadFriends()
	{
		if (PsState.m_restarting || !Social.localUser.authenticated || string.IsNullOrEmpty(PlayerPrefsX.GetGameCenterId()))
		{
			return;
		}
		Debug.Log("Starting GC Friend loading");
		Social.localUser.LoadFriends(delegate(bool success)
		{
			if (success)
			{
				Player.SendSocialFriends("gamecenter", Social.localUser.id, GenerateGCFriendArray(), FriendSendOk, FriendSendFail);
			}
			else
			{
				Debug.Log("GC Friends not loaded, trying on next login...");
			}
		});
	}

	public static void CheckAccount(Action<HttpC> _success, Action<HttpC> _fail, bool autoJoin = true)
	{
		string[] friendIds = new string[0];
		if (Social.localUser.authenticated)
		{
			Player.CheckSocialLogin("gamecenter", Social.localUser.id, friendIds, autoJoin, _success, _fail);
		}
		else
		{
			Debug.Log("GameCenter info send to server fail: Not authenticated to GC!");
		}
	}

	private static void GcLoginSendOk(HttpC _c)
	{
		Debug.Log("GC login sent to server");
		Dictionary<string, object> dictionary = ClientTools.ParseServerResponse(_c.www.text);
		if (dictionary.ContainsKey("existingId"))
		{
			string existingId = string.Empty;
			existingId = (string)dictionary["existingId"];
			if (existingId != string.Empty)
			{
				PsUIBasePopup psUIBasePopup = new PsUIBasePopup(typeof(PsUIPopupConfirmGameCenter), null, null, null, false);
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
			PsUIBasePopup psUIBasePopup2 = new PsUIBasePopup(typeof(PsUIPopupConfirmNewAccountGameCenter), null, null, null, false);
			psUIBasePopup2.SetAction("Proceed", RestartWithNewAccount);
			psUIBasePopup2.SetAction("Cancel", delegate
			{
				AskForCredentialTransfer(string.Empty);
			});
		}
		else
		{
			LoadFriends();
		}
	}

	private static void AskForCredentialTransfer(string _existingId)
	{
		PsUIBasePopup psUIBasePopup = new PsUIBasePopup(typeof(PsUIPopupConfirmTransferGameCenter), null, null, null, false);
		psUIBasePopup.SetAction("Proceed", delegate
		{
			TransferGCToCurrentId(_existingId);
		});
		psUIBasePopup.SetAction("Cancel", DisconnectGC);
	}

	private static void RestartWithNewAccount()
	{
		ResetFlow.Start(false);
	}

	private static void RestartWithExistingId(string _id)
	{
		string[] friendIds = new string[0];
		string userId = PlayerPrefsX.GetUserId();
		Player.ResolveSocialLogin("gamecenter", Social.localUser.id, _id, userId, friendIds, ResolveOk, ResolveFail);
		PsState.m_restarting = true;
		PlayerPrefsX.SetUserId(_id);
		KeyChain.SetUserId(_id);
		PlayerPrefsX.SetGameCenterId(Social.localUser.id);
		if (PsMetagameManager.m_serverQueueItems != null)
		{
			for (int i = 0; i < PsMetagameManager.m_serverQueueItems.Count; i++)
			{
				PsMetagameManager.m_serverQueueItems[i] = null;
			}
			PsMetagameManager.m_serverQueueItems.Clear();
		}
		Main.m_currentGame.m_sceneManager.ChangeScene(new StartupScene("StartupScene"), new FadeLoadingScene(Color.black));
	}

	private static void TransferGCToCurrentId(string _oldId)
	{
		string[] friendIds = new string[0];
		Player.ResolveSocialLogin("gamecenter", Social.localUser.id, PlayerPrefsX.GetUserId(), _oldId, friendIds, ResolveOk, ResolveFail);
	}

	private static void DisconnectGC()
	{
		Logout(delegate
		{
			Debug.Log("Disconnected user from GC");
		});
	}

	private static void ResolveOk(HttpC _c)
	{
		LoadFriends();
	}

	private static void ResolveFail(HttpC _c)
	{
		Debug.Log("Social resolve failed.");
	}

	private static void FriendSendOk(HttpC _c)
	{
		Debug.Log("GC friends sent to server");
	}

	private static void GcLoginSendFail(HttpC _c)
	{
		Debug.Log("GC login send to server fail");
	}

	private static void FriendSendFail(HttpC _c)
	{
		Debug.Log("GC friend send to server fail");
	}

	private static string[] GenerateGCFriendArray()
	{
		string[] array = new string[Social.localUser.friends.Length];
		for (int num = Social.localUser.friends.Length - 1; num > -1; num--)
		{
			array[num] = Social.localUser.friends[num].id;
		}
		return array;
	}

	public static bool IsLoggedIn()
	{
		return PlayerPrefsX.GetGameCenterId() != null;
	}

	public static void GetPicture(string _gameCenterId, Action<Texture2D> _callback)
	{
		if (Social.localUser.authenticated)
		{
			if (_gameCenterId == PlayerPrefsX.GetGameCenterId())
			{
				_callback(Social.localUser.image);
				return;
			}
			GameCenterPictureDownloader gameCenterPictureDownloader = new GameCenterPictureDownloader(_gameCenterId);
			gameCenterPictureDownloader.m_callback = (Action<Texture2D>)Delegate.Combine(gameCenterPictureDownloader.m_callback, _callback);
		}
	}
}
