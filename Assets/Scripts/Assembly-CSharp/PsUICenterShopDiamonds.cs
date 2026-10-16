using System.Collections.Generic;
using UnityEngine;

public class PsUICenterShopDiamonds : PsUIScrollableBase
{
	private List<PsUIGenericButton> m_buttons;

	private bool m_purchasing;

	private static PsUIPurchaseWaitPopup m_waitPopup;

	public PsUICenterShopDiamonds(UIComponent _parent)
		: base(_parent)
	{
		m_purchasing = false;
		PsMetagameManager.ShowResources(null, true, true, true, false, false);
		PsUIInfoBar psUIInfoBar = new PsUIInfoBar(GetRoot(), string.Empty);
		psUIInfoBar.SetVerticalAlign(0f);
		psUIInfoBar.SetText("Speed up your progression with GEMS.");
	}

	public override void CreateHeaderContent(UIComponent _parent)
	{
		UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.025f, 0.025f, 0f, 0f);
		uIHorizontalList.SetHorizontalAlign(0.5f);
		UIText parent = new UIText(uIHorizontalList, false, string.Empty, "Get more gems", "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
		UICanvas uICanvas = new UICanvas(parent, false, string.Empty, null, string.Empty);
		uICanvas.SetHorizontalAlign(0f);
		uICanvas.SetMargins(-1.5f, 0f, -0.125f, -0.125f, RelativeTo.ParentHeight);
		uICanvas.RemoveDrawHandler();
		Frame frame = PsState.m_uiSheet.m_atlas.GetFrame("menu_resources_diamond_icon");
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
		psUIGenericButton.SetShopButton("<color=#749fc0>$4.99</color>", "500 GEMS", "menu_shop_item_gems_tier1", false);
		psUIGenericButton.m_identifier = "500gems";
		psUIGenericButton.m_TAC.m_letTouchesThrough = true;
		m_buttons.Add(psUIGenericButton);
		psUIGenericButton = new PsUIGenericButton(uIHorizontalList);
		psUIGenericButton.SetShopButton("<color=#749fc0>$9.99</color>", "1,200 GEMS", "menu_shop_item_gems_tier2", false);
		psUIGenericButton.m_identifier = "1200gems";
		psUIGenericButton.m_TAC.m_letTouchesThrough = true;
		m_buttons.Add(psUIGenericButton);
		psUIGenericButton = new PsUIGenericButton(uIHorizontalList);
		psUIGenericButton.SetShopButton("<color=#749fc0>$19.99</color>", "2,500 GEMS", "menu_shop_item_gems_tier3", false);
		psUIGenericButton.m_identifier = "2500gems";
		psUIGenericButton.m_TAC.m_letTouchesThrough = true;
		m_buttons.Add(psUIGenericButton);
		psUIGenericButton = new PsUIGenericButton(uIHorizontalList);
		psUIGenericButton.SetShopButton("<color=#749fc0>$49.99</color>", "6,500 GEMS", "menu_shop_item_gems_tier4", false);
		psUIGenericButton.m_identifier = "6500gems";
		psUIGenericButton.m_TAC.m_letTouchesThrough = true;
		m_buttons.Add(psUIGenericButton);
		psUIGenericButton = new PsUIGenericButton(uIHorizontalList);
		psUIGenericButton.SetShopButton("<color=#749fc0>$99.99</color>", "14,000 GEMS", "menu_shop_item_gems_tier5", false);
		psUIGenericButton.m_identifier = "14000gems";
		psUIGenericButton.m_TAC.m_letTouchesThrough = true;
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
				TouchAreaS.Disable();
				Purchase(m_buttons[i].m_identifier);
				m_purchasing = true;
				break;
			}
		}
		base.Step();
	}

	public void Purchase(string _identifier)
	{
		if (PsIAPManager.CanMakePayments())
		{
			ShowLoadingPopup();
			PsMetagameManager.Purchase(_identifier, SUCCEED, ERROR);
		}
		else
		{
			TouchAreaS.Enable();
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
		}
	}

	private void SUCCEED(string _m)
	{
		HideLoadingPopup();
		Debug.Log("GEM PURCHASE SUCCESFUL");
		PsMetagameManager.PlayerDataSetSUCCEED(null);
		PsState.m_inIapFlow = false;
		TouchAreaS.Enable();
		(GetRoot() as PsUIBasePopup).CallAction("Exit");
		SoundS.PlaySingleShot("/UI/Purchase", Vector2.zero);
	}

	private void ERROR(string _e, string _id, IAP.TransactionResult _result)
	{
		HideLoadingPopup();
		Debug.Log("Gem purchase failed: " + _e);
		TouchAreaS.Enable();
		if (string.IsNullOrEmpty(_id))
		{
			if (_result == IAP.TransactionResult.Canceled)
			{
				PsState.m_inIapFlow = false;
				TouchAreaS.Enable();
				m_purchasing = false;
				return;
			}
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
			ServerManager.ThrowServerErrorException("Error", _e, null, delegate
			{
				PsState.m_inIapFlow = false;
				TouchAreaS.Enable();
				m_purchasing = false;
			});
		}
		else
		{
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
			PsState.m_inIapFlow = false;
			ServerManager.ThrowServerErrorException("Error", _e, null, delegate
			{
				PsMetagameManager.Purchase(_id, SUCCEED, ERROR);
			});
		}
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
