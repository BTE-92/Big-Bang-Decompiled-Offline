using System.Collections.Generic;
using Server;

public class PsUICenterPlayerProfiler : UICanvas
{
	private const string m_font = "KGSecondChances_Font";

	private const float m_smallTestSize = 0.03f;

	public string m_userName = string.Empty;

	private UIVerticalList m_vertList;

	public Entity m_loaderEntity;

	public PsUICenterPlayerProfiler(UIComponent _parent)
		: base(_parent, false, string.Empty, null, string.Empty)
	{
		m_loaderEntity = EntityManager.AddEntity();
		m_userName = PlayerPrefsX.GetUserName();
		HttpC playerInfo = Player.GetPlayerInfo(InfoFetchSUCCEED, InfoFetchFAILED);
		EntityManager.AddComponentToEntity(m_loaderEntity, playerInfo);
		SetWidth(1f, RelativeTo.ScreenWidth);
		SetHeight(1f, RelativeTo.ScreenHeight);
		RemoveDrawHandler();
		m_vertList = new UIVerticalList(this, string.Empty);
		m_vertList.SetWidth(0.85f, RelativeTo.ScreenHeight);
		m_vertList.SetAlign(0.5f, 0.5f);
		m_vertList.SetMargins(0.03f, 0.03f, 0f, 0.02f);
		m_vertList.SetDrawHandler(PsUIDrawHandlers.UpgradeBarsBackground);
		m_vertList.SetSpacing(0.04f, RelativeTo.ScreenHeight);
		UIHorizontalList uIHorizontalList = new UIHorizontalList(m_vertList, string.Empty);
		uIHorizontalList.SetHeight(0.08f, RelativeTo.ScreenHeight);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		UIText uIText = new UIText(uIHorizontalList, false, string.Empty, m_userName + "'s server profile\nuserid: " + PlayerPrefsX.GetUserId(), "KGSecondChances_Font", 0.025f, RelativeTo.ScreenHeight);
		uIText.SetHeight(0.04f, RelativeTo.ScreenHeight);
		uIText.RemoveDrawHandler();
	}

	private void InfoFetchSUCCEED(HttpC _c)
	{
		PsPlayerProfilerData psPlayerProfilerData = ClientTools.ParsePlayerProfilerData(_c);
		UIVerticalList uIVerticalList = new UIVerticalList(m_vertList, string.Empty);
		uIVerticalList.RemoveDrawHandler();
		UIText uIText = new UIText(uIVerticalList, false, string.Empty, "[Vehicle profiles]\n", "KGSecondChances_Font", 0.025f, RelativeTo.ScreenHeight);
		uIText.SetHeight(0.03f, RelativeTo.ScreenHeight);
		uIText.RemoveDrawHandler();
		foreach (PsPlayerProfilerData.ProfilerVehicleData skill in psPlayerProfilerData.m_skills)
		{
			UIText uIText2 = new UIText(uIVerticalList, false, string.Empty, skill.m_vehicleName + " skill: " + skill.m_skill + " / pref: " + skill.m_preference, "KGSecondChances_Font", 0.025f, RelativeTo.ScreenHeight);
			uIText2.SetHeight(0.02f, RelativeTo.ScreenHeight);
			uIText2.RemoveDrawHandler();
		}
		if (psPlayerProfilerData.m_subgenrePrefs.Count > 0)
		{
			uIText = new UIText(uIVerticalList, false, string.Empty, "\n[Subgenres]\n", "KGSecondChances_Font", 0.025f, RelativeTo.ScreenHeight);
			uIText.SetHeight(0.03f, RelativeTo.ScreenHeight);
			uIText.RemoveDrawHandler();
			foreach (KeyValuePair<string, float> subgenrePref in psPlayerProfilerData.m_subgenrePrefs)
			{
				UIText uIText3 = new UIText(uIVerticalList, false, string.Empty, subgenrePref.Key + ": " + subgenrePref.Value, "KGSecondChances_Font", 0.025f, RelativeTo.ScreenHeight);
				uIText3.SetHeight(0.02f, RelativeTo.ScreenHeight);
				uIText3.RemoveDrawHandler();
			}
		}
		Update();
	}

	private void InfoFetchFAILED(HttpC _c)
	{
		Debug.Log("Info fetch failed!");
		Debug.LogError(ServerErrors.GetNetworkError(_c.www.error));
	}

	public override void Destroy()
	{
		if (m_loaderEntity != null)
		{
			EntityManager.RemoveEntity(m_loaderEntity);
		}
		m_loaderEntity = null;
		base.Destroy();
	}

	public override void Step()
	{
	}
}
