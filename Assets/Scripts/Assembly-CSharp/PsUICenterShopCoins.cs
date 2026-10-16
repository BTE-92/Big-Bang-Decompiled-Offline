using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsUICenterShopCoins : PsUIScrollableBase
{
	private List<PsUIGenericButton> m_buttons;

	private bool m_purchasing;

	private static PsUIPurchaseWaitPopup m_waitPopup;

	public PsUICenterShopCoins(UIComponent _parent)
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
		UIText parent = new UIText(uIHorizontalList, false, string.Empty, "Get more coins", "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
		UICanvas uICanvas = new UICanvas(parent, false, string.Empty, null, string.Empty);
		uICanvas.SetHorizontalAlign(0f);
		uICanvas.SetMargins(-1.5f, 0f, -0.125f, -0.125f, RelativeTo.ParentHeight);
		uICanvas.RemoveDrawHandler();
		Frame frame = PsState.m_uiSheet.m_atlas.GetFrame("menu_resources_coin_icon");
		UISprite uISprite = new UISprite(uICanvas, false, string.Empty, PsState.m_uiSheet, frame);
		uISprite.SetSize(1f, 1f * (frame.height / frame.width), RelativeTo.ParentHeight);
		uISprite.SetHorizontalAlign(0f);
	}

	public override void CreateContent(UIComponent _parent)
	{
		m_buttons = new List<PsUIGenericButton>();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetHorizontalAlign(0f);
		uIHorizontalList.SetSpacing(0.015f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.025f);
		PsUIGenericButton psUIGenericButton = new PsUIGenericButton(uIHorizontalList);
		psUIGenericButton.SetShopButton("<color=#749fc0>99</color>", "3,000 COINS", "menu_shop_item_coins_tier1");
		psUIGenericButton.m_identifier = "3000coins";
		psUIGenericButton.m_TAC.m_letTouchesThrough = true;
		if (PsMetagameManager.m_playerStats.diamonds < 79)
		{
			psUIGenericButton.SetBlueColors(false);
			psUIGenericButton.RemoveTouchAreas();
		}
		m_buttons.Add(psUIGenericButton);
		psUIGenericButton = new PsUIGenericButton(uIHorizontalList);
		psUIGenericButton.SetShopButton("<color=#749fc0>249</color>", "10,000 COINS", "menu_shop_item_coins_tier2");
		psUIGenericButton.m_identifier = "10000coins";
		psUIGenericButton.m_TAC.m_letTouchesThrough = true;
		if (PsMetagameManager.m_playerStats.diamonds < 249)
		{
			psUIGenericButton.SetBlueColors(false);
			psUIGenericButton.RemoveTouchAreas();
		}
		m_buttons.Add(psUIGenericButton);
		psUIGenericButton = new PsUIGenericButton(uIHorizontalList);
		psUIGenericButton.SetShopButton("<color=#749fc0>690</color>", "50,000 COINS", "menu_shop_item_coins_tier3");
		psUIGenericButton.m_identifier = "50000coins";
		psUIGenericButton.m_TAC.m_letTouchesThrough = true;
		if (PsMetagameManager.m_playerStats.diamonds < 690)
		{
			psUIGenericButton.SetBlueColors(false);
			psUIGenericButton.RemoveTouchAreas();
		}
		m_buttons.Add(psUIGenericButton);
		psUIGenericButton = new PsUIGenericButton(uIHorizontalList);
		psUIGenericButton.SetShopButton("<color=#749fc0>1900</color>", "250,000 COINS", "menu_shop_item_coins_tier4");
		psUIGenericButton.m_identifier = "250000coins";
		psUIGenericButton.m_TAC.m_letTouchesThrough = true;
		if (PsMetagameManager.m_playerStats.diamonds < 1900)
		{
			psUIGenericButton.SetBlueColors(false);
			psUIGenericButton.RemoveTouchAreas();
		}
		m_buttons.Add(psUIGenericButton);
		psUIGenericButton = new PsUIGenericButton(uIHorizontalList);
		psUIGenericButton.SetShopButton("<color=#749fc0>5900</color>", "1,000,000 COINS", "menu_shop_item_coins_tier5");
		psUIGenericButton.m_identifier = "1000000coins";
		psUIGenericButton.m_TAC.m_letTouchesThrough = true;
		if (PsMetagameManager.m_playerStats.diamonds < 5900)
		{
			psUIGenericButton.SetBlueColors(false);
			psUIGenericButton.RemoveTouchAreas();
		}
		m_buttons.Add(psUIGenericButton);
	}

	public override void Step()
	{
		if (m_purchasing)
		{
			base.Step();
			return;
		}
		for (int i = 0; i < m_buttons.Count; i++)
		{
			if (m_buttons[i].m_hit)
			{
				Purchase(m_buttons[i].m_identifier);
				m_purchasing = true;
				TouchAreaS.Disable();
				break;
			}
		}
		base.Step();
	}

	public void Purchase(string _identifier)
	{
		ShowLoadingPopup();
		switch (_identifier)
		{
		case "3000coins":
			PsMetagameManager.m_playerStats.CumulateCoins(3000);
			PsMetagameManager.m_playerStats.CumulateDiamonds(-99);
			PsMetrics.ConvertDiamondsToCoins("99", "0", "shop", string.Empty, string.Empty);
			break;
		case "10000coins":
			PsMetagameManager.m_playerStats.CumulateCoins(10000);
			PsMetagameManager.m_playerStats.CumulateDiamonds(-249);
			PsMetrics.ConvertDiamondsToCoins("249", "0", "shop", string.Empty, string.Empty);
			break;
		case "50000coins":
			PsMetagameManager.m_playerStats.CumulateCoins(50000);
			PsMetagameManager.m_playerStats.CumulateDiamonds(-690);
			PsMetrics.ConvertDiamondsToCoins("690", "0", "shop", string.Empty, string.Empty);
			break;
		case "250000coins":
			PsMetagameManager.m_playerStats.CumulateCoins(250000);
			PsMetagameManager.m_playerStats.CumulateDiamonds(-1900);
			PsMetrics.ConvertDiamondsToCoins("1900", "0", "shop", string.Empty, string.Empty);
			break;
		case "1000000coins":
			PsMetagameManager.m_playerStats.CumulateCoins(1000000);
			PsMetagameManager.m_playerStats.CumulateDiamonds(-5900);
			PsMetrics.ConvertDiamondsToCoins("5900", "0", "shop", string.Empty, string.Empty);
			break;
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
