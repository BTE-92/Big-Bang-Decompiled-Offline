using System;
using System.Collections;

public class URL
{
	protected Hashtable data = new Hashtable();

	protected string host = string.Empty;

	protected string command = string.Empty;

	public URL()
	{
		AddParameter("ts", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
	}

	public void AddParameter(object _key, object _value)
	{
		data.Add(_key, _value);
	}

	public string ConstructURL()
	{
		return host + ConstructURI();
	}

	public string ConstructURI()
	{
		string text = command;
		int num = 0;
		foreach (string key in data.Keys)
		{
			string text3 = text;
			text = text3 + ((num != 0) ? "&" : "?") + key + "=" + data[key];
			num++;
		}
		return text;
	}
}
