using System.Collections.Generic;

public static class TimerS
{
	public static DynamicArray<TimerC> m_components;

	private static List<Entity> m_removeEntityList;

	private static List<TimerC> m_removeComponentList;

	private static bool m_updatingComponents;

	public static void Initialize()
	{
		m_components = new DynamicArray<TimerC>();
		m_removeEntityList = new List<Entity>();
		m_removeComponentList = new List<TimerC>();
	}

	public static TimerC AddComponent(Entity _entity, string _name, float _duration, float _delay, bool _destroyEntityOnTimeout, TimerComponentDelegate _timeoutHandler = null)
	{
		TimerC timerC = m_components.AddItem();
		timerC.name = _name;
		timerC.duration = _duration;
		timerC.currentTime = 0f - _delay;
		timerC.timeoutHandler = _timeoutHandler;
		timerC.isDone = false;
		timerC.destroyEntityOnTimeout = _destroyEntityOnTimeout;
		timerC.customComponent = null;
		EntityManager.AddComponentToEntity(_entity, timerC);
		return timerC;
	}

	public static void RemoveComponent(TimerC _c)
	{
		if (_c.p_entity == null)
		{
			Debug.LogWarning("Trying to remove component that has already been removed");
			return;
		}
		_c.timeoutHandler = null;
		EntityManager.RemoveComponentFromEntity(_c);
		if (!m_updatingComponents)
		{
			m_components.RemoveItem(_c);
		}
		else if (!m_removeComponentList.Contains(_c))
		{
			m_removeComponentList.Add(_c);
		}
	}

	public static void Update()
	{
		m_updatingComponents = true;
		for (int num = m_components.m_aliveCount - 1; num > -1; num--)
		{
			TimerC timerC = m_components.m_array[m_components.m_aliveIndices[num]];
			if (timerC.m_active)
			{
				timerC.currentTime += Main.m_gameDeltaTime;
				if (timerC.currentTime >= timerC.duration)
				{
					timerC.isDone = true;
					if (timerC.timeoutHandler != null)
					{
						timerC.timeoutHandler(timerC);
					}
					if (timerC.destroyEntityOnTimeout && !m_removeEntityList.Contains(timerC.p_entity))
					{
						m_removeEntityList.Add(timerC.p_entity);
					}
				}
			}
		}
		m_updatingComponents = false;
		while (m_removeComponentList.Count > 0)
		{
			int index = m_removeComponentList.Count - 1;
			m_components.RemoveItem(m_removeComponentList[index]);
			m_removeComponentList.RemoveAt(index);
		}
		while (m_removeEntityList.Count > 0)
		{
			int index2 = m_removeEntityList.Count - 1;
			EntityManager.RemoveEntity(m_removeEntityList[index2]);
			m_removeEntityList.RemoveAt(index2);
		}
		m_components.Update();
	}
}
