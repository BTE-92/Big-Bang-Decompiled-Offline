using UnityEngine;

public class EffectCheckpointChest : EffectCheckpoint
{
	public Animator checkpointAnimator;

	public ParticleSystem particleSystemA;

	public ParticleSystem particleSystemB;

	public override void Activate()
	{
		checkpointAnimator.CrossFade("Activate", 0.1f);
		SetBorderState(true);
	}

	public override void SetIdleActive()
	{
		checkpointAnimator.Play("IdleActive");
		if (particleSystemA != null)
		{
			particleSystemA.Stop();
		}
		SetBorderState(true);
	}

	public override void Claim()
	{
		checkpointAnimator.CrossFade("Deactivate", 0.1f);
		if (particleSystemA != null)
		{
			particleSystemA.Stop();
		}
		if (particleSystemB != null)
		{
			particleSystemB.Play();
		}
		SetBorderState(true);
	}

	public override void SetLocked()
	{
		checkpointAnimator.Play("IdleDeactive");
		if (particleSystemA != null)
		{
			particleSystemA.Stop();
		}
		SetBorderState(false);
	}

	public override void SetClaimed()
	{
		checkpointAnimator.Play("Deactivate", 0, 1f);
		if (particleSystemA != null)
		{
			particleSystemA.transform.gameObject.SetActive(false);
		}
		SetBorderState(true);
	}
}
