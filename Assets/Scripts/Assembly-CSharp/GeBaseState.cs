using System.Collections.Generic;
using UnityEngine;

public class GeBaseState : BasicState
{
	public GeScene m_parent;

	private TouchAreaC m_fullscreenTAC;

	private float m_startDistance;

	private float m_startDistanceMultipler;

	private Vector2 m_startZoom;

	private bool m_pinching;

	private float m_currentZoomPos;

	private TransformC m_multiSelectTC;

	private TLTouch m_multiSelectTouch;

	private Vector3 m_multiSelectStartPos;

	private static UIPagedCanvas m_pagedCanvas;

	private static UIVerticalList m_verticalList;

	private static UIHorizontalList m_horizontalArea;

	private static PsUIGenericButton m_loadLocalButton;

	private static PsUIGenericButton m_saveLocalButton;

	private static PsUIGenericButton m_loadServerButton;

	private static PsUIGenericButton m_saveServerButton;

	public static PsUIGenericButton m_testButton;

	public static PsUIGenericButton m_editButton;

	private static UIVerticalList m_quitList;

	private static PsUIGenericButton m_quitButton;

	public override void Enter(IStatedObject _parent)
	{
		m_parent = _parent as GeScene;
		m_quitList = new UIVerticalList(null, string.Empty);
		m_quitList.SetMargins(0.025f);
		m_quitList.SetAlign(0f, 1f);
		m_quitList.RemoveDrawHandler();
		m_quitButton = new PsUIGenericButton(m_quitList);
		m_quitButton.SetText("Quit", 0.04f);
		m_quitButton.SetHeight(0.06f, RelativeTo.ScreenHeight);
		m_quitList.Update();
		m_pagedCanvas = new UIPagedCanvas(null, string.Empty);
		m_pagedCanvas.SetWidth(0.25f, RelativeTo.ScreenHeight);
		m_pagedCanvas.SetHorizontalAlign(1f);
		m_pagedCanvas.RemoveDrawHandler();
		m_horizontalArea = new UIHorizontalList(m_pagedCanvas, "HorizontalArea");
		m_horizontalArea.SetHorizontalAlign(0f);
		m_horizontalArea.SetVerticalAlign(1f);
		UIScrollableCanvas uIScrollableCanvas = new UIScrollableCanvas(m_horizontalArea, string.Empty);
		uIScrollableCanvas.SetWidth(1f, RelativeTo.ParentWidth);
		uIScrollableCanvas.SetHeight(1f, RelativeTo.ParentHeight);
		m_verticalList = new UIVerticalList(uIScrollableCanvas, string.Empty);
		m_verticalList.RemoveDrawHandler();
		m_verticalList.SetWidth(1f, RelativeTo.ParentWidth);
		m_verticalList.SetMargins(0f, 0f, 0.025f, 0.025f);
		m_verticalList.SetAlign(0.5f, 1f);
		UpdateInfoUI(null);
		m_pagedCanvas.m_camera.clearFlags = CameraClearFlags.Color;
		m_pagedCanvas.m_camera.backgroundColor = new Color(0.9f, 0.9f, 0.9f);
		m_startZoom = Vector2.zero;
		m_startDistance = 0f;
		m_pinching = false;
		PsState.m_editorCameraPos = Vector3.zero;
		Entity entity = EntityManager.AddEntity();
		TransformC tc = TransformS.AddComponent(entity, "Editor Camera Target");
		PsState.m_editorCamTarget = CameraS.AddTargetComponent(tc, 1200f, 1200f, Vector2.zero);
		PsState.m_editorCamTarget.interpolateSpeed = 0.3333f;
		PsState.m_editorCamTarget.targetTC.transform.position = PsState.m_editorCameraPos;
		SetZoomAmount(PsState.m_editorCameraZoom);
		CameraS.SnapMainCameraFrame();
		entity = EntityManager.AddEntity();
		m_multiSelectTC = TransformS.AddComponent(entity);
		tc = TransformS.AddComponent(entity, Vector3.forward * 200f);
		tc.transform.name = "EditorCanvasTransform";
		float num = Mathf.Max(Screen.width, Screen.height);
		m_fullscreenTAC = TouchAreaS.AddRectArea(tc, "first", num, num, CameraS.m_uiCamera);
		TouchAreaS.AddTouchEventListener(m_fullscreenTAC, TouchHandler);
		m_fullscreenTAC.m_allowSecondary = false;
		m_fullscreenTAC.m_maxTouches = 2;
		m_fullscreenTAC.m_cancelOtherTouches = false;
	}

