using System;
using UnityEngine;

public class PsMenuDialogueFlow : PsDialogueFlow
{
	public Vector3 m_crewOriginalPos;

	public PsMenuDialogueFlow(PsGameLoop _gameLoop, PsNodeEventTrigger _dialogueTrigger, float _delay = 0f, Action _proceed = null)
		: base(_gameLoop, _dialogueTrigger, _delay, _proceed)
	{
		m_prodeedAction = _proceed;
		PsPlanetNode node = _gameLoop.m_node;
		bool flag = node == null || _gameLoop.m_nodeId == node.m_loop.m_nodeId || (_gameLoop.m_nodeId == node.m_loop.m_nodeId - 1 && !node.m_loop.m_activated);
		if (_gameLoop.m_dialogues != null && _gameLoop.m_dialogues.ContainsKey(_dialogueTrigger.ToString()) && flag)
		{
			Debug.LogWarning(_dialogueTrigger);
			m_dialogue = PsMetagameData.GetDialogueByIdentifier((string)_gameLoop.m_dialogues[_dialogueTrigger.ToString()]);
			m_dialogueIndex = 0;
			m_canvas = new UIScrollableCanvas(null, string.Empty);
			m_canvas.RemoveDrawHandler();
			m_canvas.Update();
			m_camera = CameraS.AddCamera("DialogueCamera", true);
			TimerS.AddComponent(PsMetagameManager.m_utilityEntity, "dialogueStartDelay", _delay, 0f, false, StartDelayHandler);
		}
		else if (_delay > 0f)
		{
			TimerS.AddComponent(PsMetagameManager.m_utilityEntity, "dialogueStartDelay", _delay, 0f, false, ProceedDelayHandler);
		}
		else
		{
			Proceed();
		}
	}

	private void CreateCrewButtonDelegate(TweenC _c)
	{
		m_crew.Talk();
		m_crew.LookAtCamera();
		Vector3 vector = new Vector3(5f, 20f, 0f);
		if (m_crew.m_character.m_character == PsDialogueCharacter.Architect)
		{
			vector = new Vector3(-5f, 20f, 0f);
		}
		Vector3 vector2 = m_crew.m_character.m_TC.transform.position + vector;
		Action a = null;
		a = ((m_dialogue.m_steps.Count - 1 != m_dialogueIndex) ? new Action(DialogueProceedAction) : ((Action)Delegate.Combine(a, new Action(DialogueProceedAction))));
		PsDialogueStep psDialogueStep = m_dialogue.m_steps[m_dialogueIndex];
		m_bubble = new PsUIFreeDialogueSpeechBubble(vector2, psDialogueStep.m_charactertext, psDialogueStep.m_proceedText, psDialogueStep.m_cancelText, a, base.Proceed, m_camera);
		m_dialogueIndex++;
	}

	private void DestroyScientiestButtonDelegate(TweenC _c)
	{
		if (m_crew != null)
		{
			m_crew.Destroy();
			m_crew = null;
		}
		Proceed();
	}

