using UnityEngine;

public class PsPlanetEditorPuzzleNode : PsPlanetLevelNode
{
	public PsPlanetEditorPuzzleNode(PsGameLoop _loop, bool _tutorial = false)
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
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.PathButtonPuzzlePrefab_GameObject);
		m_prefab = PrefabS.AddComponent(m_tc, Vector3.zero, gameObject, "PuzzleNode", true, false);
		PrefabS.SetCamera(m_prefab.p_gameObject, m_loop.m_planet.m_planetCamera);
		m_checkpointEffect = m_prefab.p_gameObject.GetComponent<EffectCheckpoint>();
		if (m_loop.m_path.m_startNodeId == 0)
		{
			m_checkpointEffect.HideObject();
		}
	}

	public override string GetTransformName()
	{
		return "PlanetNodeEditorPuzzle" + m_loop.m_nodeId;
	}
}
