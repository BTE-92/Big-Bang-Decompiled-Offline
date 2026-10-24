using UnityEngine;

public static class TextMeshS
{
	public static DynamicArray<TextMeshC> m_components;

	public static GameObject m_emptyGameObject;

	public static int m_defaultCharacterSize = 10;

	public static void Initialize()
	{
		m_components = new DynamicArray<TextMeshC>();
		m_emptyGameObject = new GameObject("TextMeshSystem: InstantiateHelper");
		TextMesh textMesh = m_emptyGameObject.AddComponent<TextMesh>();
		textMesh.characterSize = m_defaultCharacterSize;
		MeshRenderer component = m_emptyGameObject.GetComponent<MeshRenderer>();
		component.enabled = false;
	}

	public static TextMeshC AddComponent(TransformC _tc, Vector3 _offset, string _fontResourcePath, float _textboxWidth, float _textboxHeight, float _fontSize, Align _horizontalAlign, Align _verticalAlign, Camera _camera, string _name = "")
	{
		TextMeshC textMeshC = m_components.AddItem();
		textMeshC.m_TC = _tc;
		textMeshC.m_go = Object.Instantiate(m_emptyGameObject);
		textMeshC.m_go.layer = _camera.gameObject.layer;
		textMeshC.m_go.transform.name = _name;
		textMeshC.m_go.transform.parent = textMeshC.m_TC.transform;
		textMeshC.m_go.transform.localPosition = _offset;
		textMeshC.m_go.transform.localRotation = Quaternion.Euler(Vector3.zero);
		Font font = ResourceManager.GetFont(_fontResourcePath);
		textMeshC.m_textMesh = textMeshC.m_go.GetComponent("TextMesh") as TextMesh;
		textMeshC.m_textMesh.font = font;
		SetFontSize(textMeshC, _fontSize);
		textMeshC.m_renderer = textMeshC.m_go.GetComponent<MeshRenderer>();
		textMeshC.m_renderer.material = font.material;
		textMeshC.m_renderer.castShadows = false;
		textMeshC.m_renderer.receiveShadows = false;
		textMeshC.m_renderer.enabled = true;
		textMeshC.m_textboxWidth = _textboxWidth;
		textMeshC.m_textboxHeight = _textboxHeight;
		textMeshC.m_horizontalAlign = _horizontalAlign;
		textMeshC.m_verticalAlign = _verticalAlign;
		textMeshC.m_wasVisible = true;
		EntityManager.AddComponentToEntity(_tc.p_entity, textMeshC);
		return textMeshC;
	}

	public static void RemoveComponent(TextMeshC _c)
	{
		if (_c.p_entity == null)
		{
			Debug.LogWarning("Trying to remove component that has already been removed");
			return;
		}
		if (_c.m_textMesh != null)
		{
			Object.Destroy(_c.m_textMesh);
		}
		if ((bool)_c.m_renderer)
		{
			Object.Destroy(_c.m_renderer.material);
		}
		if ((bool)_c.m_go)
		{
			Object.Destroy(_c.m_go);
		}
		_c.m_renderer = null;
		_c.m_textMesh = null;
		_c.m_go = null;
		_c.m_TC = null;
		_c.m_wasVisible = false;
		EntityManager.RemoveComponentFromEntity(_c);
		m_components.RemoveItem(_c);
	}

	public static void SetVisibilityByTransformComponent(TransformC _tc, bool _visible, bool _affectChildren = false, bool _affectWholeHierarchy = false)
	{
		if (_affectWholeHierarchy)
		{
			_tc = TransformS.GetRootTransformComponent(_tc);
		}
		if (_affectChildren || _affectWholeHierarchy)
		{
			for (int i = 0; i < _tc.childs.Count; i++)
			{
				SetVisibilityByTransformComponent(_tc.childs[i], _visible, true);
			}
		}
		int aliveCount = m_components.m_aliveCount;
		for (int j = 0; j < aliveCount; j++)
		{
			TextMeshC textMeshC = m_components.m_array[m_components.m_aliveIndices[j]];
			if (textMeshC.m_TC == _tc)
			{
				SetVisibility(textMeshC, _visible);
			}
		}
	}

