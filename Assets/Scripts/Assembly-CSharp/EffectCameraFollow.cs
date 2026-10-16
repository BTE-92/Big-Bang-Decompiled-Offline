using UnityEngine;

public class EffectCameraFollow : MonoBehaviour
{
	private GameObject sceneCamera;

	private Vector3 transPos;

	public float yOffset;

	public float zOffset;

	private void Start()
	{
		sceneCamera = GameObject.FindGameObjectWithTag("MainCamera");
		transPos = base.transform.position;
		transPos.z = 0f + zOffset;
	}

	private void Update()
	{
		transPos.x = sceneCamera.transform.position.x;
		base.transform.position = transPos;
	}
}
