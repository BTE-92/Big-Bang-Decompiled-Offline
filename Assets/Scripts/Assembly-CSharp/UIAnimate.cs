using UnityEngine;

public class UIAnimate : MonoBehaviour
{
	public Animator animator;

	public void ToggleState()
	{
		animator.SetTrigger("OnPointerUp");
	}
}
