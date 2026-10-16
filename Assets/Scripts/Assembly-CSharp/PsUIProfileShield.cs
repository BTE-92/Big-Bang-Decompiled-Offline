using System;
using System.Collections.Generic;
using UnityEngine;

public class PsUIProfileShield : UICanvas
{
	private UIFittedSprite m_backgroundShield;

	private List<UIFittedSprite> m_trophyList = new List<UIFittedSprite>();

	private UICanvas m_rankSprite;

	private UIText m_rankText;

	private Frame m_trophyDisabled = PsState.m_uiSheet.m_atlas.GetFrame("challenge_shield_trophy_disabled");

	private Frame m_trophyEnabled = PsState.m_uiSheet.m_atlas.GetFrame("challenge_shield_trophy_enabled");

	public int m_rank = PsMetagameManager.m_playerStats.rank;

	public int m_cupAmount = PsMetagameManager.m_playerStats.cups;

	public bool m_showingRank;

	public PsUIProfileShield(UIComponent _parent, bool _touchable, bool _showRank = true, int _rank = -1, int _cups = -1)
		: base(_parent, _touchable, "ProfileShieldCanvas", null, string.Empty)
	{
		RemoveDrawHandler();
		if (_rank != -1)
		{
			m_rank = _rank;
		}
		if (_cups != -1)
		{
			m_cupAmount = _cups;
		}
		m_backgroundShield = new UIFittedSprite(this, false, "ShieldImage", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("submenu_versus_shield"));
		PsUIProfileImage psUIProfileImage = new PsUIProfileImage(m_backgroundShield, false, "UserImage", PlayerPrefsX.GetFacebookId(), PlayerPrefsX.GetGameCenterId(), -1, true, false, true, 0.085f, 0.0085f, "feffc4");
		psUIProfileImage.SetSize(0.4f, 0.4f, RelativeTo.ParentWidth);
		psUIProfileImage.SetVerticalAlign(0.9f);
		UIText uIText = new UIText(m_backgroundShield, false, "UserName", PlayerPrefsX.GetUserName(), "HurmeBold_Font", 0.125f, RelativeTo.ParentWidth, "#feffc4", "#000000");
		uIText.SetShadowShift(new Vector2(0.4f, -0.4f));
		uIText.SetVerticalAlign(0.4f);
		if (_showRank)
		{
			ShowRank();
			ShowCups();
		}
	}

	public void ShowCups()
	{
		Frame frame2;
		Frame frame3;
		Frame frame = (frame2 = (frame3 = m_trophyDisabled));
		if (m_cupAmount >= 3)
		{
			frame = (frame2 = (frame3 = m_trophyEnabled));
		}
		else if (m_cupAmount == 2)
		{
			frame = (frame2 = m_trophyEnabled);
		}
		else if (m_cupAmount == 1)
		{
			frame = m_trophyEnabled;
		}
		UIFittedSprite uIFittedSprite = new UIFittedSprite(m_backgroundShield, false, "Trophy1", PsState.m_uiSheet, frame);
		uIFittedSprite.SetHeight(0.25f, RelativeTo.ParentHeight);
		uIFittedSprite.SetAlign(0.18f, 0.075f);
		m_trophyList.Add(uIFittedSprite);
		UIFittedSprite uIFittedSprite2 = new UIFittedSprite(m_backgroundShield, false, "Trophy2", PsState.m_uiSheet, frame2);
		uIFittedSprite2.SetHeight(0.25f, RelativeTo.ParentHeight);
		uIFittedSprite2.SetAlign(0.5f, 0.115f);
		m_trophyList.Add(uIFittedSprite2);
		UIFittedSprite uIFittedSprite3 = new UIFittedSprite(m_backgroundShield, false, "Trophy3", PsState.m_uiSheet, frame3);
		uIFittedSprite3.SetHeight(0.25f, RelativeTo.ParentHeight);
		uIFittedSprite3.SetAlign(0.82f, 0.075f);
		m_trophyList.Add(uIFittedSprite3);
	}

	public void Win(Action _callback = null)
	{
		if (m_cupAmount >= 3)
		{
			if (m_rank < PsState.m_versusRankCap)
			{
				RankUp(_callback);
			}
			else
			{
				_callback();
			}
		}
		else
		{
			IncreaseCups(_callback);
		}
	}

	public void Lose(Action _callback = null)
	{
		if (m_rank > 5)
		{
			if (m_cupAmount == 0)
			{
				RankDown(_callback);
			}
			else
			{
				DecreaseCups(_callback);
			}
		}
		else
		{
			Debug.Log("rank not bigger than 5, not decreasing trophies/ranks");
			_callback();
		}
	}

