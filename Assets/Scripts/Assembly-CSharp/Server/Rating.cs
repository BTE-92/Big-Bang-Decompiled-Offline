using System;
using MiniJSON;

namespace Server
{
	public static class Rating
	{
		public static HttpC Save(PsMetagameManager.SaveRatingData _data, Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/rating/save");
			wOEURL.AddParameter("gameId", _data.m_minigameId);
			wOEURL.AddParameter("rating", _data.m_rating);
			wOEURL.AddParameter("victory", _data.m_playerReachedGoal.ToString().ToLower());
			wOEURL.AddParameter("playerLevel", _data.m_playerLevel);
			wOEURL.AddParameter("playerUnit", _data.m_playerUnitName);
			string text = null;
			if (_data.setData != null)
			{
				if (_data.setData.ContainsKey("hash"))
				{
					_data.setData["hash"] = ClientTools.GenerateHash(string.Empty);
				}
				else
				{
					_data.setData.Add("hash", ClientTools.GenerateHash(string.Empty));
				}
				text = Json.Serialize(_data.setData);
				return Request.Post(wOEURL, null, text, _okCallback, _failureCallback, _errorCallback, null, new string[1] { "Rating" });
			}
			return Request.Post(wOEURL, null, _okCallback, _failureCallback, _errorCallback, null, new string[1] { "Rating" });
		}
	}
}
