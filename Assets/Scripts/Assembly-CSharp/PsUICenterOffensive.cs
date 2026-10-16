using UnityEngine;

public class PsUICenterOffensive : UICanvas
{
	private PsUICrewMemberBase m_crew;

	public PsUICenterOffensive(UIComponent _parent)
		: base(_parent, false, string.Empty, null, string.Empty)
	{
		GetRoot().SetDrawHandler(PsUIDrawHandlers.DebriefBackground);
		SetWidth(1f, RelativeTo.ScreenWidth);
		SetHeight(0.85f, RelativeTo.ParentHeight);
		SetVerticalAlign(0f);
		RemoveDrawHandler();
		string text = "Is this level offending?\n\nRemember: if the level is too challenging, it's not a reason to report abuse!";
		m_crew = new PsUICrewMemberBase(m_camera, string.Empty);
		m_crew.CreateCharacter(PsDialogueCharacter.Architect, PsDialogueCharacterPosition.Right);
		m_crew.LookAtCamera();
		m_crew.Talk();
		PsUIFixedDialogueSpeechBubble psUIFixedDialogueSpeechBubble = new PsUIFixedDialogueSpeechBubble(new Vector2((float)Screen.width * 0.25f, (float)Screen.height * -0.2f), text, "YES", "NO", Ok, Cancel, SpeechBubbleHandlePosition.Right);
		psUIFixedDialogueSpeechBubble.SetCamera(m_camera, true, false);
	}

	private void Ok()
	{
		PsMetagameManager.SendRating(PsRating.Abuse, PsState.m_activeGameLoop, PsState.m_activeMinigame);
		PsState.m_activeGameLoop.m_minigameMetaData.rating = PsRating.Abuse;
		(GetRoot() as PsUIBasePopup).CallAction("Ok");
	}

	private void Cancel()
	{
		m_crew.LookAwayFromCamera();
		TimerS.AddComponent(EntityManager.AddEntity(), string.Empty, 0.2f, 0f, true, TimerHandler);
	}

	private void TimerHandler(TimerC _c)
	{
		(GetRoot() as PsUIBasePopup).CallAction("Cancel");
	}

	public override void Destroy()
	{
		m_crew.Destroy();
		base.Destroy();
	}
}
