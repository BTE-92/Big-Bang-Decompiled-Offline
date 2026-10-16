using UnityEngine;

public class TouchAreaTestState : BasicState
{
	public override void Enter(IStatedObject _parent)
	{
		Entity entity = EntityManager.AddEntity();
		TransformC transformC = TransformS.AddComponent(entity);
		TransformS.SetPosition(transformC, new Vector3(0f, 100f, -20f));
		TouchAreaC touchAreaC = TouchAreaS.AddCircleArea(transformC, "circle", 100f, CameraS.m_uiCamera);
		TouchAreaS.AddTouchEventListener(touchAreaC, Handler);
		DebugDraw.CreateCircle(CameraS.m_uiCamera, transformC, Vector2.zero, 100f);
		touchAreaC.m_maxTouches = 2;
		entity = EntityManager.AddEntity();
		transformC = TransformS.AddComponent(entity);
		TransformS.SetPosition(transformC, new Vector3(-50f, 0f, 0f));
		touchAreaC = TouchAreaS.AddRectArea(transformC, "rect LEFT", 100f, 100f, CameraS.m_uiCamera);
		TouchAreaS.AddTouchEventListener(touchAreaC, Handler);
		DebugDraw.CreateBox(CameraS.m_uiCamera, transformC, Vector2.zero, 100f, 100f);
		touchAreaC.m_allowSecondary = true;
		entity = EntityManager.AddEntity();
		transformC = TransformS.AddComponent(entity);
		TransformS.SetPosition(transformC, new Vector3(50f, 0f, 0f));
		touchAreaC = TouchAreaS.AddRectArea(transformC, "rect RIGHT", 100f, 100f, CameraS.m_uiCamera);
		TouchAreaS.AddTouchEventListener(touchAreaC, Handler);
		DebugDraw.CreateBox(CameraS.m_uiCamera, transformC, Vector2.zero, 100f, 100f);
		touchAreaC.m_allowPrimary = false;
		touchAreaC.m_allowSecondary = true;
		entity = EntityManager.AddEntity();
		transformC = TransformS.AddComponent(entity);
		TransformS.SetPosition(transformC, new Vector3(25f, 50f, -40f));
		touchAreaC = TouchAreaS.AddRectArea(transformC, "rect SMALL", 50f, 50f, CameraS.m_uiCamera);
		TouchAreaS.AddTouchEventListener(touchAreaC, Handler);
		DebugDraw.CreateBox(CameraS.m_uiCamera, transformC, Vector2.zero, 50f, 50f);
		touchAreaC.m_allowPrimary = true;
		touchAreaC.m_allowSecondary = true;
		TweenC c = TweenS.AddTransformTween(transformC, TweenedProperty.Position, TweenStyle.CubicInOut, new Vector3(25f, -200f, -10f), 3f, 0f, true);
		TweenS.SetAdditionalTweenProperties(c, -1, true, TweenStyle.CubicInOut);
	}

	public void Handler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		Debug.Log(_touchArea.m_touchCount);
		if (!_touchIsSecondary)
		{
			if (_touchArea.m_touchCount == 1)
			{
				switch (_touchPhase)
				{
				case TouchAreaPhase.Began:
				case TouchAreaPhase.RollIn:
					SpriteS.SetColorByTransformComponent(_touchArea.m_TC, Color.green);
					break;
				case TouchAreaPhase.RollOut:
					SpriteS.SetColorByTransformComponent(_touchArea.m_TC, Color.yellow);
					break;
				}
			}
			else if (_touchArea.m_touchCount == 0 && (_touchPhase == TouchAreaPhase.ReleaseIn || _touchPhase == TouchAreaPhase.ReleaseOut))
			{
				SpriteS.SetColorByTransformComponent(_touchArea.m_TC, Color.grey);
			}
			switch (_touchPhase)
			{
			case TouchAreaPhase.Began:
				Debug.Log("primary: " + _touchArea.m_name + ": " + _touchPhase);
				break;
			case TouchAreaPhase.RollIn:
				Debug.Log("primary: " + _touchArea.m_name + ": " + _touchPhase);
				break;
			case TouchAreaPhase.RollOut:
				Debug.Log("primary: " + _touchArea.m_name + ": " + _touchPhase);
				break;
			case TouchAreaPhase.ReleaseIn:
				Debug.Log("primary: " + _touchArea.m_name + ": " + _touchPhase);
				break;
			case TouchAreaPhase.ReleaseOut:
				Debug.Log("primary: " + _touchArea.m_name + ": " + _touchPhase);
				break;
			case TouchAreaPhase.DragStart:
				Debug.Log("primary: " + _touchArea.m_name + ": " + _touchPhase);
				break;
			case TouchAreaPhase.DragEnd:
				Debug.Log("primary: " + _touchArea.m_name + ": " + _touchPhase);
				break;
			}
			return;
		}
		if (_touchArea.m_touchCount == 1)
		{
			if (_touchPhase == TouchAreaPhase.RollIn)
			{
				SpriteS.SetColorByTransformComponent(_touchArea.m_TC, Color.cyan);
			}
		}
		else if (_touchArea.m_touchCount == 0 && (_touchPhase == TouchAreaPhase.RollOut || _touchPhase == TouchAreaPhase.ReleaseOut))
		{
			SpriteS.SetColorByTransformComponent(_touchArea.m_TC, Color.grey);
		}
		switch (_touchPhase)
		{
		case TouchAreaPhase.RollIn:
			Debug.Log("secondary: " + _touchArea.m_name + ": " + _touchPhase);
			break;
		case TouchAreaPhase.RollOut:
			Debug.Log("secondary: " + _touchArea.m_name + ": " + _touchPhase);
			break;
		case TouchAreaPhase.DragStart:
			Debug.Log("secondary: " + _touchArea.m_name + ": " + _touchPhase);
			break;
		case TouchAreaPhase.DragEnd:
			Debug.Log("secondary: " + _touchArea.m_name + ": " + _touchPhase);
			break;
		case TouchAreaPhase.ReleaseOut:
			Debug.Log("secondary: " + _touchArea.m_name + ": " + _touchPhase);
			break;
		}
	}

	public override void Execute()
	{
	}

	public override void Exit()
	{
		EntityManager.RemoveAllEntities();
	}
}
