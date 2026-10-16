using UnityEngine;

public class GoalSpawnPoint : BasicAssembledClass
{
	public Entity m_entity;

	public TransformC m_tc;

	public GoalSpawnPoint(GraphElement _graphElement)
		: base(_graphElement)
	{
		_graphElement.m_isCopyable = true;
		_graphElement.m_isRemovable = true;
		_graphElement.m_name = "GoalSpawnPoint";
		m_entity = EntityManager.AddEntity(new string[1] { _graphElement.m_name });
		m_assembledEntities.Add(m_entity);
		m_tc = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(m_tc, _graphElement.m_position, _graphElement.m_rotation);
		TextMeshC tmc = TextMeshS.AddComponent(m_tc, Vector3.zero, "HurmeSemiBold_Font", 0f, 0f, 50f, Align.Center, Align.Middle, CameraS.m_mainCamera, string.Empty);
		TextMeshS.SetText(tmc, "Goal", false);
		CreateGraphElementTouchArea(50f, m_tc);
	}
}
