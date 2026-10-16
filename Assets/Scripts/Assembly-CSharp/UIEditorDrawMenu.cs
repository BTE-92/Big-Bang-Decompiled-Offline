using System;
using UnityEngine;

public class UIEditorDrawMenu : UIHorizontalList
{
	public UIEditorDrawButtonsWindow m_drawWindow;

	public UIFittedSprite m_materialButton;

	private Entity m_rollEntity;

	private TransformC m_rollTC;

	private float m_prevRollAngle;

	private float m_rollInertia;

	public float m_currentScroll;

	private SpriteC[] m_itemSC;

	private bool m_touching;

	private float m_rollSpeedMultiplier = 1f;

	private int m_itemCount;

	private int m_currentPage;

	private float m_rollStartAngle;

	private float m_minAngle;

	private float m_maxAngle;

	private bool m_forcePage;

	private float m_lastFrameTouchMagnitude;

	private float m_lastFrameRollInertia;

	private Entity[] m_notifications;

	private Entity m_masterNotification;

	private bool m_flinkCanChangePage;

	public UIEditorDrawMenu(UIComponent _parent, string _tag)
		: base(_parent, _tag)
	{
		SetAlign(0f, 0f);
		SetMargins(0.02f, RelativeTo.ScreenShortest);
		SetSpacing(0.02f, RelativeTo.ScreenShortest);
		RemoveDrawHandler();
		if (PsState.m_editorIsLefty)
		{
			PsState.m_drawButtonWindowPosition = 0.975f;
			PsState.m_drawMenuAlign = 1f;
			PsState.m_objectMenuButtonAlign = 0f;
		}
		else
		{
			PsState.m_drawButtonWindowPosition = 0.025f;
			PsState.m_drawMenuAlign = 0f;
			PsState.m_objectMenuButtonAlign = 1f;
		}
		ApplyLeftySettings();
		PsState.m_activeMinigame.InitializeDrawMaterials();
		m_currentPage = PsState.m_activeMinigame.GetMatIndexFromLayerIndex(PsState.m_drawLayer);
		m_forcePage = true;
	}

	public void ApplyLeftySettings()
	{
		if (m_drawWindow != null)
		{
			m_currentPage = PsState.m_activeMinigame.GetMatIndexFromLayerIndex(PsState.m_drawLayer);
			m_forcePage = true;
			if (PsState.m_editorIsLefty)
			{
				OpenDrawWindow(true);
			}
			else
			{
				OpenDrawWindow(false);
			}
			m_drawWindow.SetHorizontalAlign(PsState.m_drawButtonWindowPosition);
			m_drawWindow.Update();
		}
		SetHorizontalAlign(PsState.m_drawMenuAlign);
		Update();
	}

