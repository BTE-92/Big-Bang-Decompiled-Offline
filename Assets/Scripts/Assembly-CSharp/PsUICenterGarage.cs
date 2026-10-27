using System;
using System.Collections.Generic;
using UnityEngine;

public class PsUICenterGarage : UICanvas
{
	private UIRectSpriteButton m_nextButton;

	private UIRectSpriteButton m_prevButton;

	private UIText m_title;

	private UI3DCanvas m_modelCanvas;

	private PrefabC m_modelPrefab;

	private UICanvas m_rightArea;

	private bool m_modelLoaded;

	private PsUnityUIUpgradeBars m_unityUIUpgradeBars;

	private List<KeyValuePair<string, int>> m_upgrades;

	private UIVerticalList m_locked;

	private Type[] m_vehicleTypes;

	private int m_currentVehicleIndex = -1;

	private bool m_resources;

	private PsUIGenericButton m_upgradeTab;

	private PsUIGenericButton m_hatsTab;

	private GarageHatPanel m_hatPanel;

	// tab switch: the panel that is on screen leaves first, then the other one comes in (they never overlap)
	private int m_tabAction;

	private float m_tabTimer;

	private const float UpgradeLeaveTime = 0.6f;

	private const float HatLeaveTime = 0.35f;

	public PsUICenterGarage(UIComponent _parent)
		: base(_parent, false, string.Empty, null, string.Empty)
	{
		UnityUIManager.ShowCanvas();
		m_resources = false;
		CameraS.MoveToBehindOther(UnityUIManager.m_canvas.worldCamera, m_camera);
		m_vehicleTypes = new Type[2]
		{
			typeof(OffroadCar),
			typeof(Motorcycle)
		};
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(1f, RelativeTo.ScreenHeight);
		RemoveDrawHandler();
		UICanvas uICanvas = new UICanvas(this, false, string.Empty, null, string.Empty);
		uICanvas.SetHeight(1f, RelativeTo.ParentHeight);
		uICanvas.SetWidth(0.54f, RelativeTo.ParentWidth);
		uICanvas.SetAlign(0f, 0f);
		uICanvas.SetMargins(0f, 0f, 0f, 0f);
		uICanvas.RemoveDrawHandler();
		m_title = new UIText(uICanvas, false, string.Empty, string.Empty, "KGSecondChances_Font", 0.08f, RelativeTo.ScreenHeight, "#93fe36", "#5c3a0a");
		m_title.SetAlign(0.5f, 0.8f);
		TransformS.SetRotation(m_title.m_TC, new Vector3(0f, 0f, 6f));
		m_modelCanvas = new UI3DCanvas(uICanvas, string.Empty);
		m_modelCanvas.SetHeight(0.8f, RelativeTo.ScreenHeight);
		m_modelCanvas.SetWidth(1f, RelativeTo.ParentWidth);
		m_modelCanvas.SetAlign(0.5f, 0f);
		m_modelCanvas.m_camera.fieldOfView = 20f;
		m_modelCanvas.m_cameraPivot.transform.Rotate(0f, -30f, 0f, Space.World);
		m_modelCanvas.m_cameraPivot.transform.Rotate(15f, 0f, 0f, Space.Self);
		m_modelCanvas.RemoveDrawHandler();
		CameraS.MoveToBehindOther(m_modelCanvas.m_camera, m_camera);
		Frame frame = PsState.m_uiSheet.m_atlas.GetFrame("hud_arrow_selector");
		m_nextButton = new UIRectSpriteButton(uICanvas, string.Empty, PsState.m_uiSheet, frame);
		m_nextButton.SetHeight(0.16f, RelativeTo.ScreenHeight);
		m_nextButton.SetAlign(0.96f, 0.4f);
		frame = PsState.m_uiSheet.m_atlas.GetFrame("hud_arrow_selector");
		frame.flipX = true;
		m_prevButton = new UIRectSpriteButton(uICanvas, string.Empty, PsState.m_uiSheet, frame);
		m_prevButton.SetHeight(0.16f, RelativeTo.ScreenHeight);
		m_prevButton.SetAlign(0.04f, 0.4f);
		ChangeVehicle(0);
		m_rightArea = new UICanvas(this, false, string.Empty, null, string.Empty);
		m_rightArea.SetHeight(1f, RelativeTo.ParentHeight);
		m_rightArea.SetWidth(0.46f, RelativeTo.ParentWidth);
		m_rightArea.SetAlign(1f, 0f);
		m_rightArea.SetMargins(0f, 0f, 0f, 0f);
		m_rightArea.RemoveDrawHandler();
		UIHorizontalList tabs = new UIHorizontalList(m_rightArea, string.Empty);
		tabs.SetAlign(0.5f, 0.86f);
		tabs.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		tabs.RemoveDrawHandler();
		m_upgradeTab = new PsUIGenericButton(tabs);
		m_upgradeTab.SetGreenColors();
		m_upgradeTab.SetText("UPGRADE", 0.04f, 0.2f);
		m_hatsTab = new PsUIGenericButton(tabs);
		m_hatsTab.SetBlueColors();
		m_hatsTab.SetText("HATS", 0.04f, 0.2f);
	}

