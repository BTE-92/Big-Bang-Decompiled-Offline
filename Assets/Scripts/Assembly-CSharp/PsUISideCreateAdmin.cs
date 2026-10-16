using System.Collections.Generic;
using Server;
using UnityEngine;

public class PsUISideCreateAdmin : PsUIScrollableBase
{
	protected bool m_createBackButton;

	protected bool m_contentLoaded;

	private string m_contentIdentifier;

	protected PsUIGenericButton m_saved;

	protected PsUIGenericButton m_hiddenButton;

	protected PsUIGenericButton m_published;

	protected PsUIGenericButton m_friends;

	protected PsUIGenericButton m_templates;

	protected PsUIGenericButton m_fixlist;

	protected PsUIGenericButton m_fixlist2;

	protected PsUIGenericButton m_back;

	private List<PsUICreateLevelBanner> m_levels = new List<PsUICreateLevelBanner>();

	public PsUISideCreateAdmin(UIComponent _parent)
		: base(_parent)
	{
		PsMetagameManager.ShowResources(null, true, true, true, false, false);
		m_scrollArea.m_maxScrollInertialX = 0f;
		m_scrollArea.m_maxScrollInertialY = 50f / (1024f / (float)Screen.width);
		m_scrollArea.SetHeight(1f, RelativeTo.ParentHeight);
		m_scrollArea.m_TAC.m_letTouchesThrough = true;
		m_createBackButton = true;
	}

	public virtual void LoadContent(string _content)
	{
		m_scrollArea.DestroyChildren();
		new PsUILoadingAnimation(m_scrollArea, false);
		m_scrollArea.Update();
		m_contentIdentifier = _content;
		HttpC httpC = null;
		switch (_content)
		{
		case "Saved":
			httpC = MiniGame.GetOwnSaved(DataSUCCEED, DataFAILED, 100);
			break;
		case "Hidden":
			httpC = MiniGame.GetHidden(DataSUCCEED, DataFAILED, null, 500);
			break;
		case "Published":
			httpC = MiniGame.GetOwnPublished(DataSUCCEED, DataFAILED, 100);
			break;
		case "Friends":
			httpC = MiniGame.GetFolloweeMinigames(DataSUCCEED, DataFAILED);
			break;
		case "Templates":
			httpC = Template.GetAll(TemplateDataSUCCEED, DataFAILED, null, 100);
			break;
		case "MaterialDanger, PsMetalPlank":
			httpC = MiniGame.GetWithItems("MaterialDanger,PsMetalPlank", DataSUCCEED, DataFAILED);
			break;
		case "MaterialDanger, PsStoneBlock":
			httpC = MiniGame.GetWithItems("MaterialDanger,PsStoneBlock", DataSUCCEED, DataFAILED);
			break;
		}
		if (httpC != null)
		{
			EntityManager.AddComponentToEntity((GetRoot() as PsUIBasePopup).m_utilityEntity, httpC);
		}
	}

	protected virtual void TemplateDataSUCCEED(PsMinigameMetaData[] _data)
	{
		for (int i = 0; i < _data.Length; i++)
		{
			_data[i].template = true;
		}
		DataSUCCEED(_data);
	}

	protected virtual void DataSUCCEED(PsMinigameMetaData[] _data)
	{
		m_scrollArea.DestroyChildren();
		UIVerticalList uIVerticalList = new UIVerticalList(m_scrollArea, string.Empty);
		uIVerticalList.SetVerticalAlign(1f);
		uIVerticalList.RemoveDrawHandler();
		if (m_createBackButton)
		{
			m_back = new PsUIGenericButton(uIVerticalList);
			m_back.SetText("Back");
			m_back.SetHorizontalAlign(0f);
		}
		for (int i = 0; i < _data.Length; i++)
		{
			if (m_contentIdentifier == "MaterialDanger, PsMetalPlank" || m_contentIdentifier == "MaterialDanger, PsStoneBlock")
			{
				PsGameLoopEditor gameLoop = new PsGameLoopEditor(_data[i].id, _data[i], false);
				m_levels.Add(new PsUICreateSavedBanner(uIVerticalList, gameLoop));
			}
			else if (m_contentIdentifier == "Templates" || m_contentIdentifier == "Hidden")
			{
				PsGameLoopEditor gameLoop2 = new PsGameLoopEditor(_data[i].id, _data[i]);
				m_levels.Add(new PsUICreateSavedBanner(uIVerticalList, gameLoop2));
			}
			else if (m_contentIdentifier == "Saved")
			{
				PsGameLoopEditor gameLoop3 = new PsGameLoopEditor(_data[i].id, _data[i]);
				m_levels.Add(new PsUICreateSavedBanner(uIVerticalList, gameLoop3));
			}
			else if (m_contentIdentifier == "Published")
			{
				PsGameLoopOwnPublished gameLoop4 = new PsGameLoopOwnPublished(_data[i].id, _data[i]);
				m_levels.Add(new PsUICreatePublishedBanner(uIVerticalList, gameLoop4));
			}
			else
			{
				PsGameLoopSocial gameLoop5 = new PsGameLoopSocial(_data[i].id, _data[i]);
				m_levels.Add(new PsUICreateFolloweeBanner(uIVerticalList, gameLoop5));
			}
		}
		m_scrollArea.Update();
	}

