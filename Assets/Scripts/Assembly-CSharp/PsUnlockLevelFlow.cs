using System;
using AdMediation;
using UnityEngine;

public class PsUnlockLevelFlow : Flow
{
	private class RefillKeysFlow : Flow
	{
		private PsUIBasePopup m_popup;

		private PsGameLoop m_gameLoop;

		public RefillKeysFlow(Action _proceed, Action _cancel, Flow _rootFlow, PsGameLoop _loop)
			: base(_proceed, _cancel, _rootFlow)
		{
			m_gameLoop = _loop;
			m_popup = new PsUIBasePopup(typeof(PsUICenterNotEnoughKeys));
			m_popup.SetAction("Exit", UserDeclinedKeyRefill);
			m_popup.SetAction("Refill", UserWantsToRefillKeys);
			m_popup.SetAction("IncreaseAndRefill", UserWantsToIncreaseKeys);
			m_popup.SetAction("Video", UserWatchesAdToRefillKeys);
			TweenS.AddTransformTween(m_popup.m_mainContent.m_parent.m_TC, TweenedProperty.Scale, TweenStyle.ElasticOut, Vector3.one * 0.75f, Vector3.one, 0.75f, 0f, true);
			PsMetagameManager.ShowResources(null, true, false, true, true, false);
		}

		private void UserWatchesAdToRefillKeys()
		{
			if (PsAdMediation.adsAvailable())
			{
				TouchAreaS.Disable();
				PsAdMediation.ShowAd(AdViewCallback);
			}
			else
			{
				UserDeclinedKeyRefill();
			}
		}

		private void AdViewCallback(AdResult _result)
		{
			Debug.Log("Ad result: " + _result);
			if (_result == AdResult.Finished)
			{
				Debug.Log("UNLOCK THE LEVEL");
				TouchAreaS.Disable();
				PsMetrics.AdWatched("unlockLevel");
				if (m_popup != null)
				{
					m_popup.Destroy();
					m_popup = null;
				}
				new PsServerQueueFlow(delegate
				{
					m_gameLoop.m_freeUnlock = true;
					TouchAreaS.Enable();
					Proceed();
				}, null, new string[1] { "login" });
			}
			else
			{
				TouchAreaS.Enable();
				UserDeclinedKeyRefill();
			}
		}

		private void UserWantsToRefillKeys()
		{
			if (PsMetagameManager.PurchaseKeys())
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

		private void UserWantsToIncreaseKeys()
		{
			if (PsMetagameManager.IncreaseKeys())
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

		private void UserDeclinedKeyRefill()
		{
			if (m_popup != null)
			{
				m_popup.Destroy();
				m_popup = null;
				Cancel();
			}
		}
	}

	private PsGameLoop m_gameLoop;

	public PsUnlockLevelFlow(Action _proceed, Action _cancel, PsGameLoop _minigameInfo)
		: base(_proceed, _cancel)
	{
		m_gameLoop = _minigameInfo;
		if (PsMetagameManager.m_playerStats.keys >= 2 || _minigameInfo.m_unlocked)
		{
			Proceed();
			return;
		}
		PsMenuScene.CreateBlur();
		new RefillKeysFlow(UserRefilledKeys, UserDidNotRefillKeys, this, m_gameLoop);
	}

	public void UserRefilledKeys()
	{
		PsMenuScene.RemoveBlur();
		if (!m_gameLoop.m_freeUnlock)
		{
			Cancel();
		}
		else
		{
			new PsServerQueueFlow(delegate
			{
				Proceed();
			}, null, null, "Unlocking level...");
		}
		PsMetagameManager.ShowResources(null, false, true, true, true, true, 0.03f, true);
		SoundS.PlaySingleShot("/UI/RefillKeys", Vector2.zero);
	}

	public void UserDidNotRefillKeys()
	{
		PsMenuScene.RemoveBlur();
		Cancel();
	}
}
