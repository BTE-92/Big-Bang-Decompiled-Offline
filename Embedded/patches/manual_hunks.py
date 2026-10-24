import io, sys

D = sys.argv[1]


def edit(name, pairs):
    p = D + '/' + name
    s = io.open(p, 'r', encoding='utf-8-sig', newline='').read()
    raw = open(p, 'rb').read()
    bom = raw.startswith(b'\xef\xbb\xbf')
    for a, b in pairs:
        if a not in s:
            print('MISSING in', name, ':', a[:70].replace('\n', ' '))
            continue
        s = s.replace(a, b, 1)
    io.open(p, 'w', encoding='utf-8-sig' if bom else 'utf-8', newline='').write(s)
    print('patched', name)


# 1) PsIngameMenu.OpenController must not crash when there is no active minigame
edit('PsIngameMenu.cs', [(
    """		if (m_controller != null)
		{
			CloseController();
		}
		if (_overlayCamera)
		{
			m_overlayCamera = CameraS.AddCamera("ControllerCamera", true);
		}
		if (PsState.m_activeMinigame.m_playerControllerType != null)
		{
			object[] args = new object[1] { m_overlayCamera };
			m_controller = Activator.CreateInstance(PsState.m_activeMinigame.m_playerControllerType, args) as Controller;
			m_controller.Open();
		}
""",
    """		if (PsState.m_activeMinigame == null)
		{
			return;
		}
		if (m_controller != null)
		{
			CloseController();
		}
		if (_overlayCamera)
		{
			m_overlayCamera = CameraS.AddCamera("ControllerCamera", true);
		}
		if (PsState.m_activeMinigame.m_playerControllerType != null)
		{
			object[] args = new object[1] { m_overlayCamera };
			m_controller = Activator.CreateInstance(PsState.m_activeMinigame.m_playerControllerType, args) as Controller;
			m_controller.Open();
		}
""")])

# 2) main menu: profile shield on the left of the settings button, no "?" button, cheat buttons always available
edit('PsMainMenuState.cs', [
    ("""		uIHorizontalList.SetHorizontalAlign(0.25f);
		uIHorizontalList.SetSpacing(0.01f, RelativeTo.ScreenHeight);
		uIHorizontalList.RemoveDrawHandler();
		m_settingsButton = new PsUIGenericButton(uIHorizontalList);
		m_settingsButton.SetIcon("menu_icon_settings_small", 0.052f);
		m_settingsButton.SetTealColors();
		m_settingsButton.SetMargins(0.011f);
		m_settingsButton.SetAlign(0.1f, 0.05f);
		m_helpButton = new PsUIGenericButton(uIHorizontalList);
		m_helpButton.SetText("?", 0.055f, 0.07f);
		m_helpButton.SetTealColors();
		m_helpButton.SetMargins(0.001f);
		m_helpButton.SetAlign(0.1f, 0.05f);
		m_versusButton = new PsUIProfileShield(m_settingsList, true, false);
		m_versusButton.SetDepthOffset(10f);
		m_versusButton.SetSize(0.285f, 0.285f, RelativeTo.ScreenHeight);
""",
     """		uIHorizontalList.SetHorizontalAlign(0f);
		uIHorizontalList.SetSpacing(0.01f, RelativeTo.ScreenHeight);
		uIHorizontalList.RemoveDrawHandler();
		m_versusButton = new PsUIProfileShield(uIHorizontalList, true, false);
		m_versusButton.SetDepthOffset(10f);
		m_versusButton.SetSize(0.285f, 0.285f, RelativeTo.ScreenHeight);
		m_settingsButton = new PsUIGenericButton(uIHorizontalList);
		m_settingsButton.SetIcon("menu_icon_settings_small", 0.052f);
		m_settingsButton.SetTealColors();
		m_settingsButton.SetMargins(0.011f);
		m_settingsButton.SetAlign(0.1f, 1f);
		m_helpButton = new PsUIGenericButton(uIHorizontalList);
		m_helpButton.SetText("?", 0.055f, 0.07f);
		m_helpButton.SetTealColors();
		m_helpButton.SetMargins(0.001f);
		m_helpButton.SetAlign(0.1f, 0.05f);
		m_helpButton.Destroy();
"""),
    ("else if (PsState.m_adminMode && m_moneyButton != null && m_moneyButton.m_hit)", "else if (true && m_moneyButton != null && m_moneyButton.m_hit)"),
    ("else if (PsState.m_adminMode && m_noMoneyButton != null && m_noMoneyButton.m_hit)", "else if (true && m_noMoneyButton != null && m_noMoneyButton.m_hit)"),
])

