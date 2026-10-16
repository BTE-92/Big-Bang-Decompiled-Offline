using UnityEngine;

public class UnsyncAnimation : MonoBehaviour
{
	private void Start()
	{
		GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime = Random.Range(0f, 1f);
	}
}
