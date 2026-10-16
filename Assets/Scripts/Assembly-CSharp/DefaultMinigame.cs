using System;
using UnityEngine;

public static class DefaultMinigame
{
	public static void ApplyWizard(Minigame _minigame = null)
	{
		if (string.IsNullOrEmpty(PsState.m_wizardPlayer))
		{
			PsState.m_wizardPlayer = "Motorcycle";
			PsState.m_wizardGameMode = "Race";
			int wizardDomeIndex = 0;
			foreach (PsEditorItem item in PsMetagameData.m_gameAreas[0].m_items)
			{
				if (item.m_unlocked)
				{
					if (item.m_identifier == "AreaMedium")
					{
						wizardDomeIndex = 1;
					}
					else if (item.m_identifier == "AreaLarge")
					{
						wizardDomeIndex = 2;
					}
				}
			}
			PsState.m_wizardDomeIndex = wizardDomeIndex;
		}
		Minigame minigame = LevelManager.m_currentLevel as Minigame;
		if (_minigame != null)
		{
			minigame = (Minigame)(LevelManager.m_currentLevel = _minigame);
		}
		minigame.m_groundNode = new LevelGroundNode();
		LevelManager.m_currentLevel.m_currentLayer.AddElement(minigame.m_groundNode);
		Vector2 vector = PsState.m_domeSizes[PsState.m_wizardDomeIndex] * 2048f;
		Vector2 vector2 = new Vector2((0f - vector.x) * 0.5f + 1200f, (float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * -0.5f + 90f);
		GraphNode graphNode = new GraphNode(_pos: new Vector2(vector.x * 0.5f - 1200f, (float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * -0.5f + 90f) + Vector2.right * 500f, _nodeType: GraphNodeType.Normal, _assembleClassType: typeof(Goal), _name: "Goal", _rot: Vector3.zero, _sca: Vector3.one);
		LevelPlayerNode levelPlayerNode = new LevelPlayerNode(Type.GetType(PsState.m_wizardPlayer), "Player", vector2 + Vector2.right * -500f, Vector3.zero, Vector3.one);
		LevelManager.AssembleCurrentLevel();
		minigame.UpdateAreaLimits(PsState.m_wizardDomeIndex);
		AutoGeometryManager.m_layers[0].PregenerateRandomGround(20f);
		AutoGeometryManager.m_layers[0].m_bytes.CopyTo(minigame.m_groundNode.m_AGLayerData[0], 0);
		AutoGeometryManager.m_layers[0].MarchTiles(new cpBB(0f, 0f, AutoGeometryManager.m_width, AutoGeometryManager.m_height));
		AutoGeometryManager.m_layers[0].TakeSnapshot();
		AutoGeometryManager.m_layers[0].CopyByteArrayToMaskTexture(AutoGeometryManager.m_layers[0].m_maskTexture, AutoGeometryManager.m_layers[0].m_bytes);
		AutoGeometryManager.UpdateMaxValueLookupTable(AutoGeometryManager.m_layers[0], LookupUpdateMode.IN_EDITOR);
		PsState.m_activeMinigame.SetGameMode(PsState.m_wizardGameMode, vector2, true);
		ucpSegmentQueryInfo info = default(ucpSegmentQueryInfo);
		ucpShapeFilter filter = default(ucpShapeFilter);
		filter.ucpShapeFilterAll();
		graphNode.m_position.y = (float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * -0.5f + 500f;
		ChipmunkProWrapper.ucpSpaceSegmentQueryFirst(graphNode.m_position, graphNode.m_position + new Vector3(0f, -10000f, 0f), 1f, filter, ref info);
		graphNode.m_position.y += -10000f * info.alpha + 90f;
		levelPlayerNode.m_position.y = (float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * -0.5f + 500f;
		ChipmunkProWrapper.ucpSpaceSegmentQueryFirst(levelPlayerNode.m_position, levelPlayerNode.m_position + new Vector3(0f, -10000f, 0f), 1f, filter, ref info);
		levelPlayerNode.m_position.y += -10000f * info.alpha + 90f;
		LevelManager.m_currentLevel.m_currentLayer.AddElement(levelPlayerNode);
		LevelManager.m_currentLevel.m_currentLayer.AddElement(graphNode);
		graphNode.Assemble();
		levelPlayerNode.Assemble();
		PsState.m_editorCameraPos = vector2 + Vector2.up * 200f + new Vector2(-200f, 0f);
		PsState.m_editorCameraZoom = 1500f;
		Debug.Log(PsState.m_wizardPlayer + " " + PsState.m_wizardGameMode + " " + PsState.m_wizardDomeIndex);
	}

	public static Minigame Assemble()
	{
		Debug.LogWarning("DEFAULT MINIGAME SETTING DOME SIZE INDEX TO 0");
		Minigame minigame = LevelManager.NewLevel(new Minigame((int)(2048f * PsState.m_editorAreaSize.x) + 256, (int)(2048f * PsState.m_editorAreaSize.y) + 256)) as Minigame;
		minigame.m_settings.Add("environment", "default");
		int num = 0;
		foreach (PsEditorItem item in PsMetagameData.m_gameAreas[0].m_items)
		{
			if (item.m_unlocked)
			{
				if (item.m_identifier == "AreaMedium")
				{
					num = 1;
				}
				else if (item.m_identifier == "AreaLarge")
				{
					num = 2;
				}
			}
		}
		minigame.m_settings.Add("domeSizeIndex", num);
		minigame.m_settings.Add("bgSeed", Main.m_gameTicks);
		PsMetrics.LevelCreationStarted();
		return minigame;
	}
}
