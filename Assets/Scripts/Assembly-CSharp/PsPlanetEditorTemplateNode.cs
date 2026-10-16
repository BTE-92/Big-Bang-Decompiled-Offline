using UnityEngine;

public class PsPlanetEditorTemplateNode : PsPlanetLevelNode
{
	public PsPlanetEditorTemplateNode(PsGameLoop _loop, bool _tutorial = false)
		: base(_loop, _tutorial)
	{
	}

	public override string GetTransformName()
	{
		return "PlanetTemplateNode";
	}

	public override void CreatePrefab()
	{
		if (m_prefab != null)
		{
			PrefabS.RemoveComponent(m_prefab);
			m_prefab = null;
		}
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.PathButtonFreshAndFreePrefab_GameObject);
		m_prefab = PrefabS.AddComponent(m_tc, Vector3.zero, gameObject, "TemplateNode", true, false);
		PrefabS.SetCamera(m_prefab.p_gameObject, m_loop.m_planet.m_planetCamera);
		m_checkpointEffect = m_prefab.p_gameObject.GetComponent<EffectCheckpoint>();
	}

	public override void SetState()
	{
		base.SetState();
		PsGameLoopEditorTemplate psGameLoopEditorTemplate = m_loop as PsGameLoopEditorTemplate;
		if (psGameLoopEditorTemplate.m_removedFromPath && !psGameLoopEditorTemplate.m_published)
		{
			m_checkpointEffect.Hide();
		}
	}
}
