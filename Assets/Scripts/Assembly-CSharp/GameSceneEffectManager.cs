using System;
using UnityEngine;

public static class GameSceneEffectManager
{
	private static Camera m_camera;

	public static OLDTVTube m_tvTube;

	private static Material m_flickMat;

	private static PrefabC m_flickPC;

	private static int m_numberOfImagesInChannelStrip;

	private static bool m_channelRoll = true;

	private static float m_channelRollSpeed = 0.005f;

	private static float m_channelRollStop = 1f;

	private static float m_staticRollSpeed = 0.01f;

	private static float m_noisePeak;

	private static float m_colorSaturation;

	private static float m_colorShiftPeak;

	private static float m_effectMultipler = 1f;

	private static bool m_effectsOn = true;

	private static bool m_saturationRoll = true;

	public static Entity m_utilityEntity;

	public static SoundC m_music;

	public static Action m_endSequenceCallback;

	public static PsDialogue m_dialogue;

	public static PsUICrewMemberBase m_crew;

	public static PsUIFixedDialogueSpeechBubble m_bubble;

	public static bool m_signalNoiseMusicPlaying;

	public static void Initialize(Camera _camera)
	{
		m_camera = _camera;
		m_tvTube = _camera.gameObject.AddComponent<OLDTVTube>();
		m_tvTube.tvMaterialTube = ResourceManager.GetMaterial(RESOURCE.RetroTVTube_Material);
		m_tvTube.tvMaterialTube.SetTexture("_Mask", ResourceManager.GetTexture(RESOURCE.RoundedCornerMask_Texture2D));
		m_tvTube.radialDistortion = 0.5f;
		m_tvTube.tvMaterialTube.SetTexture("_ChromaticAberrationTex", ResourceManager.GetTexture(RESOURCE.TVAberration_Texture2D));
		m_tvTube.tvMaterialTube.SetTexture("_NoiseTex", ResourceManager.GetTexture(RESOURCE.TVNoise_Texture2D));
		m_tvTube.tvMaterialTube.SetTexture("_StaticTex", ResourceManager.GetTexture(RESOURCE.TVPixel_Texture2D));
		m_tvTube.tvMaterialTube.SetTexture("_StaticMask", ResourceManager.GetTexture(RESOURCE.TVNoisePausePattern_Texture2D));
		m_tvTube.tvMaterialTube.SetFloat("_ScreenHorizontal", 0f);
		m_tvTube.noiseMagnetude = 0.1f;
		m_tvTube.chromaticAberrationMagnetude = 0.1f;
		m_tvTube.staticMagnetude = 0.25f;
		m_tvTube.screenSaturation = 1f;
		m_tvTube.enabled = false;
	}

	public static void StartMusic(string _sound, bool _signalNoisePlaying)
	{
		if (!PsState.m_muteMusic)
		{
			RemoveMusic();
			TransformC parentTC = EntityManager.AddEntityWithTC();
			m_music = SoundS.AddComponent(parentTC, _sound, 1f, true);
			SoundS.PlaySound(m_music, true);
		}
		m_signalNoiseMusicPlaying = _signalNoisePlaying;
	}

	public static void RemoveMusic()
	{
		if (m_music != null)
		{
			EntityManager.RemoveEntity(m_music.p_entity);
			m_music = null;
		}
		m_signalNoiseMusicPlaying = false;
	}

	public static void CreateTVChannels(string _materialResource, int _numberOfImagesInChannelStrip)
	{
		m_numberOfImagesInChannelStrip = _numberOfImagesInChannelStrip;
		m_flickMat = ResourceManager.GetMaterial(_materialResource);
		Vector2[] rect = DebugDraw.GetRect(134.4f, 89.6f, Vector2.zero);
		UVRect normalizeRect = new UVRect(0f, 0f, 1f / (float)m_numberOfImagesInChannelStrip, 1f);
		m_utilityEntity = EntityManager.AddEntity();
		m_utilityEntity.m_persistent = true;
		TransformC transformC = TransformS.AddComponent(m_utilityEntity);
		transformC.transform.parent = m_camera.transform;
		transformC.transform.localPosition = Vector3.forward * 100f;
		transformC.transform.localRotation = Quaternion.identity;
		m_flickPC = PrefabS.CreateFlatPrefabComponentsFromVectorArray(transformC, Vector3.zero, rect, DebugDraw.ColorToUInt(Color.white), DebugDraw.ColorToUInt(Color.white), m_flickMat, m_camera, string.Empty, normalizeRect);
		m_flickPC.p_gameObject.layer = 10;
		DrawMainLayerOn();
	}

