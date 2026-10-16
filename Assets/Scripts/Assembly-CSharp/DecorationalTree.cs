using System;
using UnityEngine;

public class DecorationalTree : Unit
{
	public LevelDecorationNode m_treeNode;

	public Entity m_touchEntity;

	public DecorationalTree(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_treeNode = m_graphElement as LevelDecorationNode;
		m_treeNode.m_isForegroundable = true;
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.DecorationalTreesPrefab_GameObject);
		TransformC transformC = TransformS.AddComponent(m_entity, "Tree");
		float z = ((!m_treeNode.m_inFront) ? 150f : (-75f));
		TransformS.SetTransform(transformC, _graphElement.m_position + new Vector3(0f, 0f, z) + GetZBufferBias(), _graphElement.m_rotation);
		if (m_minigame.m_editing)
		{
			int num = Enum.GetNames(typeof(PsTreeIndex)).Length;
			if (m_treeNode.m_flipped)
			{
				uint num2 = m_treeNode.m_index + 1;
				if (num2 >= num)
				{
					num2 = 0u;
				}
				m_treeNode.m_index = num2;
				m_treeNode.m_flipped = false;
				float y = 14f;
				m_graphElement.m_storedRotation += new Vector3(0f, y, 0f);
			}
		}
		GameObject gameObject2 = gameObject.transform.Find(((PsTreeIndex)m_treeNode.m_index).ToString()).gameObject;
		PrefabC prefabC = PrefabS.AddComponent(transformC, Vector3.zero, gameObject2);
		m_touchEntity = EntityManager.AddEntity();
		TransformC transformC2 = TransformS.AddComponent(m_touchEntity, "TouchCollider");
		TransformS.ParentComponent(transformC2, transformC, Vector3.zero);
		if (m_graphElement.m_storedRotation == Vector3.zero)
		{
			float y2 = UnityEngine.Random.Range(0f, 360f);
			int num3 = UnityEngine.Random.Range(0, 2);
			m_graphElement.m_storedRotation = new Vector3(num3, y2, 0f);
		}
		if ((int)m_graphElement.m_storedRotation[0] < 1)
		{
			TransformS.SetScale(transformC, new Vector3(-1f, 1f, 1f));
		}
		prefabC.p_gameObject.transform.localRotation *= Quaternion.Euler(new Vector3(0f, m_graphElement.m_storedRotation[1], 0f));
		transformC2.transform.localRotation = prefabC.p_gameObject.transform.localRotation;
		CreateEditorTouchArea(gameObject2, transformC2);
		m_graphElement.m_isRotateable = true;
	}

	public override void Destroy()
	{
		EntityManager.RemoveEntity(m_touchEntity);
		m_touchEntity = null;
		base.Destroy();
	}
}
