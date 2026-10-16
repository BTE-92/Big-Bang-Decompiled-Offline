using System;
using System.Collections.Generic;
using Server;
using UnityEngine;

public class ErrorHandler
{
	private Action<HttpC> m_okCallback;

	private Action m_errorCallback;

	private Action m_restartCallback = delegate
	{
		Main.m_currentGame.m_sceneManager.ChangeScene(new StartupScene("StartupScene"), new FadeLoadingScene(Color.black));
	};

	public ErrorHandler(Action<HttpC> _okCallback, Action _errorCallback = null)
	{
		m_okCallback = _okCallback;
		m_errorCallback = _errorCallback;
		if (m_errorCallback == null)
		{
			m_errorCallback = m_restartCallback;
		}
	}

	public bool ContentIsTypeOf(Dictionary<string, string> _headers, string _contentType)
	{
		return _headers.ContainsValue(_contentType);
	}

	public void CheckForErrors(HttpC _c)
	{
		Debug.Log("Checking For Errors: " + _c.www.url);
		Dictionary<string, string> responseHeaders = _c.www.responseHeaders;
		if (responseHeaders.ContainsKey("PLAY_STATUS"))
		{
			if (responseHeaders["PLAY_STATUS"].Equals("OK"))
			{
				if (CheckHash(_c) && m_okCallback != null)
				{
					m_okCallback(_c);
				}
			}
			else if (responseHeaders["PLAY_STATUS"].Equals("ERROR"))
			{
				switch (responseHeaders["PLAY_ERROR"])
				{
				case "VALIDATION_ERROR":
					Debug.LogError("SERVER ERROR: " + responseHeaders["PLAY_ERROR"]);
					Debug.LogError(responseHeaders["PLAY_ERROR"] + ": " + _c.www.text);
					ServerManager.ThrowServerErrorException("Connection error", "Unable to connect with the server. Check  your internet connection and try again.", null, m_errorCallback);
					break;
				case "SESSION_ERROR":
					Debug.LogWarning("SERVER ERROR: " + responseHeaders["PLAY_ERROR"]);
					Debug.LogWarning(responseHeaders["PLAY_ERROR"] + ": " + _c.www.text);
					ServerManager.ThrowServerErrorException("Session expired", "Your account was used on another device, press retry to login again.", null, LoginFlow.Restart);
					break;
				case "FATAL_ERROR":
					Debug.LogError("SERVER ERROR: " + responseHeaders["PLAY_ERROR"]);
					Debug.LogError(responseHeaders["PLAY_ERROR"] + ": " + _c.www.text);
					ServerManager.ThrowServerErrorException("Connection error", "Unable to connect with the server. Check  your internet connection and try again.", null, m_errorCallback);
					break;
				case "UNKNOWN":
					Debug.LogError("SERVER ERROR: " + responseHeaders["PLAY_ERROR"]);
					Debug.LogError(responseHeaders["PLAY_ERROR"] + ": " + _c.www.text);
					HttpS.ClearAllRequests();
					ServerManager.ThrowServerErrorException("Connection error", "Unable to connect with the server. Check  your internet connection and try again.", null, m_restartCallback);
					break;
				case "NOT_FOUND":
					Debug.LogWarning("SERVER ERROR: " + responseHeaders["PLAY_ERROR"]);
					Debug.LogWarning(responseHeaders["PLAY_ERROR"] + ": " + _c.www.text);
					m_errorCallback();
					break;
				}
			}
		}
		else
		{
			ThrowError(_c, "ERROR: RESPONSE DIDN'T HAVE STATUS HEADER");
		}
	}

	public bool CheckHash(HttpC _c)
	{
		Dictionary<string, string> responseHeaders = _c.www.responseHeaders;
		string uri = GetUri(_c.www.url);
		if (responseHeaders.ContainsKey("PLAY_HASH"))
		{
			if (ContentIsTypeOf(responseHeaders, "application/json"))
			{
				if (!ClientTools.GenerateHash(uri + _c.www.text).Equals(responseHeaders["PLAY_HASH"]))
				{
					ThrowError(_c, "ERROR: JSON HASH MISMATCH");
					return false;
				}
			}
			else
			{
				if (!ContentIsTypeOf(responseHeaders, "application/octet-stream"))
				{
					ThrowError(_c, "ERROR: RESPONSE DIDN'T HAVE CORRECT CONTENT TYPE");
					return false;
				}
				if (!ClientTools.GenerateHash(uri, _c.www.bytes).Equals(responseHeaders["PLAY_HASH"]))
				{
					ThrowError(_c, "ERROR: DATA HASH MISMATCH");
					return false;
				}
			}
			return true;
		}
		ThrowError(_c, "ERROR: RESPONSE DIDN'T HAVE HASH");
		return false;
	}

	public string GetUri(string url)
	{
		int num = 0;
		char c = '/';
		for (int i = 0; i < url.Length; i++)
		{
			if (url[i] == c)
			{
				num++;
				if (num == 3)
				{
					return url.Substring(i);
				}
			}
		}
		return string.Empty;
	}

	private void ThrowError(HttpC _c, string _errorMessage)
	{
		Debug.LogError(_c.www.error);
		Debug.LogError(_errorMessage + ", CHECK REQUEST " + _c.www.url);
		ServerManager.ThrowServerErrorException("Connection error", "Unable to connect with the server. Check  your internet connection and try again.", null, m_restartCallback);
	}
}
