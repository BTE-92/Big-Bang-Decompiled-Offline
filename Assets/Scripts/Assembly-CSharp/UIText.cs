using UnityEngine;

public class UIText : UIComponent
{
	public static float m_defaultWidth = 1f;

	public static float m_defaultHeight = 1f;

	public static cpBB m_defaultMargins = new cpBB(0f);

	public static RelativeTo m_defaultWidthRelativeTo = RelativeTo.ParentWidth;

	public static RelativeTo m_defaultHeightRelativeTo = RelativeTo.ParentHeight;

	public static RelativeTo m_defaultMarginsRelativeTo = RelativeTo.ScreenShortest;

	public TextMeshC m_tmc;

	public TextMeshC m_shadowtmc;

	public Vector2 m_shadowDirection;

	public float m_shadowDistance;

	private string m_color;

	private string m_shadowColor;

	public string m_text;

	public float m_fontSize;

	public RelativeTo m_fontSizeRelativeTo;

	public float m_fontSizeRelative;

	public bool m_adjustWidthToTextWidth;

	public string m_fontResourcePath;

	public UIText(UIComponent _parent, bool _touchable, string _tag, string _text, string _fontResourcePath, float _fontSize, RelativeTo _fontSizeRelativeTo, string _color = null, string _shadowColor = null)
		: base(_parent, _touchable, _tag, null, null, string.Empty)
	{
		SetWidth(m_defaultWidth, m_defaultWidthRelativeTo);
		SetHeight(m_defaultHeight, m_defaultHeightRelativeTo);
		SetMargins(m_defaultMargins.l, m_defaultMargins.r, m_defaultMargins.t, m_defaultMargins.b, m_defaultMarginsRelativeTo);
		SetAlign(0.5f, 0.5f);
		RemoveDrawHandler();
		m_text = _text;
		m_fontSize = _fontSize;
		m_fontSizeRelativeTo = _fontSizeRelativeTo;
		m_fontResourcePath = _fontResourcePath;
		m_color = ((_color == null) ? null : _color.Replace("#", string.Empty));
		m_shadowColor = ((_shadowColor == null) ? null : _shadowColor.Replace("#", string.Empty));
		m_tmc = TextMeshS.AddComponent(m_TC, Vector3.zero, _fontResourcePath, 0f, 0f, _fontSize, Align.Center, Align.Middle, m_camera, _tag + "Text");
		if (m_shadowColor != null)
		{
			m_shadowtmc = TextMeshS.AddComponent(m_TC, m_shadowDirection, _fontResourcePath, 0f, 0f, _fontSize, Align.Center, Align.Middle, m_camera, _tag + "Text");
			SetShadowShift(new Vector2(-0.5f, -1f).normalized);
		}
	}

	public void SetShadowShift(Vector2 _direction, float _distanceRelativeToFontSize = 0.1f)
	{
		m_shadowDirection = new Vector2(_direction.x, _direction.y);
		m_shadowDistance = _distanceRelativeToFontSize;
	}

	public void SetColor(string _color, string _shadowColor = null)
	{
		m_color = ((_color == null) ? null : _color.Replace("#", string.Empty));
		m_shadowColor = ((_shadowColor == null) ? null : _shadowColor.Replace("#", string.Empty));
		SetText(m_text);
	}

	public virtual void SetText(string _text)
	{
		m_text = _text;
		float x = (m_actualMargins.l - m_actualMargins.r) * 0.5f;
		float y = (m_actualMargins.t - m_actualMargins.b) * -0.5f;
		if (m_color != null)
		{
			TextMeshS.SetText(m_tmc, "<color=#" + m_color + ">" + m_text + "</color>");
		}
		else
		{
			TextMeshS.SetText(m_tmc, m_text);
		}
		m_tmc.m_go.transform.Translate(new Vector3(x, y));
		if (m_shadowtmc != null)
		{
			TextMeshS.SetText(m_shadowtmc, "<color=#" + m_shadowColor + ">" + m_text + "</color>");
			m_shadowtmc.m_go.transform.Translate(new Vector3(x, y) + (Vector3)(m_shadowDirection * m_shadowtmc.m_textMesh.fontSize * m_shadowDistance) + Vector3.forward * 5f);
		}
	}

	public override void Update()
	{
		CalculateReferenceSizes();
		UpdateSize();
		UpdateMargins();
		int size = Mathf.FloorToInt(m_fontSize * m_fontSizeRelative);
		size = LocUI.FitFont(this, size);
		TextMeshS.SetFontSize(m_tmc, size);
		if (m_shadowtmc != null)
		{
			TextMeshS.SetFontSize(m_shadowtmc, size);
		}
		Vector2 textSize = TextMeshS.GetTextSize(m_tmc, m_text);
		SetWidth((textSize.x + m_actualMargins.l + m_actualMargins.r) / (float)Screen.width, RelativeTo.ScreenWidth);
		SetHeight((textSize.y + m_actualMargins.t + m_actualMargins.b) / (float)Screen.height, RelativeTo.ScreenHeight);
		CalculateReferenceSizes();
		UpdateSize();
		float x = (m_actualMargins.l - m_actualMargins.r) * 0.5f;
		float y = (m_actualMargins.t - m_actualMargins.b) * -0.5f;
		if (m_color != null)
		{
			TextMeshS.SetText(m_tmc, "<color=#" + m_color + ">" + m_text + "</color>");
		}
		else
		{
			TextMeshS.SetText(m_tmc, m_text);
		}
		m_tmc.m_go.transform.Translate(new Vector3(x, y));
		if (m_shadowtmc != null)
		{
			TextMeshS.SetText(m_shadowtmc, "<color=#" + m_shadowColor + ">" + m_text + "</color>");
			m_shadowtmc.m_go.transform.Translate(new Vector3(x, y) + (Vector3)(m_shadowDirection * size * m_shadowDistance) + Vector3.forward * 5f);
		}
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

	public override void SetCamera(Camera _camera, bool _affectChildren, bool _uniqueCamera)
	{
		if (m_tmc != null)
		{
			m_tmc.m_go.layer = _camera.gameObject.layer;
		}
		base.SetCamera(_camera, _affectChildren, _uniqueCamera);
	}
}
