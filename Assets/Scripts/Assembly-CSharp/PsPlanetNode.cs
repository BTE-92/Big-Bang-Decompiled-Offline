using System;
using System.Collections.Generic;
using UnityEngine;

public class PsPlanetNode
{
	public SpriteSheet m_planetNodeSpriteSheet;

	public TransformC m_tc;

	public TransformC m_uiTC;

	public TransformC m_numberTC;

	public TransformC m_keyTC;

	public TransformC m_starsTC;

	protected TransformC m_leftStarTC;

	protected TransformC m_centerStarTC;

	protected TransformC m_rightStarTC;

	protected TransformC m_trophyTC;

	public TransformC m_highlightTC;

	protected TweenC m_highlightTween;

	protected PrefabC m_highlightPrefab;

	protected PrefabC m_roadPrefab;

	protected PrefabC m_roadPrefab2;

	public float m_roadPhase;

	public float m_roadWidth;

	public bool m_unlocked;

	protected TransformC m_unlockTC;

	protected TransformC m_unlockShiftTC;

	public TextMeshC m_starCounterText;

	public PsGameLoop m_loop;

	public bool m_created;

	public PrefabC m_prefab;

	public TextMeshC m_numberText;

	public TouchAreaC m_tac;

	public bool m_hideUI;

	protected Color m_fontColor;

	public EffectCheckpoint m_checkpointEffect;

	protected float m_starCreationTime;

	public static float m_roadXOffset = 17.5f;

	private float m_sidePathDomeScale = 0.87f;

	public PsPlanetNode(PsGameLoop _loop, bool tutorial = false)
	{
		m_loop = _loop;
		m_loop.m_node = this;
		if (tutorial)
		{
			m_planetNodeSpriteSheet = m_loop.m_planet.m_planetSpriteSheetTutorial;
		}
		else
		{
			m_planetNodeSpriteSheet = m_loop.m_planet.m_planetSpriteSheet;
		}
		CreateBase();
	}

	public static float GetXPosition(PsGameLoop _loop, int _rowOffset = 0)
	{
		int num = _loop.m_nodeId + _loop.m_path.m_startNodeId + _rowOffset;
		float num2 = 0f;
		List<PsGameLoop> row = PsPlanetManager.GetCurrentPlanet().GetRow(num);
		if (row.Count > 1)
		{
			for (int i = 0; i < row.Count; i++)
			{
				num2 += (float)row[i].m_path.m_lane;
			}
			if (row.Count > 0)
			{
				num2 /= (float)row.Count;
			}
		}
		float num3 = 1f;
		if (_loop.m_path.m_startNodeId > 0)
		{
			if (_loop.m_nodeId + _rowOffset < 0)
			{
				num3 *= -3f;
			}
			else if (_loop.m_nodeId + _rowOffset < 1)
			{
				num3 = 0f;
			}
		}
		float num4 = 0f;
		float num5 = (float)num + num4;
		return Mathf.Sin(num5 * 60f * ((float)Math.PI / 180f)) * 5f + Mathf.Sin(num5 * 25f * ((float)Math.PI / 180f)) * 5f - num2 * 15f + (float)(15 * _loop.m_path.m_lane) * num3 - m_roadXOffset;
	}

