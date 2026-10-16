using System;

public class PsUITopPlayBlock : UICanvas
{
	private PsUIGenericButton m_restartButton;

	private PsUIGenericButton m_pauseButton;

	private Action m_restartAction;

	private Action m_pauseAction;

	public PsUITopPlayBlock(Action _restartAction = null, Action _pauseAction = null)
		: base(null, false, "TopContent", null, string.Empty)
	{
		m_restartAction = _restartAction;
		m_pauseAction = _pauseAction;
		RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, "UpperLeft");
		uIHorizontalList.SetMargins(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetAlign(1f, 1f);
		uIHorizontalList.RemoveDrawHandler();
		m_pauseButton = new PsUIGenericButton(uIHorizontalList);
		m_pauseButton.SetIcon("hud_icon_pause", 0.06f);
		m_pauseButton.SetOrangeColors();
		m_restartButton = new PsUIGenericButton(uIHorizontalList);
		m_restartButton.SetIcon("hud_icon_restart", 0.06f);
		m_restartButton.SetOrangeColors();
		Update();
	}

	public override void Step()
	{
		if (m_restartButton.m_hit)
		{
			if (m_restartAction != null)
			{
				m_restartAction();
			}
		}
		else if (m_pauseButton.m_hit && m_pauseAction != null)
		{
			m_pauseAction();
		}
		base.Step();
	}
}
