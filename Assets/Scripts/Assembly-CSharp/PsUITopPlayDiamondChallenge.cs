using System;

public class PsUITopPlayDiamondChallenge : UICanvas, IPlayMenu
{
	private PsUIGenericButton m_restartButton;

	private PsUIGenericButton m_pauseButton;

	private Action m_restartAction;

	private Action m_pauseAction;

	private UIHorizontalList m_leftArea;

	private int m_starCount;

	private UIFittedSprite m_stars;

	public PsUITopPlayDiamondChallenge(Action _restartAction = null, Action _pauseAction = null)
		: base(null, false, "TopContent", null, string.Empty)
	{
		m_restartAction = _restartAction;
		m_pauseAction = _pauseAction;
		m_starCount = 0;
		RemoveDrawHandler();
		m_leftArea = new UIHorizontalList(this, "UpperLeft");
		m_leftArea.SetMargins(0.01f, RelativeTo.ScreenShortest);
		m_leftArea.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		m_leftArea.SetAlign(0f, 1f);
		m_leftArea.SetHeight(0.15f, RelativeTo.ScreenHeight);
		m_leftArea.RemoveDrawHandler();
		m_stars = new UIFittedSprite(m_leftArea, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_mode_diamond_" + PsState.m_activeMinigame.m_collectedStars));
		PsMetagameManager.ShowResources(m_camera, false, true, false, false, false, 0.15f);
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
		if (m_starCount != PsState.m_activeMinigame.m_collectedStars)
		{
			m_starCount = PsState.m_activeMinigame.m_collectedStars;
			m_stars.SetFrame(PsState.m_uiSheet.m_atlas.GetFrame("menu_mode_diamond_" + m_starCount));
			m_stars.Update();
			m_leftArea.ArrangeContents();
		}
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

	public void ApplyLeftySettings()
	{
	}
}