	public void OpenDrawWindow(bool _lefty)
	{
		if (m_drawWindow != null)
		{
			m_drawWindow.Destroy();
		}
		bool flag = true;
		m_drawWindow = new UIEditorDrawButtonsWindow(null, "DrawWindow");
		if (flag)
		{
			float num = (float)Screen.height * 0.25f;
			float num2 = (float)Screen.height * 0.125f;
			float num3 = (float)Screen.height * 0.205f;
			float num4 = (float)Screen.height * 0.175f;
			float num5 = (float)Screen.height * 0.205f;
			float radius = (float)Screen.height * 0.3f;
			if (m_rollEntity != null)
			{
				EntityManager.RemoveEntity(m_rollEntity);
			}
			Vector3 pos = new Vector3((float)Screen.width * -0.5f, (float)Screen.height * -0.5f);
			float num6 = 0f;
			int num7 = 1;
			if (_lefty)
			{
				pos = new Vector3((float)Screen.width * 0.5f, (float)Screen.height * -0.5f);
				num6 = 90f;
				num7 = -1;
			}
			m_itemCount = 6;
			m_minAngle = -45 * (m_itemCount - 1);
			m_maxAngle = 0f;
			m_rollEntity = EntityManager.AddEntity(new string[2] { "DrawMenu", "UIComponent" });
			TransformC transformC = TransformS.AddComponent(m_rollEntity, pos);
			transformC.forceRotation = true;
			Frame frame = PsState.m_uiSheet.m_atlas.GetFrame("hud_wheel_quarter");
			frame.flipX = _lefty;
			SpriteC spriteC = SpriteS.AddComponent(transformC, frame, PsState.m_uiSheet);
			SpriteS.SetSortValue(spriteC, 10f);
			SpriteS.SetOffset(spriteC, new Vector2(num * 0.5f * (float)num7, num * 0.5f), 0f);
			SpriteS.SetDimensions(spriteC, num, num);
			TouchAreaC c = TouchAreaS.AddCircleArea(transformC, "roll", radius, CameraS.m_uiCamera);
			TouchAreaS.AddTouchEventListener(c, RollTouchHandler);
			bool flag2 = false;
			for (int i = 0; i < m_itemCount; i++)
			{
				PsUnlockable psUnlockable = PsMetagameData.m_gameMaterials[0].m_items[i];
				for (int j = 0; j < PsState.m_newEditorItems.Length; j++)
				{
					if (PsState.m_newEditorItems[j] == psUnlockable.m_identifier)
					{
						flag2 = true;
						break;
					}
				}
				if (flag2)
				{
					break;
				}
			}
			if (flag2)
			{
				m_masterNotification = EntityManager.AddEntity("UIComponent");
				TransformC transformC2 = TransformS.AddComponent(m_masterNotification, "MASTERNOTIFICATION");
				TransformS.ParentComponent(transformC2, transformC, new Vector3((!_lefty) ? (0.05f * (float)Screen.height) : (-0.05f * (float)Screen.height), 0.05f * (float)Screen.height, -25f));
				Vector2[] circle = DebugDraw.GetCircle(0.03f * (float)Screen.height, 30, Vector2.zero, true);
				PrefabS.CreatePathPrefabComponentFromVectorArray(transformC2, Vector3.forward * -1f, circle, (float)Screen.height * 0.012f, Color.white, Color.white, ResourceManager.GetMaterial("Framework/Line4Mat"), CameraS.m_uiCamera, Position.Center, true);
				GGData polygon = new GGData(circle);
				Color bottomColor = DebugDraw.HexToColor("#cc0909");
				Color topColor = DebugDraw.HexToColor("#FF0C0C");
				PrefabS.CreateFlatPrefabComponentsFromPolygon(transformC2, Vector3.zero, polygon, bottomColor, topColor, ResourceManager.GetMaterial("Framework/SolidMat"), CameraS.m_uiCamera);
				TextMeshC tmc = TextMeshS.AddComponent(transformC2, Vector3.forward * -2f, "HurmeRegular_Font", 0f, 0f, 20f, Align.Center, Align.Middle, CameraS.m_uiCamera, string.Empty);
				TextMeshS.SetText(tmc, "!");
				TweenC c2 = TweenS.AddTransformTween(transformC2, TweenedProperty.Scale, TweenStyle.CubicInOut, new Vector3(1.15f, 1.15f, 1.15f), 0.5f, 0f, false);
				TweenS.SetAdditionalTweenProperties(c2, -1, true, TweenStyle.CubicInOut);
			}
			m_rollTC = TransformS.AddComponent(m_rollEntity);
			TransformS.SetRotation(m_rollTC, Vector3.forward * (45f + num6 + m_currentScroll));
			TransformS.ParentComponent(m_rollTC, transformC, Vector3.zero);
			TransformC transformC3 = TransformS.AddComponent(m_rollEntity);
			TransformS.ParentComponent(transformC3, transformC, new Vector3(Mathf.Cos((45f + num6) * ((float)Math.PI / 180f)), Mathf.Sin((45f + num6) * ((float)Math.PI / 180f))) * num5);
			Frame frame2 = PsState.m_uiSheet.m_atlas.GetFrame("hud_wheel_selector");
			frame2.flipX = _lefty;
			SpriteC spriteC2 = SpriteS.AddComponent(transformC3, frame2, PsState.m_uiSheet);
			SpriteS.SetDimensions(spriteC2, num4, num4);
			SpriteS.SetSortValue(spriteC2, 5f);
			m_itemSC = new SpriteC[m_itemCount];
			m_notifications = new Entity[m_itemCount];
			float num8 = 0f;
			for (int k = 0; k < m_itemCount; k++)
			{
				TransformC transformC4 = TransformS.AddComponent(m_rollEntity);
				transformC4.forceRotation = true;
				TransformS.ParentComponent(transformC4, m_rollTC, new Vector3(Mathf.Cos(num8 * ((float)Math.PI / 180f)), Mathf.Sin(num8 * ((float)Math.PI / 180f))) * num3);
				string editorWheelFrameName = AutoGeometryManager.m_layers[PsState.m_activeMinigame.m_layerIndexTable[k]].m_groundC.m_ground.m_editorWheelFrameName;
				Frame frame3 = PsState.m_uiSheet.m_atlas.GetFrame(editorWheelFrameName);
				if (PsState.m_editorIsLefty)
				{
					frame3.flipX = true;
				}
				m_itemSC[k] = SpriteS.AddComponent(transformC4, frame3, PsState.m_uiSheet);
				SpriteS.SetDimensions(m_itemSC[k], num2 * 1.2f, num2);
				if (!PsState.m_activeGameLoop.m_gameMode.isMaterialAvailable(k))
				{
					TextMeshC tmc2 = TextMeshS.AddComponent(transformC4, Vector3.zero, "HurmeRegular_Font", 0f, 0f, 20f, Align.Center, Align.Middle, CameraS.m_uiCamera, string.Empty);
					TextMeshS.SetText(tmc2, "Locked");
				}
				if (PsState.m_newEditorItemCount > 0)
				{
					PsUnlockable psUnlockable2 = PsMetagameData.m_gameMaterials[0].m_items[k];
					bool flag3 = false;
					for (int l = 0; l < PsState.m_newEditorItems.Length; l++)
					{
						if (PsState.m_newEditorItems[l] == psUnlockable2.m_identifier)
						{
							flag3 = true;
							break;
						}
					}
					if (flag3)
					{
						m_notifications[k] = EntityManager.AddEntity("UIComponent");
						TransformC transformC5 = TransformS.AddComponent(m_notifications[k], "NOTIFICATION");
						transformC5.forceRotation = true;
						TransformS.ParentComponent(transformC5, transformC4, new Vector3((0f - num2) * 0.5f, num2 * 0.4f, -25f));
						Vector2[] circle2 = DebugDraw.GetCircle(0.02f * (float)Screen.height, 16, Vector2.zero, true);
						PrefabS.CreatePathPrefabComponentFromVectorArray(transformC5, Vector3.forward * -1f, circle2, (float)Screen.height * 0.0075f, Color.white, Color.white, ResourceManager.GetMaterial("Framework/Line4Mat"), CameraS.m_uiCamera, Position.Center, true);
						GGData polygon2 = new GGData(circle2);
						Color bottomColor2 = DebugDraw.HexToColor("#cc0909");
						Color topColor2 = DebugDraw.HexToColor("#FF0C0C");
						PrefabS.CreateFlatPrefabComponentsFromPolygon(transformC5, Vector3.zero, polygon2, bottomColor2, topColor2, ResourceManager.GetMaterial("Framework/SolidMat"), CameraS.m_uiCamera);
						TextMeshC tmc3 = TextMeshS.AddComponent(transformC5, Vector3.forward * -2f, "HurmeRegular_Font", 0f, 0f, 20f, Align.Center, Align.Middle, CameraS.m_uiCamera, string.Empty);
						TextMeshS.SetText(tmc3, "!");
						TweenC c3 = TweenS.AddTransformTween(transformC5, TweenedProperty.Scale, TweenStyle.CubicInOut, new Vector3(1.15f, 1.15f, 1.15f), 0.5f, 0f, false);
						TweenS.SetAdditionalTweenProperties(c3, -1, true, TweenStyle.CubicInOut);
					}
				}
				num8 -= 45f;
			}
			if (m_forcePage)
			{
				m_currentScroll = (float)m_currentPage * 45f;
				TransformS.Rotate(m_rollTC, Vector3.forward * m_currentScroll);
				PsState.m_drawMenuTargetPage = m_currentPage;
				m_forcePage = false;
			}
			Update();
			UpdateMaterialSpriteColors(m_currentPage);
		}
		SwitchIconAndHighlight(false);
		m_drawWindow.RegenerateDrawButtonBackgrounds();
		if (PsState.m_activeGameLoop.m_gameMode.isMaterialAvailable(PsState.m_activeMinigame.GetMatIndexFromLayerIndex(PsState.m_drawLayer)))
		{
			m_drawWindow.SetDrawButtonsActivity(true);
		}
		else
		{
			m_drawWindow.SetDrawButtonsActivity(false);
		}
	}

