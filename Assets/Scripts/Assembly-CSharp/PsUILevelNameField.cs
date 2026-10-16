using UnityEngine;

public class PsUILevelNameField : PsUIInputTextField
{
	public PsUILevelNameField()
		: base(null)
	{
	}

	public PsUILevelNameField(UIComponent _parent)
		: base(_parent)
	{
	}

	protected override void ConstructUI()
	{
		SetMinMaxCharacterCount(1, 20);
		UIVerticalList uIVerticalList = new UIVerticalList(this, string.Empty);
		uIVerticalList.SetWidth(0.58f, RelativeTo.ScreenHeight);
		uIVerticalList.SetSpacing(0.01f, RelativeTo.ScreenHeight);
		uIVerticalList.RemoveDrawHandler();
		UIText uIText = new UIText(uIVerticalList, false, string.Empty, "Level name", "HurmeBold_Font", 0.03f, RelativeTo.ScreenHeight);
		uIText.SetColor("#a8e2ff");
		uIText.SetMargins(0.03f, 0f, 0f, 0f);
		uIText.SetHorizontalAlign(0f);
		UICanvas uICanvas = new UICanvas(uIVerticalList, true, string.Empty, null, string.Empty);
		uICanvas.SetSize(0.58f, 0.065f, RelativeTo.ScreenHeight);
		uICanvas.SetMargins(0.015f, 0.015f, 0.01f, 0.01f);
		uICanvas.SetDrawHandler(UIDrawHandlers.TextfieldDark);
		UIFittedText uIFittedText = new UIFittedText(uICanvas, true, string.Empty, string.Empty, "KGSecondChances_Font", true, "#ffffff");
		uIFittedText.SetHorizontalAlign(0f);
		uIFittedText.m_tmc.m_textMesh.fontStyle = FontStyle.Italic;
		SetTextField(uIFittedText);
	}
}
