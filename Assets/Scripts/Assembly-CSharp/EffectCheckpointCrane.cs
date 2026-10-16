using UnityEngine;

public class EffectCheckpointCrane : EffectCheckpoint
{
	public Animator checkpointAnimator;

	public override void SetLocked()
	{
		checkpointAnimator.CrossFade("IdleDeactive", 0.1f);
		SetBorderState(false);
	}

	public override void Activate()
	{
		checkpointAnimator.CrossFade("Activate", 0.1f);
		SetBorderState(true);
	}

	public override void SetIdleActive()
	{
		checkpointAnimator.CrossFade("IdleActive", 0.1f);
		SetBorderState(true);
	}

	public override void Claim()
	{
		checkpointAnimator.CrossFade("Deactivate", 0.25f);
		SetBorderState(true);
	}

	public override void SetClaimed()
	{
		checkpointAnimator.CrossFade("IdleDeactive", 0.1f);
		SetBorderState(true);
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
	}
}
