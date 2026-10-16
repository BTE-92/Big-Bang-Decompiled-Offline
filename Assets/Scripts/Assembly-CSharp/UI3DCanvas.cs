using System.Runtime.InteropServices;
using UnityEngine;

public class UI3DCanvas : UICanvas
{
	public TransformC m_cameraPivot;

	private TransformC m_content;

	public float m_cameraOffset = -500f;

	public UI3DCanvas(UIComponent _parent, string _tag)
		: base(_parent, true, _tag, null, string.Empty)
	{
		m_cameraPivot = TransformS.AddComponent(m_TAC.p_entity, "UI 3D Camera pivot");
		TransformS.ParentComponent(m_cameraPivot, m_TC, Vector3.zero);
		m_uniqueCamera = true;
		m_camera = CameraS.AddCamera("UI 3D Camera", false);
		m_camera.transform.SetParent(m_cameraPivot.transform);
		m_camera.transform.localPosition = Vector3.zero;
		m_content = TransformS.AddComponent(m_TAC.p_entity, "Content");
		TransformS.ParentComponent(m_content, m_TC, Vector3.zero);
		if (m_TAC != null)
		{
			m_TAC.m_camera = m_camera;
			m_TAC.m_TC.transform.gameObject.layer = m_camera.gameObject.layer;
		}
		m_TC.transform.gameObject.layer = m_camera.gameObject.layer;
		m_cameraPivot.transform.gameObject.layer = m_camera.gameObject.layer;
		foreach (IComponent component in m_TC.p_entity.m_components)
		{
			if (component.m_componentType == ComponentType.TouchArea)
			{
				TouchAreaS.SetCamera(component as TouchAreaC, m_camera);
			}
			else if (component.m_componentType == ComponentType.Prefab)
			{
				PrefabS.SetCamera(component as PrefabC, m_camera);
			}
		}
		AdjustCamera();
	}

	public virtual PrefabC AddGameObject(GameObject _go, Vector3 _position, Vector3 _rotaion = default(Vector3))
	{
		PrefabC prefabC = PrefabS.AddComponent(m_content, _position, _go, _go.name);
		PrefabS.SetCamera(prefabC, m_camera);
		prefabC.p_gameObject.transform.rotation = Quaternion.Euler(_rotaion);
		return prefabC;
	}

	public virtual void AdjustCamera()
	{
		if (!(m_camera != null))
		{
			return;
		}
		UIComponent parent = m_parent;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		float num = 1f * (float)Screen.width;
		float num2 = 0f * (float)Screen.width;
		while (parent != null)
		{
			if (parent is UIScrollableCanvas)
			{
				UIScrollableCanvas uIScrollableCanvas = parent as UIScrollableCanvas;
				zero += uIScrollableCanvas.m_scrollTC.transform.localPosition;
				num = uIScrollableCanvas.m_camera.rect.xMax * (float)Screen.width;
				num2 = uIScrollableCanvas.m_camera.rect.xMin * (float)Screen.width;
				break;
			}
			zero2 += parent.m_TC.transform.localPosition;
			parent = parent.m_parent;
		}
		Vector3 position = m_TC.transform.position;
		float num3 = position.x - zero.x - m_actualWidth * 0.5f + (float)Screen.width * 0.5f;
		float actualWidth = m_actualWidth;
		float num4 = 0f;
		float num5 = 0f;
		if (num3 < num2)
		{
			num4 = num2 - num3;
			num3 = num2;
		}
		if (num3 + actualWidth > num)
		{
			num5 = num3 + actualWidth - num;
		}
		float num6 = position.y + (0f - zero.y) - m_actualHeight * 0.5f;
		float actualHeight = m_actualHeight;
		float y = m_camera.transform.localPosition.y;
		m_camera.transform.localPosition = new Vector3((0f - num5 + num4) * 0.31f, y, m_cameraOffset);
		Rect rect = new Rect(num3 / (float)Screen.width, num6 / (float)Screen.height + 0.5f, (actualWidth - num4 - num5) / (float)Screen.width, actualHeight / (float)Screen.height);
		if (rect.width < 0f)
		{
			rect.width = 0f;
		}
		if (rect.height < 0f)
		{
			rect.height = 0f;
		}
		m_camera.rect = rect;
		if (m_TAC != null)
		{
			m_TAC.m_clip = true;
			m_TAC.m_clipBB.l = rect.xMin * (float)Screen.width;
			m_TAC.m_clipBB.r = rect.xMax * (float)Screen.width;
			m_TAC.m_clipBB.b = rect.yMin * (float)Screen.height;
			m_TAC.m_clipBB.t = rect.yMax * (float)Screen.height;
		}
	}

	public override void Update()
	{
		CalculateReferenceSizes();
		UpdateSize();
		UpdateAlign();
		UpdateMargins();
		if (d_Draw != null)
		{
			d_Draw(this);
		}
		UpdateChildren();
		ArrangeContents();
		if (m_parent == null)
		{
			UpdateSpecial();
		}
	}

	public override void UpdateSpecial()
	{
		UpdateUniqueCamera();
		base.UpdateSpecial();
	}

	public override void UpdateSize()
	{
		base.UpdateSize();
	}

	public override void CalculateReferenceSizes()
	{
		base.CalculateReferenceSizes();
	}

	public override void UpdateUniqueCamera()
	{
		m_camera.transform.localPosition = Vector3.forward * m_cameraOffset;
		AdjustCamera();
	}

	public override void Step()
	{
		AdjustCamera();
		base.Step();
	}
}
