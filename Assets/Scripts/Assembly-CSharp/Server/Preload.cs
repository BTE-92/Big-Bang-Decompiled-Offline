using System;

namespace Server
{
	public static class Preload
	{
		public static HttpC Check(string _fileName, Action<FileMetaData> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			ResponseHandler<FileMetaData> responseHandler = new ResponseHandler<FileMetaData>(_okCallback, ParseFromResponse);
			WOEURL wOEURL = new WOEURL("/v1/preload/checkFile");
			wOEURL.AddParameter("name", _fileName);
			return Request.Get(wOEURL, null, responseHandler.RequestOk, _failureCallback, PsMetagameManager.GetPreloadCheckERROR);
		}

		public static HttpC Download(FileMetaData _meta, Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			HttpC httpC = Request.PreloaderGet(_meta.path, null, _okCallback, _failureCallback, _errorCallback);
			httpC.objectData = _meta;
			return httpC;
		}

		public static FileMetaData ParseFromResponse(HttpC _c)
		{
			return new FileMetaData(ClientTools.ParseServerResponse(_c.www.text));
		}
	}
}
