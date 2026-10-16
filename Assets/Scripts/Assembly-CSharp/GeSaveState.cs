using System.Collections;
using System.Collections.Generic;
using System.IO;
using MiniJSON;
using Server;

public class GeSaveState : BasicState
{
	private GeScene m_parent;

	private UICanvas m_canvas;

	private bool m_localFile;

	public GeSaveState(bool _toLocalFile = false)
	{
		m_localFile = _toLocalFile;
	}

	public override void Enter(IStatedObject _parent)
	{
		m_parent = _parent as GeScene;
		m_canvas = new UICanvas(null, false, string.Empty, null, string.Empty);
		m_canvas.RemoveDrawHandler();
		UIText uIText = new UIText(m_canvas, false, string.Empty, "Saving...", "HurmeSemiBold_Font", 0.05f, RelativeTo.ScreenHeight);
		m_canvas.Update();
		string pathJson = CreateInitialNodeData();
		string itemLevelJson = CreateItemLevelData();
		if (!m_localFile)
		{
			Server.Path.SaveInitialWithItemLevelData(itemLevelJson, pathJson, SaveOk, SaveFail, SaveError);
		}
		else
		{
			SaveLocal();
		}
	}

	private void SaveLocal()
	{
		SaveLocalInitialData(CreateInitialNodeData());
		m_parent.m_stateMachine.ChangeState(new GeBaseState());
	}

	private static void SaveLocalInitialData(string _json)
	{
		string empty = string.Empty;
		empty = Main.GetPersistentDataPath() + "/LocalInitialData.txt";
		StreamWriter streamWriter = File.CreateText(empty);
		streamWriter.WriteLine(_json);
		streamWriter.Close();
	}

	private void SaveError()
	{
		Debug.LogError("Error saving path!");
	}

	private void SaveOk(HttpC _c)
	{
		SaveLocal();
	}

	private void SaveFail(HttpC _c)
	{
		string pathJson = CreateInitialNodeData();
		string itemLevelJson = CreateItemLevelData();
		Server.Path.SaveInitialWithItemLevelData(itemLevelJson, pathJson, SaveOk, SaveFail);
	}

	public override void Execute()
	{
	}

	public override void Exit()
	{
		m_canvas.Destroy();
	}

	private string CreateInitialNodeData()
	{
		Hashtable hashtable = new Hashtable();
		List<Hashtable> list = new List<Hashtable>();
		hashtable.Add("nodes", list);
		List<Hashtable> list2 = new List<Hashtable>();
		hashtable.Add("edges", list2);
		List<GraphElement> childElements = LevelManager.m_currentLevel.m_currentLayer.m_childElements;
		for (int i = 0; i < childElements.Count; i++)
		{
			Hashtable hashtable2 = new Hashtable();
			if (childElements[i].m_elementType == GraphElementType.Node)
			{
				GeNode geNode = childElements[i] as GeNode;
				list.Add(hashtable2);
				hashtable2.Add("id", geNode.m_id);
				hashtable2.Add("name", geNode.m_name);
				hashtable2.Add("x", geNode.m_position.x);
				hashtable2.Add("y", geNode.m_position.y);
				Hashtable value = ClientTools.CreateMetagameNodeDataJSON(geNode.m_metagameNodeData);
				hashtable2.Add("data", value);
			}
			else if (childElements[i].m_elementType == GraphElementType.Connection)
			{
				GeConnection geConnection = childElements[i] as GeConnection;
				list2.Add(hashtable2);
				hashtable2.Add("source", geConnection.m_start.m_id);
				hashtable2.Add("target", geConnection.m_end.m_id);
			}
		}
		string text = Json.Serialize(hashtable);
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogError("EMPTY JSON CREATED FROM NODE DATA!");
		}
		Debug.Log("INITIAL JSON " + text);
		return text;
	}

	public static string CreateItemLevelData()
	{
		Hashtable hashtable = new Hashtable();
		List<GeNode> list = new List<GeNode>(GetNodes(MetagameNodeDataType.UnlockableCategory));
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].m_name != "Menu")
			{
				for (int j = 0; j < list[i].m_outputs.Count; j++)
				{
					GeNode geNode = list[i].m_outputs[j].m_end as GeNode;
					hashtable.Add(geNode.m_metagameNodeData.m_dataIdentifier, geNode.m_metagameNodeData.m_unlockableItemLevel);
				}
			}
		}
		string text = Json.Serialize(hashtable);
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogError("EMPTY JSON CREATED FROM UNLOCKABLE NODES!");
		}
		Debug.Log("ITEM LEVEL JSON " + text);
		return text;
	}

	public static GeNode[] GetNodes(MetagameNodeDataType _geNodeType)
	{
		List<GeNode> list = new List<GeNode>();
		for (int i = 0; i < LevelManager.m_currentLevel.m_currentLayer.m_childElements.Count; i++)
		{
			GraphElement graphElement = LevelManager.m_currentLevel.m_currentLayer.m_childElements[i];
			if (graphElement.m_elementType == GraphElementType.Node)
			{
				GeNode geNode = graphElement as GeNode;
				if (geNode.m_metagameNodeData.m_nodeDataType == _geNodeType)
				{
					list.Add(geNode);
				}
			}
		}
		return list.ToArray();
	}
}
