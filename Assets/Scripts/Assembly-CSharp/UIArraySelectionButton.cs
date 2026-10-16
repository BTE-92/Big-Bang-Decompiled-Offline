using UnityEngine;

public class UIArraySelectionButton : UIComponent
{
	public static float m_defaultWidth = 1f;

	public static float m_defaultHeight = 0.05f;

	public static cpBB m_defaultMargins = new cpBB(0f);

	public static RelativeTo m_defaultWidthRelativeTo = RelativeTo.ParentWidth;

	public static RelativeTo m_defaultHeightRelativeTo = RelativeTo.ScreenShortest;

	public static RelativeTo m_defaultMarginsRelativeTo = RelativeTo.ScreenShortest;

	protected UIScrollableCanvas m_scrollableCanvas;

	protected UIPagedCanvas m_pagedCanvas;

	protected UIText m_label;

	protected object m_value;

	protected bool m_changeValue;

	public UIArraySelectionButton(UIComponent _parent, UIScrollableCanvas _scrollableCanvas, UIPagedCanvas _pagedCanvas, string _tag, UIModel _model, string _fieldName, string _label, object _value)
		: base(_parent, true, _tag, null, _model, _fieldName)
	{
		SetWidth(m_defaultWidth, m_defaultWidthRelativeTo);
		SetHeight(m_defaultHeight, m_defaultHeightRelativeTo);
		SetMargins(m_defaultMargins, m_defaultMarginsRelativeTo);
		m_TAC.m_letTouchesThrough = true;
		m_scrollableCanvas = _scrollableCanvas;
		m_pagedCanvas = _pagedCanvas;
		m_value = _value;
		m_label = new UIText(this, false, _tag, _label, "HurmeRegular_Font", 0.02f, RelativeTo.ScreenHeight);
		m_label.SetAlign(1f, 0f);
	}

	protected override void OnTouchBegan(TLTouch _touch)
	{
		base.OnTouchBegan(_touch);
		m_changeValue = true;
	}

	protected override void OnTouchDragStart(TLTouch _touch)
	{
		base.OnTouchDragStart(_touch);
		m_changeValue = false;
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		base.OnTouchRelease(_touch, _inside);
		if (m_changeValue && _inside)
		{
			SetValue(m_value);
			if (m_pagedCanvas != null)
			{
				m_scrollableCanvas.Destroy();
				m_pagedCanvas.Update();
				m_pagedCanvas.PreviousPage();
			}
		}
	}

	public override void DrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(m_TC.p_entity);
		Vector2[] rect = DebugDraw.GetRect(m_actualWidth, m_actualHeight * 0.95f, Vector2.zero);
		Color color = new Color(0.4f, 0.4f, 0.4f);
		if (m_highlight)
		{
			color = new Color(0.3f, 0.3f, 0.3f);
		}
		Camera camera = CameraS.m_uiCamera;
		if (m_parent != null)
		{
			camera = m_parent.m_camera;
		}
		uint num = DebugDraw.ColorToUInt(color);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(m_TC, Vector3.zero, rect, num, num, ResourceManager.GetMaterial("Framework/SolidMat"), camera, "UIComponent: Prefab");
	}
}
