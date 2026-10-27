using Server;
using UnityEngine;
using System;
using AdMediation;

// Admin / developer panel, opened from the "Admin" button in PsUICenterSettings.
//
// Ported from the dev-build block of the old PsUISettingsMenuContent:
//   - not in admin mode: password field + "Enter Admin Mode"
//   - in admin mode:     dev tools and resets, plus "Disable admin mode"
public class PsUICenterAdmin : PsUIScrollableBase
{
    private class PsUIAdminConfirmation : PsUIHeaderedCanvas
    {
        private PsUIGenericButton m_ok;

        private PsUIGenericButton m_cancel;

        public PsUIAdminConfirmation(UIComponent _parent, string _question)
            : base(_parent, string.Empty, false)
        {
            (GetRoot() as PsUIBasePopup).m_scrollableCanvas.SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
            SetWidth(0.65f, RelativeTo.ScreenWidth);
            SetHeight(0.45f, RelativeTo.ScreenHeight);
            SetVerticalAlign(0.4f);
            SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
            SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
            m_header.Destroy();
            new UITextbox(this, false, string.Empty, _question, "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest, false, Align.Center, Align.Middle);
            UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
            uIHorizontalList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
            uIHorizontalList.RemoveDrawHandler();
            uIHorizontalList.SetVerticalAlign(0f);
            uIHorizontalList.SetMargins(0f, 0f, 0.075f, -0.075f);
            m_ok = new PsUIGenericButton(uIHorizontalList);
            m_ok.SetText("Yes!", 0.04f);
            m_ok.SetHeight(0.075f, RelativeTo.ScreenHeight);
            m_ok.SetGreenColors();
            m_cancel = new PsUIGenericButton(uIHorizontalList);
            m_cancel.SetText("Cancel", 0.04f);
            m_cancel.SetHeight(0.075f, RelativeTo.ScreenHeight);
        }

        public override void Step()
        {
            if (m_ok.m_hit)
            {
                (GetRoot() as PsUIBasePopup).CallAction("Proceed");
                (GetRoot() as PsUIBasePopup).Destroy();
            }
            else if (m_cancel.m_hit)
            {
                (GetRoot() as PsUIBasePopup).Destroy();
            }
            base.Step();
        }
    }

    // PsUIBasePopup creates its content with Activator.CreateInstance(type, parent),
    // so each question needs its own small subclass.
    private class PsUIPopupResetGame : PsUIAdminConfirmation
    {
        public PsUIPopupResetGame(UIComponent _parent)
            : base(_parent, "Are you sure you want to RESET your progression?")
        {
        }
    }

    private class PsUIPopupResetStats : PsUIAdminConfirmation
    {
        public PsUIPopupResetStats(UIComponent _parent)
            : base(_parent, "Are you sure you want to RESET your stats?")
        {
        }
    }

    private class PsUIPopupResetKeychain : PsUIAdminConfirmation
    {
        public PsUIPopupResetKeychain(UIComponent _parent)
            : base(_parent, "Are you sure you want to RESET keychain settings?")
        {
        }
    }

    private class PsUIPopupToBuilder : PsUIAdminConfirmation
    {
        public PsUIPopupToBuilder(UIComponent _parent)
            : base(_parent, "Are you sure you want to switch to builder account?")
        {
        }
    }

    private class PsUIPopupToPlayer : PsUIAdminConfirmation
    {
        public PsUIPopupToPlayer(UIComponent _parent)
            : base(_parent, "Are you sure you want to switch back to player account?")
        {
        }
    }

    // Password for entering admin mode from this panel.
    // Leave empty to let anyone who taps "Enter Admin Mode" in. This is a client-side
    // check only, so it is a convenience gate, not security.
    private static readonly string ADMIN_PASSWORD = string.Empty;

    private const string BUILDER_USER_ID = "5582757b3800005834b7897b";

    private UIModel m_passwordModel = new UIModel(new PasswordModel(string.Empty));

    private UIComponent m_contentParent;

    private UIVerticalList m_list;

    private UITextArea m_adminPass;

    private PsUIGenericButton m_adminPassOK;

    private UIText m_adminPassStatus;

    private PsUIGenericButton m_resetGame;

    private PsUIGenericButton m_testElasticSearch;

    private PsUIGenericButton m_testAds;

    private PsUIGenericButton m_builder;

    private PsUIGenericButton m_graphEditor;

    private PsUIGenericButton m_resetLocalPath;

    private PsUIGenericButton m_resetServerPath;

    private PsUIGenericButton m_resetStats;

