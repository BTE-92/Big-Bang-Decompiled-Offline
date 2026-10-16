using System;
using UnityEngine;

public class PsMinigameDialogueFlow : PsDialogueFlow
{
	public PsMinigameDialogueFlow(PsGameLoop _gameLoop, PsNodeEventTrigger _dialogueTrigger, float _delay = 0f, Action _proceed = null)
		: base(_gameLoop, _dialogueTrigger, _delay, _proceed)
	{
		m_prodeedAction = _proceed;
		if (_dialogueTrigger == PsNodeEventTrigger.MinigameWin && PsState.m_activeMinigame.m_playerReachedGoalCount > 1)
		{
			Proceed();
			return;
		}
		if (_dialogueTrigger == PsNodeEventTrigger.MinigameLose && _gameLoop.m_dialogues != null && _gameLoop.m_dialogues.ContainsKey(PsNodeEventTrigger.MinigameLoseFirstTime.ToString()) && PsState.m_activeMinigame.m_gameDeathCount == 1)
		{
			_dialogueTrigger = PsNodeEventTrigger.MinigameLoseFirstTime;
		}
		if (_gameLoop.m_dialogues != null && _gameLoop.m_dialogues.ContainsKey(_dialogueTrigger.ToString()))
		{
			Debug.LogWarning(_dialogueTrigger);
			m_dialogue = PsMetagameData.GetDialogueByIdentifier((string)_gameLoop.m_dialogues[_dialogueTrigger.ToString()]);
			m_dialogueIndex = 0;
			TimerS.AddComponent(PsMetagameManager.m_utilityEntity, "dialogueStartDelay", _delay, 0f, false, StartDelayHandler);
			m_canvas = new UIScrollableCanvas(null, string.Empty);
			m_canvas.RemoveDrawHandler();
			m_canvas.Update();
			m_camera = CameraS.AddCamera("DialogueCamera", true);
			m_crew = new PsUICrewMemberBase(m_camera, string.Empty);
		}
		else
		{
			Proceed();
		}
	}

	protected override void DialogueProceedAction()
	{
		m_bubble = null;
		if (m_dialogue.m_steps.Count <= m_dialogueIndex)
		{
			Proceed();
			return;
		}
		PsDialogueStep psDialogueStep = m_dialogue.m_steps[m_dialogueIndex];
		SpeechBubbleHandlePosition handlePosition = SpeechBubbleHandlePosition.Left;
		if (psDialogueStep.m_character == PsDialogueCharacter.Mechanic)
		{
			m_crew.CreateCharacter(PsDialogueCharacter.Mechanic, PsDialogueCharacterPosition.Left);
		}
		else if (psDialogueStep.m_character == PsDialogueCharacter.Scientist)
		{
			m_crew.CreateCharacter(PsDialogueCharacter.Scientist, PsDialogueCharacterPosition.Left);
		}
		else if (psDialogueStep.m_character == PsDialogueCharacter.Architect)
		{
			m_crew.CreateCharacter(PsDialogueCharacter.Architect, PsDialogueCharacterPosition.Right);
			handlePosition = SpeechBubbleHandlePosition.Right;
		}
		m_crew.Talk();
		m_crew.LookAtCamera();
		Action action = DialogueProceedAction;
		if (m_dialogue.m_steps.Count - 1 == m_dialogueIndex)
		{
			action = (Action)Delegate.Combine(action, new Action(m_crew.LookAwayFromCamera));
		}
		m_bubble = new PsUIFixedDialogueSpeechBubble(new Vector2((float)Screen.width * -0.2f, (float)Screen.height * -0.1f), psDialogueStep.m_charactertext, psDialogueStep.m_proceedText, psDialogueStep.m_cancelText, action, base.Proceed, handlePosition);
		m_bubble.SetCamera(m_camera, true, false);
		m_dialogueIndex++;
	}
}
