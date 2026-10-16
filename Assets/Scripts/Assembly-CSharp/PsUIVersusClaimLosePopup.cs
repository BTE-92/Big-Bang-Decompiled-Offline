using System.Collections;

public class PsUIVersusClaimLosePopup : PsUIVersusClaimWinPopup
{
	public PsUIVersusClaimLosePopup(UIComponent _parent)
		: base(_parent)
	{
	}

	public override void CreatePrizeContent(UIComponent _parent)
	{
		UITextbox uITextbox = new UITextbox(_parent, false, string.Empty, "You lost.", "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, false, Align.Center, Align.Middle);
		PsUIGenericButton psUIGenericButton = new PsUIGenericButton(_parent);
		psUIGenericButton.SetText("Ok");
	}

	public override void ClaimAction()
	{
		PsMetagameManager.VersusChallengeRewards(false);
		PsMetagameManager.SetPlayerData(new Hashtable(), false, PsMetagameManager.PlayerDataSetSUCCEED, PsMetagameManager.PlayerDataSetFAILED);
	}
}