    private PsUIGenericButton m_resetKeychain;

    private PsUIGenericButton m_disableAdminMode;

    private PsUIGenericButton m_bteInterval;

    // "Extras" page: debug tools of the game
    private bool m_extrasPage;

    private PsUIGenericButton m_extras;

    private PsUIGenericButton m_extraFramework;

    private PsUIGenericButton m_extraMystery;

    private PsUIGenericButton m_extraFps;

    private PsUIGenericButton m_extraBack;

    private static GameObject s_fpsCounter;

    public PsUICenterAdmin(UIComponent _parent)
        : base(_parent)
    {
        PsUIBasePopup psUIBasePopup = GetRoot() as PsUIBasePopup;
        if (psUIBasePopup != null)
        {
            psUIBasePopup.m_scrollableCanvas.SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
        }
        m_scrollArea.m_maxScrollInertialX = 0f;
        m_scrollArea.m_maxScrollInertialY = 50f / (1024f / (float)Screen.width);
        m_scrollArea.m_TAC.m_letTouchesThrough = true;
    }

    public override void CreateHeaderContent(UIComponent _parent)
    {
        UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
        uIHorizontalList.RemoveDrawHandler();
        uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
        uIHorizontalList.SetMargins(0.025f, 0.025f, 0f, 0f);
        uIHorizontalList.SetHorizontalAlign(0.5f);
        new UIText(uIHorizontalList, false, string.Empty, "Admin", "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
    }

    public override void CreateContent(UIComponent _parent)
    {
        m_contentParent = _parent;
        BuildList();
    }

    private void BuildList()
    {
        if (m_list != null)
        {
            m_list.Destroy();
        }
        ClearButtons();
        m_list = new UIVerticalList(m_contentParent, string.Empty);
        m_list.RemoveDrawHandler();
        m_list.SetVerticalAlign(1f);
        m_list.SetHorizontalAlign(0.5f);
        m_list.SetMargins(0.025f, 0.025f, 0.05f, 0.05f, RelativeTo.ScreenShortest);
        m_list.SetSpacing(0.03f, RelativeTo.ScreenShortest);
        if (PsState.m_adminMode && m_extrasPage)
        {
            BuildExtraButtons();
        }
        else if (PsState.m_adminMode)
        {
            BuildAdminButtons();
        }
        else
        {
            BuildPasswordForm();
        }
    }

    private void RebuildList()
    {
        BuildList();
        m_scrollArea.Update();
    }

    private void ClearButtons()
    {
        m_adminPass = null;
        m_adminPassOK = null;
        m_adminPassStatus = null;
        m_resetGame = null;
        m_testElasticSearch = null;
        m_testAds = null;
        m_builder = null;
        m_graphEditor = null;
        m_resetLocalPath = null;
        m_resetServerPath = null;
        m_resetStats = null;
        m_resetKeychain = null;
        m_disableAdminMode = null;
        m_bteInterval = null;
        m_extras = null;
        m_extraFramework = null;
        m_extraMystery = null;
        m_extraFps = null;
        m_extraBack = null;
    }

    private PsUIGenericButton AddButton(string _text)
    {
        PsUIGenericButton psUIGenericButton = new PsUIGenericButton(m_list);
        psUIGenericButton.SetText(_text);
        psUIGenericButton.SetHeight(0.05f, RelativeTo.ScreenHeight);
        return psUIGenericButton;
    }

    private void BuildPasswordForm()
    {
        m_adminPass = new UITextArea(m_list, string.Empty, PsStrings.Get(StringID.BUTTON_SETTINGS_ADMIN), string.Empty, null, m_passwordModel, "m_password");
        m_adminPassOK = new PsUIGenericButton(m_list);
        m_adminPassOK.SetText("Enter Admin Mode", 0.04f);
        m_adminPassOK.SetGreenColors();
        m_adminPassOK.SetHeight(0.06f, RelativeTo.ScreenHeight);
        m_adminPassStatus = new UIText(m_list, false, string.Empty, "Enter password to access admin mode", "HurmeSemiBold_Font", 0.025f, RelativeTo.ScreenShortest);
    }

    private void BuildAdminButtons()
    {
        m_adminPassStatus = new UIText(m_list, false, string.Empty, "Admin mode is active.", "HurmeSemiBold_Font", 0.025f, RelativeTo.ScreenShortest);
        m_resetGame = AddButton("RESET GAME");
        m_testElasticSearch = AddButton("Test Elastic Search");
        m_testAds = AddButton("Test Ads");
        m_builder = AddButton((KeyChain.GetUserId() == BUILDER_USER_ID) ? "Switch to player account" : "Switch to builder account");
        m_graphEditor = AddButton("Progression Editor");
        m_resetLocalPath = AddButton("Reset Local Path");
        m_resetServerPath = AddButton("Reset Server Path");
        m_resetStats = AddButton("Reset stats");
        m_resetKeychain = AddButton("Reset keychain");
        m_bteInterval = AddButton(BteIntervalText());
        m_extras = AddButton("Extras");
        m_disableAdminMode = AddButton("Disable admin mode");
    }

    // second page of the admin popup: debug tools that the game still contains
    private void BuildExtraButtons()
    {
        m_adminPassStatus = new UIText(m_list, false, string.Empty, "Extras: debug tools. In the framework test, the Next test button (or Space) changes the test, the Exit button (or Escape) leaves it.", "HurmeSemiBold_Font", 0.025f, RelativeTo.ScreenShortest);
        m_extraFramework = AddButton("Framework Test");
        m_extraMystery = AddButton("Mystery box animation");
        m_extraFps = AddButton((s_fpsCounter != null) ? "FPS counter: ON" : "FPS counter: OFF");
        m_extraBack = AddButton("Back");
    }

    private void StartFrameworkTest(int _state)
    {
        FrameworkTestScene.m_startState = _state;
        (GetRoot() as PsUIBasePopup).Destroy();
        Main.m_currentGame.m_sceneManager.ChangeScene(new FrameworkTestScene("FrameworkTestScene"), new FadeLoadingScene(Color.black));
    }

    private void ToggleFpsCounter()
    {
        if (s_fpsCounter != null)
        {
            UnityEngine.Object.Destroy(s_fpsCounter);
            s_fpsCounter = null;
        }
        else
        {
            s_fpsCounter = new GameObject("FPS counter");
            UnityEngine.Object.DontDestroyOnLoad(s_fpsCounter);
            s_fpsCounter.AddComponent<HUDFps>();
        }
        RebuildList();
    }

    public override void Step()
    {
        if (m_extras != null && m_extras.m_hit)
        {
            m_extrasPage = true;
            RebuildList();
        }
        else if (m_extraBack != null && m_extraBack.m_hit)
        {
            m_extrasPage = false;
            RebuildList();
        }
        else if (m_extraFramework != null && m_extraFramework.m_hit)
        {
            StartFrameworkTest(0);
        }
        else if (m_extraMystery != null && m_extraMystery.m_hit)
        {
            new PsUIMysteryBox();
        }
        else if (m_extraFps != null && m_extraFps.m_hit)
        {
            ToggleFpsCounter();
        }
        else if (m_adminPass != null && m_adminPass.m_hit)
        {
            UITextInput uITextInput = new UITextInput("Admin password", PasswordDoneCallback, PasswordCancelCallback);
            uITextInput.SetText((string)m_adminPass.GetValue());
            uITextInput.Update();
        }
        else if (m_adminPassOK != null && m_adminPassOK.m_hit)
        {
            TryEnterAdminMode();
        }
        else if (m_bteInterval != null && m_bteInterval.m_hit)
        {
            UITextInput bteInput = new UITextInput("Hours between BTE challenges (" + BBR249Server.Api.BotPlayer.MinHoursBetweenChallenges + "-" + BBR249Server.Api.BotPlayer.MaxHoursBetweenChallenges + ")", BteIntervalDoneCallback, BteIntervalCancelCallback);
            bteInput.SetText(BteSettings.Hours.ToString());
            bteInput.Update();
        }
        else if (m_disableAdminMode != null && m_disableAdminMode.m_hit)
        {
            PsState.m_adminMode = false;
            RebuildList();
        }
        else if (m_resetGame != null && m_resetGame.m_hit)
        {
            Confirm(typeof(PsUIPopupResetGame), ResetGame);
        }
        else if (m_resetStats != null && m_resetStats.m_hit)
        {
            Confirm(typeof(PsUIPopupResetStats), ResetStats);
        }
        else if (m_resetKeychain != null && m_resetKeychain.m_hit)
        {
            Confirm(typeof(PsUIPopupResetKeychain), ResetKeychain);
        }
        else if (m_builder != null && m_builder.m_hit)
        {
            if (KeyChain.GetUserId() == BUILDER_USER_ID)
            {
                Confirm(typeof(PsUIPopupToPlayer), ChangeToOriginal);
            }
            else
            {
                Confirm(typeof(PsUIPopupToBuilder), ChangeToBuilder);
            }
        }
        else if (m_testElasticSearch != null && m_testElasticSearch.m_hit)
        {
            MinigameSearchParametres minigameSearchParametres = new MinigameSearchParametres();
            HttpC httpC = MiniGame.SearchMinigames(minigameSearchParametres, SearchOnlyMetaDataSUCCESS, SearchOnlyMetaDataFAILED, null, 300);
            httpC.objectData = minigameSearchParametres;
            Debug.LogWarning("SEARCH METADATA");
        }
        else if (m_testAds != null && m_testAds.m_hit)
        {
            if (PsAdMediation.adsAvailable())
            {
                TouchAreaS.Disable();
                PsAdMediation.ShowAd(AdCallback);
            }
        }
        else if (m_graphEditor != null && m_graphEditor.m_hit)
        {
            (GetRoot() as PsUIBasePopup).Destroy();
            Main.m_currentGame.m_sceneManager.ChangeScene(new GeScene("NodeEditorScene"), new FadeLoadingScene(Color.black));
        }
        else if (m_resetLocalPath != null && m_resetLocalPath.m_hit)
        {
            PsPlanetManager.GetCurrentPlanet().ResetLocalProgression();
            Main.m_currentGame.m_sceneManager.ChangeScene(new StartupScene("StartupScene"));
        }
        else if (m_resetServerPath != null && m_resetServerPath.m_hit)
        {
            PsPlanetManager.GetCurrentPlanet().ResetServerProgression();
        }
        base.Step();
    }

    private void Confirm(Type _popupType, Action _onProceed)
    {
        PsUIBasePopup psUIBasePopup = new PsUIBasePopup(_popupType, null, null, null, false);
        psUIBasePopup.SetAction("Proceed", _onProceed);
    }

    private void TryEnterAdminMode()
    {
        string text = (string)m_adminPass.GetValue();
        if (string.IsNullOrEmpty(ADMIN_PASSWORD) || text == ADMIN_PASSWORD)
        {
            PsState.m_adminMode = true;
            RebuildList();
        }
        else
        {
            m_adminPassStatus.SetText("Wrong password");
            m_adminPassStatus.Update();
        }
    }

    private void PasswordDoneCallback(string _value)
    {
        m_adminPass.SetValue(_value);
    }

    private void PasswordCancelCallback()
    {
    }

    private string BteIntervalText()
    {
        return "BTE challenges you every " + BteSettings.Hours + " h";
    }

    // the value is kept between 1 and 48 hours
    private void BteIntervalDoneCallback(string _value)
    {
        int hours;
        if (int.TryParse((_value ?? string.Empty).Trim(), out hours))
        {
            BteSettings.Hours = hours;
        }
        // the list is built again so that the button shows the new value (changing the text of a button that is already drawn leaves it empty)
        RebuildList();
    }

    private void BteIntervalCancelCallback()
    {
    }

    private void ResetGame()
    {
        ResetFlow.Start();
    }

    private void ResetKeychain()
    {
        TouchAreaS.Disable();
        KeyChain.SetUserId(string.Empty);
        KeyChain.SetGameCenterId(string.Empty);
        TouchAreaS.Enable();
    }

    private void ResetStats()
    {
        PsMetagamePlayerData.m_playerData.Clear();
        PsMetagameManager.m_playerStats.upgrades.Clear();
        Player.ResetPlayer(ResetSUCCEED, ResetFAILED);
        m_resetStats.DisableTouchAreas();
        m_resetStats.SetText("Resetting");
        m_resetStats.Update();
    }

    private void ResetSUCCEED(HttpC _c)
    {
        PsMetagameManager.HideResources();
        Debug.Log("RESET SUCCEED - or not");
        Application.Quit();
        Main.m_currentGame.m_sceneManager.ChangeScene(new StartupScene("StartupScene"));
    }

    private void ResetFAILED(HttpC _c)
    {
    }

    private void ChangeToBuilder()
    {
        PlayerPrefs.SetString("ORIG_USER", KeyChain.GetUserId());
        PlayerPrefs.SetString("ORIG_NAME", KeyChain.GetUserName());
        KeyChain.SetUserId(BUILDER_USER_ID);
        KeyChain.SetUserName("The Builder");
        Debug.Log("KCID: " + KeyChain.GetUserId());
        Main.m_currentGame.m_sceneManager.ChangeScene(new StartupScene("StartupScene"));
    }

    private void ChangeToOriginal()
    {
        KeyChain.SetUserId(PlayerPrefs.GetString("ORIG_USER"));
        KeyChain.SetUserName(PlayerPrefs.GetString("ORIG_NAME"));
        Main.m_currentGame.m_sceneManager.ChangeScene(new StartupScene("StartupScene"));
    }

    private void AdCallback(AdResult _result)
    {
        TouchAreaS.Enable();
        Debug.Log("Ad display result: " + _result);
    }

    private void SearchOnlyMetaDataFAILED(HttpC _c)
    {
        Debug.LogError("ELASTIC SEARCH FAILED " + _c.www);
    }

    private void SearchOnlyMetaDataSUCCESS(PsMinigameMetaData[] _minigameMetaData)
    {
        if (_minigameMetaData.Length > 0)
        {
            for (int i = 0; i < _minigameMetaData.Length; i++)
            {
                Debug.Log(string.Concat((!_minigameMetaData[i].played) ? string.Empty : "PLAYED - ", _minigameMetaData[i].name, "/", _minigameMetaData[i].difficulty, "/", _minigameMetaData[i].gameMode, "/", _minigameMetaData[i].playerUnit, "/", _minigameMetaData[i].quality, "/ liked+rated: ", _minigameMetaData[i].timesLiked, "+", _minigameMetaData[i].timesRated, "(", (float)_minigameMetaData[i].timesLiked / (float)_minigameMetaData[i].timesRated, ")"));
            }
        }
        else
        {
            Debug.LogError("SERVER RETURNED EMPTY LEVEL LIST");
        }
    }
}
public class PsUICenterSettings : PsUIHeaderedCanvas
{
    private class PsUIPopupConfirmation : PsUIHeaderedCanvas
    {
        private PsUIGenericButton m_ok;

