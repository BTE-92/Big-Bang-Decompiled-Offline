using System.Collections.Generic;

public class GameSignalTuneInState : BasicState
{
	private float m_enterTime;

	private List<PsDialogue> m_currentDialogues;

	private bool m_dialoguesRunning;

	private static bool m_sequenceOver;

	private bool m_cameraIsSet;

	public GameSignalTuneInState()
	{
		GameSceneEffectManager.Initialize(CameraS.m_mainCamera);
	}

	public override void Enter(IStatedObject _parent)
	{
		m_enterTime = Main.m_resettingGameTime;
		if (PsState.m_activeMinigame != null)
		{
			PsState.m_activeMinigame.m_gameEnded = false;
		}
		else
		{
			m_cameraIsSet = true;
		}
		m_sequenceOver = false;
		string empty = string.Empty;
		switch ((!(PsState.m_activeGameLoop is PsGameLoopBlockSignal)) ? (PsState.m_activeGameLoop as PsGameLoopBlockSignalWithoutLevel).m_unlockedProgression : (PsState.m_activeGameLoop as PsGameLoopBlockSignal).m_unlockedProgression)
		{
		case "unlock_start":
			new GenericSignalSequence(new int[9] { 1, 2, 3, 0, 1, 2, 3, 4, 5 }, "TVChannelFlicksMat_Material", 4, "intro_1", SequenceOver).StartSequence();
			break;
		case "unlock_treadmill_signal":
			new GenericSignalSequence("TVSignalTreadmill_Material", "treadmill_tv_dialogue_1").StartSequence();
			break;
		case "unlock_timeattack_signal":
			new GenericSignalSequence("TVSignalModeRace_Material", "unlock_timeattack_tv_dialogue_1").StartSequence();
			break;
		case "unlock_barrel":
			new GenericSignalSequence("TVSignalExplodingBarrels_Material", "unlock_barrel_dialogue_1").StartSequence();
			break;
		case "unlock_platforms":
			new GenericSignalSequence("TVSignalConcretePlatform_Material", "unlock_platforms_dialogue_1").StartSequence();
			break;
		case "unlock_mud":
			new GenericSignalSequence("TVSignalMaterialMud_Material", "unlock_mud_dialogue_1").StartSequence();
			break;
		case "unlock_gears":
			new GenericSignalSequence("TVSignalGears_Material", "unlock_gears_dialogue_1").StartSequence();
			break;
		case "unlock_crumbling_platforms":
			new GenericSignalSequence("TVSignalCrumblingPlatform_Material", "unlock_crumbling_dialogue_1").StartSequence();
			break;
		case "unlock_fan":
			new GenericSignalSequence("TVSignalFan_Material", "unlock_fan_dialogue_1").StartSequence();
			break;
		case "unlock_elevators":
			new GenericSignalSequence("TVSignalElevators_Material", "unlock_elevators_dialogue_1").StartSequence();
			break;
		case "unlock_ice":
			new GenericSignalSequence("TVSignalMaterialIce_Material", "unlock_ice_dialogue_1").StartSequence();
			break;
		case "unlock_crusher":
			new GenericSignalSequence("TVSignalCrusher_Material", "unlock_crusher_dialogue_1").StartSequence();
			break;
		case "unlock_bouncer":
			new GenericSignalSequence("TVSignalBouncer_Material", "unlock_bouncer_dialogue_1").StartSequence();
			break;
		case "unlock_boulder":
			new GenericSignalSequence("TVSignalRocks_Material", "unlock_boulder_dialogue_1").StartSequence();
			break;
		case "unlock_airbomb":
			new GenericSignalSequence("TVSignalAirbomb_Material", "unlock_airbomb_dialogue_1").StartSequence();
			break;
		case "unlock_portal":
			new GenericSignalSequence("TVSignalPortals_Material", "unlock_portal_dialogue_1").StartSequence();
			break;
		case "unlock_lava":
			new GenericSignalSequence("TVSignalLava_Material", "unlock_lava_dialogue_1").StartSequence();
			break;
		case "unlock_dynamics_signal":
			new GenericSignalSequence("TVSignalMetalPlank_Material", "dynamics_tv_dialogue_1").StartSequence();
			break;
		case "unlock_bike":
			new GenericSignalSequence(new int[4] { 0, 4, 3, 5 }, string.Empty, 0, "bike_intro_1", SequenceOver).StartSequence();
			break;
		default:
			DefaultSignalSequence.StartDefaultSignalSequence(SequenceOver);
			break;
		}
	}

	private static void SequenceOver()
	{
		m_sequenceOver = true;
		PsIngameMenu.OpenController();
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
		if (m_sequenceOver)
		{
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GameStartState());
		}
	}

	public override void Exit()
	{
	}
}
