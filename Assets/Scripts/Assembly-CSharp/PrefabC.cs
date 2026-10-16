using UnityEngine;

public class PrefabC : BasicComponent
{
	public static int m_componentCount;

	public string m_name;

	public bool m_wasVisible;

	public GameObject p_gameObject;

	public Mesh p_mesh;

	public TransformC p_parentTC;

	public PrefabC()
		: base(ComponentType.Prefab)
	{
		m_componentCount++;
	}

	public override void Reset()
	{
		base.Reset();
		p_mesh = null;
		p_parentTC = null;
		p_gameObject = null;
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	~PrefabC()
	{
		m_componentCount--;
	}
}
