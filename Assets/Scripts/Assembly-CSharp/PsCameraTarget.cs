using UnityEngine;

public class PsCameraTarget : BasicAssembledClass
{
	public TransformC m_tc;

	public PsCameraTarget(GraphNode _graphNode)
		: base(_graphNode)
	{
		Entity entity = EntityManager.AddEntity();
		m_assembledEntities.Add(entity);
		m_tc = TransformS.AddComponent(entity, "Camera Target");
		TransformS.SetTransform(m_tc, _graphNode.m_position, _graphNode.m_rotation);
		if (!m_minigame.m_editing)
		{
			CameraTargetC cameraTargetC = CameraS.AddTargetComponent(m_tc, 400f, 400f, Vector2.zero);
			cameraTargetC.activeRadius = 1250f;
			return;
		}
		CreateGraphElementTouchArea(20f, m_tc);
		if (m_minigame.m_miniGameSpriteSheet != null)
		{
			string frameName = "hud_camera_target_eye";
			Frame frame = m_minigame.m_miniGameSpriteSheet.m_atlas.GetFrame(frameName);
			SpriteC spriteC = SpriteS.AddComponent(m_tc, frame, m_minigame.m_miniGameSpriteSheet);
			SpriteS.SetSortValue(spriteC, 1f);
			SpriteS.SetColor(spriteC, Color.white);
		}
	}
}
