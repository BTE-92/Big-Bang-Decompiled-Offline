using UnityEngine;

public class UIFittedText : UIComponent
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

	public bool m_fitTextToWidth;

	public UIFittedText(UIComponent _parent, bool _touchable, string _tag, string _text, string _fontResourcePath, bool _fitTextToWidth, string _color = null, string _shadowColor = null)
		: base(_parent, _touchable, _tag, null, null, string.Empty)
	{
		m_fitTextToWidth = _fitTextToWidth;
		m_text = _text;
		m_color = ((_color == null) ? null : _color.Replace("#", string.Empty));
		m_shadowColor = ((_shadowColor == null) ? null : _shadowColor.Replace("#", string.Empty));
		base.SetWidth(m_defaultWidth, m_defaultWidthRelativeTo);
		base.SetHeight(m_defaultHeight, m_defaultHeightRelativeTo);
		SetMargins(m_defaultMargins.l, m_defaultMargins.r, m_defaultMargins.t, m_defaultMargins.b, m_defaultMarginsRelativeTo);
		RemoveDrawHandler();
		m_tmc = TextMeshS.AddComponent(m_TC, Vector3.zero, _fontResourcePath, 0f, 0f, 10f, Align.Center, Align.Middle, m_camera, _tag + "Text");
		if (m_shadowColor != null)
		{
			m_shadowtmc = TextMeshS.AddComponent(m_TC, m_shadowDirection, _fontResourcePath, 0f, 0f, 10f, Align.Center, Align.Middle, m_camera, _tag + "Text");
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
	}

	public void SetText(string _text)
	{
		float x = (m_actualMargins.l - m_actualMargins.r) * 0.5f;
		float y = (m_actualMargins.t - m_actualMargins.b) * -0.5f;
		m_text = _text;
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

	public override void SetHeight(float _heightRatio, RelativeTo _relativeTo)
	{
		base.SetHeight(_heightRatio, _relativeTo);
	}

	public override void SetWidth(float _widthRatio, RelativeTo _relativeTo)
	{
		base.SetWidth(_widthRatio, _relativeTo);
	}

	public override void Update()
	{
		base.SetWidth(m_defaultWidth, m_defaultWidthRelativeTo);
		base.SetHeight(m_defaultHeight, m_defaultHeightRelativeTo);
		CalculateReferenceSizes();
		UpdateSize();
		UpdateMargins();
		float x = (m_actualMargins.l - m_actualMargins.r) * 0.5f;
		float y = (m_actualMargins.t - m_actualMargins.b) * -0.5f;
		if (m_color != null)
		{
			TextMeshS.FitTextToRect(m_tmc, m_actualWidth, m_actualHeight, "<color=#" + m_color + ">" + m_text + "</color>", m_fitTextToWidth);
		}
		else
		{
			TextMeshS.FitTextToRect(m_tmc, m_actualWidth, m_actualHeight, m_text, m_fitTextToWidth);
		}
		m_tmc.m_go.transform.Translate(new Vector3(x, y));
		if (m_shadowtmc != null)
		{
			TextMeshS.FitTextToRect(m_shadowtmc, m_actualWidth, m_actualHeight, "<color=#" + m_shadowColor + ">" + m_text + "</color>", m_fitTextToWidth);
			m_shadowtmc.m_go.transform.Translate(new Vector3(x, y) + (Vector3)(m_shadowDirection * m_tmc.m_textMesh.fontSize * m_shadowDistance) + Vector3.forward * 5f);
		}
		Vector2 textSize = TextMeshS.GetTextSize(m_tmc, m_text);
		base.SetWidth((textSize.x + m_actualMargins.l + m_actualMargins.r) / (float)Screen.width, RelativeTo.ScreenWidth);
		base.SetHeight((textSize.y + m_actualMargins.t + m_actualMargins.b) / (float)Screen.height, RelativeTo.ScreenHeight);
		CalculateReferenceSizes();
		UpdateSize();
		UpdateMargins();
		UpdateAlign();
		if (!m_hidden && d_Draw != null)
		{
			d_Draw(this);
		}
		UpdateUniqueCamera();
		UpdateChildren();
		ArrangeContents();
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
