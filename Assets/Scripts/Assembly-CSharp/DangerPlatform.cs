using UnityEngine;

public class DangerPlatform : Unit
{
	private ChipmunkBodyC m_cmb;

	public DangerPlatform(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.DangerPlatformPrefab_GameObject);
		TransformC transformC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(transformC, _graphElement.m_position, _graphElement.m_rotation);
		PrefabC prefabC = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 10f) + GetZBufferBias(), gameObject);
		ucpPolyShape shape = ChipmunkProS.GeneratePolyShapeFromGameObject(gameObject.transform.Find("Collision2").gameObject, Vector2.zero, 1f, 0.25f, 0.9f, (ucpCollisionType)4, 257u);
		m_cmb = ChipmunkProS.AddStaticBody(transformC, shape, m_unitC);
		ucpPolyShape shape2 = ChipmunkProS.GeneratePolyShapeFromGameObject(gameObject.transform.Find("Collision1").gameObject, Vector2.zero, 1f, 0.25f, 0.9f, (ucpCollisionType)4, 257u);
		ChipmunkProS.AddStaticBody(transformC, shape2, m_unitC);
		if (!m_minigame.m_editing)
		{
			ChipmunkProS.AddCollisionHandler(m_cmb, CollisionHandler, (ucpCollisionType)4, (ucpCollisionType)3, true, false, true);
			ChipmunkProS.AddCollisionHandler(m_cmb, CollisionHandler, (ucpCollisionType)4, (ucpCollisionType)4, true, false, true);
		}
		CreateEditorTouchArea(prefabC.p_gameObject);
		m_graphElement.m_isRotateable = true;
	}

	private void CollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB];
		UnitC unitC = chipmunkBodyC.customComponent as UnitC;
		if (unitC == null)
		{
			return;
		}
		switch (_phase)
		{
		case ucpCollisionPhase.Begin:
			if (unitC.m_unit.m_canElectrify)
			{
				if (unitC.m_unit.m_electricitySourceCount == 0)
				{
					unitC.m_unit.SetElectrified(true, true, _pair.point);
				}
				unitC.m_unit.m_electricitySourceCount++;
			}
			break;
		case ucpCollisionPhase.Separate:
			if (unitC.m_unit.m_canElectrify)
			{
				unitC.m_unit.m_electricitySourceCount--;
				if (unitC.m_unit.m_electricitySourceCount == 0)
				{
					unitC.m_unit.SetElectrified(false);
				}
			}
			break;
		}
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		Destroy();
	}
}
