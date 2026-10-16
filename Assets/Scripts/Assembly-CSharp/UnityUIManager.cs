using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UnityUIManager
{
	public static List<IUnityUIComponent> m_uiComponents;

	public static Dictionary<string, IUnityUIComponent> m_storage;

	public static Canvas m_canvas;

	public static EventSystem m_eventSystem;

	public static Hashtable m_hiddenUI;

	public static void Initialize()
	{
		if (m_uiComponents == null)
		{
			m_uiComponents = new List<IUnityUIComponent>();
		}
		if (m_storage == null)
		{
			m_storage = new Dictionary<string, IUnityUIComponent>();
		}
		if (EventSystem.current == null)
		{
			m_eventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
			EventSystem.current = m_eventSystem;
			EventSystem.current.gameObject.AddComponent<StandaloneInputModule>();
		}
		else
		{
			m_eventSystem = EventSystem.current;
		}
		if (m_canvas == null)
		{
			m_canvas = CreateCanvas();
			m_canvas.worldCamera = CameraS.AddCamera("UnityUICamera", true);
			m_canvas.worldCamera.cullingMask = 1 << LayerMask.NameToLayer("UI");
		}
	}

	public static GameObject AddGameObjectToCanvas(GameObject _uiGameObject)
	{
		GameObject gameObject = Object.Instantiate(_uiGameObject);
		gameObject.transform.SetParent(m_canvas.transform, false);
		return gameObject;
	}

	public static void Update()
	{
		for (int i = 0; i < m_uiComponents.Count; i++)
		{
			m_uiComponents[i].Step();
		}
	}

	public static void DisableEventSystem()
	{
		m_eventSystem.enabled = false;
	}

	public static void EnableEventSystem()
	{
		m_eventSystem.enabled = true;
	}

	public static void HideCanvas()
	{
		m_canvas.enabled = false;
	}

	public static void ShowCanvas()
	{
		m_canvas.enabled = true;
	}

	public static void Store(string _name, IUnityUIComponent _component)
	{
		m_uiComponents.Remove(_component);
		m_storage.Add(_name, _component);
		_component.SetActive(false);
	}

	public static bool GetFromStorage(string _name, out IUnityUIComponent _component)
	{
		if (m_storage.ContainsKey(_name))
		{
			_component = m_storage[_name];
			m_storage.Remove(_name);
			m_uiComponents.Add(_component);
			_component.SetActive(true);
			return true;
		}
		_component = null;
		return false;
	}

	public static void HideVisibleUI(object _holder)
	{
		if (m_hiddenUI == null)
		{
			m_hiddenUI = new Hashtable();
		}
		List<CanvasGroup> list = new List<CanvasGroup>();
		foreach (Transform item in m_canvas.transform)
		{
			CanvasGroup component = item.gameObject.GetComponent<CanvasGroup>();
			if (component.alpha > 0f)
			{
				component.alpha = 0f;
				component.interactable = false;
				list.Add(component);
			}
		}
		m_hiddenUI.Add(_holder, list);
	}

	public static void ShowHiddenUI(object _holder)
	{
		if (m_hiddenUI == null || !m_hiddenUI.ContainsKey(_holder))
		{
			return;
		}
		foreach (CanvasGroup item in m_hiddenUI[_holder] as List<CanvasGroup>)
		{
			if (item != null)
			{
				item.alpha = 1f;
				item.interactable = true;
			}
		}
		m_hiddenUI.Remove(_holder);
	}

	public static Canvas CreateCanvas()
	{
		GameObject gameObject = new GameObject("UnityUICanvas");
		gameObject.layer = LayerMask.NameToLayer("UI");
		gameObject.transform.localPosition = Vector3.zero;
		Canvas canvas = gameObject.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceCamera;
		CanvasScaler canvasScaler = gameObject.AddComponent<CanvasScaler>();
		canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		canvasScaler.referenceResolution = new Vector2(1334f, 750f);
		canvasScaler.matchWidthOrHeight = 1f;
		GraphicRaycaster graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
		return canvas;
	}
}
