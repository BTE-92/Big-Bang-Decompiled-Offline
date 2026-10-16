using System.Collections.Generic;
using Server;
using UnityEngine;

public class PsUISideCreatePublished : UICanvas
{
	protected UIScrollableCanvas m_scrollArea;

	protected UIVerticalList m_contentArea;

	public List<PsUICreatePublishedBanner> m_levels = new List<PsUICreatePublishedBanner>();

	private PsUIGenericButton m_fbButton;

	private PsUIBasePopup m_waitingPopup;

	private bool m_contentLoaded;

	public PsUISideCreatePublished(UIComponent _parent)
		: base(_parent, false, "ScrollableContent", null, string.Empty)
	{
		GetRoot().SetDrawHandler(PsUIDrawHandlers.DebriefBackground);
		m_contentLoaded = false;
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
		UIText uIText = new UIText(uIScrollableCanvas, false, string.Empty, "Published\nLevels", "KGSecondChances_Font", 0.06f, RelativeTo.ScreenHeight, "#FFFFFF", "#111111");
		uIText.SetAlign(0.15f, 0.1f);
		uIText.m_TC.transform.localRotation = Quaternion.Euler(Vector3.forward * 3f);
		uIText.m_shadowDistance = 0.05f;
	}

	public virtual void CreateContent(UIComponent _parent)
	{
	}

	public void LoadContent()
	{
		new PsUILoadingAnimation(m_contentArea, false);
		m_contentArea.Update();
		HttpC ownPublished = MiniGame.GetOwnPublished(DataSUCCEED, DataFAILED, 100);
		EntityManager.AddComponentToEntity((GetRoot() as PsUIBasePopup).m_utilityEntity, ownPublished);
	}

	private void DataSUCCEED(PsMinigameMetaData[] _data)
	{
		m_contentArea.DestroyChildren();
		for (int i = 0; i < _data.Length; i++)
		{
			PsGameLoopOwnPublished gameLoop = new PsGameLoopOwnPublished(_data[i].id, _data[i]);
			m_levels.Add(new PsUICreatePublishedBanner(m_contentArea, gameLoop));
		}
		Update();
	}

	private void DataFAILED(HttpC _c)
	{
		Debug.Log("GET PUBLISHED FAILED");
		string networkError = ServerErrors.GetNetworkError(_c.www.error);
		ServerManager.ThrowServerErrorException("Connection error", networkError, delegate
		{
			HttpC ownPublished = MiniGame.GetOwnPublished(DataSUCCEED, DataFAILED);
			EntityManager.AddComponentToEntity((GetRoot() as PsUIBasePopup).m_utilityEntity, ownPublished);
			return ownPublished;
		});
	}

	public override void Step()
	{
		PsUIBasePopup psUIBasePopup = GetRoot() as PsUIBasePopup;
		float currentScrollX = psUIBasePopup.m_scrollableCanvas.m_currentScrollX;
		if (psUIBasePopup.m_pageButtonLeft == null && psUIBasePopup.m_pageButtonRight == null && currentScrollX < 0.1f)
		{
			PsMetagameManager.ShowResources((GetRoot() as PsUIBasePopup).m_overlayCamera, true, true, false, false, false);
			psUIBasePopup.CreatePageButtonRight("Challenges");
		}
		if (!m_contentLoaded && Mathf.Abs((GetRoot() as PsUIBasePopup).m_scrollableCanvas.m_currentScrollX) < 0.1f)
		{
			LoadContent();
			m_contentLoaded = true;
		}
		for (int i = 0; i < m_levels.Count; i++)
		{
			if (m_levels[i].m_button.m_hit)
			{
				m_levels[i].ButtonClicked();
				break;
			}
		}
		base.Step();
	}
}
