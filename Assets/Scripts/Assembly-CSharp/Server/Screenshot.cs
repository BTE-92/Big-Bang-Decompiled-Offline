using System;

namespace Server
{
	public static class Screenshot
	{
		public static void Save(string gameId, byte[] data, Action<HttpC> _okCallback, Action<HttpC> _failureCallback = null, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/minigame/screenshot/save");
			wOEURL.AddParameter("gameId", gameId);
			Request.Post(wOEURL, null, data, _okCallback, _failureCallback, _errorCallback);
		}

		public static HttpC Get(string gameId, Action<byte[]> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			if (string.IsNullOrEmpty(gameId))
			{
				Debug.LogError("NULL OR EMPTY ID FORBIDDEN");
			}
			ResponseHandler<byte[]> responseHandler = new ResponseHandler<byte[]>(_okCallback, ClientTools.ParseBytes);
			WOEURL wOEURL = new WOEURL("/v1/minigame/screenshot/find");
			wOEURL.AddParameter("gameId", gameId);
			return Request.Get(wOEURL, null, responseHandler.RequestOk, _failureCallback, PsMetagameManager.GetScreenshotERROR);
		}
	}
}
