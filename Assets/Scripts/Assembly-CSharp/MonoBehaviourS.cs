public static class MonoBehaviourS
{
	public static DynamicArray<MonoBehaviourC> m_components;

	public static void Initialize()
	{
		m_components = new DynamicArray<MonoBehaviourC>();
	}

	public static MonoBehaviourC AddComponent(Entity _entity, IMonoBehaviour _monoBehaviour)
	{
		MonoBehaviourC monoBehaviourC = m_components.AddItem();
		monoBehaviourC.m_monoBehaviour = _monoBehaviour;
		monoBehaviourC.m_monoBehaviour.m_component = monoBehaviourC;
		EntityManager.AddComponentToEntity(_entity, monoBehaviourC);
		return monoBehaviourC;
	}

	public static void RemoveComponent(MonoBehaviourC _c)
	{
		if (_c.p_entity == null)
		{
			Debug.LogWarning("Trying to remove component that has already been removed");
			return;
		}
		_c.m_monoBehaviour = null;
		EntityManager.RemoveComponentFromEntity(_c);
		m_components.RemoveItem(_c);
	}

	public static void Update()
	{
		for (int num = m_components.m_aliveCount - 1; num > -1; num--)
		{
			MonoBehaviourC monoBehaviourC = m_components.m_array[m_components.m_aliveIndices[num]];
			if (monoBehaviourC.m_active)
			{
				monoBehaviourC.m_monoBehaviour.TLUpdate();
			}
		}
		m_components.Update();
	}
}
