using UnityEngine;

public class AutoGeometryTestState : BasicState
{
	private AutoGeometryLayer m_agLayer;

	public override void Enter(IStatedObject _parent)
	{
		DebugDraw.m_lineWidth = 0.2f;
	}

	public override void Execute()
	{
		if (!Input.GetMouseButton(0))
		{
		}
	}

	public override void Exit()
	{
		AutoGeometryManager.DestroyAllLayers();
		EntityManager.RemoveAllEntities();
	}
}
