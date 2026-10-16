public class LavaGround : Ground
{
	public LavaGround(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_groundItemIdentifier = "MaterialLava";
		m_editorWheelFrameName = "hud_button_material_lava";
		m_elasticity = 0f;
		m_friction = 2f;
		m_segmentRadius = 8f;
		m_beltMaterialResourceName = "LavaBeltMat_Material";
		m_frontMaterialResourceName = "LavaFrontMat_Material";
		m_isBurning = true;
		m_drawSound = "/UI/DrawLoop_Danger";
		m_drawFx = "DrawingDanger_GameObject";
	}
}
