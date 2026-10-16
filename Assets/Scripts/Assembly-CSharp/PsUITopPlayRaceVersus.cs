using System;
using UnityEngine;

public class PsUITopPlayRaceVersus : UICanvas, IPlayMenu
{
	private PsUIGenericButton m_restartButton;

	private PsUIGenericButton m_pauseButton;

	private Action m_restartAction;

	private Action m_pauseAction;

	private UIVerticalList m_leftArea;

	private UIFittedSprite m_trophySprite;

	private PsGameLoopVersus m_loop;

	private UIText m_timer;

	private UIText m_popTimer;

	private int m_trophyCountDownSeconds = 5;

	private float m_opponentTime;

	public PsUITopPlayRaceVersus(Action _restartAction = null, Action _pauseAction = null)
		: base(null, false, "TopContent", null, string.Empty)
	{
		m_restartAction = _restartAction;
		m_pauseAction = _pauseAction;
		m_loop = PsState.m_activeGameLoop as PsGameLoopVersus;
		int ticks = Mathf.RoundToInt(PsState.m_activeMinigame.m_gameTicks);
		float num = HighScores.TicksToTime(ticks);
		m_opponentTime = (((PsState.m_activeGameLoop as PsGameLoopVersus).m_versusData.GetOpponentInfo() != null) ? (PsState.m_activeGameLoop as PsGameLoopVersus).m_versusData.GetOpponentInfo().score.time : 0);
		RemoveDrawHandler();
		m_leftArea = new UIVerticalList(this, "UpperLeft");
		m_leftArea.SetMargins(0.01f, RelativeTo.ScreenShortest);
		m_leftArea.SetMargins(-0.01f, 0.025f, 0.01f, 0.025f, RelativeTo.ScreenShortest);
		m_leftArea.SetSpacing(0.01f, RelativeTo.ScreenShortest);
		m_leftArea.SetAlign(0f, 1f);
		m_leftArea.RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(m_leftArea, "hList");
		uIHorizontalList.SetSpacing(0.15f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetDrawHandler(PsUIDrawHandlers.ResourceLabelBackground);
		uIHorizontalList.SetHeight(0.12f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.175f, -0.045f, 0f, 0f);
		if ((PsState.m_activeGameLoop as PsGameLoopVersus).m_versusData.matchBall)
		{
			uIHorizontalList.SetMargins(0.03f, -0.045f, 0f, 0f);
			UICanvas uICanvas = new UICanvas(uIHorizontalList, false, "TrophyCanvas", null, string.Empty);
			uICanvas.SetSize(0.15f, 0f, RelativeTo.ScreenHeight);
			uICanvas.SetMargins(0f, 0f, -0.085f, -0.115f);
			uICanvas.RemoveDrawHandler();
			string frameName = ((m_opponentTime == 0f || !(num > m_opponentTime)) ? "menu_mode_trophy_1" : "menu_mode_trophy_0");
			m_trophySprite = new UIFittedSprite(uICanvas, false, "trophy", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(frameName));
		}
		m_timer = new UIText(uIHorizontalList, false, string.Empty, HighScores.TicksToTimeString(Mathf.RoundToInt(PsState.m_activeMinigame.m_gameTicks)), "HurmeSemiBoldMN_Font", 0.05f, RelativeTo.ScreenHeight);
		m_timer.SetVerticalAlign(0.35f);
		m_timer.m_tmc.m_horizontalAlign = Align.Right;
		UIHorizontalList uIHorizontalList2 = new UIHorizontalList(this, "UpperLeft");
		uIHorizontalList2.SetMargins(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList2.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList2.SetAlign(1f, 1f);
		uIHorizontalList2.RemoveDrawHandler();
		m_restartButton = new PsUIGenericButton(uIHorizontalList2);
		m_restartButton.SetIcon("hud_icon_restart", 0.06f);
		m_restartButton.SetOrangeColors();
		m_restartButton.SetHorizontalAlign(1f);
		m_restartButton.SetRestartAmount(PsState.m_versusChallengeTryAmount - m_loop.m_tryCount, PsState.m_versusChallengeTryAmount);
		m_pauseButton = new PsUIGenericButton(uIHorizontalList2);
		m_pauseButton.SetIcon("hud_icon_pause", 0.06f);
		m_pauseButton.SetOrangeColors();
		Update();
		Step();
	}

	public override void Step()
	{
		int ticks = Mathf.RoundToInt(PsState.m_activeMinigame.m_gameTicks);
		float num = HighScores.TicksToTime(ticks);
		m_timer.SetText(HighScores.TicksToTimeString(ticks));
		if (m_trophySprite != null && m_opponentTime != 0f)
		{
			if (num > m_opponentTime - (float)m_trophyCountDownSeconds && num < m_opponentTime)
			{
				if (m_popTimer == null)
				{
					float fontSize = 0.05f;
					m_popTimer = new UIText(m_trophySprite, false, "popTimer", Mathf.CeilToInt(m_opponentTime - num).ToString(), "HurmeSemiBoldMN_Font", fontSize, RelativeTo.ScreenHeight);
					m_popTimer.SetDepthOffset(-5f);
					m_popTimer.Update();
				}
				else
				{
					m_popTimer.SetText(Mathf.CeilToInt(m_opponentTime - num).ToString());
				}
			}
			else if (num > m_opponentTime && m_popTimer != null)
			{
				m_popTimer.Destroy();
				m_popTimer = null;
				m_trophySprite.SetFrame(m_trophySprite.m_spriteSheet.m_atlas.GetFrame("menu_mode_trophy_0"));
				m_trophySprite.Update();
			}
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
