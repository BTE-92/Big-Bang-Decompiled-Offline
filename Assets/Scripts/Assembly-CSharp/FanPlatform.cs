using UnityEngine;

public class FanPlatform : Unit
{
	private ChipmunkBodyC m_cmb;

	private PrefabC m_fanRotorPrefab;

	private Vector2 m_fanAreaSize;

	private SoundC m_fanLoopSound;

	private Transform m_particleFx;

	private cpBB m_bb;

	public FanPlatform(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.FanPlatformPrefab_GameObject);
		TransformC transformC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(transformC, _graphElement.m_position, _graphElement.m_rotation);
		PrefabC prefabC = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 10f) + GetZBufferBias(), gameObject.transform.Find("fan_platform").gameObject);
		m_fanRotorPrefab = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 10f) + GetZBufferBias(), gameObject.transform.Find("fan_blades").gameObject);
		m_particleFx = prefabC.p_gameObject.transform.Find("effect_wind");
		ucpPolyShape shape = ChipmunkProS.GeneratePolyShapeFromGameObject(gameObject.transform.Find("collision1").gameObject, Vector2.zero, 1f, 0.25f, 0.9f, (ucpCollisionType)4, 257u);
		m_cmb = ChipmunkProS.AddStaticBody(transformC, shape, m_unitC);
		m_fanAreaSize = new Vector2(140f, 250f);
		ucpPolyShape shape2 = new ucpPolyShape(m_fanAreaSize.x, m_fanAreaSize.y, new Vector2(0f, m_fanAreaSize.y * 0.5f), 16777216u, 1f, 0.1f, 0.1f, (ucpCollisionType)4, true);
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.AddStaticBody(transformC, shape2, m_unitC);
		chipmunkBodyC.m_identifier = 1;
		m_bb = CameraS.CenterBB(_graphElement.m_position + transformC.transform.up * 125f, new cpBB(0f, 0f, 500f, 500f));
		if (!m_minigame.m_editing)
		{
			m_fanLoopSound = SoundS.AddCombineSoundComponent(transformC, "FanLoopSound", "/InGame/Units/FanNoiseLoop");
			ChipmunkProS.AddCollisionHandler(chipmunkBodyC, FanCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)3, true, true, false);
			ChipmunkProS.AddCollisionHandler(chipmunkBodyC, FanCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)4, true, true, false);
		}
		else
		{
			m_fanLoopSound = null;
		}
		CreateEditorTouchArea(prefabC.p_gameObject);
		m_graphElement.m_isRotateable = true;
	}

	private void FanCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB];
		UnitC unitC = chipmunkBodyC.customComponent as UnitC;
		if (unitC != null && unitC.m_unit != null)
		{
			Vector2 vector = ChipmunkProWrapper.ucpBodyGetPos(chipmunkBodyC.body);
			Vector2 vector2 = _pair.point - vector;
			if (vector2.magnitude > 50f)
			{
				float y = m_cmb.TC.transform.InverseTransformPoint(_pair.point).y;
				float num = (1f - ToolBox.getPositionBetween(y, 0f, m_fanAreaSize.y)) * Main.m_timeScale;
				Vector2 vector3 = m_cmb.TC.transform.up.normalized * 40f * num / chipmunkBodyC.shapes.Count;
				ChipmunkProWrapper.ucpBodyApplyImpulseAtWorldPoint(chipmunkBodyC.body, vector3 * chipmunkBodyC.m_mass * 0.5f, vector + vector2);
				ChipmunkProWrapper.ucpBodyApplyImpulseAtWorldPoint(chipmunkBodyC.body, vector3 * chipmunkBodyC.m_mass * 0.5f, vector);
			}
			else
			{
				float y2 = m_cmb.TC.transform.InverseTransformPoint(chipmunkBodyC.TC.transform.position).y;
				float num2 = (1f - ToolBox.getPositionBetween(y2, 0f, m_fanAreaSize.y)) * Main.m_timeScale;
				Vector2 vector4 = m_cmb.TC.transform.up.normalized * 40f * num2 / chipmunkBodyC.shapes.Count;
				ChipmunkProWrapper.ucpBodyApplyImpulseAtWorldPoint(chipmunkBodyC.body, vector4 * chipmunkBodyC.m_mass, vector);
			}
		}
	}

	public override void Update()
	{
		base.Update();
		if (m_fanRotorPrefab.p_gameObject.GetComponent<Renderer>().isVisible)
		{
			m_fanRotorPrefab.p_gameObject.transform.Rotate(new Vector3(0f, 15f, 0f), Space.Self);
		}
		if (ChipmunkProWrapper.ucpBBIntersects(CameraS.m_mainCameraFrame, m_bb))
		{
			m_particleFx.gameObject.SetActive(true);
		}
		else
		{
			m_particleFx.gameObject.SetActive(false);
		}
		if (!m_minigame.m_editing && m_minigame.m_gameStarted)
		{
			SoundS.PlaySound(m_fanLoopSound);
		}
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		Destroy();
	}
}
