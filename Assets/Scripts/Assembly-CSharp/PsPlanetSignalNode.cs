using UnityEngine;

public class PsPlanetSignalNode : PsPlanetBlockNode
{
	public PsPlanetSignalNode(PsGameLoop _loop, bool _tutorial = false)
		: base(_loop, _tutorial)
	{
	}

	public override void CreatePrefab()
	{
		if (m_prefab != null)
		{
			PrefabS.RemoveComponent(m_prefab);
			m_prefab = null;
		}
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.CheckpointAntennaPrefab_GameObject);
		m_prefab = PrefabS.AddComponent(m_tc, Vector3.zero, gameObject);
		PrefabS.SetCamera(m_prefab.p_gameObject, m_loop.m_planet.m_planetCamera);
		m_checkpointEffect = m_prefab.p_gameObject.GetComponent<EffectCheckpoint>();
	}

	public override void CreateUI()
	{
	}

	public override void RemoveUI()
	{
	}
}
