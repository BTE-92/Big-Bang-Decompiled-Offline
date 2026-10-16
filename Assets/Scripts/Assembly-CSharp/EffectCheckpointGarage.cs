using UnityEngine;

public class EffectCheckpointGarage : EffectCheckpoint
{
	public Animator checkpointAnimator;

	public Texture[] textures;

	public MeshRenderer[] meshRenderers;

	private Material instancedMaterial;

	public void SetTextureToIndex(int index)
	{
		instancedMaterial = meshRenderers[0].material;
		instancedMaterial.mainTexture = textures[index];
		for (int i = 0; i < meshRenderers.Length; i++)
		{
			meshRenderers[i].sharedMaterial = instancedMaterial;
		}
	}

	public override void SetLocked()
	{
		checkpointAnimator.CrossFade("IdleDeactive", 0.1f);
		SetBorderState(false);
	}

	public override void Activate()
	{
		checkpointAnimator.CrossFade("Activate", 0.1f);
		SetTextureToIndex(0);
		SetBorderState(true);
	}

	public override void SetIdleActive()
	{
		checkpointAnimator.CrossFade("IdleActive", 0.1f);
		SetBorderState(true);
	}

	public override void Claim()
	{
		checkpointAnimator.CrossFade("Deactivate", 0.1f);
		SetTextureToIndex(1);
		SetBorderState(true);
	}

	public override void SetClaimed()
	{
		checkpointAnimator.CrossFade("IdleDeactive", 0.1f);
		SetTextureToIndex(1);
		SetBorderState(true);
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(instancedMaterial);
	}
}
