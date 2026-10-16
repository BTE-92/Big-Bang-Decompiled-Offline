using UnityEngine;

public class PsUICharacter : UICanvas
{
	private TransformC m_prefabTC;

	private MenuCharacter m_animationScript;

	private PsDialogueCharacterPosition m_position;

	public PsDialogueCharacter m_character;

	private float m_customScale;

	public PsUICharacter(UIComponent _parent, PsDialogueCharacter _character, PsDialogueCharacterPosition _position, float _customScale = 1f)
		: base(_parent, true, "PsUICharacter", null, string.Empty)
	{
		RemoveDrawHandler();
		m_character = _character;
		m_position = _position;
		m_customScale = _customScale;
		base.SetHeight(0.4f, RelativeTo.OwnHeight);
		base.SetWidth(0.8f, RelativeTo.OwnHeight);
		m_prefabTC = TransformS.AddComponent(m_TC.p_entity);
		TransformS.ParentComponent(m_prefabTC, m_TC);
		GameObject gameObject = null;
		switch (_character)
		{
		case PsDialogueCharacter.Scientist:
			gameObject = ResourceManager.GetGameObject(RESOURCE.MenuCharProfPrefab_GameObject);
			break;
		case PsDialogueCharacter.Mechanic:
			gameObject = ResourceManager.GetGameObject(RESOURCE.MenuCharMechPrefab_GameObject);
			break;
		case PsDialogueCharacter.Architect:
			gameObject = ResourceManager.GetGameObject(RESOURCE.MenuCharBuildPrefab_GameObject);
			break;
		}
		PrefabC prefabC = PrefabS.AddComponent(m_prefabTC, Vector3.zero, gameObject);
		PrefabS.SetCamera(prefabC, m_camera);
		m_animationScript = prefabC.p_gameObject.GetComponent<MenuCharacter>();
		TransformS.SetRotation(m_prefabTC, Vector3.up * 180f);
	}

	public override void Update()
	{
		base.Update();
		TransformS.SetScale(m_prefabTC, Vector3.one * (m_actualHeight / (float)Screen.height) * ((float)Screen.height * 0.001f * m_customScale));
		if (m_position == PsDialogueCharacterPosition.Right && m_character != PsDialogueCharacter.Architect)
		{
			TransformS.SetScale(m_prefabTC, new Vector3(0f - m_prefabTC.transform.localScale.x, m_prefabTC.transform.localScale.y, m_prefabTC.transform.localScale.z));
		}
		else if (m_position == PsDialogueCharacterPosition.Left && m_character == PsDialogueCharacter.Architect)
		{
			TransformS.SetScale(m_prefabTC, new Vector3(0f - m_prefabTC.transform.localScale.x, m_prefabTC.transform.localScale.y, m_prefabTC.transform.localScale.z));
		}
	}

	public new void SetWidth(float _width, RelativeTo _relativeTo)
	{
		Debug.LogWarning("Do not use PsUICharacterButton.SetWidth() method!");
	}

	public new void SetSize(float _width, float _height, RelativeTo _relativeTo)
	{
		Debug.LogWarning("Do not use PsUICharacterButton.SetSize() method!");
	}

	public void Talk()
	{
		if (m_animationScript != null)
		{
			switch (m_character)
			{
			case PsDialogueCharacter.Scientist:
				SoundS.PlaySingleShot("/Ingame/Characters/Dialogue_Professor", Vector2.zero);
				break;
			case PsDialogueCharacter.Mechanic:
				SoundS.PlaySingleShot("/Ingame/Characters/Dialogue_Mechanic", Vector2.zero);
				break;
			case PsDialogueCharacter.Architect:
				SoundS.PlaySingleShot("/Ingame/Characters/Dialogue_Builder", Vector2.zero);
				break;
			}
			m_animationScript.Talk();
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
}
