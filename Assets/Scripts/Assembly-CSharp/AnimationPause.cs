using UnityEngine;

public class AnimationPause : MonoBehaviour
{
	private float animSpeed;

	private bool animPaused;

	private void Update()
	{
		if (PsState.m_physicsPaused && !animPaused)
		{
			animSpeed = GetComponent<Animation>()[GetComponent<Animation>().clip.name].speed;
			GetComponent<Animation>()[GetComponent<Animation>().clip.name].speed = 0f;
			animPaused = true;
		}
		else if (!PsState.m_physicsPaused && animPaused)
		{
			GetComponent<Animation>()[GetComponent<Animation>().clip.name].speed = animSpeed;
			animPaused = false;
		}
	}
}
