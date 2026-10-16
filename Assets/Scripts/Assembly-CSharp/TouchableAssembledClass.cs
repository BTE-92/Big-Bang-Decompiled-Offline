public class TouchableAssembledClass : BasicAssembledClass
{
	public Entity m_entity;

	public TouchableAssembledClass(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_entity = EntityManager.AddEntity(new string[2] { "GTAG_UNIT", _graphElement.m_name });
		m_assembledEntities.Add(m_entity);
		CreateGraphElementTouchArea(50f);
	}
}
