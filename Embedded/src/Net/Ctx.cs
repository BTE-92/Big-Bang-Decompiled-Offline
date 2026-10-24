#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using BBR249Server.Core;

namespace BBR249Server.Net;

// A request as the game makes it (method, url, headers, body). No sockets: the game hands it over in-process.
public sealed class EmbeddedRequest
{
	public string Method = "GET";
	public string Url = "";
	public Dictionary<string, string> Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
	public byte[] Body = new byte[0];
}

public sealed class EmbeddedResponse
{
	public int StatusCode = 200;
	public string ContentType = "application/json";
	public Dictionary<string, string> Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
	public byte[] Body = new byte[0];
}

public sealed class Ctx
{
	public EmbeddedRequest Request { get; }

	public EmbeddedResponse Response { get; }

	public byte[] Body => Request.Body;

	// path and query exactly as sent by the game (the response hash is computed over it)
	public string RawTarget { get; }

	public string Path { get; }

	private readonly Dictionary<string, string> _query = new Dictionary<string, string>(StringComparer.Ordinal);

	public string PlayerId => Header("PLAYER_ID");

	// base address the game uses to download server files (music banks)
	public const string BaseUrl = "http://bbr.local";

	public Ctx(EmbeddedRequest request)
	{
		Request = request;
		Response = new EmbeddedResponse();
		string url = request.Url ?? "";
		int scheme = url.IndexOf("://", StringComparison.Ordinal);
		string rest = scheme >= 0 ? url.Substring(scheme + 3) : url;
		int slash = rest.IndexOf('/');
		RawTarget = slash >= 0 ? rest.Substring(slash) : "/";
		int q = RawTarget.IndexOf('?');
		Path = q >= 0 ? RawTarget.Substring(0, q) : RawTarget;
		if (q >= 0)
		{
			foreach (string pair in RawTarget.Substring(q + 1).Split('&'))
			{
				if (pair.Length == 0) continue;
				int eq = pair.IndexOf('=');
				string k = Uri.UnescapeDataString((eq >= 0 ? pair.Substring(0, eq) : pair).Replace('+', ' '));
				string v = eq >= 0 ? Uri.UnescapeDataString(pair.Substring(eq + 1).Replace('+', ' ')) : "";
				if (!_query.ContainsKey(k)) _query[k] = v;
			}
		}
	}

	public string Q(string key, string def = "")
	{
		string v;
		return _query.TryGetValue(key, out v) ? v : def;
	}

	public int QInt(string key, int def = 0)
	{
		int result;
		if (!int.TryParse(Q(key), out result))
		{
			return def;
		}
		return result;
	}

	public bool QBool(string key)
	{
		return string.Equals(Q(key), "true", StringComparison.OrdinalIgnoreCase);
	}

	public string Header(string name)
	{
		string v;
		return Request.Headers.TryGetValue(name, out v) ? v : "";
	}

	public JsonNode? BodyJson()
	{
		if (Body.Length == 0)
		{
			return null;
		}
		try
		{
			return JsonNode.Parse(Body);
		}
		catch
		{
			return null;
		}
	}

	public Task Json(JsonNode node)
	{
		return JsonText(node.ToJsonString());
	}

	public Task JsonText(string text)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		Response.StatusCode = 200;
		Response.ContentType = "application/json";
		Response.Headers["PLAY_STATUS"] = "OK";
		Response.Headers["PLAY_HASH"] = Hasher.HashString(RawTarget + text);
		Response.Body = bytes;
		return Task.FromResult(0);
	}

	public Task Text(string text)
	{
		Response.StatusCode = 200;
		Response.ContentType = "application/json";
		Response.Body = Encoding.UTF8.GetBytes(text);
		return Task.FromResult(0);
	}

	public Task Bytes(byte[] data, string? fileSizes = null, IDictionary<string, string>? extraHeaders = null)
	{
		Response.StatusCode = 200;
		Response.ContentType = "application/octet-stream";
		Response.Headers["PLAY_STATUS"] = "OK";
		Response.Headers["PLAY_HASH"] = Hasher.HashBytes(RawTarget, data);
		if (fileSizes != null)
		{
			Response.Headers["FILE_SIZES"] = fileSizes;
		}
		if (extraHeaders != null)
		{
			foreach (KeyValuePair<string, string> extraHeader in extraHeaders)
			{
				Response.Headers[extraHeader.Key] = extraHeader.Value;
			}
		}
		Response.Body = data;
		return Task.FromResult(0);
	}

	public Task Error(string code = "NOT_FOUND")
	{
		Response.StatusCode = 200;
		Response.ContentType = "application/json";
		Response.Headers["PLAY_STATUS"] = "ERROR";
		Response.Headers["PLAY_ERROR"] = code;
		Response.Body = Encoding.UTF8.GetBytes("{}");
		return Task.FromResult(0);
	}
}
