using UnityEngine;

public class PsPlanetFlyingNode
{
	private Entity m_entity;

	private PrefabC m_flyer;

	protected TransformC m_tc;

	protected PsPlanet m_planet;

	private TransformC m_rowRot;

	private TransformC m_laneRot;

	private TweenC m_rowTween;

	private TweenC m_laneTween;

	protected PsGameLoop m_sourceLoop;

	protected PsGameLoop m_targetLoop;

	private PsFloatingPlanetNode m_targetNode;

	protected float m_takeOffTime;

	public PsPlanetFlyingNode(PsGameLoop _sourceLoop, PsGameLoop _targetLoop)
	{
		m_sourceLoop = _sourceLoop;
		m_targetLoop = _targetLoop;
		if (m_targetLoop != null)
		{
			m_targetNode = m_targetLoop.m_node as PsFloatingPlanetNode;
		}
		PsPlanetManager.GetCurrentPlanet().m_flyingNodes.Add(this);
		Create();
	}

	public void Create()
	{
		m_planet = PsPlanetManager.GetCurrentPlanet();
		m_entity = EntityManager.AddEntity();
		m_rowRot = TransformS.AddComponent(m_entity, "rowRot");
		TransformS.ParentComponent(m_rowRot, m_planet.m_alienPlanetTC);
		TransformS.SetRotation(m_rowRot, Vector3.right * m_planet.m_nodeRowAngleInterval * (m_sourceLoop.m_nodeId + m_sourceLoop.m_path.m_startNodeId + 1));
		m_laneRot = TransformS.AddComponent(m_entity, "laneRot");
		Vector3 rotation = Vector3.up * (Mathf.Atan2(PsPlanetManager.GetCurrentPlanet().m_planetRadius, PsPlanetNode.GetXPosition(m_sourceLoop)) * 57.29578f - 90f);
		TransformS.ParentComponent(m_laneRot, m_rowRot, Vector3.zero);
		TransformS.SetRotation(m_laneRot, rotation);
		m_tc = TransformS.AddComponent(m_entity, "FlyingNode");
		TransformS.ParentComponent(m_tc, m_laneRot, Vector3.forward * (0f - (PsPlanetManager.GetCurrentPlanet().m_planetRadius + 2.8f)));
		TransformS.SetRotation(m_tc, Vector3.right * -90f);
		GameObject flyerGameObject = GetFlyerGameObject();
		m_flyer = PrefabS.AddComponent(m_tc, Vector3.zero, flyerGameObject, "LevelNode");
		PrefabS.SetCamera(m_flyer.p_gameObject, PsPlanetManager.GetCurrentPlanet().m_planetCamera);
		TakeOff();
		if (m_targetNode != null)
		{
			EntityManager.SetVisibilityOfEntity(m_targetNode.m_domeTC.p_entity, false);
		}
	}

	public virtual GameObject GetFlyerGameObject()
	{
		return ResourceManager.GetGameObject(RESOURCE.FloatingPlatformTournamentPrefab_GameObject);
	}

	public virtual void TakeOff()
	{
		Vector3 endValue = m_tc.transform.localPosition + m_tc.parent.transform.worldToLocalMatrix.MultiplyVector(m_tc.transform.up) * m_planet.m_planetRadius * 0.1f;
		TweenC tweenComponent = TweenS.AddTransformTween(m_tc, TweenedProperty.Position, TweenStyle.ExpoOut, endValue, 1f, 0.45f, true);
		TweenS.AddTweenEndEventListener(tweenComponent, TakeOffEnded);
		TweenC c = TweenS.AddTransformTween(m_tc, TweenedProperty.Scale, TweenStyle.QuadIn, new Vector3(0.85f, 1.75f, 0.85f), 0.25f, 0.35f, true);
		TweenS.SetAdditionalTweenProperties(c, 0, true, TweenStyle.QuadOut);
		m_takeOffTime = -1f;
	}

	public void TakeOffEnded(TweenC _c)
	{
		if (m_targetNode != null)
		{
			PsPlanet currentPlanet = PsPlanetManager.GetCurrentPlanet();
			Vector3 eulerAngles = m_targetNode.m_laneTC.transform.localRotation.eulerAngles;
			eulerAngles.y = eulerAngles.z;
			eulerAngles.z = 0f;
			m_laneTween = TweenS.AddTransformTween(m_laneRot, TweenedProperty.Rotation, TweenStyle.CubicInOut, eulerAngles, 2.5f, 0f, true);
			TweenS.AddTweenEndEventListener(m_laneTween, LaneTweenEnded);
			m_takeOffTime = Main.m_resettingGameTime;
		}
		else
		{
			Destroy();
		}
	}

	public void LaneTweenEnded(TweenC _c)
	{
		m_laneTween = null;
		Destroy();
		EntityManager.SetVisibilityOfEntity(m_targetNode.m_domeTC.p_entity, true);
	}

	public virtual void Update()
	{
		if (m_targetLoop == null)
		{
			return;
		}
		PsPlanet currentPlanet = PsPlanetManager.GetCurrentPlanet();
		if (m_takeOffTime > -1f)
		{
			float num = m_rowRot.transform.rotation.eulerAngles.x - m_rowRot.parent.transform.rotation.eulerAngles.x;
			float num2 = m_targetNode.m_idTC.transform.eulerAngles.x - m_targetNode.m_idTC.parent.transform.eulerAngles.x - currentPlanet.m_alienPlanetTC.transform.eulerAngles.x + currentPlanet.m_floatingNodesTC.transform.localEulerAngles.x + 90f;
			float num3 = num2 - num;
			if (num3 > 180f)
			{
				num3 -= 360f;
			}
			else if (num3 < -180f)
			{
				num3 += 360f;
			}
			float num4 = TweenS.tween(TweenStyle.QuadInOut, Main.m_resettingGameTime - m_takeOffTime, 2f, 0f, 1f);
			if (Main.m_resettingGameTime - m_takeOffTime > 2f)
			{
				num4 = 1f;
			}
			m_rowRot.transform.Rotate(Vector3.right * num3 * num4);
			float num5 = currentPlanet.m_planetRadius * -1.1f;
			float num6 = 0f - m_targetNode.m_domeTC.transform.localPosition.y;
			m_tc.transform.localPosition = new Vector3(0f, 0f, num5 + num6 * num4 - 2.8f * (1f - num4));
			float num7 = m_targetNode.m_domeTC.transform.localEulerAngles.y;
			if (num7 > 180f)
			{
				num7 -= 360f;
			}
			m_tc.transform.localRotation = Quaternion.Euler(Vector3.right * -90f) * Quaternion.Euler(Vector3.up * num7 * num4);
		}
	}

	public void Destroy()
	{
		PsPlanetManager.GetCurrentPlanet().m_flyingNodes.Remove(this);
		EntityManager.RemoveEntity(m_entity);
	}
}
