using System.Collections.Generic;

public class GameSignalTuneOutState : BasicState
{
	private float m_enterTime;

	private bool m_changingScene;

	private List<PsDialogue> m_currentDialogues;

	private bool m_dialoguesRunning;

	private static bool m_sequenceOver;

	public override void Enter(IStatedObject _parent)
	{
		m_enterTime = Main.m_resettingGameTime;
		m_changingScene = false;
		m_sequenceOver = false;
		string unlockedProgression = (PsState.m_activeGameLoop as PsGameLoopBlockSignal).m_unlockedProgression;
		if (unlockedProgression == "unlock_start")
		{
			new GenericSignalSequence(new int[3] { 4, 3, 6 }, "TVChannelFlicksMat_Material", 4, "intro_end_1", SequenceOver).StartSequence();
		}
		else if (unlockedProgression == "unlock_bike")
		{
			new GenericSignalSequence(new int[3] { 4, 3, 6 }, string.Empty, 0, "bike_outro_1", SequenceOver).StartSequence();
		}
		else
		{
			DefaultSignalSequence.StartDefaultSignalEndSequence(SequenceOver);
		}
	}

	private static void SequenceOver()
	{
		m_sequenceOver = true;
	}

	public override void Execute()
	{
		if (m_sequenceOver && !m_changingScene)
		{
			PsState.m_activeGameLoop.ExitMinigame();
			m_changingScene = true;
		}
	}

	public override void Exit()
	{
	}
}
