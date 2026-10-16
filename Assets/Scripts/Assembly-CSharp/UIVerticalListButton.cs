using UnityEngine;

public class UIVerticalListButton : UIComponent
{
	public static float m_defaultWidth = 1f;

	public static float m_defaultHeight = 0.05f;

	public static cpBB m_defaultMargins = new cpBB(0f);

	public static RelativeTo m_defaultWidthRelativeTo = RelativeTo.ParentWidth;

	public static RelativeTo m_defaultHeightRelativeTo = RelativeTo.ScreenShortest;

	public static RelativeTo m_defaultMarginsRelativeTo = RelativeTo.ScreenShortest;

	protected UIScrollableCanvas m_scrollableCanvas;

	protected UIPagedCanvas m_pagedCanvas;

	public UIVerticalListButton(UIComponent _parent, UIScrollableCanvas _scrollableCanvas, UIPagedCanvas _pagedCanvas, string _tag, UIModel _model, string _fieldName)
		: base(_parent, true, _tag, null, _model, _fieldName)
	{
		m_scrollableCanvas = _scrollableCanvas;
		m_pagedCanvas = _pagedCanvas;
		m_TAC.m_letTouchesThrough = true;
		SetWidth(m_defaultWidth, m_defaultWidthRelativeTo);
		SetHeight(m_defaultHeight, m_defaultHeightRelativeTo);
		SetMargins(m_defaultMargins, m_defaultMarginsRelativeTo);
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
