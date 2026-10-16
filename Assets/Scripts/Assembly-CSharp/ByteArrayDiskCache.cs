using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ByteArrayDiskCache
{
	private string m_name;

	private int m_maxItems;

	public string m_directory;

	private List<string> m_fileList;

	public ByteArrayDiskCache(string _name, int _maxItems = int.MaxValue)
	{
		m_name = _name;
		m_maxItems = _maxItems;
		m_fileList = new List<string>();
		m_directory = Application.persistentDataPath + "/Cache/" + m_name + "/";
		if (!Directory.Exists(m_directory))
		{
			Directory.CreateDirectory(m_directory);
			Debug.Log("Creating dir for diskCache: " + m_directory);
		}
		ReadFileList();
		Debug.Log("Disk cache initialized: " + m_directory + " -- existing items: " + m_fileList.Count);
	}

	public string GetCachePath()
	{
		return m_directory;
	}

	public void SaveFileList()
	{
		string path = GetCachePath() + "filelist.txt";
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		StreamWriter streamWriter = File.CreateText(path);
		foreach (string file in m_fileList)
		{
			streamWriter.WriteLine(file);
		}
		streamWriter.Close();
	}

	private void ReadFileList()
	{
		string path = GetCachePath() + "filelist.txt";
		if (File.Exists(path))
		{
			StreamReader streamReader = File.OpenText(path);
			m_fileList.Clear();
			while (!streamReader.EndOfStream)
			{
				string item = streamReader.ReadLine();
				m_fileList.Add(item);
			}
			streamReader.Close();
		}
	}

	public int GetItemCount()
	{
		return m_fileList.Count;
	}

	public void AddItem(string _key, byte[] _content)
	{
		if (_content == null || _content.Length == 0)
		{
			Debug.LogWarning("Cannot assign null or empty value to cache");
			return;
		}
		string text = GetCachePath() + _key;
		if (m_fileList.Contains(_key))
		{
			if (File.Exists(text))
			{
				File.Delete(text);
			}
		}
		else
		{
			if (m_fileList.Count >= m_maxItems)
			{
				string text2 = m_fileList[0];
				string path = GetCachePath() + text2;
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				m_fileList.Remove(text2);
			}
			m_fileList.Add(_key);
			SaveFileList();
		}
		FileStream fileStream = File.Create(text);
		if (fileStream != null)
		{
			fileStream.Write(_content, 0, _content.Length);
			fileStream.Close();
		}
		else
		{
			Debug.LogError("Cannot create file: " + text);
		}
	}

	public byte[] GetContent(string _key)
	{
		if (m_fileList.Contains(_key))
		{
			string text = GetCachePath() + _key;
			FileStream fileStream = File.Open(text, FileMode.Open);
			if (fileStream != null)
			{
				byte[] array = new byte[(int)fileStream.Length];
				fileStream.Read(array, 0, (int)fileStream.Length);
				fileStream.Close();
				return array;
			}
			Debug.LogError("DiskCache file " + text + " does not exist!");
		}
		return null;
	}

	public void RemoveItem(string _key)
	{
		if (m_fileList.Contains(_key))
		{
			string path = GetCachePath() + _key;
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			m_fileList.Remove(_key);
			SaveFileList();
		}
	}

	public void Clear()
	{
		string path = GetCachePath() + "filelist.txt";
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		m_fileList.Clear();
		string cachePath = GetCachePath();
		if (Directory.Exists(cachePath))
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(cachePath);
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				fileInfo.Delete();
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				directoryInfo2.Delete(true);
			}
		}
		Debug.Log("Disk cache cleared: " + cachePath);
	}
}
