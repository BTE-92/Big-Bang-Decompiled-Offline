using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentGizmo : BasicGizmo
{
	public ParentGizmo(List<GraphElement> _graphElements)
		: base(_graphElements)
	{
		float num = (float)Screen.height / 768f * 100f;
		float num2 = num * 2f;
		float num3 = 0.6f * num2;
		float num4 = 1f * num2;
		float num5 = 0.05f * num2;
		float num6 = 1f * num;
		float num7 = 0.35f * num2;
		if (m_graphElements[0].m_isMoveable)
		{
			m_TAC = TouchAreaS.AddCircleArea(m_uiTC, "Move", num, CameraS.m_uiCamera);
			TouchAreaS.AddTouchEventListener(m_TAC, TouchHandler);
			SpriteC c = SpriteS.AddComponent(m_uiTC, PsState.m_uiSheet.m_atlas.GetFrame("hud_gizmo_selection_circle"), PsState.m_uiSheet);
			SpriteS.SetDimensions(c, num2, num2);
			SpriteS.ConvertSpritesToPrefabComponent(m_uiTC, true);
		}
		if (m_graphElements[0].m_isRotateable)
		{
			float num8 = 135f;
			TransformC transformC = TransformS.AddComponent(m_uiTC.p_entity, "GizmoRotateTC");
			TransformS.ParentComponent(transformC, m_uiTC, new Vector3(num6 * Mathf.Sin(num8 * ((float)Math.PI / 180f)), num6 * Mathf.Cos(num8 * ((float)Math.PI / 180f)), -10f));
			TouchAreaC c2 = TouchAreaS.AddCircleArea(transformC, "RotateZ", num7 * 0.5f, CameraS.m_uiCamera);
			TouchAreaS.AddTouchEventListener(c2, TouchHandler);
			SpriteC c3 = SpriteS.AddComponent(transformC, PsState.m_uiSheet.m_atlas.GetFrame("hud_gizmo_rotate"), PsState.m_uiSheet);
			SpriteS.SetDimensions(c3, num7, num7);
			SpriteS.ConvertSpritesToPrefabComponent(transformC, true);
		}
		if (m_graphElements[0].m_isCopyable)
		{
			float num9 = -135f;
			TransformC transformC2 = TransformS.AddComponent(m_uiTC.p_entity, "GizmoCopyTC");
			transformC2.forceRotation = true;
			TransformS.ParentComponent(transformC2, m_uiTC, new Vector3(num6 * Mathf.Sin(num9 * ((float)Math.PI / 180f)), num6 * Mathf.Cos(num9 * ((float)Math.PI / 180f)), -10f));
			TouchAreaC c4 = TouchAreaS.AddCircleArea(transformC2, "Copy", num7 * 0.5f, CameraS.m_uiCamera);
			TouchAreaS.AddTouchEventListener(c4, TouchHandler);
			SpriteC c5 = SpriteS.AddComponent(transformC2, PsState.m_uiSheet.m_atlas.GetFrame("hud_gizmo_copy"), PsState.m_uiSheet);
			SpriteS.SetDimensions(c5, num7, num7);
			SpriteS.ConvertSpritesToPrefabComponent(transformC2, true);
		}
		if (RemovableOrExtra(m_graphElements[0]))
		{
			float num10 = -45f;
			TransformC transformC3 = TransformS.AddComponent(m_uiTC.p_entity, "GizmoRemoveTC");
			transformC3.forceRotation = true;
			TransformS.ParentComponent(transformC3, m_uiTC, new Vector3(num6 * Mathf.Sin(num10 * ((float)Math.PI / 180f)), num6 * Mathf.Cos(num10 * ((float)Math.PI / 180f)), -10f));
			TouchAreaC c6 = TouchAreaS.AddCircleArea(transformC3, "Remove", num7 * 0.5f, CameraS.m_uiCamera);
			TouchAreaS.AddTouchEventListener(c6, TouchHandler);
			SpriteC c7 = SpriteS.AddComponent(transformC3, PsState.m_uiSheet.m_atlas.GetFrame("hud_gizmo_delete"), PsState.m_uiSheet);
			SpriteS.SetDimensions(c7, num7, num7);
			SpriteS.ConvertSpritesToPrefabComponent(transformC3, true);
		}
		if (m_graphElements[0].m_isFlippable)
		{
			float num11 = 70f;
			TransformC transformC4 = TransformS.AddComponent(m_uiTC.p_entity, "GizmoFlipTC");
			transformC4.forceRotation = false;
			TransformS.ParentComponent(transformC4, m_uiTC, new Vector3(num6 * Mathf.Sin(num11 * ((float)Math.PI / 180f)), num6 * Mathf.Cos(num11 * ((float)Math.PI / 180f)), -10f));
			TouchAreaC c8 = TouchAreaS.AddCircleArea(transformC4, "Flip", num7 * 0.5f, CameraS.m_uiCamera);
			TouchAreaS.AddTouchEventListener(c8, TouchHandler);
			SpriteC c9 = SpriteS.AddComponent(transformC4, PsState.m_uiSheet.m_atlas.GetFrame("hud_gizmo_flip"), PsState.m_uiSheet);
			SpriteS.SetDimensions(c9, num7, num7);
			SpriteS.ConvertSpritesToPrefabComponent(transformC4, true);
		}
		if (m_graphElements[0].m_isForegroundable)
		{
			float num12 = 15f;
			TransformC transformC5 = TransformS.AddComponent(m_uiTC.p_entity, "GizmoFgTC");
			transformC5.forceRotation = true;
			TransformS.ParentComponent(transformC5, m_uiTC, new Vector3(num6 * Mathf.Sin(num12 * ((float)Math.PI / 180f)), num6 * Mathf.Cos(num12 * ((float)Math.PI / 180f)), -10f));
			TouchAreaC c10 = TouchAreaS.AddCircleArea(transformC5, "Foreground", num7 * 0.5f, CameraS.m_uiCamera);
			TouchAreaS.AddTouchEventListener(c10, TouchHandler);
			SpriteC c11 = SpriteS.AddComponent(transformC5, PsState.m_uiSheet.m_atlas.GetFrame("hud_gizmo_depth"), PsState.m_uiSheet);
			SpriteS.SetDimensions(c11, num7, num7);
			SpriteS.ConvertSpritesToPrefabComponent(transformC5, true);
		}
		if (m_graphElements[0].m_isModifiable)
		{
		}
		if (m_graphElements[0].m_isReplaceable)
		{
		}
		if (m_graphElements[0].m_isLinkable)
		{
			float num13 = 0f;
			TransformC transformC6 = TransformS.AddComponent(m_uiTC.p_entity, "GizmoLinkTC");
			transformC6.forceRotation = false;
			TransformS.ParentComponent(transformC6, m_uiTC, new Vector3(num6 * Mathf.Sin(num13 * ((float)Math.PI / 180f)), num6 * Mathf.Cos(num13 * ((float)Math.PI / 180f)), -10f));
			TouchAreaC c12 = TouchAreaS.AddCircleArea(transformC6, "Link", num7 * 0.5f, CameraS.m_uiCamera);
			TouchAreaS.AddTouchEventListener(c12, TouchHandler);
			SpriteC c13 = SpriteS.AddComponent(transformC6, PsState.m_uiSheet.m_atlas.GetFrame("hud_gizmo_flip"), PsState.m_uiSheet);
			SpriteS.SetDimensions(c13, num7, num7);
			SpriteS.ConvertSpritesToPrefabComponent(transformC6, true);
		}
	}

	public override void TouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		if (_touchIsSecondary || m_uiTC == null || _touchCount != 1)
		{
			return;
		}
		if (_touchArea.m_name == "Link")
		{
			if (_touchPhase == TouchAreaPhase.Began || _touchPhase == TouchAreaPhase.DragStart)
			{
				if (!m_readyToLink)
				{
					Vector3 vector = _touches[0].m_currentPosition;
					vector.x -= (float)Screen.width * 0.5f;
					vector.y -= (float)Screen.height * 0.5f;
					vector.z = -10f;
					m_touchOffset = m_uiTC.transform.position - vector;
					m_readyToLink = true;
					m_startPositions = new List<Vector3>();
					for (int i = 0; i < m_graphElements.Count; i++)
					{
						m_startPositions.Add(m_graphElements[i].m_position);
					}
					EntityManager.SetVisibilityOfEntity(m_uiTC.p_entity, false);
				}
			}
			else if (m_readyToLink && (_touchPhase == TouchAreaPhase.MoveIn || _touchPhase == TouchAreaPhase.MoveOut || _touchPhase == TouchAreaPhase.StationaryIn || _touchPhase == TouchAreaPhase.StationaryOut))
			{
				TLTouch tLTouch = _touches[0];
				Vector3 vector2 = (Vector3)tLTouch.m_currentPosition + m_touchOffset;
				vector2.x -= (float)Screen.width * 0.5f;
				vector2.y -= (float)Screen.height * 0.5f;
				vector2.z = -10f;
				DragCameraAtBorders();
				Vector3 vector3 = vector2 - m_uiTC.transform.position;
				Vector3 vector4 = vector3 * 0.382f;
				Vector3 touchWorldPos = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, _touches[0].m_currentPosition);
				while (m_linkPrefabC.Count > 0)
				{
					int index = m_linkPrefabC.Count - 1;
					PrefabS.RemoveComponent(m_linkPrefabC[index]);
					m_linkPrefabC.RemoveAt(index);
				}
				for (int j = 0; j < m_graphElements.Count; j++)
				{
					Vector2[] array = new Vector2[2]
					{
						Vector2.zero,
						touchWorldPos - m_graphElements[j].m_TC.transform.position
					};
					Vector2 normalized = array[1].normalized;
					array[0] = normalized * m_graphElements[j].m_radius;
					Vector2 vector5 = array[1] - normalized * 10f;
					float f = Mathf.Atan2(0f - normalized.y, normalized.x);
					float x = Mathf.Sin(f);
					float y = Mathf.Cos(f);
					Vector2 normalized2 = new Vector2(x, y).normalized;
					Vector2[] points = new Vector2[2]
					{
						vector5 + normalized2 * 10f,
						array[1]
					};
					Vector2[] points2 = new Vector2[2]
					{
						vector5 - normalized2 * 10f,
						array[1]
					};
					m_linkPrefabC.Add(PrefabS.CreatePathPrefabComponentFromVectorArray(m_graphElements[j].m_TC, Vector3.zero, array, 4f, Color.white, ResourceManager.GetMaterial("Framework/Line8Mat"), CameraS.m_mainCamera, Position.Center, false));
					m_linkPrefabC.Add(PrefabS.CreatePathPrefabComponentFromVectorArray(m_graphElements[j].m_TC, Vector3.zero, points, 4f, Color.white, ResourceManager.GetMaterial("Framework/Line8Mat"), CameraS.m_mainCamera, Position.Center, false));
					m_linkPrefabC.Add(PrefabS.CreatePathPrefabComponentFromVectorArray(m_graphElements[j].m_TC, Vector3.zero, points2, 4f, Color.white, ResourceManager.GetMaterial("Framework/Line8Mat"), CameraS.m_mainCamera, Position.Center, false));
				}
				List<GraphElement> childElements = LevelManager.m_currentLevel.m_currentLayer.m_childElements;
				for (int k = 0; k < childElements.Count; k++)
				{
					bool flag = false;
					for (int l = 0; l < m_graphElements.Count; l++)
					{
						if (childElements[k].m_elementType != GraphElementType.Node || childElements[k] == m_graphElements[l])
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						continue;
					}
					float sqrMagnitude = (touchWorldPos - childElements[k].m_TC.transform.position).sqrMagnitude;
					if (sqrMagnitude < 2500f)
					{
						if (m_linkTarget == null)
						{
							m_linkTarget = childElements[k];
						}
						else if (sqrMagnitude < (touchWorldPos - m_linkTarget.m_TC.transform.position).sqrMagnitude)
						{
							m_linkTarget.Select(false);
							m_linkTarget = childElements[k];
						}
					}
					else if (childElements[k] == m_linkTarget)
					{
						childElements[k].Select(false);
						m_linkTarget = null;
					}
				}
				if (m_linkTarget != null)
				{
					m_linkTarget.Select(true);
				}
			}
			else
			{
				if (_touchPhase == TouchAreaPhase.DragEnd || (_touchPhase != TouchAreaPhase.ReleaseIn && _touchPhase != TouchAreaPhase.ReleaseOut))
				{
					return;
				}
				if (m_linkTarget != null)
				{
					for (int m = 0; m < m_graphElements.Count; m++)
					{
						GraphConnection graphConnection = new GeConnection(m_graphElements[m], m_linkTarget);
						LevelManager.m_currentLevel.m_currentLayer.AddElement(graphConnection);
						graphConnection.Assemble();
						m_graphElements[m].Linked();
					}
					m_linkTarget.Select(false);
					m_linkTarget.Linked();
					m_linkTarget = null;
					LevelManager.m_currentLevel.ItemChanged();
				}
				m_readyToLink = false;
				while (m_linkPrefabC.Count > 0)
				{
					int index2 = m_linkPrefabC.Count - 1;
					PrefabS.RemoveComponent(m_linkPrefabC[index2]);
					m_linkPrefabC.RemoveAt(index2);
				}
				GizmoManager.ClearSelection();
				GizmoManager.Update();
			}
		}
		else if (_touchArea.m_name == "Copy")
		{
			if (_touchPhase != TouchAreaPhase.ReleaseIn)
			{
				return;
			}
			List<GraphElement> list = new List<GraphElement>();
			List<GraphElement> list2 = new List<GraphElement>();
			List<GraphElement> parents = GizmoManager.GetParents(m_graphElements);
			if ((parents[0] as GraphNode).m_assembleClassType != null && !PsMetagameData.CanPlaceUnit((parents[0] as GraphNode).m_assembleClassType.ToString()))
			{
				return;
			}
			for (int n = 0; n < parents.Count; n++)
			{
				list.Add(parents[n]);
				GraphElement graphElement = parents[n].DeepCopy();
				graphElement.m_id = LevelManager.GetUniqueId();
				Vector3 vector6 = Vector3.right + Vector3.down;
				Vector3 vector7 = vector6 * UnityEngine.Random.Range(0f, 0.2f);
				graphElement.m_position += vector6 * 15f + vector7;
				GraphNode graphNode = graphElement as GraphNode;
				if (graphNode.m_childElements.Count != 0)
				{
					for (int num = 0; num < graphNode.m_childElements.Count; num++)
					{
						graphNode.m_childElements[num].m_position += vector6 * 15f + vector7;
					}
				}
				parents[n].m_parentElement.AddElement(graphElement);
				graphElement.Assemble();
				graphElement.Copied();
				list2.Add(graphElement);
			}
			LevelManager.m_currentLevel.ItemChanged();
			new CopyUndoAction(list.ToArray(), list2.ToArray());
			GizmoManager.ClearSelection();
			GizmoManager.AddToSelection(list2);
			GizmoManager.Update();
			SoundS.PlaySingleShot("/UI/ItemCreate", Vector3.zero);
		}
		else if (_touchArea.m_name == "Remove")
		{
			if (_touchPhase == TouchAreaPhase.ReleaseIn)
			{
				List<GraphElement> list3 = new List<GraphElement>();
				List<GraphElement> selection = GizmoManager.GetSelection();
				while (selection.Count > 0)
				{
					int index3 = selection.Count - 1;
					selection[index3].Removed();
					list3.Add(selection[index3].DeepCopy());
					selection[index3].Dispose();
					selection.RemoveAt(index3);
				}
				GizmoManager.ClearSelection();
				GizmoManager.Update();
				LevelManager.m_currentLevel.ItemChanged();
				new RemoveUndoAction(list3.ToArray());
				SoundS.PlaySingleShot("/UI/ItemDelete", Vector3.zero);
			}
		}
		else if (_touchArea.m_name == "Flip")
		{
			if (_touchPhase == TouchAreaPhase.ReleaseIn)
			{
				List<GraphElement> list4 = new List<GraphElement>();
				List<GraphElement> parents2 = GizmoManager.GetParents(m_graphElements);
				for (int num2 = 0; num2 < parents2.Count; num2++)
				{
					parents2[num2].Flipped();
					parents2[num2].m_flipped = !parents2[num2].m_flipped;
					parents2[num2].Reset();
					list4.Add(parents2[num2]);
				}
				GizmoManager.Update();
				LevelManager.m_currentLevel.ItemChanged();
				new FlipUndoAction(list4.ToArray());
				SoundS.PlaySingleShot("/UI/ItemFlip", Vector3.zero);
			}
		}
		else if (_touchArea.m_name == "Foreground")
		{
			if (_touchPhase == TouchAreaPhase.ReleaseIn)
			{
				List<GraphElement> list5 = new List<GraphElement>();
				List<GraphElement> parents3 = GizmoManager.GetParents(m_graphElements);
				for (int num3 = 0; num3 < parents3.Count; num3++)
				{
					parents3[num3].Foregrounded();
					parents3[num3].m_inFront = !parents3[num3].m_inFront;
					parents3[num3].Reset();
					list5.Add(parents3[num3]);
				}
				GizmoManager.Update();
				LevelManager.m_currentLevel.ItemChanged();
				new FlipUndoAction(list5.ToArray());
				SoundS.PlaySingleShot("/UI/ItemFlip", Vector3.zero);
			}
		}
		else if (_touchArea.m_name == "Move")
		{
			if (_touchPhase == TouchAreaPhase.Began || _touchPhase == TouchAreaPhase.DragStart)
			{
				if (!m_readyToMove)
				{
					Vector3 vector8 = _touches[0].m_currentPosition;
					vector8.x -= (float)Screen.width * 0.5f;
					vector8.y -= (float)Screen.height * 0.5f;
					vector8.z = -10f;
					m_touchOffset = m_uiTC.transform.position - vector8;
					m_readyToMove = true;
					m_startPositions = new List<Vector3>();
					for (int num4 = 0; num4 < m_graphElements.Count; num4++)
					{
						m_startPositions.Add(m_graphElements[num4].m_position);
					}
				}
			}
			else if (m_readyToMove && (_touchPhase == TouchAreaPhase.MoveIn || _touchPhase == TouchAreaPhase.MoveOut || _touchPhase == TouchAreaPhase.StationaryIn || _touchPhase == TouchAreaPhase.StationaryOut))
			{
				Vector3 vector9 = (Vector3)_touches[0].m_currentPosition + m_touchOffset;
				vector9.x -= (float)Screen.width * 0.5f;
				vector9.y -= (float)Screen.height * 0.5f;
				vector9.z = -10f;
				DragCameraAtBorders();
				Vector3 vector10 = vector9 - m_uiTC.transform.position;
				Vector3 step = vector10 * 0.382f;
				TransformS.GlobalMove(m_uiTC, step);
				Vector3 touchWorldPos2 = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, m_uiTC.transform.position + new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f), 0f - m_tacTCDepth);
				touchWorldPos2 = BasicGizmo.LimitInsideDome(touchWorldPos2);
				Vector3 vector11 = m_worldTC.transform.position - touchWorldPos2;
				MoveGizmo(touchWorldPos2);
				for (int num5 = 0; num5 < GizmoManager.m_childGizmos.Count; num5++)
				{
					bool flag2 = true;
					for (int num6 = 0; num6 < GizmoManager.m_childGizmos[num5].m_graphElements.Count; num6++)
					{
						GraphNode graphNode2 = GizmoManager.m_childGizmos[num5].m_graphElements[num6] as GraphNode;
						if (!graphNode2.m_parentElement.m_moveChilds)
						{
							flag2 = false;
						}
					}
					if (flag2)
					{
						Vector3 worldPos = GizmoManager.m_childGizmos[num5].m_worldTC.transform.position - vector11;
						GizmoManager.m_childGizmos[num5].MoveGizmo(worldPos);
					}
				}
			}
			else if (_touchPhase != TouchAreaPhase.DragEnd && (_touchPhase == TouchAreaPhase.ReleaseIn || _touchPhase == TouchAreaPhase.ReleaseOut))
			{
				LevelManager.m_currentLevel.ItemChanged();
				new MoveUndoAction(m_graphElements, m_startPositions);
				m_readyToMove = false;
			}
		}
		else
		{
			if (!(_touchArea.m_name == "RotateZ"))
			{
				return;
			}
			Vector3 touchWorldPos3 = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, m_uiTC.transform.position + new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f));
			Vector3 vector12 = _touches[0].m_currentPosition;
			Vector3 touchWorldPos4 = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, vector12);
			switch (_touchPhase)
			{
			case TouchAreaPhase.Began:
			{
				m_startRotations = new List<Vector3>();
				for (int num13 = 0; num13 < m_graphElements.Count; num13++)
				{
					m_startRotations.Add(m_graphElements[num13].m_rotation);
				}
				Vector2 vector14 = touchWorldPos4 - touchWorldPos3;
				float startAngle = Mathf.Atan2(vector14.y, vector14.x) * 57.29578f;
				m_startAngle = startAngle;
				return;
			}
			case TouchAreaPhase.MoveIn:
			case TouchAreaPhase.MoveOut:
			case TouchAreaPhase.StationaryIn:
			case TouchAreaPhase.StationaryOut:
			{
				if (m_graphElements.Count != m_startRotations.Count)
				{
					break;
				}
				Vector2 vector13 = touchWorldPos4 - touchWorldPos3;
				float num7 = Mathf.Atan2(vector13.y, vector13.x) * 57.29578f;
				for (int num8 = 0; num8 < m_graphElements.Count; num8++)
				{
					GraphElement graphElement2 = m_graphElements[num8];
					graphElement2.m_rotation = Vector3.forward * (num7 - m_startAngle) + m_startRotations[num8];
					TransformS.SetGlobalRotation(graphElement2.m_TC, graphElement2.m_rotation);
					for (int num9 = 0; num9 < graphElement2.m_assembledClasses.Count; num9++)
					{
						IAssembledClass assembledClass = graphElement2.m_assembledClasses[num9];
						for (int num10 = 0; num10 < assembledClass.m_assembledEntities.Count; num10++)
						{
							List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.ChipmunkBody, assembledClass.m_assembledEntities[num10]);
							for (int num11 = 0; num11 < componentsByEntity.Count; num11++)
							{
								ChipmunkBodyC chipmunkBodyC = componentsByEntity[num11] as ChipmunkBodyC;
								ChipmunkProWrapper.ucpBodySetAngle(chipmunkBodyC.body, ToolBox.getCappedAngle(graphElement2.m_rotation.z) * ((float)Math.PI / 180f));
							}
							List<IComponent> componentsByEntity2 = EntityManager.GetComponentsByEntity(ComponentType.Transform, assembledClass.m_assembledEntities[num10]);
							for (int num12 = 0; num12 < componentsByEntity2.Count; num12++)
							{
								TransformC c = componentsByEntity2[num12] as TransformC;
								TransformS.SetGlobalRotation(c, graphElement2.m_rotation);
							}
						}
					}
				}
				return;
			}
			}
			if (_touchPhase == TouchAreaPhase.ReleaseIn || _touchPhase == TouchAreaPhase.ReleaseOut)
			{
				LevelManager.m_currentLevel.ItemChanged();
				new RotateUndoAction(m_graphElements, m_startRotations);
			}
		}
	}

	public static bool CanRemoveExtra(GraphElement e)
	{
		GraphNode graphNode = e as GraphNode;
		if (graphNode != null && graphNode.m_assembleClassType != null)
		{
			string text = graphNode.m_assembleClassType.ToString();
			Hashtable itemCount = PsState.m_activeMinigame.m_itemCount;
			if (itemCount != null && itemCount.ContainsKey(text))
			{
				int num;
				if (text == "Goal")
				{
					num = 1;
				}
				else
				{
					if (!(text == "CollectibleStar"))
					{
						goto IL_00c4;
					}
					num = ((PsState.m_activeGameLoop.GetGameMode() == PsGameMode.StarCollect) ? 3 : 0);
				}
				return (int)itemCount[text] > num;
			}
		}
		goto IL_00c4;
		IL_00c4:
		return false;
	}

	public static bool RemovableOrExtra(GraphElement e)
	{
		if (!e.m_isRemovable)
		{
			return CanRemoveExtra(e);
		}
		return true;
	}
}
