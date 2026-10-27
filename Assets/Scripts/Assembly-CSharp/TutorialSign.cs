using System;
using UnityEngine;

public class TutorialSign : Unit
{
	private int m_collidingCount;

	private ChipmunkBodyC m_cmb;

	private string m_oldText;

	private TextModel m_model;

	private UIVerticalList m_bubbleUI;

	private float m_timerSeconds = 4f;

	private float m_currentTime = -1f;

	public TutorialSign(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.TutorialSign_GameObject);
		TransformC transformC = TransformS.AddComponent(m_entity, "ArrowSign");
		TransformS.SetTransform(transformC, _graphElement.m_position + new Vector3(0f, -0f, 100f) + GetZBufferBias(), _graphElement.m_rotation);
		PrefabS.AddComponent(transformC, Vector3.zero, gameObject);
		ucpCircleShape ucpCircleShape2 = new ucpCircleShape(100f, Vector2.zero, 1118208u, 0f, 0f, 0f, (ucpCollisionType)8, true);
		m_cmb = ChipmunkProS.AddKinematicBody(transformC, new ucpCircleShape[1] { ucpCircleShape2 }, m_unitC);
		if (!m_minigame.m_editing)
		{
			ChipmunkProS.AddCollisionHandler(m_cmb, TutorialSignCollisionHandler, (ucpCollisionType)8, (ucpCollisionType)3, true, false, true);
			CameraTargetC cameraTargetC = CameraS.AddTargetComponent(transformC, 150f, 150f, new Vector2(0f, 0f));
			cameraTargetC.activeRadius = 500f;
		}
		CreateEditorTouchArea(50f, 50f, transformC);
		m_graphElement.m_isRotateable = true;
		m_graphElement.m_isFlippable = true;
	}

	private void TutorialSignCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		switch (_phase)
		{
		case ucpCollisionPhase.Begin:
			m_collidingCount++;
			if (m_collidingCount == 1 && m_bubbleUI == null)
			{
				if (PsState.m_activeMinigame.m_currentSign != null)
				{
					PsState.m_activeMinigame.m_currentSign.DestroySign();
				}
				PsState.m_activeMinigame.m_currentSign = this;
				Debug.Log("START");
				string text = ColorMarkup((m_graphElement as LevelTextNode).m_text);
				m_bubbleUI = new UIVerticalList(null, "ArrowSign Bubble");
				m_bubbleUI.SetDrawHandler(BubbleDrawHandler);
				m_bubbleUI.SetMargins(0.05f, 0.05f, 0.025f, 0.025f);
				UITextbox uITextbox = new UITextbox(m_bubbleUI, false, string.Empty, text, "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, false, Align.Center, Align.Middle);
				uITextbox.SetWidth(0.3f, RelativeTo.ScreenHeight);
				uITextbox.SetDepthOffset(8f);
				m_bubbleUI.Update();
				m_currentTime = m_timerSeconds;
			}
			break;
		case ucpCollisionPhase.Separate:
			if (m_collidingCount == 1)
			{
				Debug.Log("Not colliding anymore, time left: " + m_currentTime);
			}
			m_collidingCount--;
			break;
		}
	}

	// Colours for the text of a sign. Besides the <color=#rrggbb>...</color> tags the text boxes already understand, a sign can use
	//   [red]text[/red]   [#ff8800]text[/]   [green]text[/]
	// (names: red, orange, yellow, green, cyan, blue, purple, pink, white, grey). "[/]" or "[/name]" ends the colour.
	private static string ColorMarkup(string _text)
	{
		if (string.IsNullOrEmpty(_text) || _text.IndexOf('[') < 0)
		{
			return _text;
		}
		System.Text.StringBuilder sb = new System.Text.StringBuilder(_text.Length + 32);
		int open = 0;
		int i = 0;
		while (i < _text.Length)
		{
			char c = _text[i];
			if (c == '[')
			{
				int end = _text.IndexOf(']', i + 1);
				if (end > i)
				{
					string tag = _text.Substring(i + 1, end - i - 1);
					if (tag.StartsWith("/") && (tag.Length == 1 || SignColor(tag.Substring(1)) != null) && open > 0)
					{
						sb.Append("</color>");
						open--;
						i = end + 1;
						continue;
					}
					string hex = SignColor(tag);
					if (hex != null)
					{
						sb.Append("<color=#").Append(hex).Append('>');
						open++;
						i = end + 1;
						continue;
					}
				}
			}
			sb.Append(c);
			i++;
		}
		while (open-- > 0)
		{
			sb.Append("</color>");
		}
		return sb.ToString();
	}

	private static string SignColor(string _name)
	{
		string n = _name.Trim().ToLower();
		if (n.StartsWith("#") && (n.Length == 7 || n.Length == 4))
		{
			for (int k = 1; k < n.Length; k++)
			{
				if (!Uri.IsHexDigit(n[k]))
				{
					return null;
				}
			}
			return n.Substring(1);
		}
		switch (n)
		{
		case "red":
			return "ff4d4d";
		case "orange":
			return "ffa11a";
		case "yellow":
			return "fbdd28";
		case "green":
			return "2efe9a";
		case "cyan":
			return "46fff1";
		case "blue":
			return "2aaded";
		case "purple":
			return "a56bff";
		case "pink":
			return "f15bfb";
		case "white":
			return "ffffff";
		case "grey":
		case "gray":
			return "a8a8a8";
		default:
			return null;
		}
	}

	private void BubbleDrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_points: new Vector2[7]
		{
			new Vector2(_c.m_actualWidth * -0.5f, _c.m_actualHeight * 0.5f),
			new Vector2(_c.m_actualWidth * 0.5f, _c.m_actualHeight * 0.5f),
			new Vector2(_c.m_actualWidth * 0.5f, _c.m_actualHeight * -0.5f),
			new Vector2(_c.m_actualWidth * 0.025f, _c.m_actualHeight * -0.5f),
			new Vector2(_c.m_actualWidth * 0f, _c.m_actualHeight * -0.5f - _c.m_actualWidth * 0.05f),
			new Vector2(_c.m_actualWidth * -0.025f, _c.m_actualHeight * -0.5f),
			new Vector2(_c.m_actualWidth * -0.5f, _c.m_actualHeight * -0.5f)
		}, _tc: _c.m_TC, _offset: Vector3.forward * 10f, _bottomColor: DebugDraw.HexToUint("00d4c8"), _topColor: DebugDraw.HexToUint("00d4c8"), _material: ResourceManager.GetMaterial("Framework/SolidMat"), _camera: CameraS.m_uiCamera, _identifier: string.Empty);
	}

	public override void Update()
	{
		base.Update();
		if (m_currentTime != -1f)
		{
			m_currentTime -= Main.m_gameDeltaTime;
			if (m_currentTime <= 0f && m_bubbleUI != null && m_collidingCount < 1)
			{
				DestroySign();
				PsState.m_activeMinigame.m_currentSign = null;
			}
		}
		if (m_bubbleUI != null)
		{
			float z = 50f;
			Vector3 position = CameraS.m_mainCamera.WorldToScreenPoint(m_minigame.m_playerTC.transform.position);
			Vector3 vector = CameraS.m_uiCamera.ScreenToWorldPoint(position);
			Vector3 vector2 = vector + Vector3.up * ((float)Screen.height * 0.2f + m_bubbleUI.m_actualHeight * 0.5f);
			m_bubbleUI.m_TC.transform.position = new Vector3(vector2.x, vector2.y, z);
		}
		if (m_graphElement.m_flipped)
		{
			m_graphElement.m_flipped = false;
			m_oldText = (m_graphElement as LevelTextNode).m_text;
			m_model = new TextModel((m_graphElement as LevelTextNode).m_text);
			PsUICenterTutorialSignTextInput psUICenterTutorialSignTextInput = new PsUICenterTutorialSignTextInput(m_model);
			psUICenterTutorialSignTextInput.Update();
		}
		if (m_model != null)
		{
			if (m_model.m_done && m_oldText != m_model.m_text)
			{
				Debug.Log(m_model.m_text);
				m_oldText = m_model.m_text;
				(m_graphElement as LevelTextNode).m_text = m_model.m_text;
				m_model = null;
			}
			else if (m_model.m_cancelled)
			{
				m_model = null;
			}
		}
	}

	public void Hide()
	{
		if (m_bubbleUI.m_TC.transform.gameObject.activeSelf)
		{
			m_bubbleUI.m_TC.transform.gameObject.SetActive(false);
		}
	}

	public void Show()
	{
		if (!m_bubbleUI.m_TC.transform.gameObject.activeSelf)
		{
			m_bubbleUI.m_TC.transform.gameObject.SetActive(true);
		}
	}

	public void DestroySign()
	{
		if (m_bubbleUI != null)
		{
			m_bubbleUI.Destroy();
			m_bubbleUI = null;
			m_currentTime = -1f;
		}
	}

	public override void Destroy()
	{
		DestroySign();
		base.Destroy();
	}
}
