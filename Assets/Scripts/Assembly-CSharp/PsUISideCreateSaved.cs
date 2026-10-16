using System.Collections.Generic;
using Server;
using UnityEngine;

public class PsUISideCreateSaved : PsUISideCreateAdmin
{
	public List<PsUICreateLevelBanner> m_levels = new List<PsUICreateLevelBanner>();

	public PsUISideCreateSaved(UIComponent _parent)
		: base(_parent)
	{
		m_createBackButton = false;
	}

	public override void LoadContent(string _content)
	{
		m_scrollArea.DestroyChildren();
		new PsUILoadingAnimation(m_scrollArea, false);
		m_scrollArea.Update();
		HttpC ownSaved = MiniGame.GetOwnSaved(DataSUCCEED, base.DataFAILED, 100);
		EntityManager.AddComponentToEntity((GetRoot() as PsUIBasePopup).m_utilityEntity, ownSaved);
	}

	public override void CreateHeaderContent(UIComponent _parent)
	{
		UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.025f, 0.025f, 0f, 0f);
		uIHorizontalList.SetHorizontalAlign(0.5f);
		UIText uIText = new UIText(uIHorizontalList, false, string.Empty, "SAVED TRACKS", "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
	}

	public override void Step()
	{
		if (!m_contentLoaded && Mathf.Abs((GetRoot() as PsUIBasePopup).m_scrollableCanvas.m_currentScrollX) < 0.1f)
		{
			LoadContent(string.Empty);
			m_contentLoaded = true;
		}
		for (int i = 0; i < m_levels.Count; i++)
		{
			if (m_levels[i].m_bannerEndIcon.m_hit)
			{
				PsMetagameManager.DeleteMinigame(m_levels[i].m_loop.m_minigameId);
				m_levels[i].Destroy();
				m_levels.RemoveAt(i);
				m_scrollArea.Update();
				break;
			}
			if (m_levels[i].m_button.m_hit)
			{
				PsMetagameManager.HideResources();
				m_levels[i].m_loop.StartLoop();
			}
		}
		base.Step();
	}

	protected override void DataSUCCEED(PsMinigameMetaData[] _data)
	{
		m_scrollArea.DestroyChildren();
		UIVerticalList uIVerticalList = new UIVerticalList(m_scrollArea, string.Empty);
		uIVerticalList.SetVerticalAlign(1f);
		uIVerticalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		uIVerticalList.SetMargins(0f, 0f, 0.02f, 0f);
		uIVerticalList.RemoveDrawHandler();
		if (m_createBackButton)
		{
			m_back = new PsUIGenericButton(uIVerticalList);
			m_back.SetText("Back");
			m_back.SetHorizontalAlign(0f);
		}
		for (int i = 0; i < _data.Length; i++)
		{
			PsGameLoopEditor gameLoop = new PsGameLoopEditor(_data[i].id, _data[i]);
			m_levels.Add(new PsUICreateSavedBanner(uIVerticalList, gameLoop));
		}
		Update();
	}

	public override void CreateContent(UIComponent _parent)
	{
	}
}