	public static void UpdateInfoUI(GeNode _node)
	{
		if (m_pagedCanvas == null)
		{
			return;
		}
		m_verticalList.DestroyChildren();
		m_loadLocalButton = null;
		m_saveLocalButton = null;
		m_loadServerButton = null;
		m_saveServerButton = null;
		if (_node == null)
		{
			if (m_pagedCanvas.GetCurrentPage() > 0)
			{
				m_pagedCanvas.GoToPage(0, true);
			}
			if (m_horizontalArea.m_childs.Count > 1)
			{
				m_horizontalArea.DestroyChildren(1);
			}
			m_testButton = null;
			UIText uIText = new UIText(m_verticalList, false, string.Empty, "Live server file", "HurmeSemiBold_Font", 0.02f, RelativeTo.ScreenHeight, "#000000");
			m_loadServerButton = new PsUIGenericButton(m_verticalList);
			m_loadServerButton.SetText("Load");
			m_loadServerButton.SetHeight(0.06f, RelativeTo.ScreenHeight);
			m_loadServerButton.SetOrangeColors();
			m_saveServerButton = new PsUIGenericButton(m_verticalList);
			m_saveServerButton.SetText("Save");
			m_saveServerButton.SetHeight(0.06f, RelativeTo.ScreenHeight);
			uIText = new UIText(m_verticalList, false, string.Empty, "Local file", "HurmeSemiBold_Font", 0.02f, RelativeTo.ScreenHeight, "#000000");
			m_loadLocalButton = new PsUIGenericButton(m_verticalList);
			m_loadLocalButton.SetText("Load");
			m_loadLocalButton.SetHeight(0.06f, RelativeTo.ScreenHeight);
			m_loadLocalButton.SetOrangeColors();
			m_saveLocalButton = new PsUIGenericButton(m_verticalList);
			m_saveLocalButton.SetText("Save");
			m_saveLocalButton.SetHeight(0.06f, RelativeTo.ScreenHeight);
		}
		else
		{
			_node.UpdateInfoUI(m_verticalList);
		}
		m_pagedCanvas.Update();
	}

	private void Import(List<PsUnlockable> _list, int _row = 0)
	{
	}

