using UnityEngine;

public class PsUICenterLoseRace : UICanvas
{
	private PsUIGenericButton m_everyplayButton;

	public PsUICenterLoseRace(UIComponent _parent)
		: base(_parent, true, string.Empty, null, string.Empty)
	{
		SetWidth(1f, RelativeTo.ScreenWidth);
		SetHeight(1f, RelativeTo.ScreenHeight);
		RemoveDrawHandler();
		UIText uIText = new UIText(this, false, string.Empty, "<color=#d1f0d8>Tap to</color> <i><color=#2EFE9A>retry</color></i>", "HurmeBold_Font", 0.08f, RelativeTo.ScreenHeight);
		uIText.SetVerticalAlign(0.95f);
		TweenC c = TweenS.AddTransformTween(uIText.m_TC, TweenedProperty.Scale, TweenStyle.QuadInOut, new Vector3(1.05f, 1.05f, 1f), 0.6f, 0f, false);
		TweenS.SetAdditionalTweenProperties(c, -1, true, TweenStyle.QuadInOut);
		if (EveryplayManager.IsEnabled() && !(PsState.m_activeGameLoop is PsGameLoopBlockSignal))
		{
			m_everyplayButton = new PsUIGenericButton(this);
			m_everyplayButton.SetIcon("hud_button_replay", 0.06f);
			m_everyplayButton.SetText("Watch\nReplay!");
			m_everyplayButton.SetBlueColors();
			m_everyplayButton.SetMargins(0.035f, 0.035f, 0.02f, 0.02f);
			m_everyplayButton.SetVerticalAlign(0.05f);
		}
	}

	public override void Step()
	{
		if (m_everyplayButton != null && m_everyplayButton.m_hit)
		{
			Debug.Log("Opening everyplay...");
			EveryplayManager.StopRecording(0f);
			EveryplayManager.PlayLastRecording();
		}
		else if (m_hit || Input.GetKeyDown(KeyCode.RightShift))
		{
			(GetRoot() as PsUIBasePopup).CallAction("Restart");
		}
		base.Step();
	}
}
