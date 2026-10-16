using System.Collections.Generic;
using UnityEngine;

public static class PsS
{
	public static DynamicArray<UnitC> m_units;

	public static DynamicArray<CustomObjectC> m_customObjects;

	public static DynamicArray<GroundC> m_grounds;

	public static void Initialize()
	{
		m_units = new DynamicArray<UnitC>();
		m_customObjects = new DynamicArray<CustomObjectC>();
		m_grounds = new DynamicArray<GroundC>();
		Buffs.Initialize();
	}

	public static UnitC AddUnit(Entity _entity, Unit _unitClass)
	{
		UnitC unitC = m_units.AddItem();
		unitC.m_unit = _unitClass;
		EntityManager.AddComponentToEntity(_entity, unitC);
		return unitC;
	}

	public static void RemoveUnit(UnitC _c)
	{
		_c.m_unit.DestroyAllContacts();
		EntityManager.RemoveComponentFromEntity(_c);
		m_units.RemoveItem(_c);
	}

	public static CustomObjectC AddCustomObject(Entity _entity, object _customObject)
	{
		CustomObjectC customObjectC = m_customObjects.AddItem();
		customObjectC.m_customObject = _customObject;
		EntityManager.AddComponentToEntity(_entity, customObjectC);
		return customObjectC;
	}

	public static void RemoveCustomObject(CustomObjectC _c)
	{
		EntityManager.RemoveComponentFromEntity(_c);
		m_customObjects.RemoveItem(_c);
	}

	public static GroundC AddGround(Entity _entity, Ground _groundClass)
	{
		GroundC groundC = m_grounds.AddItem();
		groundC.m_ground = _groundClass;
		EntityManager.AddComponentToEntity(_entity, groundC);
		return groundC;
	}

	public static void RemoveGround(GroundC _c)
	{
		EntityManager.RemoveComponentFromEntity(_c);
		m_grounds.RemoveItem(_c);
	}

	public static void Update()
	{
		int aliveCount = m_units.m_aliveCount;
		for (int i = 0; i < aliveCount; i++)
		{
			UnitC unitC = m_units.m_array[m_units.m_aliveIndices[i]];
			if (unitC.m_active)
			{
				unitC.m_unit.Update();
			}
			else
			{
				unitC.m_unit.InactiveUpdate();
			}
		}
		m_units.Update();
		aliveCount = m_grounds.m_aliveCount;
		for (int j = 0; j < aliveCount; j++)
		{
			GroundC groundC = m_grounds.m_array[m_grounds.m_aliveIndices[j]];
			if (groundC.m_active)
			{
				groundC.m_ground.Update();
			}
		}
		m_grounds.Update();
		m_customObjects.Update();
	}

	public static void ApplyBlastWave(Vector2 _blastCenter, Vector2 _blastForceMinMax, float _blastRadius, float _destroyGroundRadius = 0f, float _damage = 0f)
	{
		int aliveCount = m_units.m_aliveCount;
		for (int i = 0; i < aliveCount; i++)
		{
			UnitC unitC = m_units.m_array[m_units.m_aliveIndices[i]];
			if (!unitC.m_active || !unitC.m_unit.m_reactToBlastWaves)
			{
				continue;
			}
			Unit unit = unitC.m_unit;
			Vector2 zero = Vector2.zero;
			int num = 0;
			for (int j = 0; j < unit.m_assembledEntities.Count; j++)
			{
				Entity e = unit.m_assembledEntities[j];
				List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.ChipmunkBody, e);
				for (int k = 0; k < componentsByEntity.Count; k++)
				{
					ChipmunkBodyC chipmunkBodyC = componentsByEntity[k] as ChipmunkBodyC;
					Vector2 vector = ChipmunkProWrapper.ucpBodyGetPos(chipmunkBodyC.body);
					zero += vector;
					num++;
				}
			}
			if (_damage > 0f && num > 0)
			{
				zero /= (float)num;
				float magnitude = (_blastCenter - zero).magnitude;
				float num2 = 1f - Mathf.Min(magnitude / _blastRadius, 1f);
				unit.Damage(new Damage(DamageType.Weapon, _damage * num2));
			}
		}
		aliveCount = m_units.m_aliveCount;
		for (int l = 0; l < aliveCount; l++)
		{
			UnitC unitC2 = m_units.m_array[m_units.m_aliveIndices[l]];
			if (!unitC2.m_active || !unitC2.m_unit.m_reactToBlastWaves)
			{
				continue;
			}
			Unit unit2 = unitC2.m_unit;
			for (int m = 0; m < unit2.m_assembledEntities.Count; m++)
			{
				Entity e2 = unit2.m_assembledEntities[m];
				List<IComponent> componentsByEntity2 = EntityManager.GetComponentsByEntity(ComponentType.ChipmunkBody, e2);
				for (int n = 0; n < componentsByEntity2.Count; n++)
				{
					ChipmunkBodyC chipmunkBodyC2 = componentsByEntity2[n] as ChipmunkBodyC;
					Vector2 vector2 = ChipmunkProWrapper.ucpBodyGetPos(chipmunkBodyC2.body);
					Vector2 vector3 = -(_blastCenter - vector2).normalized;
					float magnitude2 = (_blastCenter - vector2).magnitude;
					float num3 = 1f - Mathf.Min(magnitude2 / _blastRadius, 1f);
					float num4 = _blastForceMinMax.x + (_blastForceMinMax.y - _blastForceMinMax.x) * num3;
					ChipmunkProWrapper.ucpBodyApplyImpulseAtWorldPoint(chipmunkBodyC2.body, vector3 * num4, vector2);
				}
			}
		}
		List<Entity> entitiesByTag = EntityManager.GetEntitiesByTag("GTAG_DEBRIS");
		foreach (Entity item in entitiesByTag)
		{
			List<IComponent> componentsByEntity3 = EntityManager.GetComponentsByEntity(ComponentType.ChipmunkBody, item);
			for (int num5 = 0; num5 < componentsByEntity3.Count; num5++)
			{
				ChipmunkBodyC chipmunkBodyC3 = componentsByEntity3[num5] as ChipmunkBodyC;
				Vector2 vector4 = ChipmunkProWrapper.ucpBodyGetPos(chipmunkBodyC3.body);
				Vector2 vector5 = -(_blastCenter - vector4).normalized;
				float magnitude3 = (_blastCenter - vector4).magnitude;
				float num6 = 1f - Mathf.Min(magnitude3 / _blastRadius, 1f);
				float num7 = _blastForceMinMax.x + (_blastForceMinMax.y - _blastForceMinMax.x) * num6;
				ChipmunkProWrapper.ucpBodyApplyImpulseAtWorldPoint(chipmunkBodyC3.body, vector5 * num7, vector4);
			}
		}
		if (_destroyGroundRadius > 0f)
		{
			AutoGeometryBrush brush = new AutoGeometryBrush(Mathf.Max(1.5f, _destroyGroundRadius / 16f), false, 1f, 0.66f);
			foreach (AutoGeometryLayer layer in AutoGeometryManager.m_layers)
			{
				if (layer.m_groundC.m_ground.m_destructible)
				{
					layer.PaintWithBrush(brush, _blastCenter, AGDrawMode.SUB, false, ref layer.m_bytes);
				}
			}
		}
		float magnitude4 = (_blastCenter - (Vector2)CameraS.m_mainCamera.transform.position).magnitude;
		float num8 = 1f - ToolBox.getPositionBetween(magnitude4, 100f, _blastRadius * 1.5f);
		CameraS.ShakeMainCamera(num8 * 20f, 0.3f);
	}
}
