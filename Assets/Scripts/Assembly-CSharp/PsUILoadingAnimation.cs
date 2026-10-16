using UnityEngine;

public class PsUILoadingAnimation : UIComponent
{
	protected Vector2[] m_circleShape;

	protected TransformC m_signalTC1;

	protected TransformC m_signalTC2;

	protected TransformC m_signalTC3;

	protected PrefabC m_signalPC1;

	protected PrefabC m_signalPC2;

	protected PrefabC m_signalPC3;

	public UIFittedSprite m_earth;

	public PsUILoadingAnimation(UIComponent _parent, bool _rogue)
		: base(_parent, false, string.Empty, null, null, string.Empty)
	{
		SetSize(0.2f, 0.2f, RelativeTo.ScreenHeight);
		SetAlign(0.5f, 0.5f);
		if (_rogue)
		{
			SetRogue();
		}
		RemoveDrawHandler();
		m_earth = new UIFittedSprite(this, false, "LoadingBar", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_loading_earth"));
		m_earth.SetWidth(0.5f, RelativeTo.ParentWidth);
	}

	public override void Step()
	{
		if (m_circleShape == null)
		{
			m_circleShape = DebugDraw.GetCircle(m_actualWidth * 0.2f, 36, Vector2.zero);
			m_signalTC1 = TransformS.AddComponent(m_TC.p_entity, m_TC.transform.position);
			m_signalTC2 = TransformS.AddComponent(m_TC.p_entity, m_TC.transform.position);
			m_signalTC3 = TransformS.AddComponent(m_TC.p_entity, m_TC.transform.position);
			m_signalPC1 = PrefabS.CreatePathPrefabComponentFromVectorArray(m_signalTC1, new Vector3(0f, 0f, -10f), m_circleShape, m_actualWidth * 0.025f, Color.white, ResourceManager.GetMaterial(RESOURCE.LoadingBallEmitMat_Material), m_camera, Position.Center, true);
			m_signalPC2 = PrefabS.CreatePathPrefabComponentFromVectorArray(m_signalTC2, new Vector3(0f, 0f, -10f), m_circleShape, m_actualWidth * 0.025f, Color.white, ResourceManager.GetMaterial(RESOURCE.LoadingBallEmitMat_Material), m_camera, Position.Center, true);
			m_signalPC3 = PrefabS.CreatePathPrefabComponentFromVectorArray(m_signalTC3, new Vector3(0f, 0f, -10f), m_circleShape, m_actualWidth * 0.025f, Color.white, ResourceManager.GetMaterial(RESOURCE.LoadingBallEmitMat_Material), m_camera, Position.Center, true);
		}
		float num = (float)(Main.m_gameTicks % 120) / 120f;
		float num2 = (float)((Main.m_gameTicks + 40) % 120) / 120f;
		float num3 = (float)((Main.m_gameTicks + 80) % 120) / 120f;
		TransformS.SetScale(m_signalTC1, num * 0.75f + 1f);
		TransformS.SetScale(m_signalTC2, num2 * 0.75f + 1f);
		TransformS.SetScale(m_signalTC3, num3 * 0.75f + 1f);
		PrefabS.SetShaderColor(m_signalPC1, new Color(1f, 1f, 1f, 0.75f - num * 0.75f));
		PrefabS.SetShaderColor(m_signalPC2, new Color(1f, 1f, 1f, 0.75f - num2 * 0.75f));
		PrefabS.SetShaderColor(m_signalPC3, new Color(1f, 1f, 1f, 0.75f - num3 * 0.75f));
		base.Step();
	}
}
