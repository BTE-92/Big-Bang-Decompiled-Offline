using System.Collections;

public class PsUIVersusClaimWinPopup : PsUIVersusClaimWaitingPopup
{
	protected PsUIGenericButton m_ok;

	public PsUIVersusClaimWinPopup(UIComponent _parent)
		: base(_parent)
	{
	}

	public override void CreateContent(UIComponent _parent)
	{
		_parent.RemoveTouchAreas();
		UIVerticalList uIVerticalList = new UIVerticalList(_parent, string.Empty);
		uIVerticalList.SetMargins(0.05f, 0.05f, 0.05f, 0.05f);
		uIVerticalList.RemoveDrawHandler();
		CreatePrizeContent(uIVerticalList);
	}

	public virtual void CreatePrizeContent(UIComponent _parent)
	{
		UITextbox uITextbox = new UITextbox(_parent, false, string.Empty, "You won!", "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, false, Align.Center, Align.Middle);
		UIFittedSprite uIFittedSprite = new UIFittedSprite(_parent, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_shop_item_coins"));
		uIFittedSprite.SetWidth(0.2f, RelativeTo.ParentWidth);
		m_ok = new PsUIGenericButton(_parent);
		m_ok.SetText("Claim");
	}

	public override void Step()
	{
		if (m_ok != null && m_ok.m_hit)
		{
			ClaimAction();
			(GetRoot() as PsUIBasePopup).CallAction("Claim");
		}
		base.Step();
	}

	public virtual void ClaimAction()
	{
		PsMetagameManager.VersusChallengeRewards(true);
		PsMetagameManager.SetPlayerData(new Hashtable(), false, PsMetagameManager.PlayerDataSetSUCCEED, PsMetagameManager.PlayerDataSetFAILED);
	}
}
