using UnityEngine;

public class AlienIKController : MonoBehaviour
{
	public static AlienIKController instance;

	public Animator animator;

	public Transform ikGoalHandLeft;

	public Transform ikGoalHandRight;

	public Motorcycle motorcycle;

	public float maxHeight = 1f;

	public float minHeight = -1f;

	public float ikWeight = 1f;

	private void Awake()
	{
		instance = this;
	}

	private void OnAnimatorIK()
	{
		if (ikGoalHandLeft == null || ikGoalHandRight == null)
		{
			Debug.Log("IK goals set!");
			ikGoalHandLeft = GameObject.Find("IKGoalHandLeft").transform;
			ikGoalHandRight = GameObject.Find("IKGoalHandRight").transform;
		}
		if (ikGoalHandLeft != null && ikGoalHandRight != null)
		{
			animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
			animator.SetIKPosition(AvatarIKGoal.LeftHand, ikGoalHandLeft.position);
			animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikWeight);
			animator.SetIKRotation(AvatarIKGoal.LeftHand, ikGoalHandLeft.rotation);
			animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);
			animator.SetIKPosition(AvatarIKGoal.RightHand, ikGoalHandRight.position);
			animator.SetIKRotationWeight(AvatarIKGoal.RightHand, ikWeight);
			animator.SetIKRotation(AvatarIKGoal.RightHand, ikGoalHandRight.rotation);
		}
	}

	private void OnDestroy()
	{
		instance = null;
	}
}
