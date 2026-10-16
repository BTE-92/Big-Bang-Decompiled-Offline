using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MiniJSON;

public static class PsPlanetSerializer
{
	public static void SaveProgressionLocally(PsPlanet _planet)
	{
		string text = "_" + _planet.m_identifier;
		List<PsPlanetPath> list = new List<PsPlanetPath>();
		list.Add(_planet.m_floaterManager.m_floatingPath);
		string value = Json.Serialize(ClientTools.GenerateProgressionPathJson(_planet.m_mainPath.m_nodeInfos, _planet.m_mainPath.m_currentNodeId, true, true, true, list));
		string path = GetPathString() + "/" + PlayerPrefsX.GetUserId() + text + ".txt";
		StreamWriter streamWriter = File.CreateText(path);
		streamWriter.WriteLine(value);
		streamWriter.Close();
	}

	public static PsPlanetPath LoadProgression(PsPlanet _planet, string planetPath)
	{
		string text = "_" + _planet.m_identifier;
		string text2 = GetPathString() + "/" + PlayerPrefsX.GetUserId() + text + ".txt";
		if (File.Exists(text2))
		{
			StreamReader streamReader = File.OpenText(text2);
			string text3 = string.Empty;
			for (string text4 = streamReader.ReadLine(); text4 != null; text4 = streamReader.ReadLine())
			{
				text3 += text4;
			}
			streamReader.Close();
			Dictionary<string, object> dictionary = Json.Deserialize(text3) as Dictionary<string, object>;
			List<PsPlanetPath> list = ClientTools.ParseProgressionPathData(dictionary["paths"] as List<object>);
			Debug.Log("Searching for: " + planetPath);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].m_name == planetPath)
				{
					Debug.LogWarning(planetPath + "was found from local progression file.");
					return list[i];
				}
			}
			Debug.Log(planetPath + " not found from local progression file.");
			return null;
		}
		Debug.LogWarning("NO LOCAL PROGRESSION FILE FOUND FROM: " + text2);
		return null;
	}

	public static string GetPathString()
	{
		string text = Main.GetPersistentDataPath() + "/ProgressionData";
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		return text;
	}

	public static bool FileExists(string _path)
	{
		return File.Exists(_path);
	}

	public static void SerializePlanetPathToFile(string _file, PsPlanetPath _pathData)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		Stream stream = File.Open(_file, FileMode.Create);
		if (stream != null)
		{
			binaryFormatter.Serialize(stream, _pathData);
			stream.Close();
		}
	}

	public static PsPlanetPath DeSerializeLevelFromFile(string _path)
	{
		Debug.Log("Deserializing path...");
		if (FileExists(_path))
		{
			PsPlanetPath result = null;
			try
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				Stream stream = File.Open(_path, FileMode.Open);
				result = (PsPlanetPath)binaryFormatter.Deserialize(stream);
				stream.Close();
			}
			catch
			{
			}
			return result;
		}
		Debug.LogWarning("File not found: " + _path);
		return null;
	}
}