	public override void Execute()
	{
		if (PsState.m_currentTool == EditorTool.Camera && m_fullscreenTAC.m_touchCount == 0)
		{
			PsState.m_currentTool = EditorTool.None;
		}
		if (PsState.m_currentTool == EditorTool.MultiSelect && !Input.GetKey(KeyCode.LeftShift) && m_multiSelectTouch == null)
		{
			PsState.m_currentTool = EditorTool.None;
		}
		if (Input.GetKey(KeyCode.A))
		{
			SetZoomAmount(PsState.m_editorCameraZoom - 25f);
		}
		else if (Input.GetKey(KeyCode.Z))
		{
			SetZoomAmount(PsState.m_editorCameraZoom + 25f);
		}
		if (PsState.m_currentTool != EditorTool.None)
		{
			return;
		}
		if (Input.GetKey(KeyCode.LeftShift))
		{
			PsState.m_currentTool = EditorTool.MultiSelect;
		}
		if (m_loadServerButton != null && m_loadServerButton.m_hit)
		{
			m_parent.m_stateMachine.ChangeState(new GeLoadState());
		}
		else if (m_saveServerButton != null && m_saveServerButton.m_hit)
		{
			m_parent.m_stateMachine.ChangeState(new GeSaveState());
		}
		else if (m_loadLocalButton != null && m_loadLocalButton.m_hit)
		{
			m_parent.m_stateMachine.ChangeState(new GeLoadState(true));
		}
		else if (m_saveLocalButton != null && m_saveLocalButton.m_hit)
		{
			m_parent.m_stateMachine.ChangeState(new GeSaveState(true));
		}
		else if (m_quitButton != null && m_quitButton.m_hit)
		{
			GizmoManager.ClearSelection();
			GizmoManager.Update();
			Main.m_currentGame.m_sceneManager.ChangeScene(new StartupScene("StartupScene"));
		}
		else if (m_testButton != null && m_testButton.m_hit)
		{
			if (m_testButton.m_TAC.m_customObject != null)
			{
				GeNode geNode = m_testButton.m_TAC.m_customObject as GeNode;
				MinigameSearchParametres minigameSearchParametres = new MinigameSearchParametres();
				minigameSearchParametres.m_difficulty = geNode.m_metagameNodeData.m_levelDifficulty;
				minigameSearchParametres.m_features = new string[1] { geNode.m_metagameNodeData.m_levelSubgenre.ToString() };
				minigameSearchParametres.m_features = ((geNode.m_metagameNodeData.m_levelSubgenre == PsSubgenre.Any) ? null : new string[1] { geNode.m_metagameNodeData.m_levelSubgenre.ToString() });
				minigameSearchParametres.m_gameMode = geNode.m_metagameNodeData.m_levelGameMode;
				minigameSearchParametres.m_items = geNode.m_metagameNodeData.m_levelItems.ToArray();
				minigameSearchParametres.m_playerUnitFilter = geNode.m_metagameNodeData.m_levelPlayerUnit;
				PsGameLoopGeLevel psGameLoopGeLevel = new PsGameLoopGeLevel(geNode.m_metagameNodeData.m_levelMinigameId, minigameSearchParametres);
				psGameLoopGeLevel.StartLoop();
			}
		}
		else if (m_editButton != null && m_editButton.m_hit && m_editButton.m_TAC.m_customObject != null)
		{
			GeNode geNode2 = m_testButton.m_TAC.m_customObject as GeNode;
			PsGameLoopEditor psGameLoopEditor = new PsGameLoopEditor(geNode2.m_metagameNodeData.m_levelMinigameId, geNode2.m_metagameNodeData.m_levelMinigameMetaData);
			psGameLoopEditor.StartLoop();
		}
	}

	public override void Exit()
	{
		GizmoManager.ClearSelection();
		GizmoManager.Update();
		if (PsState.m_editorCamTarget != null)
		{
			EntityManager.RemoveEntity(PsState.m_editorCamTarget.p_entity);
			PsState.m_editorCamTarget = null;
		}
		if (m_pagedCanvas != null)
		{
			m_pagedCanvas.Destroy();
		}
		m_pagedCanvas = null;
		if (m_quitList != null)
		{
			m_quitList.Destroy();
		}
		m_quitList = null;
		EntityManager.RemoveEntity(m_fullscreenTAC.p_entity);
		m_fullscreenTAC = null;
	}

	public static void CreateTransformGizmo(GraphElement _graphElement)
	{
		GizmoManager.ClearSelection();
		GizmoManager.AddToSelection(_graphElement);
		_graphElement.Select(true);
		GizmoManager.Update();
	}

	public static void RemoveTransformGizmo()
	{
		GizmoManager.ClearSelection();
		GizmoManager.Update();
	}

	private void SetZoomAmount(float _amount)
	{
		float min = 200f;
		float max = 1500f;
		PsState.m_editorCameraZoom = ToolBox.limitBetween(_amount, min, max);
		m_currentZoomPos = ToolBox.getPositionBetween(_amount, min, max);
		CameraS.SetTargetBB(PsState.m_editorCamTarget, PsState.m_editorCameraZoom * 2f, PsState.m_editorCameraZoom * 2f);
	}

	private void SelectNodesOnArea(Vector3 _midPoint, float _width, float _height)
	{
		GizmoManager.ClearSelection();
		GraphNode currentLayer = LevelManager.m_currentLevel.m_currentLayer;
		for (int i = 0; i < currentLayer.m_childElements.Count; i++)
		{
			if (currentLayer.m_childElements[i].m_elementType == GraphElementType.Node)
			{
				Vector3 position = currentLayer.m_childElements[i].m_TC.transform.position;
				if (position.x >= _midPoint.x - _width * 0.5f && position.x <= _midPoint.x + _width * 0.5f && position.y >= _midPoint.y - _height * 0.5f && position.y <= _midPoint.y + _height * 0.5f)
				{
					GizmoManager.AddToSelection(currentLayer.m_childElements[i]);
				}
			}
		}
		GizmoManager.Update();
	}

