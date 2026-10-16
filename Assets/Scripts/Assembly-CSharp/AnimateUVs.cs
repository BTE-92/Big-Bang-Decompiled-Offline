using UnityEngine;

public class AnimateUVs : MonoBehaviour
{
	public float xPan;

	public float yPan;

	private float xOffset;

	private float yOffset;

	private void Update()
	{
		if (PsState.m_activeMinigame == null || !PsState.m_activeMinigame.m_gamePaused)
		{
			xOffset = ToolBox.getRolledValue(xOffset + Main.m_gameDeltaTime * xPan, 0f, 1f);
			yOffset = ToolBox.getRolledValue(yOffset + Main.m_gameDeltaTime * yPan, 0f, 1f);
			GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(xOffset, yOffset));
		}
	}
}
