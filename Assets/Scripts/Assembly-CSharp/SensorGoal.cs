using UnityEngine;

public class SensorGoal : Unit
{
	protected const int RADIUS = 45;

	protected ChipmunkBodyC m_goalCmb;

	protected PrefabC m_prefabC;

	protected GameObject m_mainPrefab;

	private TweenC m_tween;

	private Animator m_animator;

	private TransformC m_mainTransform;

	private ucpShape m_colShape;

	private int m_colShapeRemovedTimer;

	private GameObject m_confettiLocator;

	private uint m_playerGroup;

	private CameraTargetC m_camTarget;

	private ProjectorC m_projection;

	protected TransformC m_tc;

	public SensorGoal(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_graphElement.m_name = "Goal";
		m_graphElement.m_isCopyable = false;
		m_graphElement.m_isRemovable = true;
		m_graphElement.m_isRotateable = false;
		m_tc = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(m_tc, _graphElement.m_position + new Vector3(0f, 0f, 50f), _graphElement.m_rotation);
		m_mainTransform = m_tc;
		float num = 45f;
		m_goalCmb = ChipmunkProS.AddStaticBody(_shape: m_colShape = CreateBodyShape(), _transformComponent: m_tc);
		SetGoalPrefab();
		CreateEditorTouchArea(num, num);
	}

	public virtual ucpShape CreateBodyShape()
	{
		return new ucpCircleShape(45f, Vector2.zero, 17895696u, 500f, 0.5f, 0.9f, (ucpCollisionType)6, true);
	}

	public virtual void SetGoalPrefab()
	{
		if (!m_minigame.m_editing)
		{
			ChipmunkProS.AddCollisionHandler(m_goalCmb, GoalCollisionHandler, (ucpCollisionType)6, (ucpCollisionType)3, true, false, false);
			return;
		}
		Vector2[] circle = DebugDraw.GetCircle(45f, 32, Vector2.zero);
		m_prefabC = PrefabS.CreatePathPrefabComponentFromVectorArray(m_tc, Vector3.zero, circle, 4f, Color.white, ResourceManager.GetMaterial("Framework/Line8Mat"), CameraS.m_mainCamera, Position.Center, true);
		m_minigame.SetGoalNode(this);
	}

	public override void CreateEditorTouchArea(float _width, float _height, TransformC _parentTC = null)
	{
		if (m_minigame.m_editing)
		{
			CreateGraphElementTouchArea(_width, _parentTC);
		}
	}

	protected void GoalCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		if (!m_minigame.m_playerBeamingOut && !m_minigame.m_gameEnded)
		{
			WinMiniGame();
			SoundS.PlaySingleShot("/InGame/GameEnd", Vector3.zero);
			m_playerGroup = ChipmunkProWrapper.ucpShapeGetFilter(ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB].shapes[0].shapePtr).group;
			ChipmunkProS.RemoveCollisionHandler(m_goalCmb, GoalCollisionHandler);
		}
	}

	protected virtual void WinMiniGame()
	{
		PsState.m_activeGameLoop.WinMinigame();
	}

	public override void Update()
	{
		if (!m_minigame.m_editing && m_minigame.m_gameEnded)
		{
			m_colShapeRemovedTimer--;
			if (m_colShapeRemovedTimer == 0)
			{
				ChipmunkProWrapper.ucpShapeSetGroup(m_colShape.shapePtr, m_playerGroup);
			}
		}
	}
}
