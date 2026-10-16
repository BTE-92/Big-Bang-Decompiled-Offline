using System.Collections.Generic;
using UnityEngine;

public class PsUICreateSignals : UICanvas
{
	protected UIScrollableCanvas m_scrollArea;

	protected UIVerticalList m_contentArea;

	private int m_lastUnlockedCardIndex = -1;

	public List<PsGameLoopEditorPuzzle> m_tutorials = new List<PsGameLoopEditorPuzzle>();

	public List<PsUICreateSignalBanner> m_tutorialBanners = new List<PsUICreateSignalBanner>();

	public PsUICreateSignals(UIComponent _parent)
		: base(_parent, false, "ScrollableContent", null, string.Empty)
	{
		GetRoot().SetDrawHandler(PsUIDrawHandlers.DebriefBackground);
		SetWidth(0.75f, RelativeTo.ScreenWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
		SetVerticalAlign(0f);
		float num = (float)Screen.height * 1.3f / (float)Screen.height;
		RemoveDrawHandler();
		m_scrollArea = new UIScrollableCanvas(this, string.Empty);
		m_scrollArea.SetHeight(1f - num * 0.85f, RelativeTo.ScreenHeight);
		m_scrollArea.SetVerticalAlign(0f);
		m_scrollArea.RemoveDrawHandler();
		m_scrollArea.m_passTouchesToScrollableParents = true;
		m_contentArea = new UIVerticalList(m_scrollArea, string.Empty);
		m_contentArea.SetWidth(1f, RelativeTo.ParentWidth);
		m_contentArea.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		m_contentArea.SetMargins(0.025f, 0.025f, 0.1125f, 0.05f);
		m_contentArea.SetVerticalAlign(1f);
		m_contentArea.RemoveDrawHandler();
		CreateContent(m_contentArea);
		UIScrollableCanvas uIScrollableCanvas = new UIScrollableCanvas(this, "Header");
		uIScrollableCanvas.SetWidth(1.3f, RelativeTo.ScreenHeight);
		uIScrollableCanvas.SetHeight(num, RelativeTo.ScreenHeight);
		uIScrollableCanvas.SetVerticalAlign(1f);
		uIScrollableCanvas.RemoveTouchAreas();
		uIScrollableCanvas.RemoveDrawHandler();
		UIText uIText = new UIText(uIScrollableCanvas, false, string.Empty, "Studied\nItems", "KGSecondChances_Font", 0.06f, RelativeTo.ScreenHeight, "#FFFFFF", "#111111");
		uIText.SetAlign(0.15f, 0.1f);
		uIText.m_TC.transform.localRotation = Quaternion.Euler(Vector3.forward * 3f);
		uIText.m_shadowDistance = 0.05f;
	}

	public virtual void CreateContent(UIComponent _parent)
	{
		UIVerticalList uIVerticalList = new UIVerticalList(m_contentArea, string.Empty);
		uIVerticalList.SetVerticalAlign(1f);
		uIVerticalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		uIVerticalList.RemoveDrawHandler();
		for (int i = 0; i < PsMetagameData.m_unlocks.Count; i++)
		{
			bool unlocked = PsMetagameData.m_unlocks[i].m_unlocked;
			for (int j = 0; j < PsMetagameData.m_unlocks[i].m_unlockables.Count; j++)
			{
				if (PsMetagameData.m_unlocks[i].m_unlockables[j].m_container.m_type != PsUnlockableType.Menu)
				{
					PsUnlockable signal = PsMetagameData.m_unlocks[i].m_unlockables[j];
					PsGameLoopEditorPuzzle gameLoop = null;
					m_tutorialBanners.Add(new PsUICreateSignalBanner(uIVerticalList, gameLoop, signal));
				}
			}
		}
	}

	public override void Step()
	{
		PsUIBasePopup psUIBasePopup = GetRoot() as PsUIBasePopup;
		float currentScrollX = psUIBasePopup.m_scrollableCanvas.m_currentScrollX;
		if (psUIBasePopup.m_pageButtonLeft == null && psUIBasePopup.m_pageButtonRight == null && currentScrollX < 0.1f)
		{
			psUIBasePopup.CreatePageButtonRight("Level\nBuilder");
		}
		base.Step();
	}
}
