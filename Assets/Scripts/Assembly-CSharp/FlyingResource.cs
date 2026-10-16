using UnityEngine;

public class FlyingResource
{
	public TransformC m_TC;

	public Entity m_entity;

	public PrefabC m_prefab;

	public FlyingResource(ResourceType _type, float _size, RelativeTo _relativeTo, Vector2 _position, Camera _camera = null)
	{
		m_entity = EntityManager.AddEntity();
		float num = _size;
		switch (_relativeTo)
		{
		case RelativeTo.ScreenHeight:
			num *= (float)Screen.height;
			break;
		case RelativeTo.ScreenWidth:
			num *= (float)Screen.width;
			break;
		case RelativeTo.ScreenShortest:
			num = ((Screen.height >= Screen.width) ? (num * (float)Screen.width) : (num * (float)Screen.height));
			break;
		case RelativeTo.ScreenLongest:
			num = ((Screen.height <= Screen.width) ? (num * (float)Screen.width) : (num * (float)Screen.height));
			break;
		}
		m_TC = TransformS.AddComponent(m_entity, "FlyingResource");
		TransformS.SetTransform(m_TC, _position, Vector3.zero);
		m_prefab = PrefabS.AddComponent(m_TC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.ResourcesAnimations_GameObject));
		if (_camera == null)
		{
			PrefabS.SetCamera(m_prefab, CameraS.m_uiCamera);
		}
		else
		{
			PrefabS.SetCamera(m_prefab, _camera);
		}
		Animator component = m_prefab.p_gameObject.GetComponent<Animator>();
		switch (_type)
		{
		case ResourceType.Coins:
			component.Play("ResourcesAnimationCoin");
			break;
		case ResourceType.Diamonds:
			component.Play("ResourcesAnimationGem");
			break;
		case ResourceType.Keys:
			component.Play("ResourcesAnimationKey");
			break;
		}
		m_prefab.p_gameObject.transform.localScale = new Vector3(num, num, num);
	}

	public void Destroy()
	{
		EntityManager.RemoveEntity(m_entity, true, true);
		m_entity = null;
		m_TC = null;
		m_prefab = null;
	}

	public void SetCamera(Camera _camera)
	{
		PrefabS.SetCamera(m_prefab, _camera);
	}
}
