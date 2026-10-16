using UnityEngine;

public class PsUICheckpointBanner
{
	public enum State
	{
		FOLLOW = 0,
		STAND = 1,
		UPDATE = 2
	}

	public PsGameLoop m_gameLoop;

	public TransformC m_bannerTC;

	public TransformC m_checkpointTC;

	private TextMeshC m_title;

	private TextMeshC m_titleShadow;

	private TextMeshC m_starCounter;

	private SpriteC m_backgroundSprite;

	private SpriteC m_arrowSprite;

	private SpriteC m_starSprite;

	private SpriteC m_iconSprite;

	private SpriteC m_itemSprite;

	private float m_arrowHeight;

	private float m_arrowWidth;

	private int m_requiredStars;

	private int m_lastEarnedStars;

	private float m_xOffset;

	private float m_yOffset = 4f;

	private float m_zOffset = -10f;

	private float m_width = 21f;

	private float m_height;

	private float m_widthRatio;

	public State m_state;

	public bool m_hidden;

	private TweenC m_tweenC;

	public PsUICheckpointBanner(PsGameLoop _gameLoop, TransformC _parent, TransformC _checkpointTC)
	{
		m_gameLoop = _gameLoop;
		m_state = State.UPDATE;
		m_checkpointTC = _checkpointTC;
		m_bannerTC = _parent;
		SpriteSheet planetSpriteSheet = m_gameLoop.m_planet.m_planetSpriteSheet;
		string text = string.Empty;
		string hex = "#ffffff";
		string text2 = string.Empty;
		int num = 0;
		if (m_gameLoop is PsGameLoopBlockSignal)
		{
			m_requiredStars = (m_gameLoop as PsGameLoopBlockSignal).m_requiredStars;
			text = (m_gameLoop as PsGameLoopBlockSignal).m_rewardCue;
			hex = "#9aff0a";
			text2 = "menu_cp_image_signal";
		}
		else if (m_gameLoop is PsGameLoopBlockSignalWithoutLevel)
		{
			m_requiredStars = (m_gameLoop as PsGameLoopBlockSignalWithoutLevel).m_requiredStars;
			text = (m_gameLoop as PsGameLoopBlockSignalWithoutLevel).m_rewardCue;
			hex = "#9aff0a";
			text2 = "menu_cp_image_signal";
		}
		else if (m_gameLoop is PsGameLoopBlockLevel)
		{
			m_requiredStars = (m_gameLoop as PsGameLoopBlockLevel).m_requiredStars;
			text = (m_gameLoop as PsGameLoopBlockLevel).m_rewardCue;
			switch ((m_gameLoop as PsGameLoopBlockLevel).m_blockType)
			{
			case PsPathBlockType.Chest:
				text = "Treasure";
				text2 = "menu_cp_image_treasure";
				hex = "#ff95ef";
				break;
			case PsPathBlockType.Construction:
				text2 = "menu_cp_image_builder";
				hex = "#fcff00";
				break;
			case PsPathBlockType.Garage:
				text2 = "menu_cp_image_mechanic";
				hex = "#ffcc00";
				break;
			}
		}
		if (_gameLoop.m_planet.m_banners.Count == 0)
		{
			num = PsMetagameManager.m_playerStats.stars;
		}
		if (!string.IsNullOrEmpty(text))
		{
			text = text.ToUpper();
		}
		Frame frame = planetSpriteSheet.m_atlas.GetFrame("menu_cp_sign_1");
		m_height = frame.height / frame.width * m_width;
		m_yOffset += m_height / 2f;
		m_backgroundSprite = SpriteS.AddComponent(m_bannerTC, frame, planetSpriteSheet);
		SpriteS.SetDimensions(m_backgroundSprite, m_width, m_height);
		SpriteS.SetOffset(m_backgroundSprite, new Vector3(m_xOffset, m_yOffset, m_zOffset), 0f);
		SpriteS.SetColor(m_backgroundSprite, Color.white);
		SpriteS.SetSortValue(m_backgroundSprite, 0f);
		Frame frame2 = planetSpriteSheet.m_atlas.GetFrame("menu_cp_arrow_1");
		m_arrowSprite = SpriteS.AddComponent(m_bannerTC, frame2, planetSpriteSheet);
		m_widthRatio = frame2.width / frame.width;
		m_arrowWidth = m_width * m_widthRatio;
		m_arrowHeight = frame2.height / frame2.width * m_arrowWidth;
		SetArrowShape(0f);
		SpriteS.SetColor(m_arrowSprite, Color.white);
		SpriteS.SetSortValue(m_arrowSprite, 1f);
		Frame frame3 = planetSpriteSheet.m_atlas.GetFrame("menu_cp_star");
		m_starSprite = SpriteS.AddComponent(m_bannerTC, frame3, planetSpriteSheet);
		m_widthRatio = frame3.width / frame.width;
		SpriteS.SetDimensions(m_starSprite, m_width * m_widthRatio, frame3.height / frame3.width * m_width * m_widthRatio);
		SpriteS.SetOffset(m_starSprite, new Vector3(-0.02f * m_width + m_xOffset, -0.25f * m_height + m_yOffset, -0.2f + m_zOffset), 0f);
		SpriteS.SetColor(m_starSprite, Color.white);
		SpriteS.SetSortValue(m_starSprite, -2f);
		if (!string.IsNullOrEmpty(text2))
		{
			Frame frame4 = planetSpriteSheet.m_atlas.GetFrame(text2);
			m_iconSprite = SpriteS.AddComponent(m_bannerTC, frame4, planetSpriteSheet);
			m_widthRatio = frame4.width / frame.width;
			SpriteS.SetDimensions(m_iconSprite, m_width * m_widthRatio, frame4.height / frame4.width * m_width * m_widthRatio);
			SpriteS.SetOffset(m_iconSprite, new Vector3(-0.311f * m_width + m_xOffset, 0.105f * m_height + m_yOffset, -0.1f + m_zOffset), 0f);
			SpriteS.SetColor(m_iconSprite, Color.white);
			SpriteS.SetSortValue(m_iconSprite, -2f);
		}
		if (m_gameLoop is PsGameLoopBlockSignal || m_gameLoop is PsGameLoopBlockSignalWithoutLevel)
		{
			PsUnlock psUnlock = null;
			psUnlock = ((!(m_gameLoop is PsGameLoopBlockSignal)) ? PsMetagameData.GetUnlockByIdentifier((m_gameLoop as PsGameLoopBlockSignalWithoutLevel).m_willUnlockProgression) : PsMetagameData.GetUnlockByIdentifier((m_gameLoop as PsGameLoopBlockSignal).m_willUnlockProgression));
			if (psUnlock != null)
			{
				for (int i = 0; i < psUnlock.m_unlockables.Count; i++)
				{
					if (!string.IsNullOrEmpty(psUnlock.m_unlockables[i].m_iconImage))
					{
						Frame frame5 = planetSpriteSheet.m_atlas.GetFrame(psUnlock.m_unlockables[i].m_iconImage);
						if (frame5 != null)
						{
							m_itemSprite = SpriteS.AddComponent(m_bannerTC, frame5, planetSpriteSheet);
							m_widthRatio = 0.33f;
							SpriteS.SetDimensions(m_itemSprite, m_width * m_widthRatio, frame5.height / frame5.width * m_width * m_widthRatio);
							SpriteS.SetOffset(m_itemSprite, new Vector3(-0.305f * m_width + m_xOffset, 0.105f * m_height + m_yOffset, -0.2f + m_zOffset), 0f);
							SpriteS.SetColor(m_itemSprite, Color.black);
							SpriteS.SetSortValue(m_itemSprite, -3f);
						}
						break;
					}
				}
			}
		}
		Shader shader = Shader.Find("Framework/FontShaderFg");
		m_starCounter = TextMeshS.AddComponent(m_bannerTC, new Vector3(0.275f * m_width + m_xOffset, -0.25f * m_height + m_yOffset, -0.2f + m_zOffset), "KGSecondChances_Font", 2f, 2f, m_height * 2.9f, Align.Center, Align.Middle, m_gameLoop.m_planet.m_planetCamera, string.Empty);
		m_starCounter.m_textMesh.characterSize = 1f;
		if (!m_gameLoop.m_unlocked)
		{
			TextMeshS.SetText(m_starCounter, num + " / " + m_requiredStars, false);
		}
		else
		{
			TextMeshS.SetText(m_starCounter, "Unlocked", false);
			m_starCounter.m_textMesh.characterSize = 0.65f;
		}
		m_starCounter.m_go.transform.localScale = Vector3.one;
		m_starCounter.m_renderer.material.shader = shader;
		m_title = TextMeshS.AddComponent(m_bannerTC, new Vector3(-0.1f * m_width + m_xOffset, 0.13f * m_height + m_yOffset, -0.3f + m_zOffset), "KGSecondChances_Font", 2f, 2f, m_height * 2f, Align.Left, Align.Middle, m_gameLoop.m_planet.m_planetCamera, string.Empty);
		TextMeshS.SetText(m_title, text, false);
		m_title.m_go.transform.localScale = Vector3.one;
		m_title.m_textMesh.characterSize = 1f;
		m_title.m_renderer.material.shader = shader;
		m_title.m_renderer.material.color = DebugDraw.HexToColor(hex);
		m_titleShadow = TextMeshS.AddComponent(m_bannerTC, new Vector3(-0.1f * m_width + m_xOffset + 0.1f, 0.13f * m_height + m_yOffset - 0.1f, -0.1f + m_zOffset), "KGSecondChances_Font", 2f, 2f, m_height * 2f, Align.Left, Align.Middle, m_gameLoop.m_planet.m_planetCamera, string.Empty);
		TextMeshS.SetText(m_titleShadow, text, false);
		m_titleShadow.m_go.transform.localScale = Vector3.one;
		m_titleShadow.m_textMesh.characterSize = 1f;
		m_titleShadow.m_renderer.material.shader = shader;
		m_titleShadow.m_renderer.material.color = DebugDraw.HexToColor("#333333");
		TransformS.SetScale(m_bannerTC, Vector3.zero);
		m_hidden = true;
	}