	private void TouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		if (_touchIsSecondary)
		{
			return;
		}
		Level currentLevel = LevelManager.m_currentLevel;
		if (PsState.m_currentTool == EditorTool.Camera)
		{
			switch (_touchCount)
			{
			case 1:
				if (_touchArea.m_wasDragged)
				{
					Vector3 position2 = PsState.m_editorCamTarget.targetTC.transform.position;
					position2 -= (Vector3)_touches[0].m_deltaPosition / CameraS.m_mainCameraDistanceMultipler;
					PsState.m_editorCamTarget.targetTC.transform.position = position2;
					PsState.m_editorCameraPos = position2;
				}
				if (_touchPhase == TouchAreaPhase.ReleaseIn)
				{
					PsState.m_currentTool = EditorTool.None;
					m_pinching = false;
				}
				break;
			case 2:
				if (!_touchIsSecondary)
				{
					Vector3 vector = (_touches[0].m_deltaPosition + _touches[1].m_deltaPosition) * 0.5f;
					Vector2 vector2 = _touches[0].m_currentPosition - _touches[1].m_currentPosition;
					if (_touches[0].m_primaryPhase == TouchAreaPhase.Began || _touches[1].m_primaryPhase == TouchAreaPhase.Began)
					{
						cpBB targetFrame = PsState.m_editorCamTarget.targetFrame;
						m_startZoom = new Vector2(targetFrame.r - targetFrame.l, targetFrame.t - targetFrame.b);
						m_startDistance = (_touches[0].m_currentPosition - _touches[1].m_currentPosition).magnitude;
						m_startDistanceMultipler = CameraS.m_mainCameraDistanceMultipler;
						m_pinching = true;
					}
					if (m_pinching)
					{
						float num = (vector2.magnitude - m_startDistance) * 2f / m_startDistanceMultipler;
						float zoomAmount = (m_startZoom.x - num) * 0.5f;
						SetZoomAmount(zoomAmount);
					}
					if (_touchArea.m_wasDragged)
					{
						Vector3 position = PsState.m_editorCamTarget.targetTC.transform.position;
						position -= vector / CameraS.m_mainCameraDistanceMultipler;
						position.x = Mathf.Min((float)LevelManager.m_currentLevel.m_currentLayer.m_layerWidth * 0.5f, Mathf.Max((float)LevelManager.m_currentLevel.m_currentLayer.m_layerWidth * -0.5f, position.x));
						position.y = Mathf.Min((float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * 0.5f, Mathf.Max((float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * -0.5f, position.y));
						PsState.m_editorCamTarget.targetTC.transform.position = position;
						PsState.m_editorCameraPos = position;
					}
					if (_touchPhase == TouchAreaPhase.ReleaseIn)
					{
						PsState.m_currentTool = EditorTool.None;
						m_pinching = false;
					}
				}
				break;
			default:
				PsState.m_currentTool = EditorTool.None;
				break;
			}
		}
		else if (PsState.m_currentTool == EditorTool.MultiSelect)
		{
			if (_touchCount == 1)
			{
				if (_touchIsSecondary)
				{
					return;
				}
				if (m_multiSelectTouch == null && _touchPhase == TouchAreaPhase.Began)
				{
					m_multiSelectTouch = _touches[0];
					m_multiSelectStartPos = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, _touches[0].m_startPosition);
					m_multiSelectStartPos.z = 0f;
				}
			}
			if (m_multiSelectTouch != null)
			{
				Vector3 touchWorldPos = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, m_multiSelectTouch.m_currentPosition);
				Vector3 vector3 = m_multiSelectStartPos + (touchWorldPos - m_multiSelectStartPos) * 0.5f;
				vector3.z = 0f;
				float width = Mathf.Abs(touchWorldPos.x - m_multiSelectStartPos.x);
				float height = Mathf.Abs(touchWorldPos.y - m_multiSelectStartPos.y);
				DebugDraw.Clear(CameraS.m_mainCamera, m_multiSelectTC);
				DebugDraw.CreateBox(CameraS.m_mainCamera, m_multiSelectTC, vector3, width, height);
				if (m_multiSelectTouch.m_phase == TouchPhase.Ended || m_multiSelectTouch.m_phase == TouchPhase.Canceled)
				{
					SelectNodesOnArea(vector3, width, height);
					m_multiSelectTouch = null;
					DebugDraw.Clear(CameraS.m_mainCamera, m_multiSelectTC);
				}
			}
		}
		else
		{
			if (PsState.m_currentTool != EditorTool.None)
			{
				return;
			}
			switch (_touchCount)
			{
			case 2:
				if (!_touchIsSecondary && (_touches[0].m_primaryPhase == TouchAreaPhase.Began || _touches[1].m_primaryPhase == TouchAreaPhase.Began))
				{
					PsState.m_currentTool = EditorTool.Camera;
					cpBB targetFrame2 = PsState.m_editorCamTarget.targetFrame;
					m_startZoom = new Vector2(targetFrame2.r - targetFrame2.l, targetFrame2.t - targetFrame2.b);
					m_startDistance = (_touches[0].m_currentPosition - _touches[1].m_currentPosition).magnitude;
					m_startDistanceMultipler = CameraS.m_mainCameraDistanceMultipler;
					m_pinching = true;
				}
				break;
			case 1:
				if (_touchPhase == TouchAreaPhase.DragStart || (_touchArea.m_wasDragged && _touchPhase == TouchAreaPhase.MoveIn))
				{
					PsState.m_currentTool = EditorTool.Camera;
				}
				else
				{
					if (_touchPhase != TouchAreaPhase.ReleaseIn || _touchArea.m_wasDragged || _touchIsSecondary)
					{
						break;
					}
					bool flag = false;
					if (GizmoManager.GetSelection().Count > 0)
					{
						List<GraphElement> selection = GizmoManager.GetSelection();
						GizmoManager.ClearSelection();
						new SelectUndoAction(selection, GizmoManager.GetSelection());
						GizmoManager.Update();
						flag = true;
					}
					float radius = 25f * (1f / CameraS.m_mainCameraDistanceMultipler);
					Vector3 touchWorldPos2 = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, _touches[0].m_currentPosition);
					Vector3 position3 = CameraS.m_mainCamera.transform.position;
					Vector3 vector4 = touchWorldPos2 - position3;
					RaycastHit[] array = null;
					array = Physics.SphereCastAll(touchWorldPos2 - vector4.normalized * 500f, radius, vector4.normalized, 1000f, 1 << CameraS.m_mainCamera.gameObject.layer);
					TouchAreaC touchAreaC = null;
					float num2 = 999999f;
					for (int i = 0; i < array.Length; i++)
					{
						RaycastHit raycastHit = array[i];
						float magnitude = (touchWorldPos2 - raycastHit.transform.position).magnitude;
						if (magnitude < num2)
						{
							TouchAreaBootstrap touchAreaBootstrap = raycastHit.transform.GetComponent("TouchAreaBootstrap") as TouchAreaBootstrap;
							if (touchAreaBootstrap != null)
							{
								num2 = magnitude;
								touchAreaC = touchAreaBootstrap.m_TAC;
							}
						}
					}
					if (touchAreaC != null)
					{
						touchAreaC.m_touchCount = 1;
						_touches[0].m_primaryArea = touchAreaC;
						touchAreaC.d_TouchEventDelegate(touchAreaC, _touchPhase, _touchIsSecondary, 1, _touches);
						_touchArea.m_touchCount = 0;
						break;
					}
					if (!flag)
					{
						GraphNode graphNode = new GeNode(string.Empty, touchWorldPos2);
						LevelManager.m_currentLevel.m_currentLayer.AddElement(graphNode);
						graphNode.Assemble();
						GizmoManager.AddToSelection(graphNode);
						GizmoManager.Update();
					}
					List<GraphElement> selection2 = GizmoManager.GetSelection();
					if (selection2.Count == 1)
					{
						if (selection2[0].m_elementType == GraphElementType.Node)
						{
							GeNode node = selection2[0] as GeNode;
							UpdateInfoUI(node);
						}
						else
						{
							UpdateInfoUI(null);
						}
					}
					else if (selection2.Count == 0)
					{
						UpdateInfoUI(null);
					}
				}
				break;
			}
		}
	}
}
