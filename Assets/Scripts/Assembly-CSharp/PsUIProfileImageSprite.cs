using System.Collections.Generic;
using UnityEngine;

public class PsUIProfileImageSprite
{
	private string m_facebookId;

	private bool m_fetchingImage;

	private Camera m_camera;

	private Texture2D m_texture;

	private List<PrefabC> m_prefabs;

	private Material m_material;

	private float m_borderWidth = 0.1f;

	private float m_borderRadius = 0.06f;

	private string m_borderColor = "fff9e6";

	private float m_actualHeight = 1f;

	private float m_actualWidth = 1f;

	private float m_width = 1f;

	private float m_widthRatio = 1f;

	private bool m_largePicture = true;

	private bool m_shadow;

	private TransformC m_TC;

	public PsUIProfileImageSprite(string _facebookId, Camera _camera, TransformC _parent)
	{
		m_facebookId = _facebookId;
		m_camera = _camera;
		m_TC = TransformS.AddComponent(_parent.p_entity);
		TransformS.ParentComponent(m_TC, _parent);
		TransformS.SetPosition(m_TC, Vector3.zero);
		TransformS.SetRotation(m_TC, Vector3.zero);
		LoadPicture();
	}

	public void LoadPicture()
	{
		if (m_facebookId != null)
		{
			Texture2D content = PsCaches.m_profileImages.GetContent(m_facebookId);
			if (content != null)
			{
				DisplayPicture(content);
			}
			else if (!m_fetchingImage)
			{
				HttpC picture = FacebookManager.GetPicture(m_facebookId, PictureDownloadOk, PictureDownloadFailed, m_largePicture);
				EntityManager.AddComponentToEntity(m_TC.p_entity, picture);
				m_fetchingImage = true;
			}
		}
		else
		{
			DisplayPicture(ResourceManager.GetTexture(RESOURCE.AnonymousProfile_Texture2D) as Texture2D);
		}
	}

	private void PictureDownloadFailed(HttpC _c)
	{
		DisplayPicture(ResourceManager.GetTexture(RESOURCE.AnonymousProfile_Texture2D) as Texture2D);
		m_fetchingImage = false;
	}

	private void PictureDownloadOk(Texture2D _texture)
	{
		PsCaches.m_profileImages.AddItem(m_facebookId, _texture);
		DisplayPicture(_texture);
		m_fetchingImage = false;
	}

	private void DisplayPicture(Texture2D _texture)
	{
		if (!(_texture != m_texture))
		{
			return;
		}
		m_texture = _texture;
		if (m_TC != null)
		{
			if (m_material != null)
			{
				Object.Destroy(m_material);
			}
			m_material = Object.Instantiate(ResourceManager.GetMaterial(RESOURCE.ProfileMat_Material));
			m_material.mainTexture = m_texture;
			float width = m_width;
			float num = m_widthRatio * m_width;
			Vector2[] roundedRect = DebugDraw.GetRoundedRect(width, num, num * m_borderRadius, 4, Vector2.zero);
			m_prefabs = new List<PrefabC>();
			m_prefabs.Add(PrefabS.CreateFlatPrefabComponentsFromVectorArray(m_TC, Vector3.forward * -0.1f, roundedRect, DebugDraw.HexToUint("ffffff"), DebugDraw.HexToUint("ffffff"), m_material, m_camera, "Profile", UVRect.Normal()));
			roundedRect = DebugDraw.GetRoundedRect(width + num * 0.025f, num + num * 0.025f, num * m_borderRadius, 4, Vector2.zero);
			m_prefabs.Add(PrefabS.CreatePathPrefabComponentFromVectorArray(m_TC, Vector3.forward * -0.15f, roundedRect, m_actualHeight * m_borderWidth, DebugDraw.HexToColor(m_borderColor), ResourceManager.GetMaterial("Framework/Line8Mat"), m_camera, Position.Center, true));
			if (m_shadow)
			{
				Color color = new Color(0f, 0f, 0f, 0.3f);
				m_prefabs.Add(PrefabS.CreatePathPrefabComponentFromVectorArray(m_TC, new Vector3(m_actualHeight * 0.015f, (0f - m_actualHeight) * 0.015f, 0f), roundedRect, m_actualHeight * 0.18f, color, ResourceManager.GetMaterial("Framework/Line8Mat"), m_camera, Position.Center, true));
			}
			TransformS.Scale(m_TC, new Vector3(3.6f, 3.6f, 3.6f));
			if (!m_TC.p_entity.m_visible)
			{
				PrefabS.SetVisibilityByTransformComponent(m_TC, false);
			}
		}
	}

	public void Destroy()
	{
	}
}
