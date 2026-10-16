using UnityEngine;

public class PsUICenterWinRaceVersus : UICanvas
{
	private PsGameLoopVersus m_loop;

	private PsGameModeRaceVersus m_mode;

	private PsUIGenericButton m_continueButton;

	private PsUIGenericButton m_restartButton;

	private PsUIGenericButton m_exitButton;

	private PsUIGenericButton m_everyplayButton;

	public PsUICenterWinRaceVersus(UIComponent _parent)
		: base(_parent, false, "CenterContent", null, string.Empty)
	{
		m_loop = PsState.m_activeGameLoop as PsGameLoopVersus;
		m_mode = m_loop.m_gameMode as PsGameModeRaceVersus;
		SetVerticalAlign(0f);
		GetRoot().SetDrawHandler(PsUIDrawHandlers.DebriefBackground);
		RemoveDrawHandler();
		UIVerticalList uIVerticalList = new UIVerticalList(this, string.Empty);
		uIVerticalList.SetWidth(1f, RelativeTo.ParentWidth);
		uIVerticalList.SetVerticalAlign(0f);
		uIVerticalList.SetSpacing(0.025f, RelativeTo.ScreenHeight);
		uIVerticalList.SetMargins(0f, 0f, 0.05f, 0.025f);
		uIVerticalList.RemoveDrawHandler();
		CreateTexts(uIVerticalList);
		PsUIVersusDebriefBanner psUIVersusDebriefBanner = new PsUIVersusDebriefBanner(uIVerticalList, PsState.m_activeGameLoop as PsGameLoopVersus);
		psUIVersusDebriefBanner.SetMargins(0f, 0f, 0.03f, -0.03f);
		if (EveryplayManager.IsEnabled())
		{
			m_everyplayButton = new PsUIGenericButton(uIVerticalList);
			m_everyplayButton.SetIcon("menu_icon_everyplay", 0.075f);
			m_everyplayButton.SetText("Watch\nReplay!");
		}
		else
		{
			uIVerticalList.SetMargins(0f, 0f, 0.05f, 0.1f);
		}
		UIVerticalList uIVerticalList2 = new UIVerticalList(this, string.Empty);
		uIVerticalList2.SetAlign(1f, 0f);
		uIVerticalList2.SetSpacing(0.01f, RelativeTo.ScreenHeight);
		uIVerticalList2.SetMargins(0.025f);
		uIVerticalList2.RemoveDrawHandler();
		if (!m_loop.m_versusWon || (m_loop.m_versusWon && m_loop.m_versusData.GetOpponentInfo() == null && m_loop.m_tryCount < PsState.m_versusChallengeTryAmount))
		{
			m_restartButton = new PsUIGenericButton(uIVerticalList2);
			m_restartButton.SetIcon("hud_icon_restart", 0.08f);
			m_restartButton.SetOrangeColors();
			m_restartButton.SetHorizontalAlign(1f);
			m_restartButton.SetRestartAmount(PsState.m_versusChallengeTryAmount - m_loop.m_tryCount, PsState.m_versusChallengeTryAmount);
		}
		if (m_loop.m_versusWon)
		{
			m_continueButton = new PsUIGenericButton(uIVerticalList2, 0.25f, 0.25f, 0.007f);
			m_continueButton.SetText("Continue", 0.065f);
			m_continueButton.SetHorizontalAlign(1f);
		}
		UIVerticalList uIVerticalList3 = new UIVerticalList(this, string.Empty);
		uIVerticalList3.SetMargins(0.02f);
		uIVerticalList3.SetAlign(0f, 1f);
		uIVerticalList3.RemoveDrawHandler();
		if (!m_loop.m_versusWon || (m_loop.m_versusWon && m_loop.m_versusData.GetOpponentInfo() == null && m_loop.m_tryCount < PsState.m_versusChallengeTryAmount))
		{
			m_exitButton = new PsUIGenericButton(uIVerticalList3);
			m_exitButton.SetOrangeColors();
			m_exitButton.SetIcon("menu_icon_exit");
		}
	}

