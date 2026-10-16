using System.Collections.Generic;
using Server;
using UnityEngine;

public class PsUICenterCreate : UICanvas
{
	protected UIScrollableCanvas m_scrollArea;

	protected UIVerticalList m_contentArea;

	protected PsUIGenericButton m_newButton;

	protected PsUIGenericButton m_claimButton;

	public List<PsGameLoopOwnPublished> m_published = new List<PsGameLoopOwnPublished>();

	public List<PsUICreatePublishedBanner> m_publishedBanners = new List<PsUICreatePublishedBanner>();

	public List<PsGameLoopEditor> m_saved = new List<PsGameLoopEditor>();

	public List<PsUICreateSavedBanner> m_savedBanners = new List<PsUICreateSavedBanner>();

	public List<PsGameLoopEditor> m_hiddenLoops = new List<PsGameLoopEditor>();

	public List<PsUICreateSavedBanner> m_hiddenBanners = new List<PsUICreateSavedBanner>();

	private int m_claimAllAmount;

	public PsUICenterCreate(UIComponent _parent)
		: base(_parent, false, "ScrollableContent", null, string.Empty)
	{
		GetRoot().SetDrawHandler(PsUIDrawHandlers.DebriefBackground);
		SetWidth(0.75f, RelativeTo.ScreenWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
		SetVerticalAlign(0f);
		UIHorizontalList uIHorizontalList = _parent as UIHorizontalList;
		uIHorizontalList.SetSpacing(0.25f, RelativeTo.ScreenWidth);
		uIHorizontalList.SetMargins(0.125f, 0.125f, 0f, 0f, RelativeTo.ScreenWidth);
		float num = 0.15f;
		RemoveDrawHandler();
		m_scrollArea = new UIScrollableCanvas(this, string.Empty);
		m_scrollArea.SetHeight(1f - num * 0.85f, RelativeTo.ScreenHeight);
		m_scrollArea.SetVerticalAlign(0f);
		m_scrollArea.RemoveDrawHandler();
		m_scrollArea.m_passTouchesToScrollableParents = true;
		m_contentArea = new UIVerticalList(m_scrollArea, string.Empty);
		m_contentArea.SetWidth(1f, RelativeTo.ParentWidth);
		m_contentArea.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		m_contentArea.SetMargins(0.025f, 0.025f, 0.05f, 0.05f);
		m_contentArea.SetVerticalAlign(1f);
		m_contentArea.RemoveDrawHandler();
		CreateContent(m_contentArea);
		UIScrollableCanvas uIScrollableCanvas = new UIScrollableCanvas(this, "Header");
		uIScrollableCanvas.SetWidth(1.3f, RelativeTo.ScreenHeight);
		uIScrollableCanvas.SetHeight(num, RelativeTo.ScreenHeight);
		uIScrollableCanvas.SetVerticalAlign(1f);
		uIScrollableCanvas.RemoveTouchAreas();
		uIScrollableCanvas.RemoveDrawHandler();
		UIText uIText = new UIText(uIScrollableCanvas, false, string.Empty, "Level\nBuilder", "KGSecondChances_Font", 0.06f, RelativeTo.ScreenHeight, "#FFFFFF", "#111111");
		uIText.SetAlign(0.15f, 0.1f);
		uIText.m_TC.transform.localRotation = Quaternion.Euler(Vector3.forward * 3f);
		uIText.m_shadowDistance = 0.05f;
	}

	public virtual void CreateContent(UIComponent _parent)
	{
		UIVerticalList uIVerticalList = new UIVerticalList(_parent, string.Empty);
		uIVerticalList.SetSpacing(-0.01f, RelativeTo.ScreenHeight);
		uIVerticalList.RemoveDrawHandler();
		UIFittedSprite uIFittedSprite = new UIFittedSprite(uIVerticalList, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_build_back"));
		uIFittedSprite.SetHeight(0.2f, RelativeTo.ScreenHeight);
		uIFittedSprite.SetDepthOffset(10f);
		m_newButton = new PsUIGenericButton(uIVerticalList);
		m_newButton.SetGreenColors();
		m_newButton.SetText("BUILD NEW!", 0.05f);
		m_newButton.SetMargins(0.06f, 0.06f, 0.02f, 0.02f);
		if (PsState.m_newEditorItemCount > 0)
		{
			UICanvas uICanvas = new UICanvas(m_newButton, false, "notification", null, string.Empty);
			uICanvas.SetSize(0.05f, 0.05f, RelativeTo.ScreenHeight);
			uICanvas.SetMargins(-0.065f, 0.065f, -0.03f, 0.03f);
			uICanvas.SetRogue();
			uICanvas.SetAlign(0f, 1f);
			uICanvas.SetDepthOffset(-10f);
			uICanvas.RemoveDrawHandler();
			UICanvas uICanvas2 = new UICanvas(uICanvas, false, string.Empty, null, string.Empty);
			uICanvas2.SetSize(1f, 1f, RelativeTo.ParentHeight);
			uICanvas2.SetMargins(0.15f, RelativeTo.OwnHeight);
			uICanvas2.SetDrawHandler(PsUIDrawHandlers.NotificationDrawhandler);
			TweenC c = TweenS.AddTransformTween(uICanvas2.m_TC, TweenedProperty.Scale, TweenStyle.CubicInOut, new Vector3(1.1f, 1.1f, 1.1f), 0.5f, 0f, false);
			TweenS.SetAdditionalTweenProperties(c, -1, true, TweenStyle.CubicInOut);
			UIFittedText uIFittedText = new UIFittedText(uICanvas2, false, string.Empty, "!", "KGSecondChances_Font", true);
		}
		LoadContent();
	}

	public void LoadContent()
	{
		new PsUILoadingAnimation(m_contentArea, false);
		HttpC own = MiniGame.GetOwn(DataSUCCEED, DataFAILED, 100);
		EntityManager.AddComponentToEntity((GetRoot() as PsUIBasePopup).m_utilityEntity, own);
	}

	private void DataSUCCEED(PsMinigameMetaData[] _data)
	{
		m_contentArea.DestroyChildren(1);
		m_claimAllAmount = 0;
		m_published.Clear();
		m_saved.Clear();
		m_hiddenLoops.Clear();
		for (int i = 0; i < _data.Length; i++)
		{
			if (_data[i].published)
			{
				PsGameLoopOwnPublished item = new PsGameLoopOwnPublished(_data[i].id, _data[i]);
				m_claimAllAmount += _data[i].rewardCoins;
				m_published.Add(item);
			}
			else if (!_data[i].published)
			{
				PsGameLoopEditor item2 = new PsGameLoopEditor(_data[i].id, _data[i]);
				m_saved.Add(item2);
			}
			else if (_data[i].hidden)
			{
				PsGameLoopEditor item3 = new PsGameLoopEditor(_data[i].id, _data[i]);
				m_hiddenLoops.Add(item3);
			}
		}
		if (m_saved.Count > 0)
		{
			UIVerticalList uIVerticalList = new UIVerticalList(m_contentArea, string.Empty);
			uIVerticalList.SetVerticalAlign(1f);
			uIVerticalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
			uIVerticalList.RemoveDrawHandler();
			UIText uIText = new UIText(uIVerticalList, false, string.Empty, "Unfinished Levels", "HurmeBold_Font", 0.04f, RelativeTo.ScreenHeight);
			for (int j = 0; j < m_saved.Count; j++)
			{
				m_savedBanners.Add(new PsUICreateSavedBanner(uIVerticalList, m_saved[j]));
			}
		}
		if (m_published.Count > 0)
		{
			UIVerticalList uIVerticalList2 = new UIVerticalList(m_contentArea, string.Empty);
			uIVerticalList2.SetVerticalAlign(1f);
			uIVerticalList2.SetSpacing(0.02f, RelativeTo.ScreenHeight);
			uIVerticalList2.RemoveDrawHandler();
			UIText uIText2 = new UIText(uIVerticalList2, false, string.Empty, "Finished Levels", "HurmeBold_Font", 0.04f, RelativeTo.ScreenHeight);
			if (m_published.Count > 1 && m_claimAllAmount > 0)
			{
				m_claimButton = new PsUIGenericButton(uIVerticalList2);
				m_claimButton.SetTealColors();
				m_claimButton.SetText("Claim All " + m_claimAllAmount, 0.04f);
				m_claimButton.SetIcon("menu_resources_coin_icon", 0.065f);
			}
			for (int k = 0; k < m_published.Count; k++)
			{
				m_publishedBanners.Add(new PsUICreatePublishedBanner(uIVerticalList2, m_published[k]));
			}
		}
		if (PsState.m_adminMode && m_hiddenLoops.Count > 0)
		{
			UIVerticalList uIVerticalList3 = new UIVerticalList(m_contentArea, string.Empty);
			uIVerticalList3.SetVerticalAlign(1f);
			uIVerticalList3.SetSpacing(0.02f, RelativeTo.ScreenHeight);
			uIVerticalList3.RemoveDrawHandler();
			UIText uIText3 = new UIText(uIVerticalList3, false, string.Empty, "Hidden Levels", "HurmeBold_Font", 0.04f, RelativeTo.ScreenHeight);
			for (int l = 0; l < m_hiddenLoops.Count; l++)
			{
				m_hiddenBanners.Add(new PsUICreateSavedBanner(uIVerticalList3, m_hiddenLoops[l]));
			}
		}
		if (m_saved.Count == 0 && m_published.Count == 0 && m_hiddenLoops.Count == 0)
		{
			new UIText(m_contentArea, false, string.Empty, "Create new levels here!", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenHeight);
		}
		Update();
	}

	private void DataFAILED(HttpC _c)
	{
		Debug.Log("GET PUBLISHED FAILED");
		string networkError = ServerErrors.GetNetworkError(_c.www.error);
		ServerManager.ThrowServerErrorException("Connection error", networkError, delegate
		{
			HttpC own = MiniGame.GetOwn(DataSUCCEED, DataFAILED);
			EntityManager.AddComponentToEntity((GetRoot() as PsUIBasePopup).m_utilityEntity, own);
			return own;
		});
	}

	public override void Step()
	{
		if (m_newButton != null && m_newButton.m_hit)
		{
			PsMetagameManager.HideResources();
			PsGameLoopEditor psGameLoopEditor = new PsGameLoopEditor(null, null);
			psGameLoopEditor.StartLoop();
		}
		else if (m_claimButton != null && m_claimButton.m_hit && m_claimAllAmount > 0)
		{
			Debug.Log("claim all");
			for (int i = 0; i < m_publishedBanners.Count; i++)
			{
				if (!m_publishedBanners[i].m_claimed)
				{
					m_publishedBanners[i].m_claimed = true;
					m_publishedBanners[i].SwitchToPlayButton();
					m_publishedBanners[i].m_rewardAmount = 0;
				}
			}
			PsMetagameManager.ClaimAllMinigames(m_claimAllAmount, (Vector2)m_camera.WorldToScreenPoint(m_claimButton.m_TC.transform.position) - new Vector2(Screen.width, Screen.height) * 0.5f);
			m_claimAllAmount = 0;
			m_claimButton.m_UItext.SetText("Claim All: " + m_claimAllAmount);
		}
		int num = 0;
		for (int j = 0; j < m_publishedBanners.Count; j++)
		{
			if (m_publishedBanners[j].m_button.m_hit)
			{
				m_publishedBanners[j].ButtonClicked();
			}
			if (m_publishedBanners[j].m_claimed && m_publishedBanners[j].m_rewardAmount > 0)
			{
				num += m_publishedBanners[j].m_rewardAmount;
				m_publishedBanners[j].m_rewardAmount = 0;
			}
		}
		if (num > 0)
		{
			m_claimAllAmount -= num;
			if (m_claimButton != null)
			{
				m_claimButton.m_UItext.SetText("Claim All: " + m_claimAllAmount);
			}
		}
		for (int k = 0; k < m_savedBanners.Count; k++)
		{
			if (m_savedBanners[k].m_bannerEndIcon.m_hit)
			{
				PsMetagameManager.DeleteMinigame(m_savedBanners[k].m_loop.m_minigameId);
				m_savedBanners[k].Destroy();
				m_savedBanners.RemoveAt(k);
				m_scrollArea.Update();
				break;
			}
			if (m_savedBanners[k].m_button.m_hit)
			{
				PsMetagameManager.HideResources();
				m_savedBanners[k].m_loop.StartLoop();
			}
		}
		base.Step();
	}
}
