using System;
using UnityEngine;

public class SmallGear : Unit
{
	private ChipmunkBodyC m_cmb;

	private float m_angle;

	private SoundC m_gearSound;

	public SmallGear(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.SmallGearPrefab_GameObject);
		TransformC transformC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(transformC, _graphElement.m_position, _graphElement.m_rotation);
		PrefabC prefabC = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 0f) + GetZBufferBias(), gameObject);
		if (!m_minigame.m_editing)
		{
			m_gearSound = SoundS.AddCombineSoundComponent(transformC, "BigGearLoopSound", "/InGame/Units/BigGearTurning", 2f);
		}
		else
		{
			m_gearSound = null;
		}
		ucpPolyShape[] shapes = ChipmunkProS.GeneratePolyShapesFromChildren(gameObject.transform.Find("Collision1").gameObject, Vector2.zero, 1f, 0.25f, 0.8f, (ucpCollisionType)4, 257u, false, m_graphElement.m_flipped);
		m_cmb = ChipmunkProS.AddKinematicBody(transformC, shapes, m_unitC);
		CreateEditorTouchArea(prefabC.p_gameObject);
		m_graphElement.m_isRotateable = true;
		m_graphElement.m_isFlippable = true;
		if (m_graphElement.m_flipped)
		{
			TransformS.SetScale(m_cmb.TC, new Vector3(-1f, 1f, 1f));
		}
	}

	public override void Update()
	{
		base.Update();
		if (!m_minigame.m_editing && m_minigame.m_gameStarted)
		{
			m_angle = ToolBox.getCappedAngle(m_angle - 0.3f * Main.m_timeScale * (float)((!m_graphElement.m_flipped) ? 1 : (-1)) * 1.6f);
			ChipmunkProWrapper.ucpBodySetAngle(m_cmb.body, (m_graphElement.m_rotation.z + m_angle) * ((float)Math.PI / 180f));
			SoundS.PlaySound(m_gearSound);
		}
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		Destroy();
	}
}
