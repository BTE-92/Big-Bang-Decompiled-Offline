using System;
using UnityEngine;

public class PsRollingBoulder : Unit
{
	private ChipmunkBodyC m_cmb;

	private PrefabC m_pc;

	private TransformC m_tc;

	private CameraTargetC m_ctc;

	public PsRollingBoulder(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_tc = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(m_tc, _graphElement.m_position + new Vector3(0f, 0f, 0f) + GetZBufferBias(), _graphElement.m_rotation);
		m_hitPoints = 1f;
		m_hitPointType = HitPointType.Lives;
		CreateBody();
		m_graphElement.m_isRotateable = false;
	}

	private void CreateBody()
	{
		if (m_minigame.m_editing)
		{
			LevelRollingBoulderNode levelRollingBoulderNode = m_graphElement as LevelRollingBoulderNode;
			int num = Enum.GetNames(typeof(PsRollingBoulderRadius)).Length;
			if (levelRollingBoulderNode.m_flipped)
			{
				int num2 = (int)(levelRollingBoulderNode.m_boulderRadius + 1);
				if (num2 >= num)
				{
					num2 = 0;
				}
				levelRollingBoulderNode.m_boulderRadius = (PsRollingBoulderRadius)(int)Enum.GetValues(typeof(PsRollingBoulderRadius)).GetValue(num2);
				levelRollingBoulderNode.m_flipped = false;
			}
		}
		float num3 = 1f;
		string resourceIdentifier = "BoulderBigPrefab_GameObject";
		switch (((LevelRollingBoulderNode)m_graphElement).m_boulderRadius)
		{
		case PsRollingBoulderRadius.Large260:
			num3 = 130f;
			resourceIdentifier = "BoulderBigPrefab_GameObject";
			break;
		case PsRollingBoulderRadius.Medium125:
			num3 = 62.5f;
			resourceIdentifier = "BoulderMediumPrefab_GameObject";
			break;
		case PsRollingBoulderRadius.Small60:
			num3 = 30f;
			resourceIdentifier = "BoulderSmallPrefab_GameObject";
			break;
		}
		float num4 = ChipmunkProWrapper.ucpAreaForCircle(0f, num3);
		float num5 = 0.05f;
		m_pc = PrefabS.AddComponent(m_tc, new Vector3(0f, 0f, 0f), ResourceManager.GetGameObject(resourceIdentifier));
		ucpCircleShape shape = new ucpCircleShape(num3, Vector2.zero, 17895696u, num4 * num5, 0.25f, 0.5f, (ucpCollisionType)4);
		m_cmb = ChipmunkProS.AddDynamicBody(m_tc, shape);
		m_cmb.customComponent = m_unitC;
		if (!m_minigame.m_editing)
		{
			m_ctc = CameraS.AddTargetComponent(m_tc, 400f, 400f, Vector2.zero);
			m_ctc.activeRadius = 0f;
		}
		else
		{
			CreateEditorTouchArea(m_pc.p_gameObject);
		}
	}

	public override void Update()
	{
		base.Update();
		if (!m_minigame.m_editing)
		{
			float magnitude = ChipmunkProWrapper.ucpBodyGetVel(m_cmb.body).magnitude;
			if (magnitude > 1f)
			{
				m_ctc.activeRadius = magnitude * 4f;
			}
			else
			{
				m_ctc.activeRadius = 0f;
			}
		}
	}
}
