using System;
using UnityEngine;

namespace Server
{
	public static class InAppPurchase
	{
		public static HttpC GetConfig(Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v1/iap/config");
			return Request.Get(url, null, _okCallback, _failureCallback, _errorCallback);
		}

		public static HttpC CompletePurchase(string _json, Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/iap/purchase");
			wOEURL.AddParameter("platform", Application.platform.ToString());
			wOEURL.AddParameter("id", KeyChain.GetUserId());
			Debug.Log(wOEURL.ConstructURL());
			return Request.Post(wOEURL, "SERVER_COMPLETE_PURCHASE", _json, _okCallback, _failureCallback, _errorCallback, null, new string[1] { "ValidateReceipt" });
		}
	}
}
