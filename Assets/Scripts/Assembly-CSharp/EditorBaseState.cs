using System.Collections.Generic;
using Server;
using UnityEngine;

public class EditorBaseState : BasicState
{
	private const int BRUSH_Z_OFFSET = 90;

	private Vector2 brushXYOffset = new Vector2(16f, 16f);

	private TouchAreaC m_fullscreenTAC;

	private float m_startDistance;

	private float m_startDistanceMultipler;

	private Vector2 m_startZoom;

	private bool m_pinching;

	private bool m_drawing;

	private Vector2 m_brushPos;

	private AutoGeometryBrush m_dynamicBrush;

	private AutoGeometryBrush m_dynamicMaskBrush;

	private float m_lastBrushCameraDistance;

	private int m_lastBrushGroundIndex;

	private TransformC m_brushVisualTC;

	private SoundC m_brushSound;

	private float m_brushSoundVelocity;

	private float m_currentZoomPos;

	private TransformC m_drawFxTC;

	private PrefabC m_drawFx;

	private int m_everyplayPauseTicks;

	private bool m_recording;

	public override void Enter(IStatedObject _parent)
	{
		AutoGeometryManager.SetDirtyTileTracking(false);
		m_startZoom = Vector2.zero;
		m_startDistance = 0f;
		m_pinching = false;
		m_lastBrushGroundIndex = -1;
		Entity entity = EntityManager.AddEntity();
		TransformC tc = TransformS.AddComponent(entity, "Editor Camera Target");
		PsState.m_editorCamTarget = CameraS.AddTargetComponent(tc, 1200f, 1200f, Vector2.zero);
		PsState.m_editorCamTarget.interpolateSpeed = 0.3333f;
		PsState.m_editorCamTarget.targetTC.transform.position = PsState.m_editorCameraPos;
		SetZoomAmount(PsState.m_editorCameraZoom);
		entity = EntityManager.AddEntity();
		tc = TransformS.AddComponent(entity, Vector3.forward * 200f);
		tc.transform.name = "EditorCanvasTransform";
		float num = Mathf.Max(Screen.width, Screen.height);
		m_fullscreenTAC = TouchAreaS.AddRectArea(tc, "first", num, num, CameraS.m_uiCamera);
		TouchAreaS.AddTouchEventListener(m_fullscreenTAC, TouchHandler);
		m_fullscreenTAC.m_allowSecondary = false;
		m_fullscreenTAC.m_maxTouches = 2;
		m_fullscreenTAC.m_cancelOtherTouches = false;
		m_everyplayPauseTicks = 0;
		m_recording = true;
		if (1 == 0)
		{
		}
	}

	private void OverrideLevelData()
	{
		Debug.LogWarning("OVERRIDING LEVEL DATA");
		string id = PsState.m_activeGameLoop.m_minigameMetaData.id;
		if (string.IsNullOrEmpty(id))
		{
			Debug.LogWarning("OVERRIDE CANCELLED - level not saved before");
			return;
		}
		PsState.m_activeMinigame.SetLayerItems();
		byte[] array = ClientTools.CreateLevelZipWithoutMetadataAndScreenshot();
		if (array != null)
		{
			MiniGame.OverrideData(id, array, OverrideDataSUCCEED, OverrideDataFAILED);
		}
	}

	private void OverrideDataSUCCEED(HttpC _c)
	{
		Debug.LogWarning("OVERRIDE DATA SUCCEED");
	}

	private void OverrideDataFAILED(HttpC _c)
	{
		Debug.LogWarning("OVERRIDE DATA FAILED");
	}

	public override void Execute()
	{
		if (!m_recording)
		{
			m_everyplayPauseTicks++;
			EveryplayManager.SetPaused(true);
			if (m_everyplayPauseTicks >= 3)
			{
				m_recording = true;
				m_everyplayPauseTicks = 0;
			}
		}
		else
		{
			EveryplayManager.SetPaused(false);
			m_recording = false;
			m_everyplayPauseTicks = 0;
		}
		if (PsState.m_currentTool == EditorTool.Camera && m_fullscreenTAC.m_touchCount == 0)
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
	}

