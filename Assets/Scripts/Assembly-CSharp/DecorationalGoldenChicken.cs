using UnityEngine;

public class DecorationalGoldenChicken : Unit
{
	public LevelDecorationNode m_chickenNode;

	public DecorationalGoldenChicken(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_chickenNode = m_graphElement as LevelDecorationNode;
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.DecorationalGoldenChicken_GameObject);
		TransformC transformC = TransformS.AddComponent(m_entity, "GoldenChicken");
		TransformS.SetTransform(transformC, _graphElement.m_position + new Vector3(0f, 0f, 150f) + GetZBufferBias(), _graphElement.m_rotation);
		PrefabC prefabC = PrefabS.AddComponent(transformC, Vector3.zero, gameObject, string.Empty);
		m_chickenNode.m_isFlippable = true;
		if (m_chickenNode.m_flipped)
		{
			TransformS.SetScale(transformC, new Vector3(-1f, 1f, 1f));
		}
		CreateEditorTouchArea(50f, 50f, transformC);
	}
}
