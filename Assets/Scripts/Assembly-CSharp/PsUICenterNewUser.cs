using System;
using UnityEngine;

public class PsUICenterNewUser : UICanvas
{
	private PsUIPlayerNameField m_playerNameField;

	private PsUIGenericButton m_facebookButton;

	private PsUIInfoBar m_facebookInfo;

	private PsUIGenericButton m_doneButton;

	private PsUIWaitPopup m_waitingPopup;

	private bool m_nameSet;

	private SpriteSheet m_darkenSpriteSheet;

	private TransformC spriteTC;

	public PsUICenterNewUser(UIComponent _parent)
		: base(_parent, false, "NewUser", null, string.Empty)
	{
		spriteTC = TransformS.AddComponent(EntityManager.AddEntity());
		spriteTC.transform.position = new Vector3(0f, 0f, 26f);
		Material material = new Material(Shader.Find("WOE/Unlit/UnlitTransparentBg"));
		m_darkenSpriteSheet = SpriteS.AddSpriteSheet(m_camera, material, 1f);
		SpriteC sprite = SpriteS.AddComponent(spriteTC, new Frame(0f, 0f, Screen.width, Screen.height), m_darkenSpriteSheet);
		SpriteS.SetColor(sprite, new Color(0f, 0f, 0f, 0f));
		TweenS.AddTransformTween(spriteTC, TweenedProperty.Alpha, TweenStyle.Linear, Vector3.zero, new Vector3(0.5f, 0.5f, 0.5f), 0.2f, 0f, true);
		RemoveDrawHandler();
		UIVerticalList uIVerticalList = new UIVerticalList(this, string.Empty);
		uIVerticalList.SetVerticalAlign(0.65f);
		uIVerticalList.SetSpacing(0.01765f, RelativeTo.ScreenHeight);
		uIVerticalList.RemoveDrawHandler();
		m_playerNameField = new PsUIPlayerNameField(uIVerticalList);
		m_playerNameField.SetHorizontalAlign(0.5f);
		m_playerNameField.SetCallbacks(DoneWriting);
		if (m_playerNameField.GetText().Length < 3)
		{
			m_playerNameField.ChangeTitleColor();
		}
		UIHorizontalList uIHorizontalList = new UIHorizontalList(uIVerticalList, "fbList");
		uIHorizontalList.SetSpacing(-0.0075f, RelativeTo.ScreenWidth);
		uIHorizontalList.RemoveDrawHandler();
		m_facebookInfo = new PsUIInfoBar(uIHorizontalList, "  Connect to Facebook for your Profile Picture and Friends   ", false);
		m_facebookInfo.SetDepthOffset(-15f);
		m_facebookButton = new PsUIGenericButton(uIHorizontalList, 0.25f, 0.25f, 0.003f);
		m_facebookButton.SetIcon("menu_icon_facebook");
		m_facebookButton.SetText("Connect");
		m_facebookButton.SetDepthOffset(-30f);
	}

	private void DoneWriting(string _name)
	{
		m_playerNameField.ChangeTitleColor("60caf5");
		m_nameSet = false;
		CreateDoneButton();
	}

	private void CreateDoneButton()
	{
		if (m_doneButton == null)
		{
			m_doneButton = new PsUIGenericButton(this);
			m_doneButton.SetGreenColors();
			m_doneButton.SetText("Done!", 0.09f);
			m_doneButton.SetAlign(0.85f, 0.15f);
			m_doneButton.Update();
			TweenS.AddTransformTween(m_doneButton.m_TC, TweenedProperty.Scale, TweenStyle.ExpoInOut, Vector3.zero, Vector3.one, 0.4f, 0f, true);
		}
	}

	private void RemoveDoneButton()
	{
		if (m_doneButton != null)
		{
			m_doneButton.Destroy();
			m_doneButton = null;
		}
	}

	public override void Step()
	{
		if (m_doneButton != null && m_doneButton.m_hit)
		{
			DoneButtonPressed();
		}
		else if (m_facebookButton != null && m_facebookButton.m_hit)
		{
			m_waitingPopup = new PsUIWaitPopup();
			FacebookManager.Login(FacebookLoginDone);
		}
		base.Step();
	}

	private void DoneButtonPressed()
	{
		if (!m_nameSet)
		{
			ChangeName(m_playerNameField.GetText(), delegate
			{
				Exit();
			});
		}
		else
		{
			Exit();
		}
	}

	public void Exit()
	{
		(GetRoot() as PsUIBasePopup).CallAction("Exit");
	}

	private void FacebookLoginDone()
	{
		if (!string.IsNullOrEmpty(PlayerPrefsX.GetFacebookId()))
		{
			m_playerNameField.UpdateProfileImage();
			m_facebookButton.Destroy();
			m_facebookInfo.Destroy();
			string facebookName = PlayerPrefsX.GetFacebookName();
			if (string.IsNullOrEmpty(m_playerNameField.GetText()) && !string.IsNullOrEmpty(facebookName))
			{
				m_playerNameField.SetText(facebookName);
				ChangeName(facebookName);
			}
			else
			{
				DestroyWaitingPopup();
			}
		}
		else
		{
			DestroyWaitingPopup();
			if (!string.IsNullOrEmpty(m_playerNameField.GetText()))
			{
				CreateDoneButton();
			}
		}
	}

	private void ChangeName(string _name, Action customCallBack = null)
	{
		if (customCallBack == null)
		{
			customCallBack = delegate
			{
				Debug.Log("Name was changed");
			};
		}
		if (_name.Equals(PlayerPrefsX.GetUserName()))
		{
			return;
		}
		PlayerPrefsX.SetUserName(_name);
		KeyChain.SetUserName(_name);
		if (m_waitingPopup == null)
		{
			m_waitingPopup = new PsUIWaitPopup();
		}
		new PsServerQueueFlow(null, delegate
		{
			PsServerRequest.ServerChangeName(delegate(HttpC c)
			{
				NameChangeSUCCEED(c);
				customCallBack();
			}, NameChangeFAILED);
		}, null);
	}

	private void DestroyWaitingPopup()
	{
		if (m_waitingPopup != null)
		{
			m_waitingPopup.Destroy();
			m_waitingPopup = null;
		}
	}

	private void NameChangeSUCCEED(HttpC _c)
	{
		CreateDoneButton();
		m_nameSet = true;
		Debug.Log("NAME CHANGE SUCCEED");
		PsAchievementManager.Complete("uniqueSnowflake");
		DestroyWaitingPopup();
	}

	private void NameChangeFAILED(HttpC _c)
	{
		Debug.Log("NAME CHANGE FAILED");
		string networkError = ServerErrors.GetNetworkError(_c.www.error);
		ServerManager.ThrowServerErrorException("Connection error", networkError, () => PsServerRequest.ServerChangeName(NameChangeSUCCEED, NameChangeFAILED));
	}

	public override void Destroy()
	{
		if (m_darkenSpriteSheet != null)
		{
			SpriteS.RemoveSpriteSheet(m_darkenSpriteSheet);
			m_darkenSpriteSheet = null;
		}
		EntityManager.RemoveEntity(spriteTC.p_entity);
		base.Destroy();
	}
}
