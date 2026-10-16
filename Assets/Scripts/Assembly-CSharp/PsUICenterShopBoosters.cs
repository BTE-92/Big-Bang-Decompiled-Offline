using System;
using System.Collections;
using System.Collections.Generic;
using AdMediation;
using UnityEngine;

public class PsUICenterShopBoosters : PsUIScrollableBase
{
	private List<PsUIGenericButton> m_buttons;

	private PsUIGenericButton m_adButton;

	private bool m_purchasing;

	private PsUIBasePopup m_popup;

	private bool m_freeBooster;

	public bool m_createTutorialArrow;

	private PsUITutorialArrowUI m_tutorialArrow;

	private int[,] m_boostPrices = new int[3, 2]
	{
		{ 3, 780 },
		{ 10, 2500 },
		{ 50, 12000 }
	};

	private static PsUIPurchaseWaitPopup m_waitPopup;

	public PsUICenterShopBoosters(UIComponent _parent)
		: base(_parent)
	{
		m_purchasing = false;
		PsMetagameManager.ShowResources(null, true, true, true, false, false);
	}

	public override void CreateHeaderContent(UIComponent _parent)
	{
		UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.025f, 0.025f, 0f, 0f);
		uIHorizontalList.SetHorizontalAlign(0.5f);
		UIText parent = new UIText(uIHorizontalList, false, string.Empty, "Get more boosters", "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
		UICanvas uICanvas = new UICanvas(parent, false, string.Empty, null, string.Empty);
		uICanvas.SetHorizontalAlign(0f);
		uICanvas.SetMargins(-1.5f, 0f, -0.125f, -0.125f, RelativeTo.ParentHeight);
		uICanvas.RemoveDrawHandler();
		Frame frame = PsState.m_uiSheet.m_atlas.GetFrame("hud_boost_boost");
		UISprite uISprite = new UISprite(uICanvas, false, string.Empty, PsState.m_uiSheet, frame);
		uISprite.SetSize(1f, 1f * (frame.height / frame.width), RelativeTo.ParentHeight);
		uISprite.SetHorizontalAlign(0f);
	}

	public override void CreateContent(UIComponent _parent)
	{
		m_buttons = new List<PsUIGenericButton>();
		m_freeBooster = PsState.m_activeGameLoop.m_freeConsumableUnlock == "Booster" && PsMetagameManager.m_playerStats.GetBoosterValue() == 0;
		UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetHorizontalAlign(0.5f);
		uIHorizontalList.SetSpacing(0.01f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.025f);
		if (PsAdMediation.adsAvailable() || m_freeBooster)
		{
			m_adButton = new PsUIGenericButton(uIHorizontalList);
			if (m_freeBooster)
			{
				m_adButton.SetShopButton("FREE ", "1 BOOSTER", "hud_boost_boost", false);
			}
			else
			{
				m_adButton.SetShopButton("VIDEO", "FREE BOOSTER", "hud_boost_boost", false);
			}
			m_adButton.m_TAC.m_letTouchesThrough = true;
			m_adButton.m_identifier = "1BoosterAd";
		}
		for (int i = 0; i < m_boostPrices.GetLength(0); i++)
		{
			PsUIGenericButton psUIGenericButton = new PsUIGenericButton(uIHorizontalList);
			string priceText = m_boostPrices[i, 1].ToString();
			int num = m_boostPrices[i, 0];
			string text = ((num <= 1) ? " BOOSTER" : " BOOSTERS");
			psUIGenericButton.SetShopButton(priceText, num + text, "hud_boost_boost", false, true);
			psUIGenericButton.m_TAC.m_letTouchesThrough = true;
			psUIGenericButton.m_identifier = num + "Booster";
			m_buttons.Add(psUIGenericButton);
		}
	}

	public override void Step()
	{
		if (m_purchasing)
		{
			base.Step();
			return;
		}
		if (m_createTutorialArrow)
		{
			if (m_tutorialArrow == null && m_adButton != null)
			{
				m_tutorialArrow = new PsUITutorialArrowUI(m_adButton);
			}
			m_createTutorialArrow = false;
		}
		for (int i = 0; i < m_boostPrices.GetLength(0); i++)
		{
			if (m_buttons[i].m_hit)
			{
				TouchAreaS.Disable();
				Purchase(m_boostPrices[i, 0], m_boostPrices[i, 1]);
				m_purchasing = true;
				break;
			}
		}
		if (!m_purchasing && m_adButton != null && m_adButton.m_hit)
		{
			if (m_freeBooster)
			{
				TouchAreaS.Disable();
				Purchase(1, 0);
				m_purchasing = true;
			}
			else
			{
				TouchAreaS.Disable();
				PsAdMediation.ShowAd(AdViewCallback);
			}
		}
		base.Step();
	}

