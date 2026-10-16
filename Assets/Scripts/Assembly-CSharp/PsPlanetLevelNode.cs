using System;
using UnityEngine;

public class PsPlanetLevelNode : PsPlanetNode
{
	private UITextbox m_bubbleTextBox;

	private UIVerticalList m_imageArea;

	private PsUIProfileImage m_profileImage;

	private TransformC m_bubbleScaleTC;

	private bool m_inactiveSet;

	private bool m_numberCreated;

	private bool m_uiCreated;

	protected bool m_updateUI;

	public bool m_createFlyingNode;

	private Entity m_bubbleEntity;

	public PsPlanetLevelNode(PsGameLoop _loop, bool _tutorial = false)
		: base(_loop, _tutorial)
	{
	}

	public override void CreatePrefab()
	{
		if (m_prefab != null)
		{
			PrefabS.RemoveComponent(m_prefab);
			m_prefab = null;
		}
		GameObject gameObject = null;
		m_prefab = PrefabS.AddComponent(_gameObject: (m_loop.m_path == m_loop.m_planet.m_mainPath) ? ResourceManager.GetGameObject(RESOURCE.PathButtonLargePrefab_GameObject) : ((m_loop.m_minigameSearchParametres.m_difficulty == PsGameDifficulty.Impossible) ? ResourceManager.GetGameObject(RESOURCE.PathButtonInsanePrefab_GameObject) : ((m_loop.m_minigameSearchParametres.m_gameMode != PsGameMode.StarCollect) ? ResourceManager.GetGameObject(RESOURCE.PathButtonRacePrefab_GameObject) : ResourceManager.GetGameObject(RESOURCE.PathButtonFindStarsPrefab_GameObject))), _parentTC: m_tc, _offset: Vector3.zero, _identifier: "LevelNode", _resetLocalRotation: true, _resetLocalPosition: false);
		PrefabS.SetCamera(m_prefab.p_gameObject, m_loop.m_planet.m_planetCamera);
		m_checkpointEffect = m_prefab.p_gameObject.GetComponent<EffectCheckpoint>();
	}

	public override string GetTransformName()
	{
		if (m_loop.m_minigameSearchParametres.m_gameMode == PsGameMode.StarCollect)
		{
			return "PlanetNodeStarCollect" + m_loop.m_nodeId;
		}
		return "PlanetNode" + m_loop.m_nodeId;
	}

	public override void SetClaimed()
	{
		base.SetClaimed();
		CreateUI();
	}

	public override void CreateUI()
	{
		base.CreateUI();
		if (!m_numberCreated || !m_uiCreated)
		{
			if (m_loop.m_path.m_startNodeId == 0 && !m_numberCreated)
			{
				m_numberText = TextMeshS.AddComponent(m_numberTC, Vector3.zero, "KGSecondChances_Font", 2f, 2f, 34f, Align.Center, Align.Middle, m_loop.m_planet.m_planetCamera, m_loop.m_nodeId.ToString());
				TextMeshS.SetText(m_numberText, m_loop.m_levelNumber.ToString());
				m_numberText.m_textMesh.characterSize = 1f;
				m_numberText.m_renderer.material.shader = Shader.Find("Framework/FontShader");
				m_fontColor = DebugDraw.HexToColor("#2b8698");
				m_numberText.m_renderer.material.color = m_fontColor;
				m_numberCreated = true;
				m_updateUI = true;
			}
			if ((m_loop.m_unlocked && !m_inactiveSet && !m_uiCreated && !(m_loop is PsGameLoopEditorTemplate)) || (!m_loop.m_unlocked && m_loop.m_nodeId == m_loop.m_path.m_currentNodeId && !m_inactiveSet && !m_uiCreated && m_loop.m_path.m_name == "MainPath"))
			{
				float num = 3.6f;
				Frame frame = m_planetNodeSpriteSheet.m_atlas.GetFrame("menu_star_small_empty");
				Frame frame2 = m_planetNodeSpriteSheet.m_atlas.GetFrame("menu_star_small_full");
				Frame frame3 = ((m_loop.m_scoreOld <= 0) ? frame : frame2);
				SpriteC spriteC = SpriteS.AddComponent(m_leftStarTC, frame3, m_planetNodeSpriteSheet);
				SpriteS.SetDimensions(spriteC, num * (frame3.width / frame3.height), num);
				SpriteS.SetColor(spriteC, Color.white);
				frame3 = ((m_loop.m_scoreOld <= 1) ? frame : frame2);
				SpriteC spriteC2 = SpriteS.AddComponent(m_centerStarTC, frame3, m_planetNodeSpriteSheet);
				SpriteS.SetSortValue(spriteC2, -1f);
				SpriteS.SetDimensions(spriteC2, num * (frame3.width / frame3.height), num);
				SpriteS.SetColor(spriteC2, Color.white);
				frame3 = ((m_loop.m_scoreOld <= 2) ? frame : frame2);
				SpriteC spriteC3 = SpriteS.AddComponent(m_rightStarTC, frame3, m_planetNodeSpriteSheet);
				SpriteS.SetDimensions(spriteC3, num * (frame3.width / frame3.height), num);
				SpriteS.SetColor(spriteC3, Color.white);
				m_uiCreated = true;
				m_updateUI = true;
			}
			else
			{
				CreateSidePathUI();
			}
			if (m_inactiveSet)
			{
				m_inactiveSet = false;
			}
			if (m_updateUI)
			{
				Update();
			}
		}
	}

