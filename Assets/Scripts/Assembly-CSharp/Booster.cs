using System.Collections;

public class Booster
{
	public PsUIBoosterButton m_boosterButton;

	public bool m_free;

	public bool m_used;

	public bool m_available;

	public Booster(bool _free)
	{
		m_free = _free;
	}

	public void SetBoosterButton(PsUIBoosterButton _button)
	{
		m_boosterButton = _button;
	}

	public virtual void Use(Vehicle _vehicle)
	{
		PsMetagameManager.m_playerStats.CumulateBoosters(-1);
		PsMetagameManager.SetPlayerData(new Hashtable(), false, PsMetagameManager.PlayerDataSetSUCCEED, PsMetagameManager.PlayerDataSetFAILED);
		m_boosterButton.UseBoost();
		m_used = true;
		PsState.m_activeGameLoop.m_boosterUsed = true;
	}

	public static bool IsBoosterAllowed()
	{
		if (PsState.m_activeGameLoop.m_boosterUsed || PsState.m_activeGameLoop.m_context == PsMinigameContext.Fresh || PsState.m_activeGameLoop.m_context == PsMinigameContext.Signal || PsState.m_activeGameLoop.m_context == PsMinigameContext.EditorPuzzle || PsState.m_activeGameLoop.m_context == PsMinigameContext.EditorTemplate)
		{
			return false;
		}
		PsUnlockable unlockableByIdentifier = PsMetagameData.GetUnlockableByIdentifier("Boost");
		return (PsState.m_activeGameLoop.m_context != PsMinigameContext.Editor && PsState.m_activeGameLoop.m_context != PsMinigameContext.Challenge && unlockableByIdentifier.m_unlocked) || true;
	}
}
