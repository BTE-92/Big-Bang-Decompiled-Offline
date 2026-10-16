using System;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

public class Achievement
{
	protected string m_identifier;

	protected string m_name;

	protected string m_description;

	private bool m_completed;

	private bool m_hidden;

	private int m_percentCompleted;

	protected int m_value;

	protected int m_target;

	public string id
	{
		get
		{
			return m_identifier;
		}
		set
		{
			m_identifier = value;
		}
	}

	public string humanReadableName
	{
		get
		{
			return m_name;
		}
		set
		{
			m_name = value;
		}
	}

	public string humanReadableDescription
	{
		get
		{
			return m_description;
		}
		set
		{
			m_description = value;
		}
	}

	public bool completed
	{
		get
		{
			CheckFlags();
			return m_completed;
		}
	}

	public bool hidden
	{
		get
		{
			CheckFlags();
			return m_hidden;
		}
	}

	public int percentCompleted
	{
		get
		{
			return m_percentCompleted;
		}
		set
		{
			m_percentCompleted = Math.Min(value, 100);
			CheckFlags();
		}
	}

	public int targetValue
	{
		get
		{
			return m_target;
		}
	}

	public int currentValue
	{
		get
		{
			return m_value;
		}
	}

	public Achievement(string _identifier, string _name = null, string _description = null, int _target = 1)
	{
		m_identifier = _identifier;
		m_name = _name;
		m_description = _description;
		m_value = 0;
		m_target = _target;
		m_percentCompleted = 0;
		CheckFlags();
	}

	public Achievement(Dictionary<string, object> _dictionary)
	{
		FromDictionary(_dictionary);
	}

	protected void CheckFlags()
	{
		if (m_percentCompleted >= 0)
		{
			m_hidden = false;
		}
		else
		{
			m_hidden = true;
		}
		if (m_percentCompleted == 100)
		{
			m_completed = true;
		}
		else
		{
			m_completed = false;
		}
	}

	public virtual void Refresh()
	{
		m_percentCompleted = Mathf.FloorToInt((float)m_value / (float)m_target * 100f);
		CheckFlags();
	}

	public virtual void IncrementProgress(int _value)
	{
		if (!completed)
		{
			m_value += _value;
			m_percentCompleted = Mathf.FloorToInt((float)m_value / (float)m_target * 100f);
			CheckFlags();
			if (m_percentCompleted >= 100)
			{
				Complete();
			}
			ReportProgress();
		}
	}

	public void ReportProgress()
	{
	}

	public void UpdateProgress(int _progressValue)
	{
		m_value = _progressValue;
		Refresh();
	}

	public void Complete()
	{
		m_value = m_target;
		m_percentCompleted = 100;
		m_hidden = false;
		m_completed = true;
	}

	public string ToJson()
	{
		return Json.Serialize(ToDict());
	}

	public Dictionary<string, object> ToDict()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("id", m_identifier);
		dictionary.Add("name", m_name);
		dictionary.Add("description", m_description);
		dictionary.Add("value", m_value);
		dictionary.Add("target", m_target);
		return dictionary;
	}

	private void FromDictionary(Dictionary<string, object> _dictionary)
	{
		if (!_dictionary.ContainsKey("id") && !_dictionary.ContainsKey("name") && !_dictionary.ContainsKey("description") && !_dictionary.ContainsKey("value") && !_dictionary.ContainsKey("target"))
		{
			Debug.LogError("Invalid dictionary");
			return;
		}
		m_identifier = (string)_dictionary["id"];
		m_name = (string)_dictionary["name"];
		m_description = (string)_dictionary["description"];
		m_value = Convert.ToInt32(_dictionary["value"]);
		m_target = Convert.ToInt32(_dictionary["target"]);
		CheckFlags();
	}
}
