using System;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : Unit
{
	private const int RADIUS = 45;

	private ChipmunkBodyC m_body;

	private ucpShape m_shape;

	private TransformC m_tc;

	private string m_sensorText;

	private TextModel m_textModel;

	public static Dictionary<string, Action> m_listeners;

	public Sensor(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		if (PsState.m_activeGameLoop != null && PsState.m_activeGameLoop.m_nodeId == 3)
		{
			Action callBack = delegate
			{
				PsState.m_activeMinigame.TweenTimeScale(0.3f, TweenStyle.ExpoIn, 0.3f, delegate
				{
					PsState.m_activeMinigame.TweenTimeScale(1f, TweenStyle.ExpoIn, 0.4f);
				}, 5f);
			};
			Action callBack2 = delegate
			{
				PsState.m_activeMinigame.TweenTimeScale(0.3f, TweenStyle.ExpoIn, 0.3f, delegate
				{
					PsState.m_activeMinigame.TweenTimeScale(1f, TweenStyle.ExpoIn, 0.4f);
				}, 6f);
			};
			AddListener("backflip", callBack);
			AddListener("frontflip", callBack2);
		}
		m_graphElement.m_name = "Sensor";
		m_graphElement.m_isCopyable = true;
		m_graphElement.m_isRemovable = true;
		m_graphElement.m_isFlippable = true;
		m_graphElement.m_isRotateable = false;
		m_tc = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(m_tc, _graphElement.m_position + new Vector3(0f, 0f, 50f), _graphElement.m_rotation);
		m_shape = new ucpCircleShape(45f, Vector2.zero, 17895696u, 0f, 0f, 0f, (ucpCollisionType)10, true);
		m_body = ChipmunkProS.AddStaticBody(m_tc, m_shape);
		if (!m_minigame.m_editing)
		{
			ChipmunkProS.AddCollisionHandler(m_body, CollisionHandler, (ucpCollisionType)10, (ucpCollisionType)3, true, false, false);
		}
		else
		{
			Vector2[] circle = DebugDraw.GetCircle(45f, 32, Vector2.zero);
			PrefabS.CreatePathPrefabComponentFromVectorArray(m_tc, Vector3.zero, circle, 4f, Color.yellow, ResourceManager.GetMaterial("Framework/Line8Mat"), CameraS.m_mainCamera, Position.Center, true);
		}
		CreateEditorTouchArea(45f, 45f);
	}

	public override void CreateEditorTouchArea(float _width, float _height, TransformC _parentTC = null)
	{
		if (m_minigame.m_editing)
		{
			CreateGraphElementTouchArea(_width, _parentTC);
		}
	}

	protected void CollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		string text = (m_graphElement as LevelTextNode).m_text;
		if (m_listeners != null && m_listeners.ContainsKey(text))
		{
			m_listeners[text]();
			m_listeners.Remove(text);
		}
	}

	public override void Update()
	{
		if (!m_minigame.m_editing)
		{
			return;
		}
		if (m_graphElement.m_flipped)
		{
			m_graphElement.m_flipped = false;
			m_sensorText = (m_graphElement as LevelTextNode).m_text;
			m_textModel = new TextModel((m_graphElement as LevelTextNode).m_text);
			PsUICenterTutorialSignTextInput psUICenterTutorialSignTextInput = new PsUICenterTutorialSignTextInput(m_textModel);
			psUICenterTutorialSignTextInput.Update();
		}
		if (m_textModel != null)
		{
			if (m_textModel.m_done && m_sensorText != m_textModel.m_text)
			{
				m_sensorText = m_textModel.m_text;
				(m_graphElement as LevelTextNode).m_text = m_textModel.m_text;
				m_textModel = null;
			}
			else if (m_textModel.m_cancelled)
			{
				m_textModel = null;
			}
		}
	}

	public static void AddListener(string _sensorName, Action _callBack)
	{
		if (m_listeners == null)
		{
			m_listeners = new Dictionary<string, Action>();
		}
		if (!m_listeners.ContainsKey(_sensorName))
		{
			m_listeners.Add(_sensorName, _callBack);
		}
	}
}
