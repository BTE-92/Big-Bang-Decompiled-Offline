public class PsUIPlayerNameField : PsUIInputTextField
{
	private PsUIProfileImage m_profileImage;

	private UIText m_title;

	public PsUIPlayerNameField()
		: this(null)
	{
	}

	public PsUIPlayerNameField(UIComponent _parent)
		: base(_parent)
	{
	}

	protected override void ConstructUI()
	{
		SetMinMaxCharacterCount(3, 16);
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
		uIHorizontalList.SetSpacing(0.01f, RelativeTo.ScreenHeight);
		uIHorizontalList.RemoveDrawHandler();
		m_profileImage = new PsUIProfileImage(uIHorizontalList, false, string.Empty, PlayerPrefsX.GetFacebookId(), PlayerPrefsX.GetGameCenterId());
		m_profileImage.SetSize(0.135f, 0.135f, RelativeTo.ScreenHeight);
		m_profileImage.SetAlign(0f, 0f);
		m_profileImage.RemoveDrawHandler();
		UIVerticalList uIVerticalList = new UIVerticalList(uIHorizontalList, string.Empty);
		uIVerticalList.SetWidth(0.6f, RelativeTo.ScreenHeight);
		uIVerticalList.SetSpacing(0.01f, RelativeTo.ScreenHeight);
		uIVerticalList.RemoveDrawHandler();
		m_title = new UIText(uIVerticalList, false, string.Empty, "Enter your name", "HurmeBold_Font", 0.045f, RelativeTo.ScreenHeight);
		m_title.SetColor("#60caf5");
		m_title.SetHorizontalAlign(0.07f);
		UICanvas uICanvas = new UICanvas(uIVerticalList, true, string.Empty, null, string.Empty);
		uICanvas.SetSize(0.6f, 0.09f, RelativeTo.ScreenHeight);
		uICanvas.SetMargins(0.02f, 0.02f, 0.015f, 0.015f);
		uICanvas.SetDrawHandler(UIDrawHandlers.TextfieldDark);
		UIFittedText uIFittedText = new UIFittedText(uICanvas, true, string.Empty, string.Empty, "KGSecondChances_Font", true, "#b1f92a");
		uIFittedText.SetHorizontalAlign(0f);
		SetTextField(uIFittedText);
	}

	public void UpdateProfileImage()
	{
		m_profileImage.m_facebookId = PlayerPrefsX.GetFacebookId();
		m_profileImage.LoadPicture();
		m_profileImage.Update();
	}

	public void ChangeTitleColor(string _color = "fff7e8")
	{
		m_title.SetColor(_color);
	}
}
