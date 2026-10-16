using System;

public class PsSkipLevelFlow : Flow
{
	public PsSkipLevelFlow(Action _proceed, Action _cancel, int _diamondAmount)
		: base(_proceed, _cancel)
	{
		if (PsMetagameManager.m_playerStats.diamonds >= _diamondAmount)
		{
			new PsServerQueueFlow(Proceed, delegate
			{
				PsMetagameManager.SkipLevel(_diamondAmount, PsState.m_activeGameLoop.GetPlayerUnit());
			}, null);
		}
		else
		{
			new PsGetDiamondsFlow();
		}
	}
}
