public class ConcreteGround : Ground
{
	public ConcreteGround(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_groundItemIdentifier = "MaterialConcrete";
		m_editorWheelFrameName = "hud_button_material_concrete";
		m_elasticity = 0.5f;
		m_friction = 0.7f;
		m_beltMaterialResourceName = "ConcreteBeltMat_Material";
		m_frontMaterialResourceName = "ConcreteFrontMat_Material";
		m_tireRollSound = "/InGame/Vehicles/TireRollLoop_Sand";
		m_drawSound = "/UI/DrawLoop_Sand";
		m_driveFX = "DrivingSand_GameObject";
		m_drawFx = "DrawingSand_GameObject";
		m_destructible = false;
	}
}
