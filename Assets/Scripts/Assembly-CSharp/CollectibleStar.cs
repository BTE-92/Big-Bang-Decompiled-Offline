using UnityEngine;

public class CollectibleStar : Unit
{
	private const int RADIUS = 30;

	public ChipmunkBodyC m_starCmb;

	private PrefabC m_prefabC;

	public Vector3 m_pos;

	public TransformC m_tc;

	private Material m_materialInstance;

	public CollectibleStar(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_graphElement.m_name = "CollectibleStar";
		m_graphElement.m_isCopyable = false;
		m_graphElement.m_isRemovable = false;
		m_graphElement.m_isRotateable = false;
		string resourceIdentifier = "CollectibleStarPrefab_GameObject";
		if (PsState.m_activeGameLoop.m_context == PsMinigameContext.Challenge)
		{
			resourceIdentifier = "CollectibleGemPrefab_GameObject";
		}
		else if (PsState.m_activeGameLoop.m_context == PsMinigameContext.Fresh)
		{
			resourceIdentifier = "CollectibleKeyPrefab_GameObject";
		}
		GameObject gameObject = ResourceManager.GetGameObject(resourceIdentifier);
		m_tc = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(m_tc, _graphElement.m_position, _graphElement.m_rotation);
		m_pos = _graphElement.m_position;
		float num = 30f;
		ucpShape shape = new ucpCircleShape(num, Vector2.zero, 16777216u, 50f, 0.5f, 0.9f, (ucpCollisionType)4, true);
		m_starCmb = ChipmunkProS.AddKinematicBody(m_tc, shape);
		m_prefabC = PrefabS.AddComponent(m_tc, Vector3.zero, gameObject);
		if (PsState.m_activeGameLoop is PsGameLoopEditorPuzzle && PsState.m_activeMinigame.m_editing)
		{
			EffectCollectible componentInChildren = m_prefabC.p_gameObject.transform.GetComponentInChildren<EffectCollectible>();
			if (componentInChildren != null)
			{
				componentInChildren.hideFx = true;
			}
			m_materialInstance = m_prefabC.p_gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>()
				.material;
			m_materialInstance.shader = Shader.Find("WOE/Units/Greyscale");
		}
		else
		{
			m_materialInstance = null;
		}
		if (!m_minigame.m_editing)
		{
			ChipmunkProS.AddCollisionHandler(m_starCmb, StarCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)3, true, false, false);
			CreateCamTarget();
		}
		CreateEditorTouchArea(num, num);
		m_reactToBlastWaves = false;
	}

	public void CreateCamTarget()
	{
		CameraTargetC cameraTargetC = CameraS.AddTargetComponent(m_starCmb.TC, 150f, 150f, new Vector2(0f, 0f));
		cameraTargetC.activeRadius = 400f;
	}

	public override void CreateEditorTouchArea(float _width, float _height, TransformC _parentTC = null)
	{
		if (m_minigame.m_editing)
		{
			CreateGraphElementTouchArea(_width, _parentTC);
		}
	}

	public void Collect()
	{
		Entity entity = EntityManager.AddEntity(new string[1] { "GTAG_AUTODESTROY" });
		TransformC parentTC = TransformS.AddComponent(entity, m_tc.transform.position);
		PrefabC c = PrefabS.AddComponent(parentTC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.PickUpStarSparks_GameObject), string.Empty, false);
		PrefabS.SetCamera(c, CameraS.m_mainCamera);
		TimerC timerC = TimerS.AddComponent(entity, "RemoveTimer", 1f, 0f, true);
		PsState.m_activeMinigame.m_collectedStars++;
		Vector2 vector = m_tc.transform.position;
		if (PsState.m_activeGameLoop.m_context == PsMinigameContext.Challenge)
		{
			SoundS.PlaySingleShotWithParameter("/Ingame/Units/GotDiamond", new Vector3(vector.x, vector.y, 0f), "CollectableNumber", PsState.m_activeMinigame.m_collectedStars);
		}
		else if (PsState.m_activeGameLoop.m_context == PsMinigameContext.Fresh)
		{
			SoundS.PlaySingleShotWithParameter("/Ingame/Units/GotKey", new Vector3(vector.x, vector.y, 0f), "CollectableNumber", PsState.m_activeMinigame.m_collectedStars);
		}
		else
		{
			SoundS.PlaySingleShotWithParameter("/Ingame/Units/GotCollectable", new Vector3(vector.x, vector.y, 0f), "CollectableNumber", PsState.m_activeMinigame.m_collectedStars);
		}
		Destroy();
		m_isDead = true;
	}

	public override void Destroy()
	{
		if (m_materialInstance != null)
		{
			Object.Destroy(m_materialInstance);
			m_materialInstance = null;
		}
		base.Destroy();
	}

	private void StarCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		if (!m_isDead)
		{
			Collect();
		}
	}
}
