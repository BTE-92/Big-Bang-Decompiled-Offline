using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformBase : Unit
{
	private static CombineSound m_combSound;

	protected GameObject m_mainPrefab;

	protected ChipmunkBodyC m_cmb;

	protected PrefabC m_platformPrefabC;

	protected TransformC m_platformTC;

	protected TransformC m_cylinderTC;

	protected TransformC m_pipeTC;

	protected TransformC m_gearTC;

	protected TransformC m_endTC;

	protected TransformC m_startTC;

	protected Entity m_ghostEntity;

	protected TransformC m_ghostCylinderTC;

	protected TransformC m_ghostPlatformTC;

	protected TransformC m_ghostGearTC;

	protected GraphNode m_childNode;

	protected Vector2 m_startPos;

	protected Vector2 m_moveDir;

	protected float m_moveMagnitude;

	protected float m_duration;

	protected float m_rotMultiplier;

	protected float m_currentTime;

	protected float m_currentValue;

	protected SoundC m_moveSound;

	public MovingPlatformBase(GraphElement _graphElement, string _mainPrefabName)
		: base(_graphElement, UnitType.Basic)
	{
		m_mainPrefab = ResourceManager.GetGameObject(_mainPrefabName + "_GameObject");
		m_platformTC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(m_platformTC, _graphElement.m_position, _graphElement.m_rotation);
		m_cylinderTC = TransformS.AddComponent(m_entity, "Cylinder");
		TransformS.SetTransform(m_cylinderTC, _graphElement.m_position, _graphElement.m_rotation);
		m_gearTC = TransformS.AddComponent(m_entity, "Gear");
		TransformS.SetTransform(m_gearTC, _graphElement.m_position, _graphElement.m_rotation);
		m_pipeTC = TransformS.AddComponent(m_entity, "Pipe");
		TransformS.SetTransform(m_pipeTC, _graphElement.m_position, _graphElement.m_rotation);
		m_startTC = TransformS.AddComponent(m_entity, "StartNode");
		TransformS.SetTransform(m_startTC, _graphElement.m_position, _graphElement.m_rotation);
		m_endTC = TransformS.AddComponent(m_entity, "EndNode");
		TransformS.SetTransform(m_endTC, _graphElement.m_position, _graphElement.m_rotation);
		GraphNode graphNode = m_graphElement as GraphNode;
		if (graphNode.m_childElements.Count == 0)
		{
			graphNode.AddElement(CreateChild());
		}
		else
		{
			m_childNode = graphNode.m_childElements[0] as GraphNode;
		}
		m_moveDir = ((Vector2)m_childNode.m_position - (Vector2)m_graphElement.m_position).normalized;
		m_moveMagnitude = ((Vector2)m_childNode.m_position - (Vector2)m_graphElement.m_position).magnitude;
		m_duration = 1f / 140f * m_moveMagnitude;
		CreateObject();
		TransformS.SetGlobalRotation(m_pipeTC, new Vector3(0f, 0f, Mathf.Atan2(m_moveDir.y, m_moveDir.x) * 57.29578f));
		TransformS.SetGlobalRotation(m_cylinderTC, m_pipeTC.transform.rotation.eulerAngles);
		TransformS.SetGlobalRotation(m_startTC, m_pipeTC.transform.rotation.eulerAngles);
		TransformS.SetGlobalRotation(m_endTC, m_pipeTC.transform.rotation.eulerAngles);
		TransformS.SetGlobalPosition(m_cylinderTC, m_graphElement.m_position + Vector3.forward * 60f);
		TransformS.SetGlobalPosition(m_startTC, m_graphElement.m_position + Vector3.forward * 60f + (Vector3)m_moveDir * -50f);
		TransformS.SetGlobalPosition(m_endTC, m_graphElement.m_position + Vector3.forward * 60f + (Vector3)m_moveDir * (m_moveMagnitude + 50f));
		TransformS.SetGlobalPosition(m_gearTC, m_graphElement.m_position + Vector3.forward * 50f);
		TransformS.SetGlobalPosition(m_pipeTC, (m_graphElement.m_position + m_childNode.m_position) / 2f + Vector3.forward * 60f);
		m_rotMultiplier = 1f;
		m_currentTime = -1f;
		m_currentValue = 0f;
		CreateEditorTouchArea(m_platformPrefabC.p_gameObject);
		m_startPos = m_graphElement.m_position;
		if (!m_minigame.m_editing)
		{
			CreateColliders();
			m_moveSound = SoundS.AddCombineSoundComponent(m_platformTC, "ElevatorMoveLoopSound", "/InGame/Units/MachineMovementLoop", 2f);
			m_combSound = SoundS.GetCombineSoundWithKey("ElevatorMoveLoopSound");
			SoundS.PlaySound(m_moveSound);
		}
		else
		{
			m_combSound = null;
			m_moveSound = null;
		}
	}

	public virtual GraphNode CreateChild()
	{
		m_childNode = new GraphNode(GraphNodeType.Child, typeof(TouchableChildNode), "ChildNode", (Vector2)m_graphElement.m_position + Vector2.up * 100f, Vector3.zero, Vector3.one);
		m_childNode.m_isRemovable = false;
		m_childNode.m_isCopyable = false;
		m_childNode.m_minDistanceFromParent = 100f;
		m_childNode.m_maxDistanceFromParent = 1000f;
		return m_childNode;
	}

	public virtual void CreateColliders()
	{
		ucpPolyShape ucpPolyShape2 = ChipmunkProS.GeneratePolyShapeFromGameObject(m_mainPrefab.transform.Find("Collision1").gameObject, Vector2.zero, 1f, 0.25f, 0.9f, (ucpCollisionType)4, 257u);
		ucpPolyShape2.group = GetGroup();
		m_cmb = ChipmunkProS.AddKinematicBody(m_platformTC, ucpPolyShape2, m_unitC);
	}

	public virtual void CreateObject()
	{
		GameObject gameObject = m_mainPrefab.transform.Find("MovingPlatform").gameObject;
		m_platformPrefabC = PrefabS.AddComponent(m_platformTC, new Vector3(0f, 0f, 10f) + GetZBufferBias(), gameObject);
		GameObject gameObject2 = m_mainPrefab.transform.Find("RailCylinder").gameObject;
		PrefabS.AddComponent(m_cylinderTC, new Vector3(0f, 0f, 60f) + GetZBufferBias(), gameObject2);
		GameObject gameObject3 = m_mainPrefab.transform.Find("GearWheel").gameObject;
		PrefabS.AddComponent(m_gearTC, new Vector3(0f, 0f, 50f) + GetZBufferBias(), gameObject3);
		GameObject gameObject4 = m_mainPrefab.transform.Find("Rail").gameObject;
		PrefabS.AddComponent(m_pipeTC, new Vector3(0f, 0f, 60f) + GetZBufferBias(), gameObject4);
		m_pipeTC.transform.localScale = new Vector3((m_moveMagnitude + 100f) / 100f, 1f, 1f);
		GameObject gameObject5 = m_mainPrefab.transform.Find("RailNode").gameObject;
		PrefabS.AddComponent(m_startTC, new Vector3(0f, 0f, 60f) + GetZBufferBias(), gameObject5);
		m_startTC.transform.localScale = new Vector3(-1f, 1f, 1f);
		GameObject gameObject6 = m_mainPrefab.transform.Find("RailNode").gameObject;
		PrefabS.AddComponent(m_endTC, new Vector3(0f, 0f, 60f) + GetZBufferBias(), gameObject6);
	}

	public override void Update()
	{
		base.Update();
		EditUpdate();
		TransformS.SetGlobalPosition(m_cylinderTC, m_platformTC.transform.position + Vector3.forward * 60f);
		TransformS.SetGlobalPosition(m_gearTC, m_platformTC.transform.position + Vector3.forward * 50f);
		if (m_moveSound != null && m_cmb != null && m_combSound != null && !m_combSound.isOutOfRange && !m_combSound.isPaused && m_combSound.nearestSoundC == m_moveSound)
		{
			float magnitude = ChipmunkProWrapper.ucpBodyGetVel(m_cmb.body).magnitude;
			float positionBetween = ToolBox.getPositionBetween(magnitude, 0f, 80f);
			SoundS.SetSoundParameter(m_moveSound, "Speed", positionBetween);
		}
	}

	public virtual void EditUpdate()
	{
		if (m_minigame.m_editing)
		{
			SyncPositionToGraphElementPosition();
			m_moveDir = ((Vector2)m_childNode.m_position - (Vector2)m_graphElement.m_position).normalized;
			m_moveMagnitude = ((Vector2)m_childNode.m_position - (Vector2)m_graphElement.m_position).magnitude;
			m_duration = 1f / 140f * m_moveMagnitude;
			m_pipeTC.transform.localScale = new Vector3((m_moveMagnitude + 100f) / 100f, 1f, 1f);
			TransformS.SetGlobalRotation(m_pipeTC, new Vector3(0f, 0f, Mathf.Atan2(m_moveDir.y, m_moveDir.x) * 57.29578f));
			TransformS.SetGlobalRotation(m_cylinderTC, m_pipeTC.transform.rotation.eulerAngles);
			TransformS.SetGlobalRotation(m_startTC, m_pipeTC.transform.rotation.eulerAngles);
			TransformS.SetGlobalRotation(m_endTC, m_pipeTC.transform.rotation.eulerAngles);
			TransformS.SetGlobalPosition(m_pipeTC, (m_graphElement.m_position + m_childNode.m_position) / 2f + Vector3.forward * 60f);
			TransformS.SetGlobalPosition(m_startTC, m_graphElement.m_position + Vector3.forward * 60f + (Vector3)m_moveDir * -50f);
			TransformS.SetGlobalPosition(m_endTC, m_graphElement.m_position + Vector3.forward * 60f + (Vector3)m_moveDir * (m_moveMagnitude + 50f));
			if (m_ghostEntity == null)
			{
				CreateEndGhost();
				return;
			}
			UpdateEndGhost();
			TransformS.SetGlobalRotation(m_ghostCylinderTC, m_pipeTC.transform.rotation.eulerAngles);
		}
	}

	public void DestroyEndGhost()
	{
		if (m_ghostEntity != null)
		{
			EntityManager.RemoveEntity(m_ghostEntity);
			m_ghostEntity = null;
		}
	}

	public void UpdateEndGhost()
	{
		if (m_ghostEntity != null)
		{
			TransformS.SetGlobalTransform(m_ghostPlatformTC, m_childNode.m_position, m_graphElement.m_TC.transform.rotation.eulerAngles);
			TransformS.SetGlobalPosition(m_ghostCylinderTC, m_ghostPlatformTC.transform.position + Vector3.forward * 60f);
			TransformS.SetGlobalTransform(m_ghostGearTC, m_ghostPlatformTC.transform.position + Vector3.forward * 50f, m_gearTC.transform.rotation.eulerAngles);
		}
	}

	public void CreateEndGhost()
	{
		if (m_ghostEntity != null)
		{
			return;
		}
		m_ghostEntity = EntityManager.AddEntity();
		m_ghostPlatformTC = TransformS.AddComponent(m_ghostEntity, "GhostPlatform");
		m_ghostCylinderTC = TransformS.AddComponent(m_ghostEntity, "GhostCylinder");
		m_ghostGearTC = TransformS.AddComponent(m_ghostEntity, "GhostGear");
		CreateGhostObject();
		Material material = ResourceManager.GetMaterial(RESOURCE.UnitGhostMaterial_Material);
		List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.Transform, m_ghostEntity);
		foreach (TransformC item in componentsByEntity)
		{
			GameObject gameObject = item.transform.gameObject;
			Component[] componentsInChildren = gameObject.GetComponentsInChildren(typeof(Renderer));
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Renderer renderer = componentsInChildren[i] as Renderer;
				renderer.material = material;
			}
		}
		TransformS.SetGlobalTransform(m_ghostPlatformTC, m_childNode.m_position, m_graphElement.m_rotation);
		TransformS.SetGlobalPosition(m_ghostCylinderTC, m_ghostPlatformTC.transform.position + Vector3.forward * 60f);
		TransformS.SetGlobalTransform(m_ghostGearTC, m_ghostPlatformTC.transform.position + Vector3.forward * 50f, m_gearTC.transform.rotation.eulerAngles);
	}

	public virtual void CreateGhostObject()
	{
		PrefabS.AddComponent(m_ghostPlatformTC, Vector3.forward * 10f, m_mainPrefab.transform.Find("MovingPlatform").gameObject);
		PrefabS.AddComponent(m_ghostCylinderTC, m_ghostPlatformTC.transform.position + Vector3.forward * 60f, m_mainPrefab.transform.Find("RailCylinder").gameObject);
		PrefabS.AddComponent(m_ghostGearTC, m_ghostPlatformTC.transform.position + Vector3.forward * 50f, m_mainPrefab.transform.Find("GearWheel").gameObject);
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		Destroy();
	}

	public override void Destroy()
	{
		base.Destroy();
		DestroyEndGhost();
	}
}
