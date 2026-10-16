using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MiniJSON;
using Server;
using UnityEngine;

public class GeLoadState : BasicState
{
	private GeScene m_parent;

	private UICanvas m_canvas;

	private bool m_localFile;

	public GeLoadState(bool _fromLocalFile = false)
	{
		m_localFile = _fromLocalFile;
	}

	public override void Enter(IStatedObject _parent)
	{
		m_parent = _parent as GeScene;
		m_canvas = new UICanvas(null, false, string.Empty, null, string.Empty);
		m_canvas.RemoveDrawHandler();
		UIText uIText = new UIText(m_canvas, false, string.Empty, "Loading...", "HurmeSemiBold_Font", 0.05f, RelativeTo.ScreenHeight);
		m_canvas.Update();
		if (!m_localFile)
		{
			Server.Path.GetInitial(LoadOk, LoadFail);
		}
		else
		{
			LoadLocal();
		}
	}

	private void LoadLocal()
	{
		Dictionary<string, object> dictionary = LoadLocalInitialData();
		if (dictionary != null)
		{
			ParseData(dictionary);
		}
		m_parent.m_stateMachine.ChangeState(new GeBaseState());
	}

	public static Dictionary<string, object> LoadLocalInitialData()
	{
		string empty = string.Empty;
		empty = Main.GetPersistentDataPath() + "/LocalInitialData.txt";
		if (File.Exists(empty))
		{
			Debug.LogWarning("LOADING LOCAL INITIAL PATH FILE: " + empty);
			StreamReader streamReader = File.OpenText(empty);
			string text = string.Empty;
			for (string text2 = streamReader.ReadLine(); text2 != null; text2 = streamReader.ReadLine())
			{
				text += text2;
			}
			return Json.Deserialize(text) as Dictionary<string, object>;
		}
		Debug.LogWarning("NO LOCAL INITIAL PATH FILE: " + empty);
		return null;
	}

	private void LoadOk(HttpC _c)
	{
		string response = Encoding.UTF8.GetString(FilePacker.UnZipBytes(_c.www.bytes));
		Dictionary<string, object> dataDict = ClientTools.ParseServerResponse(response);
		ParseData(dataDict);
		m_parent.m_stateMachine.ChangeState(new GeBaseState());
	}

	private void LoadFail(HttpC _c)
	{
		Server.Path.GetInitial(LoadOk, LoadFail);
	}

	public override void Execute()
	{
	}

	public override void Exit()
	{
		m_canvas.Destroy();
	}

	private void ParseData(Dictionary<string, object> _dataDict)
	{
		Debug.Log("PARSE PATH DATA");
		Dictionary<int, PsMetagameDataHelper> dictionary = new Dictionary<int, PsMetagameDataHelper>();
		if (_dataDict.ContainsKey("nodes"))
		{
			List<object> list = _dataDict["nodes"] as List<object>;
			for (int i = 0; i < list.Count; i++)
			{
				Dictionary<string, object> dictionary2 = list[i] as Dictionary<string, object>;
				int num = Convert.ToInt32(dictionary2["id"]);
				string empty = string.Empty;
				try
				{
					empty = (string)dictionary2["name"];
				}
				catch
				{
					empty = string.Empty;
				}
				float x = Convert.ToSingle(dictionary2["x"]);
				float y = Convert.ToSingle(dictionary2["y"]);
				Dictionary<string, object> dataDict = dictionary2["data"] as Dictionary<string, object>;
				MetagameNodeData metagameNodeData = ClientTools.ParseMetagameNodeDataJSON(dataDict);
				PsMetagameDataHelper psMetagameDataHelper = new PsMetagameDataHelper();
				psMetagameDataHelper.m_id = num;
				psMetagameDataHelper.m_name = empty;
				psMetagameDataHelper.m_x = x;
				psMetagameDataHelper.m_y = y;
				psMetagameDataHelper.m_metagameNodeData = metagameNodeData;
				dictionary.Add(num, psMetagameDataHelper);
			}
		}
		LevelManager.NewLevel(10000, 10000);
		LevelManager.m_currentLevel.m_currentLayer.m_disabledAtStart = false;
		foreach (KeyValuePair<int, PsMetagameDataHelper> item in dictionary)
		{
			PsMetagameDataHelper value = item.Value;
			GeNode geNode = new GeNode(value.m_name, new Vector2(value.m_x, value.m_y));
			geNode.m_parsingHelperNodeId = value.m_id;
			geNode.m_metagameNodeData = value.m_metagameNodeData;
			LevelManager.m_currentLevel.m_currentLayer.AddElement(geNode);
		}
		if (_dataDict.ContainsKey("edges"))
		{
			List<object> list2 = _dataDict["edges"] as List<object>;
			for (int j = 0; j < list2.Count; j++)
			{
				Dictionary<string, object> dictionary3 = list2[j] as Dictionary<string, object>;
				int num2 = Convert.ToInt32(dictionary3["source"]);
				int num3 = Convert.ToInt32(dictionary3["target"]);
				PsMetagameDataHelper psMetagameDataHelper2 = dictionary[num2];
				PsMetagameDataHelper psMetagameDataHelper3 = dictionary[num3];
				psMetagameDataHelper2.m_outputs.Add(psMetagameDataHelper3);
				psMetagameDataHelper3.m_inputs.Add(psMetagameDataHelper2);
				GeNode elementByHelperId = GetElementByHelperId(num2);
				GeNode elementByHelperId2 = GetElementByHelperId(num3);
				GraphConnection element = new GeConnection(elementByHelperId, elementByHelperId2);
				LevelManager.m_currentLevel.m_currentLayer.AddElement(element);
			}
		}
		LevelManager.AssembleCurrentLevel();
	}

	public GeNode GetElementByHelperId(int _id)
	{
		List<GraphElement> childElements = LevelManager.m_currentLevel.m_currentLayer.m_childElements;
		for (int i = 0; i < childElements.Count; i++)
		{
			if (childElements[i].m_elementType == GraphElementType.Node && (childElements[i] as GeNode).m_parsingHelperNodeId == _id)
			{
				return (GeNode)childElements[i];
			}
		}
		return null;
	}
}
