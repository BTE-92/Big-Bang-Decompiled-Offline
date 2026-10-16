using UnityEngine;

public class DebugBillboard : MonoBehaviour
{
	public Transform[] billboards;

	public Camera planetCamera;

	public Vector3 lookAtDirection;

	private void Update()
	{
		Transform[] array = billboards;
		foreach (Transform transform in array)
		{
			transform.LookAt(planetCamera.transform.position);
		}
	}
}
