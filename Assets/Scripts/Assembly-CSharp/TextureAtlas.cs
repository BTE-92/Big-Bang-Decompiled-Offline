using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

public class TextureAtlas
{
	private struct AtlasFrame
	{
		public string name;

		public bool rotated;

		public bool trimmed;

		public Rect rect;
	}

	private Hashtable m_frames = new Hashtable();

	public TextureAtlas(TextAsset _JSON)
	{
		string text = _JSON.text;
		Dictionary<string, object> dictionary = Json.Deserialize(text) as Dictionary<string, object>;
		List<object> list = dictionary["frames"] as List<object>;
		for (int i = 0; i < list.Count; i++)
		{
			AtlasFrame atlasFrame = default(AtlasFrame);
			Dictionary<string, object> dictionary2 = list[i] as Dictionary<string, object>;
			string text2 = dictionary2["filename"] as string;
			text2 = (atlasFrame.name = text2.Substring(0, text2.Length - 4));
			Dictionary<string, object> dictionary3 = dictionary2["frame"] as Dictionary<string, object>;
			atlasFrame.rect = new Rect((long)dictionary3["x"], (long)dictionary3["y"], (long)dictionary3["w"], (long)dictionary3["h"]);
			atlasFrame.rotated = (bool)dictionary2["rotated"];
			atlasFrame.trimmed = (bool)dictionary2["trimmed"];
			m_frames.Add(text2, atlasFrame);
		}
	}

	public Frame GetFrame(string frameName)
	{
		if (m_frames.ContainsKey(frameName))
		{
			AtlasFrame atlasFrame = (AtlasFrame)m_frames[frameName];
			return new Frame(atlasFrame.rect.x, atlasFrame.rect.y, atlasFrame.rect.width, atlasFrame.rect.height);
		}
		Debug.LogError("Requested frame (" + frameName + ") missing!");
		return null;
	}

	public string[] GetFrameNames(string _contains = "")
	{
		List<string> list = new List<string>();
		foreach (DictionaryEntry frame in m_frames)
		{
			if (_contains == string.Empty || frame.Key.ToString().Contains(_contains))
			{
				list.Add(frame.Key.ToString());
			}
		}
		return list.ToArray();
	}
}
