using System;
using UnityEngine;

namespace Server
{
	public static class Comment
	{
		public static HttpC Save(string gameId, string comment, string _tag, string _tournamentId, Action<HttpC> _okCallback, Action<HttpC> _failureCallback = null, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/minigame/comment/save");
			wOEURL.AddParameter("gameId", gameId);
			wOEURL.AddParameter("comment", WWW.EscapeURL(comment));
			wOEURL.AddParameter("hash", ClientTools.GenerateHash(gameId + PlayerPrefsX.GetUserId() + comment));
			wOEURL.AddParameter("tag", _tag);
			if (!string.IsNullOrEmpty(_tournamentId))
			{
				wOEURL.AddParameter("tournamentId", _tournamentId);
			}
			return Request.Post(wOEURL, null, _okCallback, _failureCallback, _errorCallback);
		}

		public static HttpC Get(string _gameId, Action<CommentData[]> _okCallback, Action<HttpC> _failureCallback = null, Action _errorCallback = null)
		{
			ResponseHandler<CommentData[]> responseHandler = new ResponseHandler<CommentData[]>(_okCallback, ClientTools.ParseComments);
			WOEURL wOEURL = new WOEURL("/v1/minigame/comment/find");
			wOEURL.AddParameter("gameId", _gameId);
			return Request.Get(wOEURL, "GET_COMMENTS", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}
	}
}
