using UnityEngine;

public class PsUITutorialNodeArrow : PsUITutorialArrowBase
{
	private PsPlanetNode m_node;

	public PsUITutorialNodeArrow(PsPlanetNode _node)
	{
		m_node = _node;
		TouchAreaS.Enable();
		if (PsMetagameManager.m_tutorialArrow != null)
		{
			PsMetagameManager.m_tutorialArrow.Destroy();
		}
		PsMetagameManager.m_tutorialArrow = this;
		int num = 11;
		int num2 = 10;
		GameObject gameObject = new GameObject("3D Tutorial Camera");
		m_3Dcamera = gameObject.AddComponent<Camera>();
		CameraS.m_cameras.Add(m_3Dcamera);
		m_originalCamera = GameObject.Find("PlanetCamera").GetComponent<Camera>();
		m_3Dcamera.CopyFrom(m_originalCamera);
		m_3Dcamera.clearFlags = CameraClearFlags.Depth;
		m_3Dcamera.depth = 15f;
		m_3Dcamera.gameObject.layer = num;
		m_3Dcamera.cullingMask = 0;
		m_3Dcamera.cullingMask |= 1 << num;
		GetTarget();
		DrawCanvas();
		GetTargetSprites(m_node.m_tc.p_entity);
		SetCamera();
	}

	protected override void GetTarget()
	{
		m_goTarget = m_node.m_prefab.p_gameObject;
		m_touchArea = m_node.m_tac;
	}

	protected override void SetCamera()
	{
		base.SetCamera();
		if (m_node.m_highlightTC != null)
		{
			PrefabS.SetCamera(m_node.m_highlightTC.transform.gameObject, m_3Dcamera);
		}
	}

	public override void Step()
	{
		if (m_destroy)
		{
			Destroy();
			return;
		}
		Vector3 vector = m_3Dcamera.WorldToViewportPoint(m_goTarget.transform.position);
		offset.SetAlign(vector.x, vector.y);
		offset.UpdateAlign();
	}

	public override void Destroy()
	{
		if (PsMetagameManager.m_tutorialArrow == this)
		{
			PsMetagameManager.m_tutorialArrow = null;
		}
		if (m_canvas != null)
		{
			if (m_fadeOutSpriteSheet != null)
			{
				SpriteS.RemoveSpriteSheet(m_fadeOutSpriteSheet);
			}
			m_fadeOutSpriteSheet = null;
			m_canvas.Destroy();
		}
		PrefabS.SetCamera(m_goTarget, m_originalCamera);
		PrefabS.SetCamera(m_node.m_highlightTC.transform.gameObject, m_originalCamera);
		if (m_spriteList.Count > 0)
		{
			m_spriteList[0].p_spriteSheet.SetCamera(m_originalCamera);
		}
		CameraS.m_cameras.Remove(m_3Dcamera);
		Object.DestroyImmediate(m_3Dcamera.gameObject);
		TouchAreaS.RemoveTouchCameraFilter();
		TouchAreaS.RemoveTouchEventListener(m_node.m_tac, TouchHandler);
		TouchAreaS.SetCamera(m_node.m_tac, m_originalCamera);
	}
}
