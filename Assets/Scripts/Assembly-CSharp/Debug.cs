using System.Diagnostics;
using UnityEngine;

public static class Debug
{
	public static bool m_filterLog = true;

	public static bool m_filterLogError = true;

	public static bool m_filterLogWarning = true;

	public static bool m_filterLogInfo = true;

	public static void Initialize(bool _log, bool _error, bool _warning, bool _info)
	{
		m_filterLog = _log;
		m_filterLogError = _error;
		m_filterLogWarning = _warning;
		m_filterLogInfo = _info;
	}

	public static void Log(object message)
	{
		if (m_filterLog)
		{
			UnityEngine.Debug.Log(message);
		}
	}

	public static void LogError(object message)
	{
		if (m_filterLogError)
		{
			UnityEngine.Debug.LogError(message);
		}
	}

	public static void LogWarning(object message)
	{
		if (m_filterLogWarning)
		{
			UnityEngine.Debug.LogWarning(message);
		}
	}

	public static void LogInfo(object message)
	{
		if (m_filterLogInfo)
		{
			UnityEngine.Debug.Log(message);
		}
	}

	public static void Assert(bool ok, string message = "")
	{
		if (!ok && m_filterLogError)
		{
			LogError("Error with LibTessDotNET: " + message);
		}
	}

	public static void LogStackTrace()
	{
		if (m_filterLogInfo)
		{
			StackTrace message = new StackTrace();
			Log(message);
		}
	}

	public static string GetSystemStats()
	{
		string text = "FULL FRAMEWORK STATS: \n";
		string text2 = text;
		text = text2 + "Entities: " + EntityManager.m_entities.m_aliveCount + "\n";
		text2 = text;
		text = text2 + "Transforms: " + TransformS.m_components.m_aliveCount + "\n";
		text2 = text;
		text = text2 + "Prefabs: " + PrefabS.m_components.m_aliveCount + "\n";
		int num = 0;
		int aliveCount = SpriteS.m_sheets.m_aliveCount;
		for (int i = 0; i < aliveCount; i++)
		{
			num += SpriteS.m_sheets.m_array[SpriteS.m_sheets.m_aliveIndices[i]].m_components.m_aliveCount;
		}
		text2 = text;
		text = text2 + "Sprites: " + num + "\n";
		text2 = text;
		text = text2 + "Sounds: " + SoundS.m_components.m_aliveCount + "\n";
		text2 = text;
		text = text2 + "Text meshes: " + TextMeshS.m_components.m_aliveCount + "\n";
		text2 = text;
		text = text2 + "Tweens: " + TweenS.m_components.m_aliveCount + "\n";
		text2 = text;
		text = text2 + "Projectors: " + ProjectorS.m_components.m_aliveCount + "\n";
		text2 = text;
		text = text2 + "Touch areas: " + TouchAreaS.m_areas.m_aliveCount + "\n";
		text2 = text;
		text = text2 + "Timers: " + TimerS.m_components.m_aliveCount + "\n";
		text2 = text;
		text = text2 + "Http: " + HttpS.m_components.m_aliveCount + "\n";
		text2 = text;
		text = text2 + "Chipmunk bodies: " + ChipmunkProS.m_bodies.m_aliveCount + "(" + ChipmunkProWrapper.ucpGetTotalBodyCount() + ")\n";
		text2 = text;
		text = text2 + "Chipmunk constraints: " + ChipmunkProS.m_constraints.m_aliveCount + "(" + ChipmunkProWrapper.ucpGetTotalConstraintCount() + ")\n";
		text += "\nUNITY STATS:\n";
		text2 = text;
		text = text2 + "GameObjects: " + Object.FindObjectsOfType(typeof(GameObject)).Length + "\n";
		text2 = text;
		text = text2 + "Materials: " + Object.FindObjectsOfType(typeof(Material)).Length + "\n";
		text2 = text;
		return text2 + "Meshes: " + Object.FindObjectsOfType(typeof(Mesh)).Length + "\n";
	}
}
