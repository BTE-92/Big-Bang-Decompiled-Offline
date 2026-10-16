using UnityEngine;

public class PsUILevelDescriptionField : PsUIInputTextField
{
	private PsUIProfileImage m_profileImage;

	public PsUILevelDescriptionField()
		: base(null)
	{
	}

	public PsUILevelDescriptionField(UIComponent _parent)
		: base(_parent)
	{
	}

	protected override void ConstructUI()
	{
		SetMinMaxCharacterCount(2, 80);
		UIVerticalList uIVerticalList = new UIVerticalList(this, string.Empty);
		uIVerticalList.SetWidth(0.56f, RelativeTo.ScreenHeight);
		uIVerticalList.SetSpacing(0.015f, RelativeTo.ScreenHeight);
		uIVerticalList.RemoveDrawHandler();
		UIText uIText = new UIText(uIVerticalList, false, string.Empty, "Level Description", "HurmeBold_Font", 0.03f, RelativeTo.ScreenHeight);
		uIText.SetColor("#a8e2ff");
		uIText.SetMargins(0.03f, 0f, 0f, 0f);
		uIText.SetHorizontalAlign(0f);
		UIHorizontalList uIHorizontalList = new UIHorizontalList(uIVerticalList, string.Empty);
		uIHorizontalList.SetSpacing(0.04f, RelativeTo.ScreenHeight);
		uIHorizontalList.RemoveDrawHandler();
		m_profileImage = new PsUIProfileImage(uIHorizontalList, false, string.Empty, PlayerPrefsX.GetFacebookId(), PlayerPrefsX.GetGameCenterId());
		m_profileImage.SetSize(0.08f, 0.08f, RelativeTo.ScreenHeight);
		TransformS.SetRotation(m_profileImage.m_TC, Vector3.forward * 6f);
		m_profileImage.SetAlign(0.5f, 1f);
		m_profileImage.RemoveDrawHandler();
		UIVerticalList uIVerticalList2 = new UIVerticalList(uIHorizontalList, string.Empty);
		uIVerticalList2.SetWidth(0.8f, RelativeTo.ParentWidth);
		uIVerticalList2.SetSpacing(0.005f, RelativeTo.ScreenHeight);
		uIVerticalList2.RemoveDrawHandler();
		UIText uIText2 = new UIText(uIVerticalList2, false, string.Empty, PlayerPrefsX.GetUserName(), "HurmeBold_Font", 0.035f, RelativeTo.ScreenHeight, "#fffec6");
		uIText2.SetHorizontalAlign(0f);
		UITextbox uITextbox = new UITextbox(uIVerticalList2, true, string.Empty, string.Empty, "HurmeRegular_Font", 0.025f, RelativeTo.ScreenHeight);
		uITextbox.SetMargins(0.01f, 0.01f, 0.005f, 0.015f);
		uITextbox.m_tmc.m_textMesh.color = DebugDraw.HexToColor("#33a8d4");
		uITextbox.SetDrawHandler(UIDrawHandlers.DescriptionField);
		uITextbox.SetMinRows(3);
		uITextbox.SetMaxRows(3);
		SetTextField(uITextbox);
	}
}
