using UnityEngine;

public class UnityUIComponent<T> : IUnityUIComponent where T : UnityUIBundle, new()
{
	public GameObject m_uiGameObject;

	public T m_uiBundle;

	public CanvasGroup m_canvasGroup;

	public Camera m_followCamera;

	private float m_cameraOffset;

	public bool m_hidden;

	public UnityUIComponent(GameObject _uiGameObject)
	{
		UnityUIManager.m_uiComponents.Add(this);
		m_uiGameObject = UnityUIManager.AddGameObjectToCanvas(_uiGameObject);
		m_canvasGroup = m_uiGameObject.GetComponent<CanvasGroup>();
		if (m_canvasGroup == null)
		{
			m_canvasGroup = m_uiGameObject.AddComponent<CanvasGroup>();
		}
		m_uiBundle = m_uiGameObject.GetComponent<T>();
		LocUI.TranslateUI(m_uiGameObject);
	}

	public virtual void Step()
	{
		if (m_followCamera != null && m_followCamera.transform.hasChanged)
		{
			m_uiGameObject.transform.position = new Vector3(0f - m_followCamera.transform.position.x + m_cameraOffset, 0f, 0f);
		}
	}

	public virtual void Show()
	{
		m_hidden = false;
	}

	public virtual void Hide()
	{
		m_hidden = true;
	}

	public virtual void Destroy()
	{
		UnityUIManager.m_uiComponents.Remove(this);
		Object.Destroy(m_uiGameObject);
	}

	public void SetHorizontalScrollCamera(Camera _camera, float _offset = 0f)
	{
		m_followCamera = _camera;
		m_cameraOffset = _offset;
	}

	public void SetActive(bool _flag)
	{
		m_uiGameObject.SetActive(_flag);
	}
}
