using UnityEngine;

public class TouchableChildNode : BasicAssembledClass
{
	public Entity m_entity;

	public TouchableChildNode(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_entity = EntityManager.AddEntity(new string[2] { "GTAG_UNIT", _graphElement.m_name });
		m_assembledEntities.Add(m_entity);
		TransformC parentTC = TransformS.AddComponent(m_entity, _graphElement.m_position + Vector3.forward * 100f);
		CreateGraphElementTouchArea(50f, parentTC);
	}
}
