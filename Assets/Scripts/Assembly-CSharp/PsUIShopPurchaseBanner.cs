using System;

public class PsUIShopPurchaseBanner : PsUIGenericButton
{
	private class PsUIShopPopupNotEnoughResources : PsUIPopup
	{
		private PsUIGenericButton m_ok;

		public PsUIShopPopupNotEnoughResources(bool _coins, Action _proceed = null, Action _cancel = null)
			: base(_proceed, _cancel)
		{
			UIVerticalList uIVerticalList = new UIVerticalList(this, string.Empty);
			uIVerticalList.SetMargins(0.05f, 0.05f, 0.05f, 0.05f);
			uIVerticalList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
			uIVerticalList.SetDrawHandler(PsUIPopup.PopupDrawHandler);
			new UITextbox(uIVerticalList, false, string.Empty, "Not enough " + ((!_coins) ? "diamonds" : "coins") + "! Earn more by playing!", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest);
			UIHorizontalList uIHorizontalList = new UIHorizontalList(uIVerticalList, string.Empty);
			uIHorizontalList.RemoveDrawHandler();
			uIHorizontalList.RemoveTouchAreas();
			uIHorizontalList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
			m_ok = new PsUIGenericButton(uIHorizontalList);
			m_ok.SetText("Ok");
			m_ok.SetGreenColors();
			m_ok.Update();
			Update();
			UpdatePopupBG();
		}

		public override void Step()
		{
			if (m_ok.m_hit)
			{
				Destroy();
			}
			else
			{
				base.Step();
			}
		}
	}

	private PsUnlockable m_shopItem;

	private Action m_itemClicked;

	private bool m_virtualCurrency;

	public PsUIShopPurchaseBanner(UIVerticalList _parent, PsUnlockable _shopItem, Action _itemClicked)
		: base(_parent)
	{
		m_shopItem = _shopItem;
		m_itemClicked = _itemClicked;
		SetGreenColors();
		SetText(_shopItem.m_name);
		Update();
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		base.OnTouchRelease(_touch, _inside);
		PsMetrics.ButtonPressed(m_shopItem.m_identifier, "shop");
		if (!m_virtualCurrency)
		{
			m_itemClicked();
		}
	}

	protected override void OnTouchDragStart(TLTouch _touch)
	{
		Highlight(false);
		HighlightSecondary(false);
		m_end = true;
	}

	public override void Destroy()
	{
		base.Destroy();
	}
}
