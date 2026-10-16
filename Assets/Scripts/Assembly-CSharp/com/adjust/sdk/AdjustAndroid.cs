using System;
using UnityEngine;

namespace com.adjust.sdk
{
    public class AdjustAndroid : IAdjust
    {
        public AdjustAndroid()
        {
            // Stubbed constructor
        }

        public void onPause()
        {
            // Stubbed
        }

        public void onResume()
        {
            // Stubbed
        }

        public void start(AdjustConfig adjustConfig)
        {
            // Stubbed
        }

        public void trackEvent(AdjustEvent adjustEvent)
        {
            // Stubbed
        }

        public bool isEnabled()
        {
            return false;
        }

        public void setEnabled(bool enabled)
        {
            // Stubbed
        }

        public void setOfflineMode(bool enabled)
        {
            // Stubbed
        }

        public void setReferrer(string referrer)
        {
            // Stubbed
        }

        public void setDeviceToken(string deviceToken)
        {
            // Stubbed
        }
    }
}