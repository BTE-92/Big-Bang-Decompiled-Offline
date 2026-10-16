using System;
using System.Collections.Generic;
using UnityEngine;

public static class TweenS
{
	public static DynamicArray<TweenC> m_components;

	private static List<TweenC> m_removeList;

	public static bool m_updatingComponents;

	public static void Initialize()
	{
		m_components = new DynamicArray<TweenC>();
		m_removeList = new List<TweenC>();
	}

	public static void AddTweenEndEventListener(TweenC _tweenComponent, TweenEventDelegate _tweenEventHandler)
	{
		_tweenComponent.tweenEndEvent = (TweenEventDelegate)Delegate.Combine(_tweenComponent.tweenEndEvent, _tweenEventHandler);
	}

	public static void RemoveTweenEndEventListener(TweenC _tweenComponent, TweenEventDelegate _tweenEventHandler)
	{
		_tweenComponent.tweenEndEvent = (TweenEventDelegate)Delegate.Remove(_tweenComponent.tweenEndEvent, _tweenEventHandler);
	}

	public static void AddTweenStartEventListener(TweenC _tweenComponent, TweenEventDelegate _tweenEventHandler)
	{
		_tweenComponent.tweenStartEvent = (TweenEventDelegate)Delegate.Combine(_tweenComponent.tweenStartEvent, _tweenEventHandler);
	}

	public static void RemoveTweenStartEventListener(TweenC _tweenComponent, TweenEventDelegate _tweenEventHandler)
	{
		_tweenComponent.tweenStartEvent = (TweenEventDelegate)Delegate.Remove(_tweenComponent.tweenStartEvent, _tweenEventHandler);
	}

	public static TweenC AddTween(Entity _entity, TweenStyle _style, float _startValue, float _endValue, float _duration, float _delay)
	{
		TweenC tweenC = m_components.AddItem();
		tweenC.currentTweenStyle = _style;
		tweenC.startValue = Vector3.one * _startValue;
		tweenC.endValue = Vector3.one * _endValue;
		tweenC.currentValue = tweenC.startValue;
		tweenC.duration = _duration;
		tweenC.delay = _delay;
		tweenC.startTime = Main.m_resettingGameTime + _delay;
		tweenC.currentTime = Main.m_resettingGameTime;
		EntityManager.AddComponentToEntity(_entity, tweenC);
		return tweenC;
	}

	public static TweenC AddTween(Entity _entity, TweenStyle _style, Vector3 _startValue, Vector3 _endValue, float _duration, float _delay)
	{
		TweenC tweenC = m_components.AddItem();
		tweenC.currentTweenStyle = _style;
		tweenC.startValue = _startValue;
		tweenC.endValue = _endValue;
		tweenC.currentValue = _startValue;
		tweenC.duration = _duration;
		tweenC.delay = _delay;
		tweenC.startTime = Main.m_resettingGameTime + _delay;
		tweenC.currentTime = Main.m_resettingGameTime;
		EntityManager.AddComponentToEntity(_entity, tweenC);
		return tweenC;
	}

	public static void ReInitialize(TweenC _c, TweenStyle _style, Vector3 _startValue, Vector3 _endValue, float _duration, float _delay)
	{
		_c.m_active = true;
		_c.currentTweenStyle = _style;
		_c.startValue = _startValue;
		_c.endValue = _endValue;
		_c.currentValue = _startValue;
		_c.duration = _duration;
		_c.delay = _delay;
		_c.startTime = Main.m_resettingGameTime + _delay;
		_c.currentTime = Main.m_resettingGameTime;
	}

	public static TweenC AddTransformTween(TransformC _tc, TweenedProperty _component, TweenStyle _style, Vector3 _endValue, float _duration, float _delay, bool _removeComponentAtFinish)
	{
		return AddTransformTween(_tc, _component, _style, _endValue, _duration, _delay, false, _removeComponentAtFinish);
	}

