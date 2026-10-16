using UnityEngine;

public class EffectCheckpointPathButton : EffectCheckpoint
{
	public GameObject activeObject;

	public GameObject deactiveObject;

	public override void ShowObject()
	{
		base.ShowObject();
	}

	public override void HideObject()
	{
		base.HideObject();
	}

	public override void Activate()
	{
		activeObject.SetActive(true);
		deactiveObject.SetActive(false);
		SetBorderState(true);
	}

	public override void Claim()
	{
		activeObject.SetActive(false);
		deactiveObject.SetActive(true);
	}

	public override void SetIdleActive()
	{
		Activate();
		SetBorderState(true);
	}

	public override void SetLocked()
	{
		Claim();
		SetBorderState(false);
	}

	public override void SetClaimed()
	{
		Activate();
		SetBorderState(true);
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
	}

	public override void Hide()
	{
		activeObject.SetActive(false);
		deactiveObject.SetActive(false);
		if (insideObject != null)
		{
			insideObject.SetActive(false);
		}
	}
}
