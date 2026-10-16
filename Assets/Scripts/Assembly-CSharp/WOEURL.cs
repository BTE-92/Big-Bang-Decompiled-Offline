using System.Collections;

public class WOEURL : URL
{
	public WOEURL(string _command)
	{
		if (!string.IsNullOrEmpty(_command))
		{
			host = "http://192.168.0.24";//"https://woeprod.traplightgames.com";
			command = _command;
		}
	}

	public HttpC AddGetComponent(string _tag, string[] _queueTags = null, bool _destroyAfterDone = true)
	{
		return HttpS.AddGetComponent(ConstructURL(), _tag, _queueTags, _destroyAfterDone, ConstructHeaders());
	}

	public HttpC AddPostComponent(string _tag)
	{
		Hashtable postHeader = ConstructHeaders();
		return HttpS.AddPostComponent(ConstructURL(), _tag, postHeader);
	}

	public HttpC AddPostComponent(string _tag, string _JSON, Hashtable _postHeader = null, string[] _queueTags = null, bool _destroyAfterDone = true)
	{
		_postHeader = ((_postHeader == null) ? ConstructHeaders(_JSON) : AddGameHeaders(_postHeader, _JSON));
		return HttpS.AddPostComponent(ConstructURL(), _tag, _JSON, _postHeader, _queueTags, _destroyAfterDone);
	}

	public HttpC AddPostComponent(string _tag, byte[] _data, Hashtable _postHeader = null, string[] _queueTags = null, bool _destroyAfterDone = true)
	{
		string url = ConstructURL();
		if (_postHeader == null)
		{
			_postHeader = ConstructHeaders(_data);
		}
		else
		{
			_postHeader = AddGameHeaders(_postHeader, _data);
			_postHeader.Add("Content-Type", "application/octet-stream");
		}
		return HttpS.AddPostComponent(url, _tag, _data, _postHeader, _queueTags, _destroyAfterDone);
	}

	private Hashtable ConstructHeaders()
	{
		Hashtable headers = new Hashtable();
		return AddGameHeaders(headers);
	}

	private Hashtable ConstructHeaders(byte[] _data)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Content-Type", "application/octet-stream");
		return AddGameHeaders(hashtable, _data);
	}

	private Hashtable ConstructHeaders(string _json)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Content-Type", "application/json");
		return AddGameHeaders(hashtable, _json);
	}

	private Hashtable AddGameHeaders(Hashtable _headers)
	{
		_headers.Add("PLAY_HASH", ClientTools.GenerateHash(ConstructURI()));
		return AddSessionHeaders(_headers);
	}

	private Hashtable AddGameHeaders(Hashtable _headers, byte[] _data)
	{
		_headers.Add("PLAY_HASH", ClientTools.GenerateHash(ConstructURI(), _data));
		return AddSessionHeaders(_headers);
	}

	private Hashtable AddGameHeaders(Hashtable _headers, string _json)
	{
		_headers.Add("PLAY_HASH", ClientTools.GenerateHash(ConstructURI() + _json));
		return AddSessionHeaders(_headers);
	}

	private Hashtable AddSessionHeaders(Hashtable _headers)
	{
		_headers.Add("SESSION_ID", PsState.m_sessionId);
		if (KeyChain.GetUserId() != null)
		{
			_headers.Add("PLAYER_ID", KeyChain.GetUserId());
		}
		return _headers;
	}
}
