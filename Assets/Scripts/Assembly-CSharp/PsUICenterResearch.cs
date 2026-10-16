using UnityEngine;

public class PsUICenterResearch : UICanvas
{
	protected UIScrollableCanvas m_scrollArea;

	protected UIHorizontalList m_hList;

	private UIFittedSprite m_sprite;

	private PsUIGenericButton m_proceed;

	public int m_lastUnlockedCardIndex = -1;

	public PsUICenterResearch(UIComponent _parent)
		: base(_parent, false, string.Empty, null, string.Empty)
	{
		SetMargins(0f, 0f, 0f, 0f);
		RemoveDrawHandler();
		SetHeader(this);
		PsUIInfoBar psUIInfoBar = new PsUIInfoBar(GetRoot(), string.Empty);
		psUIInfoBar.SetVerticalAlign(0f);
		if (Random.value < 0.5f)
		{
			psUIInfoBar.SetText("Levels you PLAY feature unlocked items");
		}
		else
		{
			psUIInfoBar.SetText("Unlocked items can be used when BUILDING new levels");
		}
		m_scrollArea = new UIScrollableCanvas(this, "ResearchScrollableCanvas");
		m_scrollArea.SetHeight(1f, RelativeTo.ParentHeight);
		m_scrollArea.SetWidth(1f, RelativeTo.ParentWidth);
		m_scrollArea.SetVerticalAlign(0f);
		m_scrollArea.m_maxScrollInertialY = 0f;
		m_scrollArea.m_maxScrollInertialX = 200f / (1024f / (float)Screen.width);
		m_scrollArea.RemoveDrawHandler();
		m_hList = new UIHorizontalList(m_scrollArea, string.Empty);
		m_hList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		m_hList.RemoveDrawHandler();
		m_hList.SetHorizontalAlign(0f);
		UICanvas uICanvas = new UICanvas(m_hList, false, string.Empty, null, string.Empty);
		uICanvas.SetWidth(0.2f, RelativeTo.ScreenWidth);
		uICanvas.RemoveDrawHandler();
		for (int i = 0; i < PsMetagameData.m_unlocks.Count; i++)
		{
			bool unlocked = PsMetagameData.m_unlocks[i].m_unlocked;
			for (int j = 0; j < PsMetagameData.m_unlocks[i].m_unlockables.Count; j++)
			{
				if (PsMetagameData.m_unlocks[i].m_unlockables[j].m_container.m_type != PsUnlockableType.Menu)
				{
					if (unlocked)
					{
						m_lastUnlockedCardIndex++;
					}
					CreateItemCard(m_hList, PsMetagameData.m_unlocks[i].m_unlockables[j], unlocked);
				}
			}
		}
		UICanvas uICanvas2 = new UICanvas(m_hList, false, string.Empty, null, string.Empty);
		uICanvas2.SetWidth(0.2f, RelativeTo.ScreenWidth);
		uICanvas2.RemoveDrawHandler();
		SetScrollPosition();
	}

	protected virtual void SetScrollPosition()
	{
		m_scrollArea.SetScrollPositionToChild(m_hList.m_childs[m_lastUnlockedCardIndex]);
	}

	protected virtual void SetHeader(UIComponent _parent)
	{
		UIText uIText = new UIText(_parent, false, "ResearchTitle", "Item Collection", "KGSecondChances_Font", 0.09f, RelativeTo.ScreenHeight, "#93fe36", "#5c3a0a");
		uIText.SetAlign(0.5f, 0.9f);
		uIText.SetShadowShift(new Vector2(0f, -1f), 0.035f);
	}

	protected virtual void CreateItemCard(UIComponent _parent, PsUnlockable _unlock, bool _isUnlocked = true)
	{
		PsUIItemCardButton psUIItemCardButton = new PsUIItemCardButton(_parent, _unlock);
		psUIItemCardButton.m_TAC.m_letTouchesThrough = true;
	}
}
