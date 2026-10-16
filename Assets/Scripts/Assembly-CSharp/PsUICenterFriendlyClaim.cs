using Server;

public class PsUICenterFriendlyClaim : UICanvas
{
	public PsUICenterFriendlyClaim(UIComponent _parent)
		: base(_parent, false, "CenterFriendlyClaim", null, string.Empty)
	{
		UIText uIText = new UIText(this, false, string.Empty, "You won!", "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight);
		uIText.SetVerticalAlign(0.6f);
		PsUILoadingAnimation psUILoadingAnimation = new PsUILoadingAnimation(this, false);
		psUILoadingAnimation.SetVerticalAlign(0.4f);
		Claim();
	}

	private void Claim()
	{
		Friendly.Claim(PsState.m_activeGameLoop.m_versusData.timedEventId, ClaimOk, ClaimFailed);
	}

	private void ClaimOk(HttpC _c)
	{
		Debug.Log("Friendly race claim ok, id: " + PsState.m_activeGameLoop.m_versusData.timedEventId);
		(GetRoot() as PsUIBasePopup).CallAction("Exit");
	}

	private void ClaimFailed(HttpC _c)
	{
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), delegate
		{
			HttpC httpC = Friendly.Claim((string)_c.objectData, ClaimOk, ClaimFailed);
			httpC.objectData = _c.objectData;
			return httpC;
		});
	}
}