	public static TweenC AddTransformTween(TransformC _tc, TweenedProperty _component, TweenStyle _style, Vector3 _endValue, float _duration, float _delay, bool _globalTarget, bool _removeComponentAtFinish)
	{
		Vector3 startValue = Vector3.zero;
		switch (_component)
		{
		case TweenedProperty.Position:
			startValue = _tc.transform.localPosition;
			break;
		case TweenedProperty.Rotation:
			startValue = ((!_tc.forceRotation) ? _tc.transform.localRotation.eulerAngles : _tc.forcedRotation.eulerAngles);
			break;
		case TweenedProperty.Scale:
			startValue = ((!_tc.forceScale) ? _tc.transform.localScale : _tc.forcedScale);
			break;
		}
		return AddTransformTween(_tc, _component, _style, startValue, _endValue, _duration, _delay, _globalTarget, _removeComponentAtFinish);
	}

	public static TweenC AddTransformTween(TransformC _tc, TweenedProperty _component, TweenStyle _style, Vector3 _startValue, Vector3 _endValue, float _duration, float _delay, bool _removeComponentAtFinish)
	{
		return AddTransformTween(_tc, _component, _style, _startValue, _endValue, _duration, _delay, false, _removeComponentAtFinish);
	}

	public static TweenC AddTransformTween(TransformC _tc, TweenedProperty _component, TweenStyle _style, Vector3 _startValue, Vector3 _endValue, float _duration, float _delay, bool _globalTarget, bool _removeComponentAtFinish)
	{
		TweenC tweenC = m_components.AddItem();
		tweenC.p_TC = _tc;
		tweenC.tweenedProperty = _component;
		tweenC.currentTweenStyle = _style;
		tweenC.startValue = _startValue;
		if (_globalTarget && _component == TweenedProperty.Position)
		{
			_endValue = _tc.transform.position + (_endValue - _tc.transform.position);
		}
		tweenC.endValue = _endValue;
		tweenC.currentValue = _startValue;
		tweenC.duration = _duration;
		tweenC.delay = _delay;
		tweenC.startTime = Main.m_resettingGameTime + _delay;
		tweenC.currentTime = Main.m_resettingGameTime;
		tweenC.removeComponentAtFinish = _removeComponentAtFinish;
		tweenC.alphaSprite = true;
		tweenC.alphaPrefabs = false;
		tweenC.alphaText = false;
		if (_tc != null)
		{
			EntityManager.AddComponentToEntity(_tc.p_entity, tweenC);
		}
		else
		{
			tweenC.m_active = true;
			tweenC.m_wasActive = true;
		}
		return tweenC;
	}

	public static void SetTweenAlphaProperties(TweenC _c, bool _alphaSprite, bool _alphaPrefabs, bool _alphaText, Shader _shader = null)
	{
		_c.alphaSprite = _alphaSprite;
		_c.alphaPrefabs = _alphaPrefabs;
		_c.alphaText = _alphaText;
		_c.alphaShader = _shader;
	}

	public static TweenC AddCurvedTransformTween(TransformC _tc, TweenedProperty _component, TweenStyle _style, Vector3 _endValue, Vector3 _control0, Vector3 _control1, float _duration, float _delay, bool _removeComponentAtFinish)
	{
		Vector3 startValue = Vector3.zero;
		switch (_component)
		{
		case TweenedProperty.Position:
			startValue = _tc.transform.localPosition;
			break;
		case TweenedProperty.Rotation:
			startValue = ((!_tc.forceRotation) ? _tc.transform.localRotation.eulerAngles : _tc.forcedRotation.eulerAngles);
			break;
		case TweenedProperty.Scale:
			startValue = ((!_tc.forceScale) ? _tc.transform.localScale : _tc.forcedScale);
			break;
		}
		return AddCurvedTransformTween(_tc, _component, _style, startValue, _endValue, _control0, _control1, _duration, _delay, _removeComponentAtFinish);
	}

