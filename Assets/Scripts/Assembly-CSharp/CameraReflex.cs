using UnityEngine;

public class CameraReflex : MonoBehaviour
{
	private OLDTVTube _oldtvtube;

	private void Start()
	{
		_oldtvtube = GetComponent<OLDTVTube>();
		string text = WebCamTexture.devices[0].name;
		WebCamDevice[] devices = WebCamTexture.devices;
		for (int i = 0; i < devices.Length; i++)
		{
			WebCamDevice webCamDevice = devices[i];
			if (webCamDevice.isFrontFacing)
			{
				text = webCamDevice.name;
				break;
			}
		}
		Debug.Log("Using " + text + " as webcam");
		WebCamTexture webCamTexture = new WebCamTexture(text);
		_oldtvtube.reflex = webCamTexture;
		webCamTexture.Play();
	}
}
