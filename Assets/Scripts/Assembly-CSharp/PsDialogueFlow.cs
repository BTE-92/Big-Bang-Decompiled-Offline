using System;
using UnityEngine;

public class PsDialogueFlow
{
	protected int m_dialogueIndex;

	protected PsDialogue m_dialogue;

	protected Action m_prodeedAction;

	protected UIScrollableCanvas m_canvas;

	protected PsUICrewMemberBase m_crew;

	protected Camera m_camera;

	protected UIComponent m_bubble;

	public PsDialogueFlow(PsGameLoop _gameLoop, PsNodeEventTrigger _dialogueTrigger, float _delay = 0f, Action _proceed = null)
	{
		PsState.m_dialogues.Add(this);
	}

	protected virtual void StartDelayHandler(TimerC _c)
	{
		TimerS.RemoveComponent(_c);
		DialogueProceedAction();
	}

	protected virtual void ProceedDelayHandler(TimerC _c)
	{
		TimerS.RemoveComponent(_c);
		Proceed();
	}

	protected virtual void DialogueProceedAction()
	{
		Debug.LogWarning("OVERWRITE THIS");
	}

	protected void Proceed()
	{
		Destroy();
		if (m_prodeedAction != null)
		{
			m_prodeedAction();
		}
	}

	public void Destroy()
	{
		if (m_bubble != null)
		{
			m_bubble.Destroy();
			m_bubble = null;
		}
		if (m_crew != null)
		{
			m_crew.Destroy();
			m_crew = null;
		}
		if (m_camera != null)
		{
			CameraS.RemoveCamera(m_camera);
			m_camera = null;
		}
		if (m_canvas != null)
		{
			m_canvas.Destroy();
			m_canvas = null;
		}
		PsState.m_dialogues.Remove(this);
	}
}