	public static TweenC AddCurvedTransformTween(TransformC _tc, TweenedProperty _component, TweenStyle _style, Vector3 _startValue, Vector3 _endValue, Vector3 _control0, Vector3 _control1, float _duration, float _delay, bool _removeComponentAtFinish)
	{
		TweenC tweenC = m_components.AddItem();
		tweenC.p_TC = _tc;
		tweenC.tweenedProperty = _component;
		tweenC.currentTweenStyle = _style;
		tweenC.startValue = _startValue;
		tweenC.endValue = _endValue;
		tweenC.currentValue = _startValue;
		tweenC.control0 = _control0;
		tweenC.control1 = _control1;
		tweenC.duration = _duration;
		tweenC.delay = _delay;
		tweenC.startTime = Main.m_resettingGameTime + _delay;
		tweenC.currentTime = Main.m_resettingGameTime;
		tweenC.curved = true;
		tweenC.removeComponentAtFinish = _removeComponentAtFinish;
		tweenC.alphaSprite = true;
		tweenC.alphaPrefabs = false;
		tweenC.alphaText = false;
		if (_tc != null)
		{
			EntityManager.AddComponentToEntity(_tc.p_entity, tweenC);
		}
		else
		{
			tweenC.m_active = true;
			tweenC.m_wasActive = true;
		}
		return tweenC;
	}

	public static void RemoveComponent(TweenC _c)
	{
		if (_c.tweenEndEvent != null)
		{
			Delegate[] invocationList = _c.tweenEndEvent.GetInvocationList();
			Delegate[] array = invocationList;
			foreach (Delegate obj in array)
			{
				_c.tweenEndEvent = (TweenEventDelegate)Delegate.Remove(_c.tweenEndEvent, (TweenEventDelegate)obj);
			}
			_c.tweenEndEvent = null;
		}
		if (_c.tweenStartEvent != null)
		{
			Delegate[] invocationList2 = _c.tweenStartEvent.GetInvocationList();
			Delegate[] array2 = invocationList2;
			foreach (Delegate obj2 in array2)
			{
				_c.tweenStartEvent = (TweenEventDelegate)Delegate.Remove(_c.tweenStartEvent, (TweenEventDelegate)obj2);
			}
			_c.tweenStartEvent = null;
		}
		EntityManager.RemoveComponentFromEntity(_c);
		if (!m_updatingComponents)
		{
			m_components.RemoveItem(_c);
		}
		else if (!m_removeList.Contains(_c))
		{
			m_removeList.Add(_c);
		}
	}