	private void ShowHats()
	{
		if (m_hatPanel != null || m_tabAction != 0)
		{
			return;
		}
		// the upgrade panel slides out, the hats come in once it is gone
		m_unityUIUpgradeBars.Hide();
		m_tabAction = 1;
		m_tabTimer = UpgradeLeaveTime;
	}

	private void ShowUpgrades()
	{
		if (m_hatPanel == null || m_tabAction != 0)
		{
			return;
		}
		SlideHatPanel(false);
		m_tabAction = 2;
		m_tabTimer = HatLeaveTime;
	}

	private void SlideHatPanel(bool _in)
	{
		Vector3 home = m_hatPanel.m_TC.transform.localPosition;
		Vector3 away = home + Vector3.right * ((float)Screen.width * 0.5f);
		if (_in)
		{
			TweenS.AddTransformTween(m_hatPanel.m_TC, TweenedProperty.Position, TweenStyle.CubicOut, away, home, 0.35f, 0f, true);
		}
		else
		{
			TweenS.AddTransformTween(m_hatPanel.m_TC, TweenedProperty.Position, TweenStyle.CubicIn, home, away, HatLeaveTime, 0f, true);
		}
	}

	private void TickTabs()
	{
		if (m_tabAction == 0)
		{
			return;
		}
		m_tabTimer -= Time.unscaledDeltaTime;
		if (m_tabTimer > 0f)
		{
			return;
		}
		if (m_tabAction == 1)
		{
			m_hatPanel = new GarageHatPanel(m_rightArea);
			m_hatPanel.SetAlign(0.5f, 0f);
			m_rightArea.Update();
			SlideHatPanel(true);
		}
		else
		{
			m_hatPanel.Destroy();
			m_hatPanel = null;
			m_unityUIUpgradeBars.Show();
		}
		m_tabAction = 0;
	}

	private void ChangeVehicle(int _index)
	{
		_index = Mathf.Clamp(_index, 0, m_vehicleTypes.Length - 1);
		if (_index != m_currentVehicleIndex)
		{
			string identifier = m_vehicleTypes[_index].ToString();
			PsUpgradeableEditorItem psUpgradeableEditorItem = PsMetagameData.GetUnlockableByIdentifier(identifier) as PsUpgradeableEditorItem;
			m_title.SetText(psUpgradeableEditorItem.m_name);
			if (m_unityUIUpgradeBars != null)
			{
				CheckButtons(_index);
			}
			m_currentVehicleIndex = _index;
			LoadVehicle();
			if (m_unityUIUpgradeBars != null)
			{
				m_unityUIUpgradeBars.ChangeVehicle(m_upgrades, psUpgradeableEditorItem.m_upgradeSteps, m_vehicleTypes[_index]);
			}
			else
			{
				m_unityUIUpgradeBars = new PsUnityUIUpgradeBars(m_upgrades, psUpgradeableEditorItem.m_upgradeSteps, m_vehicleTypes[_index]);
				m_unityUIUpgradeBars.m_uiBundle.m_switchButton.gameObject.SetActive(false);
				m_unityUIUpgradeBars.Show();
			}
			// the original hid the upgrades of a vehicle that was not unlocked yet; the offline game has every vehicle open
			m_unityUIUpgradeBars.m_uiBundle.m_contentCanvasGroup.alpha = 1f;
			m_unityUIUpgradeBars.m_uiBundle.m_contentCanvasGroup.interactable = true;
		}
	}

