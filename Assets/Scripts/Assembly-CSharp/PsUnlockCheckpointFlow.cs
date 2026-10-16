using System;
using AdMediation;
using UnityEngine;

public class PsUnlockCheckpointFlow : Flow
{
	private class PurchaseStarsFlow : Flow
	{
		private PsUIBasePopup m_popup;

		public PurchaseStarsFlow(Action _proceed, Action _cancel, Flow _rootFlow)
			: base(_proceed, _cancel, _rootFlow)
		{
			m_popup = new PsUIBasePopup(typeof(PsUICenterNotEnoughStars));
			m_popup.SetAction("Exit", UserDeclinedStarPurchase);
			m_popup.SetAction("Purchase", UserWantsToPurchaseStars);
			TweenS.AddTransformTween(m_popup.m_mainContent.m_parent.m_TC, TweenedProperty.Scale, TweenStyle.ElasticOut, Vector3.one * 0.75f, Vector3.one, 0.75f, 0f, true);
			PsMetagameManager.ShowResources(null, true, false, true, false, false);
		}

		private void UserWatchesAdToGainStars()
		{
			if (PsAdMediation.adsAvailable())
			{
				TouchAreaS.Disable();
				PsAdMediation.ShowAd(AdViewCallback);
			}
			else
			{
				UserDeclinedStarPurchase();
			}
		}

		private void AdViewCallback(AdResult _result)
		{
			Debug.Log("Ad result: " + _result);
			if (_result == AdResult.Finished)
			{
				TouchAreaS.Disable();
				PsMetrics.AdWatched("unlockCheckpoint");
				m_popup.Destroy();
				m_popup = null;
				new PsServerQueueFlow(delegate
				{
					TouchAreaS.Enable();
					Proceed();
				}, null, new string[1] { "login" });
			}
			else
			{
				TouchAreaS.Enable();
				UserDeclinedStarPurchase();
			}
		}

		private void UserWantsToPurchaseStars()
		{
			int stars = 0;
			int num = 0;
			if (PsState.m_activeGameLoop is PsGameLoopBlockLevel)
			{
				num = (PsState.m_activeGameLoop as PsGameLoopBlockLevel).m_requiredStars;
				stars = num - PsMetagameManager.m_playerStats.stars;
			}
			else if (PsState.m_activeGameLoop is PsGameLoopBlockSignal)
			{
				num = (PsState.m_activeGameLoop as PsGameLoopBlockSignal).m_requiredStars;
				stars = num - PsMetagameManager.m_playerStats.stars;
			}
			if (PsMetagameManager.PurchaseStars(stars, false))
			{
				m_popup.Destroy();
				m_popup = null;
				Proceed();
			}
			else
			{
				new PsGetDiamondsFlow();
			}
		}

		private void UserDeclinedStarPurchase()
		{
			m_popup.Destroy();
			m_popup = null;
			Cancel();
		}
	}

	public PsUnlockCheckpointFlow(Action _proceed, Action _cancel, PsGameLoopBlockLevel _minigameInfo)
		: base(_proceed, _cancel)
	{
		Debug.LogWarning("BLOCK LEVEL");
		if (PsMetagameManager.m_playerStats.stars >= _minigameInfo.m_requiredStars || _minigameInfo.m_unlocked)
		{
			Proceed();
			return;
		}
		PsMenuScene.CreateBlur();
		new PurchaseStarsFlow(UserPurchasedStars, UserDidNotPurchaseStars, this);
	}

	public PsUnlockCheckpointFlow(Action _proceed, Action _cancel, PsGameLoopBlockSignalWithoutLevel _minigameInfo)
		: base(_proceed, _cancel)
	{
		Debug.LogWarning("BLOCK SIGNAL WITHOUT LEVEL");
		if (PsMetagameManager.m_playerStats.stars >= _minigameInfo.m_requiredStars || _minigameInfo.m_unlocked)
		{
			Proceed();
			return;
		}
		PsMenuScene.CreateBlur();
		new PurchaseStarsFlow(UserPurchasedStars, UserDidNotPurchaseStars, this);
	}

	public PsUnlockCheckpointFlow(Action _proceed, Action _cancel, PsGameLoopBlockSignal _minigameInfo)
		: base(_proceed, _cancel)
	{
		Debug.LogWarning("BLOCK SIGNAL");
		if (PsMetagameManager.m_playerStats.stars >= _minigameInfo.m_requiredStars || _minigameInfo.m_unlocked)
		{
			Proceed();
			return;
		}
		PsMenuScene.CreateBlur();
		new PurchaseStarsFlow(UserPurchasedStars, UserDidNotPurchaseStars, this);
	}

	public void UserPurchasedStars()
	{
		SoundS.PlaySingleShot("/Ingame/Events/UnlockCheckpoint", Vector2.zero);
		new PsServerQueueFlow(delegate
		{
			PsMenuScene.RemoveBlur();
			Proceed();
		}, null, null, "Unlocking checkpoint...");
	}

	public void UserDidNotPurchaseStars()
	{
		PsMenuScene.RemoveBlur();
		Cancel();
	}
}
