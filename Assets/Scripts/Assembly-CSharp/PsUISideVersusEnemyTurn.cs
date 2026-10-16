using UnityEngine;

public class PsUISideVersusEnemyTurn : PsUIScrollableBase
{
	private UIVerticalList m_vlist;

	private bool m_created;

	public PsUISideVersusEnemyTurn(UIComponent _parent)
		: base(_parent)
	{
		m_scrollArea.m_maxScrollInertialY = 50f / (1024f / (float)Screen.height);
		m_scrollArea.m_maxScrollInertialX = 0f;
		m_scrollArea.m_TAC.m_letTouchesThrough = true;
	}

	public override void CreateHeaderContent(UIComponent _parent)
	{
		UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.025f, 0.025f, 0f, 0f);
		uIHorizontalList.SetHorizontalAlign(0.5f);
		UIText uIText = new UIText(uIHorizontalList, false, string.Empty, "Opponent's turn", "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
	}

	public override void Step()
	{
		if (PsPlanetManager.m_timedEvents != null && !m_created)
		{
			CreateContent(m_scrollArea);
		}
		base.Step();
	}

	public override void CreateContent(UIComponent _parent)
	{
		if (PsPlanetManager.m_timedEvents == null)
		{
			return;
		}
		m_vlist = new UIVerticalList(_parent, string.Empty);
		m_vlist.SetMargins(0f, 0f, 0.03f, 0.03f);
		m_vlist.SetSpacing(0.0125f, RelativeTo.ScreenHeight);
		m_vlist.RemoveDrawHandler();
		for (int i = 0; i < PsPlanetManager.m_timedEvents.versusChallenges.Count; i++)
		{
			VersusMetaData versusMetaData = PsPlanetManager.m_timedEvents.versusChallenges[i];
			PsGameLoopVersus psGameLoopVersus = new PsGameLoopVersus(PsMinigameContext.Versus, versusMetaData, versusMetaData.timedEventId, versusMetaData.gameId, null, 1, -1, 0, versusMetaData.timeLeft, true);
			if (!(versusMetaData.currentPlayer != PlayerPrefsX.GetUserId()) || versusMetaData.GetOpponentInfo() == null || string.IsNullOrEmpty(versusMetaData.winner))
			{
			}
		}
		m_created = true;
	}
}