	private void UpdateMaterialSpriteColors(int _selected)
	{
		float num = 0.35f;
		float num2 = 0.5f;
		Color color = new Color(num, num, num);
		Color color2 = new Color(num2, num2, num2);
		for (int i = 0; i < m_itemCount; i++)
		{
			if (i != _selected)
			{
				SpriteS.SetColor(m_itemSC[i], color);
			}
			else
			{
				SpriteS.SetColor(m_itemSC[i], color2);
			}
		}
	}

	private void RollTouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		Vector2 currentPosition = _touches[0].m_currentPosition;
		if (PsState.m_editorIsLefty)
		{
			currentPosition -= new Vector2(Screen.width, 0f);
		}
		switch (_touchPhase)
		{
		case TouchAreaPhase.Began:
			m_flinkCanChangePage = true;
			m_touching = true;
			m_rollStartAngle = Mathf.Atan2(currentPosition.y, currentPosition.x) * 57.29578f;
			m_prevRollAngle = m_rollStartAngle;
			break;
		case TouchAreaPhase.MoveIn:
		case TouchAreaPhase.MoveOut:
		{
			float num4 = Mathf.Atan2(currentPosition.y, currentPosition.x) * 57.29578f;
			m_rollInertia = num4 - m_prevRollAngle;
			m_prevRollAngle = num4;
			break;
		}
		case TouchAreaPhase.ReleaseIn:
		case TouchAreaPhase.ReleaseOut:
		{
			float num = Mathf.Atan2(currentPosition.y, currentPosition.x) * 57.29578f;
			float num2 = (num - m_prevRollAngle + m_lastFrameRollInertia) / 2f;
			float num3 = (m_lastFrameTouchMagnitude + _touches[0].m_deltaPosition.magnitude) / 2f;
			if (m_flinkCanChangePage && num3 > (float)Screen.width * Main.m_gameDeltaTime * 0.4f)
			{
				if (num2 > 150f * Main.m_gameDeltaTime)
				{
					ChangePage(m_currentPage + 1);
				}
				else if (num2 < -150f * Main.m_gameDeltaTime)
				{
					ChangePage(m_currentPage - 1);
				}
			}
			m_touching = false;
			break;
		}
		}
		m_lastFrameRollInertia = m_rollInertia;
		m_lastFrameTouchMagnitude = _touches[0].m_deltaPosition.magnitude;
	}

	public override void Step()
	{
		if (m_touching)
		{
			int value = Mathf.RoundToInt(m_currentScroll / 45f);
			value = Mathf.Clamp(value, 0, m_itemCount - 1);
			if (value != m_currentPage)
			{
				m_flinkCanChangePage = false;
				ChangePage(value);
			}
		}
		else
		{
			if (PsState.m_drawMenuTargetPage != m_currentPage)
			{
				ChangePage(PsState.m_drawMenuTargetPage);
			}
			else if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				ChangePage(m_currentPage + 1);
			}
			else if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				ChangePage(m_currentPage - 1);
			}
			float num = (float)m_currentPage * 45f;
			if (m_currentScroll != num)
			{
				m_rollInertia = Mathf.Lerp(m_currentScroll, num, 0.1f) - m_currentScroll;
			}
		}
		if (Mathf.Abs(m_rollInertia) > 0f)
		{
			float num2 = -45f;
			float num3 = 45f * (float)m_itemCount;
			float num4 = m_currentScroll + m_rollInertia;
			if (num4 < num2)
			{
				m_rollInertia = num2 - m_currentScroll;
				num4 = num2;
			}
			else if (num4 > num3)
			{
				m_rollInertia = num3 - m_currentScroll;
				num4 = num3;
			}
			m_currentScroll = num4;
			TransformS.Rotate(m_rollTC, Vector3.forward * m_rollInertia);
			m_rollInertia = 0f;
		}
		base.Step();
	}

	private void ChangePage(int _page)
	{
		m_currentPage = Mathf.Clamp(_page, 0, m_itemCount - 1);
		PsState.m_drawMenuTargetPage = m_currentPage;
		if (m_notifications[m_currentPage] != null)
		{
			PsMetagameManager.RemoveNewEditorItem(PsMetagameData.m_gameMaterials[0].m_items[m_currentPage].m_identifier);
			EntityManager.RemoveEntity(m_notifications[m_currentPage], true, true);
			m_notifications[m_currentPage] = null;
			bool flag = false;
			for (int i = 0; i < m_notifications.Length; i++)
			{
				if (m_notifications[i] != null)
				{
					flag = true;
					break;
				}
			}
			if (!flag && m_masterNotification != null)
			{
				EntityManager.RemoveEntity(m_masterNotification, true, true);
				m_masterNotification = null;
			}
		}
		if (PsState.m_activeMinigame != null)
		{
			PsState.m_drawLayer = PsState.m_activeMinigame.m_layerIndexTable[m_currentPage];
		}
		UpdateMaterialSpriteColors(m_currentPage);
		SwitchIconAndHighlight();
		if (PsState.m_activeGameLoop.m_gameMode.isMaterialAvailable(m_currentPage))
		{
			m_drawWindow.SetDrawButtonsActivity(true);
			m_drawWindow.RegenerateDrawButtonBackgrounds();
		}
		else
		{
			m_drawWindow.SetDrawButtonsActivity(false);
		}
		AutoGeometryLayer selectedLayer = AutoGeometryManager.m_layers[PsState.m_drawLayer];
		AutoGeometryManager.UpdateMaxValueLookupTable(selectedLayer, LookupUpdateMode.IN_EDITOR);
	}

	public void CloseDrawWindow()
	{
		if (m_rollEntity != null)
		{
			EntityManager.RemoveEntity(m_rollEntity, true, true);
		}
		for (int i = 0; i < m_notifications.Length; i++)
		{
			if (m_notifications[i] != null)
			{
				EntityManager.RemoveEntity(m_notifications[i], true, true);
				m_notifications[i] = null;
			}
		}
		if (m_masterNotification != null)
		{
			EntityManager.RemoveEntity(m_masterNotification, true, true);
			m_masterNotification = null;
		}
		if (m_drawWindow != null)
		{
			m_drawWindow.Destroy();
			m_drawWindow = null;
		}
		if (AutoGeometryManager.m_layers.Count > PsState.m_drawLayer)
		{
			AutoGeometryLayer layer = AutoGeometryManager.m_layers[PsState.m_drawLayer];
			AutoGeometryManager.HighlightLayer(layer, 0f);
		}
	}

	public void SwitchIconAndHighlight(bool _playSound = true)
	{
		if (_playSound)
		{
			SoundS.PlaySingleShot("/UI/ButtonSwitchMaterial", Vector3.zero);
		}
		AutoGeometryLayer layer = AutoGeometryManager.m_layers[PsState.m_drawLayer];
		AutoGeometryManager.HighlightLayer(layer, 0.3f);
	}

	public override void Destroy()
	{
		if (m_drawWindow != null)
		{
			CloseDrawWindow();
		}
		base.Destroy();
	}
}
