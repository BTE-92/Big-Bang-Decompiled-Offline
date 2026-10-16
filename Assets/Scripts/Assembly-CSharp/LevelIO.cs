using System;
using System.Collections.Generic;
using System.IO;

public static class LevelIO
{
	public static string GetLevelPath()
	{
		string text = Main.GetApplicationDataPath() + "/" + Main.m_currentGame.m_projectCode + "/Resources/" + Main.m_currentGame.m_projectCode + "/Levels";
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		return text;
	}

	public static List<string> GetLevels()
	{
		List<string> list = new List<string>();
		string[] files = Directory.GetFiles(GetLevelPath());
		string[] separator = new string[3] { "/", "\\", "." };
		for (int i = 0; i < files.Length; i++)
		{
			string[] array = files[i].Split(separator, StringSplitOptions.None);
			if (array[array.Length - 1] != "meta")
			{
				string item = array[array.Length - 2];
				list.Add(item);
			}
		}
		return list;
	}

	public static bool LevelExists(string _levelFile)
	{
		List<string> levels = GetLevels();
		for (int i = 0; i < levels.Count; i++)
		{
			if (levels[i] == _levelFile)
			{
				return true;
			}
		}
		return false;
	}
}
