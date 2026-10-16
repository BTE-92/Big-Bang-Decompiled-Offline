using System;

public class PsUITopPlayStarCollect : UICanvas, IPlayMenu
{
	protected PsUIGenericButton m_restartButton;

	protected PsUIGenericButton m_pauseButton;

	protected Action m_restartAction;

	protected Action m_pauseAction;

	protected UIHorizontalList m_leftArea;

	protected int m_starCount;

	protected UIFittedSprite m_stars;

	private string m_frameName;

	public PsUITopPlayStarCollect(Action _restartAction = null, Action _pauseAction = null)
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
		CreateScoreArea(m_leftArea);
		CreateCoinArea();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, "UpperLeft");
		uIHorizontalList.SetMargins(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetAlign(1f, 1f);
		uIHorizontalList.RemoveDrawHandler();
		CreateRestartArea(uIHorizontalList);
		Update();
	}

	public virtual void CreateScoreArea(UIComponent _parent)
	{
		if (PsState.m_activeGameLoop.m_context == PsMinigameContext.Fresh)
		{
			m_frameName = "menu_mode_key_";
		}
		else
		{
			m_frameName = "menu_mode_star_";
		}
		m_stars = new UIFittedSprite(_parent, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(m_frameName + PsState.m_activeMinigame.m_collectedStars));
	}

	public virtual void CreateRestartArea(UIComponent _parent)
	{
		m_pauseButton = new PsUIGenericButton(_parent);
		m_pauseButton.SetIcon("hud_icon_pause", 0.06f);
		m_pauseButton.SetOrangeColors();
		m_restartButton = new PsUIGenericButton(_parent);
		m_restartButton.SetIcon("hud_icon_restart", 0.06f);
		m_restartButton.SetOrangeColors();
	}

	public virtual void CreateCoinArea()
	{
		PsMetagameManager.ShowResources(m_camera, false, true, false, false, false, 0.15f);
	}

	public override void Step()
	{
		if (m_stars != null && m_starCount != PsState.m_activeMinigame.m_collectedStars)
		{
			m_starCount = PsState.m_activeMinigame.m_collectedStars;
			m_stars.SetFrame(PsState.m_uiSheet.m_atlas.GetFrame(m_frameName + m_starCount));
			m_stars.Update();
			m_leftArea.ArrangeContents();
		}
		if (m_restartButton != null && m_restartButton.m_hit)
		{
			if (m_restartAction != null)
			{
				m_restartAction();
			}
		}
		else if (m_pauseButton != null && m_pauseButton.m_hit && m_pauseAction != null)
		{
			m_pauseAction();
		}
		base.Step();
	}

	public virtual void ApplyLeftySettings()
	{
	}
}
