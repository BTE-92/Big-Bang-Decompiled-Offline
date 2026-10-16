public class IceGround : Ground
{
	public IceGround(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_groundItemIdentifier = "MaterialIce";
		m_editorWheelFrameName = "hud_button_material_ice";
		m_elasticity = 0.8f;
		m_friction = 0.3f;
		m_beltMaterialResourceName = "IceBeltMat_Material";
		m_frontMaterialResourceName = "IceFrontMat_Material";
		m_tireRollSound = "/InGame/Vehicles/TireRollLoop_Ice";
		m_drawSound = "/UI/DrawLoop_Ice";
		m_driveFX = "DrivingIce_GameObject";
		m_drawFx = "DrawingIce_GameObject";
		m_destructible = true;
		m_isFrozen = true;
	}
}
