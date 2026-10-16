using System;
using System.Collections;

namespace Server
{
	public static class Template
	{
		public static HttpC GetAll(Action<PsMinigameMetaData[]> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null, int _limit = 10)
		{
			ResponseHandler<PsMinigameMetaData[]> responseHandler = new ResponseHandler<PsMinigameMetaData[]>(_okCallback, ClientTools.ParseTemplateList);
			WOEURL url = new WOEURL("/v1/template/list");
			return Request.Get(url, "TEMPLATE_LIST", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC Get(string _id, Action<PsMinigameMetaData> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/template/find");
			ResponseHandler<PsMinigameMetaData> responseHandler = new ResponseHandler<PsMinigameMetaData>(_okCallback, ClientTools.ParseTemplate);
			wOEURL.AddParameter("id", _id);
			HttpC httpC = Request.Get(wOEURL, "TEMPLATE_FIND", responseHandler.RequestOk, _failureCallback, _errorCallback);
			httpC.objectData = _id;
			return httpC;
		}

		public static HttpC Save(string _id, Action<PsMinigameMetaData> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			DataBlob dataBlob = ClientTools.CreateLevelDataBlob(null);
			WOEURL wOEURL = new WOEURL("/v1/template/save");
			ResponseHandler<PsMinigameMetaData> responseHandler = new ResponseHandler<PsMinigameMetaData>(_okCallback, ClientTools.ParseTemplate);
			if (!string.IsNullOrEmpty(_id))
			{
				wOEURL.AddParameter("id", _id);
			}
			return Request.Post(wOEURL, null, dataBlob.data, responseHandler.RequestOk, _failureCallback, _errorCallback, dataBlob.header);
		}

		public static HttpC Duplicate(string _templateId, Hashtable _metaChanges, Action<PsMinigameMetaData> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/template/duplicate");
			ResponseHandler<PsMinigameMetaData> responseHandler = new ResponseHandler<PsMinigameMetaData>(_okCallback, ClientTools.ParseTemplate);
			wOEURL.AddParameter("templateId", _templateId);
			string json = ClientTools.GenerateJson(_metaChanges);
			return Request.Post(wOEURL, "TEMPLATE_FIND", json, responseHandler.RequestOk, _failureCallback, _errorCallback);
		}
	}
}