	public void CreateSidePathUI()
	{
		if (m_loop.m_path == m_loop.m_planet.m_mainPath || m_loop.m_unlocked || m_loop.m_nodeId < m_loop.m_path.m_currentNodeId || m_uiCreated)
		{
			return;
		}
		float num = 3f;
		string text = string.Empty;
		string hex = "#ffffff";
		if (m_loop is PsGameLoopLevel)
		{
			if (m_loop.m_minigameSearchParametres.m_difficulty == PsGameDifficulty.Impossible)
			{
				text = "INSANE";
				hex = "#ff6969";
			}
			else if (m_loop.m_minigameSearchParametres.m_gameMode == PsGameMode.StarCollect)
			{
				text = "CATCH  STARS";
				hex = "#ffec13";
			}
			else
			{
				text = "RACING";
				hex = "#00ffff";
			}
		}
		else if (m_loop is PsGameLoopEditorPuzzle)
		{
			text = "BUILDER  PUZZLE";
			hex = "#ffb137";
		}
		else if (m_loop is PsGameLoopEditorTemplate)
		{
			text = "CREATE  LEVEL";
			hex = "#96feff";
		}
		else if (m_loop is PsGameLoopVersus)
		{
			text = "VERSUS  RACE";
			hex = "#ffec13";
		}
		Frame frame = m_planetNodeSpriteSheet.m_atlas.GetFrame("menu_domeinfobar");
		SpriteC spriteC = SpriteS.AddComponent(m_keyTC, frame, m_planetNodeSpriteSheet);
		SpriteS.SetDimensions(spriteC, num * (frame.width / frame.height), num);
		SpriteS.SetOffset(spriteC, new Vector3(0f, 0.6f, -0.5f), 0f);
		TextMeshC textMeshC = TextMeshS.AddComponent(m_keyTC, new Vector3(0f, 0.6f, -0.8f), "KGSecondChances_Font", 2f, 2f, 12f, Align.Center, Align.Middle, m_loop.m_planet.m_planetCamera, m_loop.m_nodeId.ToString());
		TextMeshS.SetText(textMeshC, text, false);
		textMeshC.m_textMesh.characterSize = 1f;
		textMeshC.m_renderer.material.shader = Shader.Find("Framework/FontShaderFg");
		textMeshC.m_renderer.material.color = DebugDraw.HexToColor(hex);
		m_uiCreated = true;
		m_updateUI = true;
	}

	public override void SetActive()
	{
		base.SetActive();
	}

	public override void SetInactive()
	{
		base.SetInactive();
		m_inactiveSet = true;
		CreateUI();
	}

	public override void Activate()
	{
		base.Activate();
	}

