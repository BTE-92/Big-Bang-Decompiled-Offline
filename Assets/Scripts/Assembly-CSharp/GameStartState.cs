public class GameStartState : BasicState
{
	private bool m_cameraIsSet;

	public override void Enter(IStatedObject _parent)
	{
	}

	public override void Execute()
	{
		if (!m_cameraIsSet)
		{
			GraphElement element = LevelManager.m_currentLevel.m_currentLayer.GetElement("Player");
			if (element != null)
			{
				CameraS.SnapMainCameraFrame();
				m_cameraIsSet = true;
			}
		}
	}

	public override void Exit()
	{
	}
}
