using UnityEngine;

public class EffectCheckpoint : MonoBehaviour
{
	public Renderer borderRenderer;

	public GameObject insideObject;

	private Material instancedBorderMaterial;

	public virtual void ShowObject()
	{
		if (insideObject != null)
		{
			insideObject.SetActive(true);
		}
	}

	public virtual void HideObject()
	{
		if (insideObject != null)
		{
			insideObject.SetActive(false);
		}
	}

	public virtual void SetLocked()
	{
	}

	public virtual void Activate()
	{
	}

	public virtual void SetIdleActive()
	{
	}

	public virtual void Claim()
	{
	}

	public virtual void SetClaimed()
	{
	}

	public virtual void Hide()
	{
	}

	public void SetBorderState(bool state)
	{
		instancedBorderMaterial = borderRenderer.material;
		if (state)
		{
			instancedBorderMaterial.color = new Color(0.8941f, 0.5725f, 0.2156f, 1f);
		}
		else
		{
			instancedBorderMaterial.color = new Color(0.9333f, 0.7137f, 0.4117f, 1f) * 1.05f;
		}
	}

	public virtual void OnDestroy()
	{
		Object.Destroy(instancedBorderMaterial);
	}
}
