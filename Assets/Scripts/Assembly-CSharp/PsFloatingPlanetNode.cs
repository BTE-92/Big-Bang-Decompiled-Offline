using UnityEngine;

public class PsFloatingPlanetNode : PsPlanetNode
{
	public TransformC m_rootTC;

	public TransformC m_idTC;

	public TransformC m_laneTC;

	public TransformC m_domeTC;

	protected TransformC m_domeUITC;

	protected TransformC m_domeNumberTC;

	protected string m_prefabName;

	protected Projector m_shadowProjector;

	protected TweenC m_shadowTween;

	protected TweenC m_relationalTween;

	private bool m_destroyStarted;

	public bool m_newNode;

	public SpriteC bgSpriteC;

	public PsFloatingPlanetNode(PsTimedEventLoop _loop, bool _tutorial = false)
		: base(_loop, _tutorial)
	{
	}

	public override void CreateBase()
	{
		Entity entity = EntityManager.AddEntity(new string[2] { "PlanetUI", "Tutorial" });
		float x = (float)m_loop.m_levelNumber * m_loop.m_planet.m_nodeRowAngleInterval;
		m_idTC = TransformS.AddComponent(entity, "FloaterIdTC");
		TransformS.ParentComponent(m_idTC, m_loop.m_planet.m_floatingNodesTC);
		TransformS.SetRotation(m_idTC, new Vector3(x, 0f, 0f));
		m_laneTC = TransformS.AddComponent(entity, "FloaterLaneTC");
		TransformS.ParentComponent(m_laneTC, m_idTC);
		TransformS.SetRotation(m_laneTC, new Vector3(0f, 0f, 10f + (float)m_loop.m_path.m_lane * -7.5f));
		Vector3 pos = new Vector3(0f, 1.1f * m_loop.m_planet.m_planetRadius, 0f);
		m_rootTC = TransformS.AddComponent(entity, "FloaterRootTC", pos);
		TransformS.ParentComponent(m_rootTC, m_laneTC);
		m_rootTC.transform.localEulerAngles = Vector3.zero;
		m_tc = TransformS.AddComponent(entity, "FloatingNodeTween" + m_loop.m_levelNumber);
		TransformS.ParentComponent(m_tc, m_rootTC, Vector3.zero);
		m_tc.transform.localRotation = Quaternion.identity;
		Initialize();
	}

	public override void Initialize()
	{
		m_domeTC = TransformS.AddComponent(m_tc.p_entity, GetTransformName());
		TransformS.ParentComponent(m_domeTC, m_tc, Vector3.zero);
		m_domeUITC = TransformS.AddComponent(m_tc.p_entity);
		TransformS.ParentComponent(m_domeUITC, m_domeTC, Vector3.zero);
		m_domeNumberTC = TransformS.AddComponent(m_tc.p_entity);
		TransformS.ParentComponent(m_domeNumberTC, m_domeUITC, new Vector3(0f, -5f, -5f));
		m_domeUITC.transform.LookAt(m_loop.m_planet.m_planetCamera.gameObject.transform, m_loop.m_planet.m_planetCamera.gameObject.transform.up);
		m_domeUITC.transform.localRotation *= Quaternion.Euler(Vector3.up * 180f);
		base.Initialize();
	}

	public override string GetTransformName()
	{
		return "FloaterNode" + m_loop.m_levelNumber;
	}

	public override void CreateTouchArea()
	{
		m_tac = TouchAreaS.AddCircleArea(m_domeUITC, "FloatingPlanetNode" + m_loop.m_nodeId, 5f, m_loop.m_planet.m_planetCamera);
		TouchAreaS.AddTouchEventListener(m_tac, TouchHandler);
	}

	public override void CreatePrefab()
	{
		if (m_prefab != null)
		{
			PrefabS.RemoveComponent(m_prefab);
			m_prefab = null;
		}
		SetPrefabName();
		if (string.IsNullOrEmpty(m_prefabName))
		{
			m_prefabName = "FloatingPlatformPrefab";
		}
		GameObject gameObject = ResourceManager.GetGameObject(m_prefabName + "_GameObject");
		m_prefab = PrefabS.AddComponent(m_domeTC, Vector3.zero, gameObject);
		PrefabS.SetCamera(m_prefab.p_gameObject, m_loop.m_planet.m_planetCamera);
		TweenC c = TweenS.AddTransformTween(m_domeTC, TweenedProperty.Position, TweenStyle.QuadInOut, Vector3.zero, Vector3.up * 5f, Random.Range(1.9f, 2.1f), Random.Range(0f, 1f), true);
		TweenS.SetAdditionalTweenProperties(c, -1, true, TweenStyle.QuadInOut);
		TweenC c2 = TweenS.AddTransformTween(m_domeTC, TweenedProperty.Rotation, TweenStyle.QuadInOut, Vector3.up * -90f, Vector3.up * 90f, Random.Range(8f, 10f), Random.Range(0f, 2f), true);
		TweenS.SetAdditionalTweenProperties(c2, -1, true, TweenStyle.QuadInOut);
	}

	public virtual void SetPrefabName()
	{
		m_prefabName = string.Empty;
	}