        private PsUIGenericButton m_cancel;

        public PsUIPopupConfirmation(UIComponent _parent)
            : base(_parent, string.Empty, false)
        {
            (GetRoot() as PsUIBasePopup).m_scrollableCanvas.SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
            SetWidth(0.65f, RelativeTo.ScreenWidth);
            SetHeight(0.45f, RelativeTo.ScreenHeight);
            SetVerticalAlign(0.4f);
            SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
            SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
            m_header.Destroy();
            new UITextbox(this, false, string.Empty, "Are you sure you want to RESET your game progression?", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest, false, Align.Center, Align.Middle);
            UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
            uIHorizontalList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
            uIHorizontalList.RemoveDrawHandler();
            uIHorizontalList.SetVerticalAlign(0f);
            uIHorizontalList.SetMargins(0f, 0f, 0.075f, -0.075f);
            m_ok = new PsUIGenericButton(uIHorizontalList);
            m_ok.SetText("Yes!", 0.04f);
            m_ok.SetHeight(0.075f, RelativeTo.ScreenHeight);
            m_ok.SetGreenColors();
            m_cancel = new PsUIGenericButton(uIHorizontalList);
            m_cancel.SetText("Cancel", 0.04f);
            m_cancel.SetHeight(0.075f, RelativeTo.ScreenHeight);
        }