	public virtual void CreateBase()
	{
		int num = m_loop.m_nodeId + m_loop.m_path.m_startNodeId;
		float xPosition = GetXPosition(m_loop);
		float num2 = 0f;
		float num3 = (float)num + num2;
		float num4 = Mathf.Cos((m_loop.m_planet.m_pathStartOffset + num3 * m_loop.m_planet.m_nodeRowAngleInterval) * ((float)Math.PI / 180f)) * m_loop.m_planet.m_planetRadius;
		float num5 = Mathf.Sin((m_loop.m_planet.m_pathStartOffset + num3 * m_loop.m_planet.m_nodeRowAngleInterval) * ((float)Math.PI / 180f)) * m_loop.m_planet.m_planetRadius;
		Vector3 pos = new Vector3(xPosition, num4, num5).normalized * m_loop.m_planet.m_planetRadius;
		float x = Mathf.Atan2(0f - num4, num5) * 57.29578f + 180f;
		float y = Mathf.Atan2(m_loop.m_planet.m_planetRadius, xPosition) * 57.29578f + 90f;
		Entity entity = EntityManager.AddEntity(new string[2] { "PlanetUI", "Tutorial" });
		m_tc = TransformS.AddComponent(entity, GetTransformName(), pos);
		TransformS.ParentComponent(m_tc, m_loop.m_planet.m_alienPlanetTC);
		TransformS.SetRotation(m_tc, new Vector3(x, 0f, 0f));
		m_tc.transform.Rotate(new Vector3(90f, y, 0f), Space.Self);
		Initialize();
	}

	public virtual string GetTransformName()
	{
		return "PlanetNode" + m_loop.m_nodeId;
	}

	public virtual void Initialize()
	{
		m_uiTC = TransformS.AddComponent(m_tc.p_entity);
		m_numberTC = TransformS.AddComponent(m_uiTC.p_entity);
		m_keyTC = TransformS.AddComponent(m_uiTC.p_entity, "keyTC");
		m_starsTC = TransformS.AddComponent(m_uiTC.p_entity);
		m_leftStarTC = TransformS.AddComponent(m_tc.p_entity);
		m_centerStarTC = TransformS.AddComponent(m_tc.p_entity);
		m_rightStarTC = TransformS.AddComponent(m_tc.p_entity);
		m_trophyTC = TransformS.AddComponent(m_tc.p_entity);
		TransformS.ParentComponent(m_uiTC, m_tc, Vector3.zero);
		TransformS.ParentComponent(m_numberTC, m_uiTC, new Vector3(0f, 1.5f, -6f));
		TransformS.ParentComponent(m_starsTC, m_uiTC, new Vector3(0f, -1.5f, -10f));
		TransformS.ParentComponent(m_keyTC, m_starsTC, Vector3.up * -1.5f + Vector3.forward * -0.1f);
		TransformS.ParentComponent(m_leftStarTC, m_starsTC, Vector3.right * -2.8f);
		TransformS.ParentComponent(m_centerStarTC, m_starsTC, Vector3.up * -0.75f + Vector3.forward * -0.1f);
		TransformS.ParentComponent(m_rightStarTC, m_starsTC, Vector3.right * 2.8f);
		TransformS.ParentComponent(m_trophyTC, m_starsTC, Vector3.right * 6.3f);
		m_unlockTC = TransformS.AddComponent(m_tc.p_entity);
		TransformS.ParentComponent(m_unlockTC, m_uiTC, new Vector3(0f, 14f, 0f));
		m_unlockShiftTC = TransformS.AddComponent(m_tc.p_entity);
		TransformS.ParentComponent(m_unlockShiftTC, m_unlockTC, Vector3.zero);
		m_uiTC.transform.LookAt(m_loop.m_planet.m_planetCamera.gameObject.transform, m_loop.m_planet.m_planetCamera.gameObject.transform.up);
		m_uiTC.transform.localRotation *= Quaternion.Euler(Vector3.up * 180f);
		SpriteS.ConvertSpritesToPrefabComponent(m_uiTC, true);
		m_fontColor = DebugDraw.GetColor(48f, 30f, 26f, 255f);
		Create();
	}

	public virtual void Create()
	{
		m_starCreationTime = 0f;
		CreatePrefab();
		if (m_loop.m_path != m_loop.m_planet.m_mainPath)
		{
			TransformS.SetScale(m_tc, m_sidePathDomeScale);
		}
		SetState();
		CreateRoad();
		CreateTouchArea();
	}

