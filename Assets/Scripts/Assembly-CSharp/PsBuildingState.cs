using System;
using System.Collections;
using UnityEngine;

public class PsBuildingState : BasicState
{
	public static Hashtable m_currentSubmenus = new Hashtable();

	private PsMenuScene m_parent;

	private UIHorizontalList m_buildingUIBase;

	private UICanvas m_sideMenu;

	private PsUIGenericButton m_backButton;

	public UICanvas m_contentCanvas;

	public Action m_createPopup;

	private UIHorizontalList m_backList;

	private int m_forcedSubMenuIndex = -1;

	private Type m_contentType;

	private static bool m_reset = false;

	public PsBuildingState(Type _contentType)
	{
		m_contentType = _contentType;
	}

	public override void Enter(IStatedObject _parent)
	{
		m_reset = false;
		PsMetagameManager.ShowResources();
		m_parent = _parent as PsMenuScene;
		PsMenuScene.CreateBlur();
		m_buildingUIBase = new UIHorizontalList(null, "BuildingUIBase");
		m_buildingUIBase.SetHorizontalAlign(0f);
		m_buildingUIBase.RemoveDrawHandler();
		m_sideMenu = new UICanvas(m_buildingUIBase, false, "SideMenu", null, string.Empty);
		m_sideMenu.SetWidth(0.2f, RelativeTo.ScreenWidth);
		m_sideMenu.SetDrawHandler(SideMenuDrawHandler);
		UIVerticalList uIVerticalList = new UIVerticalList(m_sideMenu, "SideMenu");
		uIVerticalList.SetWidth(1f, RelativeTo.ParentWidth);
		uIVerticalList.SetVerticalAlign(1f);
		uIVerticalList.RemoveDrawHandler();
		UIVerticalList uIVerticalList2 = new UIVerticalList(uIVerticalList, string.Empty);
		uIVerticalList2.SetMargins(0.025f, 0f, 0.025f, 0f, RelativeTo.ScreenShortest);
		uIVerticalList2.SetHorizontalAlign(0f);
		uIVerticalList2.RemoveDrawHandler();
		m_backList = new UIHorizontalList(uIVerticalList2, "BackButton");
		m_backList.SetSpacing(0.01f, RelativeTo.ScreenHeight);
		m_backList.RemoveDrawHandler();
		m_backButton = new PsUIGenericButton(m_backList);
		m_backButton.SetText("Back");
		m_contentCanvas = new UICanvas(m_buildingUIBase, false, "ContentCanvas", null, string.Empty);
		m_contentCanvas.SetWidth(0.65f, RelativeTo.ScreenWidth);
		m_contentCanvas.RemoveDrawHandler();
		object[] args = new object[1] { m_contentCanvas };
		object obj = Activator.CreateInstance(m_contentType, args);
		m_buildingUIBase.Update();
		if (m_createPopup != null)
		{
			m_createPopup();
			m_createPopup = null;
		}
	}

	public static void Reset()
	{
		m_reset = true;
	}

	public override void Execute()
	{
		if (m_backButton.m_hit)
		{
			SoundS.PlaySingleShot("/UI/ButtonBack", Vector3.zero);
			m_parent.m_stateMachine.ChangeState(new PsMainMenuState());
			PsMenuScene.m_lastState = null;
		}
	}

	private void DestroyUI()
	{
		if (m_buildingUIBase != null)
		{
			m_buildingUIBase.Destroy();
			m_buildingUIBase = null;
		}
	}

	public override void Exit()
	{
		DestroyUI();
		PsMetagameManager.HideResources();
	}

	private void SideMenuDrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] points = new Vector2[4]
		{
			new Vector2(_c.m_actualWidth * -0.55f, _c.m_actualHeight * 0.55f),
			new Vector2(_c.m_actualWidth * -0.55f + _c.m_actualHeight * 0.1f, _c.m_actualHeight * 0.55f),
			new Vector2(_c.m_actualWidth * -0.55f + _c.m_actualHeight * 0.2f, _c.m_actualHeight * -0.55f),
			new Vector2(_c.m_actualWidth * -0.55f, _c.m_actualHeight * -0.55f)
		};
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * 5f, points, DebugDraw.HexToUint("00396e"), DebugDraw.HexToUint("00659b"), ResourceManager.GetMaterial(RESOURCE.MenuMetalMat_Material), _c.m_camera, "BG");
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 4f, points, (float)Screen.height * 0.0125f, DebugDraw.HexToColor("0092c7"), ResourceManager.GetMaterial("Framework/Line4Mat"), _c.m_camera, Position.Center, true);
	}
}