	private void BackToMenu()
	{
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
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

	private Vector3 CalcGroundBoundsForPos(Vector3 _campos)
	{
		float num = Mathf.Abs(PsState.m_editorCamTarget.targetFrame.b - PsState.m_editorCamTarget.targetFrame.t);
		_campos.y = Mathf.Max(_campos.y, (float)(-LevelManager.m_currentLevel.m_currentLayer.m_layerHeight) * 0.5f + num * 0.2f);
		return _campos;
	}

	private void SetZoomAmount(float _amount)
	{
		float min = 200f;
		float max = 1500f;
		PsState.m_editorCameraZoom = ToolBox.limitBetween(_amount, min, max);
		m_currentZoomPos = ToolBox.getPositionBetween(_amount, min, max);
		CameraS.SetTargetBB(PsState.m_editorCamTarget, PsState.m_editorCameraZoom * 2f, PsState.m_editorCameraZoom * 2f);
		PsState.m_editorCamTarget.targetTC.transform.position = CalcGroundBoundsForPos(PsState.m_editorCamTarget.targetTC.transform.position);
	}

	private void TouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		if (_touchIsSecondary)
		{
			return;
		}
		Minigame minigame = LevelManager.m_currentLevel as Minigame;
		AutoGeometryLayer autoGeometryLayer = minigame.m_groundNode.m_AGLayer[PsState.m_drawLayer];
		if (PsState.m_currentTool == EditorTool.Camera)
		{
			switch (_touchCount)
			{
			case 1:
				if (_touchArea.m_wasDragged)
				{
					Vector3 position2 = PsState.m_editorCamTarget.targetTC.transform.position;
					position2 -= (Vector3)_touches[0].m_deltaPosition / CameraS.m_mainCameraDistanceMultipler;
					position2 = BasicGizmo.LimitInsideDome(position2);
					position2 = CalcGroundBoundsForPos(position2);
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
		else if (PsState.m_currentTool == EditorTool.Paint)
		{
			Vector2 currentPosition = _touches[0].m_currentPosition;
			if (_touchPhase == TouchAreaPhase.Began || !m_drawing)
			{
				minigame.m_groundsModificationCount++;
				m_drawing = true;
				m_brushPos = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, currentPosition, 90f) + (Vector3)brushXYOffset;
				autoGeometryLayer.TakeSnapshot();
				autoGeometryLayer.ResetUndoRect();
				SoundS.PlaySingleShot("/UI/DrawBegin", Vector3.zero);
				m_brushSoundVelocity = 0f;
			}
			PaintWithBrush(currentPosition, 0.5f, true);
			if (_touchPhase == TouchAreaPhase.ReleaseIn || _touchPhase == TouchAreaPhase.ReleaseOut)
			{
				m_drawing = false;
				RemoveDrawFx();
				EntityManager.RemoveEntity(m_brushVisualTC.p_entity);
				m_brushVisualTC = null;
				SoundS.PlaySingleShot("/UI/DrawEnd", Vector3.zero);
				LevelManager.m_currentLevel.SetLayerItems();
			}
		}
		else
		{
			if (PsState.m_currentTool != EditorTool.None)
			{
				return;
			}
			if (m_drawing)
			{
				m_drawing = false;
				RemoveDrawFx();
				EntityManager.RemoveEntity(m_brushVisualTC.p_entity);
				m_brushVisualTC = null;
				SoundS.PlaySingleShot("/UI/DrawEnd", Vector3.zero);
				LevelManager.m_currentLevel.SetLayerItems();
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
					if (GizmoManager.GetSelection().Count > 0)
					{
						List<GraphElement> selection = GizmoManager.GetSelection();
						GizmoManager.ClearSelection();
						new SelectUndoAction(selection, GizmoManager.GetSelection());
						GizmoManager.Update();
					}
					float radius = 25f * (1f / CameraS.m_mainCameraDistanceMultipler);
					Vector3 touchWorldPos = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, _touches[0].m_currentPosition);
					Vector3 position3 = CameraS.m_mainCamera.transform.position;
					Vector3 vector3 = touchWorldPos - position3;
					RaycastHit[] array = null;
					array = Physics.SphereCastAll(touchWorldPos - vector3.normalized * 500f, radius, vector3.normalized, 1000f, 1 << CameraS.m_mainCamera.gameObject.layer);
					TouchAreaC touchAreaC = null;
					float num2 = 999999f;
					for (int i = 0; i < array.Length; i++)
					{
						RaycastHit raycastHit = array[i];
						float magnitude = (touchWorldPos - raycastHit.transform.position).magnitude;
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
					int layerAtWorldPos = AutoGeometryManager.GetLayerAtWorldPos(TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, _touches[0].m_currentPosition, 90f) + (Vector3)brushXYOffset * 0.5f);
					if (layerAtWorldPos < 0)
					{
						layerAtWorldPos = AutoGeometryManager.GetLayerAtWorldPos(TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, _touches[0].m_currentPosition, -210f) + (Vector3)brushXYOffset * 0.5f);
					}
					if (layerAtWorldPos >= 0 && PsState.m_activeGameLoop.m_gameMode.isMaterialAvailable(PsState.m_activeMinigame.GetMatIndexFromLayerIndex(layerAtWorldPos)))
					{
						PsState.m_drawLayer = layerAtWorldPos;
						PsState.m_drawMenuTargetPage = PsState.m_activeMinigame.GetMatIndexFromLayerIndex(layerAtWorldPos);
					}
				}
				break;
			}
		}
	}

	private void PaintWithBrush(Vector2 _touchPos, float _speed, bool _soften)
	{
		Minigame activeMinigame = PsState.m_activeMinigame;
		activeMinigame.m_changed = true;
		bool rectBrush = activeMinigame.m_groundNode.m_AGLayer[PsState.m_drawLayer].m_groundC.m_ground.m_rectBrush;
		if (Mathf.Abs(m_lastBrushCameraDistance - CameraS.m_mainCameraDistanceMultipler) > 0.01f || m_lastBrushGroundIndex != PsState.m_drawLayer)
		{
			m_lastBrushCameraDistance = CameraS.m_mainCameraDistanceMultipler;
			m_lastBrushGroundIndex = PsState.m_drawLayer;
			float brushSize = (rectBrush ? Mathf.Lerp(1.5f, 6.25f, m_currentZoomPos) : Mathf.Lerp(2.5f, 7.5f, m_currentZoomPos));
			if (m_dynamicBrush != null)
			{
				m_dynamicBrush.Destroy();
			}
			m_dynamicBrush = new AutoGeometryBrush(brushSize, rectBrush);
		}
		float maxStep = m_dynamicBrush.m_width;
		Vector2 vector = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, _touchPos, 90f);
		Vector2 vector2 = vector + brushXYOffset - m_brushPos;
		float max = 200f * _speed;
		vector2 = vector2.normalized * ToolBox.limitBetween(vector2.magnitude, 0f, max);
		if (m_brushVisualTC == null)
		{
			Entity entity = EntityManager.AddEntity();
			m_brushVisualTC = TransformS.AddComponent(entity);
			Vector2[] array = null;
			PrefabS.CreatePathPrefabComponentFromVectorArray(_points: (!rectBrush) ? DebugDraw.GetCircle(m_dynamicBrush.m_width * 8, 36, Vector2.zero, false) : DebugDraw.GetRoundedRect(m_dynamicBrush.m_width * 16, m_dynamicBrush.m_width * 16, 8f, 2, Vector2.zero, false), _tc: m_brushVisualTC, _offset: Vector3.zero, _width: 8f, _color: Color.white, _material: ResourceManager.GetMaterial("Framework/Line8Mat"), _camera: CameraS.m_mainCamera, _align: Position.Center, _closed: true);
			m_brushSound = SoundS.AddComponent(_event: (!PsState.m_addDown) ? "/UI/EraseLoop" : activeMinigame.m_groundNode.m_AGLayer[PsState.m_drawLayer].m_groundC.m_ground.m_drawSound, _parentTC: m_brushVisualTC);
			SoundS.SetSoundParameter(m_brushSound, "Velocity", 0f);
			SoundS.PlaySound(m_brushSound);
			StartDrawFx((Vector3)vector + Vector3.forward * -90f);
		}
		else
		{
			float positionBetween = ToolBox.getPositionBetween(vector2.magnitude, 0.1f, 20f);
			m_brushSoundVelocity += (positionBetween - m_brushSoundVelocity) * 0.25f;
			SoundS.SetSoundParameter(m_brushSound, "Velocity", m_brushSoundVelocity);
		}
		TransformS.SetPosition(m_brushVisualTC, (Vector3)vector + Vector3.forward * -90f);
		if (m_drawFx != null)
		{
			TransformS.SetPosition(m_drawFxTC, m_brushVisualTC.transform.localPosition);
		}
		if (vector2.magnitude > 1f)
		{
			m_brushPos = PaintLine(m_brushPos, m_brushPos + vector2, maxStep, _soften);
		}
	}

	private Vector2 PaintLine(Vector2 _startPos, Vector2 _endPos, float _maxStep, bool _soften)
	{
		Minigame minigame = LevelManager.m_currentLevel as Minigame;
		Vector2 vector = _endPos - _startPos;
		Vector2 vector2 = _startPos;
		int num = Mathf.FloorToInt(1f + vector.magnitude / _maxStep);
		Vector2 vector3 = vector / num;
		for (int i = 0; i < num; i++)
		{
			vector2 += vector3;
			AutoGeometryManager.PaintWithBrush(minigame.m_groundNode.m_AGLayer[PsState.m_drawLayer], m_dynamicBrush, vector2, PsState.m_addDown, m_dynamicBrush.m_subPixelAccuracy);
		}
		return vector2;
	}

	private void StartDrawFx(Vector3 _pos)
	{
		Minigame minigame = LevelManager.m_currentLevel as Minigame;
		string drawFx = minigame.m_groundNode.m_AGLayer[PsState.m_drawLayer].m_groundC.m_ground.m_drawFx;
		if (m_drawFx == null && drawFx != null)
		{
			Entity entity = EntityManager.AddEntity("GTAG_AUTODESTROY");
			m_drawFxTC = TransformS.AddComponent(entity, _pos);
			GameObject gameObject = ResourceManager.GetGameObject(drawFx);
			m_drawFx = PrefabS.AddComponent(m_drawFxTC, Vector3.zero, gameObject, drawFx, false);
		}
	}

	private void RemoveDrawFx()
	{
		if (m_drawFx != null)
		{
			TimerS.AddComponent(m_drawFxTC.p_entity, "RemoveTimerForDrawFx", 1f, 0f, true);
			m_drawFx.p_gameObject.GetComponent<ParticleSystem>().Stop(true);
			m_drawFx = null;
		}
	}

	public override void Exit()
	{
		if (PsState.m_editorCamTarget != null)
		{
			EntityManager.RemoveEntity(PsState.m_editorCamTarget.p_entity);
			PsState.m_editorCamTarget = null;
		}
		EntityManager.RemoveEntity(m_fullscreenTAC.p_entity);
		m_fullscreenTAC = null;
	}
}