	private void CreateSpeechBubble()
	{
		if (m_loop == null)
		{
			return;
		}
		if (m_loop.m_minigameMetaData == null && !m_loop.m_loadingMetaData)
		{
			m_loop.LoadMinigameMetaData(CreateSpeechBubble);
			Debug.LogWarning("start loading minigame metadata, adding callback");
			return;
		}
		if (m_loop.m_minigameMetaData == null)
		{
			PsGameLoop loop = m_loop;
			loop.m_loadMetadataCallback = (Action)Delegate.Combine(loop.m_loadMetadataCallback, new Action(CreateSpeechBubble));
			Debug.LogWarning("still loading minigame metadata, adding callback");
			return;
		}
		Camera planetCamera = PsPlanetManager.GetCurrentPlanet().m_planetCamera;
		Frame frame = m_planetNodeSpriteSheet.m_atlas.GetFrame("menu_gamebanner_mini");
		float num = 0.4f;
		float num2 = 9f;
		float num3 = frame.width * num / frame.height * num2 * 0.94f;
		m_bubbleEntity = EntityManager.AddEntity();
		m_bubbleScaleTC = TransformS.AddComponent(m_bubbleEntity, "bubbleScaleTC");
		TransformS.ParentComponent(m_bubbleScaleTC, m_unlockTC, new Vector3(0f, -10f, 0f));
		TransformS.SetRotation(m_bubbleScaleTC, Vector3.zero);
		TransformC transformC = TransformS.AddComponent(m_bubbleEntity, "bubbleTC");
		TransformS.ParentComponent(transformC, m_bubbleScaleTC);
		Vector2[] roundedRect = DebugDraw.GetRoundedRect(num3, num2 * (1f - num), 0.1f * num2, 8, Vector2.zero);
		Vector2[] array = new Vector2[roundedRect.Length + 3];
		Array.Copy(roundedRect, 0, array, 0, 8);
		array[8] = new Vector2(roundedRect[8].x + 3f, roundedRect[8].y);
		array[9] = new Vector2(roundedRect[8].x + 0.7f, roundedRect[8].y - 2f);
		array[10] = new Vector2(roundedRect[8].x + 1f, roundedRect[8].y);
		Array.Copy(roundedRect, 8, array, 11, roundedRect.Length - 8);
		PrefabS.CreatePathPrefabComponentFromVectorArray(transformC, new Vector3(0.15f, -0.15f, 0.3f), array, 0.7f, new Color(0f, 0f, 0f, 0.3f), ResourceManager.GetMaterial("Framework/Line4Mat"), planetCamera, Position.Center, true);
		PrefabS.CreatePathPrefabComponentFromVectorArray(transformC, Vector3.forward * 0.1f, array, 0.25f, DebugDraw.HexToColor("#fffec6"), ResourceManager.GetMaterial("Framework/Line8Mat"), planetCamera, Position.Center, true);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(transformC, Vector3.zero, array, DebugDraw.HexToUint("#fffec6"), DebugDraw.HexToUint("#fffec6"), ResourceManager.GetMaterial("Framework/SolidMat"), planetCamera, string.Empty);
		SpriteC spriteC = SpriteS.AddComponent(transformC, frame, m_planetNodeSpriteSheet);
		SpriteS.SetDimensions(spriteC, num2 * num * (frame.width / frame.height), num2 * num);
		SpriteS.SetColor(spriteC, Color.white);
		SpriteS.SetOffset(spriteC, new Vector3(num3 / 2f * 0.02f, num2 * (1f - num) / 2f + spriteC.height / 2f * 0.5f, -0.2f), 0f);
		TextMeshC textMeshC = TextMeshS.AddComponent(transformC, new Vector3((0f - num3) / 2f * 0.96f, 5.1f, -0.4f), "KGSecondChances_Font", spriteC.width, spriteC.height, 22f, Align.Left, Align.Top, planetCamera, string.Empty);
		TextMeshS.SetText(textMeshC, m_loop.GetName(), false);
		textMeshC.m_textMesh.characterSize = 1f;
		textMeshC.m_renderer.material.shader = Shader.Find("Framework/FontShaderFg");
		textMeshC.m_textMesh.GetComponent<Renderer>().material.SetColor("_Color", DebugDraw.HexToColor("ffffff"));
		textMeshC.m_textMesh.fontStyle = FontStyle.Italic;
		TextMeshC textMeshC2 = TextMeshS.AddComponent(transformC, new Vector3((0f - num3) / 2f * 0.96f + 0.06f, 5.02f, -0.3f), "KGSecondChances_Font", spriteC.width, spriteC.height, 22f, Align.Left, Align.Top, planetCamera, string.Empty);
		TextMeshS.SetText(textMeshC2, m_loop.GetName(), false);
		textMeshC2.m_textMesh.characterSize = 1f;
		textMeshC2.m_renderer.material.shader = Shader.Find("Framework/FontShaderFg");
		textMeshC2.m_textMesh.GetComponent<Renderer>().material.SetColor("_Color", DebugDraw.HexToColor("000000"));
		textMeshC2.m_textMesh.fontStyle = FontStyle.Italic;
		Frame frame2 = null;
		switch (m_loop.GetDifficulty())
		{
		case PsGameDifficulty.Easy:
			frame2 = PsState.m_uiSheet.m_atlas.GetFrame("menu_difficulty_casual");
			break;
		case PsGameDifficulty.Tricky:
			frame2 = PsState.m_uiSheet.m_atlas.GetFrame("menu_difficulty_tricky");
			break;
		case PsGameDifficulty.Insane:
			frame2 = PsState.m_uiSheet.m_atlas.GetFrame("menu_difficulty_hardcore");
			break;
		case PsGameDifficulty.Impossible:
			frame2 = PsState.m_uiSheet.m_atlas.GetFrame("menu_difficulty_impossible");
			break;
		}
		if (frame2 != null)
		{
			SpriteC spriteC2 = SpriteS.AddComponent(transformC, frame2, m_planetNodeSpriteSheet);
			SpriteS.SetDimensions(spriteC2, num2 * 0.5f * (frame2.width / frame2.height), num2 * 0.5f);
			SpriteS.SetColor(spriteC2, Color.white);
			SpriteS.SetOffset(spriteC2, new Vector3(num3 / 2f * 0.9f, num2 * (1f - num) / 2f + spriteC.height / 2f * 0.5f, -0.3f), 0f);
		}
		m_profileImage = new PsUIProfileImage(null, false, string.Empty, m_loop.GetFacebookId(), m_loop.GetGamecenterId(), -1, true, true);
		m_profileImage.SetSize(num2 * (1f - num) * 0.79f, num2 * (1f - num) * 0.79f, RelativeTo.World);
		m_profileImage.SetCamera(planetCamera, true, false);
		m_profileImage.Update();
		TransformS.ParentComponent(m_profileImage.m_TC, transformC);
		TransformS.SetPosition(m_profileImage.m_TC, new Vector3((0f - num3) / 2f + m_profileImage.m_actualWidth / 2f * 1.04f, (0f - num2) * 0.03f, -0.1f));
		TransformS.SetRotation(m_profileImage.m_TC, Vector3.forward * 4f);
		string text = m_loop.GetDescription();
		if (string.IsNullOrEmpty(text) || text.Equals("No Description"))
		{
			if (m_loop.m_gameMode is PsGameModeStarCollect)
			{
				text = PsStrings.Get(StringID.GAME_MODE_DESC_STARHUNT);
			}
			else if (m_loop.m_gameMode is PsGameModeRace)
			{
				text = PsStrings.Get(StringID.GAME_MODE_DESC_RACING);
			}
		}
		TextMeshC textMeshC3 = TextMeshS.AddComponent(transformC, new Vector3((0f - num3) / 3.7f, 1.4f, -0.1f), "HurmeRegular_Font", 300f, spriteC.height, 15f, Align.Left, Align.Top, planetCamera, string.Empty);
		text = TextMeshS.WrapText(textMeshC3, text, num3 * 7.5f, 2, true);
		TextMeshS.SetText(textMeshC3, text, false);
		textMeshC3.m_textMesh.characterSize = 1f;
		textMeshC3.m_renderer.material.shader = Shader.Find("Framework/FontShaderFg");
		textMeshC3.m_textMesh.GetComponent<Renderer>().material.SetColor("_Color", DebugDraw.HexToColor("000000"));
		SpriteS.ConvertSpritesToPrefabComponent(transformC, true);
		transformC.forceRotation = true;
		TransformS.SetRotation(transformC, planetCamera.transform.rotation.eulerAngles);
		TransformS.SetPosition(transformC, new Vector3(10.5f, 8f, 0f));
		TransformS.SetScale(m_bubbleScaleTC, Vector3.zero);
	}

