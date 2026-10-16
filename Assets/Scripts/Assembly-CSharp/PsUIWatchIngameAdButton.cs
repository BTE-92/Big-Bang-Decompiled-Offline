using AdMediation;

public class PsUIWatchIngameAdButton : UIRectSpriteButton
{
	public PsUIWatchIngameAdButton(UIComponent _parent)
		: base(_parent, "WatchAdForDoubleCoins", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_double_coins_ad_button"))
	{
	}

	public override void Step()
	{
		if (m_hit)
		{
			WatchAd();
		}
		base.Step();
	}

	public void WatchAd()
	{
		TouchAreaS.Disable();
		PsAdMediation.ShowAd(AdViewCallback);
	}

	private void AdViewCallback(AdResult _result)
	{
		Debug.Log("Ad result: " + _result);
		if (_result == AdResult.Finished)
		{
			PsMetagameManager.ResetCoinBoostTime();
			PsMetagameManager.UpdateCoinValueBooster();
			Destroy();
		}
		TouchAreaS.Enable();
	}
}
