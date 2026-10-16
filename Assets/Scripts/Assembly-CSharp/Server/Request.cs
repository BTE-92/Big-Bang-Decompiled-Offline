using System;
using System.Collections;

namespace Server
{
	public static class Request
	{
		public static HttpC Get(WOEURL _url, string _tag)
		{
			return Get(_url, _tag, GenericSuccessCallback);
		}

		public static HttpC Get(WOEURL _url, string _tag, Action<HttpC> _okCallback)
		{
			return Get(_url, _tag, _okCallback, GenericFailureCallback);
		}

		public static HttpC Get(WOEURL _url, string _tag, Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			ErrorHandler errorHandler = new ErrorHandler(_okCallback, _errorCallback);
			HttpC httpC = _url.AddGetComponent(_tag);
			httpC.requestComplete += errorHandler.CheckForErrors;
			httpC.requestFailed += _failureCallback;
			return httpC;
		}

		public static HttpC PreloaderGet(string _path, string _tag, Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			HttpC httpC = HttpS.AddGetComponent(_path, _tag, null, true, new Hashtable());
			httpC.requestComplete += _okCallback;
			httpC.requestFailed += _failureCallback;
			return httpC;
		}

		public static HttpC Post(WOEURL _url, string _tag)
		{
			return Post(_url, _tag, null);
		}

		public static HttpC Post(WOEURL _url, string _tag, string _json)
		{
			return Post(_url, _tag, _json, GenericSuccessCallback);
		}

		public static HttpC Post(WOEURL _url, string _tag, string _json, Action<HttpC> _okCallback)
		{
			return Post(_url, _tag, _json, _okCallback, GenericFailureCallback);
		}

		public static HttpC Post(WOEURL _url, string _tag, string _json, Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null, Hashtable _headers = null, string[] _queueTags = null)
		{
			ErrorHandler errorHandler = new ErrorHandler(_okCallback, _errorCallback);
			HttpC httpC = _url.AddPostComponent(_tag, _json, _headers, _queueTags);
			httpC.requestComplete += errorHandler.CheckForErrors;
			httpC.requestFailed += _failureCallback;
			return httpC;
		}

		public static HttpC Post(WOEURL _url, string _tag, Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null, Hashtable _headers = null, string[] _queueTags = null)
		{
			return Post(_url, _tag, new byte[1], _okCallback, _failureCallback, _errorCallback, _headers, _queueTags);
		}

		public static HttpC Post(WOEURL _url, string _tag, byte[] _data, Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null, Hashtable _headers = null, string[] _queueTags = null)
		{
			ErrorHandler errorHandler = new ErrorHandler(_okCallback, _errorCallback);
			HttpC httpC = _url.AddPostComponent(_tag, _data, _headers, _queueTags);
			httpC.requestComplete += errorHandler.CheckForErrors;
			httpC.requestFailed += _failureCallback;
			return httpC;
		}

		public static void GenericSuccessCallback(HttpC _c, string _requestIdentifier)
		{
			Debug.Log("SERVER REQUEST SUCCEEDED: " + _requestIdentifier);
		}

		public static void GenericSuccessCallback(HttpC _c)
		{
			Debug.Log("SERVER REQUEST SUCCEEDED");
		}

		public static void GenericFailureCallback(HttpC _c)
		{
			ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error));
		}

		public static void OnFailureInstantRetryCallback(HttpC _c, Action _retryFunction)
		{
			ServerErrors.GetNetworkError(_c.www.error);
			_retryFunction();
		}
	}
}
