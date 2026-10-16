public class DangerousGround : Ground
{
	public DangerousGround(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_groundItemIdentifier = "MaterialDanger";
		m_editorWheelFrameName = "hud_button_material_danger";
		m_elasticity = 0.1f;
		m_friction = 0.8f;
		m_rectBrush = true;
		m_beltMaterialResourceName = "DangerBeltMat_Material";
		m_frontMaterialResourceName = "DangerFrontMat_Material";
		m_isElectrified = true;
		m_drawSound = "/UI/DrawLoop_Danger";
		m_drawFx = "DrawingDanger_GameObject";
	}
}