	public void SetArrowShape(float _value)
	{
		_value = Mathf.Clamp(_value, 0f, 1f);
		SpriteS.SetDimensions(m_arrowSprite, m_arrowWidth * ((_value + 1f) / 2f), m_arrowHeight * (1f / (_value + 1f)) * 0.5f);
		SpriteS.SetOffset(m_arrowSprite, new Vector3(m_xOffset, ((0f - m_height) / 2f + (0f - m_arrowSprite.height) / 2f) * 0.99f + m_yOffset, 0.01f + m_zOffset), 0f);
	}

	public void Hide()
	{
		if (m_tweenC != null)
		{
			TweenS.RemoveComponent(m_tweenC);
			m_tweenC = null;
		}
		m_tweenC = TweenS.AddTransformTween(m_bannerTC, TweenedProperty.Scale, TweenStyle.Linear, Vector3.zero, 0.2f, 0f, false);
		m_hidden = true;
		TweenS.AddTweenEndEventListener(m_tweenC, delegate
		{
			TweenS.RemoveComponent(m_tweenC);
			m_tweenC = null;
		});
	}

	public void Show()
	{
		if (m_tweenC != null)
		{
			TweenS.RemoveComponent(m_tweenC);
			m_tweenC = null;
		}
		m_tweenC = TweenS.AddTransformTween(m_bannerTC, TweenedProperty.Scale, TweenStyle.BackOut, Vector3.one, 0.4f, 0f, false);
		m_hidden = false;
		TweenS.AddTweenEndEventListener(m_tweenC, delegate
		{
			TweenS.RemoveComponent(m_tweenC);
			m_tweenC = null;
		});
	}

