using System;
using System.Collections.Generic;

namespace Server
{
	public static class Challenge
	{
		public static HttpC GetDaily(Action<List<PsChallengeMetaData>> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			ResponseHandler<List<PsChallengeMetaData>> responseHandler = new ResponseHandler<List<PsChallengeMetaData>>(_okCallback, ClientTools.ParseChallengeList);
			WOEURL url = new WOEURL("/v1/challenge/daily/find");
			return Request.Get(url, "CHALLENGE_GET_DAILY", responseHandler.RequestOk, _failCallback, _errorCallback);
		}
	}
}
