using System;

namespace Server
{
	public static class Ads
	{
		public static HttpC GetAdNetworkConfig(Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v1/ads/config");
			return Request.Get(url, null, _okCallback, _failureCallback, _errorCallback);
		}
	}
}