	public void IncreaseStar(TimerC _c)
	{
		if (m_starCounter != null)
		{
			m_lastEarnedStars = Mathf.Max(0, m_lastEarnedStars - 1);
			TextMeshS.SetText(m_starCounter, PsMetagameManager.m_playerStats.stars - m_lastEarnedStars + " / " + m_requiredStars, false);
		}
		TimerS.RemoveComponent(_c);
	}

	public void SetLastEarnedStars(int _count)
	{
		m_lastEarnedStars = _count;
		if (m_starCounter != null)
		{
			TextMeshS.SetText(m_starCounter, PsMetagameManager.m_playerStats.stars - m_lastEarnedStars + " / " + m_requiredStars, false);
		}
	}

	public void Destroy()
	{
		if (m_starCounter != null)
		{
			TextMeshS.RemoveComponent(m_starCounter);
			m_starCounter = null;
		}
		if (m_title != null)
		{
			TextMeshS.RemoveComponent(m_title);
			m_title = null;
		}
		if (m_titleShadow != null)
		{
			TextMeshS.RemoveComponent(m_titleShadow);
			m_titleShadow = null;
		}
		if (m_backgroundSprite != null)
		{
			SpriteS.RemoveComponent(m_backgroundSprite);
			m_backgroundSprite = null;
		}
		if (m_arrowSprite != null)
		{
			SpriteS.RemoveComponent(m_arrowSprite);
			m_arrowSprite = null;
		}
		if (m_starSprite != null)
		{
			SpriteS.RemoveComponent(m_starSprite);
			m_starSprite = null;
		}
		if (m_iconSprite != null)
		{
			SpriteS.RemoveComponent(m_iconSprite);
			m_iconSprite = null;
		}
		if (m_itemSprite != null)
		{
			SpriteS.RemoveComponent(m_itemSprite);
			m_itemSprite = null;
		}
		if (m_bannerTC != null)
		{
			TransformS.RemoveComponent(m_bannerTC);
			m_bannerTC = null;
		}
	}

	public static bool operator ==(PsUICheckpointBanner _banner, PsGameLoop _gameLoop)
	{
		return _banner.m_gameLoop == _gameLoop;
	}

	public static bool operator !=(PsUICheckpointBanner _banner, PsGameLoop _gameLoop)
	{
		return _banner.m_gameLoop != _gameLoop;
	}
}
