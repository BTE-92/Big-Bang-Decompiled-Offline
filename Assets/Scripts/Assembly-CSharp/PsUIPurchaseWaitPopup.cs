using System;

public class PsUIPurchaseWaitPopup : PsUIPopup
{
	private UIVerticalList m_verticalArea;

	private Action m_callBack;

	public PsUIPurchaseWaitPopup(Action _proceed)
		: base(_proceed)
	{
		m_callBack = _proceed;
		UIVerticalList uIVerticalList = new UIVerticalList(this, string.Empty);
		uIVerticalList.SetMargins(0.05f, 0.05f, 0.05f, 0.05f);
		uIVerticalList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		uIVerticalList.SetDrawHandler(null);
		new PsUILoadingAnimation(uIVerticalList, false);
		new UIText(uIVerticalList, false, "Loading", "Waiting for store", "HurmeSemiBold_Font", 0.025f, RelativeTo.ScreenShortest);
		Update();
		UpdatePopupBG();
	}

	public void Close()
	{
		if (m_callBack != null)
		{
			m_callBack();
		}
		Destroy();
	}
}