	public virtual void CreateTouchArea()
	{
		m_tac = TouchAreaS.AddCircleArea(m_uiTC, "PlanetNode" + m_loop.m_nodeId, 5f, m_loop.m_planet.m_planetCamera);
		TouchAreaS.AddTouchEventListener(m_tac, TouchHandler);
	}

	public virtual void CreateRoad()
	{
		DestroyRoad();
		if (m_loop.m_nodeId <= 1 && m_loop.m_path.m_startNodeId <= 0)
		{
			return;
		}
		int num = 10;
		float num2 = 1f / (float)num;
		float num3 = m_loop.m_planet.m_nodeRowAngleInterval / (float)num;
		List<Vector3> list = new List<Vector3>();
		List<Vector3> list2 = new List<Vector3>();
		Vector2 zero = Vector2.zero;
		Vector2 p1_start = Vector2.up * 10f;
		Vector2 p2_end = Vector2.up * 20f;
		Vector2 p3_endHandle = Vector2.up * 30f;
		zero.x = GetXPosition(m_loop, -2);
		p1_start.x = GetXPosition(m_loop, -1);
		p2_end.x = GetXPosition(m_loop);
		p3_endHandle.x = GetXPosition(m_loop, 1);
		float num4 = 0f;
		for (int i = 0; i < num; i++)
		{
			int num5 = m_loop.m_nodeId + m_loop.m_path.m_startNodeId;
			float num6 = 0f;
			if (m_loop.m_nodeId == 1)
			{
				num6 *= (float)i / (float)num;
			}
			float num7 = (float)num5 + num6;
			float x = ToolBox.PointOnBezierSpline(zero, p1_start, p2_end, p3_endHandle, num4).x;
			float y = Mathf.Cos((m_loop.m_planet.m_pathStartOffset + (num7 - 1f) * m_loop.m_planet.m_nodeRowAngleInterval + (float)i * num3) * ((float)Math.PI / 180f)) * m_loop.m_planet.m_planetRadius;
			float z = Mathf.Sin((m_loop.m_planet.m_pathStartOffset + (num7 - 1f) * m_loop.m_planet.m_nodeRowAngleInterval + (float)i * num3) * ((float)Math.PI / 180f)) * m_loop.m_planet.m_planetRadius;
			Vector3 item = new Vector3(x, y, z).normalized * (m_loop.m_planet.m_planetRadius - 0.1f);
			if ((float)i < m_roadPhase * (float)num)
			{
				list.Add(item);
			}
			else if (list2.Count == 0)
			{
				float num8 = m_roadPhase * (float)num;
				x = ToolBox.PointOnBezierSpline(zero, p1_start, p2_end, p3_endHandle, m_roadPhase).x;
				y = Mathf.Cos((m_loop.m_planet.m_pathStartOffset + (num7 - 1f) * m_loop.m_planet.m_nodeRowAngleInterval + num8 * num3) * ((float)Math.PI / 180f)) * m_loop.m_planet.m_planetRadius;
				z = Mathf.Sin((m_loop.m_planet.m_pathStartOffset + (num7 - 1f) * m_loop.m_planet.m_nodeRowAngleInterval + num8 * num3) * ((float)Math.PI / 180f)) * m_loop.m_planet.m_planetRadius;
				Vector3 item2 = new Vector3(x, y, z).normalized * (m_loop.m_planet.m_planetRadius - 0.1f);
				list.Add(item2);
				list2.Add(item2);
			}
			else
			{
				list2.Add(item);
			}
			num4 += num2;
		}
		Color color = new Color(0.8941f, 0.5725f, 0.2156f, 1f);
		Color color2 = new Color(0.9333f, 0.7137f, 0.4117f, 1f) * 1.05f;
		float num9 = 3f;
		if (m_loop.m_path.m_startNodeId > 0)
		{
			num9 = 1.8539999f;
		}
		if (list.Count > 1)
		{
			m_roadPrefab = PrefabS.CreateSphericalPathFromVectorArray(m_tc.parent, Vector3.zero, list.ToArray(), m_roadWidth * num9, color, ResourceManager.GetMaterial("Framework/Line32Mat"), PsPlanetManager.GetCurrentPlanet().m_planetCamera, Position.Center, false);
		}
		if (list2.Count > 1)
		{
			m_roadPrefab2 = PrefabS.CreateSphericalPathFromVectorArray(m_tc.parent, Vector3.zero, list2.ToArray(), m_roadWidth * num9, color2, ResourceManager.GetMaterial("Framework/Line32Mat"), PsPlanetManager.GetCurrentPlanet().m_planetCamera, Position.Center, false);
		}
	}