	public static void RemoveAllTweensFromEntity(Entity _e)
	{
		List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.Tween, _e);
		while (componentsByEntity.Count > 0)
		{
			RemoveComponent(componentsByEntity[0] as TweenC);
			componentsByEntity.RemoveAt(0);
		}
	}

	public static void SetAdditionalTweenProperties(TweenC _c, int _repeatCount, bool _mirror = false, TweenStyle _mirroredTweenStyle = TweenStyle.Linear)
	{
		_c.repeats = _repeatCount;
		_c.mirrored = _mirror;
		_c.mirroredTweenStyle = _mirroredTweenStyle;
		if (_mirror && _c.repeats != -1)
		{
			_c.repeats = _c.repeats * 2 + 1;
		}
	}

	public static void SetRemoveEntityAtFinish(TweenC _c, bool _remove)
	{
		_c.removeEntityAtFinish = _remove;
	}

	public static void SetRemoveComponentAtFinish(TweenC _c, bool _remove)
	{
		_c.removeComponentAtFinish = _remove;
	}

	public static void Update()
	{
		m_updatingComponents = true;
		int aliveCount = m_components.m_aliveCount;
		for (int i = 0; i < aliveCount; i++)
		{
			TweenC tweenC = m_components.m_array[m_components.m_aliveIndices[i]];
			if (tweenC.m_active)
			{
				tweenC.currentTime += ((!tweenC.useUnscaledDeltaTime) ? Main.m_gameDeltaTime : Main.m_dt);
				float num = Mathf.Max(tweenC.currentTime - tweenC.startTime, 0f);
				if (!tweenC.hasStarted && num > 0f)
				{
					if (tweenC.tweenStartEvent != null)
					{
						tweenC.tweenStartEvent(tweenC);
					}
					tweenC.hasStarted = true;
				}
				if (num >= tweenC.duration)
				{
					if (tweenC.currentRepeat < tweenC.repeats || tweenC.repeats == -1)
					{
						num -= tweenC.duration;
						tweenC.startTime += tweenC.duration;
						tweenC.currentRepeat++;
						if (tweenC.mirrored)
						{
							Vector3 startValue = tweenC.startValue;
							tweenC.startValue = tweenC.endValue;
							tweenC.endValue = startValue;
							TweenStyle currentTweenStyle = tweenC.currentTweenStyle;
							tweenC.currentTweenStyle = tweenC.mirroredTweenStyle;
							tweenC.mirroredTweenStyle = currentTweenStyle;
						}
					}
					else
					{
						tweenC.m_active = false;
						tweenC.currentValue = tweenC.endValue;
						tweenC.hasFinished = true;
					}
				}
				UpdateTween(tweenC, num);
				if (num >= tweenC.duration && tweenC.hasFinished)
				{
					if (tweenC.tweenEndEvent != null)
					{
						tweenC.tweenEndEvent(tweenC);
					}
					if (tweenC.removeComponentAtFinish)
					{
						RemoveComponent(tweenC);
					}
				}
			}
			aliveCount = m_components.m_aliveCount;
		}
		m_updatingComponents = false;
		while (m_removeList.Count > 0)
		{
			int index = m_removeList.Count - 1;
			TweenC tweenC2 = m_removeList[index];
			if (tweenC2.removeEntityAtFinish)
			{
				EntityManager.RemoveEntity(tweenC2.p_entity);
			}
			else
			{
				RemoveComponent(tweenC2);
			}
			m_removeList.RemoveAt(index);
		}
		m_components.Update();
	}

	public static void UpdateTween(TweenC c, float t)
	{
		if (c.p_TC == null)
		{
			if (c.m_active)
			{
				c.currentValue.x = tween(c.currentTweenStyle, t, c.duration, c.startValue.x, c.endValue.x - c.startValue.x);
				if (c.tweenedProperty != TweenedProperty.None)
				{
					c.currentValue.y = tween(c.currentTweenStyle, t, c.duration, c.startValue.y, c.endValue.y - c.startValue.y);
					c.currentValue.z = tween(c.currentTweenStyle, t, c.duration, c.startValue.z, c.endValue.z - c.startValue.z);
				}
			}
			return;
		}
		TransformC p_TC = c.p_TC;
		if (c.tweenedProperty == TweenedProperty.Position)
		{
			if (c.m_active)
			{
				if (!c.curved)
				{
					c.currentValue.x = tween(c.currentTweenStyle, t, c.duration, c.startValue.x, c.endValue.x - c.startValue.x);
					c.currentValue.y = tween(c.currentTweenStyle, t, c.duration, c.startValue.y, c.endValue.y - c.startValue.y);
					c.currentValue.z = tween(c.currentTweenStyle, t, c.duration, c.startValue.z, c.endValue.z - c.startValue.z);
					p_TC.transform.localPosition = c.currentValue;
				}
				else
				{
					c.currentValue = curveTween(c.currentTweenStyle, t, c.duration, c.startValue, c.endValue, c.control0, c.control1);
					p_TC.transform.localPosition = c.currentValue;
				}
			}
			else
			{
				p_TC.transform.localPosition = c.endValue;
				c.currentValue = c.endValue;
			}
			p_TC.updatePosition = true;
		}
		else if (c.tweenedProperty == TweenedProperty.Rotation)
		{
			if (c.m_active)
			{
				c.currentValue.x = tween(c.currentTweenStyle, t, c.duration, c.startValue.x, c.endValue.x - c.startValue.x);
				c.currentValue.y = tween(c.currentTweenStyle, t, c.duration, c.startValue.y, c.endValue.y - c.startValue.y);
				c.currentValue.z = tween(c.currentTweenStyle, t, c.duration, c.startValue.z, c.endValue.z - c.startValue.z);
				if (p_TC.forceRotation)
				{
					p_TC.forcedRotation = Quaternion.Euler(c.currentValue);
				}
				else
				{
					p_TC.transform.localRotation = Quaternion.Euler(c.currentValue);
				}
			}
			else
			{
				if (p_TC.forceRotation)
				{
					p_TC.forcedRotation = Quaternion.Euler(c.endValue);
				}
				else
				{
					p_TC.transform.localRotation = Quaternion.Euler(c.endValue);
				}
				c.currentValue = c.endValue;
				if (c.removeComponentAtFinish)
				{
					m_removeList.Add(c);
				}
			}
			p_TC.updateRotation = true;
		}
		else if (c.tweenedProperty == TweenedProperty.Scale)
		{
			if (c.m_active)
			{
				c.currentValue.x = tween(c.currentTweenStyle, t, c.duration, c.startValue.x, c.endValue.x - c.startValue.x);
				c.currentValue.y = tween(c.currentTweenStyle, t, c.duration, c.startValue.y, c.endValue.y - c.startValue.y);
				c.currentValue.z = tween(c.currentTweenStyle, t, c.duration, c.startValue.z, c.endValue.z - c.startValue.z);
				if (p_TC.forceScale)
				{
					p_TC.forcedScale = c.currentValue;
				}
				else
				{
					p_TC.transform.localScale = c.currentValue;
				}
			}
			else
			{
				if (p_TC.forceScale)
				{
					p_TC.forcedScale = c.endValue;
				}
				else
				{
					p_TC.transform.localScale = c.endValue;
				}
				c.currentValue = c.endValue;
			}
			p_TC.updateScale = true;
			p_TC.updateRotation = true;
		}
		else
		{
			if (c.tweenedProperty != TweenedProperty.Alpha)
			{
				return;
			}
			if (c.m_active)
			{
				c.currentValue.x = tween(c.currentTweenStyle, t, c.duration, c.startValue.x, c.endValue.x - c.startValue.x);
			}
			else
			{
				c.currentValue = c.endValue;
			}
			if (c.alphaSprite)
			{
				SpriteS.SetAlphaByTransformComponent(c.p_TC, c.currentValue.x);
			}
			if (c.alphaText && c.p_TC.p_entity != null)
			{
				List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.TextMesh, c.p_TC.p_entity);
				for (int i = 0; i < componentsByEntity.Count; i++)
				{
					TextMeshC textMeshC = componentsByEntity[i] as TextMeshC;
					if (c.alphaShader != null)
					{
						textMeshC.m_textMesh.GetComponent<Renderer>().material.shader = c.alphaShader;
					}
					Color color = textMeshC.m_textMesh.GetComponent<Renderer>().material.color;
					color.a = c.currentValue.x;
					textMeshC.m_textMesh.GetComponent<Renderer>().material.color = color;
				}
			}
			if (!c.alphaPrefabs || c.p_TC.p_entity == null)
			{
				return;
			}
			List<IComponent> componentsByEntity2 = EntityManager.GetComponentsByEntity(ComponentType.Prefab, c.p_TC.p_entity);
			for (int j = 0; j < componentsByEntity2.Count; j++)
			{
				PrefabC prefabC = componentsByEntity2[j] as PrefabC;
				if (prefabC.p_gameObject != null && prefabC.p_gameObject.GetComponent<Renderer>() != null && prefabC.p_gameObject.GetComponent<Renderer>().material != null)
				{
					if (c.alphaShader != null)
					{
						prefabC.p_gameObject.GetComponent<Renderer>().material.shader = c.alphaShader;
					}
					Color color2 = prefabC.p_gameObject.GetComponent<Renderer>().material.color;
					color2.a = c.currentValue.x;
					prefabC.p_gameObject.GetComponent<Renderer>().material.color = color2;
				}
			}
		}
	}

	public static float tween(TweenStyle _style, float _currentTime, float _duration, float _start, float _difference)
	{
		switch (_style)
		{
		case TweenStyle.Linear:
			return linear(_currentTime, _start, _duration, _difference);
		case TweenStyle.QuadIn:
			return easeInQuad(_currentTime, _start, _duration, _difference);
		case TweenStyle.QuadOut:
			return easeOutQuad(_currentTime, _start, _duration, _difference);
		case TweenStyle.QuadInOut:
			return easeInOutQuad(_currentTime, _start, _duration, _difference);
		case TweenStyle.CubicIn:
			return easeInCubic(_currentTime, _start, _duration, _difference);
		case TweenStyle.CubicOut:
			return easeOutCubic(_currentTime, _start, _duration, _difference);
		case TweenStyle.CubicInOut:
			return easeInOutCubic(_currentTime, _start, _duration, _difference);
		case TweenStyle.ExpoIn:
			return easeInExpo(_currentTime, _start, _duration, _difference);
		case TweenStyle.ExpoOut:
			return easeOutExpo(_currentTime, _start, _duration, _difference);
		case TweenStyle.ExpoInOut:
			return easeOutExpo(_currentTime, _start, _duration, _difference);
		case TweenStyle.BackOut:
			return easeOutBack(_currentTime, _start, _duration, _difference);
		case TweenStyle.ElasticOut:
			return easeOutElastic(_currentTime, _start, _duration, _difference);
		case TweenStyle.BounceOut:
			return easeOutBounce(_currentTime, _start, _duration, _difference);
		default:
			return 0f;
		}
	}

	public static Vector3 curveTween(TweenStyle _style, float _currentTime, float _duration, Vector3 _start, Vector3 _end, Vector3 _control0, Vector3 _control1)
	{
		switch (_style)
		{
		case TweenStyle.Linear:
			return ToolBox.GetBezierPoint(linear(_currentTime, 0f, _duration, 1f), _start, _control0, _control1, _end);
		case TweenStyle.QuadIn:
			return ToolBox.GetBezierPoint(easeInQuad(_currentTime, 0f, _duration, 1f), _start, _control0, _control1, _end);
		case TweenStyle.QuadOut:
			return ToolBox.GetBezierPoint(easeOutQuad(_currentTime, 0f, _duration, 1f), _start, _control0, _control1, _end);
		case TweenStyle.QuadInOut:
			return ToolBox.GetBezierPoint(easeInOutQuad(_currentTime, 0f, _duration, 1f), _start, _control0, _control1, _end);
		case TweenStyle.CubicIn:
			return ToolBox.GetBezierPoint(easeInCubic(_currentTime, 0f, _duration, 1f), _start, _control0, _control1, _end);
		case TweenStyle.CubicOut:
			return ToolBox.GetBezierPoint(easeOutCubic(_currentTime, 0f, _duration, 1f), _start, _control0, _control1, _end);
		case TweenStyle.CubicInOut:
			return ToolBox.GetBezierPoint(easeInOutCubic(_currentTime, 0f, _duration, 1f), _start, _control0, _control1, _end);
		case TweenStyle.ExpoIn:
			return ToolBox.GetBezierPoint(easeInExpo(_currentTime, 0f, _duration, 1f), _start, _control0, _control1, _end);
		case TweenStyle.ExpoOut:
			return ToolBox.GetBezierPoint(easeOutExpo(_currentTime, 0f, _duration, 1f), _start, _control0, _control1, _end);
		case TweenStyle.ExpoInOut:
			return ToolBox.GetBezierPoint(easeInOutExpo(_currentTime, 0f, _duration, 1f), _start, _control0, _control1, _end);
		case TweenStyle.BackOut:
			return ToolBox.GetBezierPoint(easeOutBack(_currentTime, 0f, _duration, 1f), _start, _control0, _control1, _end);
		case TweenStyle.ElasticOut:
			return ToolBox.GetBezierPoint(easeOutElastic(_currentTime, 0f, _duration, 1f), _start, _control0, _control1, _end);
		case TweenStyle.BounceOut:
			return ToolBox.GetBezierPoint(easeOutBounce(_currentTime, 0f, _duration, 1f), _start, _control0, _control1, _end);
		default:
			return Vector3.zero;
		}
	}

	public static float easeInQuad(float t, float b, float d, float c)
	{
		return c * (t /= d) * t + b;
	}

	public static float easeOutQuad(float t, float b, float d, float c)
	{
		return (0f - c) * (t /= d) * (t - 2f) + b;
	}

	public static float easeInOutQuad(float t, float b, float d, float c)
	{
		if ((t /= d / 2f) < 1f)
		{
			return c / 2f * t * t + b;
		}
		return (0f - c) / 2f * ((t -= 1f) * (t - 2f) - 1f) + b;
	}

	public static float easeInCubic(float t, float b, float d, float c)
	{
		return c * (t /= d) * t * t + b;
	}

	public static float easeOutCubic(float t, float b, float d, float c)
	{
		return c * ((t = t / d - 1f) * t * t + 1f) + b;
	}

	public static float easeInOutCubic(float t, float b, float d, float c)
	{
		if ((t /= d / 2f) < 1f)
		{
			return c / 2f * t * t * t + b;
		}
		return c / 2f * ((t -= 2f) * t * t + 2f) + b;
	}

	public static float easeInExpo(float t, float b, float d, float c)
	{
		return (t != 0f) ? (c * Mathf.Pow(2f, 10f * (t / d - 1f)) + b - c * 0.001f) : b;
	}

	public static float easeOutExpo(float t, float b, float d, float c)
	{
		return (t != d) ? (c * 1.001f * (0f - Mathf.Pow(2f, -10f * t / d) + 1f) + b) : (b + c);
	}

	public static float easeInOutExpo(float t, float b, float d, float c)
	{
		if (t == 0f)
		{
			return b;
		}
		if (t == d)
		{
			return b + c;
		}
		if ((t /= d / 2f) < 1f)
		{
			return c / 2f * Mathf.Pow(2f, 10f * (t - 1f)) + b - c * 0.0005f;
		}
		return c / 2f * 1.0005f * (0f - Mathf.Pow(2f, -10f * (t -= 1f)) + 2f) + b;
	}

	public static float easeOutBack(float t, float b, float d, float c)
	{
		float num = 1.70158f;
		return c * ((t = t / d - 1f) * t * ((num + 1f) * t + num) + 1f) + b;
	}

	public static float easeOutElastic(float t, float b, float d, float c)
	{
		float num = (float)Math.PI * 2f;
		float num2 = 0.5f;
		float num3 = 0.5f;
		if (t == 0f)
		{
			return b;
		}
		if ((t /= d) == 1f)
		{
			return b + c;
		}
		if (num2 != 0f)
		{
			num2 = d * 0.3f;
		}
		float num4;
		if (num3 != 0f || (c > 0f && num3 < c) || (c < 0f && num3 < 0f - c))
		{
			num3 = c;
			num4 = num2 / 4f;
		}
		else
		{
			num4 = num2 / num * Mathf.Asin(c / num3);
		}
		return num3 * Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * d - num4) * num / num2) + c + b;
	}

	public static float easeOutBounce(float t, float b, float d, float c)
	{
		if ((t /= d) < 0.36363637f)
		{
			return c * (7.5625f * t * t) + b;
		}
		if (t < 0.72727275f)
		{
			return c * (7.5625f * (t -= 0.54545456f) * t + 0.75f) + b;
		}
		if (t < 0.90909094f)
		{
			return c * (7.5625f * (t -= 0.8181818f) * t + 0.9375f) + b;
		}
		return c * (7.5625f * (t -= 21f / 22f) * t + 63f / 64f) + b;
	}

	public static float linear(float t, float b, float d, float c)
	{
		return c * t / d + b;
	}
}