	protected void DataFAILED(HttpC _httpc)
	{
	}

	public override void CreateContent(UIComponent _parent)
	{
		m_scrollArea.DestroyChildren();
		UIVerticalList uIVerticalList = new UIVerticalList(_parent, string.Empty);
		uIVerticalList.RemoveDrawHandler();
		m_saved = new PsUIGenericButton(uIVerticalList);
		m_saved.SetText("Saved");
		m_hiddenButton = new PsUIGenericButton(uIVerticalList);
		m_hiddenButton.SetText("Hidden");
		m_published = new PsUIGenericButton(uIVerticalList);
		m_published.SetText("Published");
		m_friends = new PsUIGenericButton(uIVerticalList);
		m_friends.SetText("Friend Levels");
		m_templates = new PsUIGenericButton(uIVerticalList);
		m_templates.SetText("Level Templates");
		m_fixlist = new PsUIGenericButton(uIVerticalList);
		m_fixlist.SetText("MetalPlank & MaterialDanger");
		m_fixlist2 = new PsUIGenericButton(uIVerticalList);
		m_fixlist2.SetText("PsStoneBlock & MaterialDanger");
	}

	public override void Step()
	{
		if (m_saved != null && m_saved.m_hit)
		{
			LoadContent("Saved");
			return;
		}
		if (m_hiddenButton != null && m_hiddenButton.m_hit)
		{
			LoadContent("Hidden");
			return;
		}
		if (m_published != null && m_published.m_hit)
		{
			LoadContent("Published");
			return;
		}
		if (m_friends != null && m_friends.m_hit)
		{
			LoadContent("Friends");
			return;
		}
		if (m_templates != null && m_templates.m_hit)
		{
			LoadContent("Templates");
			return;
		}
		if (m_fixlist != null && m_fixlist.m_hit)
		{
			LoadContent("MaterialDanger, PsMetalPlank");
			return;
		}
		if (m_fixlist2 != null && m_fixlist2.m_hit)
		{
			LoadContent("MaterialDanger, PsStoneBlock");
			return;
		}
		for (int i = 0; i < m_levels.Count; i++)
		{
			if (m_levels[i].m_button.m_hit)
			{
				PsMetagameManager.HideResources();
				m_levels[i].m_loop.StartLoop();
			}
			else if (m_levels[i].m_bannerEndIcon.m_hit)
			{
				PsMetagameManager.DeleteMinigame(m_levels[i].m_loop.m_minigameId);
				m_levels[i].Destroy();
				m_levels.RemoveAt(i);
				m_scrollArea.Update();
				break;
			}
		}
		if (m_back != null && m_back.m_hit)
		{
			m_back = null;
			CreateContent(m_scrollArea);
			m_scrollArea.Update();
		}
		else
		{
			base.Step();
		}
	}

	protected void DeleteSUCCEED(HttpC _httpc)
	{
		Debug.LogWarning("Delete succeed");
	}

	protected void DeleteFAILED(HttpC _httpc)
	{
		Debug.LogWarning("Delete failed");
	}

	protected void ClaimSUCCEED(HttpC _httpc)
	{
		Debug.LogWarning("Claim succeed");
	}

	protected void ClaimFAILED(HttpC _httpc)
	{
		Debug.LogWarning("Claim failed");
	}
}
