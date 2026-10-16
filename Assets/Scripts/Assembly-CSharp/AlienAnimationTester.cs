using UnityEngine;

public class AlienAnimationTester : MonoBehaviour
{
	public int blinkDensity = 75;

	public int cheerDensity = 10;

	private Animator curAnim;

	private void Awake()
	{
		curAnim = GetComponentInChildren<Animator>();
	}

	private void Update()
	{
		if (Random.Range(0, blinkDensity) == 1 && !curAnim.GetCurrentAnimatorStateInfo(2).IsName("Hit"))
		{
			curAnim.SetTrigger("Hit");
		}
		if (Random.Range(0, cheerDensity) == 1 && !curAnim.GetCurrentAnimatorStateInfo(2).IsName("JumpCheer_A"))
		{
			curAnim.SetTrigger("JumpCheer");
		}
	}
}
