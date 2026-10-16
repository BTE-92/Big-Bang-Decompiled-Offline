using System;
using UnityEngine;

public class PsUICenterLevelLoading : UICanvas
{
	private PsUIScreenshot m_screenshot;

	private PsUIGenericButton m_playButton;

	private PsUIVersusLoadingBanner m_versusBanner;

	private PsLevelLoadingScene m_scene;

	private UIText m_loadingText;

	private UICanvas m_background;

	private bool m_building;

	private bool m_animationDone;

	private bool m_gameStarted;

	public PsUICenterLevelLoading(UIComponent _parent)
		: base(_parent, false, string.Empty, null, string.Empty)
	{
		m_scene = Main.m_currentGame.m_sceneManager.m_loadingScene as PsLevelLoadingScene;
		PsMenuScene.CreateBlur();
		SetWidth(0.7f, RelativeTo.ScreenWidth);
		SetHeight(1f, RelativeTo.ScreenHeight);
		RemoveDrawHandler();
		UpdateScreenshot();
		m_loadingText = new UIText(this, false, string.Empty, "LOADING LEVEL...", "HurmeBold_Font", 0.045f, RelativeTo.ScreenHeight, "#ffffff");
		m_loadingText.SetVerticalAlign(0.18f);
	}

	private void UpdateScreenshot()
	{
		if (PsState.m_activeGameLoop == null)
		{
			return;
		}
		if (PsState.m_activeGameLoop.m_minigameMetaData == null)
		{
			if (!PsState.m_activeGameLoop.m_loadingMetaData)
			{
				PsState.m_activeGameLoop.LoadMinigameMetaData(UpdateScreenshot);
				return;
			}
			PsGameLoop activeGameLoop = PsState.m_activeGameLoop;
			activeGameLoop.m_loadMetadataCallback = (Action)Delegate.Combine(activeGameLoop.m_loadMetadataCallback, new Action(UpdateScreenshot));
			return;
		}
		m_screenshot = new PsUIScreenshot(this, false, "Screenshot", Vector2.zero, PsState.m_activeGameLoop);
		m_screenshot.SetSize(0.71999997f, 0.504f, RelativeTo.ScreenHeight);
		m_screenshot.SetDepthOffset(-10f);
		if (m_versusBanner != null)
		{
			m_versusBanner.UpdateOpponentInfo();
		}
	}

	public override void Step()
	{
		if (!m_building && PsState.m_activeGameLoop.m_minigameBytes != null && m_loadingText != null)
		{
			m_building = true;
			m_loadingText.SetText("BUILDING LEVEL...");
			((GetRoot() as PsUIBasePopup).m_topContent as PsUITopLevelLoading).SetBackButtonActive(false);
		}
		if (PsState.m_activeGameLoop is PsGameLoopVersus && m_versusBanner == null)
		{
			m_versusBanner = new PsUIVersusLoadingBanner(this, PsState.m_activeGameLoop as PsGameLoopVersus);
			m_versusBanner.SetVerticalAlign(0.275f);
			m_versusBanner.SetDepthOffset(-25f);
			TweenS.AddTransformTween(m_versusBanner.m_TC, TweenedProperty.Scale, TweenStyle.BackOut, Vector3.zero, m_versusBanner.m_TC.transform.localScale, 0.3f, 0f, true);
		}
		if (m_playButton == null && m_scene.m_outroStarted)
		{
			m_playButton = new PsUIGenericButton(this);
			m_playButton.SetText("PLAY", 0.06f, 0.2f);
			m_playButton.SetAlign(0.72f, 0.14f);
			m_playButton.SetGreenColors();
			m_playButton.SetSound(null);
			m_loadingText.Destroy();
			Update();
			TweenS.AddTransformTween(m_playButton.m_TC, TweenedProperty.Scale, TweenStyle.BackOut, Vector3.zero, m_playButton.m_TC.transform.localScale, 0.3f, 0f, true);
			((GetRoot() as PsUIBasePopup).m_topContent as PsUITopLevelLoading).SetBackButtonActive(true);
		}
		if (m_screenshot != null && !m_animationDone)
		{
			Update();
			TweenS.AddTransformTween(m_screenshot.m_TC, TweenedProperty.Scale, TweenStyle.BackOut, Vector3.zero, m_screenshot.m_TC.transform.localScale, 0.3f, 0f, true);
			m_animationDone = true;
			SoundS.PlaySingleShot("/UI/DomeScreenAppear", Vector2.zero);
		}
		if (m_playButton != null && m_playButton.m_hit && !m_gameStarted)
		{
			m_gameStarted = true;
			(GetRoot() as PsUIBasePopup).CallAction("StartGame");
			((GetRoot() as PsUIBasePopup).m_topContent as PsUITopLevelLoading).Close();
			TweenS.AddTransformTween(m_playButton.m_TC, TweenedProperty.Scale, TweenStyle.Linear, m_playButton.m_TC.transform.localScale, Vector3.zero, 0.2f, 0f, true);
			TweenS.AddTransformTween(m_screenshot.m_TC, TweenedProperty.Scale, TweenStyle.Linear, m_screenshot.m_TC.transform.localScale, Vector3.zero, 0.2f, 0f, true);
			if (m_versusBanner != null)
			{
				TweenS.AddTransformTween(m_versusBanner.m_TC, TweenedProperty.Scale, TweenStyle.Linear, m_versusBanner.m_TC.transform.localScale, Vector3.zero, 0.2f, 0f, true);
			}
		}
		base.Step();
	}

	public override void Destroy()
	{
		base.Destroy();
	}
}
