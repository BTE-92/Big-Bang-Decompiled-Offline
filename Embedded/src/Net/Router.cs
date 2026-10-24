#nullable enable
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using BBR249Server.Core;

namespace BBR249Server.Net;

public static class Router
{
	private static readonly Dictionary<string, Func<Ctx, Task>> Routes = new Dictionary<string, Func<Ctx, Task>>(StringComparer.OrdinalIgnoreCase);

	private static readonly HashSet<string> SeenUnhandled = new HashSet<string>();

	public static int Count => Routes.Count;

	// optional log sink (Unity's Debug.Log)
	public static Action<string>? Log;

	public static void Map(string path, Func<Ctx, Task> handler)
	{
		Routes[path] = handler;
	}

	private static void Say(string s)
	{
		if (Log != null) Log(s);
	}

	public static EmbeddedResponse Handle(EmbeddedRequest request)
	{
		Ctx ctx = new Ctx(request);
		string path = ctx.Path;
		try
		{
			Func<Ctx, Task>? handler;
			if (Routes.TryGetValue(path, out handler))
			{
				handler(ctx).GetAwaiter().GetResult();
				if (Settings.Verbose) Say("[HTTP] " + request.Method + " " + ctx.RawTarget + " -> " + Status(ctx));
				return ctx.Response;
			}
			if (TryServeStatic(ctx))
			{
				return ctx.Response;
			}
			if (path.StartsWith("/v2.", StringComparison.Ordinal) || request.Url.IndexOf("graph.facebook.com", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				ctx.Text("{}").GetAwaiter().GetResult();
			}
			else
			{
				if (SeenUnhandled.Add(path))
				{
					Say("[HTTP] not implemented (answered with {}): " + path);
				}
				ctx.Json(new JsonObject()).GetAwaiter().GetResult();
			}
		}
		catch (Exception ex)
		{
			Say("[HTTP] ERROR " + request.Method + " " + ctx.RawTarget + ": " + ex);
			ctx.Response.StatusCode = 500;
			ctx.Response.Headers["PLAY_STATUS"] = "ERROR";
			ctx.Response.Headers["PLAY_ERROR"] = "FATAL_ERROR";
			ctx.Response.Body = new byte[0];
		}
		return ctx.Response;
	}

	private static string Status(Ctx c)
	{
		string s;
		string e;
		string st = c.Response.Headers.TryGetValue("PLAY_STATUS", out s) ? s : c.Response.StatusCode.ToString();
		return st + (c.Response.Headers.TryGetValue("PLAY_ERROR", out e) ? " " + e : "");
	}

	// The music banks of the online game no longer exist: every bank is a 16-byte placeholder, the game then plays a built-in track.
	private static bool TryServeStatic(Ctx ctx)
	{
		if (!ctx.Path.StartsWith("/ServerData/", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		ctx.Response.StatusCode = 200;
		ctx.Response.ContentType = "application/octet-stream";
		ctx.Response.Body = new byte[16];
		return true;
	}
}
