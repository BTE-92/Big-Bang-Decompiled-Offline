using UnityEngine;

public class Screenshot : MonoBehaviour
{
	public bool m_hasShot;

	private bool m_takeShot;

	private RenderTexture m_texture;

	private Camera m_camera;

	private Vector3 m_shotCamPos;

	private Texture2D m_tex2D;

	public void Initialize(int _width, int _height)
	{
		// ARGB32 on every platform: the screenshot is read back (ReadPixels) and uploaded as a level screenshot, and reading an RGB565 target
		// gives a garbled picture on iPhone / iPad (mixed colours, half of it grey) and a flat grey one on PC.
		m_texture = new RenderTexture(_width, _height, 24, RenderTextureFormat.ARGB32);
	}

	private void OnRenderObject()
	{
		if (m_takeShot)
		{
			m_takeShot = false;
			RenderTexture targetTexture = m_camera.targetTexture;
			Vector3 position = m_camera.transform.position;
			m_camera.targetTexture = m_texture;
			m_camera.transform.position = m_shotCamPos;
			m_camera.aspect = 1f;
			m_camera.Render();
			m_camera.targetTexture = targetTexture;
			m_camera.transform.position = position;
			m_camera.ResetAspect();
			m_hasShot = true;
			if (m_tex2D != null)
			{
				Object.Destroy(m_tex2D);
			}
			m_tex2D = null;
			GetScreenshotTex2D();
		}
	}

	private void OnApplicationPause(bool _pauseStatus)
	{
		if (!_pauseStatus && m_tex2D != null)
		{
			Graphics.Blit(m_tex2D, m_texture);
		}
	}

	public void TakeScreenshot(Camera _cam, Vector3 _shotCamPos)
	{
		m_takeShot = true;
		m_camera = _cam;
		m_shotCamPos = _shotCamPos;
	}

	public byte[] GetScreenshotJPGBytes()
	{
		if (m_texture != null)
		{
			Texture2D screenshotTex2D = GetScreenshotTex2D();
			JPGEncoder jPGEncoder = new JPGEncoder(screenshotTex2D, 90f, string.Empty, true);
			if (jPGEncoder.isDone)
			{
				byte[] bytes = jPGEncoder.GetBytes();
				Object.DestroyImmediate(screenshotTex2D);
				return bytes;
			}
		}
		return null;
	}

	public Texture2D GetScreenshotTex2D()
	{
		if (m_texture != null && m_tex2D == null)
		{
			m_tex2D = new Texture2D(m_texture.width, m_texture.height, TextureFormat.RGB24, false, true);
			m_tex2D.wrapMode = TextureWrapMode.Clamp;
			RenderTexture.active = m_texture;
			m_tex2D.ReadPixels(new Rect(0f, 0f, m_texture.width, m_texture.height), 0, 0);
			m_tex2D.Apply();
			RenderTexture.active = null;
			return m_tex2D;
		}
		if (m_tex2D != null)
		{
			return m_tex2D;
		}
		return null;
	}

	public void OnDestroy()
	{
		if (m_texture != null)
		{
			Object.DestroyImmediate(m_texture);
			m_texture = null;
		}
		m_camera = null;
	}
}
