using System;

namespace Server
{
	public static class Achievement
	{
		public static HttpC Upsert(string _json, Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v1/achievement/upsert");
			return Request.Post(url, "UPSERT_ACHIEVEMENT", _json, _okCallback, _failureCallback, _errorCallback);
		}
	}
}
