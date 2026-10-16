using UnityEngine;

public class RotateObject : MonoBehaviour
{
	public float rotateX;

	public float rotateY;

	public float rotateZ;

	private void Update()
	{
		if ((!(GetComponent<Renderer>() != null) || GetComponent<Renderer>().isVisible) && (PsState.m_activeMinigame == null || !PsState.m_activeMinigame.m_gamePaused))
		{
			base.transform.Rotate(new Vector3(rotateX * Main.m_gameDeltaTime, rotateY * Main.m_gameDeltaTime, rotateZ * Main.m_gameDeltaTime));
		}
	}
}
