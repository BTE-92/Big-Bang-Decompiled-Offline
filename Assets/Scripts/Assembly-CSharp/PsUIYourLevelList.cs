using System.Collections.Generic;
using Server;

public class PsUIYourLevelList : UIVerticalList
{
	private bool m_created;

	private List<PsUICreatePublishedBanner> m_levels;

	public PsUIYourLevelList(UIComponent _parent)
		: base(_parent, string.Empty)
	{
		m_levels = new List<PsUICreatePublishedBanner>();
		SetWidth(1f, RelativeTo.ParentWidth);
		SetSpacing(0.02f, RelativeTo.ScreenHeight);
		SetMargins(0.025f, 0.025f, 0.1125f, 0.05f);
		SetVerticalAlign(1f);
		RemoveDrawHandler();
		LoadContent();
	}

	public void LoadContent()
	{
		new PsUILoadingAnimation(this, false);
		m_parent.Update();
		HttpC ownPublished = MiniGame.GetOwnPublished(DataSUCCEED, DataFAILED, 100);
		EntityManager.AddComponentToEntity(m_TC.p_entity, ownPublished);
	}

	private void DataSUCCEED(PsMinigameMetaData[] _data)
	{
		DestroyChildren();
		m_levels = new List<PsUICreatePublishedBanner>();
		for (int i = 0; i < _data.Length; i++)
		{
			PsGameLoopOwnPublished gameLoop = new PsGameLoopOwnPublished(_data[i].id, _data[i]);
			m_levels.Add(new PsUICreatePublishedBanner(this, gameLoop));
		}
		if (m_levels.Count == 0)
		{
			UITextbox uITextbox = new UITextbox(this, false, string.Empty, "Find and create new levels from planet path.", "HurmeSemiBold_Font", 0.04f, RelativeTo.ScreenHeight, false, Align.Center);
			uITextbox.SetWidth(1f, RelativeTo.ParentWidth);
			uITextbox.SetHeight(1f, RelativeTo.ParentHeight);
		}
		m_parent.Update();
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
