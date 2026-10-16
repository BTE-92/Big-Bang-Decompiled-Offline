using UnityEngine;

public class EffectOffroadCarEngineT2 : MonoBehaviour
{
	[HideInInspector]
	public OffroadCar vehicle;

	public Animator flapperAnimator;

	private bool flapperTriggered;

	private void Start()
	{
		if (PsState.m_activeMinigame != null)
		{
			vehicle = PsState.m_activeMinigame.m_playerUnit as OffroadCar;
		}
	}

	private void Update()
	{
		if (PsState.m_activeMinigame != null && flapperAnimator != null)
		{
			if ((Controller.GetButtonState("Throttle") == ControllerButtonState.ON && !flapperTriggered) || (Controller.GetButtonState("Reverse") == ControllerButtonState.ON && !flapperTriggered))
			{
				flapperAnimator.SetTrigger("Open");
				flapperAnimator.ResetTrigger("Close");
				flapperTriggered = true;
			}
			else if (Controller.GetButtonState("Throttle") == ControllerButtonState.OFF && Controller.GetButtonState("Reverse") == ControllerButtonState.OFF && flapperTriggered)
			{
				flapperAnimator.SetTrigger("Close");
				flapperAnimator.ResetTrigger("Open");
				flapperTriggered = false;
			}
		}
	}
}