        public override void Step()
        {
            if (m_ok.m_hit)
            {
                (GetRoot() as PsUIBasePopup).CallAction("Reset");
                (GetRoot() as PsUIBasePopup).Destroy();
            }
            else if (m_cancel.m_hit)
            {
                (GetRoot() as PsUIBasePopup).Destroy();
            }
            base.Step();
        }
    }

    private class PsUIPopupNameWaiting : PsUIHeaderedCanvas
    {
        public PsUIPopupNameWaiting(UIComponent _parent)
            : base(_parent, string.Empty, false)
        {
            (GetRoot() as PsUIBasePopup).m_scrollableCanvas.SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
            SetWidth(0.65f, RelativeTo.ScreenWidth);
            SetHeight(0.45f, RelativeTo.ScreenHeight);
            SetVerticalAlign(0.4f);
            SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
            SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
            m_header.Destroy();
            new UITextbox(this, false, string.Empty, "Changing your name...", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest, false, Align.Center, Align.Middle);
        }
    }

    private const string m_font = "KGSecondChances_Font";

    private const float m_smallTextSize = 0.03f;

    private const string m_textColor = "#a8e2ff";

    // Set to false to hide the Admin button (the admin panel lives in PsUICenterAdmin).
    private const bool m_showAdminButton = true;

