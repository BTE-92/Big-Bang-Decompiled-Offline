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
#if (UNITY_IOS || UNITY_WEBGL) && !UNITY_EDITOR
			// on iOS and WebGL sqlite is compiled into the app (Plugins/iOS/sqlite); everywhere else it is a shared library
			Db.NativeApi = new IosSqliteApi();
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
			// WebGL: no threads, no shared memory (so no WAL); the file system is in memory and written to IndexedDB by SyncToBrowser
			// the assembly's embedded resources are not available in the WebGL build: the files come from Resources/BBR/server (name with . replaced by _, *.bytes)
			BBR249Server.Core.EmbeddedResources.Provider = delegate(string name)
			{
				TextAsset asset = Resources.Load<TextAsset>("BBR/server/" + name.Replace('.', '_'));
				return asset != null ? asset.bytes : null;
			};
			Db.StartupPragmas = "PRAGMA journal_mode=MEMORY; PRAGMA locking_mode=EXCLUSIVE; PRAGMA foreign_keys=OFF; PRAGMA synchronous=OFF;";
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

#if UNITY_WEBGL && !UNITY_EDITOR
	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void BBR_SyncFileSystem();

	private static float s_lastSync;

	private static bool s_dirty;
#endif

	// WebGL only: the database sits in the browser's in-memory file system, so every change has to be written to IndexedDB
	// (at most every few seconds) or it is lost when the page is closed. Does nothing on the other platforms.
	public static void SyncToBrowser(bool _force = false)
	{
#if UNITY_WEBGL && !UNITY_EDITOR
		s_dirty = true;
		if (_force || Time.realtimeSinceStartup - s_lastSync > 5f)
		{
			s_lastSync = Time.realtimeSinceStartup;
			s_dirty = false;
			BBR_SyncFileSystem();
		}
#endif
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
