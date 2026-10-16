using UnityEngine;

public class PsUIPopupChangeName : PsUIHeaderedCanvas
{
	private class UINameTextArea : UITextArea
	{
		public UINameTextArea(UIComponent _parent, string _tag, string _label, string _tip, Camera _camera, UIModel _model, string _STRINGfieldName, int rows = -1, int _maxCharacterCount = 128)
			: base(_parent, _tag, _label, _tip, _camera, _model, _STRINGfieldName, rows, _maxCharacterCount)
		{
			m_labelText = (string)GetValue();
		}
	}

	public string m_userName = string.Empty;

	public UIModel m_nameModel;

	private PsUIPlayerNameField m_playerNameField;

	private PsUIGenericButton m_changeButton;

	private bool m_changeEnabled;

	private bool m_login;

	public PsUIPopupChangeName(UIComponent _parent)
		: base(_parent, string.Empty, false)
	{
		Debug.LogWarning("Change name");
		(GetRoot() as PsUIBasePopup).m_scrollableCanvas.SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
		SetWidth(0.65f, RelativeTo.ScreenWidth);
		SetHeight(0.45f, RelativeTo.ScreenHeight);
		SetVerticalAlign(0.4f);
		SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
		SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
		m_header.SetDrawHandler(PsUIDrawHandlers.ScrollingUIHeader);
		m_header.SetMargins(0.0125f, 0.0125f, 0.0125f, 0f);
		m_changeEnabled = true;
		m_fieldName = "ChangeName";
		m_nameModel = new UIModel(this);
		m_userName = PlayerPrefsX.GetUserName();
	}

	public void CreateUI(bool login = false)
	{
		m_login = login;
		UIHorizontalList uIHorizontalList = new UIHorizontalList(m_header, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.025f, 0.025f, 0f, 0f);
		uIHorizontalList.SetHorizontalAlign(0.5f);
		UIText uIText = new UIText(uIHorizontalList, false, string.Empty, "Enter your name", "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
		m_playerNameField = new PsUIPlayerNameField(this);
		m_playerNameField.SetText(PlayerPrefsX.GetUserName());
		m_playerNameField.SetCallbacks(ChangeName);
		UIHorizontalList uIHorizontalList2 = new UIHorizontalList(this, "nameChangeButtons");
		uIHorizontalList2.SetHeight(0.1f, RelativeTo.ScreenHeight);
		uIHorizontalList2.RemoveDrawHandler();
		uIHorizontalList2.SetVerticalAlign(0f);
		uIHorizontalList2.SetMargins(0f, 0f, 0.075f, -0.075f);
		m_changeButton = new PsUIGenericButton(uIHorizontalList2);
		m_changeButton.SetText("Ok");
		m_changeButton.SetGreenColors();
	}

	private void ChangeName(string _name)
	{
		if (!_name.Equals(PlayerPrefsX.GetUserName()))
		{
			m_userName = _name;
			if (m_changeButton != null && (string.IsNullOrEmpty(m_userName) || m_userName.Length < 3) && m_changeEnabled)
			{
				m_changeEnabled = false;
				m_changeButton.SetGreenColors(false);
				m_changeButton.DisableTouchAreas();
				m_changeButton.Update();
			}
			else if (m_changeButton != null && !string.IsNullOrEmpty(m_userName) && m_userName.Length >= 3 && !m_changeEnabled)
			{
				m_changeEnabled = true;
				m_changeButton.SetGreenColors();
				m_changeButton.EnableTouchAreas();
				m_changeButton.Update();
			}
		}
	}

	public override void Step()
	{
		if (m_changeButton != null && m_changeButton.m_hit)
		{
			if (!m_userName.Equals(PlayerPrefsX.GetUserName()) || m_login)
			{
				PlayerPrefsX.SetUserName(m_userName);
				KeyChain.SetUserName(m_userName);
				(GetRoot() as PsUIBasePopup).CallAction("Proceed");
				(GetRoot() as PsUIBasePopup).Destroy();
			}
			else
			{
				(GetRoot() as PsUIBasePopup).Destroy();
			}
		}
		base.Step();
	}
}