	public virtual void DestroyRoad()
	{
		if (m_roadPrefab != null)
		{
			PrefabS.RemoveComponent(m_roadPrefab);
			m_roadPrefab = null;
		}
		if (m_roadPrefab2 != null)
		{
			PrefabS.RemoveComponent(m_roadPrefab2);
			m_roadPrefab2 = null;
		}
	}

	public virtual void CreatePrefab()
	{
	}

	public virtual void CreateUI()
	{
	}

	public virtual void RemoveUI()
	{
	}

	public virtual void Update()
	{
		if (m_loop.m_drawRoad)
		{
			float num = Mathf.Min(0.5f, Main.m_resettingGameTime - m_loop.m_roadDrawStart);
			m_roadPhase = TweenS.tween(TweenStyle.Linear, num, 0.5f, 0f, 1f);
			CreateRoad();
			if (num == 0.5f)
			{
				m_loop.m_drawRoad = false;
			}
		}
		if (m_loop.m_scaleRoad)
		{
			float num2 = Mathf.Min(1f, Main.m_resettingGameTime - m_loop.m_roadScaleStart);
			m_roadWidth = TweenS.tween(TweenStyle.ElasticOut, num2, 1f, 0f, 1f);
			CreateRoad();
			if (num2 == 1f)
			{
				m_loop.m_scaleRoad = false;
			}
		}
		float num3 = TweenS.tween(TweenStyle.CubicOut, Mathf.Min(0.5f, Main.m_resettingGameTime - m_starCreationTime), 0.5f, 0f, 1f);
		float num4 = Vector3.Dot(m_tc.transform.position.normalized, Vector3.up);
		m_uiTC.transform.LookAt(m_loop.m_planet.m_planetCamera.gameObject.transform, m_loop.m_planet.m_planetCamera.gameObject.transform.up);
		m_uiTC.transform.localRotation *= Quaternion.Euler(Vector3.up * 180f);
		float num5 = 1f;
		if (num4 > 0.9f)
		{
			num5 = 0f;
		}
		else if (num4 > 0.8f && num4 < 0.9f)
		{
			num5 = TweenS.tween(TweenStyle.CubicIn, num4 - 0.8f, 0.1f, 1f, -1f);
		}
		TransformS.SetPosition(m_numberTC, new Vector3(0f, 1.5f * num4, -6f) + Vector3.up * -2.25f * (1f - num5));
		TransformS.SetPosition(m_starsTC, new Vector3(0f, 0.5f + -6f * (1f - num4), -10f) + Vector3.up * -1f * (1f - num5));
		if (num3 < 1f && m_loop.m_path == m_loop.m_planet.m_mainPath)
		{
			Quaternion localRotation = Quaternion.Euler(Vector3.right * 110f * (1f - num3));
			m_starsTC.transform.localRotation = localRotation;
			m_keyTC.transform.localRotation = localRotation;
		}
		else if (m_hideUI)
		{
			m_keyTC.transform.localScale *= Mathf.Lerp(m_keyTC.transform.localScale.x, 0f, 0.1f);
			m_keyTC.updateScale = true;
		}
		else
		{
			Quaternion localRotation2 = Quaternion.Euler(Vector3.right * 110f * (1f - num5));
			m_numberTC.transform.localRotation = localRotation2;
			m_starsTC.transform.localRotation = localRotation2;
			m_keyTC.transform.localRotation = localRotation2;
		}
		SpriteS.SetColorByTransformComponent(m_starsTC, new Color(1f, 1f, 1f, num5), true);
		m_fontColor.a = num5;
		if (m_numberText != null)
		{
			m_numberText.m_textMesh.GetComponent<Renderer>().material.SetColor("_Color", m_fontColor);
		}
		if (m_highlightTween != null)
		{
			Color color = m_highlightPrefab.p_gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>()
				.material.GetColor("_TintColor");
			color.a = m_highlightTC.transform.localScale.y * 0.35f;
			m_highlightPrefab.p_gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>()
				.material.SetColor("_TintColor", color);
			m_highlightPrefab.p_gameObject.transform.GetChild(0).GetChild(1).GetComponent<Renderer>()
				.material.SetColor("_TintColor", color);
		}
	}

