using AdMediation;
using Server;
using UnityEngine;

public class PsUISettingsMenuContent : UIScrollableCanvas
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

	private class PsUIPopupKeychainConfirmation : PsUIPopupConfirmation
	{
		public PsUIPopupKeychainConfirmation(UIComponent _parent)
			: base(_parent)
		{
			new UITextbox(this, false, string.Empty, "Are you sure you want to RESET keychain settings?", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest, false, Align.Center, Align.Middle);
		}
	}

	private class PsUIPopupProgressionConfirmation : PsUIPopupConfirmation
	{
		public PsUIPopupProgressionConfirmation(UIComponent _parent)
			: base(_parent)
		{
			new UITextbox(this, false, string.Empty, "Are you sure you want to RESET your progression?", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest, false, Align.Center, Align.Middle);
		}
	}

	private class PsUIPopupStatsConfirmation : PsUIPopupConfirmation
	{
		public PsUIPopupStatsConfirmation(UIComponent _parent)
			: base(_parent)
		{
			new UITextbox(this, false, string.Empty, "Are you sure you want to RESET your stats?", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest, false, Align.Center, Align.Middle);
		}
	}

	private class PsUIPopupBuilderConfirmation : PsUIPopupConfirmation
	{
		public PsUIPopupBuilderConfirmation(UIComponent _parent)
			: base(_parent)
		{
			new UITextbox(this, false, string.Empty, "Are you sure you want to switch to builder account?", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest, false, Align.Center, Align.Middle);
		}
	}

	private class PsUIPopupUserConfirmation : PsUIPopupConfirmation
	{
		public PsUIPopupUserConfirmation(UIComponent _parent)
			: base(_parent)
		{
			new UITextbox(this, false, string.Empty, "Are you sure you want to switch back to player account?", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest, false, Align.Center, Align.Middle);
		}
	}

	private class PsUIPopupWaiting : PsUIHeaderedCanvas
	{
		public PsUIPopupWaiting(UIComponent _parent)
			: base(_parent, string.Empty, false)
		{
			(GetRoot() as PsUIBasePopup).m_scrollableCanvas.SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
			SetWidth(0.65f, RelativeTo.ScreenWidth);
			SetHeight(0.45f, RelativeTo.ScreenHeight);
			SetVerticalAlign(0.4f);
			SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
			SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
			m_header.Destroy();
		}
	}

	private class PsUIPopupFacebookWaiting : PsUIPopupWaiting
	{
		public PsUIPopupFacebookWaiting(UIComponent _parent)
			: base(_parent)
		{
			new UITextbox(this, false, string.Empty, "Disconnecting from Facebook...", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest, false, Align.Center, Align.Middle);
		}
	}

	private class PsUIPopupNameWaiting : PsUIPopupWaiting
	{
		public PsUIPopupNameWaiting(UIComponent _parent)
			: base(_parent)
		{
			new UITextbox(this, false, string.Empty, "Changing your name...", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest, false, Align.Center, Align.Middle);
		}
	}

	private const string FACEBOOK_LOGOUT = "Disconnect from Facebook";

	private const string FACEBOOK_LOGIN = "Connect to Facebook";

	private const string GAMECENTER_LOGIN = "Connect to GameCenter";

	private const string GAMECENTER_LOGOUT = "Disconnect from GameCenter";

	private Entity m_utilityEntity;

	private UIVerticalList m_verticalArea;

	private PsUIGenericButton m_facebookButton;

	private string m_facebookId;

	private PsUIGenericButton m_handedness;

	private PsUIGenericButton m_mute;

	private PsUIGenericButton m_resetGame;

	private PsUIGenericButton m_resetStats;

	private PsUIGenericButton m_resetLocalPath;

	private PsUIGenericButton m_resetServerPath;

	private PsUIGenericButton m_resetKeychain;

	private PsUIGenericButton m_disableAdminMode;

	private PsUIGenericButton m_testElasticSearch;

	private PsUIGenericButton m_testAds;

	private PsUIGenericButton m_everyPlayEnabled;

	private PsUIGenericButton m_changeName;

	private PsUIGenericButton m_graphEditor;

	private PsUIGenericButton m_builder;

	private UITextArea m_adminPass;

	private PsUIGenericButton m_adminPassOK;

	private UIText m_adminPassStatus;

	private UIModel m_passwordModel;

	private UIVerticalList m_passwordArea;

	private PsUIBasePopup m_waitingPopup;

	public PsUISettingsMenuContent(UIComponent _parent)
		: base(_parent, "SettingsMenuContent")
	{
		m_TAC.m_letTouchesThrough = true;
		m_passTouchesToScrollableParents = true;
		m_passwordModel = new UIModel(new PasswordModel(string.Empty));
		m_utilityEntity = EntityManager.AddEntity();
		RemoveDrawHandler();
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
		m_verticalArea = new UIVerticalList(this, "VerticalArea");
		m_verticalArea.SetVerticalAlign(1f);
		m_verticalArea.SetHorizontalAlign(0.5f);
		m_verticalArea.SetMargins(0.025f, 0.025f, 0.075f, 0.075f, RelativeTo.ScreenShortest);
		m_verticalArea.SetSpacing(0.05f, RelativeTo.ScreenShortest);
		m_verticalArea.RemoveDrawHandler();
		m_handedness = new PsUIGenericButton(m_verticalArea);
		m_handedness.SetText("Left handed: " + ((!PsState.m_editorIsLefty) ? "<color=#e83808>" : "<color=#a0ff00>") + PsState.m_editorIsLefty + "</color>", 0.04f);
		m_handedness.SetOrangeColors();
		m_handedness.SetHeight(0.06f, RelativeTo.ScreenHeight);
		m_handedness.Update();
		if (EveryplayManager.IsSupported())
		{
			m_everyPlayEnabled = new PsUIGenericButton(m_verticalArea);
			m_everyPlayEnabled.SetText("Everyplay: " + ((!PsState.m_everyplayEnabled) ? "<color=#e83808>" : "<color=#a0ff00>") + ((!PsState.m_everyplayEnabled) ? "Disabled" : "Enabled") + "</color>", 0.04f);
			m_everyPlayEnabled.SetOrangeColors();
			m_everyPlayEnabled.SetHeight(0.06f, RelativeTo.ScreenHeight);
			m_everyPlayEnabled.Update();
		}
		else
		{
			m_everyPlayEnabled = null;
		}
		m_changeName = new PsUIGenericButton(m_verticalArea);
		m_changeName.SetText("Change your name", 0.04f);
		m_changeName.SetOrangeColors();
		m_changeName.SetHeight(0.06f, RelativeTo.ScreenHeight);
		m_changeName.Update();
		if (PlayerPrefsX.GetFacebookId() != null)
		{
			m_facebookButton = new PsUIGenericButton(m_verticalArea);
			m_facebookButton.SetText("Disconnect from Facebook", 0.04f);
			m_facebookButton.SetHeight(0.06f, RelativeTo.ScreenHeight);
		}
		m_resetGame = new PsUIGenericButton(m_verticalArea);
		m_resetGame.SetText("RESET GAME", 0.04f);
		m_resetGame.SetHeight(0.06f, RelativeTo.ScreenHeight);
	}

	private void GenerateDevBuildButtons()
	{
		if (m_passwordArea != null)
		{
			m_passwordArea.Destroy();
		}
		m_passwordArea = new UIVerticalList(m_verticalArea, string.Empty);
		m_passwordArea.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		m_passwordArea.RemoveDrawHandler();
		m_passwordArea.RemoveTouchAreas();
		if (1 == 0)
		{
			m_adminPass = new UITextArea(m_passwordArea, string.Empty, PsStrings.Get(StringID.BUTTON_SETTINGS_ADMIN), string.Empty, m_camera, m_passwordModel, "m_password");
			m_adminPassOK = new PsUIGenericButton(m_passwordArea);
			m_adminPassOK.SetText("Enter Admin Mode", 0.04f);
			m_adminPassOK.SetGreenColors();
			m_adminPassOK.SetHeight(0.06f, RelativeTo.ScreenHeight);
			m_adminPassOK.Update();
			m_adminPassStatus = new UIText(m_passwordArea, false, string.Empty, "Enter password to access admin mode", "HurmeSemiBold_Font", 0.025f, RelativeTo.ScreenShortest);
		}
		else
		{
			GenerateAdminModeButtons();
		}
	}

	private void GenerateAdminModeButtons()
	{
		m_adminPassStatus = new UIText(m_passwordArea, false, string.Empty, "Admin mode is active.", "HurmeSemiBold_Font", 0.025f, RelativeTo.ScreenShortest);
		m_testElasticSearch = new PsUIGenericButton(m_passwordArea);
		m_testElasticSearch.SetText("Test Elastic Search");
		m_testElasticSearch.SetHeight(0.05f, RelativeTo.ScreenHeight);
		m_testAds = new PsUIGenericButton(m_passwordArea);
		m_testAds.SetText("Test Ads");
		m_testAds.SetHeight(0.05f, RelativeTo.ScreenHeight);
		m_builder = new PsUIGenericButton(m_passwordArea);
		m_builder.SetText((!(KeyChain.GetUserId() == "5582757b3800005834b7897b")) ? "Switch to builder account" : "Switch to player account");
		m_builder.SetHeight(0.05f, RelativeTo.ScreenHeight);
		m_graphEditor = new PsUIGenericButton(m_passwordArea);
		m_graphEditor.SetText("Progression Editor");
		m_graphEditor.SetHeight(0.05f, RelativeTo.ScreenHeight);
		m_resetLocalPath = new PsUIGenericButton(m_passwordArea);
		m_resetLocalPath.SetText("Reset Local Path");
		m_resetLocalPath.SetHeight(0.05f, RelativeTo.ScreenHeight);
		m_resetServerPath = new PsUIGenericButton(m_passwordArea);
		m_resetServerPath.SetText("Reset Server Path");
		m_resetServerPath.SetHeight(0.05f, RelativeTo.ScreenHeight);
		m_resetStats = new PsUIGenericButton(m_passwordArea);
		m_resetStats.SetText("Reset stats");
		m_resetStats.SetHeight(0.05f, RelativeTo.ScreenHeight);
		m_resetKeychain = new PsUIGenericButton(m_passwordArea);
		m_resetKeychain.SetText("Reset keychain");
		m_resetKeychain.SetHeight(0.05f, RelativeTo.ScreenHeight);
		m_disableAdminMode = new PsUIGenericButton(m_passwordArea);
		m_disableAdminMode.SetText("Disable admin mode");
		m_disableAdminMode.SetHeight(0.05f, RelativeTo.ScreenHeight);
	}

	private void FacebookLoginDone()
	{
		Debug.Log("Facebook disconnected");
		m_facebookId = PlayerPrefsX.GetFacebookId();
		UIComponent parent = m_facebookButton.m_parent;
		m_facebookButton.Destroy();
		parent.Update();
		if (m_waitingPopup != null)
		{
			m_waitingPopup.Destroy();
			m_waitingPopup = null;
		}
	}

	public override void Destroy()
	{
		base.Destroy();
		EntityManager.RemoveEntity(m_utilityEntity);
	}

	private void DoneCallback(string _value)
	{
		Debug.Log("DONE: " + _value);
		m_adminPass.SetValue(_value);
	}

	private void KeyPressedCallback(string _value)
	{
		Debug.Log("PRESSED: " + _value);
	}

	private void CancelCallback()
	{
		Debug.Log("CANCEL");
	}

	public override void Step()
	{
		if (m_adminPass != null && m_adminPass.m_hit)
		{
			UITextInput uITextInput = new UITextInput("Admin password", DoneCallback, CancelCallback, KeyPressedCallback);
			uITextInput.SetText((string)m_adminPass.GetValue());
			uITextInput.Update();
		}
		else if (m_facebookButton != null && m_facebookButton.m_hit)
		{
			m_waitingPopup = new PsUIBasePopup(typeof(PsUIPopupFacebookWaiting), null, null, null, false);
			FacebookManager.Logout(FacebookLoginDone, true);
		}
		if (m_handedness.m_hit)
		{
			PsState.m_editorIsLefty = !PsState.m_editorIsLefty;
			m_handedness.SetText("Left handed: " + ((!PsState.m_editorIsLefty) ? "<color=#e83808>" : "<color=#a0ff00>") + PsState.m_editorIsLefty + "</color>", 0.04f);
			m_handedness.Update();
			PlayerPrefsX.SetLefty(PsState.m_editorIsLefty);
		}
		if (m_everyPlayEnabled != null && m_everyPlayEnabled.m_hit)
		{
			PsState.m_everyplayEnabled = !PsState.m_everyplayEnabled;
			m_everyPlayEnabled.SetText("Everyplay: " + ((!PsState.m_everyplayEnabled) ? "<color=#e83808>" : "<color=#a0ff00>") + ((!PsState.m_everyplayEnabled) ? "Disabled" : "Enabled") + "</color>", 0.04f);
			m_everyPlayEnabled.Update();
			PlayerPrefsX.SetEveryplayEnabled(PsState.m_everyplayEnabled);
			EveryplayManager.SetEnabled(PsState.m_everyplayEnabled);
		}
		if (m_changeName.m_hit)
		{
			PsUIBasePopup psUIBasePopup = new PsUIBasePopup(typeof(PsUIPopupChangeName), null, null, null, false);
			psUIBasePopup.SetAction("Proceed", ChangeName);
			(psUIBasePopup.m_mainContent as PsUIPopupChangeName).CreateUI();
			psUIBasePopup.Update();
		}
		if (m_resetKeychain != null && m_resetKeychain.m_hit)
		{
			PsUIBasePopup psUIBasePopup2 = new PsUIBasePopup(typeof(PsUIPopupKeychainConfirmation), null, null, null, false);
			psUIBasePopup2.SetAction("Proceed", ResetKeychain);
		}
		if (m_disableAdminMode != null && m_disableAdminMode.m_hit)
		{
			PsState.m_adminMode = false;
			GenerateDevBuildButtons();
			Update();
		}
		if (m_resetGame != null && m_resetGame.m_hit)
		{
			PsUIBasePopup psUIBasePopup3 = new PsUIBasePopup(typeof(PsUIPopupProgressionConfirmation), null, null, null, false);
			psUIBasePopup3.SetAction("Proceed", ResetGame);
		}
		if (m_resetStats != null && m_resetStats.m_hit)
		{
			PsUIBasePopup psUIBasePopup4 = new PsUIBasePopup(typeof(PsUIPopupStatsConfirmation), null, null, null, false);
			psUIBasePopup4.SetAction("Proceed", ResetStats);
		}
		else if (m_builder != null && m_builder.m_hit)
		{
			if (KeyChain.GetUserId() == "5582757b3800005834b7897b")
			{
				PsUIBasePopup psUIBasePopup5 = new PsUIBasePopup(typeof(PsUIPopupUserConfirmation), null, null, null, false);
				psUIBasePopup5.SetAction("Proceed", ChangeToOriginal);
			}
			else
			{
				PsUIBasePopup psUIBasePopup6 = new PsUIBasePopup(typeof(PsUIPopupBuilderConfirmation), null, null, null, false);
				psUIBasePopup6.SetAction("Proceed", ChangeToBuilder);
			}
		}
		else if (m_testElasticSearch != null && m_testElasticSearch.m_hit)
		{
			MinigameSearchParametres minigameSearchParametres = new MinigameSearchParametres();
			HttpC httpC = MiniGame.SearchMinigames(minigameSearchParametres, SearchOnlyMetaDataSUCCESS, SearchOnlyMetaDataFAILED, null, 300);
			httpC.objectData = minigameSearchParametres;
			Debug.LogWarning("SEARCH METADATA");
		}
		else if (m_testAds != null && m_testAds.m_hit && PsAdMediation.adsAvailable())
		{
			TouchAreaS.Disable();
			PsAdMediation.ShowAd(adCallback);
		}
		else
		{
			bool hit;
			int num;
			if (m_graphEditor != null)
			{
				hit = m_graphEditor.m_hit;
			}
			else
				num = 0;
			if (m_resetLocalPath != null && m_resetLocalPath.m_hit)
			{
				PsPlanetManager.GetCurrentPlanet().ResetLocalProgression();
				Main.m_currentGame.m_sceneManager.ChangeScene(new StartupScene("StartupScene"));
			}
			else if (m_resetServerPath == null || !m_resetServerPath.m_hit)
			{
				bool hit2;
				int num2;
				if (m_adminPassOK != null)
				{
					hit2 = m_adminPassOK.m_hit;
				}
				else
					num2 = 0;
			}
			else
			{
				PsPlanetManager.GetCurrentPlanet().ResetServerProgression();
			}
		}
		base.Step();
	}

	private void ResetKeychain()
	{
		TouchAreaS.Disable();
		KeyChain.SetUserId(string.Empty);
		KeyChain.SetGameCenterId(string.Empty);
		TouchAreaS.Enable();
	}

	private void ResetGame()
	{
		ResetFlow.Start();
	}

	private void ResetOK(HttpC _c)
	{
		PsServerRequest.ServerResetProgression(ProgressionServerResetSUCCEED, ProgressionServerResetFAILED);
	}

	private void ProgressionServerResetSUCCEED(HttpC _c)
	{
		PsPlanetManager.GetCurrentPlanet().ResetLocalProgression();
		PsMetagameManager.ResetProgressionSUCCEED(_c);
		TouchAreaS.Enable();
		Main.m_currentGame.m_sceneManager.ChangeScene(new StartupScene("StartupScene"));
	}

	private void ProgressionServerResetFAILED(HttpC _c)
	{
		Debug.Log("PROGRESSION RESET FAILED");
		string networkError = ServerErrors.GetNetworkError(_c.www.error);
		ServerManager.ThrowServerErrorException("Connection error", networkError, () => PsServerRequest.ServerResetProgression(ProgressionServerResetSUCCEED, ProgressionServerResetFAILED));
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

	private void ChangeToBuilder()
	{
		PlayerPrefs.SetString("ORIG_USER", KeyChain.GetUserId());
		PlayerPrefs.SetString("ORIG_NAME", KeyChain.GetUserName());
		KeyChain.SetUserId("5582757b3800005834b7897b");
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

	private void ChangeName()
	{
		m_waitingPopup = new PsUIBasePopup(typeof(PsUIPopupNameWaiting), null, null, null, false);
		new PsServerQueueFlow(null, delegate
		{
			PsServerRequest.ServerChangeName(NameChangeSUCCEED, NameChangeFAILED);
		}, null);
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

	private void adCallback(AdResult _result)
	{
		TouchAreaS.Enable();
		Debug.Log("Ad display result: " + _result);
	}

	protected void SearchOnlyMetaDataFAILED(HttpC _c)
	{
		Debug.LogError("ELASTIC SEARCH FAILED " + _c.www);
	}

	protected virtual void SearchOnlyMetaDataSUCCESS(PsMinigameMetaData[] _minigameMetaData)
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
