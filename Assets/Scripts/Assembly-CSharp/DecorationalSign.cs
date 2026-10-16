using System;
using UnityEngine;

public class DecorationalSign : Unit
{
	public LevelDecorationNode m_signNode;

	public GameObject m_mainPrefab;

	public TransformC m_tc;

	public DecorationalSign(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_signNode = m_graphElement as LevelDecorationNode;
		m_signNode.m_isForegroundable = true;
		m_tc = TransformS.AddComponent(m_entity, "Sign");
		float z = ((!m_signNode.m_inFront) ? 150f : (-75f));
		TransformS.SetTransform(m_tc, m_signNode.m_position + new Vector3(0f, 0f, z) + GetZBufferBias(), m_signNode.m_rotation);
		if (m_minigame.m_editing)
		{
			int num = Enum.GetNames(typeof(PsSignIndex)).Length;
			if (m_signNode.m_flipped)
			{
				uint num2 = m_signNode.m_index + 1;
				if (num2 >= num)
				{
					num2 = 0u;
				}
				m_signNode.m_index = num2;
				m_signNode.m_flipped = false;
			}
		}
		m_mainPrefab = ResourceManager.GetGameObject(RESOURCE.DecorationalSignsPrefab_GameObject);
		GameObject gameObject = m_mainPrefab.transform.Find(((PsSignIndex)m_signNode.m_index).ToString()).gameObject;
		PrefabS.AddComponent(m_tc, Vector3.zero, gameObject);
		CreateEditorTouchArea(gameObject, m_tc);
	}
}
