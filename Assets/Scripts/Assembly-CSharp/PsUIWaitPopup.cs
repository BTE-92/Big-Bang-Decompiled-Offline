using System;

public class PsUIWaitPopup : PsUIPopup
{
	private UIVerticalList m_verticalArea;

	private Action m_callBack;

	public PsUIWaitPopup(Action _proceed = null)
		: base(_proceed)
	{
		m_callBack = _proceed;
		UIVerticalList uIVerticalList = new UIVerticalList(this, string.Empty);
		uIVerticalList.SetMargins(0.05f, 0.05f, 0.05f, 0.05f);
		uIVerticalList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		uIVerticalList.SetDrawHandler(null);
		new PsUILoadingAnimation(uIVerticalList, false);
		new UIText(uIVerticalList, false, "Loading", "Loading...", "HurmeSemiBold_Font", 0.025f, RelativeTo.ScreenShortest);
		Update();
		UpdatePopupBG();
	}
}
