using UnityEngine;

public class PsUICenterTutorialSignTextInput : UIScrollableCanvas
{
	private UICanvas m_mainArea;

	private PsUIGenericButton m_backButton;

	private Keyboard m_keyboard;

	private UIFittedText m_textField;

	private TextModel m_textModel;

	public PsUICenterTutorialSignTextInput(TextModel _model)
		: base(null, string.Empty)
	{
		m_textModel = _model;
		SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
		m_maxScrollInertialY = 0f;
		m_mainArea = new UICanvas(this, false, string.Empty, null, string.Empty);
		m_mainArea.SetHeight(0.4f, RelativeTo.ScreenHeight);
		m_mainArea.SetWidth(1f, RelativeTo.ScreenWidth);
		m_mainArea.SetAlign(0f, 1f);
		m_mainArea.RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(m_mainArea, "UpperLeft");
		uIHorizontalList.SetMargins(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetAlign(0f, 1f);
		uIHorizontalList.RemoveDrawHandler();
		m_backButton = new PsUIGenericButton(uIHorizontalList);
		m_backButton.SetIcon("hud_icon_back", 0.06f);
		m_backButton.SetOrangeColors();
		UIVerticalList uIVerticalList = new UIVerticalList(m_mainArea, string.Empty);
		uIVerticalList.SetWidth(0.58f, RelativeTo.ScreenHeight);
		uIVerticalList.SetSpacing(0.01f, RelativeTo.ScreenHeight);
		uIVerticalList.RemoveDrawHandler();
		UIText uIText = new UIText(uIVerticalList, false, string.Empty, "Sign text", "HurmeBold_Font", 0.03f, RelativeTo.ScreenHeight);
		uIText.SetColor("#a8e2ff");
		uIText.SetMargins(0.03f, 0f, 0f, 0f);
		uIText.SetHorizontalAlign(0f);
		UICanvas uICanvas = new UICanvas(uIVerticalList, true, string.Empty, null, string.Empty);
		uICanvas.SetSize(0.58f, 0.065f, RelativeTo.ScreenHeight);
		uICanvas.SetMargins(0.015f, 0.015f, 0.01f, 0.01f);
		uICanvas.SetDrawHandler(UIDrawHandlers.TextfieldDark);
		m_textField = new UIFittedText(uICanvas, true, string.Empty, string.Empty, "KGSecondChances_Font", true, "#ffffff");
		m_textField.SetHorizontalAlign(0f);
		m_textField.m_tmc.m_textMesh.fontStyle = FontStyle.Italic;
	}

	public override void Update()
	{
		if (m_keyboard == null)
		{
			m_keyboard = new Keyboard(m_textModel);
			m_keyboard.OpenKeyboard();
		}
		base.Update();
	}

	public override void Step()
	{
		m_keyboard.UpdateKeyboard();
		if (m_backButton.m_hit)
		{
			m_keyboard.CloseKeyboard(true);
			Destroy();
			return;
		}
		if (m_textModel.m_done || m_textModel.m_cancelled)
		{
			Destroy();
			return;
		}
		if (m_textModel.m_text != m_textField.m_text)
		{
			m_textField.SetText(m_textModel.m_text);
			m_textField.Update();
		}
		base.Step();
	}
}
