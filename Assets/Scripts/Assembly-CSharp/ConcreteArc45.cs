using UnityEngine;

public class ConcreteArc45 : Unit
{
	private TransformC m_tc;

	private ChipmunkBodyC m_cmb;

	private PrefabC m_pc;

	public ConcreteArc45(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_tc = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetGlobalTransform(m_tc, _graphElement.m_position, _graphElement.m_rotation);
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.ConcreteArc45Prefab_GameObject);
		m_pc = PrefabS.AddComponent(m_tc, Vector3.zero + GetZBufferBias(), gameObject.transform.Find("arc").gameObject);
		ucpSegmentShape[] array = new ucpSegmentShape[6];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new ucpSegmentShape(gameObject.transform.Find("Colliders/locator" + (i + 1)).localPosition, gameObject.transform.Find("Colliders/locator" + (i + 2)).localPosition, 10f, Vector2.zero, 257u, 20f, 0.1f, 1f, (ucpCollisionType)4);
		}
		m_cmb = ChipmunkProS.AddStaticBody(m_tc, array);
		m_cmb.customComponent = m_unitC;
		m_graphElement.m_isRotateable = true;
		CreateEditorTouchArea(m_pc.p_gameObject);
	}
}
