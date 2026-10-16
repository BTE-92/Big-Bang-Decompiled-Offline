using System;

namespace Prime31
{
	public class FlurryAndroidManager : AbstractManager
	{
		public static event Action<string> adAvailableForSpaceEvent;

		public static event Action<string> adNotAvailableForSpaceEvent;

		public static event Action<string> onAdClosedEvent;

		public static event Action<string> onApplicationExitEvent;

		public static event Action<string> onRenderFailedEvent;

		public static event Action<string> spaceDidFailToReceiveAdEvent;

		public static event Action<string> spaceDidReceiveAdEvent;

		public static event Action<string> onAdClickedEvent;

		public static event Action<string> onAdOpenedEvent;

		public static event Action<string> onVideoCompletedEvent;

		static FlurryAndroidManager()
		{
			AbstractManager.initialize(typeof(FlurryAndroidManager));
		}

		public void adAvailableForSpace(string adSpace)
		{
			if (adAvailableForSpaceEvent != null)
			{
				adAvailableForSpaceEvent(adSpace);
			}
		}

		public void adNotAvailableForSpace(string adSpace)
		{
			if (adNotAvailableForSpaceEvent != null)
			{
				adNotAvailableForSpaceEvent(adSpace);
			}
		}

		public void onAdClosed(string adSpace)
		{
			if (onAdClosedEvent != null)
			{
				onAdClosedEvent(adSpace);
			}
		}

		public void onApplicationExit(string adSpace)
		{
			if (onApplicationExitEvent != null)
			{
				onApplicationExitEvent(adSpace);
			}
		}

		public void onRenderFailed(string adSpace)
		{
			if (onRenderFailedEvent != null)
			{
				onRenderFailedEvent(adSpace);
			}
		}

		public void spaceDidFailToReceiveAd(string adSpace)
		{
			if (spaceDidFailToReceiveAdEvent != null)
			{
				spaceDidFailToReceiveAdEvent(adSpace);
			}
		}

		public void spaceDidReceiveAd(string adSpace)
		{
			if (spaceDidReceiveAdEvent != null)
			{
				spaceDidReceiveAdEvent(adSpace);
			}
		}

		public void onAdClicked(string adSpace)
		{
			onAdClickedEvent.fire(adSpace);
		}

		public void onAdOpened(string adSpace)
		{
			onAdOpenedEvent.fire(adSpace);
		}

		public void onVideoCompleted(string adSpace)
		{
			onVideoCompletedEvent.fire(adSpace);
		}
	}
}
