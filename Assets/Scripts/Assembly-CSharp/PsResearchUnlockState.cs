public class PsResearchUnlockState : BasicState, IStatedObject
{
	public override void Enter(IStatedObject _parent)
	{
		PsMenuScene.m_lastState = "PsResearchUnlockState";
		PsUIBasePopup popup = new PsUIBasePopup(typeof(PsUICenterResearchUnlock));
		popup.SetAction("Exit", delegate
		{
			if (PsPlanetManager.GetCurrentPlanet().m_mainPath.m_currentNodeId == 2)
			{
				PsUIBaseState psUIBaseState = new PsUIBaseState(typeof(PsUICenterNewUser));
				psUIBaseState.SetAction("Exit", delegate
				{
					Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new PsMainMenuState());
				});
				Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(psUIBaseState);
			}
			else
			{
				Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new PsMainMenuState());
			}
			popup.Destroy();
		});
		PsMenuScene.CreateBlur();
	}

	public override void Exit()
	{
	}
}
