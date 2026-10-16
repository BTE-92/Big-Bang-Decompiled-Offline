public class SandGround : Ground
{
	public SandGround(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_groundItemIdentifier = "MaterialSand";
		m_editorWheelFrameName = "hud_button_material_dirt";
		m_elasticity = 0.5f;
		m_friction = 0.7f;
		m_beltMaterialResourceName = "SandBeltMat_Material";
		m_frontMaterialResourceName = "SandFrontMat_Material";
		m_groundPropStorageName = "SandGroundPropStorage_GameObject";
		m_tireRollSound = "/InGame/Vehicles/TireRollLoop_Sand";
		m_drawSound = "/UI/DrawLoop_Sand";
		m_driveFX = "DrivingSand_GameObject";
		m_drawFx = "DrawingSand_GameObject";
		m_destructible = true;
	}
}
