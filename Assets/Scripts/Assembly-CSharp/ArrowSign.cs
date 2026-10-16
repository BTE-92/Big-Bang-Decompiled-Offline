using UnityEngine;

public class ArrowSign : Unit
{
	public ArrowSign(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.ArrowSignPrefab_GameObject);
		TransformC transformC = TransformS.AddComponent(m_entity, "ArrowSign");
		TransformS.SetTransform(transformC, _graphElement.m_position + new Vector3(0f, -0f, 300f) + GetZBufferBias(), _graphElement.m_rotation);
		PrefabS.AddComponent(transformC, Vector3.zero, gameObject);
		CreateEditorTouchArea(50f, 50f, transformC);
		m_graphElement.m_isRotateable = true;
	}
}
