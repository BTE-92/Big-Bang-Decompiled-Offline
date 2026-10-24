using System;
using System.Collections.Generic;
using System.Text;
using BBR249Server;
using BBR249Server.Net;
using UnityEngine;

// Drop-in for WWW in the game's HTTP layer. Requests to the game's server are answered by the embedded server
// (OfflineServer) without any network; every other url still goes through a real WWW.
public class GameWWW
{
	private readonly WWW m_real;

	private readonly EmbeddedResponse m_response;

	private readonly string m_url;

	private string m_error;

	private Dictionary<string, string> m_headers;

	private string m_text;

	public GameWWW(string _url)
		: this(_url, null, null)
	{
	}

	public GameWWW(string _url, byte[] _postData, Dictionary<string, string> _headers)
	{
		m_url = _url;
		if (!OfflineServer.IsServerUrl(_url))
		{
			m_real = (_postData == null && _headers == null) ? new WWW(_url) : new WWW(_url, _postData, _headers);
			return;
		}
		if (!OfflineServer.EnsureStarted())
		{
			m_error = "Offline server failed to start: " + OfflineServer.LastError;
			return;
		}
		EmbeddedRequest request = new EmbeddedRequest();
		request.Method = (_postData == null) ? "GET" : "POST";
		request.Url = _url;
		request.Body = _postData ?? new byte[0];
		if (_headers != null)
		{
			foreach (KeyValuePair<string, string> header in _headers)
			{
				request.Headers[header.Key] = header.Value;
			}
		}
		try
		{
			m_response = EmbeddedServer.Handle(request);
		}
		catch (Exception e)
		{
			Debug.LogError("[OfflineServer] request failed: " + e);
			m_error = "500 Internal Server Error";
			return;
		}
		if (m_response.StatusCode >= 400)
		{
			m_error = m_response.StatusCode + " " + (m_response.StatusCode == 404 ? "Not Found" : "Internal Server Error");
		}
	}

	public bool isDone
	{
		get { return m_real != null ? m_real.isDone : true; }
	}

	public float progress
	{
		get { return m_real != null ? m_real.progress : 1f; }
	}

	public string error
	{
		get { return m_real != null ? m_real.error : m_error; }
	}

	public string url
	{
		get { return m_real != null ? m_real.url : m_url; }
	}

	public byte[] bytes
	{
		get
		{
			if (m_real != null)
			{
				return m_real.bytes;
			}
			return m_response != null ? m_response.Body : new byte[0];
		}
	}

	public string text
	{
		get
		{
			if (m_real != null)
			{
				return m_real.text;
			}
			if (m_text == null)
			{
				m_text = (m_response != null) ? Encoding.UTF8.GetString(m_response.Body) : string.Empty;
			}
			return m_text;
		}
	}

	public Dictionary<string, string> responseHeaders
	{
		get
		{
			if (m_real != null)
			{
				return m_real.responseHeaders;
			}
			if (m_headers == null)
			{
				m_headers = new Dictionary<string, string>();
				if (m_response != null)
				{
					foreach (KeyValuePair<string, string> header in m_response.Headers)
					{
						m_headers[header.Key.ToUpperInvariant()] = header.Value;
					}
					m_headers["CONTENT-TYPE"] = m_response.ContentType;
					m_headers["STATUS"] = "HTTP/1.1 " + m_response.StatusCode + " OK";
				}
			}
			return m_headers;
		}
	}

	public void LoadImageIntoTexture(Texture2D _texture)
	{
		if (m_real != null)
		{
			m_real.LoadImageIntoTexture(_texture);
			return;
		}
		if (m_response != null)
		{
			_texture.LoadImage(m_response.Body);
		}
	}
}
