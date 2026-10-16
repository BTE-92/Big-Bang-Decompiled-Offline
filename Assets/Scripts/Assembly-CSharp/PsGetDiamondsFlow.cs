using System;
using UnityEngine;

public class PsGetDiamondsFlow : Flow
{
	private class ShopFlow : Flow
	{
		private PsUIBasePopup m_popup;

		public ShopFlow(Flow _rootFlow, Action _proceed = null, Action _cancel = null)
			: base(_proceed, _cancel, _rootFlow)
		{
			m_popup = new PsUIBasePopup(typeof(PsUICenterShopDiamonds));
			m_popup.SetAction("Exit", UserClosedTheShop);
			TweenS.AddTransformTween(m_popup.m_mainContent.m_parent.m_TC, TweenedProperty.Scale, TweenStyle.ElasticOut, Vector3.one * 0.75f, Vector3.one, 0.75f, 0f, true);
		}

		private void UserClosedTheShop()
		{
			m_popup.Destroy();
			m_popup = null;
			if (Cancel != null)
			{
				Cancel();
			}
		}
	}

	private PsUIBasePopup m_popup;

	public PsGetDiamondsFlow(Action _proceed = null, Action _cancel = null)
		: base(_proceed, _cancel)
	{
		m_popup = new PsUIBasePopup(typeof(PsUICenterNotEnoughDiamonds));
		m_popup.SetAction("EnterShop", UserProceededToPurchaseDiamonds);
		m_popup.SetAction("Exit", UserDeclinedDiamondPurchase);
		TweenS.AddTransformTween(m_popup.m_mainContent.m_parent.m_TC, TweenedProperty.Scale, TweenStyle.ElasticOut, Vector3.one * 0.75f, Vector3.one, 0.75f, 0f, true);
		PsMetagameManager.ShowResources(null, true, false, true, false, false);
	}

	private void UserProceededToPurchaseDiamonds()
	{
		m_popup.Destroy();
		m_popup = null;
		new ShopFlow(this, Proceed, Cancel);
	}

	private void UserDeclinedDiamondPurchase()
	{
		m_popup.Destroy();
		m_popup = null;
		if (Cancel != null)
		{
			Cancel();
		}
	}
}
