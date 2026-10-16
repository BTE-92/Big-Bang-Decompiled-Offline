using UnityEngine;

public class PsUICenterRateGame : UICanvas
{
	private PsUIGenericButton m_twoThumbsUpButton;

	private PsUIGenericButton m_thumbsUpButton;

	private PsUIGenericButton m_thumbsDownButton;

	private PsUIGenericButton m_twoThumbsDownButton;

	private UIText m_titleText;

	private PsUIGenericButton m_offensiveButton;

	private PsUIBasePopup m_offensive;

	private bool m_ratingDone;

	public PsUICenterRateGame(UIComponent _parent)
		: base(_parent, false, "CenterContent", null, string.Empty)
	{
		GetRoot().SetDrawHandler(PsUIDrawHandlers.DebriefBackground);
		SetWidth(1f, RelativeTo.ScreenWidth);
		SetHeight(0.85f, RelativeTo.ParentHeight);
		SetVerticalAlign(0f);
		RemoveDrawHandler();
		m_offensiveButton = new PsUIGenericButton(this);
		m_offensiveButton.SetText("Offensive");
		m_offensiveButton.SetRedColors();
		m_offensiveButton.SetAlign(0.02f, 0.04f);
		UIVerticalList uIVerticalList = new UIVerticalList(this, "Center");
		uIVerticalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIVerticalList.SetAlign(0.5f, 0.1f);
		uIVerticalList.RemoveDrawHandler();
		m_titleText = new UIText(uIVerticalList, false, string.Empty, "Anonymous thumbs", "HurmeSemiBold_Font", 0.04f, RelativeTo.ScreenHeight, "ffffff", "343b2e");
		UIHorizontalList uIHorizontalList = new UIHorizontalList(uIVerticalList, "Center");
		uIHorizontalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.RemoveDrawHandler();
		m_twoThumbsUpButton = new PsUIGenericButton(uIHorizontalList);
		m_twoThumbsUpButton.SetIcon("menu_thumbs_two_up_off", 0.1f);
		m_twoThumbsUpButton.SetSpeechHandle(SpeechBubbleHandlePosition.BottomLeft);
		m_thumbsUpButton = new PsUIGenericButton(uIHorizontalList);
		m_thumbsUpButton.SetIcon("menu_thumbs_up_off", 0.1f);
		m_thumbsUpButton.SetSpeechHandle(SpeechBubbleHandlePosition.BottomLeft);
		m_thumbsDownButton = new PsUIGenericButton(uIHorizontalList);
		m_thumbsDownButton.SetIcon("menu_thumbs_down_off", 0.1f);
		m_thumbsDownButton.SetSpeechHandle(SpeechBubbleHandlePosition.BottomRight);
		m_twoThumbsDownButton = new PsUIGenericButton(uIHorizontalList);
		m_twoThumbsDownButton.SetIcon("menu_thumbs_two_down_off", 0.1f);
		m_twoThumbsDownButton.SetSpeechHandle(SpeechBubbleHandlePosition.BottomRight);
	}

	public override void Step()
	{
		if (m_twoThumbsUpButton != null && m_twoThumbsUpButton.m_hit && !m_ratingDone)
		{
			m_ratingDone = true;
			PsMetagameManager.SendRating(PsRating.Elated, PsState.m_activeGameLoop, PsState.m_activeMinigame);
			PsState.m_activeGameLoop.m_minigameMetaData.rating = PsRating.Elated;
			SoundS.PlaySingleShotWithParameter("/UI/Thumbs", Vector2.zero, "Rating", 2f);
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
		}
		else if (m_thumbsUpButton != null && m_thumbsUpButton.m_hit && !m_ratingDone)
		{
			m_ratingDone = true;
			PsMetagameManager.SendRating(PsRating.Positive, PsState.m_activeGameLoop, PsState.m_activeMinigame);
			PsState.m_activeGameLoop.m_minigameMetaData.rating = PsRating.Positive;
			SoundS.PlaySingleShotWithParameter("/UI/Thumbs", Vector2.zero, "Rating", 1f);
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
		}
		else if (m_thumbsDownButton != null && m_thumbsDownButton.m_hit && !m_ratingDone)
		{
			m_ratingDone = true;
			PsMetagameManager.SendRating(PsRating.Negative, PsState.m_activeGameLoop, PsState.m_activeMinigame);
			PsState.m_activeGameLoop.m_minigameMetaData.rating = PsRating.Negative;
			SoundS.PlaySingleShotWithParameter("/UI/Thumbs", Vector2.zero, "Rating", -1f);
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
		}
		else if (m_twoThumbsDownButton != null && m_twoThumbsDownButton.m_hit && !m_ratingDone)
		{
			m_ratingDone = true;
			PsMetagameManager.SendRating(PsRating.Rejecting, PsState.m_activeGameLoop, PsState.m_activeMinigame);
			PsState.m_activeGameLoop.m_minigameMetaData.rating = PsRating.Rejecting;
			SoundS.PlaySingleShotWithParameter("/UI/Thumbs", Vector2.zero, "Rating", -2f);
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
		}
		else if (m_offensiveButton.m_hit)
		{
			m_offensive = new PsUIBasePopup(typeof(PsUICenterOffensive), typeof(PsUITopBanner), null, null, true, false);
			m_offensive.SetAction("Ok", OkOffensive);
			m_offensive.SetAction("Cancel", CancelOffensive);
		}
		base.Step();
	}

	public void OkOffensive()
	{
		m_offensive.Destroy();
		m_offensive = null;
		(GetRoot() as PsUIBasePopup).CallAction("Exit");
	}

	public void CancelOffensive()
	{
		m_offensive.Destroy();
		m_offensive = null;
	}
}
