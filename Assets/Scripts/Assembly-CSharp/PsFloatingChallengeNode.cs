using UnityEngine;

public class PsFloatingChallengeNode : PsFloatingPlanetNode
{
	public PsFloatingChallengeNode(PsTimedEventLoop _loop, bool _tutorial = false)
		: base(_loop, _tutorial)
	{
	}

	public override void SetPrefabName()
	{
		m_prefabName = "FloatingPlatformDailyDiamondsPrefab";
	}

	public override void CreateUI()
	{
		Vector3 vector = new Vector3(0f, -4f, -5f);
		TransformS.ParentComponent(m_leftStarTC, m_domeUITC, m_leftStarTC.transform.localPosition + vector);
		TransformS.ParentComponent(m_centerStarTC, m_domeUITC, m_centerStarTC.transform.localPosition + vector);
		TransformS.ParentComponent(m_rightStarTC, m_domeUITC, m_rightStarTC.transform.localPosition + vector);
		Frame frame = m_planetNodeSpriteSheet.m_atlas.GetFrame("menu_diamond_small_empty");
		Frame frame2 = m_planetNodeSpriteSheet.m_atlas.GetFrame("menu_diamond_small_full");
		Frame frame3 = ((m_loop.m_scoreOld <= 0) ? frame : frame2);
		SpriteC spriteC = SpriteS.AddComponent(m_leftStarTC, frame3, m_planetNodeSpriteSheet);
		SpriteS.SetDimensions(spriteC, 4f * (frame3.width / frame3.height), 4f);
		SpriteS.SetColor(spriteC, Color.white);
		frame3 = ((m_loop.m_scoreOld <= 1) ? frame : frame2);
		SpriteC spriteC2 = SpriteS.AddComponent(m_centerStarTC, frame3, m_planetNodeSpriteSheet);
		SpriteS.SetSortValue(spriteC2, -1f);
		SpriteS.SetDimensions(spriteC2, 4f * (frame3.width / frame3.height), 4f);
		SpriteS.SetColor(spriteC2, Color.white);
		frame3 = ((m_loop.m_scoreOld <= 2) ? frame : frame2);
		SpriteC spriteC3 = SpriteS.AddComponent(m_rightStarTC, frame3, m_planetNodeSpriteSheet);
		SpriteS.SetDimensions(spriteC3, 4f * (frame3.width / frame3.height), 4f);
		SpriteS.SetColor(spriteC3, Color.white);
		TransformS.Move(m_domeNumberTC, Vector3.down * 3.1f);
		base.CreateUI();
	}

	public override void GiveStars(TimerComponentDelegate _starsGivenDelegate)
	{
		TimerC timerC = TimerS.AddComponent(EntityManager.AddEntity(), string.Empty, (float)m_loop.m_scoreBest * 0.25f + 0.5f, 0f, true, _starsGivenDelegate);
		if (m_loop.m_scoreCurrent > 0)
		{
			timerC = TimerS.AddComponent(m_tc.p_entity, string.Empty, 0.5f, 0f, false, GiveDiamond);
			timerC.customComponent = m_leftStarTC;
		}
		if (m_loop.m_scoreCurrent > 1)
		{
			timerC = TimerS.AddComponent(m_tc.p_entity, string.Empty, 0.75f, 0f, false, GiveDiamond);
			timerC.customComponent = m_centerStarTC;
		}
		if (m_loop.m_scoreCurrent > 2)
		{
			timerC = TimerS.AddComponent(m_tc.p_entity, string.Empty, 1f, 0f, false, GiveDiamond);
			timerC.customComponent = m_rightStarTC;
		}
	}

	private void GiveDiamond(TimerC _c)
	{
		Frame frame = m_planetNodeSpriteSheet.m_atlas.GetFrame("menu_diamond_small_full");
		TransformC transformC = _c.customComponent as TransformC;
		SpriteS.RemoveComponentsByTransformComponent(transformC);
		SpriteC spriteC = SpriteS.AddComponent(transformC, frame, m_planetNodeSpriteSheet);
		if (transformC == m_centerStarTC)
		{
			SpriteS.SetSortValue(spriteC, -1f);
		}
		SpriteS.SetDimensions(spriteC, 4f * (frame.width / frame.height), 4f);
		SpriteS.SetColor(spriteC, Color.white);
		TweenS.AddTransformTween(transformC, TweenedProperty.Scale, TweenStyle.CubicOut, Vector3.one * 1.25f, Vector3.one, 0.5f, 0f, true);
		TimerS.RemoveComponent(_c);
		SoundS.PlaySingleShot("/Metagame/StarAppear", Vector3.zero);
	}

	public override string GetTransformName()
	{
		return "FloaterDailyDiamond";
	}
}
