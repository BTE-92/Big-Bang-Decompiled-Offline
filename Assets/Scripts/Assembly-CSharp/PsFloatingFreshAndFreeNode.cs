using UnityEngine;

public class PsFloatingFreshAndFreeNode : PsFloatingPlanetNode
{
	public PsFloatingFreshAndFreeNode(PsTimedEventLoop _loop, bool _tutorial = false)
		: base(_loop, _tutorial)
	{
	}

	public override void SetPrefabName()
	{
		m_prefabName = "FloatingPlatformDailyDiamondsPrefab";
	}

	public override void CreateUI()
	{
		base.CreateUI();
		Frame frame = m_planetNodeSpriteSheet.m_atlas.GetFrame("menu_hovertext_new");
		SpriteC spriteC = SpriteS.AddComponent(m_domeNumberTC, frame, m_planetNodeSpriteSheet);
		SpriteS.SetDimensions(spriteC, 3f * (frame.width / frame.height), 3f);
		SpriteS.SetColor(spriteC, new Color(1f, 1f, 1f));
		SpriteS.SetOffset(spriteC, new Vector3(-2.5f, 7.5f, 0f), 20f);
	}

	public override string GetTransformName()
	{
		return "FloaterFreshAndFree";
	}
}