	public static void SetAlpha(TextMeshC _c, float _alpha)
	{
		Material material = _c.m_textMesh.transform.GetComponent<MeshRenderer>().material;
		Color color = material.GetColor("_Color");
		color.a = _alpha;
		material.SetColor("_Color", color);
	}

	public static void SetVisibility(TextMeshC _c, bool _visible, bool _markVisibility = true)
	{
		_c.m_go.SetActive(_visible);
		if (_markVisibility)
		{
			_c.m_wasVisible = _visible;
		}
	}

	public static void FitTextToRect(TextMeshC _tmc, float _textboxWidth, float _textboxHeight, string _text, bool _fitTextToWidth = true)
	{
		_text = Loc.T(_text);
		_tmc.m_textboxWidth = _textboxWidth * ((float)m_defaultCharacterSize / _tmc.m_textMesh.characterSize);
		_tmc.m_textboxHeight = _textboxHeight * ((float)m_defaultCharacterSize / _tmc.m_textMesh.characterSize);
		int num = Mathf.FloorToInt(_tmc.m_textboxHeight);
		if (_fitTextToWidth && _tmc.m_textboxWidth > 0f)
		{
			Vector2 textSize = GetTextSize(_tmc.m_textMesh.font, num, _tmc.m_textMesh.lineSpacing, _text);
			if (textSize.x > _tmc.m_textboxWidth)
			{
				float num2 = textSize.x / _tmc.m_textboxWidth;
				num = Mathf.FloorToInt(_tmc.m_textboxHeight / num2);
			}
			num = LocUI.ShrinkFit(num);
		}
		else
		{
			num = Mathf.FloorToInt(_tmc.m_textboxHeight);
		}
		SetFontSize(_tmc, num);
		_tmc.m_textMesh.text = _text;
		_tmc.m_textWidth = _tmc.m_renderer.bounds.size.x;
		_tmc.m_textHeight = _tmc.m_renderer.bounds.size.y;
		SetAlign(_tmc, _tmc.m_horizontalAlign, _tmc.m_verticalAlign);
	}

	public static void SetFontSize(TextMeshC _tmc, float _fontSizeINT)
	{
		_tmc.m_textMesh.fontSize = Mathf.FloorToInt(_fontSizeINT * ((float)m_defaultCharacterSize / _tmc.m_textMesh.characterSize));
	}

	public static void SetText(TextMeshC _tmc, string _text, bool _offsetGO = true)
	{
		_text = Loc.T(_text);
		_text = ToolBox.GetUTF8String(_text);
		_tmc.m_textMesh.text = _text;
		_tmc.m_textWidth = _tmc.m_renderer.bounds.size.x;
		_tmc.m_textHeight = _tmc.m_renderer.bounds.size.y;
		SetAlign(_tmc, _tmc.m_horizontalAlign, _tmc.m_verticalAlign, _offsetGO);
	}

	public static void SetTextOptimized(TextMeshC _tmc, string _text)
	{
		_text = Loc.T(_text);
		_tmc.m_textMesh.text = _text;
		_tmc.m_textWidth = _tmc.m_renderer.bounds.size.x;
		_tmc.m_textHeight = _tmc.m_renderer.bounds.size.y;
	}

	public static string CutTextToWidth(TextMeshC _tmc, string _text, float _width)
	{
		_text = Loc.T(_text);
		_text = ToolBox.GetUTF8String(_text);
		for (float x = GetTextSize(_tmc, _text).x; x > _width; x = GetTextSize(_tmc, _text).x)
		{
			int lastCharIndexWithoutTags = GetLastCharIndexWithoutTags(_tmc, _text);
			if (lastCharIndexWithoutTags >= 0)
			{
				_text = _text.Remove(lastCharIndexWithoutTags, 1);
			}
			else
			{
				x = 0f;
			}
		}
		return _text;
	}

