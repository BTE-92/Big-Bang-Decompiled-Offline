public class PsUIVersusOutOfTriesPopup : PsUIHeaderedCanvas
{
	private PsUIGenericButton m_continue;

	private PsUIGenericButton m_giveUp;

	private VersusMetaData m_versusData;

	private PsGameLoopVersus m_loop;

	private PsGameModeRaceVersus m_mode;

	private bool m_onlyForfeit;

	private bool m_sendBest;

	private string m_contentText;

	private string m_continueText;

	private string m_giveUpText;

	public PsUIVersusOutOfTriesPopup(UIComponent _parent)
		: base(_parent, string.Empty, false)
	{
		SetWidth(0.65f, RelativeTo.ScreenWidth);
		SetHeight(0.45f, RelativeTo.ScreenHeight);
		SetVerticalAlign(0.4f);
		SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
		SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
		m_header.SetDrawHandler(PsUIDrawHandlers.ScrollingUIHeader);
		m_header.SetMargins(0.0125f, 0.0125f, 0.0125f, 0f);
		CreateContent(this);
		CreateHeaderContent(m_header);
	}

	public void Initialize()
	{
		m_onlyForfeit = false;
		m_sendBest = false;
		m_loop = PsState.m_activeGameLoop as PsGameLoopVersus;
		m_mode = m_loop.m_gameMode as PsGameModeRaceVersus;
		m_versusData = m_loop.m_versusData;
		if (m_versusData.GetOpponentInfo() == null)
		{
			if (m_versusData.GetOwnInfo().score.time == 0 && m_mode.m_bestGhostBlob.data == null)
			{
				m_contentText = "This race has been <color=#fc5b22>abandoned</color>, as you didn't reach the goal.";
				if (m_versusData.GetOwnInfo().rank >= 4)
				{
					m_contentText += "\nYou <color=#fc5b22>lost one trophy</color>.";
				}
				m_giveUpText = "Continue";
				m_onlyForfeit = true;
			}
			else
			{
				m_contentText = "Buckle up, it's <color=#aff6ff>opponent's turn</color> to race against your best time!";
				m_continueText = "Continue";
				m_sendBest = true;
			}
		}
		else
		{
			m_contentText = "Buckle up, it's <color=#aff6ff>" + m_versusData.GetOpponentInfo().score.name + "'s turn </color> to race against your best time!";
			string text = "<color=#fc5b22>You will lose</color> the race if ";
			if (m_versusData.GetOwnInfo().rank >= 4)
			{
				text = "<color=#fc5b22>You will lose a trophy</color> and the race if ";
			}
			if (m_versusData.GetOwnInfo().score.time == 0 && m_mode.m_bestGhostBlob.data == null)
			{
				m_contentText = text + m_versusData.GetOpponentInfo().score.name + " reaches the goal...";
			}
			m_continueText = "Continue";
			m_sendBest = true;
		}
	}

	public void CreateHeaderContent(UIComponent _parent)
	{
		UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.025f, 0.025f, 0f, 0f);
		uIHorizontalList.SetHorizontalAlign(0.5f);
		UIText uIText = new UIText(uIHorizontalList, false, string.Empty, "End of turn", "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
	}

	public void CreateContent(UIComponent _parent)
	{
		Initialize();
		_parent.RemoveTouchAreas();
		UITextbox uITextbox = new UITextbox(_parent, false, string.Empty, m_contentText, "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, false, Align.Center, Align.Middle, "#aff6f");
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetAlign(0.5f, 0f);
		uIHorizontalList.SetSpacing(0.1f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0f, 0f, 0.075f, -0.075f);
		if (m_onlyForfeit)
		{
			m_giveUp = new PsUIGenericButton(uIHorizontalList);
			m_giveUp.SetText(m_giveUpText);
			m_giveUp.SetRedColors();
		}
		if (m_sendBest)
		{
			m_continue = new PsUIGenericButton(uIHorizontalList);
			m_continue.SetText(m_continueText);
		}
	}

	public override void Step()
	{
		if (m_continue != null && m_continue.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Continue");
		}
		else if (m_giveUp != null && m_giveUp.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Forfeit");
		}
		base.Step();
	}
}