	public virtual void Destroy()
	{
		DestroyRoad();
		EntityManager.RemoveEntity(m_tc.p_entity);
		if (m_highlightTC != null)
		{
			UnityEngine.Object.Destroy(m_highlightPrefab.p_gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>()
				.material);
				UnityEngine.Object.Destroy(m_highlightPrefab.p_gameObject.transform.GetChild(0).GetChild(1).GetComponent<Renderer>()
					.material);
					EntityManager.RemoveEntity(m_highlightTC.p_entity);
				}
				m_loop.m_scaleRoad = false;
				m_loop.m_drawRoad = false;
				m_loop.m_node = null;
				m_loop = null;
				m_created = false;
			}

			public virtual void Deactivate()
			{
				if (m_checkpointEffect != null)
				{
					m_checkpointEffect.Claim();
				}
				RemoveHighlight();
			}

			public virtual void SetInactive()
			{
				if (m_checkpointEffect != null)
				{
					m_checkpointEffect.SetLocked();
				}
				RemoveHighlight();
				m_roadWidth = 1f;
				m_roadPhase = 0f;
			}

			public virtual void Activate()
			{
				if (m_unlocked)
				{
					SetActive();
					return;
				}
				m_roadWidth = 1f;
				m_roadPhase = 0f;
				m_loop.m_drawRoad = true;
				m_loop.m_roadDrawStart = Main.m_resettingGameTime;
				TimerS.AddComponent(m_tc.p_entity, string.Empty, 0.4f, 0f, false, Activate2);
				m_unlocked = false;
			}

			public virtual void Activate2(TimerC _c)
			{
				TimerS.RemoveComponent(_c);
				if (m_checkpointEffect != null)
				{
					m_checkpointEffect.Activate();
				}
				m_starCreationTime = Main.m_resettingGameTime;
				CreateUI();
				Highlight();
				m_unlocked = true;
			}

			public virtual void SetActive()
			{
				if (m_checkpointEffect != null)
				{
					m_checkpointEffect.SetIdleActive();
				}
				CreateUI();
				Highlight();
				m_roadWidth = 1f;
				m_roadPhase = 1f;
				m_loop.m_drawRoad = true;
				m_unlocked = true;
			}

			public virtual void SetClaimed()
			{
				if (m_checkpointEffect != null)
				{
					m_checkpointEffect.SetClaimed();
				}
				m_roadWidth = 1f;
				m_roadPhase = 1f;
				m_unlocked = true;
			}

			public virtual void Claim()
			{
				if (m_checkpointEffect != null)
				{
					m_checkpointEffect.Claim();
				}
				RemoveHighlight();
			}

			public virtual void SetHidden()
			{
				SetInactive();
				m_tc.transform.localScale = Vector3.zero;
				m_roadWidth = 0f;
				m_roadPhase = 0f;
			}

