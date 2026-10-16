using System;

public class PsUITopPlaySignal : UICanvas, IPlayMenu
{
	private PsUIGenericButton m_exitButton;

	private Action m_exitAction;

	public PsUITopPlaySignal(Action _exitAction = null)
		: base(null, false, "TopContent", null, string.Empty)
	{
		m_exitAction = _exitAction;
		RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, "UpperLeft");
		uIHorizontalList.SetMargins(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetAlign(0f, 1f);
		uIHorizontalList.RemoveDrawHandler();
		m_exitButton = new PsUIGenericButton(uIHorizontalList);
		m_exitButton.SetText("Skip");
		m_exitButton.SetOrangeColors();
		Update();
	}

	public void ApplyLeftySettings()
	{
	}

	public override void Step()
	{
		if (m_exitButton.m_hit && m_exitAction != null)
		{
			m_exitAction();
		}
		base.Step();
	}
}