	public override void Update()
	{
		base.Update();
		CheckButtons(m_currentVehicleIndex);
		m_modelCanvas.m_cameraOffset = -550f;
	}

	private void LoadVehicle()
	{
		if (m_modelPrefab != null)
		{
			PrefabS.RemoveComponent(m_modelPrefab);
			m_modelPrefab = null;
		}
		Vector3 position = ((m_vehicleTypes[m_currentVehicleIndex] != typeof(OffroadCar)) ? Vector3.zero : (Vector3.right * 9f));
		UpdateUpgradeValues(m_vehicleTypes[m_currentVehicleIndex]);
		m_modelPrefab = m_modelCanvas.AddGameObject(ResourceManager.GetGameObject(m_vehicleTypes[m_currentVehicleIndex].ToString() + "Prefab_GameObject"), position);
		if (m_vehicleTypes[m_currentVehicleIndex] == typeof(OffroadCar))
		{
			Texture tierTexture = GetTierTexture(m_vehicleTypes[m_currentVehicleIndex].ToString(), PsMetagameManager.m_playerStats.GetUpgradeValue(m_vehicleTypes[m_currentVehicleIndex], "tier", 1));
			m_modelPrefab.p_gameObject.transform.Find("OffroadCar/Chassis").GetComponent<Renderer>().sharedMaterial.mainTexture = tierTexture;
		}
		else if (m_vehicleTypes[m_currentVehicleIndex] == typeof(Motorcycle))
		{
			Texture tierTexture2 = GetTierTexture(m_vehicleTypes[m_currentVehicleIndex].ToString(), PsMetagameManager.m_playerStats.GetUpgradeValue(m_vehicleTypes[m_currentVehicleIndex], "tier", 1));
			m_modelPrefab.p_gameObject.transform.Find("DirtBike/Chassis").GetComponentInChildren<Renderer>().sharedMaterial.mainTexture = tierTexture2;
		}
		Transform transform = FindChild(m_modelPrefab.p_gameObject.transform, "Parts");
		if (transform != null)
		{
			transform.gameObject.SetActive(false);
		}
		foreach (KeyValuePair<string, int> upgrade in m_upgrades)
		{
			CheckVisualUpgrade(upgrade.Key, upgrade.Value);
		}
	}

	public Texture GetTierTexture(string _vehicleName, int _tier)
	{
		int num = Mathf.Min(_tier - 1, 4);
		string resourceIdentifier = _vehicleName + "DifT" + num + "_Texture2D";
		return ResourceManager.GetTexture(resourceIdentifier);
	}

	private void UpdateUpgradeValues(Type type)
	{
		m_upgrades = new List<KeyValuePair<string, int>>();
		KeyValuePair<string, int> item = new KeyValuePair<string, int>("power", PsMetagameManager.m_playerStats.GetUpgradeValue(type, "power"));
		m_upgrades.Add(item);
		item = new KeyValuePair<string, int>("grip", PsMetagameManager.m_playerStats.GetUpgradeValue(type, "grip"));
		m_upgrades.Add(item);
		item = new KeyValuePair<string, int>("handling", PsMetagameManager.m_playerStats.GetUpgradeValue(type, "handling"));
		m_upgrades.Add(item);
	}