	protected override void DialogueProceedAction()
	{
		TouchAreaS.Enable();
		m_bubble = null;
		if (m_dialogue.m_steps.Count <= m_dialogueIndex)
		{
			if (m_crew != null)
			{
				Vector3 vector = -Vector3.right * Screen.height * 0.35f;
				if (m_crew.m_character.m_character == PsDialogueCharacter.Architect)
				{
					vector = Vector3.right * Screen.height * 0.35f;
				}
				TweenC tweenComponent = TweenS.AddTransformTween(m_crew.m_character.m_TC, TweenedProperty.Position, TweenStyle.ExpoInOut, m_crewOriginalPos + vector, 1f, 0f, true);
				TweenS.AddTweenEndEventListener(tweenComponent, DestroyScientiestButtonDelegate);
				PsMainMenuState.HideUIElementsSwitch();
			}
			else
			{
				Proceed();
			}
			return;
		}
		Action a = null;
		PsDialogueStep psDialogueStep = m_dialogue.m_steps[m_dialogueIndex];
		Vector3 vector2 = Vector3.zero;
		if (m_crew == null)
		{
			PsMainMenuState.HideUIElementsSwitch();
		}
		if (psDialogueStep.m_character == PsDialogueCharacter.Scientist)
		{
			if (m_crew == null)
			{
				m_crew = new PsUICrewMemberBase(m_camera, string.Empty);
				m_crew.SetDepthOffset(-20f);
				m_crew.CreateCharacter(PsDialogueCharacter.Scientist, PsDialogueCharacterPosition.Left);
				m_crewOriginalPos = m_crew.m_character.m_TC.transform.localPosition;
				TweenC tweenComponent2 = TweenS.AddTransformTween(m_crew.m_character.m_TC, TweenedProperty.Position, TweenStyle.ExpoInOut, m_crewOriginalPos - Vector3.right * Screen.height * 0.35f, m_crewOriginalPos, 1f, 0f, true);
				TweenS.AddTweenEndEventListener(tweenComponent2, CreateCrewButtonDelegate);
				return;
			}
			vector2 = m_crew.m_character.m_TC.transform.position + new Vector3(5f, 20f, 0f);
			m_crew.Talk();
			m_crew.LookAtCamera();
		}
		else if (psDialogueStep.m_character == PsDialogueCharacter.Mechanic)
		{
			if (m_crew == null)
			{
				m_crew = new PsUICrewMemberBase(m_camera, string.Empty);
				m_crew.SetDepthOffset(-20f);
				m_crew.CreateCharacter(PsDialogueCharacter.Mechanic, PsDialogueCharacterPosition.Left);
				m_crewOriginalPos = m_crew.m_character.m_TC.transform.localPosition;
				TweenC tweenComponent3 = TweenS.AddTransformTween(m_crew.m_character.m_TC, TweenedProperty.Position, TweenStyle.ExpoInOut, m_crewOriginalPos - Vector3.right * Screen.height * 0.35f, m_crewOriginalPos, 1f, 0f, true);
				TweenS.AddTweenEndEventListener(tweenComponent3, CreateCrewButtonDelegate);
				return;
			}
			vector2 = m_crew.m_character.m_TC.transform.position + new Vector3(5f, 20f, 0f);
			m_crew.Talk();
			m_crew.LookAtCamera();
		}
		else if (psDialogueStep.m_character == PsDialogueCharacter.Architect)
		{
			if (m_crew == null)
			{
				m_crew = new PsUICrewMemberBase(m_camera, string.Empty);
				m_crew.SetDepthOffset(-20f);
				m_crew.CreateCharacter(PsDialogueCharacter.Architect, PsDialogueCharacterPosition.Right);
				m_crewOriginalPos = m_crew.m_character.m_TC.transform.localPosition;
				TweenC tweenComponent4 = TweenS.AddTransformTween(m_crew.m_character.m_TC, TweenedProperty.Position, TweenStyle.ExpoInOut, m_crewOriginalPos + Vector3.right * Screen.height * 0.35f, m_crewOriginalPos, 1f, 0f, true);
				TweenS.AddTweenEndEventListener(tweenComponent4, CreateCrewButtonDelegate);
				return;
			}
			vector2 = m_crew.m_character.m_TC.transform.position + new Vector3(-5f, 20f, 0f);
			m_crew.Talk();
			m_crew.LookAtCamera();
		}
		m_bubble = new PsUIFreeDialogueSpeechBubble(_proceedOk: (m_dialogue.m_steps.Count - 1 != m_dialogueIndex) ? new Action(DialogueProceedAction) : ((Action)Delegate.Combine(a, new Action(DialogueProceedAction))), _rootPos: vector2, _text: psDialogueStep.m_charactertext, _okText: psDialogueStep.m_proceedText, _cancelText: psDialogueStep.m_cancelText, _proceedCancel: base.Proceed);
		m_bubble.SetCamera(m_camera, true, false);
		m_dialogueIndex++;
	}
}
