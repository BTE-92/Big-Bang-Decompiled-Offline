using System;

namespace Server
{
	public static class Metagame
	{
		public static HttpC GetTimedEvents(Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v1/metagame/getTimed");
			return Request.Get(url, "TIMED_EVENT_GET", _okCallback, _failureCallback, _errorCallback);
		}
	}
}
