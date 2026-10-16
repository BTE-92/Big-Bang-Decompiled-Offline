using UnityEngine;

namespace ChartboostSDK
{
    // Platform-independent stub of the Chartboost bridge.
    // Public API is identical to the original Android version so the rest of the
    // project compiles unchanged, but nothing talks to Java or any ad network.
    // Every "has..." query returns false, every "show/cache..." call does nothing.
    public class CBExternal
    {
        private static bool initialized;

        private static string _logTag = "ChartboostSDK";

        private static string _customId = string.Empty;

        private static bool _autoCacheAds = true;

        public static void Log(string message)
        {
            if (CBSettings.isLogging() && UnityEngine.Debug.isDebugBuild)
            {
                Debug.Log(_logTag + "/" + message);
            }
        }

        public static void init()
        {
            initialized = true;
            Log("Chartboost disabled: init skipped (stub)");
        }

        public static bool isAnyViewVisible()
        {
            return false;
        }

        public static void cacheInterstitial(CBLocation location)
        {
        }

        public static bool hasInterstitial(CBLocation location)
        {
            return false;
        }

        public static void showInterstitial(CBLocation location)
        {
        }

        public static void cacheMoreApps(CBLocation location)
        {
        }

        public static bool hasMoreApps(CBLocation location)
        {
            return false;
        }

        public static void showMoreApps(CBLocation location)
        {
        }

        public static void cacheInPlay(CBLocation location)
        {
        }

        public static bool hasInPlay(CBLocation location)
        {
            return false;
        }

        public static CBInPlay getInPlay(CBLocation location)
        {
            return null;
        }

        public static void cacheRewardedVideo(CBLocation location)
        {
        }

        public static bool hasRewardedVideo(CBLocation location)
        {
            return false;
        }

        public static void showRewardedVideo(CBLocation location)
        {
        }

        public static void chartBoostShouldDisplayInterstitialCallbackResult(bool result)
        {
        }

        public static void chartBoostShouldDisplayRewardedVideoCallbackResult(bool result)
        {
        }

        public static void chartBoostShouldDisplayMoreAppsCallbackResult(bool result)
        {
        }

        public static void didPassAgeGate(bool pass)
        {
        }

        public static void setShouldPauseClickForConfirmation(bool shouldPause)
        {
        }

        public static string getCustomId()
        {
            return _customId;
        }

        public static void setCustomId(string customId)
        {
            _customId = customId;
        }

        public static bool getAutoCacheAds()
        {
            return _autoCacheAds;
        }

        public static void setAutoCacheAds(bool autoCacheAds)
        {
            _autoCacheAds = autoCacheAds;
        }

        public static void setShouldRequestInterstitialsInFirstSession(bool shouldRequest)
        {
        }

        public static void setShouldDisplayLoadingViewForMoreApps(bool shouldDisplay)
        {
        }

        public static void setShouldPrefetchVideoContent(bool shouldPrefetch)
        {
        }

        public static void trackLevelInfo(string eventLabel, CBLevelType type, int mainLevel, int subLevel, string description)
        {
        }

        public static void trackLevelInfo(string eventLabel, CBLevelType type, int mainLevel, string description)
        {
        }

        public static void setGameObjectName(string name)
        {
        }

        public static void pause(bool paused)
        {
        }

        public static void destroy()
        {
            initialized = false;
        }

        public static bool onBackPressed()
        {
            // false = Chartboost did not handle the back button, so the game should.
            return false;
        }

        public static void trackInAppGooglePlayPurchaseEvent(string title, string description, string price, string currency, string productID, string purchaseData, string purchaseSignature)
        {
        }

        public static void trackInAppAmazonStorePurchaseEvent(string title, string description, string price, string currency, string productID, string userID, string purchaseToken)
        {
        }
    }
}