using UnityEngine;

public class Goal : Unit
{
	private const int RADIUS = 45;

	private ChipmunkBodyC m_goalCmb;

	private PrefabC m_prefabC;

	private TweenC m_tween;

	private Animator m_animator;

	private TransformC m_mainTransform;

	private ucpShape m_colShape;

	private int m_colShapeRemovedTimer;

	private GameObject m_confettiLocator;

	private uint m_playerGroup;

	private CameraTargetC m_camTarget;

	private ProjectorC m_projection;

	private Material[] m_materialInstances;

	public Goal(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_graphElement.m_name = "Goal";
		m_graphElement.m_isCopyable = false;
		m_graphElement.m_isRemovable = false;
		m_graphElement.m_isRotateable = false;
		float mass = 500f;
		GameObject gameObject = ResourceManager.GetGameObject("GoalBotPrefab_GameObject");
		TransformC transformC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(transformC, _graphElement.m_position + new Vector3(0f, 0f, 50f), _graphElement.m_rotation);
		m_mainTransform = transformC;
		float num = 45f;
		m_goalCmb = ChipmunkProS.AddDynamicBody(transformC, m_colShape = new ucpCircleShape(num, Vector2.zero, 17895696u, mass, 0.5f, 0.9f, (ucpCollisionType)4), m_unitC);
		ChipmunkProS.SetBodyGravity(m_goalCmb, Vector2.zero);
		ChipmunkProS.SetBodyLinearDamp(m_goalCmb, new Vector2(0.99f, 0.99f));
		m_goalCmb.m_identifier = 2;
		m_prefabC = PrefabS.AddComponent(transformC, Vector3.zero, gameObject);
		m_prefabC.p_gameObject.transform.Rotate(new Vector3(0f, 180f, 0f));
		if (PsState.m_activeGameLoop is PsGameLoopEditorPuzzle && PsState.m_activeMinigame.m_editing)
		{
			Renderer[] componentsInChildren = m_prefabC.p_gameObject.transform.GetComponentsInChildren<Renderer>();
			m_materialInstances = new Material[componentsInChildren.Length];
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				m_materialInstances[i] = componentsInChildren[i].material;
				m_materialInstances[i].shader = Shader.Find("WOE/Units/Greyscale");
			}
		}
		else
		{
			m_materialInstances = null;
		}
		m_confettiLocator = m_prefabC.p_gameObject.transform.FindChild("GoalBotBody/ConfettiLocator").gameObject;
		m_animator = m_prefabC.p_gameObject.GetComponent("Animator") as Animator;
		if (!m_minigame.m_editing)
		{
			ChipmunkProS.AddCollisionHandler(m_goalCmb, GoalCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)3, true, false, false);
			CreateCamTarget();
		}
		else
		{
			m_minigame.SetGoalNode(this);
		}
		m_tween = TweenS.AddTween(m_entity, TweenStyle.QuadInOut, -10f, 10f, 2f, 0f);
		TweenS.SetAdditionalTweenProperties(m_tween, -1, true, TweenStyle.QuadInOut);
		m_colShapeRemovedTimer = 5;
		m_projection = ProjectorS.AddComponent(m_mainTransform, ResourceManager.GetMaterial(RESOURCE.ShadowMaterial_Material), 256, new Vector3(0f, -20f));
		CreateEditorTouchArea(num, num);
	}

	public override void CreateEditorTouchArea(float _width, float _height, TransformC _parentTC = null)
	{
		if (m_minigame.m_editing)
		{
			CreateGraphElementTouchArea(_width, _parentTC);
		}
	}

	public void CreateCamTarget()
	{
		m_camTarget = CameraS.AddTargetComponent(m_mainTransform, 200f, 200f, new Vector2(0f, 0f));
		m_camTarget.activeRadius = 500f;
	}

	private void GoalCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		if (!m_minigame.m_playerBeamingOut && !m_minigame.m_playerReachedGoal && !m_minigame.m_gameEnded && !m_minigame.m_playerUnit.m_isDead)
		{
			m_minigame.m_playerReachedGoal = true;
			m_minigame.m_playerReachedGoalCount++;
			m_camTarget.activeRadius = float.MaxValue;
			CameraS.SetTargetBB(m_camTarget, 400f, 400f);
			Vehicle vehicle = m_minigame.m_playerUnit as Vehicle;
			if (vehicle != null)
			{
				vehicle.ReachedGoal();
			}
			m_minigame.m_playerUnit.SetAsTeleporting(15, m_mainTransform.transform, m_mainTransform.transform, true, false, true);
			m_animator.SetTrigger("GameEnd");
			PrefabC prefabC = PrefabS.AddComponent(m_mainTransform, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.ConfettiBurst_GameObject));
			prefabC.p_gameObject.transform.position = m_confettiLocator.transform.position;
			prefabC.p_gameObject.transform.Rotate(new Vector3(-90f, 0f, 0f), Space.Self);
			PsState.m_activeGameLoop.WinMinigame();
			SoundS.PlaySingleShot("/InGame/GameEnd", Vector3.zero);
			ChipmunkProS.RemoveCollisionHandler(m_goalCmb, GoalCollisionHandler);
		}
	}

	public override void Update()
	{
		m_prefabC.p_gameObject.transform.localPosition = new Vector3(0f, m_tween.currentValue.x, 0f);
		if (m_minigame.m_playerTC != null)
		{
			m_mainTransform.transform.LookAt(m_minigame.m_playerTC.transform.position + new Vector3(0f, 0f, -200f));
			return;
		}
		LevelPlayerNode levelPlayerNode = LevelManager.m_currentLevel.m_currentLayer.GetElement("Player") as LevelPlayerNode;
		if (levelPlayerNode != null)
		{
			m_mainTransform.transform.LookAt(levelPlayerNode.m_position + new Vector3(0f, 0f, -200f));
		}
	}

	public override void Destroy()
	{
		if (m_materialInstances != null)
		{
			for (int i = 0; i < m_materialInstances.Length; i++)
			{
				Object.Destroy(m_materialInstances[i]);
				m_materialInstances[i] = null;
			}
			m_materialInstances = null;
		}
		base.Destroy();
	}
}
