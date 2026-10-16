using System;
using UnityEngine;

public class PsUITopLevelLoading : UICanvas
{
	private PsUIGenericButton m_exitButton;

	private PsUILevelHeader m_header;

	private PsUIInfoBar m_infoBar;

	private bool m_animationDone;

	public PsUITopLevelLoading(UIComponent _parent)
		: base(_parent, false, "TopContent", null, string.Empty)
	{
		RemoveDrawHandler();
		CreateLeftArea();
		CreateHeader(false);
		m_infoBar = new PsUIInfoBar(this, string.Empty);
	}

	protected virtual void CreateLeftArea()
	{
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, "UpperLeft");
		uIHorizontalList.SetMargins(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetAlign(0f, 1f);
		uIHorizontalList.RemoveDrawHandler();
		m_exitButton = new PsUIGenericButton(uIHorizontalList);
		m_exitButton.SetIcon("hud_icon_back", 0.06f);
		m_exitButton.SetOrangeColors();
	}

	private void CreateHeader(bool _update = true)
	{
		if (PsState.m_activeGameLoop == null)
		{
			return;
		}
		if (PsState.m_activeGameLoop.m_minigameMetaData == null)
		{
			if (!PsState.m_activeGameLoop.m_loadingMetaData)
			{
				PsState.m_activeGameLoop.LoadMinigameMetaData(delegate
				{
					CreateHeader();
				});
				Debug.LogWarning("start loading minigame metadata, adding callback");
				return;
			}
			PsGameLoop activeGameLoop = PsState.m_activeGameLoop;
			activeGameLoop.m_loadMetadataCallback = (Action)Delegate.Combine(activeGameLoop.m_loadMetadataCallback, (Action)delegate
			{
				CreateHeader();
			});
			Debug.LogWarning("still loading minigame metadata, adding callback");
		}
		else
		{
			m_header = new PsUILevelHeader(this);
			m_header.SetVerticalAlign(0.98f);
			if (_update)
			{
				Update();
			}
		}
	}

	public override void Update()
	{
		base.Update();
		if (!m_animationDone && m_header != null)
		{
			TweenS.AddTransformTween(m_header.m_TC, TweenedProperty.Position, TweenStyle.BackOut, m_header.m_TC.transform.localPosition + new Vector3(0f, 120f, 0f), m_header.m_TC.transform.localPosition, 0.3f, 0f, true);
			m_animationDone = true;
		}
	}

	public override void Step()
	{
		if (m_exitButton != null && m_exitButton.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
		}
		base.Step();
	}

	public void Close()
	{
		TweenS.AddTransformTween(m_header.m_TC, TweenedProperty.Position, TweenStyle.Linear, m_header.m_TC.transform.localPosition, m_header.m_TC.transform.localPosition + new Vector3(0f, 120f, 0f), 0.2f, 0f, true);
		m_infoBar.TweenAway();
	}

	public void SetBackButtonActive(bool flag)
	{
		if (m_exitButton != null)
		{
			if (flag)
			{
				m_exitButton.SetOrangeColors();
				m_exitButton.m_TAC.m_active = true;
			}
			else
			{
				m_exitButton.SetGrayColors();
				m_exitButton.m_TAC.m_active = false;
			}
			m_exitButton.Update();
		}
	}
}
