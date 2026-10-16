using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsMainMenuState : BasicState, IStatedObject
{
	private TouchAreaC m_rollTAC;

	private PsUIGenericButton m_settingsButton;

	private static UICanvas m_uiCanvas;

	public static PsUIProfileShield m_versusButton;

	public static PsUIGenericButton m_helpButton;

	public static UIVerticalList m_settingsList;

	public Entity m_vignetteEntity;

	private bool m_released;

	private PsUIGenericButton m_moneyButton;

	private PsUIGenericButton m_noMoneyButton;

	private PsUISwitch m_skipSwitch;

	public static PsUIGenericButton m_createButton;

	private float m_prevRollAngle;

	public static float m_rollInertia;

	public static float m_currentScroll;

	private int m_changePage;

	private float m_rollDrag;

	private float m_rollStartAngle;

	private float m_prevSnapPos;

	private static int m_itemCount = 3;

	public static int m_snapPos;

	public static int m_targetPage = 9999;

	private bool m_forcePage;

	private float m_rollAngle;

	private static float m_angle = 45f;

	public static int m_enterMenuAfterRoll = 9999;

	public static bool m_locked;

	public static int m_forcedSubMenu = -1;

	private static bool m_dragging;

	public override void Enter(IStatedObject _parent)
	{
		PsMenuScene.m_lastState = "PsMainMenuState";
		PsMenuScene.RemoveBlur();
		if (m_vignetteEntity == null)
		{
			m_vignetteEntity = EntityManager.AddEntity();
			TransformC parentTC = TransformS.AddComponent(m_vignetteEntity, new Vector3((float)Screen.width * -0.5f, (float)Screen.height * -0.5f, 100f));
			PrefabC c = PrefabS.AddComponent(parentTC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.MainMenuVignettePrefab_GameObject), "vignette", false);
			PrefabS.SetCamera(c, CameraS.m_uiCamera);
			TransformC transformC = TransformS.AddComponent(m_vignetteEntity, new Vector3((float)Screen.width * 0.5f, (float)Screen.height * -0.5f, 100f));
			PrefabC c2 = PrefabS.AddComponent(transformC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.MainMenuVignettePrefab_GameObject), "vignette", false);
			TransformS.SetScale(transformC, new Vector3(-1f, 1f, 1f));
			PrefabS.SetCamera(c2, CameraS.m_uiCamera);
		}
		m_uiCanvas = new UICanvas(null, false, string.Empty, null, string.Empty);
		m_uiCanvas.SetMargins(0.025f);
		m_uiCanvas.RemoveDrawHandler();
		m_settingsList = new UIVerticalList(m_uiCanvas, string.Empty);
		m_settingsList.RemoveDrawHandler();
		m_settingsList.SetSpacing(0.025f, RelativeTo.ScreenHeight);
		m_settingsList.SetAlign(0f, 1f);
		UIHorizontalList uIHorizontalList = new UIHorizontalList(m_settingsList, string.Empty);
		uIHorizontalList.SetHorizontalAlign(0.25f);
		uIHorizontalList.SetSpacing(0.01f, RelativeTo.ScreenHeight);
		uIHorizontalList.RemoveDrawHandler();
		m_settingsButton = new PsUIGenericButton(uIHorizontalList);
		m_settingsButton.SetIcon("menu_icon_settings_small", 0.052f);
		m_settingsButton.SetTealColors();
		m_settingsButton.SetMargins(0.011f);
		m_settingsButton.SetAlign(0.1f, 0.05f);
		m_helpButton = new PsUIGenericButton(uIHorizontalList);
		m_helpButton.SetText("?", 0.055f, 0.07f);
		m_helpButton.SetTealColors();
		m_helpButton.SetMargins(0.001f);
		m_helpButton.SetAlign(0.1f, 0.05f);
		m_versusButton = new PsUIProfileShield(m_settingsList, true, false);
		m_versusButton.SetDepthOffset(10f);
		m_versusButton.SetSize(0.285f, 0.285f, RelativeTo.ScreenHeight);
		m_createButton = new PsUIGenericButton(m_uiCanvas, 0.25f, 0.25f, 0.02f);
		m_createButton.SetMargins(0.05f);
		m_createButton.SetGlareOffset(-0.0175f);
		m_createButton.SetUpperShineOffset(0.0175f, -0.0185f);
		m_createButton.SetLowerShineOffset(-0.0175f, 0.0185f);
		m_createButton.SetText("CREATE\nLEVEL", 0.045f);
		m_createButton.SetOrangeColors();
		m_createButton.SetAlign(1f, 0.5f);
		if (PsState.m_newEditorItemCount > 0)
		{
			UICanvas uICanvas = new UICanvas(m_createButton, false, "notification", null, string.Empty);
			uICanvas.SetSize(0.06f, 0.06f, RelativeTo.ScreenHeight);
			uICanvas.SetMargins(-0.05f, 0.05f, -0.05f, 0.05f);
			uICanvas.SetRogue();
			uICanvas.SetAlign(0f, 1f);
			uICanvas.SetDepthOffset(-10f);
			uICanvas.RemoveDrawHandler();
			UICanvas uICanvas2 = new UICanvas(uICanvas, false, string.Empty, null, string.Empty);
			uICanvas2.SetSize(1f, 1f, RelativeTo.ParentHeight);
			uICanvas2.SetMargins(0.15f, RelativeTo.OwnHeight);
			uICanvas2.SetDrawHandler(PsUIDrawHandlers.NotificationDrawhandler);
			TweenC c3 = TweenS.AddTransformTween(uICanvas2.m_TC, TweenedProperty.Scale, TweenStyle.CubicInOut, new Vector3(1.1f, 1.1f, 1.1f), 0.5f, 0f, false);
			TweenS.SetAdditionalTweenProperties(c3, -1, true, TweenStyle.CubicInOut);
			UIFittedText uIFittedText = new UIFittedText(uICanvas2, false, string.Empty, PsState.m_newEditorItemCount.ToString(), "KGSecondChances_Font", true);
		}
		if (PsState.m_adminMode)
		{
			UIVerticalList uIVerticalList = new UIVerticalList(m_uiCanvas, "adminButtons");
			uIVerticalList.SetAlign(0f, 0f);
			uIVerticalList.SetSpacing(0.015f, RelativeTo.ScreenHeight);
			uIVerticalList.RemoveDrawHandler();
			m_moneyButton = new PsUIGenericButton(uIVerticalList);
			m_moneyButton.SetGreenColors();
			m_moneyButton.SetText("MONEY!");
			m_moneyButton.SetIcon("menu_resources_coin_icon");
			m_noMoneyButton = new PsUIGenericButton(uIVerticalList);
			m_noMoneyButton.SetOrangeColors();
			m_noMoneyButton.SetText("LESS MONEY!");
			m_noMoneyButton.SetIcon("menu_resources_coin_icon");
			m_skipSwitch = new PsUISwitch(uIVerticalList, PsState.m_adminModeSkipping, 0.03f, "Skipping", "Normal", 0.12f);
		}
		if (!string.IsNullOrEmpty(string.Empty))
		{
			UIText uIText = new UIText(m_uiCanvas, false, string.Empty, string.Empty, "KGSecondChances_Font", 0.03f, RelativeTo.ScreenHeight, "#ffd700", "#000000");
			uIText.SetShadowShift(new Vector2(0.5f, -0.85f));
			uIText.SetAlign(1f, 0f);
		}
		m_uiCanvas.Update();
		m_rollTAC = TouchAreaS.AddRectArea(PsPlanetManager.GetCurrentPlanet().m_rollTC, "Roll", Screen.width, Screen.height, CameraS.m_uiCamera);
		TouchAreaS.AddTouchEventListener(m_rollTAC, AlienPlanetTouchHandler);
		m_released = true;
		m_dragging = false;
		PsMetagameManager.ShowResources(null, false, true, true, true, true, 0.03f, true);
		if (PsState.m_activeGameLoop != null)
		{
			PsState.m_activeGameLoop.BackAtMenu();
		}
	}

	private void ExitCreate()
	{
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new PsMainMenuState());
	}

	private static List<UIComponent> GetHideableUIComponents()
	{
		List<UIComponent> list = new List<UIComponent>();
		if (m_createButton != null)
		{
			list.Add(m_createButton);
		}
		if (m_settingsList != null)
		{
			list.Add(m_settingsList);
		}
		if (PsMetagameManager.m_menuResourceView != null)
		{
			list.Add(PsMetagameManager.m_menuResourceView);
		}
		return list;
	}

	public static void HideUIElementsSwitch()
	{
		List<UIComponent> hideableUIComponents = GetHideableUIComponents();
		for (int i = 0; i < hideableUIComponents.Count; i++)
		{
			TransformC tC = hideableUIComponents[i].m_TC;
			Vector3 position = CameraS.m_uiCamera.WorldToViewportPoint(tC.transform.position);
			int num = (int)(position.x + 0.5f);
			position.x += ((float)num - position.x) * 2f;
			Vector3 endValue = CameraS.m_uiCamera.ViewportToWorldPoint(position);
			TweenS.AddTransformTween(tC, TweenedProperty.Position, TweenStyle.ExpoInOut, endValue, 0.6f, 0f, true);
		}
	}

	public override void Execute()
	{
		PsPlanetManager.Update();
		if (PsPlanetManager.GetCurrentPlanet().m_forcedRoll)
		{
			return;
		}
		if (m_versusButton != null && m_versusButton.m_hit && PsState.m_activeGameLoop == null)
		{
			PsPlanetManager.GetCurrentPlanet().FastForward();
			PsUIBaseState psUIBaseState = null;
			psUIBaseState = ((!PsState.m_adminMode) ? new PsUIBaseState(typeof(PsUITabbedProfile), typeof(PsUIVersusTopBack)) : new PsUIBaseState(typeof(PsUICenterCreate), typeof(PsUIVersusTopBack), typeof(PsUITabbedProfile), typeof(PsUISideCreateAdmin)));
			psUIBaseState.SetAction("Exit", ExitCreate);
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(psUIBaseState);
			PsMenuScene.CreateBlur();
		}
		else if (m_createButton != null && m_createButton.m_hit && PsState.m_activeGameLoop == null)
		{
			PsPlanetManager.GetCurrentPlanet().FastForward();
			PsUIBaseState psUIBaseState2 = null;
			psUIBaseState2 = new PsUIBaseState(typeof(PsUICenterCreate), typeof(PsUITopBackButton));
			psUIBaseState2.SetAction("Exit", ExitCreate);
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(psUIBaseState2);
			PsMenuScene.CreateBlur();
		}
		else if (m_settingsButton != null && m_settingsButton.m_hit && PsState.m_activeGameLoop == null)
		{
			PsPlanetManager.GetCurrentPlanet().FastForward();
			PsUIBaseState psUIBaseState3 = new PsUIBaseState(typeof(PsUICenterSettings));
			psUIBaseState3.SetAction("Exit", delegate
			{
				Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new PsMainMenuState());
			});
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(psUIBaseState3);
			PsMenuScene.CreateBlur();
		}
		else if (m_helpButton != null && m_helpButton.m_hit && PsState.m_activeGameLoop == null)
		{
			PsPlanetManager.GetCurrentPlanet().FastForward();
			HelpshiftManager.ShowFAQ();
		}
		else if (PsState.m_adminMode && m_moneyButton != null && m_moneyButton.m_hit)
		{
			PsPlanetManager.GetCurrentPlanet().FastForward();
			Hashtable data = new Hashtable();
			PsMetagameManager.UpdateResourceStats(10000, 1000, 100, 10);
			PsMetagameManager.SetPlayerData(data, false, PsMetagameManager.PlayerDataSetSUCCEED, PsMetagameManager.PlayerDataSetFAILED);
		}
		else if (PsState.m_adminMode && m_noMoneyButton != null && m_noMoneyButton.m_hit)
		{
			PsPlanetManager.GetCurrentPlanet().FastForward();
			Hashtable data2 = new Hashtable();
			PsMetagameManager.UpdateResourceStats(-Mathf.FloorToInt((float)PsMetagameManager.m_playerStats.coins * 0.5f), -Mathf.FloorToInt((float)PsMetagameManager.m_playerStats.diamonds * 0.5f), -Mathf.FloorToInt((float)PsMetagameManager.m_playerStats.stars * 0.5f), -Mathf.FloorToInt((float)PsMetagameManager.m_playerStats.bolts * 0.5f));
			PsMetagameManager.m_playerStats.keys = 0;
			PsMetagameManager.SetPlayerData(data2, false, PsMetagameManager.PlayerDataSetSUCCEED, PsMetagameManager.PlayerDataSetFAILED);
		}
		if (m_skipSwitch != null && m_skipSwitch.m_hit)
		{
			PsState.m_adminModeSkipping = m_skipSwitch.m_enabled;
		}
		if (m_released)
		{
			m_rollInertia *= 0.85f;
			m_rollDrag = 0f;
		}
		PsPlanet currentPlanet = PsPlanetManager.GetCurrentPlanet();
		float num = m_rollInertia;
		if (m_released)
		{
			if (0f - currentPlanet.m_currentAngle < currentPlanet.m_minAngle)
			{
				num += (currentPlanet.m_minAngle - (0f - currentPlanet.m_currentAngle)) * -0.0927f;
			}
			else if (0f - currentPlanet.m_currentAngle > currentPlanet.m_maxAngle)
			{
				num += (currentPlanet.m_maxAngle - (0f - currentPlanet.m_currentAngle)) * -0.0927f;
			}
		}
		else if (0f - currentPlanet.m_currentAngle < currentPlanet.m_minAngle)
		{
			float num2 = Mathf.Sin((currentPlanet.m_minAngle - (0f - currentPlanet.m_currentAngle)) * ((float)Math.PI / 180f)) * (float)Screen.height;
			float num3 = Mathf.Max(0f, ((float)Screen.height - num2) / (float)Screen.height * 0.618f);
			num *= num3;
		}
		else if (0f - currentPlanet.m_currentAngle > currentPlanet.m_maxAngle)
		{
			float num4 = Mathf.Sin((0f - (currentPlanet.m_maxAngle - (0f - currentPlanet.m_currentAngle))) * ((float)Math.PI / 180f)) * (float)Screen.height;
			float num5 = Mathf.Max(0f, ((float)Screen.height - num4) / (float)Screen.height * 0.618f);
			num *= num5;
		}
		if (Mathf.Abs(num) > 0.01f)
		{
			currentPlanet.m_currentAngle += num;
		}
		else
		{
			m_rollInertia = 0f;
		}
	}

	public void AlienPlanetTouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		if (m_locked)
		{
			return;
		}
		if (_touchPhase == TouchAreaPhase.DragStart)
		{
			m_released = false;
			m_dragging = true;
			m_rollStartAngle = _touches[0].m_currentPosition.y * 90f / (float)Screen.height;
			m_prevRollAngle = m_rollStartAngle;
		}
		else if (m_dragging && (_touchPhase == TouchAreaPhase.MoveIn || _touchPhase == TouchAreaPhase.MoveOut || _touchPhase == TouchAreaPhase.StationaryIn || _touchPhase == TouchAreaPhase.StationaryOut))
		{
			m_rollAngle = _touches[0].m_currentPosition.y * 90f / (float)Screen.height;
			m_rollDrag = _touches[0].m_deltaPosition.y * 90f / (float)Screen.height;
			m_rollInertia = (m_rollInertia + m_rollAngle - m_prevRollAngle) * 0.5f;
			m_prevRollAngle = m_rollAngle;
		}
		else if (_touchPhase == TouchAreaPhase.DragEnd)
		{
			m_released = true;
			m_dragging = false;
			if (_touchArea.m_wasDragged)
			{
				m_rollAngle = _touches[0].m_currentPosition.y * 90f / (float)Screen.height;
				m_rollDrag = _touches[0].m_deltaPosition.y * 90f / (float)Screen.height;
				m_rollInertia = (m_rollInertia + m_rollAngle - m_prevRollAngle) * 0.5f;
				m_prevRollAngle = m_rollAngle;
			}
		}
		if (_touchPhase != TouchAreaPhase.ReleaseIn || _touchArea.m_wasDragged)
		{
			return;
		}
		PsPlanet currentPlanet = PsPlanetManager.GetCurrentPlanet();
		float radius = 12f;
		Ray ray = currentPlanet.m_planetCamera.ScreenPointToRay(_touches[0].m_currentPosition);
		RaycastHit[] array = null;
		array = Physics.SphereCastAll(ray.origin, radius, ray.direction, 1000f, 1 << currentPlanet.m_planetCamera.gameObject.layer);
		TouchAreaC touchAreaC = null;
		float num = 999999f;
		foreach (RaycastHit raycastHit in array)
		{
			float num2 = 999999f;
			TouchAreaBootstrap touchAreaBootstrap = raycastHit.transform.GetComponent("TouchAreaBootstrap") as TouchAreaBootstrap;
			if (touchAreaBootstrap != null)
			{
				Vector2 a = currentPlanet.m_planetCamera.WorldToScreenPoint(touchAreaBootstrap.m_TAC.m_TC.transform.position);
				num2 = Vector2.Distance(a, _touches[0].m_currentPosition);
				if (num2 < num)
				{
					num = num2;
					touchAreaC = touchAreaBootstrap.m_TAC;
				}
			}
		}
		if (touchAreaC != null)
		{
			touchAreaC.m_touchCount = 1;
			_touches[0].m_primaryArea = touchAreaC;
			if (touchAreaC.d_TouchEventDelegate != null)
			{
				touchAreaC.d_TouchEventDelegate(touchAreaC, _touchPhase, _touchIsSecondary, 1, _touches);
			}
			_touchArea.m_touchCount = 0;
		}
	}

	public override void Exit()
	{
		PsMetagameManager.HideResources();
		if (m_vignetteEntity != null)
		{
			EntityManager.RemoveEntity(m_vignetteEntity);
			m_vignetteEntity = null;
		}
		m_versusButton = null;
		m_uiCanvas.Destroy();
		m_uiCanvas = null;
		TouchAreaS.RemoveArea(m_rollTAC);
		m_rollTAC = null;
	}
}