			public virtual void Reveal()
			{
				if (m_tc != null && m_tc.p_entity != null && m_loop != null)
				{
					Vector3 one = Vector3.one;
					if (m_loop.m_path != m_loop.m_planet.m_mainPath)
					{
						one *= m_sidePathDomeScale;
					}
					TweenS.AddTransformTween(m_tc, TweenedProperty.Scale, TweenStyle.ElasticOut, one, 0.5f, 0f, true);
					m_loop.m_keepNodeHidden = false;
					SetState();
					m_roadWidth = 0f;
					m_roadPhase = 0f;
					m_loop.m_scaleRoad = true;
					m_loop.m_roadScaleStart = Main.m_resettingGameTime;
				}
			}

			public virtual void SetState()
			{
				bool keepNodeHidden = m_loop.m_keepNodeHidden;
				bool flag = m_loop.m_nodeId > m_loop.m_path.m_currentNodeId;
				bool flag2 = m_loop.m_nodeId < m_loop.m_path.m_currentNodeId;
				bool flag3 = m_loop.m_nodeId == m_loop.m_path.m_currentNodeId;
				bool flag4 = m_loop == PsState.m_activeGameLoop;
				bool flag5 = PsState.m_activeGameLoop != null && (PsState.m_activeGameLoop.m_nodeId + 1 == m_loop.m_nodeId || (PsState.m_activeGameLoop.m_sidePath == m_loop.m_path && m_loop.m_nodeId == 1));
				if (keepNodeHidden)
				{
					SetHidden();
				}
				else if (flag || (flag3 && flag5))
				{
					SetInactive();
				}
				else if (flag4 || flag3)
				{
					SetActive();
				}
				else if (flag2)
				{
					SetClaimed();
				}
			}

			public virtual void GiveStars(TimerComponentDelegate _starsGivenDelegate)
			{
				TimerC timerC = TimerS.AddComponent(m_tc.p_entity, string.Empty, (float)m_loop.m_scoreBest * 0.25f + 0.5f, 0f, false, _starsGivenDelegate);
				int lastEarnedStars = Mathf.Max(0, m_loop.m_scoreCurrent - m_loop.m_scoreOld);
				PsPlanetPath mainPath = PsPlanetManager.GetCurrentPlanet().m_mainPath;
				PsGameLoop nodeInfo = mainPath.GetNodeInfo(mainPath.GetLastBlockId());
				bool flag = nodeInfo.m_planet.m_banners != null && nodeInfo.m_planet.m_banners.Count > 0;
				if (nodeInfo != null && flag)
				{
					nodeInfo.m_planet.m_banners[0].SetLastEarnedStars(lastEarnedStars);
				}
				if (m_loop.m_scoreCurrent > 0)
				{
					TimerComponentDelegate timerComponentDelegate = GiveStar;
					if (m_loop.m_scoreOld == 0 && flag)
					{
						timerComponentDelegate = (TimerComponentDelegate)Delegate.Combine(timerComponentDelegate, new TimerComponentDelegate(nodeInfo.m_planet.m_banners[0].IncreaseStar));
					}
					timerC = TimerS.AddComponent(m_tc.p_entity, string.Empty, 0.5f, 0f, false, timerComponentDelegate);
					timerC.customComponent = m_leftStarTC;
				}
				if (m_loop.m_scoreCurrent > 1)
				{
					TimerComponentDelegate timerComponentDelegate2 = GiveStar;
					if (m_loop.m_scoreOld <= 1 && flag)
					{
						timerComponentDelegate2 = (TimerComponentDelegate)Delegate.Combine(timerComponentDelegate2, new TimerComponentDelegate(nodeInfo.m_planet.m_banners[0].IncreaseStar));
					}
					timerC = TimerS.AddComponent(m_tc.p_entity, string.Empty, 0.75f, 0f, false, timerComponentDelegate2);
					timerC.customComponent = m_centerStarTC;
				}
				if (m_loop.m_scoreCurrent > 2)
				{
					TimerComponentDelegate timerComponentDelegate3 = GiveStar;
					if (m_loop.m_scoreOld <= 2 && flag)
					{
						timerComponentDelegate3 = (TimerComponentDelegate)Delegate.Combine(timerComponentDelegate3, new TimerComponentDelegate(nodeInfo.m_planet.m_banners[0].IncreaseStar));
					}
					timerC = TimerS.AddComponent(m_tc.p_entity, string.Empty, 1f, 0f, false, timerComponentDelegate3);
					timerC.customComponent = m_rightStarTC;
				}
			}

