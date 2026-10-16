using UnityEngine;

public class UISprite : UICanvas
{
	public SpriteSheet m_spriteSheet;

	public Frame m_frame;

	public TransformC m_spriteTC;

	public bool m_adjustWidthToFitSprite;

	public bool m_convertToPrefab;

	private float m_clipLeft;

	private float m_clipRight;

	private float m_clipBottom;

	private float m_clipTop;

	private bool m_clip;

	public Shader m_overrideShader;

	public PrefabC m_rect;

	private Color m_color;

	public UISprite(UIComponent _parent, bool _touchable, string _tag, SpriteSheet _spriteSheet, Frame _frame, bool _convertToPrefab = true)
		: base(_parent, _touchable, _tag, null, string.Empty)
	{
		m_spriteSheet = _spriteSheet;
		m_frame = _frame;
		m_convertToPrefab = _convertToPrefab;
		m_spriteTC = TransformS.AddComponent(m_TC.p_entity, "Sprite");
		TransformS.ParentComponent(m_spriteTC, m_TC, Vector3.zero);
		m_color = Color.gray;
	}

	public void SetColor(Color _color)
	{
		m_color = _color;
	}

	public override void DrawHandler(UIComponent _c)
	{
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
		TransformS.ParentComponent(m_spriteTC, m_TC, Vector3.zero);
		if (num2 > 0f && num > 0f)
		{
			if (m_rect != null)
			{
				PrefabS.RemoveComponent(m_rect);
			}
			Vector2[] rect = DebugDraw.GetRect(m_frame.width * num, m_frame.height * num2, Vector2.zero);
			float num3 = m_spriteSheet.m_material.mainTexture.width;
			float num4 = m_spriteSheet.m_material.mainTexture.height;
			UVRect uVRect = new UVRect(m_frame.x / num3, 1f - m_frame.y / num4 - m_frame.height / num4, m_frame.width / num3, m_frame.height / num4);
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
		}
	}

	public void SetOverrideShader(Shader _overrideShader)
	{
		m_overrideShader = _overrideShader;
	}
}
