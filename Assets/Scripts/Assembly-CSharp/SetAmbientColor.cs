using UnityEngine;

public class SetAmbientColor : MonoBehaviour
{
	public Color ambientColor = new Color(0f, 0f, 0f, 0f);

	private void Start()
	{
		RenderSettings.ambientLight = ambientColor;
	}
}
