public class PsUICenterWinRaceFriendly : PsUICenterWinRaceVersus
{
	public PsUICenterWinRaceFriendly(UIComponent _parent)
		: base(_parent)
	{
	}

	public override void TotalWin()
	{
		(PsState.m_activeGameLoop as PsGameLoopVersus).m_versusData.winner = PlayerPrefsX.GetUserId();
		PsUIBasePopup popup = new PsUIBasePopup(typeof(PsUICenterFriendlyClaim));
		popup.SetAction("Exit", delegate
		{
			popup.Destroy();
			(GetRoot() as PsUIBasePopup).CallAction("Continue");
		});
	}
}
