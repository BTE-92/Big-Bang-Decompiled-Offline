using System.Collections.Generic;
using UnityEngine;

public static class PsState
{
	public const int PATH_MAJOR_VERSION = 2;

	public const bool DEV_BUILD = false;

	public const bool LOG_ALL = false;

	public const string SERVER_BASE = "http://192.168.0.24";//"https://woeprod.traplightgames.com";

	public const string BUILD_SERVER = "";

	public const string STORE_ID = "com.traplight.bigbangracing";

	public const string OmniataApiKey = "a132fbb1";

	public const bool PRODUCTION_BUILD = true;

	public const string FlurryIOSApiKey = "WQWM93PDWY44322VD3K8";

	public const string FlurryAndroidApiKey = "2XG43WSQHR5C77VCXMJQ";

	public const string UNITY_ADS_GAMEID = "96752";

	public const string AdjustToken = "m4ck5yjbw7pc";

	public const bool LOCAL_INITIAL_DATA = false;

	public const string GTAG_AUTODESTROY = "GTAG_AUTODESTROY";

	public const string GTAG_INGAME_COIN = "GTAG_COIN";

	public const string GTAG_INGAME_DEBRIS = "GTAG_DEBRIS";

	public const string GTAG_UNIT = "GTAG_UNIT";

	public const int SCREENSHOT_SIZE = 512;

	public const float ZBUFFER_FIGHT_BIAS = 0.5f;

	public const int MAX_TRACK_NAME_CHARACTERS = 32;

	public const int UI_SPRITE_REFERENCE_SCALE = 1536;

	public const float EDITOR_MAX_ZOOM_LEVEL = 1500f;

	public const float EDITOR_MIN_ZOOM_LEVEL = 200f;

	public const uint CP_LAYER_DEBRIS = 1u;

	public const uint CP_LAYER_GROUND = 17u;

	public const uint CP_LAYER_STATIC = 257u;

	public const uint CP_LAYER_ALL_BUT_NO_STATICS = 1118208u;

	public const uint CP_LAYER_DEFAULT = 17895696u;

	public const uint CP_LAYER_TRIGGER = 16777216u;

	public const uint CP_LAYER_AUTODECOS = 268435456u;

	public static PerformanceClass m_performanceClass = ToolBox.GetDevicePerformanceClass();

	public static bool m_muteSoundFX = PlayerPrefsX.GetMuteSoundFX();

	public static bool m_muteMusic = PlayerPrefsX.GetMuteMusic();

	public static string m_sessionId = string.Empty;

	public static bool m_adminMode = true;//false;

	public static bool m_adminModeSkipping = false;

	public static bool m_enableAutodecos = false;

	public static bool m_adWatched = false;

	public static bool m_inIapFlow = false;

	public static bool m_inLoginFlow = false;

	public static bool m_inStartupSequence = true;

	public static bool m_restarting = false;

	public static int m_versusChallengeTryAmount = 10;

	public static int m_versusTokenMaxCount = 10;

	public static int m_versusRankCap = 25;

	public static string m_fixedFreshMinigameId = "567011c8340000ce0286aca1";

	public static float m_rentPriceMultiplier = 0.5f;

	public static int m_minimumRentPrice = 5;

	public static string[] m_newEditorItems;

	public static int m_newEditorItemCount = 0;

	public static int m_coinValueBoosterSecs = 180;

	public static int[] COIN_VALUES = new int[5] { 1, 2, 5, 10, 50 };

	public static SpriteSheet m_uiSheet;

	public static Vector2[] m_domeSizes = new Vector2[3]
	{
		new Vector2(3f, 1f),
		new Vector2(6f, 2f),
		new Vector2(9f, 2f)
	};

	public static Vector2 m_editorAreaSize = m_domeSizes[2];

	public static bool m_editorIsLefty = PlayerPrefsX.GetLefty();

	public static bool m_everyplayEnabled = PlayerPrefsX.GetEveryplayEnabled();

	public static float m_drawButtonWindowPosition;

	public static float m_drawMenuAlign;

	public static float m_objectMenuButtonAlign;

	public static CameraTargetC m_editorCamTarget;

	public static Vector3 m_editorCameraPos;

	public static float m_editorCameraZoom;

	public static EditorTool m_currentTool;

	public static string m_editorLastSelectedItemCategory;

	public static string m_freeWizardLastSelectedItemCategory;

	public static bool m_cameraManTakeShot;

	public static string m_wizardPlayer;

	public static string m_wizardGameMode;

	public static int m_wizardDomeIndex;

	public static int m_currentBrush = 0;

	public static int m_drawLayer;

	public static int m_drawMenuTargetPage;

	public static bool m_addDown;

	public static bool m_subDown;

	public static bool m_specialDown;

	public static PsGameLoop m_activeGameLoop;

	public static Minigame m_activeMinigame;

	public static bool m_physicsPaused;

	public static List<UIComponent> m_openPopups = new List<UIComponent>();

	public static List<PsDialogueFlow> m_dialogues = new List<PsDialogueFlow>();

	public static bool m_UIComponentsHidden = false;

	public static PsUIBasePopup m_openSocialPopup = null;

	public static int m_debugCoinsCollected;

	public static int m_debugCoinsValue;

	public static float m_debugCoinsTime;

	public static bool m_canAutoPause
	{
		get
		{
			return m_activeMinigame != null && m_activeMinigame.m_gameStarted && Main.m_currentGame.m_currentScene is GameScene;
		}
	}
}
