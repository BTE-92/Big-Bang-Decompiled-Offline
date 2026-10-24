using UnityEngine;

public class UIFittedSprite : UICanvas
{
	public SpriteSheet m_spriteSheet;

	public Frame m_frame;

	public TransformC m_spriteTC;

	public bool m_adjustWidthToFitSprite;

	public bool m_convertToPrefab;

	public bool m_scaleOnTouch;

	public TweenC m_touchScaleTween;

	public Shader m_overrideShader;

	private Color m_color;

	public PrefabC m_rect;

	public UIFittedSprite(UIComponent _parent, bool _touchable, string _tag, SpriteSheet _spriteSheet, Frame _frame, bool _convertToPrefab = true, bool _adjustWidthToFitSprite = true)
		: base(_parent, _touchable, _tag, null, string.Empty)
	{
		m_scaleOnTouch = true;
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
		SetMargins(0f);
		m_spriteSheet = _spriteSheet;
		m_frame = _frame;
		m_convertToPrefab = _convertToPrefab;
		m_adjustWidthToFitSprite = _adjustWidthToFitSprite;
		m_spriteTC = TransformS.AddComponent(m_TC.p_entity, "Sprite");
		TransformS.ParentComponent(m_spriteTC, m_TC, Vector3.zero);
		m_color = Color.gray;
	}

	public void SetColor(Color _color)
	{
		m_color = _color;
	}

	public override void Update()
	{
		if (m_frame != null && m_spriteSheet != null && !m_hidden)
		{
			CalculateReferenceSizes();
			UpdateSize();
			UpdateMargins();
			float num = 0f;
			float num2 = 0f;
			if (m_frame.height > 0f)
			{
				num2 = m_actualHeight / m_frame.height;
			}
			if (m_frame.width > 0f)
			{
				num = m_actualWidth / m_frame.width;
			}
			float num3 = Mathf.Min(num2, num);
			float num4 = m_frame.height / m_frame.width;
			if (num2 < num)
			{
				SetWidth(m_actualHeight / num4 / (float)Screen.width, RelativeTo.ScreenWidth);
			}
			else
			{
				SetHeight(m_actualWidth * num4 / (float)Screen.height, RelativeTo.ScreenHeight);
			}
			CalculateReferenceSizes();
			UpdateSize();
			UpdateAlign();
			if (m_convertToPrefab)
			{
			}
			if (m_rect != null)
			{
				PrefabS.RemoveComponent(m_rect);
			}
			Vector2[] rect = DebugDraw.GetRect(m_frame.width * num3, m_frame.height * num3, Vector2.zero);
			float num5 = m_spriteSheet.m_material.mainTexture.width;
			float num6 = m_spriteSheet.m_material.mainTexture.height;
			UVRect uVRect = new UVRect(m_frame.x / num5, 1f - m_frame.y / num6 - m_frame.height / num6, m_frame.width / num5, m_frame.height / num6);
			if (m_frame.flipX)
			{
				uVRect.left += uVRect.width;
				uVRect.width *= -1f;
			}
			if (m_frame.flipY)
			{
				uVRect.bottom += uVRect.height;
				uVRect.height *= -1f;
			}
			m_rect = PrefabS.CreateFlatPrefabComponentsFromVectorArray(m_spriteTC, Vector3.zero, rect, DebugDraw.ColorToUInt(m_color), DebugDraw.ColorToUInt(m_color), m_spriteSheet.m_material, m_camera, string.Empty, uVRect);
			if (m_overrideShader != null)
			{
				m_rect.p_gameObject.GetComponent<Renderer>().material.shader = m_overrideShader;
			}
			if (!m_hidden && d_Draw != null)
			{
				d_Draw(this);
			}
			UpdateUniqueCamera();
			UpdateChildren();
			ArrangeContents();
			if (m_parent == null)
			{
				UpdateSpecial();
			}
		}
	}

	public override void Destroy()
	{
		m_overrideShader = null;
		base.Destroy();
	}

	public void SetFrame(Frame _frame)
	{
		m_frame = _frame;
	}

	public void SetOverrideShader(Shader _overrideShader)
	{
		m_overrideShader = _overrideShader;
	}

	public override void DrawHandler(UIComponent _c)
	{
	}
}
