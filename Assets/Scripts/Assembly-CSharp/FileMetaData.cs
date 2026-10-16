using System;
using System.Collections.Generic;
using MiniJSON;

public class FileMetaData
{
	public string name;

	public string type;

	public string path;

	public int version;

	public FileMetaData()
	{
		name = string.Empty;
		type = string.Empty;
		path = string.Empty;
		version = -1;
	}

	public FileMetaData(string _json)
	{
		ParseFromJSON(_json);
	}

	public FileMetaData(Dictionary<string, object> _dict)
	{
		ParseFromDictionary(_dict);
	}

	public void ParseFromJSON(string _json)
	{
		ParseFromDictionary(Json.Deserialize(_json) as Dictionary<string, object>);
	}

	public void ParseFromDictionary(Dictionary<string, object> _dict)
	{
		if (_dict.ContainsKey("name"))
		{
			name = (string)_dict["name"];
		}
		if (_dict.ContainsKey("type"))
		{
			type = (string)_dict["type"];
		}
		if (_dict.ContainsKey("path"))
		{
			path = (string)_dict["path"];
		}
		if (_dict.ContainsKey("version"))
		{
			version = Convert.ToInt32(_dict["version"]);
		}
	}
}
