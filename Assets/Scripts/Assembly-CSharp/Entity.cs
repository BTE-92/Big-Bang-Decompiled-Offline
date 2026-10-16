using System.Collections.Generic;

public class Entity : IPoolable
{
	public static int m_instanceCount;

	private int _index = -1;

	private bool _active;

	public List<IComponent> m_components;

	public int m_componentsChecksum;

	public bool m_persistent;

	public bool m_visible;

	public int m_entityLogicCount;

	public EntityLogicDelegate d_entityLogic;

	public PoolableState m_state;

	public int m_index
	{
		get
		{
			return _index;
		}
		set
		{
			_index = value;
		}
	}

	public bool m_active
	{
		get
		{
			return _active;
		}
		set
		{
			_active = value;
		}
	}

	public Entity()
	{
		m_components = new List<IComponent>();
		m_instanceCount++;
	}

	public void Reset()
	{
		m_components.Clear();
		m_entityLogicCount = 0;
		m_componentsChecksum = 0;
		m_persistent = false;
		m_visible = true;
	}

	public void Destroy()
	{
	}

	~Entity()
	{
		m_instanceCount--;
	}
}
