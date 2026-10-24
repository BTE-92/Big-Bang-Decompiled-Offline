using UnityEngine;

public class UITextbox : UIComponent
{
	public static float m_defaultWidth = 1f;

	public static float m_defaultHeight = 1f;

	public static cpBB m_defaultMargins = new cpBB(0f);

	public static RelativeTo m_defaultWidthRelativeTo = RelativeTo.ParentWidth;

	public static RelativeTo m_defaultHeightRelativeTo = RelativeTo.ParentHeight;

	public static RelativeTo m_defaultMarginsRelativeTo = RelativeTo.ScreenShortest;

	public TextMeshC m_tmc;

	public string m_text;

	public float m_fontSize;

	public bool m_intFontSize;

	public RelativeTo m_fontSizeRelativeTo;

	public float m_fontSizeRelative;

	public bool m_adjustWidthToTextWidth;

	public string m_fontResourcePath;

	public Align m_textHorizontalAlign;

	public Align m_textVerticalAlign;

	public int m_maxRows;

	public int m_minRows;

	private string m_color;

	private float m_maxWidth;

	private bool m_dots;

	public UITextbox(UIComponent _parent, bool _touchable, string _tag, string _text, string _fontResourcePath, float _fontSize, RelativeTo _fontSizeRelativeTo, bool _adjustWidthToTextWidth = false, Align _horizontalAlign = Align.Left, Align _verticalAlign = Align.Top, string _color = null, bool _intFontSize = true)
		: base(_parent, _touchable, _tag, null, null, string.Empty)
	{
		if (_text == null)
		{
			_text = "null";
		}
		SetWidth(m_defaultWidth, m_defaultWidthRelativeTo);
		SetHeight(m_defaultHeight, m_defaultHeightRelativeTo);
		SetMargins(m_defaultMargins.l, m_defaultMargins.r, m_defaultMargins.t, m_defaultMargins.b, m_defaultMarginsRelativeTo);
		SetAlign(0.5f, 0.5f);
		m_color = ((_color == null) ? null : _color.Replace("#", string.Empty));
		m_text = _text;
		m_fontSize = _fontSize;
		m_intFontSize = _intFontSize;
		m_fontSizeRelativeTo = _fontSizeRelativeTo;
		m_adjustWidthToTextWidth = _adjustWidthToTextWidth;
		m_fontResourcePath = _fontResourcePath;
		m_textHorizontalAlign = _horizontalAlign;
		m_textVerticalAlign = _verticalAlign;
		m_maxRows = -1;
		m_maxWidth = 0f;
		float textboxWidth = m_actualWidth - m_actualMargins.l - m_actualMargins.r;
		float textboxHeight = m_actualHeight - m_actualMargins.t - m_actualMargins.b;
		m_tmc = TextMeshS.AddComponent(m_TC, Vector3.zero, m_fontResourcePath, textboxWidth, textboxHeight, m_fontSize, m_textHorizontalAlign, m_textVerticalAlign, m_camera, "Text");
	}

	public void SetText(string _text)
	{
		m_text = _text;
		if (m_tmc != null)
		{
			if (m_color != null)
			{
				TextMeshS.SetText(m_tmc, "<color=#" + m_color + ">" + _text + "</color>");
			}
			else
			{
				TextMeshS.SetText(m_tmc, _text);
			}
			TextMeshS.WrapTextToTextbox(m_tmc, m_actualWidth - m_actualMargins.l - m_actualMargins.r, m_actualHeight - m_actualMargins.t - m_actualMargins.b, _text, m_maxRows);
			float x = (m_actualMargins.l - m_actualMargins.r) * 0.5f;
			float y = (m_actualMargins.b - m_actualMargins.t) * 0.5f;
			m_tmc.m_go.transform.Translate(new Vector3(x, y));
		}
	}

	public void SetMinRows(int _minRows)
	{
		m_minRows = _minRows;
	}

	public void SetMaxRows(int _maxRows)
	{
		m_maxRows = _maxRows;
	}

	public void UseDotsWhenWrapping(bool _flag)
	{
		m_dots = _flag;
	}

	public void SetMaxWidth(float _maxWidth)
	{
		m_maxWidth = _maxWidth;
	}

