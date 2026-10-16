using System;
using System.Collections;
using System.Linq;

namespace Server
{
	public static class Path
	{
		public static HttpC GetInitial(Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null, int minorVersion = 0)
		{
			WOEURL wOEURL = new WOEURL("/v1/path/db/find");
			wOEURL.AddParameter("majorVersion", PlayerPrefsX.GetMajorPath().ToString());
			if (minorVersion != 0)
			{
				wOEURL.AddParameter("version", 2.ToString());
			}
			return Request.Get(wOEURL, "PATH_DB_FIND", _okCallback, _failureCallback, _errorCallback);
		}

		public static HttpC SaveInitialWithItemLevelData(string _itemLevelJson, string _pathJson, Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v2/path/db/update");
			wOEURL.AddParameter("majorVersion", 2.ToString());
			DataBlob dataBlob = ClientTools.CreatePathDataBlob(_itemLevelJson, _pathJson);
			Hashtable header = dataBlob.header;
			return Request.Post(wOEURL, "PATH_DB_UPDATE_V2", dataBlob.data, _okCallback, _failureCallback, _errorCallback, dataBlob.header);
		}

		public static HttpC ResetProgression(Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v1/path/reset");
			return Request.Post(url, "PATH_RESET", _okCallback, _failureCallback, _errorCallback);
		}

		public static HttpC GetProgression(string[] _roots, int[] _startNodes, Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/path/get");
			string text = string.Empty;
			string text2 = string.Empty;
			for (int i = 0; i < _roots.Count(); i++)
			{
				text2 += _roots[i];
				text += _startNodes[i];
			}
			wOEURL.AddParameter("roots", text);
			wOEURL.AddParameter("starts", text2);
			return Request.Get(wOEURL, "PATH_PROGRESSION_GET", _okCallback, _failureCallback, _errorCallback);
		}
	}
}
