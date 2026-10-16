using System.Collections.Generic;
using SimpleJSON;

namespace com.adjust.sdk
{
	public class AdjustAttribution
	{
		public string trackerToken { get; set; }

		public string trackerName { get; set; }

		public string network { get; set; }

		public string campaign { get; set; }

		public string adgroup { get; set; }

		public string creative { get; set; }

		public string clickLabel { get; set; }

		public AdjustAttribution()
		{
		}

		public AdjustAttribution(string jsonString)
		{
			JSONNode jSONNode = JSON.Parse(jsonString);
			if (!(jSONNode == null))
			{
				trackerName = getJsonString(jSONNode, "trackerName");
				trackerToken = getJsonString(jSONNode, "trackerToken");
				network = getJsonString(jSONNode, "network");
				campaign = getJsonString(jSONNode, "campaign");
				adgroup = getJsonString(jSONNode, "adgroup");
				creative = getJsonString(jSONNode, "creative");
				clickLabel = getJsonString(jSONNode, "clickLabel");
			}
		}

		public AdjustAttribution(Dictionary<string, string> dicAttributionData)
		{
			if (dicAttributionData != null)
			{
				trackerName = TryGetValue(dicAttributionData, "trackerName");
				trackerToken = TryGetValue(dicAttributionData, "trackerToken");
				network = TryGetValue(dicAttributionData, "network");
				campaign = TryGetValue(dicAttributionData, "campaign");
				adgroup = TryGetValue(dicAttributionData, "adgroup");
				creative = TryGetValue(dicAttributionData, "creative");
				clickLabel = TryGetValue(dicAttributionData, "clickLabel");
			}
		}

		private static string TryGetValue(Dictionary<string, string> dic, string key)
		{
			string value;
			if (dic.TryGetValue(key, out value))
			{
				return value;
			}
			return null;
		}

		private string getJsonString(JSONNode node, string key)
		{
			JSONNode jsonValue = getJsonValue(node, key);
			if (jsonValue == null)
			{
				return null;
			}
			return jsonValue.Value;
		}

		private JSONNode getJsonValue(JSONNode node, string key)
		{
			if (node == null)
			{
				return null;
			}
			JSONNode jSONNode = node[key];
			if (jSONNode.GetType() == typeof(JSONLazyCreator))
			{
				return null;
			}
			return jSONNode;
		}
	}
}
