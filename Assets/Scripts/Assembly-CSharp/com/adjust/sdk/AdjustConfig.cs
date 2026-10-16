using System;

namespace com.adjust.sdk
{
	public class AdjustConfig
	{
		internal string appToken;

		internal string sceneName;

		internal string defaultTracker;

		internal bool? eventBufferingEnabled;

		internal AdjustLogLevel? logLevel;

		internal AdjustEnvironment environment;

		internal Action<AdjustAttribution> attributionChangedDelegate;

		internal bool macMd5TrackingEnabled;

		internal string processName;

		internal Action<string> logDelegate;

		public AdjustConfig(string appToken, AdjustEnvironment environment)
		{
			sceneName = string.Empty;
			this.appToken = appToken;
			this.environment = environment;
			macMd5TrackingEnabled = false;
			processName = string.Empty;
		}

		public void setLogLevel(AdjustLogLevel logLevel)
		{
			this.logLevel = logLevel;
		}

		public void setDefaultTracker(string defaultTracker)
		{
			this.defaultTracker = defaultTracker;
		}

		public void setEventBufferingEnabled(bool eventBufferingEnabled)
		{
			this.eventBufferingEnabled = eventBufferingEnabled;
		}

		public void setAttributionChangedDelegate(Action<AdjustAttribution> attributionChangedDelegate, string sceneName = "Adjust")
		{
			this.attributionChangedDelegate = attributionChangedDelegate;
			this.sceneName = sceneName;
		}

		public Action<AdjustAttribution> getAttributionChangedDelegate()
		{
			return attributionChangedDelegate;
		}

		public void setMacMd5TrackingEnabled(bool macMd5TrackingEnabled)
		{
			this.macMd5TrackingEnabled = macMd5TrackingEnabled;
		}

		public void setProcessName(string processName)
		{
			this.processName = processName;
		}

		public void setLogDelegate(Action<string> logDelegate)
		{
			this.logDelegate = logDelegate;
		}
	}
}
