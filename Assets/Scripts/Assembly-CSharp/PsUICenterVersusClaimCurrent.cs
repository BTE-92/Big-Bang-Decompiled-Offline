public class PsUICenterVersusClaimCurrent : PsUICenterVersusClaimFinished
{
	public PsUICenterVersusClaimCurrent(UIComponent _parent)
		: base(_parent)
	{
	}

	public override void AddFinished(UIComponent _parent)
	{
		PsGameLoopVersus loop = PsState.m_activeGameLoop as PsGameLoopVersus;
		PsUIVersusClaimBanner psUIVersusClaimBanner = new PsUIVersusClaimBanner(m_versusUIList, loop);
	}

	public override void CreateProflieShield(UIComponent _parent)
	{
		int oldRank = (PsState.m_activeGameLoop as PsGameLoopVersus).m_oldRank;
		int oldCupAmount = (PsState.m_activeGameLoop as PsGameLoopVersus).m_oldCupAmount;
		m_shield = new PsUIProfileShield(m_contentList, false, true, oldRank, oldCupAmount);
		m_shield.RemoveDrawHandler();
		m_shield.SetHeight(0.42f, RelativeTo.ScreenHeight);
		m_shield.SetMargins(0f, 0f, 0.05f, 0.02f);
	}
}
