using System;
using System.Collections.Generic;
using UnityEngine;

public class SpeedRamp : Unit
{
	private ChipmunkBodyC m_cmb;

	private int m_soundTriggerTimer;

	private IntPtr m_beltShape;

	private Material m_beltMaterial;

	private bool m_updater;

	private int m_addSpeedCounter;

	private List<UnitC> m_speedAddedList;

	private float m_rampBaseAngle;

	public SpeedRamp(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_speedAddedList = new List<UnitC>();
		m_updater = false;
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.SpeedRampPrefab_GameObject);
		TransformC transformC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(transformC, _graphElement.m_position, _graphElement.m_rotation);
		GameObject gameObject2 = gameObject.transform.Find("BoostRampBody").gameObject;
		PrefabC prefabC = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 10f) + GetZBufferBias(), gameObject2);
		PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 10f) + GetZBufferBias(), gameObject.transform.Find("BoostRampBelt").gameObject);
		m_beltMaterial = gameObject.transform.Find("BoostRampBelt").GetComponent<Renderer>().sharedMaterial;
		ucpPolyShape ucpPolyShape2 = ChipmunkProS.GeneratePolyShapeFromGameObject(gameObject.transform.Find("CollisionBelt").gameObject, Vector2.zero, 1f, 0.25f, 0.1f, (ucpCollisionType)4, 257u, false, m_graphElement.m_flipped);
		ucpPolyShape[] array = ChipmunkProS.GeneratePolyShapesFromChildren(gameObject.transform.Find("CollisionBody").gameObject, Vector2.zero, 1f, 0.25f, 0.9f, (ucpCollisionType)4, 257u, false, m_graphElement.m_flipped);
		ucpPolyShape[] array2 = new ucpPolyShape[array.Length + 1];
		array.CopyTo(array2, 0);
		array2[array2.Length - 1] = ucpPolyShape2;
		m_cmb = ChipmunkProS.AddStaticBody(transformC, array2, m_unitC);
		m_beltShape = ucpPolyShape2.shapePtr;
		if (!m_minigame.m_editing)
		{
			ChipmunkProS.AddCollisionHandler(m_cmb, RampCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)3, true, true, false);
			ChipmunkProS.AddCollisionHandler(m_cmb, RampCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)4, true, true, false);
		}
		m_rampBaseAngle = 27.5f;
		CreateEditorTouchArea(prefabC.p_gameObject);
		m_graphElement.m_isRotateable = true;
		m_graphElement.m_isFlippable = true;
		if (m_graphElement.m_flipped)
		{
			TransformS.SetScale(m_cmb.TC, new Vector3(-1f, 1f, 1f));
		}
		int num = 0;
		for (int i = 0; i < PsS.m_units.m_aliveCount; i++)
		{
			if (num > 0)
			{
				break;
			}
			UnitC unitC = PsS.m_units.m_array[PsS.m_units.m_aliveIndices[i]];
			SpeedRamp speedRamp = unitC.m_unit as SpeedRamp;
			if (speedRamp != null && speedRamp != this && !speedRamp.m_isDead)
			{
				num++;
			}
			else if (speedRamp != null && speedRamp == this)
			{
				break;
			}
		}
		if (num == 0)
		{
			m_updater = true;
		}
	}

	public override void Destroy()
	{
		m_isDead = true;
		if (m_updater)
		{
			for (int i = 0; i < PsS.m_units.m_aliveCount; i++)
			{
				UnitC unitC = PsS.m_units.m_array[PsS.m_units.m_aliveIndices[i]];
				SpeedRamp speedRamp = unitC.m_unit as SpeedRamp;
				if (speedRamp != null && speedRamp != this)
				{
					speedRamp.m_updater = true;
					break;
				}
			}
		}
		base.Destroy();
	}

	public void UpdateMaterial()
	{
		m_beltMaterial.mainTextureOffset -= Vector2.up * 0.02f;
		if (m_beltMaterial.mainTextureOffset.y < 0f)
		{
			m_beltMaterial.mainTextureOffset = new Vector2(m_beltMaterial.mainTextureOffset.x, 1f);
		}
	}

	private void RampCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB];
		UnitC unitC = chipmunkBodyC.customComponent as UnitC;
		if (unitC != null && unitC.m_unit != null && (_phase == ucpCollisionPhase.Begin || _phase == ucpCollisionPhase.Persist) && _pair.shapeA == m_beltShape && !m_speedAddedList.Contains(unitC))
		{
			if (_phase == ucpCollisionPhase.Begin && m_soundTriggerTimer <= 0 && unitC.m_unit.m_entity == m_minigame.m_playerTC.p_entity)
			{
				m_soundTriggerTimer = 60;
				SoundS.PlaySingleShot("/Ingame/Units/BoostRamp", new Vector3(_pair.point.x, _pair.point.y, 0f));
			}
			float num = m_rampBaseAngle + 90f;
			float f = (m_cmb.TC.transform.rotation.eulerAngles.z + ((!m_graphElement.m_flipped) ? num : (0f - num))) * ((float)Math.PI / 180f);
			Vector2 normalized = new Vector2(Mathf.Sin(f), 0f - Mathf.Cos(f)).normalized;
			unitC.m_unit.AddSpeed(normalized * 20f, 1000f);
			m_speedAddedList.Add(unitC);
		}
	}

	public override void Update()
	{
		base.Update();
		if (m_updater)
		{
			UpdateMaterial();
		}
		if (m_soundTriggerTimer > 0)
		{
			m_soundTriggerTimer--;
		}
		m_speedAddedList.Clear();
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		Destroy();
	}
}
