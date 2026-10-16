using UnityEngine;

public class DecorationalPicketFence : Unit
{
	public LevelDecorationNode m_fenceNode;

	public DecorationalPicketFence(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_fenceNode = m_graphElement as LevelDecorationNode;
		m_fenceNode.m_isForegroundable = true;
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.DecorationPicketFencePrefab_GameObject);
		TransformC transformC = TransformS.AddComponent(m_entity, "PicketFence");
		float z = ((!m_fenceNode.m_inFront) ? 150f : (-75f));
		TransformS.SetTransform(transformC, _graphElement.m_position + new Vector3(0f, 0f, z) + GetZBufferBias(), _graphElement.m_rotation);
		PrefabS.AddComponent(transformC, Vector3.zero, gameObject, string.Empty);
		m_fenceNode.m_isFlippable = false;
		CreateEditorTouchArea(180f, 90f, transformC);
	}
}
