public class MudGround : Ground
{
	public MudGround(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_groundItemIdentifier = "MaterialMud";
		m_editorWheelFrameName = "hud_button_material_mud";
		m_elasticity = 0.1f;
		m_friction = 0.4f;
		m_segmentRadius = 8f;
		m_beltMaterialResourceName = "MudBeltMat_Material";
		m_frontMaterialResourceName = "MudFrontMat_Material";
		m_destructible = true;
		m_skiddable = true;
		m_depth = 150f;
		m_zOffset = 10f;
		m_tireRollSound = "/InGame/Vehicles/TireRollLoop_Mud";
		m_drawSound = "/UI/DrawLoop_Mud";
		m_driveFX = "MudSplatter_GameObject";
		m_drawFx = "DrawingMud_GameObject";
	}
}
