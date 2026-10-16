using System;
using UnityEngine;

public class DecorationalRock : Unit
{
	public LevelDecorationNode m_rockNode;

	public Entity m_touchEntity;

	public DecorationalRock(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_rockNode = m_graphElement as LevelDecorationNode;
		m_rockNode.m_isForegroundable = true;
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.DecorationRocksPrefab_GameObject);
		TransformC transformC = TransformS.AddComponent(m_entity, "Rock");
		float z = ((!m_rockNode.m_inFront) ? 150f : (-75f));
		TransformS.SetTransform(transformC, _graphElement.m_position + new Vector3(0f, 0f, z) + GetZBufferBias(), _graphElement.m_rotation);
		if (m_minigame.m_editing)
		{
			int num = Enum.GetNames(typeof(PsTreeIndex)).Length;
			if (m_rockNode.m_flipped)
			{
				uint num2 = m_rockNode.m_index + 1;
				if (num2 >= num)
				{
					num2 = 0u;
				}
				m_rockNode.m_index = num2;
				m_rockNode.m_flipped = false;
				float y = 14f;
				m_graphElement.m_storedRotation += new Vector3(0f, y, 0f);
			}
		}
		GameObject gameObject2 = gameObject.transform.Find(((PsRockIndex)m_rockNode.m_index).ToString()).gameObject;
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