	private void IncreaseCups(Action _callback)
	{
		TimerC timerC = TimerS.AddComponent(EntityManager.AddEntity(), string.Empty, 0f, 0.3f, true, delegate
		{
			Entity entity = EntityManager.AddEntity();
			TransformC parentTC = TransformS.AddComponent(entity, m_trophyList[m_cupAmount].m_TC.transform.position);
			PrefabC c = PrefabS.AddComponent(parentTC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.WinTrophyEffect_GameObject));
			PrefabS.SetCamera(c, m_camera);
			TimerC timerC2 = TimerS.AddComponent(entity, "RemoveTimer", 1f, 0f, true);
			m_trophyList[m_cupAmount].m_frame = m_trophyEnabled;
			m_trophyList[m_cupAmount].Update();
			m_cupAmount++;
			if (_callback != null)
			{
				_callback();
			}
		});
	}

	private void RankUp(Action _callback)
	{
		TweenC c = TweenS.AddTransformTween(m_rankSprite.m_TC, TweenedProperty.Scale, TweenStyle.ExpoIn, new Vector3(1.2f, 1.2f, 1f), 0.4f, 0f, false);
		TweenS.SetAdditionalTweenProperties(c, 0, true, TweenStyle.ExpoOut);
		TimerS.AddComponent(EntityManager.AddEntity(), string.Empty, 0f, 0.4f, true, delegate
		{
			for (int i = 0; i < m_trophyList.Count; i++)
			{
				m_trophyList[i].m_frame = m_trophyDisabled;
				m_trophyList[i].Update();
			}
			m_cupAmount = 0;
			m_rank++;
			m_rankText.SetText(ConvertRank(m_rank).ToString());
			Entity entity = EntityManager.AddEntity();
			TransformC parentTC = TransformS.AddComponent(entity, m_rankSprite.m_TC.transform.position);
			PrefabC c2 = PrefabS.AddComponent(parentTC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.WinTrophyEffect_GameObject));
			PrefabS.SetCamera(c2, m_camera);
			TimerS.AddComponent(entity, "RemoveTimer", 1f, 0f, true);
			IncreaseCups(_callback);
		});
	}

	private void RankDown(Action _callback)
	{
		TweenC c = TweenS.AddTransformTween(m_rankSprite.m_TC, TweenedProperty.Scale, TweenStyle.ExpoIn, new Vector3(0.8f, 0.8f, 1f), 0.4f, 0f, false);
		TweenS.SetAdditionalTweenProperties(c, 0, true, TweenStyle.ExpoOut);
		TimerS.AddComponent(EntityManager.AddEntity(), string.Empty, 0f, 0.4f, true, delegate
		{
			for (int i = 0; i < m_trophyList.Count; i++)
			{
				m_trophyList[i].m_frame = m_trophyEnabled;
				m_trophyList[i].Update();
			}
			m_cupAmount = 3;
			m_rank--;
			m_rankText.SetText(ConvertRank(m_rank).ToString());
			Entity entity = EntityManager.AddEntity();
			TransformC parentTC = TransformS.AddComponent(entity, m_rankSprite.m_TC.transform.position);
			PrefabC c2 = PrefabS.AddComponent(parentTC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.WinTrophyEffect_GameObject));
			PrefabS.SetCamera(c2, m_camera);
			TimerS.AddComponent(entity, "RemoveTimer", 1f, 0f, true);
			DecreaseCups(_callback);
		});
	}

	private void DecreaseCups(Action _callback)
	{
		TimerC timerC = TimerS.AddComponent(EntityManager.AddEntity(), string.Empty, 0f, 0.3f, true, delegate
		{
			m_cupAmount--;
			Entity entity = EntityManager.AddEntity();
			TransformC parentTC = TransformS.AddComponent(entity, m_trophyList[m_cupAmount].m_TC.transform.position);
			PrefabC c = PrefabS.AddComponent(parentTC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.WinTrophyEffect_GameObject));
			PrefabS.SetCamera(c, m_camera);
			TimerC timerC2 = TimerS.AddComponent(entity, "RemoveTimer", 1f, 0f, true);
			m_trophyList[m_cupAmount].m_frame = m_trophyDisabled;
			m_trophyList[m_cupAmount].Update();
			if (_callback != null)
			{
				_callback();
			}
		});
	}

	public void ShowRank()
	{
		m_showingRank = true;
		int num = ConvertRank(m_rank);
		m_rankSprite = new UIFittedSprite(m_backgroundShield, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("challenge_rank_base"));
		m_rankSprite.SetHeight(0.325f, RelativeTo.ParentHeight);
		m_rankSprite.SetDepthOffset(-3f);
		m_rankSprite.SetAlign(0.09f, 0.95f);
		TransformS.SetRotation(m_rankSprite.m_TC, new Vector3(0f, 0f, 6f));
		float fontSize = 0.55f;
		m_rankText = new UIText(m_rankSprite, false, "RankNumber", num.ToString(), "HurmeBold_Font", fontSize, RelativeTo.ParentHeight, "FFFFFF");
		m_rankText.SetVerticalAlign(0.55f);
	}

	private int ConvertRank(int _rank)
	{
		int num = PsState.m_versusRankCap + 1 - _rank;
		return (num < 2) ? 1 : num;
	}
}
