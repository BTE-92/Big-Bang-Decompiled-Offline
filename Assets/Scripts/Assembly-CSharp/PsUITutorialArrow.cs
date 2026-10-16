using System.Collections.Generic;
using UnityEngine;

public class PsUITutorialArrow : PsUITutorialArrowBase
{
	public string m_target;

	public string m_entityName;

	public int m_id;

	protected Entity m_entityTarget;

	protected TransformC trans;

	protected bool m_transfound;

	protected bool hasTarget;

	public PsUITutorialArrow(string _entityTag, string _tcName, int _id = 0)
	{
		TouchAreaS.Enable();
		if (PsMetagameManager.m_tutorialArrow != null)
		{
			PsMetagameManager.m_tutorialArrow.Destroy();
		}
		PsMetagameManager.m_tutorialArrow = this;
		m_id = _id;
		m_entityName = _entityTag;
		m_target = _tcName;
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
		if (hasTarget)
		{
			DrawCanvas();
			SetCamera();
		}
	}

	public new virtual void GetTarget()
	{
		Entity entityTarget = null;
		List<Entity> entitiesByTag = EntityManager.GetEntitiesByTag(m_entityName);
		for (int i = 0; i < entitiesByTag.Count; i++)
		{
			TouchAreaC touchAreaC = null;
			for (int j = 0; j < entitiesByTag[i].m_components.Count; j++)
			{
				if (entitiesByTag[i].m_components[j].m_componentType == ComponentType.TouchArea)
				{
					touchAreaC = entitiesByTag[i].m_components[j] as TouchAreaC;
				}
				if (entitiesByTag[i].m_components[j].m_componentType == ComponentType.Transform && (entitiesByTag[i].m_components[j] as TransformC).transform.name == m_target)
				{
					trans = entitiesByTag[i].m_components[j] as TransformC;
					entityTarget = entitiesByTag[i];
					m_transfound = true;
				}
			}
			if (m_transfound)
			{
				if (touchAreaC == null)
				{
					Debug.LogError("Tutorial target " + m_entityName + " toucharea not found!");
				}
				m_touchArea = touchAreaC;
				break;
			}
		}
		if (!m_transfound)
		{
			Debug.LogWarning("Tutorial not found: " + m_target);
			m_destroy = true;
			return;
		}
		Debug.LogWarning("Tutorial gameobject found: " + m_target);
		m_entityTarget = entityTarget;
		GetTargetSprites(m_entityTarget);
		m_goTarget = trans.transform.gameObject;
		hasTarget = true;
	}

	public override void Step()
	{
		if (m_destroy)
		{
			Destroy();
			return;
		}
		if (!hasTarget)
		{
			GetTarget();
			if (hasTarget)
			{
				SetCamera();
				DrawCanvas();
			}
			else
			{
				m_destroy = true;
			}
		}
		if (!hasTarget)
		{
			return;
		}
		if (m_goTarget == null || m_goTarget.name != m_target)
		{
			hasTarget = false;
			TouchAreaS.RemoveTouchCameraFilter();
			m_canvas.Destroy();
		}
		else if (m_3Dcamera != null)
		{
			Vector3 vector = m_3Dcamera.WorldToViewportPoint(m_goTarget.transform.position);
			offset.SetAlign(vector.x, vector.y);
			offset.UpdateAlign();
			if (vector.x >= 0f && vector.x <= 1f && vector.y >= 0f && vector.y <= 1f)
			{
				TouchAreaS.SetTouchCameraFilter(m_3Dcamera);
			}
			else
			{
				TouchAreaS.RemoveTouchCameraFilter();
			}
		}
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
		if (m_transfound)
		{
			PrefabS.SetCamera(m_goTarget, m_originalCamera);
			if (m_spriteList != null && m_spriteList.Count > 0)
			{
				m_spriteList[0].p_spriteSheet.SetCamera(m_originalCamera);
			}
			CameraS.m_cameras.Remove(m_3Dcamera);
			Object.DestroyImmediate(m_3Dcamera.gameObject);
			TouchAreaS.RemoveTouchCameraFilter();
			TouchAreaS.RemoveTouchEventListener(m_touchArea, TouchHandler);
			TouchAreaS.SetCamera(m_touchArea, m_originalCamera);
		}
	}
}
