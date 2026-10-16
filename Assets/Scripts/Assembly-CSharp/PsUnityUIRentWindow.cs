using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsUnityUIRentWindow : UnityUIComponent<PsUnityUIRentWindowBundle>
{
	public List<int> m_rentUpgradeValues;

	public List<KeyValuePair<string, int>> m_upgradesValues;

	private int m_rentTier;

	private int m_rentPrice;

	public PsUnityUIRentWindow(List<KeyValuePair<string, int>> _upgradeValues, List<int> _rentUpgradeValues)
		: base(ResourceManager.GetGameObject(RESOURCE.RentWindow_GameObject))
	{
		m_hidden = true;
		SetUpgradeValues(_upgradeValues, _rentUpgradeValues);
		m_uiBundle.m_banner.m_name.text = string.Empty;
		m_uiBundle.m_rentButtonBundle.m_button.onClick.AddListener(delegate
		{
			if (PsState.m_activeGameLoop.m_freeConsumableUnlock == "Rent")
			{
				RentSucceed();
			}
			else
			{
				new PsRentVehicleFlow(RentSucceed, RentCancelled, PsState.m_activeGameLoop, m_rentPrice);
			}
		});
	}

	public void SetUpgradeValues(List<KeyValuePair<string, int>> _upgradeValues, List<int> _rentUpgradeValues)
	{
		m_upgradesValues = _upgradeValues;
		m_rentUpgradeValues = _rentUpgradeValues;
		m_rentTier = m_rentUpgradeValues[m_rentUpgradeValues.Count - 1];
		m_rentPrice = 0;
		for (int i = 0; i < 3; i++)
		{
			m_uiBundle.m_upgradeTexts[i].text = Vehicle.GetUpgradeName(m_upgradesValues[i].Key).ToUpper();
			int num = m_rentUpgradeValues[i] - m_upgradesValues[i].Value;
			if (num < 0)
			{
				m_uiBundle.m_upgradeValues[i].text = num.ToString();
				m_uiBundle.m_upgradeValues[i].color = Color.red;
			}
			else
			{
				m_uiBundle.m_upgradeValues[i].text = "+" + num;
				m_uiBundle.m_upgradeValues[i].color = Color.green;
			}
			m_rentPrice += num;
		}
		m_rentPrice = (int)((float)m_rentPrice * PsState.m_rentPriceMultiplier);
		if (m_rentPrice < 1)
		{
			m_rentPrice = 0;
		}
		else if (m_rentPrice < PsState.m_minimumRentPrice)
		{
			m_rentPrice = PsState.m_minimumRentPrice;
		}
		int num2 = 1;
		int upgradeValue = PsMetagameManager.m_playerStats.GetUpgradeValue(Type.GetType(PsState.m_activeMinigame.m_playerUnitName), "tier", 1);
		if (m_rentTier > upgradeValue)
		{
			num2 += m_rentTier - upgradeValue;
			if (num2 > 3)
			{
				num2 = 3;
			}
		}
		else if (m_rentTier < upgradeValue)
		{
			num2 = 0;
		}
		if (PsState.m_activeGameLoop.m_freeConsumableUnlock == "Rent")
		{
			m_rentPrice = 0;
		}
		m_uiBundle.m_rentButtonBundle.m_topPrice.SetPrice(m_rentPrice, Resource.Gem);
		m_uiBundle.m_banner.m_leftWingAnimator.SetInteger("tier", m_rentTier);
		m_uiBundle.m_banner.m_rightWingAnimator.SetInteger("tier", m_rentTier);
	}

	public override void Show()
	{
		base.Show();
		if (PsState.m_activeGameLoop.m_rentedVehicle)
		{
			m_uiBundle.m_rentButtonBundle.gameObject.SetActive(false);
		}
		else
		{
			PsIngameMenu.m_controller.DisableController();
		}
		m_uiBundle.m_animator.SetBool("show", true);
		SoundS.PlaySingleShot("/Ingame/Vehicles/SwitchVehicle", Vector2.zero);
	}

	public override void Hide()
	{
		base.Hide();
		m_uiBundle.m_animator.SetBool("show", false);
	}

	public void RentSucceed()
	{
		if (PsMetagameManager.m_menuResourceView == null)
		{
			PsMetagameManager.ShowResources(null, true, true, true, true, false, 0.03f, true);
		}
		PsState.m_activeGameLoop.m_rentedVehicle = true;
		Hashtable progressionJSON = null;
		if (PsState.m_activeGameLoop.m_path != null && PsState.m_activeGameLoop.m_path.m_name != "MainPath")
		{
			progressionJSON = ClientTools.GenerateProgressionPathJson(PsState.m_activeGameLoop.m_path);
		}
		else if (PsState.m_activeGameLoop.m_path != null)
		{
			progressionJSON = ClientTools.GenerateProgressionPathJson(PsState.m_activeGameLoop, PsState.m_activeGameLoop.m_path.m_currentNodeId);
		}
		if (PsState.m_activeGameLoop.m_path != null)
		{
			PsMetagameManager.SaveProgression(progressionJSON, false);
		}
		m_uiBundle.m_rentButtonBundle.gameObject.SetActive(false);
		PsIngameMenu.m_controller.EnableController();
		SoundS.PlaySingleShot("/Ingame/Vehicles/RentVehicle", Vector2.zero);
	}

	public void RentCancelled()
	{
		if (PsMetagameManager.m_menuResourceView == null)
		{
			PsMetagameManager.ShowResources(null, true, true, true, true, false, 0.03f, true);
		}
	}
}
