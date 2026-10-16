using UnityEngine;

public class EarthGoal : SensorGoal
{
	public EarthGoal(GraphElement _graphElement)
		: base(_graphElement)
	{
	}

	public override ucpShape CreateBodyShape()
	{
		m_mainPrefab = ResourceManager.GetGameObject("GoalGatePrefab_GameObject");
		Vector2[] rect = DebugDraw.GetRect(m_mainPrefab.GetComponent<Renderer>().bounds.size.x / 2f, m_mainPrefab.GetComponent<Renderer>().bounds.size.y, Vector2.zero);
		return new ucpPolyShape(rect, Vector2.zero, 17895696u, 500f, 0.5f, 0.9f, (ucpCollisionType)6, true);
	}

	public override void SetGoalPrefab()
	{
		m_prefabC = PrefabS.AddComponent(m_tc, new Vector3(0f, (0f - m_mainPrefab.GetComponent<Renderer>().bounds.size.y) / 2f), m_mainPrefab);
		if (!m_minigame.m_editing)
		{
			ChipmunkProS.AddCollisionHandler(m_goalCmb, base.GoalCollisionHandler, (ucpCollisionType)6, (ucpCollisionType)3, true, false, false);
			return;
		}
		Vector2[] circle = DebugDraw.GetCircle(45f, 32, Vector2.zero);
		m_minigame.SetGoalNode(this);
	}

	protected override void WinMiniGame()
	{
		float num = 0.75f;
		PsState.m_activeGameLoop.WinMinigame();
	}
}
