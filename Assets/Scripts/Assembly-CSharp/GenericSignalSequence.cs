using System;
using System.Collections.Generic;
using UnityEngine;

public class GenericSignalSequence
{
	private int m_tvIndex;

	private int m_dialogIndex;

	private int m_channelCount;

	private Action m_endSequenceCallBack;

	private string m_material;

	private string m_dialogIdentifier;

	private SpeechBubbleHandlePosition m_bubbleHandlePosition;

	private SpeechBubbleHandleType m_bubbleHandleType;

	private Queue<Action> m_sequenceQueue;

	public GenericSignalSequence(Array _signalSequenceOrder, string _material, int _channelCount, string _dialogIdentifier, Action _endSequenceCallback = null)
	{
		m_channelCount = _channelCount;
		m_endSequenceCallBack = _endSequenceCallback;
		m_tvIndex = 1;
		m_dialogIndex = 0;
		m_material = _material;
		m_dialogIdentifier = _dialogIdentifier;
		CreateSequenceOrder(_signalSequenceOrder);
	}

	public GenericSignalSequence(string _material, int _channelCount, string _dialogIdentifier, Action _endSequenceCallback = null)
		: this(new int[5] { 0, 1, 2, 3, 6 }, _material, _channelCount, _dialogIdentifier, _endSequenceCallback)
	{
	}

	public GenericSignalSequence(string _material, string _dialogIdentifier, Action _endSequenceCallback = null)
		: this(new int[4] { 1, 2, 3, 6 }, _material, 1, _dialogIdentifier, _endSequenceCallback)
	{
	}

	private void CreateSequenceOrder(Array order)
	{
		m_sequenceQueue = new Queue<Action>();
		foreach (int item in order)
		{
			switch (item)
			{
			case 0:
				m_sequenceQueue.Enqueue(ChannelNoise);
				break;
			case 1:
				m_sequenceQueue.Enqueue(ChannelRoll);
				break;
			case 2:
				m_sequenceQueue.Enqueue(ChannelStabile);
				break;
			case 3:
				m_sequenceQueue.Enqueue(ChannelDialogue);
				break;
			case 4:
				m_sequenceQueue.Enqueue(ChannelSignalLevel);
				break;
			case 5:
				m_sequenceQueue.Enqueue(ChannelPlaySignalLevel);
				break;
			default:
				m_sequenceQueue.Enqueue(ChannelEndSignal);
				break;
			}
		}
	}

	public void StartSequence()
	{
		if (m_channelCount > 0)
		{
			GameSceneEffectManager.CreateTVChannels(m_material, m_channelCount);
		}
		else
		{
			GameSceneEffectManager.CreateTVChannels("TVChannelFlicksMat_Material", 4);
		}
		GameSceneEffectManager.m_dialogue = PsMetagameData.GetDialogueByIdentifier(m_dialogIdentifier);
		GameSceneEffectManager.m_crew = new PsUICrewMemberBase(CameraS.m_uiCamera, string.Empty);
		GameSceneEffectManager.m_bubble = null;
		PsDialogueCharacterPosition characterPosition = GameSceneEffectManager.m_dialogue.m_steps[0].m_characterPosition;
		if (characterPosition == PsDialogueCharacterPosition.Left)
		{
			m_bubbleHandlePosition = SpeechBubbleHandlePosition.Left;
			m_bubbleHandleType = SpeechBubbleHandleType.SmallToLeft;
		}
		else
		{
			m_bubbleHandlePosition = SpeechBubbleHandlePosition.Right;
			m_bubbleHandleType = SpeechBubbleHandleType.SmallToRight;
		}
		GameSceneEffectManager.m_crew.CreateCharacter(GameSceneEffectManager.m_dialogue.m_steps[0].m_character, characterPosition);
		NextChannelAction();
		GameSceneEffectManager.StartMusic("/Cutscenes/Television", true);
	}

	private void NextChannelAction(TimerC _c)
	{
		if (_c != null)
		{
			TimerS.RemoveComponent(_c);
		}
		if (m_sequenceQueue.Count > 0)
		{
			Action action = m_sequenceQueue.Dequeue();
			action();
		}
		else
		{
			Debug.LogError("No more next actions for the SignalSequence");
		}
	}

	private void NextChannelAction()
	{
		if (GameSceneEffectManager.m_bubble != null)
		{
			GameSceneEffectManager.m_bubble = null;
		}
		NextChannelAction(null);
	}

	private void ChannelNoise()
	{
		GameSceneEffectManager.StartMusic("/Cutscenes/Television", true);
		GameSceneEffectManager.SetChannel(0);
		TimerS.AddComponent(GameSceneEffectManager.m_utilityEntity, string.Empty, 0.4f, 0f, false, NextChannelAction);
	}

	private void ChannelRoll()
	{
		GameSceneEffectManager.SetChannel(m_tvIndex);
		GameSceneEffectManager.SetChannelRoll(true);
		m_tvIndex++;
		TimerS.AddComponent(GameSceneEffectManager.m_utilityEntity, string.Empty, 1f, 0f, false, NextChannelAction);
	}

	private void ChannelStabile()
	{
		GameSceneEffectManager.SetChannelRoll(false);
		TimerS.AddComponent(GameSceneEffectManager.m_utilityEntity, string.Empty, 2f, 0f, false, NextChannelAction);
	}

	private void ChannelDialogue()
	{
		PsDialogueStep psDialogueStep = GameSceneEffectManager.m_dialogue.m_steps[m_dialogIndex];
		Action action = NextChannelAction;
		if (GameSceneEffectManager.m_crew != null)
		{
			GameSceneEffectManager.m_crew.Talk();
			GameSceneEffectManager.m_crew.LookAtCamera();
			if (GameSceneEffectManager.m_dialogue.m_steps.Count - 1 == m_dialogIndex)
			{
				action = (Action)Delegate.Combine(action, new Action(GameSceneEffectManager.m_crew.LookAwayFromCamera));
			}
		}
		m_dialogIndex++;
		GameSceneEffectManager.m_bubble = new PsUIFixedDialogueSpeechBubble(new Vector2((float)Screen.width * -0.2f, (float)Screen.height * -0.1f), psDialogueStep.m_charactertext, psDialogueStep.m_proceedText, psDialogueStep.m_cancelText, action, null, m_bubbleHandlePosition, m_bubbleHandleType);
	}

	private void ChannelSignalLevel()
	{
		GameSceneEffectManager.SetChannel(-1);
		GameSceneEffectManager.SetChannelRoll(true);
		TimerS.AddComponent(GameSceneEffectManager.m_utilityEntity, string.Empty, 1f, 0f, false, NextChannelAction);
	}

	private void ChannelPlaySignalLevel()
	{
		GameSceneEffectManager.StartMusic("/Cutscenes/RacingAudience_AmbienceLoop", false);
		GameSceneEffectManager.SetEffects(false);
		GameSceneEffectManager.DestroyCrew();
		m_endSequenceCallBack();
	}

	private void ChannelEndSignal()
	{
		if (m_endSequenceCallBack != null)
		{
			m_endSequenceCallBack();
		}
		else
		{
			PsState.m_activeGameLoop.ExitMinigame();
		}
		GameSceneEffectManager.DestroyCrew();
	}
}
