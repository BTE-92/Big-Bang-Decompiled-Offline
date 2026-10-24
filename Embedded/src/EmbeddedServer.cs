#nullable enable
using System;
using System.IO;
using BBR249Server.Api;
using BBR249Server.Core;
using BBR249Server.Data;
using BBR249Server.Net;

namespace BBR249Server;

// Entry point used by the game: Initialize() once, then Handle() for each request the game used to send to the online server.
public static class EmbeddedServer
{
	private static bool _ready;

	public static bool Ready => _ready;

	public static int RouteCount => Router.Count;

	public static void Initialize(string dataRoot, Action<string>? log = null)
	{
		if (_ready) return;
		Router.Log = log;
		Settings.BaseDir = dataRoot;
		Directory.CreateDirectory(Settings.DataDir);
		EnsureServerData();
		Db.Init();
		MapContent.EnsureBuilderTemplate();
		MapContent.EnsurePuzzleLevel();
		MapContent.EnsureSeedLevels();
		PlayerApi.Register();
		MinigameApi.Register();
		LevelIoApi.Register();
		LeftoverApi.Register();
		ScoreApi.Register();
		TimedApi.Register();
		ChallengeApi.Register();
		MiscApi.Register();
		_ready = true;
		if (log != null) log("[EmbeddedServer] ready, " + Router.Count + " routes, data in " + Settings.DataDir);
	}

	public static EmbeddedResponse Handle(EmbeddedRequest request)
	{
		if (!_ready) throw new InvalidOperationException("EmbeddedServer.Initialize was not called");
		return Router.Handle(request);
	}

	public static void Shutdown()
	{
		Db.Close();
		_ready = false;
	}

	// The adventure map is built into the assembly: it is copied to Data\ServerData\InitialData on first start.
	private static void EnsureServerData()
	{
		if (File.Exists(Settings.InitialDataFile)) return;
		using Stream? res = typeof(EmbeddedServer).Assembly.GetManifestResourceStream("LocalInitialData.txt");
		if (res == null) return;
		Directory.CreateDirectory(Path.GetDirectoryName(Settings.InitialDataFile)!);
		using FileStream fs = File.Create(Settings.InitialDataFile);
		res.CopyTo(fs);
	}
}
