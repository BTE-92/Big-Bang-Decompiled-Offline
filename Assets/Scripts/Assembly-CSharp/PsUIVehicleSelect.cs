using System;
using System.Collections.Generic;
using UnityEngine;

public class PsUIVehicleSelect : UICanvas
{
	private class AnimationEndEvent
	{
		private Animator m_animator;

		private string m_animationName;

		private Action m_event;

		public AnimationEndEvent(Animator _animator, string _stateName, Action _event)
		{
			m_animator = _animator;
			m_animationName = _stateName;
			m_event = _event;
		}

		public bool Check()
		{
			if (m_animator == null)
			{
				return false;
			}
			AnimatorStateInfo currentAnimatorStateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.normalizedTime >= 1f && currentAnimatorStateInfo.IsName(m_animationName))
			{
				m_event();
				return true;
			}
			return false;
		}
	}

	public List<int> m_rentVehicleValues;

	public List<KeyValuePair<string, int>> m_deffaultVehicleUpgrades;

	private PsUnityUIUpgradeBars m_unityUIUpgradeBars;

	private PsUnityUIRentWindow m_unityUIRentWindow;

	private int m_rentPrice;

	private PsSelectedVehicle m_selectedVehicle;

	private PsSelectedVehicle m_deffaultVehicle;

	private UIRectSpriteButton m_nextButton;

	private UIRectSpriteButton m_prevButton;

	private UnityUIComponent<PsUnityUIVehicleButtonsBundle> m_vehicleButtons;

	private PsUITutorialArrowForUnityUI m_tutorialArrow;

	private bool m_checkAnimation;

	private List<AnimationEndEvent> m_AnimationEvents;

	private PsUITutorialArrowUI m_gasTutorialArrow;

	private bool m_createGasTutorialArrow;

	public PsUIVehicleSelect(UIComponent _parent)
		: base(_parent, false, "vehicleSelect", null, string.Empty)
	{
		m_AnimationEvents = new List<AnimationEndEvent>();
		PsIngameMenu.OpenController(true);
		RemoveDrawHandler();
		bool unlocked = PsMetagameData.GetUnlockableByIdentifier("Upgrade").m_unlocked;
		bool unlocked2 = PsMetagameData.GetUnlockableByIdentifier("Rent").m_unlocked;
		m_selectedVehicle = PsSelectedVehicle.None;
		if (PsState.m_activeGameLoop.m_userHasSelectedVehicle)
		{
			m_selectedVehicle = PsState.m_activeGameLoop.m_selectedVehicle;
		}
		else
		{
			if (PsState.m_activeGameLoop.m_minigameMetaData.playerUnit == "Motorcycle")
			{
				m_selectedVehicle = PsSelectedVehicle.Motorcycle;
			}
			else
			{
				m_selectedVehicle = PsSelectedVehicle.OffroadCar;
			}
			PsState.m_activeGameLoop.m_selectedVehicle = m_selectedVehicle;
		}
		if (PsState.m_activeGameLoop.m_minigameMetaData.playerUnit == "Motorcycle")
		{
			m_deffaultVehicle = PsSelectedVehicle.Motorcycle;
		}
		else
		{
			m_deffaultVehicle = PsSelectedVehicle.OffroadCar;
		}
		RefreshUpgradeValues();
		if (unlocked || unlocked2)
		{
			(PsState.m_activeMinigame.m_playerUnit as Vehicle).m_alienCharacter.SetVehicleSelectTarget(400f, 300f, new Vector2(75f, 35f));
		}
		CameraS.SnapMainCameraFrame();
		if (!unlocked)
		{
			return;
		}
		CameraS.MoveToBehindOther(UnityUIManager.m_canvas.worldCamera, CameraS.m_uiCamera);
		m_unityUIUpgradeBars = new PsUnityUIUpgradeBars(PsState.m_activeMinigame.m_playerUnit.GetUpgrades(), (PsState.m_activeMinigame.m_playerUnit as Vehicle).m_upgradeSteps, PsState.m_activeMinigame.m_playerUnit.GetType());
		m_unityUIUpgradeBars.Show();
		m_unityUIUpgradeBars.m_uiBundle.m_levelUpButton.onClick.AddListener(ResetPlayer);
		if (PsMetagameData.GetUnlockableByIdentifier("Motorcycle").m_unlocked)
		{
			m_vehicleButtons = new UnityUIComponent<PsUnityUIVehicleButtonsBundle>(ResourceManager.GetGameObject(RESOURCE.VehicleSelectionButtons_GameObject));
			m_vehicleButtons.m_uiBundle.m_offroadCar.m_button.gameObject.SetActive(false);
			m_vehicleButtons.m_uiBundle.m_motorcycle.m_button.onClick.AddListener(delegate
			{
				m_vehicleButtons.m_uiBundle.m_motorcycle.m_button.interactable = false;
				m_vehicleButtons.m_uiBundle.m_offroadCar.m_button.interactable = true;
				m_vehicleButtons.m_uiBundle.m_rent.m_button.interactable = true;
				UnityUIManager.DisableEventSystem();
				PsIngameMenu.m_controller.DisabletouchAreas();
				PsState.m_activeGameLoop.m_selectedVehicle = PsSelectedVehicle.Motorcycle;
				PsState.m_activeGameLoop.m_userHasSelectedVehicle = true;
				Action action = delegate
				{
					SwitchPlayerCharacter();
					ChangeVehicle();
					m_unityUIUpgradeBars.Show();
					UnityUIManager.EnableEventSystem();
				};
				AnimationEndEvent item;
				if (m_unityUIRentWindow != null && !m_unityUIRentWindow.m_hidden)
				{
					m_unityUIRentWindow.Hide();
					item = new AnimationEndEvent(m_unityUIRentWindow.m_uiBundle.m_animator, "RentalDisappear", action);
				}
				else
				{
					m_unityUIUpgradeBars.Hide();
					item = new AnimationEndEvent(m_unityUIUpgradeBars.m_uiBundle.m_animator, "MyCarDisappear", action);
				}
				m_AnimationEvents.Add(item);
			});
			if (PsState.m_activeGameLoop.m_selectedVehicle == PsSelectedVehicle.OffroadCar)
			{
				m_vehicleButtons.m_uiBundle.m_offroadCar.m_button.interactable = false;
			}
			else if (PsState.m_activeGameLoop.m_selectedVehicle == PsSelectedVehicle.Motorcycle)
			{
				m_vehicleButtons.m_uiBundle.m_motorcycle.m_button.interactable = false;
			}
			if (unlocked2)
			{
				m_unityUIRentWindow = new PsUnityUIRentWindow(m_deffaultVehicleUpgrades, m_rentVehicleValues);
				m_vehicleButtons.m_uiBundle.m_rent.m_button.onClick.AddListener(delegate
				{
					m_vehicleButtons.m_uiBundle.m_rent.m_button.interactable = false;
					m_vehicleButtons.m_uiBundle.m_offroadCar.m_button.interactable = true;
					m_vehicleButtons.m_uiBundle.m_motorcycle.m_button.interactable = true;
					UnityUIManager.DisableEventSystem();
					PsIngameMenu.m_controller.DisabletouchAreas();
					m_selectedVehicle = PsState.m_activeGameLoop.m_selectedVehicle;
					PsState.m_activeGameLoop.m_selectedVehicle = PsSelectedVehicle.CreatorVehicle;
					PsState.m_activeGameLoop.m_userHasSelectedVehicle = true;
					Action action = delegate
					{
						SwitchPlayerCharacter();
						RefreshUpgradeValues();
						ResetPlayer();
						m_unityUIRentWindow.SetUpgradeValues(m_deffaultVehicleUpgrades, m_rentVehicleValues);
						m_unityUIRentWindow.Show();
						UnityUIManager.EnableEventSystem();
					};
					m_unityUIUpgradeBars.Hide();
					AnimationEndEvent item = new AnimationEndEvent(m_unityUIUpgradeBars.m_uiBundle.m_animator, "MyCarDisappear", action);
					m_AnimationEvents.Add(item);
				});
				if (PsState.m_activeGameLoop.m_selectedVehicle == PsSelectedVehicle.CreatorVehicle)
				{
					m_vehicleButtons.m_uiBundle.m_rent.m_button.interactable = false;
				}
				if (m_selectedVehicle == PsSelectedVehicle.CreatorVehicle)
				{
					m_unityUIUpgradeBars.Hide();
					m_unityUIRentWindow.Show();
				}
				if (!PsState.m_activeGameLoop.m_rentedVehicle && PsState.m_activeGameLoop is PsGameLoopBlockLevel && (PsState.m_activeGameLoop as PsGameLoopBlockLevel).m_unlockedProgression.Equals("unlock_rentals"))
				{
					TouchAreaS.Disable();
					CameraS.BringToFront(UnityUIManager.m_canvas.worldCamera);
					m_tutorialArrow = new PsUITutorialArrowForUnityUI(m_vehicleButtons.m_uiBundle.m_rent.transform, new Vector2(-0.09f, -0.022f));
					m_vehicleButtons.m_uiBundle.m_rent.m_button.onClick.AddListener(delegate
					{
						if (m_tutorialArrow != null)
						{
							if (m_tutorialArrow != null && !m_tutorialArrow.m_destroyed)
							{
								m_tutorialArrow.Destroy();
								m_tutorialArrow = null;
							}
							m_tutorialArrow = new PsUITutorialArrowForUnityUI(m_unityUIRentWindow.m_uiBundle.m_rentButtonBundle.transform, new Vector2(-0.16f, -0.022f), true);
							AnimationEndEvent item = new AnimationEndEvent(m_unityUIRentWindow.m_uiBundle.m_animator, "RentalAppear", delegate
							{
								m_tutorialArrow.CreateArrow();
								m_checkAnimation = false;
							});
							m_AnimationEvents.Add(item);
						}
					});
					m_unityUIRentWindow.m_uiBundle.m_rentButtonBundle.m_button.onClick.AddListener(delegate
					{
						if (m_tutorialArrow != null)
						{
							if (m_tutorialArrow != null && !m_tutorialArrow.m_destroyed)
							{
								m_tutorialArrow.Destroy();
								m_tutorialArrow = null;
							}
							CameraS.MoveToBehindOther(UnityUIManager.m_canvas.worldCamera, CameraS.m_uiCamera);
							m_createGasTutorialArrow = true;
						}
					});
				}
			}
			else
			{
				m_vehicleButtons.m_uiBundle.m_rent.gameObject.SetActive(false);
			}
		}
		if (!(PsState.m_activeGameLoop is PsGameLoopBlockLevel) || !(PsState.m_activeGameLoop as PsGameLoopBlockLevel).m_unlockedProgression.Equals("unlock_upgrades"))
		{
			return;
		}
		m_unityUIUpgradeBars.m_uiBundle.m_animator.Play("MyVehicleVisible");
		for (int num = 0; num < 3; num++)
		{
			if (m_deffaultVehicleUpgrades[num].Value > 0)
			{
				CameraS.MoveToBehindOther(UnityUIManager.m_canvas.worldCamera, CameraS.m_uiCamera);
				m_createGasTutorialArrow = true;
				break;
			}
			if (m_deffaultVehicleUpgrades[num].Value > 0)
			{
				continue;
			}
			if (m_tutorialArrow == null)
			{
				CameraS.BringToFront(UnityUIManager.m_canvas.worldCamera);
				m_tutorialArrow = new PsUITutorialArrowForUnityUI(m_unityUIUpgradeBars.m_uiBundle.m_content.transform.GetChild(num), new Vector2(-0.16f, -0.022f));
			}
			int nextIndex = num + 1;
			m_unityUIUpgradeBars.m_uiBundle.m_buttonBundles[num].m_button.onClick.AddListener(delegate
			{
				if (m_unityUIUpgradeBars.m_resetPlayer && m_tutorialArrow != null)
				{
					if (m_tutorialArrow != null && !m_tutorialArrow.m_destroyed)
					{
						m_tutorialArrow.Destroy();
						m_tutorialArrow = null;
					}
					if (nextIndex < 3)
					{
						m_tutorialArrow = new PsUITutorialArrowForUnityUI(m_unityUIUpgradeBars.m_uiBundle.m_content.transform.GetChild(nextIndex), new Vector2(-0.16f, -0.022f));
					}
					else
					{
						CameraS.MoveToBehindOther(UnityUIManager.m_canvas.worldCamera, CameraS.m_uiCamera);
						m_createGasTutorialArrow = true;
					}
				}
			});
		}
	}

	public void RefreshUpgradeValues()
	{
		m_rentVehicleValues = PsState.m_activeGameLoop.GetRentVehicleValues();
		PsState.m_activeGameLoop.m_rentUpgradeValues = m_rentVehicleValues;
		Type rentVehicleType = PsState.m_activeGameLoop.GetRentVehicleType();
		m_deffaultVehicleUpgrades = new List<KeyValuePair<string, int>>();
		KeyValuePair<string, int> item = new KeyValuePair<string, int>("power", PsMetagameManager.m_playerStats.GetUpgradeValue(rentVehicleType, "power"));
		m_deffaultVehicleUpgrades.Add(item);
		item = new KeyValuePair<string, int>("grip", PsMetagameManager.m_playerStats.GetUpgradeValue(rentVehicleType, "grip"));
		m_deffaultVehicleUpgrades.Add(item);
		item = new KeyValuePair<string, int>("handling", PsMetagameManager.m_playerStats.GetUpgradeValue(rentVehicleType, "handling"));
		m_deffaultVehicleUpgrades.Add(item);
	}

	public void ChangeVehicle()
	{
		m_unityUIUpgradeBars.ChangeVehicle(PsState.m_activeMinigame.m_playerUnit.GetUpgrades(), (PsState.m_activeMinigame.m_playerUnit as Vehicle).m_upgradeSteps, PsState.m_activeMinigame.m_playerUnit.GetType());
	}

	public void SwitchPlayerCharacter()
	{
		PsSelectedVehicle psSelectedVehicle = PsState.m_activeGameLoop.m_selectedVehicle;
		if (psSelectedVehicle == PsSelectedVehicle.CreatorVehicle)
		{
			psSelectedVehicle = (PsSelectedVehicle)(int)Enum.Parse(typeof(PsSelectedVehicle), PsState.m_activeGameLoop.GetRentVehicleType().ToString());
		}
		PsState.m_activeMinigame.m_playerNode.m_assembleClassType = Type.GetType(psSelectedVehicle.ToString());
		ResetPlayer();
		Debug.LogWarning(psSelectedVehicle);
	}

	public void ResetPlayer()
	{
		PsState.m_activeMinigame.m_playerNode.Reset();
		PsIngameMenu.OpenController(true);
		(PsState.m_activeMinigame.m_playerUnit as Vehicle).m_alienCharacter.SetVehicleSelectTarget(400f, 300f, new Vector2(75f, 35f));
		CameraS.SnapMainCameraFrame();
	}

	public override void Destroy()
	{
		if (m_unityUIUpgradeBars != null)
		{
			m_unityUIUpgradeBars.Destroy();
			m_unityUIUpgradeBars = null;
		}
		if (m_unityUIRentWindow != null)
		{
			m_unityUIRentWindow.Destroy();
			m_unityUIRentWindow = null;
		}
		if (m_vehicleButtons != null)
		{
			m_vehicleButtons.Destroy();
			m_vehicleButtons = null;
		}
		if (m_tutorialArrow != null && !m_tutorialArrow.m_destroyed)
		{
			m_tutorialArrow.Destroy();
			m_tutorialArrow = null;
		}
		if (m_gasTutorialArrow != null && !m_gasTutorialArrow.m_desroyed)
		{
			UnityUIManager.EnableEventSystem();
			m_gasTutorialArrow.Destroy();
			m_gasTutorialArrow = null;
		}
		UnityUIManager.EnableEventSystem();
		base.Destroy();
	}

	public override void Step()
	{
		base.Step();
		for (int i = 0; i < m_AnimationEvents.Count; i++)
		{
			if (m_AnimationEvents[i].Check())
			{
				m_AnimationEvents.RemoveAt(i);
			}
		}
		if (m_unityUIUpgradeBars != null && m_unityUIUpgradeBars.m_resetPlayer)
		{
			ResetPlayer();
			m_unityUIUpgradeBars.m_resetPlayer = false;
		}
		if (m_gasTutorialArrow == null && m_createGasTutorialArrow)
		{
			Controller.m_controllerDisabled = false;
			UnityUIManager.DisableEventSystem();
			m_gasTutorialArrow = new PsUITutorialArrowUI((PsIngameMenu.m_controller as MotorcycleController).m_rightButton, false);
			m_createGasTutorialArrow = false;
		}
		if (m_tutorialArrow != null)
		{
			Controller.m_controllerDisabled = true;
		}
	}
}
