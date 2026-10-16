using System;

public class PsUIPopupPlaylistReward : PsUIPopup
{
	private UIVerticalList m_verticalArea;

	private PsUIGenericButton m_close;

	public PsUIPopupPlaylistReward(string _rewardMessage, Action _proceed = null, Action _cancel = null)
		: base(_proceed, _cancel)
	{
		UIVerticalList uIVerticalList = new UIVerticalList(this, string.Empty);
		uIVerticalList.SetMargins(0.05f, 0.05f, 0.05f, 0.05f);
		uIVerticalList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		uIVerticalList.SetDrawHandler(PsUIPopup.PopupDrawHandler);
		new UITextbox(uIVerticalList, false, string.Empty, _rewardMessage, "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest);
		UIHorizontalList uIHorizontalList = new UIHorizontalList(uIVerticalList, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.RemoveTouchAreas();
		uIHorizontalList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		m_close = new PsUIGenericButton(uIHorizontalList);
		m_close.SetText("Nice!", 0.04f);
		m_close.SetGreenColors();
		m_close.Update();
		Update();
		UpdatePopupBG();
	}

	public override void Step()
	{
		if (m_close.m_hit)
		{
			Destroy();
		}
		else
		{
			base.Step();
		}
	}
}
