using System;
using System.IO;
using BBR249Server;
using BBR249Server.Data;
using UnityEngine;

// Starts the game server that now lives inside the game (BBR249.Embedded.dll): the same code as the old online server,
// with its database in the game's own data folder. Nothing goes over the network.
public static class OfflineServer
{
	// Address the game uses for its server. Requests to it (and to the old online hosts) are answered in-process.
	public const string BaseUrl = "http://bbr.local";

	private static bool s_started;

	private static string s_error;

	public static string LastError
	{
		get { return s_error; }
	}

	public static string DataRoot
	{
		get { return Path.Combine(Application.persistentDataPath, "BBROffline"); }
	}

	// True when the url belongs to the game's server (and so must be answered by the embedded server).
	public static bool IsServerUrl(string _url)
	{
		if (string.IsNullOrEmpty(_url))
		{
			return false;
		}
		Uri uri;
		if (!Uri.TryCreate(_url, UriKind.Absolute, out uri))
		{
			return false;
		}
		string host = uri.Host.ToLowerInvariant();
		return host == "bbr.local"
			|| host == "woeprod.traplightgames.com"
			|| host == "graph.facebook.com";
	}

	public static bool EnsureStarted()
	{
		if (s_started)
		{
			return true;
		}
		if (s_error != null)
		{
			return false;
		}
		try
		{
			Directory.CreateDirectory(DataRoot);
#if UNITY_IOS && !UNITY_EDITOR
			// on iOS sqlite is compiled into the app (Plugins/iOS/sqlite); everywhere else it is a shared library
			Db.NativeApi = new IosSqliteApi();
#endif
			EmbeddedServer.Initialize(DataRoot, delegate(string line) { Debug.Log(line); });
			s_started = true;
			return true;
		}
		catch (Exception e)
		{
			s_error = e.GetType().Name + ": " + e.Message;
			Debug.LogError("[OfflineServer] could not start: " + e);
			return false;
		}
	}

	public static void Shutdown()
	{
		if (s_started)
		{
			EmbeddedServer.Shutdown();
			s_started = false;
		}
	}
}
