using System;
using System.Collections.Generic;

namespace Server
{
	public static class Reward
	{
		public static HttpC ClaimRewards(List<PsMinigameMetaData> _gamesToClaim, Action<RewardData> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v1/reward/claim");
			string json = ClientTools.GenerateIdListJson(_gamesToClaim);
			ResponseHandler<RewardData> responseHandler = new ResponseHandler<RewardData>(_okCallback, ClientTools.ParseRewardData);
			return Request.Post(url, "REWARD_CLAIM", json, responseHandler.RequestOk, _failCallback, _errorCallback);
		}
	}
}