# 3) publish / save screens: no "Publish Hidden" / "Save As Template" switches
edit('PsUICenterEditorPublish.cs', [(
    """		if (PsState.m_adminMode)
		{
			UIVerticalList uIVerticalList2 = new UIVerticalList(this, string.Empty);
			uIVerticalList2.SetSpacing(0.01f, RelativeTo.ScreenHeight);
			uIVerticalList2.SetAlign(0.5f, -0.25f);
			uIVerticalList2.RemoveDrawHandler();
			UIText uIText = new UIText(uIVerticalList2, false, string.Empty, "Publish Hidden", "HurmeBold_Font", 0.03f, RelativeTo.ScreenHeight);
			uIText.SetColor("#a8e2ff");
			m_publishHidden = new PsUISwitch(uIVerticalList2);
		}
""", "")])
edit('PsUICenterEditorSave.cs', [(
    """		if (PsState.m_adminMode)
		{
			UIVerticalList uIVerticalList2 = new UIVerticalList(this, string.Empty);
			uIVerticalList2.SetSpacing(0.01f, RelativeTo.ScreenHeight);
			uIVerticalList2.SetAlign(0.5f, -0.25f);
			uIVerticalList2.RemoveDrawHandler();
			UIText uIText = new UIText(uIVerticalList2, false, string.Empty, "Save As Template", "HurmeBold_Font", 0.03f, RelativeTo.ScreenHeight);
			uIText.SetColor("#a8e2ff");
			m_saveAsTemplate = new PsUISwitch(uIVerticalList2, PsState.m_activeGameLoop.m_minigameMetaData.template);
		}
""", "")])

# 4) settings: no Facebook button, a Language button instead
edit('PsUICenterSettings.cs', [(
    """			m_fbButton.SetText("Disconnect from Facebook");
		}
		UIText uIText5 = new UIText(this,""",
    """			m_fbButton.SetText("Disconnect from Facebook");
		}
		m_fbButton.Destroy();
		LocUI.AddSettingsButton(uIVerticalList);
		UIText uIText5 = new UIText(this,""")])

# 5) friends list text: no Facebook
edit('PsUIChallengeFriendList.cs', [(
    '((!string.IsNullOrEmpty(PlayerPrefsX.GetFacebookId())) ? "Invite them to play!" : "Connect to Facebook!")',
    '((!string.IsNullOrEmpty("x")) ? "Invite them to play!" : "Connect to Facebook!")')])

# 6) editor: no admin info box
edit('PsUIEditorEdit.cs', [(
    """	public virtual void CreateAdminInfo()
	{
		if (PsState.m_adminMode)
		{
			UICanvas uICanvas = new UICanvas(this, false, "EditorMenu", null, string.Empty);
			uICanvas.SetHeight(0.1f, RelativeTo.ScreenHeight);
			uICanvas.SetWidth(0.4f, RelativeTo.ScreenWidth);
			uICanvas.SetDrawHandler(UIDrawHandlers.EditorPopupContentArea);
			uICanvas.SetMargins(0.1f, 0.1f, 0.1f, 0.1f, RelativeTo.OwnHeight);
			uICanvas.SetVerticalAlign(1f);
			m_adminInfo = new UITextbox(uICanvas, false, "AdminInfo", string.Empty, "HurmeSemiBold_Font", 0.02f, RelativeTo.ScreenShortest, true, Align.Center);
			m_adminInfo.SetVerticalAlign(1f);
		}
	}
""",
    """	public virtual void CreateAdminInfo()
	{
	}
""")])

# 7) upgrade bars: translated "FREE"
edit('PsUnityUIUpgradeBars.cs', [(
    'm_uiBundle.m_buttonBundles[_upgradeIndex].m_text.text = "FREE";',
    'm_uiBundle.m_buttonBundles[_upgradeIndex].m_text.text = Loc.T("FREE");')])