	private static int GetLastCharIndexWithoutTags(TextMeshC _tmc, string _string)
	{
		bool flag = false;
		for (int num = _string.Length - 1; num >= 0; num--)
		{
			char c = _string[num];
			if (flag)
			{
				if (c == '<')
				{
					flag = false;
				}
			}
			else
			{
				if (c != '>')
				{
					return num;
				}
				flag = true;
			}
		}
		return -1;
	}

	public static void SetTextToTextbox(TextMeshC _tmc, float _textboxWidth, float _textboxHeight, string _text)
	{
		_text = Loc.T(_text);
		_text = ToolBox.GetUTF8String(_text);
		_tmc.m_textboxWidth = _textboxWidth * ((float)m_defaultCharacterSize / _tmc.m_textMesh.characterSize);
		_tmc.m_textboxHeight = _textboxHeight * ((float)m_defaultCharacterSize / _tmc.m_textMesh.characterSize);
		_tmc.m_textMesh.text = _text;
		_tmc.m_textWidth = _tmc.m_renderer.bounds.size.x;
		_tmc.m_textHeight = _tmc.m_renderer.bounds.size.y;
		SetAlign(_tmc, _tmc.m_horizontalAlign, _tmc.m_verticalAlign);
	}

	public static void WrapTextToTextbox(TextMeshC _tmc, float _textboxWidth, float _textboxHeight, string _text, int _maxRows = -1)
	{
		_text = Loc.T(_text);
		_text = ToolBox.GetUTF8String(_text);
		_tmc.m_textboxWidth = _textboxWidth * ((float)m_defaultCharacterSize / _tmc.m_textMesh.characterSize);
		_tmc.m_textboxHeight = _textboxHeight * ((float)m_defaultCharacterSize / _tmc.m_textMesh.characterSize);
		_tmc.m_textMesh.text = WrapText(_tmc, _text, _tmc.m_textboxWidth, _maxRows);
		_tmc.m_textWidth = _tmc.m_renderer.bounds.size.x;
		_tmc.m_textHeight = _tmc.m_renderer.bounds.size.y;
		SetAlign(_tmc, _tmc.m_horizontalAlign, _tmc.m_verticalAlign);
	}

	public static void WrapTextToTextbox(TextMeshC _tmc, float _textboxWidth, float _textboxHeight, float _rowHeight, string _text, int _maxRows = -1)
	{
		_text = Loc.T(_text);
		_text = ToolBox.GetUTF8String(_text);
		_tmc.m_textboxWidth = _textboxWidth * ((float)m_defaultCharacterSize / _tmc.m_textMesh.characterSize);
		_tmc.m_textboxHeight = _textboxHeight * ((float)m_defaultCharacterSize / _tmc.m_textMesh.characterSize);
		SetFontSize(_tmc, _rowHeight);
		_tmc.m_textMesh.text = WrapText(_tmc, _text, _tmc.m_textboxWidth, _maxRows);
		_tmc.m_textWidth = _tmc.m_renderer.bounds.size.x;
		_tmc.m_textHeight = _tmc.m_renderer.bounds.size.y;
		SetAlign(_tmc, _tmc.m_horizontalAlign, _tmc.m_verticalAlign);
	}

	public static float GetHeightToWidthRatio(TextMeshC _tmc, string _text)
	{
		_text = Loc.T(_text);
		return GetHeightToWidthRatio(_tmc.m_textMesh.font, _tmc.m_textMesh.fontSize, _tmc.m_textMesh.lineSpacing, _text);
	}

	public static float GetHeightToWidthRatio(Font _font, int _fontSize, float _lineSpacing, string _text)
	{
		_text = Loc.T(_text);
		Vector2 textSize = GetTextSize(_font, _fontSize, _lineSpacing, _text);
		return textSize.y / textSize.x;
	}

	public static Vector2 GetTextSize(TextMeshC _tmc, string _text)
	{
		_text = Loc.T(_text);
		return GetTextSize(_tmc.m_textMesh.font, _tmc.m_textMesh.fontSize, _tmc.m_textMesh.lineSpacing, _text);
	}

