using Server;
using UnityEngine;

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

	private PsUIBasePopup m_waitingPopup;

	private PsUIGenericButton m_fbButton;

	private PsUIGenericButton m_forumButton;

	private PsUISwitch m_musicSwitch;

	private PsUISwitch m_soundsSwitch;

	private PsUISwitch m_handedSwitch;

	private PsUISwitch m_everyplaySwitch;

	private PsUIPlayerNameField m_palyerNameField;

	private PsUIGenericButton m_resetButton;

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
