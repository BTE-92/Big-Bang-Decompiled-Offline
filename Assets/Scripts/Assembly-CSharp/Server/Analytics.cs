using System;

namespace Server
{
	public static class Analytics
	{
		public static void TrackPurchase(string _itemName, float _price, string _currencyCode)
		{
			TrackPurchase(_itemName, _price, _currencyCode, AnalyticsOkCallback, delegate(HttpC c)
			{
				AnalyticsFailCallback(c, delegate
				{
					TrackPurchase(_itemName, _price, _currencyCode);
				});
			});
		}

		private static HttpC TrackPurchase(string _itemName, float _price, string _currencyCode, Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/analytics/trackPurchase");
			wOEURL.AddParameter("name", _itemName);
			wOEURL.AddParameter("price", _price);
			wOEURL.AddParameter("currencyCode", _currencyCode);
			return Request.Post(wOEURL, null, _okCallback, _failureCallback, _errorCallback);
		}

		private static void AnalyticsOkCallback(HttpC _c)
		{
		}

		private static void AnalyticsFailCallback(HttpC _c, Action _retry)
		{
			ServerErrors.GetNetworkError(_c.www.error);
			_retry();
		}
	}
}