	public override void CreateUI()
	{
		if (m_numberText == null)
		{
			Frame frame = m_planetNodeSpriteSheet.m_atlas.GetFrame("menu_timer_background");
			bgSpriteC = SpriteS.AddComponent(m_domeNumberTC, frame, m_planetNodeSpriteSheet);
			SpriteS.SetColor(bgSpriteC, new Color(1f, 1f, 1f, 0.6f));
			m_numberText = TextMeshS.AddComponent(m_domeNumberTC, Vector3.forward * -0.5f, "HurmeSemiBold_Font", 5f, 2f, 30f, Align.Center, Align.Middle, m_loop.m_planet.m_planetCamera, m_loop.m_levelNumber.ToString());
			string bannerString = (m_loop as PsTimedEventLoop).GetBannerString();
			TextMeshS.SetText(m_numberText, bannerString, false);
			m_numberText.m_textMesh.characterSize = 0.6f;
			SpriteS.SetDimensions(bgSpriteC, m_numberText.m_renderer.bounds.size.x * 1.2f, m_numberText.m_renderer.bounds.size.y * 0.8f);
			m_numberText.m_renderer.material.shader = Shader.Find("Framework/FontShaderFg");
		}
	}

	public override void CreateRoad()
	{
	}

	public override void Update()
	{
		m_domeUITC.transform.LookAt(m_loop.m_planet.m_planetCamera.gameObject.transform, m_loop.m_planet.m_planetCamera.gameObject.transform.up);
		m_domeUITC.transform.localRotation *= Quaternion.Euler(Vector3.up * 180f);
		bool flag = false;
		if (m_numberText != null && m_tc.p_entity.m_visible)
		{
			if (Main.m_gameTicks % 30 == 0)
			{
				string bannerString = (m_loop as PsTimedEventLoop).GetBannerString();
				flag = m_numberText.m_textMesh.text.Length != bannerString.Length;
				TextMeshS.SetTextOptimized(m_numberText, bannerString);
			}
			if (flag)
			{
				SpriteS.SetDimensions(bgSpriteC, m_numberText.m_renderer.bounds.size.x * 1.2f, m_numberText.m_renderer.bounds.size.y * 0.8f);
			}
		}
		UpdateDestructionAnimation();
		if (m_newNode)
		{
			UpdateFloatingNodePosition();
		}
	}

	protected virtual void UpdateFloatingNodePosition()
	{
		Quaternion quaternion = Quaternion.Euler(m_loop.m_planet.m_nodeRowAngleInterval * (float)m_loop.m_levelNumber, 0f, 0f);
		Quaternion localRotation = Quaternion.Lerp(m_idTC.transform.localRotation, quaternion, 0.025f);
		m_idTC.transform.localRotation = localRotation;
		if (Quaternion.Angle(quaternion, m_idTC.transform.localRotation) < 0.05f)
		{
			m_newNode = false;
		}
	}

	public virtual void SetNewNode()
	{
		m_newNode = true;
		float num = 90f;
		num += m_loop.m_planet.m_nodeRowAngleInterval * (float)m_loop.m_levelNumber;
		m_idTC.transform.localEulerAngles = new Vector3(num, 0f, 0f);
		SoundS.PlaySingleShot("/Metagame/FlyingDomeAppear", Vector2.zero);
	}

	public void RearrangeNodePosition()
	{
		m_newNode = true;
	}

	private void UpdateDestructionAnimation()
	{
		if (m_shadowProjector != null && m_shadowTween != null)
		{
			m_shadowProjector.orthographicSize = m_shadowTween.currentValue.x;
			if (m_shadowTween.currentValue.x < 0.1f)
			{
				TweenS.RemoveComponent(m_shadowTween);
				m_shadowTween = null;
				m_shadowProjector = null;
			}
		}
	}

	public void DestroyNode()
	{
		if (m_destroyStarted)
		{
			return;
		}
		m_destroyStarted = true;
		m_shadowProjector = m_prefab.p_gameObject.GetComponentInChildren<Projector>();
		if (m_shadowProjector != null)
		{
			m_shadowTween = TweenS.AddTween(m_tc.p_entity, TweenStyle.ExpoIn, m_shadowProjector.orthographicSize, 0f, 0.4f, 0f);
		}
		TweenC scaleTween = TweenS.AddTransformTween(m_tc, TweenedProperty.Scale, TweenStyle.ExpoIn, new Vector3(0f, 0f, 0f), 0.5f, 0f, false);
		TweenS.AddTweenEndEventListener(scaleTween, delegate
		{
			TweenS.RemoveComponent(scaleTween);
			if (PsPlanetManager.GetCurrentPlanet().m_spaceEntity != null)
			{
				Entity entity = EntityManager.AddEntity();
				TransformC parentTC = TransformS.AddComponent(entity, m_tc.transform.position);
				PrefabC c = PrefabS.AddComponent(parentTC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.FloatingDomeDeathEffect_GameObject));
				PrefabS.SetCamera(c, m_loop.m_planet.m_planetCamera);
				m_loop.m_planet.m_floaterManager.RearrangeFloaters(this);
				m_loop.m_planet.m_floaterManager.m_floatingNodeList.Remove(this);
				TimerC timerC = TimerS.AddComponent(entity, "FloatingNodeRemoveTimer", 1f, 0f, true);
				Destroy();
				SoundS.PlaySingleShot("/Metagame/FlyingDomeDisappear", Vector2.zero);
			}
		});
	}

	public void TimedDestructionOfNode(float _delay)
	{
		TimerS.AddComponent(m_tc.p_entity, "FloatingNodeDestroyTimer", 0f, _delay, false, delegate(TimerC _c)
		{
			TimerS.RemoveComponent(_c);
			DestroyNode();
		});
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public override void Highlight()
	{
	}

	public override void RemoveHighlight()
	{
	}

	public override void SetState()
	{
		SetActive();
	}

	public override void TouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		if (_touchCount == 1 && _touchPhase == TouchAreaPhase.ReleaseIn && !_touches[0].m_dragged)
		{
			SoundS.PlaySingleShot("/UI/DomeSelect", Vector3.zero);
			m_loop.StartLoop();
		}
	}
}
