using System.Collections.Generic;
using UnityEngine;

public class PsUIProfileImage : UIComponent
{
	public TransformC m_prefabTC;

	public Texture2D m_texture;

	public List<PrefabC> m_prefabs;

	public string m_facebookId;

	public bool m_fetchingImage;

	public Material m_material;

	public bool m_shadow;

	public bool m_largePicture;

	public float m_borderWidth;

	public float m_borderRadius;

	public string m_borderColor;

	public PsUIProfileImage(UIComponent _parent, bool _touchable, string _tag, string _facebookId, string _gameCenterId, int _rank = -1, bool _loadImmediately = true, bool _shadow = false, bool _largePicture = false, float _borderWidth = 0.1f, float _borderRadius = 0.06f, string _borderColor = "fff9e6")
		: base(_parent, _touchable, _tag, null, null, string.Empty)
	{
		m_facebookId = _facebookId;
		m_fetchingImage = false;
		m_prefabTC = TransformS.AddComponent(m_TC.p_entity, "PrefabTC");
		TransformS.ParentComponent(m_prefabTC, m_TC, Vector3.zero);
		m_shadow = _shadow;
		m_largePicture = _largePicture;
		m_borderWidth = _borderWidth;
		m_borderRadius = _borderRadius;
		m_borderColor = _borderColor;
		SetMargins(-0.1f, RelativeTo.OwnHeight);
		if (_rank != -1)
		{
			UICanvas uICanvas = new UICanvas(this, false, string.Empty, null, string.Empty);
			uICanvas.SetAlign(0f, 1f);
			uICanvas.SetSize(0.275f, 0.275f, RelativeTo.ParentHeight);
			uICanvas.SetDrawHandler(RankDrawHandler);
			string text = (PsState.m_versusRankCap + 1 - _rank).ToString();
			UIFittedText uIFittedText = new UIFittedText(uICanvas, false, string.Empty, text, "HurmeRegular_Font", true, "000000");
			uIFittedText.SetSize(1f, 1f, RelativeTo.ParentHeight);
		}
		if (_loadImmediately)
		{
			LoadPicture();
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

	private void DisplayPicture(Texture2D _texture)
	{
		if (!(_texture != m_texture))
		{
			return;
		}
		m_texture = _texture;
		if (m_prefabTC != null)
		{
			PrefabS.RemoveComponentsByEntity(m_prefabTC.p_entity);
			if (m_material != null)
			{
				Object.Destroy(m_material);
			}
			m_material = Object.Instantiate(ResourceManager.GetMaterial(RESOURCE.ProfileMat_Material));
			m_material.mainTexture = m_texture;
			float actualWidth = m_actualWidth;
			float actualHeight = m_actualHeight;
			Vector2[] roundedRect = DebugDraw.GetRoundedRect(actualWidth, actualHeight, actualHeight * m_borderRadius, 4, Vector2.zero);
			m_prefabs = new List<PrefabC>();
			m_prefabs.Add(PrefabS.CreateFlatPrefabComponentsFromVectorArray(m_prefabTC, Vector3.forward * -0.1f, roundedRect, DebugDraw.HexToUint("ffffff"), DebugDraw.HexToUint("ffffff"), m_material, m_camera, "Profile", UVRect.Normal()));
			roundedRect = DebugDraw.GetRoundedRect(actualWidth + actualHeight * 0.025f, actualHeight + actualHeight * 0.025f, actualHeight * m_borderRadius, 4, Vector2.zero);
			m_prefabs.Add(PrefabS.CreatePathPrefabComponentFromVectorArray(m_prefabTC, new Vector3(0f, 0f, 0.01f), roundedRect, m_actualHeight * m_borderWidth, DebugDraw.HexToColor(m_borderColor), ResourceManager.GetMaterial("Framework/Line8Mat"), m_camera, Position.Center, true));
			if (m_shadow)
			{
				Color color = new Color(0f, 0f, 0f, 0.3f);
				m_prefabs.Add(PrefabS.CreatePathPrefabComponentFromVectorArray(m_prefabTC, new Vector3(m_actualHeight * 0.015f, (0f - m_actualHeight) * 0.015f, 0f), roundedRect, m_actualHeight * 0.18f, color, ResourceManager.GetMaterial("Framework/Line8Mat"), m_camera, Position.Center, true));
			}
			if (!m_TC.p_entity.m_visible)
			{
				PrefabS.SetVisibilityByTransformComponent(m_prefabTC, false);
			}
		}
	}

	public override void Update()
	{
		CalculateReferenceSizes();
		UpdateSize();
		UpdateMargins();
		UpdateAlign();
		if (!m_hidden && d_Draw != null)
		{
			d_Draw(this);
		}
		TransformS.SetPosition(m_prefabTC, new Vector2(m_actualMargins.l - m_actualMargins.r, m_actualMargins.b - m_actualMargins.t) * 0.5f);
		if (m_prefabs != null)
		{
			PrefabS.RemoveComponentsByEntity(m_prefabTC.p_entity);
		}
		if (m_material != null)
		{
			Object.Destroy(m_material);
		}
		m_material = Object.Instantiate(ResourceManager.GetMaterial(RESOURCE.ProfileMat_Material));
		m_material.mainTexture = m_texture;
		float actualWidth = m_actualWidth;
		float actualHeight = m_actualHeight;
		Vector2[] roundedRect = DebugDraw.GetRoundedRect(actualWidth, actualHeight, actualHeight * m_borderRadius, 4, Vector2.zero);
		m_prefabs = new List<PrefabC>();
		m_prefabs.Add(PrefabS.CreateFlatPrefabComponentsFromVectorArray(m_prefabTC, Vector3.forward * -0.1f, roundedRect, DebugDraw.HexToUint("ffffff"), DebugDraw.HexToUint("ffffff"), m_material, m_camera, "Profile", UVRect.Normal()));
		roundedRect = DebugDraw.GetRoundedRect(actualWidth + actualHeight * 0.025f, actualHeight + actualHeight * 0.025f, actualHeight * m_borderRadius, 4, Vector2.zero);
		m_prefabs.Add(PrefabS.CreatePathPrefabComponentFromVectorArray(m_prefabTC, Vector3.forward * -0.3f, roundedRect, m_actualHeight * m_borderWidth, DebugDraw.HexToColor(m_borderColor), ResourceManager.GetMaterial("Framework/Line8Mat"), m_camera, Position.Center, true));
		if (m_shadow)
		{
			Color color = new Color(0f, 0f, 0f, 0.3f);
			m_prefabs.Add(PrefabS.CreatePathPrefabComponentFromVectorArray(m_prefabTC, new Vector3(m_actualHeight * 0.015f, (0f - m_actualHeight) * 0.015f, 0f), roundedRect, m_actualHeight * 0.18f, color, ResourceManager.GetMaterial("Framework/Line8Mat"), m_camera, Position.Center, true));
		}
		UpdateUniqueCamera();
		UpdateChildren();
		ArrangeContents();
	}

	public override void Destroy()
	{
		m_texture = null;
		m_material = null;
		base.Destroy();
		m_prefabs.Clear();
		m_prefabTC = null;
	}

	public void RankDrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] circle = DebugDraw.GetCircle(_c.m_actualHeight * 0.5f, 16, Vector2.zero);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.zero, circle, DebugDraw.HexToUint("fff9e6"), DebugDraw.HexToUint("fff9e6"), ResourceManager.GetMaterial("Framework/SolidMat"), m_camera, "Rank", UVRect.Normal());
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * -0.1f, circle, _c.m_actualHeight * 0.3f, DebugDraw.HexToColor("fff9e6"), ResourceManager.GetMaterial("Framework/Line4Mat"), m_camera, Position.Center, true);
	}
}
