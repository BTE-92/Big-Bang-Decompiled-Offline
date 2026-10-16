using System.Collections.Generic;
using UnityEngine;

public class PsUICharacterButton : UICanvas
{
	private TransformC m_prefabTC;

	private MenuCharacter m_animationScript;

	private bool m_scaleOnTouch = true;

	private TweenC m_touchScaleTween;

	private Renderer[] m_renderers;

	private List<Material> m_removeMats = new List<Material>();

	public PsUICharacterButton(UIComponent _parent, PsDialogueCharacter _character)
		: base(_parent, true, "PsUICharacterButton", null, string.Empty)
	{
		RemoveDrawHandler();
		base.SetWidth(0.8f, RelativeTo.OwnHeight);
		m_prefabTC = TransformS.AddComponent(m_TC.p_entity);
		TransformS.ParentComponent(m_prefabTC, m_TC);
		GameObject gameObject = null;
		switch (_character)
		{
		case PsDialogueCharacter.Scientist:
			gameObject = ResourceManager.GetGameObject(RESOURCE.MenuCharProfButtonPrefab_GameObject);
			break;
		case PsDialogueCharacter.Mechanic:
			gameObject = ResourceManager.GetGameObject(RESOURCE.MenuCharMechButtonPrefab_GameObject);
			break;
		case PsDialogueCharacter.Architect:
			gameObject = ResourceManager.GetGameObject(RESOURCE.MenuCharBuildButtonPrefab_GameObject);
			break;
		}
		PrefabC prefabC = PrefabS.AddComponent(m_prefabTC, Vector3.zero, gameObject);
		PrefabS.SetCamera(prefabC, m_camera);
		m_animationScript = prefabC.p_gameObject.GetComponent<MenuCharacter>();
		m_renderers = prefabC.p_gameObject.GetComponentsInChildren<Renderer>(true);
		TransformS.SetRotation(m_prefabTC, Vector3.up * 180f);
	}

	public override void Update()
	{
		base.Update();
		TransformS.SetScale(m_prefabTC, Vector3.one * (m_actualHeight / (float)Screen.height) * ((float)Screen.height * 0.0012f));
	}

	public new void SetWidth(float _width, RelativeTo _relativeTo)
	{
		Debug.LogWarning("Do not use PsUICharacterButton.SetWidth() method!");
	}

	public new void SetSize(float _width, float _height, RelativeTo _relativeTo)
	{
		Debug.LogWarning("Do not use PsUICharacterButton.SetSize() method!");
	}

	protected override void OnTouchRollIn(TLTouch _touch, bool _secondary)
	{
		base.OnTouchRollIn(_touch, _secondary);
		if (m_scaleOnTouch)
		{
			if (m_touchScaleTween != null)
			{
				TweenS.RemoveComponent(m_touchScaleTween);
				m_touchScaleTween = null;
			}
			m_touchScaleTween = TweenS.AddTransformTween(m_TC, TweenedProperty.Scale, TweenStyle.BackOut, new Vector3(1.05f, 1.05f, 1.05f), 0.1f, 0f, false);
			Poke();
		}
	}

	protected override void OnTouchBegan(TLTouch _touch)
	{
		base.OnTouchBegan(_touch);
		if (m_scaleOnTouch)
		{
			if (m_touchScaleTween != null)
			{
				TweenS.RemoveComponent(m_touchScaleTween);
				m_touchScaleTween = null;
			}
			m_touchScaleTween = TweenS.AddTransformTween(m_TC, TweenedProperty.Scale, TweenStyle.BackOut, new Vector3(1.05f, 1.05f, 1.05f), 0.1f, 0f, false);
			Poke();
		}
	}

	protected override void OnTouchRollOut(TLTouch _touch, bool _secondary)
	{
		base.OnTouchRollOut(_touch, _secondary);
		if (m_scaleOnTouch)
		{
			if (m_touchScaleTween != null)
			{
				TweenS.RemoveComponent(m_touchScaleTween);
				m_touchScaleTween = null;
			}
			m_touchScaleTween = TweenS.AddTransformTween(m_TC, TweenedProperty.Scale, TweenStyle.BackOut, new Vector3(1f, 1f, 1f), 0.1f, 0f, false);
		}
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		base.OnTouchRelease(_touch, _inside);
		if (m_scaleOnTouch)
		{
			if (m_touchScaleTween != null)
			{
				TweenS.RemoveComponent(m_touchScaleTween);
				m_touchScaleTween = null;
			}
			m_touchScaleTween = TweenS.AddTransformTween(m_TC, TweenedProperty.Scale, TweenStyle.BackOut, new Vector3(1f, 1f, 1f), 0.1f, 0f, false);
			SoundS.PlaySingleShot("/UI/ButtonNormal", Vector3.zero);
		}
	}

	public void Activate()
	{
		m_animationScript.Activate();
	}

	public void Deactivate()
	{
		m_animationScript.Deactivate();
	}

	public void Talk()
	{
		if (m_animationScript != null)
		{
			m_animationScript.Talk();
		}
	}

	public void Poke()
	{
		if (m_animationScript != null)
		{
			m_animationScript.Poke();
		}
	}

	public void LookAtCamera()
	{
		if (m_animationScript != null)
		{
			m_animationScript.LookAtCamera();
		}
	}

	public void LookAwayFromCamera()
	{
		if (m_animationScript != null)
		{
			m_animationScript.LookAwayFromCamera();
		}
	}

	public override void Destroy()
	{
		for (int num = m_removeMats.Count - 1; num >= 0; num--)
		{
			Object.Destroy(m_removeMats[num]);
		}
		m_removeMats.Clear();
		m_renderers = null;
		m_removeMats = null;
		base.Destroy();
	}
}