    private PsUIBasePopup m_waitingPopup;

    private PsUIGenericButton m_fbButton;

    private PsUIGenericButton m_forumButton;

    private PsUISwitch m_musicSwitch;

    private PsUISwitch m_soundsSwitch;

    private PsUISwitch m_handedSwitch;

    private PsUISwitch m_everyplaySwitch;

    private PsUIPlayerNameField m_palyerNameField;

    private PsUIGenericButton m_resetButton;

    private PsUIGenericButton m_adminButton;

    public PsUICenterSettings(UIComponent _parent)
        : base(_parent, string.Empty)
    {
        SetWidth(0.8f, RelativeTo.ScreenWidth);
        SetHeight(0.75f, RelativeTo.ScreenHeight);
        SetVerticalAlign(0.4f);
        SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
        SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
        m_header.SetDrawHandler(PsUIDrawHandlers.ScrollingUIHeader);
        m_header.SetMargins(0.0125f, 0.0125f, 0.0125f, 0f);
        CreateContent(this);
        CreateHeaderContent(m_header);
    }

    public void CreateHeaderContent(UIComponent _parent)
    {
        UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
        uIHorizontalList.RemoveDrawHandler();
        uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
        uIHorizontalList.SetMargins(0.025f, 0.025f, 0f, 0f);
        uIHorizontalList.SetHorizontalAlign(0.5f);
        UIText parent = new UIText(uIHorizontalList, false, string.Empty, "Settings", "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
        UICanvas uICanvas = new UICanvas(parent, false, string.Empty, null, string.Empty);
        uICanvas.SetHorizontalAlign(0f);
        uICanvas.SetMargins(-1.5f, 0f, -0.125f, -0.125f, RelativeTo.ParentHeight);
        uICanvas.RemoveDrawHandler();
        Frame frame = PsState.m_uiSheet.m_atlas.GetFrame("menu_icon_settings");
        UISprite uISprite = new UISprite(uICanvas, false, string.Empty, PsState.m_uiSheet, frame);
        uISprite.SetSize(1f, 1f * (frame.height / frame.width), RelativeTo.ParentHeight);
        uISprite.SetHorizontalAlign(0f);
        uISprite.SetColor(DebugDraw.HexToColor("#95e5ff") * Color.gray);
    }

    public void CreateContent(UIComponent _parent)
    {
        UIVerticalList uIVerticalList = new UIVerticalList(_parent, string.Empty);
        uIVerticalList.SetWidth(0.85f, RelativeTo.ParentWidth);
        uIVerticalList.RemoveDrawHandler();
        uIVerticalList.SetSpacing(0.035f, RelativeTo.ScreenHeight);
        UIHorizontalList uIHorizontalList = new UIHorizontalList(uIVerticalList, string.Empty);
        uIHorizontalList.RemoveDrawHandler();
        uIHorizontalList.SetSpacing(0.08f, RelativeTo.ScreenWidth);
        UIVerticalList uIVerticalList2 = new UIVerticalList(uIHorizontalList, string.Empty);
        uIVerticalList2.SetSpacing(0.01f, RelativeTo.ScreenHeight);
        uIVerticalList2.RemoveDrawHandler();
        UIHorizontalList uIHorizontalList2 = new UIHorizontalList(uIVerticalList2, string.Empty);
        uIHorizontalList2.RemoveDrawHandler();
        uIHorizontalList2.SetSpacing(0.01f, RelativeTo.ScreenHeight);
        UISprite uISprite = new UISprite(uIHorizontalList2, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_icon_music"));
        uISprite.SetSize(0.03f, 0.03f, RelativeTo.ScreenHeight);
        uISprite.SetColor(DebugDraw.HexToColor("#a8e2ff") * Color.gray);
        UIText uIText = new UIText(uIHorizontalList2, false, string.Empty, "Music", "KGSecondChances_Font", 0.03f, RelativeTo.ScreenHeight);
        uIText.SetColor("#a8e2ff");
        m_musicSwitch = new PsUISwitch(uIVerticalList2, !PsState.m_muteMusic);
        UIVerticalList uIVerticalList3 = new UIVerticalList(uIHorizontalList, string.Empty);
        uIVerticalList3.SetSpacing(0.01f, RelativeTo.ScreenHeight);
        uIVerticalList3.RemoveDrawHandler();
        UIHorizontalList uIHorizontalList3 = new UIHorizontalList(uIVerticalList3, string.Empty);
        uIHorizontalList3.RemoveDrawHandler();
        uIHorizontalList3.SetSpacing(0.01f, RelativeTo.ScreenHeight);
        UISprite uISprite2 = new UISprite(uIHorizontalList3, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_icon_sounds"));
        uISprite2.SetColor(DebugDraw.HexToColor("#a8e2ff") * Color.gray);
        uISprite2.SetSize(0.03f, 0.03f, RelativeTo.ScreenHeight);
        UIText uIText2 = new UIText(uIHorizontalList3, false, string.Empty, "Sounds", "KGSecondChances_Font", 0.03f, RelativeTo.ScreenHeight);
        uIText2.SetColor("#a8e2ff");
        m_soundsSwitch = new PsUISwitch(uIVerticalList3, !PsState.m_muteSoundFX);
        UIVerticalList uIVerticalList4 = new UIVerticalList(uIHorizontalList, string.Empty);
        uIVerticalList4.SetSpacing(0.01f, RelativeTo.ScreenHeight);
        uIVerticalList4.RemoveDrawHandler();
        UIText uIText3 = new UIText(uIVerticalList4, false, string.Empty, "Left-Handed", "KGSecondChances_Font", 0.03f, RelativeTo.ScreenHeight);
        uIText3.SetColor("#a8e2ff");
        m_handedSwitch = new PsUISwitch(uIVerticalList4, PsState.m_editorIsLefty);
        UIHorizontalList uIHorizontalList4 = new UIHorizontalList(uIVerticalList, string.Empty);
        uIHorizontalList4.RemoveDrawHandler();
        uIHorizontalList4.SetSpacing(0.08f, RelativeTo.ScreenWidth);
        if (EveryplayManager.IsSupported())
        {
            UIVerticalList uIVerticalList5 = new UIVerticalList(uIHorizontalList4, string.Empty);
            uIVerticalList5.SetSpacing(0.01f, RelativeTo.ScreenHeight);
            uIVerticalList5.RemoveDrawHandler();
            UIHorizontalList uIHorizontalList5 = new UIHorizontalList(uIVerticalList5, string.Empty);
            uIHorizontalList5.RemoveDrawHandler();
            uIHorizontalList5.SetSpacing(0.01f, RelativeTo.ScreenHeight);
            UISprite uISprite3 = new UISprite(uIHorizontalList5, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_icon_everyplay"));
            uISprite3.SetSize(0.03f, 0.03f, RelativeTo.ScreenHeight);
            uISprite3.SetColor(DebugDraw.HexToColor("#a8e2ff") * Color.gray);
            UIText uIText4 = new UIText(uIHorizontalList5, false, string.Empty, "Everyplay", "KGSecondChances_Font", 0.03f, RelativeTo.ScreenHeight);
            uIText4.SetColor("#a8e2ff");
            m_everyplaySwitch = new PsUISwitch(uIVerticalList5, PsState.m_everyplayEnabled, 0.03f, "Enabled", "Disabled", 0.12f);
        }
        UIVerticalList uIVerticalList6 = new UIVerticalList(uIHorizontalList4, string.Empty);
        uIVerticalList6.RemoveDrawHandler();
        m_forumButton = new PsUIGenericButton(uIVerticalList6);
        m_forumButton.SetIcon("hud_icon_publish", 0.044999998f);
        m_forumButton.SetText("Forum");
        m_palyerNameField = new PsUIPlayerNameField(uIVerticalList);
        m_palyerNameField.SetText(PlayerPrefsX.GetUserName());
        m_palyerNameField.SetCallbacks(ChangeName);
        m_fbButton = new PsUIGenericButton(uIVerticalList);
        m_fbButton.SetIcon("menu_icon_facebook", 0.044999998f);
        if (string.IsNullOrEmpty(PlayerPrefsX.GetFacebookId()))
        {
            m_fbButton.SetText("Connect to Facebook");
        }
        else
        {
            m_fbButton.SetText("Disconnect from Facebook");
        }
        UIText uIText5 = new UIText(this, false, string.Empty, "Client version " + TrackedBundleVersion.Current.version, "KGSecondChances_Font", 0.03f, RelativeTo.ParentHeight);
        uIText5.SetMargins(0f, 0.05f, 0f, 0f, RelativeTo.ParentHeight);
        uIText5.SetAlign(1f, 0f);
        if (m_showAdminButton)
        {
            m_adminButton = new PsUIGenericButton(this);
            m_adminButton.SetText("Admin", 0.035f);
            m_adminButton.SetOrangeColors();
            m_adminButton.SetHeight(0.075f, RelativeTo.ScreenHeight);
            m_adminButton.SetAlign(0.015f, 0.05f);
            m_adminButton.SetMargins(0.05f, 0.05f, 0.05f, 0.05f, RelativeTo.ScreenHeight);
        }
    }

    private void OpenAdminPanel()
    {
        PsUIBasePopup adminPopup = new PsUIBasePopup(typeof(PsUICenterAdmin), null, null, null, false);
        adminPopup.SetAction("Exit", delegate
        {
            adminPopup.Destroy();
        });
    }

    private void ChangeName(string _name)
    {
        if (!_name.Equals(PlayerPrefsX.GetUserName()))
        {
            PlayerPrefsX.SetUserName(_name);
            KeyChain.SetUserName(_name);
            m_waitingPopup = new PsUIBasePopup(typeof(PsUIPopupNameWaiting), null, null, null, false);
            new PsServerQueueFlow(null, delegate
            {
                PsServerRequest.ServerChangeName(NameChangeSUCCEED, NameChangeFAILED);
            }, null);
        }
    }

    private void NameChangeSUCCEED(HttpC _c)
    {
        Debug.Log("NAME CHANGE SUCCEED");
        PsAchievementManager.Complete("uniqueSnowflake");
        m_waitingPopup.Destroy();
        m_waitingPopup = null;
    }

    private void NameChangeFAILED(HttpC _c)
    {
        Debug.Log("NAME CHANGE FAILED");
        string networkError = ServerErrors.GetNetworkError(_c.www.error);
        ServerManager.ThrowServerErrorException("Connection error", networkError, () => PsServerRequest.ServerChangeName(NameChangeSUCCEED, NameChangeFAILED));
    }

    public override void Step()
    {
        if (m_musicSwitch.m_hit)
        {
            PsState.m_muteMusic = !m_musicSwitch.m_enabled;
            PlayerPrefsX.SetMuteMusic(PsState.m_muteMusic);
            (Main.m_currentGame.m_currentScene as PsMenuScene).UpdateMusic();
        }
        else if (m_soundsSwitch.m_hit)
        {
            PsState.m_muteSoundFX = !m_soundsSwitch.m_enabled;
            PlayerPrefsX.SetMuteSoundFX(PsState.m_muteSoundFX);
            SoundS.m_canPlaySounds = !PsState.m_muteSoundFX;
        }
        else if (m_handedSwitch.m_hit)
        {
            PsState.m_editorIsLefty = m_handedSwitch.m_enabled;
            PlayerPrefsX.SetLefty(PsState.m_editorIsLefty);
        }
        else if (m_everyplaySwitch != null && m_everyplaySwitch.m_hit)
        {
            PsState.m_everyplayEnabled = m_everyplaySwitch.m_enabled;
            PlayerPrefsX.SetEveryplayEnabled(PsState.m_everyplayEnabled);
            EveryplayManager.SetEnabled(PsState.m_everyplayEnabled);
        }
        else if (m_fbButton.m_hit)
        {
            m_waitingPopup = new PsUIBasePopup(typeof(PsUIPopupFacebookWaiting), null, null, null, false);
            if (string.IsNullOrEmpty(PlayerPrefsX.GetFacebookId()))
            {
                FacebookManager.Login(FacebookLoginDone);
            }
            else
            {
                FacebookManager.Logout(FacebookLogoutDone, true);
            }
        }
        else if (m_forumButton.m_hit)
        {
            Application.OpenURL("http://forum.whatonearthgame.com/");
        }
        else if (m_adminButton != null && m_adminButton.m_hit)
        {
            OpenAdminPanel();
        }
        base.Step();
    }

    private void Reset()
    {
        ResetFlow.Start();
    }

    private void FacebookLoginDone()
    {
        Debug.Log("Facebook login done");
        if (string.IsNullOrEmpty(PlayerPrefsX.GetFacebookId()))
        {
            m_fbButton.SetText("Connect to Facebook");
        }
        else
        {
            m_fbButton.SetText("Disconnect from Facebook");
        }
        m_palyerNameField.UpdateProfileImage();
        m_fbButton.m_parent.Update();
        if (m_waitingPopup != null)
        {
            m_waitingPopup.Destroy();
            m_waitingPopup = null;
        }
    }

    private void FacebookLogoutDone()
    {
        Debug.Log("Facebook logout done");
        if (string.IsNullOrEmpty(PlayerPrefsX.GetFacebookId()))
        {
            m_fbButton.SetText("Connect to Facebook");
        }
        else
        {
            m_fbButton.SetText("Disconnect from Facebook");
        }
        m_palyerNameField.UpdateProfileImage();
        m_fbButton.m_parent.Update();
        if (m_waitingPopup != null)
        {
            m_waitingPopup.Destroy();
            m_waitingPopup = null;
        }
    }
}