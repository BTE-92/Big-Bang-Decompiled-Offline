using System;
using System.Collections.Generic;
using UnityEngine;

public class PsUITopPlayRace : UICanvas, IPlayMenu
{
	protected PsUIGenericButton m_restartButton;

	protected PsUIGenericButton m_pauseButton;

	private Action m_restartAction;

	private Action m_pauseAction;

	private UIVerticalList m_leftArea;

	private int m_starCount;

	private UIFittedSprite m_stars;

	private UIText m_timer;

	private UIText m_popTimer;

	private PsGameModeRace m_race;

	private string m_frameName;

	private List<UIStarInflate> m_starList;

	private float[] m_medalTimes;

	public PsUITopPlayRace(Action _restartAction = null, Action _pauseAction = null)
		: base(null, false, "TopContent", null, string.Empty)
	{
		m_restartAction = _restartAction;
		m_pauseAction = _pauseAction;
		m_race = PsState.m_activeGameLoop.m_gameMode as PsGameModeRace;
		int ticks = Mathf.RoundToInt(PsState.m_activeMinigame.m_gameTicks);
		float num = HighScores.TicksToTime(ticks);
		m_medalTimes = new float[3] { m_race.m_oneMedalTime, m_race.m_twoMedalTime, m_race.m_threeMedalTime };
		Debug.Log(num + " " + m_race.m_threeMedalTime + " " + m_race.m_twoMedalTime + " " + m_race.m_oneMedalTime);
		if (num < m_race.m_threeMedalTime)
		{
			m_starCount = 3;
		}
		else if (num < m_race.m_twoMedalTime)
		{
			m_starCount = 2;
		}
		else if (num < m_race.m_oneMedalTime)
		{
			m_starCount = 1;
		}
		else
		{
			m_starCount = 0;
		}
		RemoveDrawHandler();
		m_leftArea = new UIVerticalList(this, "UpperLeft");
		m_leftArea.SetMargins(0.01f, RelativeTo.ScreenShortest);
		m_leftArea.SetMargins(-0.01f, 0.025f, 0.01f, 0.025f, RelativeTo.ScreenShortest);
		m_leftArea.SetSpacing(0.01f, RelativeTo.ScreenShortest);
		m_leftArea.SetAlign(0f, 1f);
		m_leftArea.RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(m_leftArea, "hList");
		uIHorizontalList.SetSpacing(0.425f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetDrawHandler(PsUIDrawHandlers.ResourceLabelBackground);
		uIHorizontalList.SetHeight(0.12f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.03f, -0.055f, 0f, 0f);
		UIComponent uIComponent = new UIComponent(uIHorizontalList, false, string.Empty, null, null, string.Empty);
		uIComponent.SetSize(0f, 0f, RelativeTo.ScreenHeight);
		uIComponent.SetMargins(0.275f, 0f, 0f, 0f, RelativeTo.ScreenShortest);
		uIComponent.RemoveDrawHandler();
		UICanvas uICanvas = new UICanvas(uIComponent, false, "TrophyCanvas", null, string.Empty);
		uICanvas.SetHeight(0.15f, RelativeTo.ScreenHeight);
		uICanvas.SetWidth(0.3f, RelativeTo.ScreenHeight);
		uICanvas.RemoveDrawHandler();
		m_stars = new UIFittedSprite(uICanvas, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_mode_star_0"));
		m_timer = new UIText(uIHorizontalList, false, string.Empty, HighScores.TicksToTimeString(Mathf.RoundToInt(PsState.m_activeMinigame.m_gameTicks)), "HurmeSemiBoldMN_Font", 0.05f, RelativeTo.ScreenHeight);
		m_timer.m_tmc.m_horizontalAlign = Align.Right;
		m_timer.SetVerticalAlign(0.35f);
		CreateCoinArea();
		UIHorizontalList uIHorizontalList2 = new UIHorizontalList(this, "UpperLeft");
		uIHorizontalList2.SetMargins(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList2.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList2.SetAlign(1f, 1f);
		uIHorizontalList2.RemoveDrawHandler();
		CreateRestartArea(uIHorizontalList2);
		Update();
		RelativeTo relativeTo = RelativeTo.ScreenHeight;
		float num2 = 0.16f * (float)Screen.height;
		float num3 = 0f * m_stars.m_actualHeight;
		m_starList = new List<UIStarInflate>();
		for (int i = 0; i < m_starCount; i++)
		{
			float num4 = m_stars.m_actualWidth * 0.296f;
			float num5 = m_stars.m_actualWidth * 0.292f;
			float num6 = m_stars.m_actualHeight * 0.0155f;
			Vector3 pos = m_stars.m_TC.transform.position + new Vector3(2f, 1f, 0f) + new Vector3((float)i * num4 - num5, (float)i * num6, (float)i * -1f - 1f);
			TransformC parentTC = TransformS.AddComponent(m_TC.p_entity, "InflatingStar", pos);
			PrefabC prefabC = PrefabS.AddComponent(parentTC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.UIStarInflatePrefab_GameObject));
			prefabC.p_gameObject.transform.Rotate(0f, 180f, 0f);
			prefabC.p_gameObject.transform.localScale = new Vector3(num2 / 100f, num2 / 100f, 1f);
			PrefabS.SetCamera(prefabC, m_camera);
			m_starList.Add((UIStarInflate)prefabC.p_gameObject.GetComponent(typeof(UIStarInflate)));
		}
		Step();
	}

	public virtual void CreateRestartArea(UIComponent _parent)
	{
		m_restartButton = new PsUIGenericButton(_parent);
		m_restartButton.SetIcon("hud_icon_restart", 0.06f);
		m_restartButton.SetOrangeColors();
		m_pauseButton = new PsUIGenericButton(_parent);
		m_pauseButton.SetIcon("hud_icon_pause", 0.06f);
		m_pauseButton.SetOrangeColors();
	}

	public virtual void CreateCoinArea()
	{
		PsMetagameManager.ShowResources(m_camera, false, true, false, false, false, 0.15f);
	}

	public override void Step()
	{
		int ticks = Mathf.RoundToInt(PsState.m_activeMinigame.m_gameTicks);
		float num = HighScores.TicksToTime(ticks);
		float num2 = 5f;
		for (int i = 0; i < m_starList.Count; i++)
		{
			if (num > m_medalTimes[i] - num2 && num < m_medalTimes[i])
			{
				float inflationAmount = (num - (m_medalTimes[i] - num2)) / num2;
				m_starList[i].SetInflationAmount(inflationAmount);
				if (i == m_starList.Count - 1)
				{
					if (m_popTimer == null)
					{
						float fontSize = 0.05f;
						m_popTimer = new UIText(this, false, "popTimer", Mathf.CeilToInt(m_medalTimes[i] - num).ToString(), "HurmeSemiBoldMN_Font", fontSize, RelativeTo.ScreenHeight);
						Vector3 position = m_starList[i].transform.position;
						m_popTimer.SetDepthOffset(-25f);
						m_popTimer.Update();
						m_popTimer.m_TC.transform.position = new Vector3(position.x, position.y - m_popTimer.m_actualHeight / 6f, m_popTimer.m_TC.transform.position.z);
					}
					else
					{
						m_popTimer.SetText(Mathf.CeilToInt(m_medalTimes[i] - num).ToString());
					}
				}
			}
			else if (num > m_medalTimes[i] && !m_starList[i].exploded)
			{
				m_starList[i].SetInflationAmount(1f);
				if (i > 0 && num > m_medalTimes[i - 1] - num2)
				{
					m_popTimer.SetText(Mathf.CeilToInt(m_medalTimes[i - 1] - num).ToString());
					Vector3 position2 = m_starList[i - 1].transform.position;
					float num3 = -0.05f;
					float num4 = 0.1f;
					m_popTimer.m_TC.transform.position = new Vector3(position2.x, position2.y - m_popTimer.m_actualHeight / 6f, m_popTimer.m_TC.transform.position.z);
				}
				else
				{
					m_popTimer.Destroy();
					m_popTimer = null;
				}
				m_starList.RemoveAt(i);
			}
		}
		m_timer.SetText(HighScores.TicksToTimeString(ticks));
		if (m_restartButton.m_hit)
		{
			if (m_restartAction != null)
			{
				m_restartAction();
			}
		}
		else if (m_pauseButton.m_hit && m_pauseAction != null)
		{
			m_pauseAction();
		}
		base.Step();
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public void ApplyLeftySettings()
	{
	}
}
