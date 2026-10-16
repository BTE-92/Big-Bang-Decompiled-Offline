using UnityEngine;

public class PsUITopPause : UICanvas
{
	private PsUIGenericButton m_exitButton;

	private PsUIGenericButton m_restartButton;

	protected UIHorizontalList m_leftArea;

	public PsUITopPause(UIComponent _parent)
		: base(_parent, false, "TopContent", null, string.Empty)
	{
		RemoveDrawHandler();
		CreateLeftArea();
		CreateMiddleArea();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, "UpperRight");
		uIHorizontalList.SetMargins(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetAlign(1f, 1f);
		uIHorizontalList.RemoveDrawHandler();
		m_restartButton = new PsUIGenericButton(uIHorizontalList);
		m_restartButton.SetIcon("hud_icon_restart", 0.06f);
		m_restartButton.SetOrangeColors();
	}

	public virtual void CreateLeftArea()
	{
		m_leftArea = new UIHorizontalList(this, "UpperLeft");
		m_leftArea.SetMargins(0.025f, RelativeTo.ScreenShortest);
		m_leftArea.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		m_leftArea.SetAlign(0f, 1f);
		m_leftArea.RemoveDrawHandler();
		m_exitButton = new PsUIGenericButton(m_leftArea);
		m_exitButton.SetIcon("hud_icon_menu_exit", 0.06f);
		m_exitButton.SetOrangeColors();
	}

	protected void SkipLevel()
	{
		(GetRoot() as PsUIBasePopup).CallAction("Skip");
	}

	public virtual void CreateMiddleArea()
	{
	}

	public void DescriptionDrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_points: new Vector2[7]
		{
			new Vector2(_c.m_actualWidth * -0.5f, _c.m_actualHeight * 0.5f),
			new Vector2(_c.m_actualWidth * -0.5f, _c.m_actualHeight * -0.5f),
			new Vector2(_c.m_actualWidth * 0.5f, _c.m_actualHeight * -0.5f),
			new Vector2(_c.m_actualWidth * 0.5f, _c.m_actualHeight * 0.5f),
			new Vector2(_c.m_actualWidth * -0.4f, _c.m_actualHeight * 0.5f),
			new Vector2(_c.m_actualWidth * -0.45f, _c.m_actualHeight * 0.5f + 0.025f * (float)Screen.height),
			new Vector2(_c.m_actualWidth * -0.45f, _c.m_actualHeight * 0.5f)
		}, _tc: _c.m_TC, _offset: Vector3.zero, _bottomColor: DebugDraw.HexToUint("fffec6"), _topColor: DebugDraw.HexToUint("fffec6"), _material: ResourceManager.GetMaterial("Framework/SolidMat"), _camera: _c.m_camera, _identifier: string.Empty);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_points: new Vector2[4]
		{
			new Vector2(_c.m_actualWidth * -0.5f, _c.m_actualHeight * 0.5f),
			new Vector2(_c.m_actualWidth * -0.5f, _c.m_actualHeight * -0.5f),
			new Vector2(_c.m_actualWidth * 0.5f, _c.m_actualHeight * -0.5f),
			new Vector2(_c.m_actualWidth * 0.5f, _c.m_actualHeight * 0.5f)
		}, _tc: _c.m_TC, _offset: Vector3.forward + Vector3.down + Vector3.right * 0.5f, _width: 0.004f * (float)Screen.height, _color: DebugDraw.GetColor(0f, 0f, 0f, 110f), _material: ResourceManager.GetMaterial("Framework/SolidMat"), _camera: _c.m_camera, _align: Position.Center, _closed: true);
	}

	public override void Step()
	{
		if (m_exitButton.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
		}
		else if (m_restartButton.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Restart");
		}
		base.Step();
	}
}
