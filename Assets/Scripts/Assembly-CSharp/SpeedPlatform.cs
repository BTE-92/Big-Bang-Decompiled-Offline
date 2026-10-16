using System;
using UnityEngine;

public class SpeedPlatform : Unit
{
	private ChipmunkBodyC m_cmb;

	private int m_soundTriggerTimer;

	private IntPtr m_beltShape;

	public SpeedPlatform(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		GameObject gameObject = ResourceManager.GetGameObject("Units/MetalPlatformMediumPrefab");
		TransformC transformC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(transformC, _graphElement.m_position, _graphElement.m_rotation);
		GameObject gameObject2 = gameObject.transform.Find("MetalPlatformMedium").gameObject;
		PrefabC prefabC = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 10f) + GetZBufferBias(), gameObject2);
		ucpPolyShape ucpPolyShape2 = ChipmunkProS.GeneratePolyShapeFromGameObject(gameObject.transform.Find("Collision1").gameObject, Vector2.zero, 1f, 0.25f, 0.1f, (ucpCollisionType)4, 257u, false, m_graphElement.m_flipped);
		m_cmb = ChipmunkProS.AddStaticBody(transformC, new ucpShape[1] { ucpPolyShape2 }, m_unitC);
		m_beltShape = ucpPolyShape2.shapePtr;
		if (!m_minigame.m_editing)
		{
			ChipmunkProS.AddCollisionHandler(m_cmb, PlatformCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)3, true, true, false);
			ChipmunkProS.AddCollisionHandler(m_cmb, PlatformCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)4, true, true, false);
		}
		CreateEditorTouchArea(prefabC.p_gameObject);
		m_graphElement.m_isRotateable = true;
		m_graphElement.m_isFlippable = true;
		if (m_graphElement.m_flipped)
		{
			TransformS.SetScale(m_cmb.TC, new Vector3(-1f, 1f, 1f));
		}
	}

	private void PlatformCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB];
		UnitC unitC = chipmunkBodyC.customComponent as UnitC;
		if (unitC != null && unitC.m_unit != null && (_phase == ucpCollisionPhase.Begin || _phase == ucpCollisionPhase.Persist) && _pair.shapeA == m_beltShape && _phase == ucpCollisionPhase.Begin && m_soundTriggerTimer <= 0 && unitC.m_unit.m_entity == m_minigame.m_playerTC.p_entity)
		{
			m_soundTriggerTimer = 60;
			SoundS.PlaySingleShot("/Ingame/Units/BoostRamp", new Vector3(_pair.point.x, _pair.point.y, 0f));
		}
	}

	public override void Update()
	{
		base.Update();
		if (m_soundTriggerTimer > 0)
		{
			m_soundTriggerTimer--;
		}
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		Destroy();
	}
}
