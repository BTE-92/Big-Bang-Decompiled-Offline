namespace UnityEngine.Advertisements
{
    internal class UnityAdsAndroid : UnityAdsPlatform
    {
        public override void init(string gameId, bool testModeEnabled, string gameObjectName, string unityVersion)
        {
            Utils.LogDebug("UnityAndroid (Stub): init(), gameId=" + gameId + ", testModeEnabled=" + testModeEnabled + ", gameObjectName=" + gameObjectName);
        }

        public override bool show(string zoneId, string rewardItemKey, string options)
        {
            Utils.LogDebug("UnityAndroid (Stub): show()");
            return false;
        }

        public override void hide()
        {
            Utils.LogDebug("UnityAndroid (Stub): hide()");
        }

        public override bool isSupported()
        {
            Utils.LogDebug("UnityAndroid (Stub): isSupported()");
            return false;
        }

        public override string getSDKVersion()
        {
            Utils.LogDebug("UnityAndroid (Stub): getSDKVersion()");
            return "1.0.0-stub";
        }

        public override bool canShowZone(string zone)
        {
            return false;
        }

        public override bool hasMultipleRewardItems()
        {
            Utils.LogDebug("UnityAndroid (Stub): hasMultipleRewardItems()");
            return false;
        }

        public override string getRewardItemKeys()
        {
            Utils.LogDebug("UnityAndroid (Stub): getRewardItemKeys()");
            return string.Empty;
        }

        public override string getDefaultRewardItemKey()
        {
            Utils.LogDebug("UnityAndroid (Stub): getDefaultRewardItemKey()");
            return string.Empty;
        }

        public override string getCurrentRewardItemKey()
        {
            Utils.LogDebug("UnityAndroid (Stub): getCurrentRewardItemKey()");
            return string.Empty;
        }

        public override bool setRewardItemKey(string rewardItemKey)
        {
            Utils.LogDebug("UnityAndroid (Stub): setRewardItemKey() rewardItemKey=" + rewardItemKey);
            return false;
        }

        public override void setDefaultRewardItemAsRewardItem()
        {
            Utils.LogDebug("UnityAndroid (Stub): setDefaultRewardItemAsRewardItem()");
        }

        public override string getRewardItemDetailsWithKey(string rewardItemKey)
        {
            Utils.LogDebug("UnityAndroid (Stub): getRewardItemDetailsWithKey() rewardItemKey=" + rewardItemKey);
            return string.Empty;
        }

        public override string getRewardItemDetailsKeys()
        {
            Utils.LogDebug("UnityAndroid (Stub): getRewardItemDetailsKeys()");
            return string.Empty;
        }

        public override void setLogLevel(Advertisement.DebugLevel logLevel)
        {
            Utils.LogDebug("UnityAndroid (Stub): setLogLevel()");
        }
    }
}