	public static Vector2 GetTextSize(Font _font, int _fontSize, float _lineSpacing, string _text)
	{
		_text = Loc.T(_text);
		_text = ToolBox.GetUTF8String(_text);
		_font.RequestCharactersInTexture(_text, _fontSize);
		GUIStyle gUIStyle = new GUIStyle();
		gUIStyle.font = _font;
		gUIStyle.fontSize = _fontSize;
		char[] separator = new char[1] { '\n' };
		string[] array = _text.Split(separator);
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		for (int i = 0; i < array.Length; i++)
		{
			char[] array2 = array[i].ToCharArray();
			bool flag = false;
			int num4 = 0;
			int num5 = array2.Length - 1;
			while (num5 >= 0 && object.Equals(array2[num5], ' '))
			{
				num4++;
				num5--;
			}
			for (int j = 0; j < array2.Length - num4; j++)
			{
				char c = array2[j];
				if (flag)
				{
					if (c == '>')
					{
						flag = false;
					}
				}
				else if (c == '<')
				{
					flag = true;
				}
				else
				{
					CharacterInfo info;
					_font.GetCharacterInfo(c, out info, _fontSize, FontStyle.Normal);
					num += info.width;
				}
			}
			num2 = ((!(num2 > num)) ? num : num2);
			num3 += gUIStyle.lineHeight * _lineSpacing;
			num = 0f;
		}
		return new Vector2(num2, num3);
	}

	public static string WrapText(TextMeshC _tmc, string _text, float _width, int _maxRows = -1, bool _dots = false)
	{
		_text = Loc.T(_text);
		return WrapText(_tmc.m_textMesh.font, _tmc.m_textMesh.fontSize, _tmc.m_textMesh.lineSpacing, _text, _width, _maxRows, _dots);
	}

	public static string WrapText(Font _font, int _fontSize, float _lineSpacing, string _text, float _width, int _maxRows = -1, bool _dots = false)
	{
		_text = Loc.T(_text);
		if (_width == 0f || _text.Length <= 0 || (_maxRows >= 0 && _maxRows < 2))
		{
			return _text;
		}
		_font.RequestCharactersInTexture(_text, _fontSize);
		bool flag = false;
		string text = string.Empty;
		char[] separator = new char[1] { '\n' };
		string[] array = _text.Split(separator);
		string text2 = string.Empty;
		float num = 0f;
		float num2 = 0f;
		int num3 = 1;
		for (int i = 0; i < array.Length; i++)
		{
			string text3 = string.Empty;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			char[] array2 = array[i].ToCharArray();
			for (int j = 0; j < array2.Length; j++)
			{
				char c = array2[j];
				if (flag)
				{
					text += c;
					if (c == '>')
					{
						flag = false;
						text3 += text;
					}
					if (j == array2.Length - 1)
					{
						text2 += text3;
					}
					continue;
				}
				if (c == '<')
				{
					flag = true;
					text = string.Empty;
					text += c;
					continue;
				}
				CharacterInfo info;
				_font.GetCharacterInfo(c, out info, _fontSize, FontStyle.Normal);
				num4 = info.width;
				num6 = ((!(num6 > 0f - info.vert.height)) ? (0f - info.vert.height) : num6);
				if (c == ' ' || j == array2.Length - 1)
				{
					if (c != ' ')
					{
						text3 += c;
						num5 += num4;
					}
					if (num7 + num5 <= _width)
					{
						num7 += num5;
						text2 += text3;
						text3 = string.Empty;
						num5 = 0f;
					}
				}
				if (num7 + num5 > _width)
				{
					num3++;
					if (_maxRows > 0 && num3 > _maxRows)
					{
						if (_dots)
						{
							_font.GetCharacterInfo('.', out info, _fontSize, FontStyle.Normal);
							float width = info.width;
							if (width * 3f + num7 <= _width)
							{
								text2 += "...";
							}
							else
							{
								int startIndex = text2.LastIndexOf(" ");
								text2 = text2.Remove(startIndex);
								text2 += "...";
							}
						}
						return text2;
					}
					num2 = ((!(num2 > num7)) ? num7 : num2);
					num += num6 + _lineSpacing;
					if (text3.Contains(" "))
					{
						text2 += text3.Replace(" ", "\n");
						num7 = num5;
					}
					else if (text3.Length > 0)
					{
						text2 += text3.Insert(text3.Length - 1, "\n");
						num7 = num4;
					}
					num6 = 0f;
					text3 = string.Empty;
					num5 = 0f;
				}
				text3 += c;
				num5 += num4;
			}
			num2 = ((!(num2 > num7)) ? num7 : num2);
			num += num6 + _lineSpacing * 10f;
			if (_maxRows > -1 && i > _maxRows - 2)
			{
				return text2;
			}
			if (i < array.Length - 1)
			{
				text2 += "\n";
			}
		}
		return text2;
	}