	public override void Update()
	{
		CalculateReferenceSizes();
		UpdateSize();
		UpdateMargins();
		float size = ((!m_intFontSize) ? (m_fontSize * m_fontSizeRelative) : ((float)Mathf.FloorToInt(m_fontSize * m_fontSizeRelative)));
		size = LocUI.FitBoxFont(this, size);
		TextMeshS.SetFontSize(m_tmc, size);
		float width = (m_actualWidth - m_actualMargins.l - m_actualMargins.r) * ((float)TextMeshS.m_defaultCharacterSize / m_tmc.m_textMesh.characterSize);
		string text = TextMeshS.WrapText(m_tmc, m_text, width, m_maxRows, m_dots);
		Vector2 textSize = TextMeshS.GetTextSize(m_tmc, text);
		if (m_maxWidth > 0f)
		{
			text = TextMeshS.CutTextToWidth(m_tmc, text, m_maxWidth);
		}
		if (m_adjustWidthToTextWidth)
		{
			if (m_fontSizeRelativeTo != RelativeTo.World)
			{
				SetWidth((textSize.x + m_actualMargins.l + m_actualMargins.r) / (float)Screen.width, RelativeTo.ScreenWidth);
			}
			else
			{
				SetWidth(textSize.x + m_actualMargins.l + m_actualMargins.r, RelativeTo.World);
			}
		}
		if (m_fontSizeRelativeTo != RelativeTo.World)
		{
			if (textSize.y < ((float)m_tmc.m_textMesh.fontSize + m_tmc.m_textMesh.lineSpacing) * (float)m_minRows)
			{
				SetHeight((((float)m_tmc.m_textMesh.fontSize + m_tmc.m_textMesh.lineSpacing) * (float)m_minRows + m_actualMargins.t + m_actualMargins.b) / (float)Screen.height, RelativeTo.ScreenHeight);
			}
			else
			{
				SetHeight((textSize.y + m_actualMargins.t + m_actualMargins.b) / (float)Screen.height, RelativeTo.ScreenHeight);
			}
		}
		else if (textSize.y < ((float)m_tmc.m_textMesh.fontSize + m_tmc.m_textMesh.lineSpacing) * (float)m_minRows)
		{
			SetHeight(((float)m_tmc.m_textMesh.fontSize + m_tmc.m_textMesh.lineSpacing) * (float)m_minRows + m_actualMargins.t + m_actualMargins.b, RelativeTo.World);
		}
		else
		{
			float num = textSize.y / ((float)TextMeshS.m_defaultCharacterSize / m_tmc.m_textMesh.characterSize);
			SetHeight(num + m_actualMargins.t + m_actualMargins.b, RelativeTo.World);
		}
		CalculateReferenceSizes();
		UpdateSize();
		if (m_color != null)
		{
			text = "<color=#" + m_color + ">" + text + "</color>";
		}
		float textboxWidth = m_actualWidth - m_actualMargins.l - m_actualMargins.r;
		float textboxHeight = m_actualHeight - m_actualMargins.t - m_actualMargins.b;
		TextMeshS.WrapTextToTextbox(m_tmc, textboxWidth, textboxHeight, text, m_maxRows);
		float x = (m_actualMargins.l - m_actualMargins.r) * 0.5f;
		float y = (m_actualMargins.b - m_actualMargins.t) * 0.5f;
		m_tmc.m_go.transform.Translate(new Vector3(x, y));
		UpdateAlign();
		UpdateMargins();
		if (!m_hidden && d_Draw != null)
		{
			d_Draw(this);
		}
		UpdateUniqueCamera();
		UpdateChildren();
		ArrangeContents();
	}

	public override void CalculateReferenceSizes()
	{
		base.CalculateReferenceSizes();
		m_fontSizeRelative = Screen.height;
		if (m_fontSizeRelativeTo == RelativeTo.World)
		{
			m_fontSizeRelative = 1f;
		}
		else if (m_fontSizeRelativeTo == RelativeTo.OwnHeight)
		{
			m_fontSizeRelative = m_actualHeight;
		}
		else if (m_fontSizeRelativeTo == RelativeTo.OwnWidth)
		{
			m_fontSizeRelative = m_actualWidth;
		}
		else if (m_fontSizeRelativeTo == RelativeTo.ParentHeight)
		{
			if (m_parent != null)
			{
				m_fontSizeRelative = m_parent.m_actualHeight;
			}
			else
			{
				m_fontSizeRelative = Screen.height;
			}
		}
		else if (m_fontSizeRelativeTo == RelativeTo.ParentWidth)
		{
			if (m_parent != null)
			{
				m_fontSizeRelative = m_parent.m_actualWidth;
			}
			else
			{
				m_fontSizeRelative = Screen.width;
			}
		}
		else if (m_fontSizeRelativeTo == RelativeTo.ParentLongest)
		{
			if (m_parent != null)
			{
				m_fontSizeRelative = Mathf.Max(m_parent.m_actualHeight, m_parent.m_actualWidth);
			}
			else
			{
				m_fontSizeRelative = Mathf.Max(Screen.height, Screen.width);
			}
		}
		else if (m_fontSizeRelativeTo == RelativeTo.ParentShortest)
		{
			if (m_parent != null)
			{
				m_fontSizeRelative = Mathf.Min(m_parent.m_actualHeight, m_parent.m_actualWidth);
			}
			else
			{
				m_fontSizeRelative = Mathf.Min(Screen.height, Screen.width);
			}
		}
		else if (m_fontSizeRelativeTo == RelativeTo.ScreenHeight)
		{
			m_fontSizeRelative = Screen.height;
		}
		else if (m_fontSizeRelativeTo == RelativeTo.ScreenWidth)
		{
			m_fontSizeRelative = Screen.width;
		}
		else if (m_fontSizeRelativeTo == RelativeTo.ScreenLongest)
		{
			m_fontSizeRelative = Mathf.Max(Screen.height, Screen.width);
		}
		else if (m_fontSizeRelativeTo == RelativeTo.ScreenShortest)
		{
			m_fontSizeRelative = Mathf.Min(Screen.height, Screen.width);
		}
	}

	public override void DrawHandler(UIComponent _c)
	{
	}

	public override void SetCamera(Camera _camera, bool _affectChildren, bool _uniqueCamera)
	{
		if (m_tmc != null)
		{
			m_tmc.m_go.layer = _camera.gameObject.layer;
		}
		base.SetCamera(_camera, _affectChildren, _uniqueCamera);
	}
}
