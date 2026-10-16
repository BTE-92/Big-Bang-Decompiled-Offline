using System;
using UnityEngine;

public class UITooShortNamePopup : UIScrollableCanvas
{
	private int m_minLength;

	private PsUIGenericButton m_okButton;

	private Action m_closeCallback;

	public UITooShortNamePopup(int _minLength, Action _closeCallback)
		: base(null, "Popup")
	{
		m_minLength = _minLength;
		m_closeCallback = _closeCallback;
		SetWidth(1f, RelativeTo.ScreenWidth);
		SetHeight(1f, RelativeTo.ScreenHeight);
		SetMargins(0.25f, 0.25f, 0.05f, 0.05f);
		SetDepthOffset(20f);
		SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
		UIVerticalList uIVerticalList = new UIVerticalList(this, string.Empty);
		uIVerticalList.SetMargins(0.05f, 0.05f, 0.05f, 0.05f);
		uIVerticalList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		uIVerticalList.SetDrawHandler(PsUIPopup.PopupDrawHandler);
		new UIText(uIVerticalList, false, string.Empty, "Text has to have at least " + m_minLength + " characters.", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest);
		UIHorizontalList uIHorizontalList = new UIHorizontalList(uIVerticalList, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.RemoveTouchAreas();
		uIHorizontalList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		m_okButton = new PsUIGenericButton(uIHorizontalList);
		m_okButton.SetText("Ok", 0.04f);
		m_okButton.SetOrangeColors();
		m_okButton.Update();
		Update();
		UpdatePopupBG();
	}

	public void UpdatePopupBG()
	{
		if (d_Draw != null)
		{
			d_Draw(this);
		}
	}

	public override void Step()
	{
		if (m_okButton.m_hit)
		{
			Destroy();
			m_closeCallback();
		}
		else
		{
			base.Step();
		}
	}

	public static void PopupDrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Material material = ResourceManager.GetMaterial(RESOURCE.MenuMetalMat_Material);
		string hex = "008ce5";
		string hex2 = "0061a3";
		Vector2[] roundedRect = DebugDraw.GetRoundedRect(_c.m_actualWidth, _c.m_actualHeight * 0.95f, (float)Screen.height * 0.025f, 2, Vector2.zero);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.zero, roundedRect, DebugDraw.HexToUint(hex), DebugDraw.HexToUint(hex2), material, _c.m_camera, "BG");
	}
}