	private void BubbleDrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] roundedRect = DebugDraw.GetRoundedRect(_c.m_actualWidth, _c.m_actualHeight, 0.1f * _c.m_actualHeight, 8, Vector2.zero);
		Vector2[] array = new Vector2[roundedRect.Length + 2];
		Array.Copy(roundedRect, 0, array, 0, 8);
		array[8] = new Vector2(roundedRect[8].x + 2f, roundedRect[8].y);
		array[9] = new Vector2(roundedRect[8].x + 1f, roundedRect[8].y - 2f);
		Array.Copy(roundedRect, 8, array, 10, roundedRect.Length - 8);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, new Vector3(0.2f, -0.2f, 0.3f), array, 0.4f, DebugDraw.HexToColor("#666666"), ResourceManager.GetMaterial("Framework/Line4Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, new Vector3(0.2f, -0.2f, 0.2f), array, DebugDraw.HexToUint("#666666"), DebugDraw.HexToUint("#666666"), ResourceManager.GetMaterial("Framework/SolidMat"), _c.m_camera, string.Empty);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 0.1f, array, 0.25f, DebugDraw.HexToColor("#fffec6"), ResourceManager.GetMaterial("Framework/Line8Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.zero, array, DebugDraw.HexToUint("#fffec6"), DebugDraw.HexToUint("#fffec6"), ResourceManager.GetMaterial("Framework/SolidMat"), _c.m_camera, string.Empty);
	}

	public override void Update()
	{
		base.Update();
		if (m_bubbleScaleTC != null)
		{
			float num = Vector3.Dot(m_tc.transform.position.normalized, Vector3.up);
			TransformS.SetPosition(m_unlockTC, new Vector3(0f, 14f + -6f * (1f - num), -10f));
			if (num > 0.9f || m_hideUI)
			{
				m_bubbleScaleTC.transform.localScale = Vector3.Lerp(m_bubbleScaleTC.transform.localScale, Vector3.zero, 0.3f);
			}
			else
			{
				m_bubbleScaleTC.transform.localScale = Vector3.Lerp(m_bubbleScaleTC.transform.localScale, Vector3.one, 0.3f);
			}
		}
		if (m_loop.m_createFlyingNode && m_loop.m_planet.m_floaterManager.m_floatersAlive)
		{
			CreateFlyingFriendlyNode();
			m_loop.m_createFlyingNode = false;
			m_loop.m_versusData = null;
		}
	}

	public void CreateFlyingFriendlyNode()
	{
		VersusMetaData versusData = m_loop.m_versusData;
		PsGameLoopFriendly psGameLoopFriendly = new PsGameLoopFriendly(PsMinigameContext.Friendly, versusData, versusData.timedEventId, versusData.gameId, PsPlanetManager.GetCurrentPlanet().m_floaterManager.m_floatingPath, PsPlanetManager.GetCurrentPlanet().m_floaterManager.m_floatingNodeList.Count + 1, PsPlanetManager.GetCurrentPlanet().m_floaterManager.m_floatingNodeList.Count + 1, 0, versusData.timeLeft, true);
		PsFloatingVersusNode item = new PsFloatingVersusFriendlyNode(psGameLoopFriendly);
		PsPlanetManager.GetCurrentPlanet().m_floaterManager.m_floatingPath.m_nodeInfos.Add(psGameLoopFriendly);
		PsPlanetManager.GetCurrentPlanet().m_floaterManager.m_floatingNodeList.Add(item);
		new PsPlanetFlyingFriendlyNode(m_loop, psGameLoopFriendly);
	}

	public override void Reveal()
	{
		base.Reveal();
		if (m_loop != null)
		{
			int num = m_loop.m_nodeId - m_loop.m_path.GetPrevBlockId(m_loop.m_nodeId);
			if (m_loop.m_path.m_startNodeId == 0)
			{
				SoundS.PlaySingleShotWithParameter("/Metagame/LevelAppear", Vector3.zero, "NodeNumber", num);
			}
		}
	}

	public override void Activate2(TimerC _c)
	{
		base.Activate2(_c);
		if (m_loop.m_path.m_startNodeId == 0)
		{
			SoundS.PlaySingleShot("/Metagame/LevelHighlight", Vector3.zero);
		}
	}

	public override void Destroy()
	{
		if (m_bubbleEntity != null)
		{
			EntityManager.RemoveEntity(m_bubbleEntity);
			m_bubbleEntity = null;
		}
		if (m_profileImage != null)
		{
			m_profileImage.Destroy();
			m_profileImage = null;
		}
		base.Destroy();
	}
}
