using UnityEngine;

public class PsUIVersusExitPopup : PsUIHeaderedCanvas
{
	private PsUIGenericButton m_exit;

	private PsUIGenericButton m_giveUp;

	private bool m_continue;

	private bool m_exitContinue;

	private VersusMetaData m_versusData;

	private PsGameLoopVersus m_loop;

	private string m_contentText;

	private string m_exitText;

	private string m_giveUpText;

	private string m_headerText;

	public PsUIVersusExitPopup(UIComponent _parent)
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
		m_continue = false;
		m_exitContinue = false;
		m_versusData = PsState.m_activeGameLoop.m_versusData;
		m_loop = PsState.m_activeGameLoop as PsGameLoopVersus;
		m_headerText = "Exit race?";
		m_exitText = "EXIT";
		if (m_versusData.GetOpponentInfo() == null && (m_versusData.GetOwnInfo().score.time > 0 || (m_loop.m_gameMode as PsGameModeRaceVersus).m_bestGhostBlob.data != null))
		{
			m_continue = true;
			m_contentText = "Do you want to <color=#aff6ff>exit</color> or <color=#51ff08>end your turn</color>?";
			m_giveUpText = "END TURN";
			return;
		}
		if (m_versusData.GetOpponentInfo() == null && m_versusData.GetOwnInfo().score.time == 0 && (m_loop.m_gameMode as PsGameModeRaceVersus).m_bestGhostBlob.data == null)
		{
			m_contentText = "Do you want to <color=#aff6ff>exit</color> or <color=#fc5b22>give up</color>?";
			if (m_versusData.GetOwnInfo().rank >= 4)
			{
				m_contentText = "Do you want to <color=#aff6ff>exit</color> or <color=#fc5b22>give up and lose a trophy</color>?";
			}
			m_giveUpText = "GIVE UP";
			return;
		}
		if (m_versusData.GetOpponentInfo() != null && m_loop.m_tryCount < PsState.m_versusChallengeTryAmount)
		{
			m_contentText = "Do you want to <color=#aff6ff>exit</color> or <color=#fc5b22>give up</color>?";
			if (m_versusData.GetOwnInfo().rank >= 4)
			{
				m_contentText = "Do you want to <color=#aff6ff>exit</color> or <color=#fc5b22>give up and lose a trophy</color>?";
			}
			m_giveUpText = "GIVE UP";
			return;
		}
		m_headerText = "End of turn";
		string text = "<color=#fc5b22>You will lose</color> the race if ";
		if (m_versusData.GetOwnInfo().rank >= 4)
		{
			text = "<color=#fc5b22>You will lose a trophy</color> and the race if ";
		}
		m_contentText = text + m_versusData.GetOpponentInfo().score.name + " wins your best time...";
		m_giveUpText = "SAMPSA, remove\nthis button";
		m_exitText = "CONTINUE";
		m_exitContinue = true;
	}

	public void CreateHeaderContent(UIComponent _parent)
	{
		UICanvas uICanvas = new UICanvas(_parent, false, string.Empty, null, string.Empty);
		uICanvas.SetHeight(1f, RelativeTo.ParentHeight);
		uICanvas.SetWidth(1f, RelativeTo.ParentHeight);
		uICanvas.SetHorizontalAlign(0f);
		uICanvas.SetDepthOffset(-10f);
		uICanvas.SetMargins(-0.075f, 0.05f, -0.025f, 0f);
		uICanvas.RemoveDrawHandler();
		UIFittedSprite uIFittedSprite = new UIFittedSprite(uICanvas, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_icon_trophy_full"));
		uIFittedSprite.SetHeight(1.35f, RelativeTo.ParentHeight);
		TransformS.SetRotation(uIFittedSprite.m_TC, new Vector3(0f, 0f, 3.5f));
		UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.025f, 0.025f, 0f, 0f);
		uIHorizontalList.SetHorizontalAlign(0.5f);
		UIText uIText = new UIText(uIHorizontalList, false, string.Empty, m_headerText, "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
	}

	public void CreateContent(UIComponent _parent)
	{
		Initialize();
		_parent.RemoveTouchAreas();
		UITextbox uITextbox = new UITextbox(_parent, false, string.Empty, m_contentText, "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, false, Align.Center, Align.Middle, "#ffffff");
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetAlign(0.5f, 0f);
		uIHorizontalList.SetSpacing(0.1f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0f, 0f, 0.075f, -0.075f);
		if (!m_exitContinue)
		{
			m_giveUp = new PsUIGenericButton(uIHorizontalList);
			m_giveUp.SetText(m_giveUpText);
			m_giveUp.SetRedColors();
			if (m_continue)
			{
				m_giveUp.SetGreenColors();
			}
		}
		m_exit = new PsUIGenericButton(uIHorizontalList);
		m_exit.SetText(m_exitText);
	}

	public override void Step()
	{
		if (m_giveUp != null && m_giveUp.m_hit)
		{
			if (m_continue)
			{
				(GetRoot() as PsUIBasePopup).CallAction("Continue");
			}
			else
			{
				(GetRoot() as PsUIBasePopup).CallAction("Forfeit");
			}
		}
		else if (m_exit != null && m_exit.m_hit)
		{
			if (m_exitContinue)
			{
				(GetRoot() as PsUIBasePopup).CallAction("Continue");
			}
			else
			{
				(GetRoot() as PsUIBasePopup).CallAction("Exit");
			}
		}
		base.Step();
	}
}
