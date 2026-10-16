public class MetalGround : Ground
{
	public MetalGround(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_groundItemIdentifier = "MaterialMetal";
		m_editorWheelFrameName = "hud_button_material_metal";
		m_elasticity = 0.8f;
		m_friction = 0.85f;
		m_rectBrush = true;
		m_marchHard = false;
		m_smoothingAngle = 0f;
		m_beltMaterialResourceName = "MetalBeltMat_Material";
		m_frontMaterialResourceName = "MetalFrontMat_Material";
		m_tireRollSound = "/InGame/Vehicles/TireRollLoop_Metal";
		m_drawSound = "/UI/DrawLoop_Metal";
	}
}