	public void CreateTexts(UIComponent _parent)
	{
		int num = PsState.m_versusChallengeTryAmount - m_loop.m_tryCount;
		if (m_loop.m_versusData.GetOpponentInfo() == null)
		{
			if ((!m_mode.m_saveGhost || m_mode.m_previousBest == int.MaxValue) && (!m_mode.m_saveGhost || m_mode.m_previousBest != int.MaxValue))
			{
				UIText uIText = new UIText(_parent, false, string.Empty, string.Empty, "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, null, "000000");
				uIText.SetShadowShift(new Vector2(1f, -0.45f), 0.05f);
			}
			if (m_mode.m_previousBest != int.MaxValue)
			{
				int num2 = m_loop.m_timeScoreCurrent - m_mode.m_previousBest;
				string text = "NEW\nBEST TIME!";
				string color = "#51ff08";
				if (num2 >= 0)
				{
					text = "NO RECORD";
					color = "#fc5b22";
				}
				num2 = Mathf.Abs(num2);
				UIText uIText2 = new UIText(_parent, false, string.Empty, text, "KGSecondChances_Font", 0.12f, RelativeTo.ScreenHeight, color, "000000");
				uIText2.SetMargins(0f, 0f, 0.02f, -0.02f);
				uIText2.SetShadowShift(new Vector2(1f, -0.45f), 0.05f);
			}
			else
			{
				UIText uIText3 = new UIText(_parent, false, string.Empty, "NEW\nBEST TIME!", "KGSecondChances_Font", 0.12f, RelativeTo.ScreenHeight, "#51ff08", "000000");
				uIText3.SetMargins(0f, 0f, 0.02f, -0.02f);
				uIText3.SetShadowShift(new Vector2(1f, -0.45f), 0.05f);
			}
			string text2 = "Your best time:  " + HighScores.TimeScoreToTimeString(m_mode.m_bestVersusTime) + string.Empty;
			UIText uIText4 = new UIText(_parent, false, string.Empty, text2, "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, null, "000000");
			uIText4.SetShadowShift(new Vector2(1f, -0.45f), 0.05f);
		}
		else if (m_loop.m_versusWon && m_loop.m_versusData.GetOpponentInfo() != null)
		{
			int num3 = Mathf.Abs(m_loop.m_timeScoreCurrent - m_loop.m_versusData.GetOpponentInfo().score.time);
			string color2 = "#51ff08";
			string text3 = "POLE\nPOSITION!";
			UIText uIText5 = new UIText(_parent, false, string.Empty, text3, "KGSecondChances_Font", 0.12f, RelativeTo.ScreenHeight, color2, "000000");
			if (!m_loop.m_versusData.matchBall)
			{
				string text4 = "Let's see if " + m_loop.m_versusData.GetOpponentInfo().score.name + " can outrun your time " + HighScores.TimeScoreToTimeString(m_mode.m_bestVersusTime) + string.Empty;
				UIText uIText6 = new UIText(_parent, false, string.Empty, text4, "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, null, "000000");
				uIText6.SetShadowShift(new Vector2(1f, -0.45f), 0.05f);
			}
			else
			{
				uIText5.SetText("RACE WON!");
				string text5 = "You earned a TROPHY by winning " + m_loop.m_versusData.GetOpponentInfo().score.name + "!";
				if (m_loop.m_versusData.GetOpponentInfo().rank >= 4)
				{
					text5 = "You nabbed a TROPHY from " + m_loop.m_versusData.GetOpponentInfo().score.name + "!";
				}
				UIText uIText7 = new UIText(_parent, false, string.Empty, text5, "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, null, "000000");
				uIText7.SetShadowShift(new Vector2(1f, -0.45f), 0.05f);
			}
			uIText5.SetMargins(0f, 0f, 0.02f, -0.02f);
			uIText5.SetShadowShift(new Vector2(1f, -0.45f), 0.05f);
		}
		else if (!m_loop.m_versusWon)
		{
			int score = m_loop.m_timeScoreCurrent - m_loop.m_versusData.GetOpponentInfo().score.time;
			string color3 = "#fc5b22";
			UIText uIText8 = new UIText(_parent, false, string.Empty, "DRIVE\nFASTER", "KGSecondChances_Font", 0.12f, RelativeTo.ScreenHeight, color3, "000000");
			uIText8.SetMargins(0f, 0f, 0.02f, -0.02f);
			uIText8.SetShadowShift(new Vector2(1f, -0.45f), 0.05f);
			string text6 = "You were " + HighScores.TimeScoreToTimeString(score) + " seconds slower than " + m_loop.m_versusData.GetOpponentInfo().score.name;
			UIText uIText9 = new UIText(_parent, false, string.Empty, text6, "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, null, "000000");
			uIText9.SetShadowShift(new Vector2(1f, -0.45f), 0.05f);
		}
	}

	public virtual void TotalWin()
	{
		(PsState.m_activeGameLoop as PsGameLoopVersus).m_versusData.winner = PlayerPrefsX.GetUserId();
		PsUIBasePopup popup = new PsUIBasePopup(typeof(PsUICenterVersusClaimCurrent));
		popup.SetAction("Exit", delegate
		{
			popup.Destroy();
			(GetRoot() as PsUIBasePopup).CallAction("Continue");
		});
	}

	public override void Step()
	{
		if (m_continueButton != null && m_continueButton.m_hit)
		{
			if ((PsState.m_activeGameLoop.m_gameMode as PsGameModeRaceVersus).m_totalWin)
			{
				TotalWin();
			}
			else
			{
				(GetRoot() as PsUIBasePopup).CallAction("Continue");
			}
		}
		else if (m_restartButton != null && m_restartButton.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Restart");
		}
		else if (m_exitButton != null && m_exitButton.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
		}
		else if (m_everyplayButton != null && m_everyplayButton.m_hit)
		{
			Debug.Log("Opening everyplay...");
			EveryplayManager.StopRecording(0f);
			EveryplayManager.PlayLastRecording();
		}
		base.Step();
	}
}
