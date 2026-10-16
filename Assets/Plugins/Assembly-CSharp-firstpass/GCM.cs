using System;
using System.Collections.Generic;
using UnityEngine;

public class GCM
{
	private const string CLASS_NAME = "com.kskkbys.unitygcmplugin.UnityGCMRegister";

	private static GameObject _receiver;

	public static void Initialize()
	{
		if (Application.platform == RuntimePlatform.Android && _receiver == null)
		{
			_receiver = new GameObject("GCMReceiver");
			_receiver.AddComponent<GCMReceiver>();
		}
	}

	public static void ShowToast(string message)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.kskkbys.unitygcmplugin.Util"))
			{
				androidJavaClass.CallStatic("showToast", message);
			}
		}
	}

	public static void Register(params string[] senderIds)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.kskkbys.unitygcmplugin.UnityGCMRegister"))
			{
				string text = string.Join(",", senderIds);
				androidJavaClass.CallStatic("register", text);
			}
		}
	}

	public static void Unregister()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.kskkbys.unitygcmplugin.UnityGCMRegister"))
			{
				androidJavaClass.CallStatic("unregister");
			}
		}
	}

	public static bool IsRegistered()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.kskkbys.unitygcmplugin.UnityGCMRegister"))
			{
				return androidJavaClass.CallStatic<bool>("isRegistered", new object[0]);
			}
		}
		return false;
	}

	public static string GetRegistrationId()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.kskkbys.unitygcmplugin.UnityGCMRegister"))
			{
				return androidJavaClass.CallStatic<string>("getRegistrationId", new object[0]);
			}
		}
		return null;
	}

	public static void SetRegisteredOnServer(bool isRegistered)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.kskkbys.unitygcmplugin.UnityGCMRegister"))
			{
				androidJavaClass.CallStatic("setRegisteredOnServer", isRegistered);
			}
		}
	}

	public static bool IsRegisteredOnServer()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.kskkbys.unitygcmplugin.UnityGCMRegister"))
			{
				return androidJavaClass.CallStatic<bool>("isRegisteredOnServer", new object[0]);
			}
		}
		return false;
	}

	public static void SetRegisterOnServerLifespan(long lifespan)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.kskkbys.unitygcmplugin.UnityGCMRegister"))
			{
				androidJavaClass.CallStatic("setRegisterOnServerLifespan", lifespan);
			}
		}
	}

	public static long GetRegisterOnServerLifespan()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.kskkbys.unitygcmplugin.UnityGCMRegister"))
			{
				return androidJavaClass.CallStatic<long>("getRegisterOnServerLifespan", new object[0]);
			}
		}
		return 0L;
	}

	public static void SetErrorCallback(Action<string> onError)
	{
		GCMReceiver._onError = onError;
	}

	public static void SetMessageCallback(Action<Dictionary<string, object>> onMessage)
	{
		GCMReceiver._onMessage = onMessage;
	}

	public static void SetRegisteredCallback(Action<string> onRegistered)
	{
		GCMReceiver._onRegistered = onRegistered;
	}

	public static void SetUnregisteredCallback(Action<string> onUnregistered)
	{
		GCMReceiver._onUnregistered = onUnregistered;
	}

	public static void SetDeleteMessagesCallback(Action<int> onDeleteMessages)
	{
		GCMReceiver._onDeleteMessages = onDeleteMessages;
	}
}
