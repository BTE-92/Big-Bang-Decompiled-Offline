using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsUnityUIUpgradeBars : UnityUIComponent<PsUnityUIUpgradeBarsBundle>
{
	private List<KeyValuePair<string, int>> m_upgrades = new List<KeyValuePair<string, int>>();

	private int[] m_coinPrices;

	private int[] m_boltPrices;

	private int m_maxUpgrades;

	private string[] m_prices;

	private Type m_playerUnit;

	private int m_tier;

	public bool m_tierChanged;

	public bool m_resetPlayer;

	private PsUIBasePopup m_popup;

	public int m_selectedButtonIndex = -1;

	private PsUnityUIUpgradeBannerBundle m_banner;

	private Animator m_bannerAnimator;

	public PsUnityUIUpgradeBars(List<KeyValuePair<string, int>> _upgrades, int _maxUpgrades, Type _playerUnit)
		: base(ResourceManager.GetGameObject(RESOURCE.UpgradeWindow_GameObject))
	{
		m_hidden = true;
		ChangeVehicle(_upgrades, _maxUpgrades, _playerUnit);
		m_uiBundle.m_levelUpButton.onClick.AddListener(SetTierChange);
	}

	public override void Show()
	{
		base.Show();
		m_uiBundle.m_animator.SetBool("show", true);
	}

	public override void Hide()
	{
		base.Hide();
		m_uiBundle.m_animator.SetBool("show", false);
	}

	public void ChangeVehicle(List<KeyValuePair<string, int>> _upgrades, int _maxUpgrades, Type _playerUnit)
	{
		if (m_selectedButtonIndex >= 0)
		{
			HidePrice(m_selectedButtonIndex);
		}
		m_upgrades = _upgrades;
		m_maxUpgrades = _maxUpgrades;
		m_playerUnit = _playerUnit;
		m_tier = PsMetagameManager.m_playerStats.GetUpgradeValue(_playerUnit, "tier", 1);
		m_tierChanged = false;
		m_resetPlayer = false;
		PsUpgradeableEditorItem psUpgradeableEditorItem = PsMetagameData.GetUnlockableByIdentifier(_playerUnit.ToString()) as PsUpgradeableEditorItem;
		m_prices = psUpgradeableEditorItem.m_upgradePrices;
		for (int i = 0; i < 3; i++)
		{
			int index = i;
			m_uiBundle.m_texts[i].text = Loc.T(Vehicle.GetUpgradeName(m_upgrades[i].Key).ToUpper());
			if (m_uiBundle.m_buttonBundles[i].m_button.onClick != null)
			{
				m_uiBundle.m_buttonBundles[i].m_button.onClick.RemoveAllListeners();
			}
			m_uiBundle.m_buttonBundles[i].m_button.onClick.AddListener(delegate
			{
				OnClick(index);
			});
			m_uiBundle.m_buttonBundles[index].m_price1.SetPrice(0, Resource.Coin);
			m_uiBundle.m_buttonBundles[index].m_price2.SetPrice(0, Resource.Bolt);
		}
		foreach (Transform item in m_uiBundle.m_banners.transform)
		{
			if (item.name.Equals(m_playerUnit.ToString()))
			{
				item.gameObject.SetActive(true);
				m_banner = item.GetComponent<PsUnityUIUpgradeBannerBundle>();
				m_bannerAnimator = m_uiBundle.m_banners.GetComponent<Animator>();
				m_bannerAnimator.Play("default");
			}
			else
			{
				item.gameObject.SetActive(false);
			}
		}
		SetTier();
		SetFilling();
		ParseCurrentPrices();
		m_banner.m_name.text = Loc.T(string.Empty);
		if (CanLevelUp())
		{
			ShowLevelUpButton();
		}
		else
		{
			HideLevelUpButton();
		}
		SoundS.PlaySingleShot("/Ingame/Vehicles/SwitchVehicle", Vector2.zero);
	}

	private void OnClick(int _upgradeIndex)
	{
		if (m_selectedButtonIndex == _upgradeIndex)
		{
			Upgrade(_upgradeIndex);
			HidePrice(_upgradeIndex);
		}
		else
		{
			ShowPrice(_upgradeIndex);
		}
	}

	private void ParseCurrentPrices()
	{
		m_coinPrices = new int[3];
		m_boltPrices = new int[3];
		for (int i = 0; i < 3; i++)
		{
			int value = m_upgrades[i].Value;
			int num = 0;
			int num2 = 0;
			if (value < m_maxUpgrades)
			{
				string[] array = m_prices[value].Split(' ');
				string empty = string.Empty;
				string empty2 = string.Empty;
				for (int j = 0; j < array.Length; j++)
				{
					empty = array[j].Substring(array[j].Length - 1, 1);
					empty2 = array[j].Substring(0, array[j].Length - 1);
					if (empty.Equals("g"))
					{
						m_coinPrices[i] = Convert.ToInt32(empty2);
					}
					if (empty.Equals("b"))
					{
						m_boltPrices[i] = Convert.ToInt32(empty2);
					}
				}
			}
			m_uiBundle.m_buttonBundles[i].m_price1.SetPrice(m_coinPrices[i]);
			if (m_coinPrices[i] != 0)
			{
				m_uiBundle.m_buttonBundles[i].m_price1.gameObject.SetActive(true);
			}
			else
			{
				m_uiBundle.m_buttonBundles[i].m_price1.gameObject.SetActive(false);
			}
			m_uiBundle.m_buttonBundles[i].m_price2.SetPrice(m_boltPrices[i]);
			if (m_boltPrices[i] != 0)
			{
				m_uiBundle.m_buttonBundles[i].m_price2.gameObject.SetActive(true);
			}
			else
			{
				m_uiBundle.m_buttonBundles[i].m_price2.gameObject.SetActive(false);
			}
		}
	}

	private void SetFilling()
	{
		for (int i = 0; i < 3; i++)
		{
			int num = m_upgrades[i].Value - (m_tier - 1) * 5;
			m_uiBundle.m_buttonBundles[i].m_button.interactable = num < 5 && m_upgrades[i].Value < m_maxUpgrades;
			m_uiBundle.m_fillerAnimators[i].SetInteger("fillStep", (m_tier != 5) ? num : 5);
			m_selectedButtonIndex = -1;
			m_uiBundle.m_buttonBundles[i].m_prices.SetActive(false);
			m_uiBundle.m_buttonBundles[i].m_icon.gameObject.SetActive(true);
		}
	}

	private void SetTier()
	{
		m_bannerAnimator.SetInteger("tier", m_tier);
		if (m_tier > 1)
		{
			m_banner.m_leftWingAnimator.gameObject.SetActive(true);
			m_banner.m_rightWingAnimator.gameObject.SetActive(true);
			m_banner.m_leftWingAnimator.SetInteger("tier", m_tier);
			m_banner.m_rightWingAnimator.SetInteger("tier", m_tier);
		}
		else
		{
			m_banner.m_leftWingAnimator.gameObject.SetActive(false);
			m_banner.m_rightWingAnimator.gameObject.SetActive(false);
		}
	}

	private void Upgrade(int _upgradeIndex)
	{
		if (PsMetagameManager.m_playerStats.bolts < m_boltPrices[_upgradeIndex])
		{
			UnityUIManager.DisableEventSystem();
			PsUIBasePopup popup = new PsUIBasePopup(typeof(PsUIPopupNotEnoughBolts), null, null, null, false);
			popup.SetAction("Proceed", UnityUIManager.EnableEventSystem);
			popup.SetAction("Exit", delegate
			{
				popup.Destroy();
				UnityUIManager.EnableEventSystem();
			});
			popup.BringToFront();
		}
		else if (PsMetagameManager.m_playerStats.coins < m_coinPrices[_upgradeIndex])
		{
			TouchAreaS.Disable();
			OpenCoinPopup(_upgradeIndex);
		}
		else
		{
			PsMetagameManager.m_playerStats.CumulateCoins(-m_coinPrices[_upgradeIndex]);
			PsMetagameManager.m_playerStats.CumulateBolts(-m_boltPrices[_upgradeIndex]);
			SetUpgrade(_upgradeIndex);
			m_resetPlayer = true;
		}
	}

	private void ShowPrice(int _upgradeIndex)
	{
		if (m_selectedButtonIndex >= 0)
		{
			HidePrice(m_selectedButtonIndex);
		}
		m_uiBundle.m_fillerAnimators[_upgradeIndex].SetBool("highlightNext", true);
		m_selectedButtonIndex = _upgradeIndex;
		if (m_boltPrices[_upgradeIndex] == 0 && m_coinPrices[_upgradeIndex] == 0)
		{
			m_uiBundle.m_buttonBundles[_upgradeIndex].m_text.text = Loc.T("FREE");
			m_uiBundle.m_buttonBundles[_upgradeIndex].m_text.gameObject.SetActive(true);
		}
		else
		{
			m_uiBundle.m_buttonBundles[_upgradeIndex].m_prices.SetActive(true);
		}
		m_uiBundle.m_buttonBundles[_upgradeIndex].m_icon.gameObject.SetActive(false);
		SoundS.PlaySingleShot("/UI/ButtonConfirm", Vector2.zero);
	}

	private void HidePrice(int _upgradeIndex)
	{
		if (_upgradeIndex >= 0)
		{
			m_selectedButtonIndex = -1;
			m_uiBundle.m_fillerAnimators[_upgradeIndex].SetBool("highlightNext", false);
			m_uiBundle.m_fillerAnimators[_upgradeIndex].SetInteger("fillStep", m_upgrades[_upgradeIndex].Value - (m_tier - 1) * 5);
			m_uiBundle.m_buttonBundles[_upgradeIndex].m_text.gameObject.SetActive(false);
			m_uiBundle.m_buttonBundles[_upgradeIndex].m_prices.SetActive(false);
			m_uiBundle.m_buttonBundles[_upgradeIndex].m_icon.gameObject.SetActive(true);
		}
	}

	private void OpenBoltPopup(int _upgradeIndex)
	{
	}

	private void OpenCoinPopup(int _upgradeIndex)
	{
		TouchAreaS.Enable();
		PsMetagameManager.m_coinsToDiamondsConvertAmount = m_coinPrices[_upgradeIndex] - PsMetagameManager.m_playerStats.coins;
		m_popup = new PsUIBasePopup(typeof(PsUICenterNotEnoughCoins));
		m_popup.SetAction("Exit", DestroyPopup);
		m_popup.SetAction("Upgrade", delegate
		{
			UpgradeWithDiamonds(_upgradeIndex);
		});
		TweenS.AddTransformTween(m_popup.m_mainContent.m_parent.m_TC, TweenedProperty.Scale, TweenStyle.ElasticOut, Vector3.one * 0.75f, Vector3.one, 0.75f, 0f, true);
		PsMetagameManager.ShowResources(null, true, true, true, false, false);
	}

	public void UpgradeWithDiamonds(int _upgradeIndex)
	{
		DestroyPopup();
		PsMetrics.ConvertDiamondsToCoins(PsMetagameManager.CoinsToDiamonds(PsMetagameManager.m_coinsToDiamondsConvertAmount).ToString(), PsMetagameManager.m_coinsToDiamondsConvertAmount.ToString(), "upgrade", m_upgrades[_upgradeIndex].Key, (m_upgrades[_upgradeIndex].Value + 1).ToString());
		PsMetagameManager.m_playerStats.CumulateCoins(-PsMetagameManager.m_playerStats.coins);
		PsMetagameManager.m_playerStats.CumulateBolts(-m_boltPrices[_upgradeIndex]);
		PsMetagameManager.m_playerStats.CumulateDiamonds(-PsMetagameManager.CoinsToDiamonds(PsMetagameManager.m_coinsToDiamondsConvertAmount));
		SetUpgrade(_upgradeIndex);
		m_uiBundle.m_fillerAnimators[_upgradeIndex].SetInteger("fillStep", m_upgrades[_upgradeIndex].Value - (m_tier - 1) * 5);
		if (PsState.m_activeMinigame != null)
		{
			PsIngameMenu.OpenController(true);
		}
	}

	public void DestroyPopup()
	{
		if (Main.m_currentGame.m_currentScene is GameScene)
		{
			PsMetagameManager.ShowResources((PsIngameMenu.m_popupMenu == null) ? null : PsIngameMenu.m_popupMenu.m_overlayCamera, true, true, true, true, false, 0.03f, true);
		}
		else
		{
			PsMetagameManager.ShowResources(null, true, true, true, false);
		}
		if (m_popup != null)
		{
			m_popup.Destroy();
			m_popup = null;
		}
		SetTier();
		SetFilling();
		if (PsState.m_activeMinigame != null)
		{
			PsIngameMenu.OpenController(true);
		}
	}

	private void SetUpgrade(int _upgradeIndex)
	{
		m_upgrades[_upgradeIndex] = new KeyValuePair<string, int>(m_upgrades[_upgradeIndex].Key, m_upgrades[_upgradeIndex].Value + 1);
		PsMetagameManager.m_playerStats.SetUpgradeValue(m_playerUnit, m_upgrades[_upgradeIndex].Key, m_upgrades[_upgradeIndex].Value);
		int num = m_upgrades[_upgradeIndex].Value / 5;
		if (num > 0 && m_upgrades[_upgradeIndex].Value % 5 == 0)
		{
			if (CanLevelUp())
			{
				ShowLevelUpButton();
			}
			else
			{
				m_uiBundle.m_buttonBundles[_upgradeIndex].m_button.interactable = false;
			}
		}
		PsMetagameManager.m_playerStats.upgraded = true;
		PsMetagameManager.SetPlayerData(new Hashtable(), false, PsMetagameManager.PlayerDataSetSUCCEED, PsMetagameManager.PlayerDataSetFAILED);
		ParseCurrentPrices();
		PsMetagameManager.m_coinsToDiamondsConvertAmount = 0;
		if (m_upgrades[_upgradeIndex].Value % 5 == 0)
		{
			SoundS.PlaySingleShot("/Ingame/Vehicles/UpgradeBigStep", Vector2.zero);
		}
		else
		{
			SoundS.PlaySingleShot("/Ingame/Vehicles/UpgradeSmallStep", Vector2.zero);
		}
	}

	private void ShowLevelUpButton()
	{
		for (int i = 0; i < m_uiBundle.m_buttonBundles.Length; i++)
		{
			m_uiBundle.m_buttonBundles[i].m_button.interactable = true;
		}
		m_uiBundle.m_contentCanvasGroup.alpha = 0f;
		m_uiBundle.m_contentCanvasGroup.interactable = false;
		m_uiBundle.m_levelUpButton.gameObject.SetActive(true);
	}

	private void HideLevelUpButton()
	{
		m_uiBundle.m_contentCanvasGroup.alpha = 1f;
		m_uiBundle.m_contentCanvasGroup.interactable = true;
		m_uiBundle.m_levelUpButton.gameObject.SetActive(false);
	}

	private void SetTierChange()
	{
		if (PsMetagameManager.m_playerStats.TierChanged(m_playerUnit))
		{
			PsMetagameManager.m_playerStats.upgraded = true;
			PsMetagameManager.SetPlayerData(new Hashtable(), false, PsMetagameManager.PlayerDataSetSUCCEED, PsMetagameManager.PlayerDataSetFAILED);
			m_tierChanged = true;
			m_uiBundle.m_levelUpButton.gameObject.SetActive(false);
			m_uiBundle.m_contentCanvasGroup.alpha = 1f;
			m_uiBundle.m_contentCanvasGroup.interactable = true;
			m_tier = PsMetagameManager.m_playerStats.GetUpgradeValue(m_playerUnit, "tier", 1);
			if (PsState.m_activeMinigame != null)
			{
				(PsState.m_activeMinigame.m_playerUnit as Vehicle).CreateUpgradeFX();
			}
			SoundS.PlaySingleShot("/Ingame/Vehicles/UpgradeTier", Vector2.zero);
			SetTier();
			SetFilling();
		}
	}

	private bool CanLevelUp()
	{
		int num = 999999;
		for (int i = 0; i < m_upgrades.Count; i++)
		{
			if (m_upgrades[i].Value < num)
			{
				num = m_upgrades[i].Value;
			}
		}
		if (Mathf.FloorToInt((float)num / 5f) + 1 > m_tier)
		{
			return true;
		}
		return false;
	}
}
