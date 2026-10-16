using UnityEngine;

public class PsUIBoosterSelect : UIVerticalList
{
	private PsUIGenericButton m_speed;

	private PsUIGenericButton m_magnet;

	private UIFittedSprite m_sprite;

	public bool m_shown;

	private TweenC m_tween;

	public PsUIBoosterSelect(UIComponent _parent, string _tag)
		: base(_parent, _tag)
	{
		m_shown = false;
		SetWidth(0.4f, RelativeTo.ScreenWidth);
		SetSpacing(0.05f, RelativeTo.ScreenHeight);
		SetDrawHandler(BackgroundDrawHandler);
		m_sprite = new UIFittedSprite(this, true, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_mission_banner_level_base_levelup"));
		m_sprite.SetHeight(0.125f, RelativeTo.ScreenLongest);
		UIVerticalList uIVerticalList = new UIVerticalList(this, string.Empty);
		uIVerticalList.SetWidth(1f, RelativeTo.ParentWidth);
		uIVerticalList.SetMargins(0.05f);
		uIVerticalList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		uIVerticalList.RemoveDrawHandler();
		m_speed = new PsUIGenericButton(uIVerticalList);
		m_speed.SetText("Speed Booster");
		m_magnet = new PsUIGenericButton(uIVerticalList);
		m_magnet.SetText("Star Magnet");
	}

	public override void Step()
	{
		if (m_speed.m_hit)
		{
			m_tween = TweenS.AddTransformTween(m_TC, TweenedProperty.Position, TweenStyle.CubicInOut, m_TC.transform.position, m_TC.transform.position - Vector3.up * m_actualHeight * 0.7f, 0.25f, 0f, false);
			m_shown = false;
		}
		else if (m_magnet.m_hit)
		{
			m_tween = TweenS.AddTransformTween(m_TC, TweenedProperty.Position, TweenStyle.CubicInOut, m_TC.transform.position, m_TC.transform.position - Vector3.up * m_actualHeight * 0.7f, 0.25f, 0f, false);
			m_shown = false;
		}
		else if (m_sprite.m_hit)
		{
			if (m_tween != null)
			{
				TweenS.RemoveComponent(m_tween);
				m_tween = null;
			}
			if (!m_shown)
			{
				m_tween = TweenS.AddTransformTween(m_TC, TweenedProperty.Position, TweenStyle.CubicInOut, m_TC.transform.position, m_TC.transform.position + Vector3.up * m_actualHeight * 0.7f, 0.25f, 0f, false);
				m_shown = true;
			}
			else if (m_shown)
			{
				m_tween = TweenS.AddTransformTween(m_TC, TweenedProperty.Position, TweenStyle.CubicInOut, m_TC.transform.position, m_TC.transform.position - Vector3.up * m_actualHeight * 0.7f, 0.25f, 0f, false);
				m_shown = false;
			}
		}
		base.Step();
	}

	public void BackgroundDrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_points: new Vector2[4]
		{
			new Vector2((0f - _c.m_actualWidth) * 0.5f, _c.m_actualHeight * 0.5f),
			new Vector2((0f - _c.m_actualWidth) * 0.5f, (0f - _c.m_actualHeight) * 0.5f),
			new Vector2(_c.m_actualWidth * 0.5f, (0f - _c.m_actualHeight) * 0.5f),
			new Vector2(_c.m_actualWidth * 0.5f, _c.m_actualHeight * 0.5f)
		}, _tc: _c.m_TC, _offset: Vector3.zero, _bottomColor: DebugDraw.HexToUint("cf5300"), _topColor: DebugDraw.HexToUint("cf5300"), _material: ResourceManager.GetMaterial("Framework/SolidMat"), _camera: _c.m_camera, _identifier: string.Empty);
	}
}
