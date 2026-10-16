using System;
using System.Collections;
using MiniJSON;

namespace Server
{
	public static class Customisation
	{
		public static HttpC Add(Hashtable _offroadCarVisual, Hashtable _offroadCarUpgrade, Hashtable _motorcycleVisual, Hashtable _motorcycleUpgrade, Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v1/customisation/add");
			Hashtable hashtable = new Hashtable();
			if (_offroadCarVisual != null)
			{
				hashtable.Add("OffroadCarVisual", _offroadCarVisual);
			}
			if (_offroadCarUpgrade != null)
			{
				hashtable.Add("OffroadCarUpgrade", _offroadCarUpgrade);
			}
			if (_motorcycleVisual != null)
			{
				hashtable.Add("MotorcycleVisual", _motorcycleVisual);
			}
			if (_motorcycleUpgrade != null)
			{
				hashtable.Add("MotorcycleUpgrade", _motorcycleUpgrade);
			}
			string json = Json.Serialize(hashtable);
			return Request.Post(url, "ADD_CUSTOMISATION", json, _okCallback, _failureCallback, _errorCallback);
		}
	}
}