	public static void SetAlign(TextMeshC _tmc, Align _horizontal, Align _vertical = Align.Top, bool _offsetGO = true)
	{
		_tmc.m_horizontalAlign = _horizontal;
		_tmc.m_verticalAlign = _vertical;
		Vector3 zero = Vector3.zero;
		if (_tmc.m_textboxWidth > 0f)
		{
			switch (_horizontal)
			{
			case Align.Left:
				zero.x = _tmc.m_textboxWidth * -0.5f;
				break;
			case Align.Right:
				zero.x = _tmc.m_textboxWidth * 0.5f;
				break;
			}
		}
		if (_tmc.m_textboxHeight > 0f)
		{
			switch (_vertical)
			{
			case Align.Top:
				zero.y = _tmc.m_textboxHeight * 0.5f;
				break;
			case Align.Bottom:
				zero.y = _tmc.m_textboxHeight * -0.5f;
				break;
			}
		}
		if (_offsetGO)
		{
			_tmc.m_go.transform.localPosition = zero;
		}
		if (_horizontal == Align.Left && _vertical == Align.Top)
		{
			_tmc.m_textMesh.alignment = TextAlignment.Left;
			_tmc.m_textMesh.anchor = TextAnchor.UpperLeft;
		}
		else if (_horizontal == Align.Left && _vertical == Align.Middle)
		{
			_tmc.m_textMesh.alignment = TextAlignment.Left;
			_tmc.m_textMesh.anchor = TextAnchor.MiddleLeft;
		}
		else if (_horizontal == Align.Left && _vertical == Align.Bottom)
		{
			_tmc.m_textMesh.alignment = TextAlignment.Left;
			_tmc.m_textMesh.anchor = TextAnchor.LowerLeft;
		}
		else if (_horizontal == Align.Center && _vertical == Align.Top)
		{
			_tmc.m_textMesh.alignment = TextAlignment.Center;
			_tmc.m_textMesh.anchor = TextAnchor.UpperCenter;
		}
		else if (_horizontal == Align.Center && _vertical == Align.Middle)
		{
			_tmc.m_textMesh.alignment = TextAlignment.Center;
			_tmc.m_textMesh.anchor = TextAnchor.MiddleCenter;
		}
		else if (_horizontal == Align.Center && _vertical == Align.Bottom)
		{
			_tmc.m_textMesh.alignment = TextAlignment.Center;
			_tmc.m_textMesh.anchor = TextAnchor.LowerCenter;
		}
		else if (_horizontal == Align.Right && _vertical == Align.Top)
		{
			_tmc.m_textMesh.alignment = TextAlignment.Right;
			_tmc.m_textMesh.anchor = TextAnchor.UpperRight;
		}
		else if (_horizontal == Align.Right && _vertical == Align.Middle)
		{
			_tmc.m_textMesh.alignment = TextAlignment.Right;
			_tmc.m_textMesh.anchor = TextAnchor.MiddleRight;
		}
		else if (_horizontal == Align.Right && _vertical == Align.Bottom)
		{
			_tmc.m_textMesh.alignment = TextAlignment.Right;
			_tmc.m_textMesh.anchor = TextAnchor.LowerRight;
		}
	}

	public static void Update()
	{
		m_components.Update();
	}
}