	public void Purchase(int _boostCount, int _coinPrice)
	{
		if (_coinPrice <= PsMetagameManager.m_playerStats.coins)
		{
			ShowLoadingPopup();
			SetBoosters(_boostCount, -_coinPrice);
		}
		else
		{
			OpenCoinPopup(_boostCount, _coinPrice);
		}
	}

	private void AdViewCallback(AdResult _result)
	{
		Debug.Log("Ad result: " + _result);
		if (_result == AdResult.Finished)
		{
			ShowLoadingPopup();
			SetBoosters(1, 0);
		}
		else
		{
			TouchAreaS.Enable();
		}
	}

	private void OpenCoinPopup(int _boostAmount, int _coinAmount)
	{
		TouchAreaS.Enable();
		PsMetagameManager.m_coinsToDiamondsConvertAmount = _coinAmount - PsMetagameManager.m_playerStats.coins;
		m_popup = new PsUIBasePopup(typeof(PsUICenterNotEnoughCoins));
		m_popup.SetAction("Exit", DestroyPopup);
		m_popup.SetAction("Upgrade", delegate
		{
			BuyWithDiamonds(_boostAmount, _boostAmount);
		});
		TweenS.AddTransformTween(m_popup.m_mainContent.m_parent.m_TC, TweenedProperty.Scale, TweenStyle.ElasticOut, Vector3.one * 0.75f, Vector3.one, 0.75f, 0f, true);
		PsMetagameManager.ShowResources(null, true, true, true, false, false);
	}

	public void DestroyPopup()
	{
		m_purchasing = false;
		m_popup.Destroy();
		m_popup = null;
	}

	public void BuyWithDiamonds(int _boostAmount, int _coinAmount)
	{
		TouchAreaS.Disable();
		DestroyPopup();
		int cumulateDiamonds = PsMetagameManager.CoinsToDiamonds(PsMetagameManager.m_coinsToDiamondsConvertAmount);
		SetBoosters(_boostAmount, -PsMetagameManager.m_playerStats.coins, cumulateDiamonds);
	}

	public void SetBoosters(int _boughtBoosterCount, int _cumulateCoins, int _cumulateDiamonds = 0)
	{
		PsMetagameManager.m_playerStats.CumulateBoosters(_boughtBoosterCount);
		if (_cumulateCoins != 0)
		{
			PsMetagameManager.m_playerStats.CumulateCoins(Math.Abs(_cumulateCoins) * -1);
		}
		if (_cumulateDiamonds != 0)
		{
			PsMetagameManager.m_playerStats.CumulateDiamonds(Math.Abs(_cumulateDiamonds) * -1);
		}
		PsMetagameManager.SetPlayerData(new Hashtable(), false, SUCCEED, FAILED);
	}

	private void SUCCEED(HttpC _c)
	{
		HideLoadingPopup();
		PsMetagameManager.PlayerDataSetSUCCEED(_c);
		(GetRoot() as PsUIBasePopup).CallAction("Exit");
		TouchAreaS.Enable();
		SoundS.PlaySingleShot("/UI/Purchase", Vector2.zero);
	}

	private void FAILED(HttpC _c)
	{
		HideLoadingPopup();
		Debug.Log("COIN PURCHASE FAILED");
		string networkError = ServerErrors.GetNetworkError(_c.www.error);
		Hashtable setData = _c.objectData as Hashtable;
		ServerManager.ThrowServerErrorException("Connection error", networkError, () => PsServerRequest.ServerPlayerSetData(setData, SUCCEED, FAILED));
	}

	private static void ShowLoadingPopup()
	{
		Debug.Log("Show loading popup");
		if (m_waitPopup != null)
		{
			m_waitPopup.Destroy();
			m_waitPopup = null;
		}
		m_waitPopup = new PsUIPurchaseWaitPopup(null);
	}

	private static void HideLoadingPopup()
	{
		Debug.Log("Hide loading popup");
		if (m_waitPopup != null)
		{
			m_waitPopup.Destroy();
			m_waitPopup = null;
		}
	}
}