	public static void DrawMainLayerOn()
	{
		m_camera.cullingMask |= 256;
		m_camera.cullingMask |= 512;
	}

	public static void DrawMainLayerOff()
	{
		m_camera.cullingMask &= -257;
		m_camera.cullingMask &= -513;
	}

	public static void Enable()
	{
		m_tvTube.enabled = true;
	}

	public static void Disable()
	{
		m_tvTube.enabled = false;
	}

	public static void Destroy()
	{
		UnityEngine.Object.Destroy(m_tvTube);
		m_tvTube = null;
		m_flickMat = null;
		m_flickPC = null;
		DestroyCrew();
		EntityManager.RemoveEntity(m_utilityEntity);
		m_utilityEntity = null;
		RemoveMusic();
	}

	public static void SetChannel(int _channel)
	{
		if (_channel == -1)
		{
			DrawMainLayerOn();
			m_flickPC.p_gameObject.GetComponent<Renderer>().enabled = false;
		}
		else
		{
			DrawMainLayerOff();
			m_flickPC.p_gameObject.GetComponent<Renderer>().enabled = true;
		}
		Vector2 textureOffset = m_flickMat.GetTextureOffset("_MainTex");
		m_flickMat.SetTextureOffset("_MainTex", new Vector2((float)_channel * (1f / (float)m_numberOfImagesInChannelStrip), textureOffset.y));
	}

	public static void SetChannelRoll(bool _on)
	{
		if (_on)
		{
			m_channelRollSpeed = -0.1f;
		}
		else
		{
			m_channelRollStop = 0f;
		}
		m_channelRoll = _on;
	}

	public static void SetEffects(bool _on)
	{
		m_effectsOn = _on;
	}

	public static void Update()
	{
		if ((object)m_flickMat != null && (object)m_tvTube != null)
		{
			Vector2 textureOffset = m_flickMat.GetTextureOffset("_MainTex");
			Vector2 textureOffset2 = m_tvTube.tvMaterialTube.GetTextureOffset("_StaticMask");
			if (!m_channelRoll)
			{
				m_channelRollSpeed = (m_channelRollStop - textureOffset.y) * 0.1f;
			}
			m_flickMat.SetTextureOffset("_MainTex", new Vector2(textureOffset.x, ToolBox.getRolledValue(textureOffset.y + m_channelRollSpeed, 0f, 1f)));
			m_staticRollSpeed = 0.005f + Mathf.Sin(Main.m_resettingGameTime * Mathf.Sin(Main.m_resettingGameTime * 0.0618f)) * 0.01f;
			m_tvTube.tvMaterialTube.SetTextureOffset("_StaticMask", new Vector2(textureOffset2.x, ToolBox.getRolledValue(textureOffset2.y + m_staticRollSpeed, 0f, 1f)));
			if (m_effectsOn)
			{
				m_effectMultipler += (1f - m_effectMultipler) * 0.05f;
			}
			else
			{
				m_effectMultipler *= 0.95f;
			}
			m_tvTube.noiseMagnetude = 0.1f * m_effectMultipler + 0.1f;
			m_tvTube.chromaticAberrationMagnetude = 0.1f * m_effectMultipler + 0.025f;
			m_tvTube.staticMagnetude = 0.125f * m_effectMultipler + 0.0125f;
		}
	}

	public static void DestroyCrew()
	{
		if (m_crew != null)
		{
			m_crew.Destroy();
			m_crew = null;
		}
		if (m_bubble != null)
		{
			m_bubble.Destroy();
			m_bubble = null;
		}
		DrawMainLayerOn();
	}
}
