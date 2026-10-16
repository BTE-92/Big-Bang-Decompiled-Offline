using System;

namespace Prime31
{
	public class FlurryManager : AbstractManager
	{
		public static event Action<string> spaceDidDismissEvent;

		public static event Action<string> spaceWillLeaveApplicationEvent;

		public static event Action<string> spaceDidFailToRenderEvent;

		public static event Action<string> spaceDidFailToReceiveAdEvent;

		public static event Action<string> spaceDidReceiveAdEvent;

		public static event Action<string> videoDidFinishEvent;

		static FlurryManager()
		{
			AbstractManager.initialize(typeof(FlurryManager));
		}

		private void spaceDidDismiss(string space)
		{
			if (spaceDidDismissEvent != null)
			{
				spaceDidDismissEvent(space);
			}
		}

		private void spaceWillLeaveApplication(string space)
		{
			if (spaceWillLeaveApplicationEvent != null)
			{
				spaceWillLeaveApplicationEvent(space);
			}
		}

		private void spaceDidFailToRender(string space)
		{
			if (spaceDidFailToRenderEvent != null)
			{
				spaceDidFailToRenderEvent(space);
			}
		}

		private void spaceDidFailToReceiveAd(string space)
		{
			if (spaceDidFailToReceiveAdEvent != null)
			{
				spaceDidFailToReceiveAdEvent(space);
			}
		}

		private void spaceDidReceiveAd(string space)
		{
			if (spaceDidReceiveAdEvent != null)
			{
				spaceDidReceiveAdEvent(space);
			}
		}

		private void videoDidFinish(string space)
		{
			if (videoDidFinishEvent != null)
			{
				videoDidFinishEvent(space);
			}
		}
	}
}