			protected virtual void GiveStar(TimerC _c)
			{
				Frame frame = m_planetNodeSpriteSheet.m_atlas.GetFrame("menu_star_small_full");
				TransformC transformC = _c.customComponent as TransformC;
				SpriteS.RemoveComponentsByTransformComponent(transformC);
				SpriteC spriteC = SpriteS.AddComponent(transformC, frame, m_planetNodeSpriteSheet);
				if (transformC == m_centerStarTC)
				{
					SpriteS.SetSortValue(spriteC, -1f);
				}
				SpriteS.SetDimensions(spriteC, 4f * (frame.width / frame.height), 4f);
				SpriteS.SetColor(spriteC, Color.white);
				TweenS.AddTransformTween(transformC, TweenedProperty.Scale, TweenStyle.CubicOut, Vector3.one * 1.25f, Vector3.one, 0.5f, 0f, true);
				TimerS.RemoveComponent(_c);
				SoundS.PlaySingleShot("/Metagame/StarAppear", Vector3.zero);
			}

			public virtual void Highlight()
			{
				if (m_highlightTC == null)
				{
					Entity entity = EntityManager.AddEntity();
					m_highlightTC = TransformS.AddComponent(entity);
					m_highlightTC.transform.localScale = new Vector3(1f, 0f, 1f);
					TransformS.ParentComponent(m_highlightTC, m_tc, Vector3.zero);
					m_highlightTC.transform.localRotation = Quaternion.identity;
					GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.PathButtonActiveEffectPrefab_GameObject);
					m_highlightPrefab = PrefabS.AddComponent(m_highlightTC, Vector3.zero, gameObject);
					m_highlightPrefab.p_gameObject.transform.localScale = Vector3.one * 1.6f;
					PrefabS.SetCamera(m_highlightPrefab.p_gameObject, m_loop.m_planet.m_planetCamera);
					m_highlightTween = TweenS.AddTransformTween(m_highlightTC, TweenedProperty.Scale, TweenStyle.CubicOut, new Vector3(1f, 0f, 1f), Vector3.one, 1f, 0f, false);
				}
			}

			public virtual void RemoveHighlight()
			{
				if (m_highlightTC != null)
				{
					m_highlightTween = TweenS.AddTransformTween(m_highlightTC, TweenedProperty.Scale, TweenStyle.CubicOut, Vector3.one, new Vector3(1f, 0f, 1f), 1f, 0f, false);
					TweenS.AddTweenEndEventListener(m_highlightTween, RemoveHighlightDelegate);
				}
			}

			protected void RemoveHighlightDelegate(TweenC _c)
			{
				if (m_highlightTC != null)
				{
					UnityEngine.Object.Destroy(m_highlightPrefab.p_gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>()
						.material);
						UnityEngine.Object.Destroy(m_highlightPrefab.p_gameObject.transform.GetChild(0).GetChild(1).GetComponent<Renderer>()
							.material);
							EntityManager.RemoveEntity(m_highlightTC.p_entity);
							m_highlightTC = null;
							m_highlightTween = null;
						}
					}

					public virtual void TouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
					{
						if (_touchCount == 1 && _touchPhase == TouchAreaPhase.ReleaseIn && !_touches[0].m_dragged)
						{
							TouchAreaS.CancelAllTouches();
							if (m_loop.m_nodeId <= m_loop.m_path.m_currentNodeId)
							{
								SoundS.PlaySingleShot("/UI/DomeSelect", Vector3.zero);
							}
							m_loop.StartLoop();
						}
					}
				}