	public override void Step()
	{
		if (!m_resources && Mathf.Abs((GetRoot() as PsUIBasePopup).m_scrollableCanvas.m_currentScrollX) < 0.1f)
		{
			PsMetagameManager.ShowResources((GetRoot() as PsUIBasePopup).m_overlayCamera, true, true, true, false, true, 0.03f, true);
			m_resources = true;
		}
		else
		{
			m_resources = false;
		}
		if (m_unityUIUpgradeBars.m_followCamera == null)
		{
			m_unityUIUpgradeBars.SetHorizontalScrollCamera(m_camera, m_rightArea.m_TC.transform.position.x);
		}
		if (PsMetagameManager.m_playerStats.upgraded)
		{
			PsMetagameManager.m_playerStats.upgraded = false;
			foreach (KeyValuePair<string, int> upgrade in m_upgrades)
			{
				CheckVisualUpgrade(upgrade.Key, upgrade.Value);
			}
		}
		if (m_unityUIUpgradeBars.m_tierChanged && m_vehicleTypes[m_currentVehicleIndex] == typeof(OffroadCar))
		{
			Texture tierTexture = GetTierTexture(m_vehicleTypes[m_currentVehicleIndex].ToString(), PsMetagameManager.m_playerStats.GetUpgradeValue(m_vehicleTypes[m_currentVehicleIndex], "tier", 1));
			m_modelPrefab.p_gameObject.transform.Find("OffroadCar/Chassis").GetComponent<Renderer>().sharedMaterial.mainTexture = tierTexture;
			m_unityUIUpgradeBars.m_tierChanged = false;
		}
		else if (m_unityUIUpgradeBars.m_tierChanged && m_vehicleTypes[m_currentVehicleIndex] == typeof(Motorcycle))
		{
			Texture tierTexture2 = GetTierTexture(m_vehicleTypes[m_currentVehicleIndex].ToString(), PsMetagameManager.m_playerStats.GetUpgradeValue(m_vehicleTypes[m_currentVehicleIndex], "tier", 1));
			m_modelPrefab.p_gameObject.transform.Find("DirtBike/Chassis").GetComponentInChildren<Renderer>().sharedMaterial.mainTexture = tierTexture2;
			m_unityUIUpgradeBars.m_tierChanged = false;
		}
		TickTabs();
		if (m_hatsTab != null && m_hatsTab.m_hit)
		{
			ShowHats();
		}
		else if (m_upgradeTab != null && m_upgradeTab.m_hit)
		{
			ShowUpgrades();
		}
		if (m_nextButton.m_hit)
		{
			ChangeVehicle(m_currentVehicleIndex + 1);
		}
		else if (m_prevButton.m_hit)
		{
			ChangeVehicle(m_currentVehicleIndex - 1);
		}
		base.Step();
	}

	private void CheckButtons(int newIndex)
	{
		if (m_currentVehicleIndex == 0)
		{
			m_prevButton.EnableTouchAreas();
			m_prevButton.SetColor(DebugDraw.HexToColor("#ffffff"));
		}
		else if (m_currentVehicleIndex == m_vehicleTypes.Length - 1)
		{
			m_nextButton.EnableTouchAreas();
			m_nextButton.SetColor(DebugDraw.HexToColor("#ffffff"));
		}
		if (newIndex == 0)
		{
			m_prevButton.DisableTouchAreas();
			m_prevButton.SetColor(DebugDraw.HexToColor("#666666"));
		}
		else if (newIndex == m_vehicleTypes.Length - 1)
		{
			m_nextButton.DisableTouchAreas();
			m_nextButton.SetColor(DebugDraw.HexToColor("#666666"));
		}
		m_prevButton.Update();
		m_nextButton.Update();
	}

	public void CheckVisualUpgrade(string type, int level)
	{
		Transform transform = null;
		type = char.ToUpper(type[0]) + type.Substring(1);
		int num = 0;
		if (level >= 5)
		{
			num = ((level < 10) ? 1 : ((level < 15) ? 2 : 3));
		}
		for (int i = 0; i < 4; i++)
		{
			transform = FindChild(m_modelPrefab.p_gameObject.transform, type + "T" + i);
			if (transform != null)
			{
				if (i == num)
				{
					transform.gameObject.SetActive(true);
				}
				else
				{
					transform.gameObject.SetActive(false);
				}
			}
		}
	}

	private Transform FindChild(Transform t, string name)
	{
		foreach (Transform item in t)
		{
			Transform transform2 = ((!(item.name == name)) ? FindChild(item, name) : item);
			if (transform2 != null)
			{
				return transform2;
			}
		}
		return null;
	}

	public override void Destroy()
	{
		if (m_unityUIUpgradeBars != null)
		{
			m_unityUIUpgradeBars.Destroy();
			m_unityUIUpgradeBars = null;
		}
		base.Destroy();
	}
}
