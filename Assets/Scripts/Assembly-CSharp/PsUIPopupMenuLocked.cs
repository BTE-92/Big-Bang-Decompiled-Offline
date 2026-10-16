using System;

public class PsUIPopupMenuLocked : PsUIPopup
{
	private PsUIGenericButton m_ok;

	public PsUIPopupMenuLocked(Action _proceed = null, Action _cancel = null)
		: base(_proceed, _cancel)
	{
		UIVerticalList uIVerticalList = new UIVerticalList(this, string.Empty);
		uIVerticalList.SetMargins(0.05f, 0.05f, 0.05f, 0.05f);
		uIVerticalList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		uIVerticalList.SetDrawHandler(PsUIPopup.PopupDrawHandler);
		UIHorizontalList uIHorizontalList = new UIHorizontalList(uIVerticalList, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.RemoveTouchAreas();
		uIHorizontalList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		m_ok = new PsUIGenericButton(uIHorizontalList);
		m_ok.SetText("Ok!", 0.04f);
		m_ok.SetGreenColors();
		m_ok.Update();
		Update();
		UpdatePopupBG();
	}

	public override void Step()
	{
		if (m_ok.m_hit)
		{
			Destroy();
		}
		else
		{
			base.Step();
		}
	}
}
