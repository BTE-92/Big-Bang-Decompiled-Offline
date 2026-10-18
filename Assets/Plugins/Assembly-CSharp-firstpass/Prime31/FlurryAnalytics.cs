using System.Collections.Generic;
using UnityEngine;

namespace Prime31
{
    public class FlurryAnalytics
    {
        private static AndroidJavaClass _flurryAgent;
        private static AndroidJavaObject _plugin;

        static FlurryAnalytics()
        {
            // Stubbed out: native Java initialization disabled
        }

        public static void startSession(string apiKey, bool enableLogging = false)
        {
            // Stubbed out
        }

        public static void onEndSession()
        {
            // Stubbed out
        }

        public static void addUserCookie(string key, string value)
        {
            // Stubbed out
        }

        public static void clearUserCookies()
        {
            // Stubbed out
        }

        public static void setContinueSessionMillis(long milliseconds)
        {
            // Stubbed out
        }

        public static void logEvent(string eventName)
        {
            // Stubbed out
        }

        public static void logEvent(string eventName, bool isTimed)
        {
            // Stubbed out
        }

        public static void logEvent(string eventName, Dictionary<string, string> parameters)
        {
            // Stubbed out
        }

        public static void logEvent(string eventName, Dictionary<string, string> parameters, bool isTimed)
        {
            // Stubbed out
        }

        public static void endTimedEvent(string eventName)
        {
            // Stubbed out
        }

        public static void endTimedEvent(string eventName, Dictionary<string, string> parameters)
        {
            // Stubbed out
        }

        public static void onPageView()
        {
            // Stubbed out
        }

        public static void onError(string errorId, string message, string errorClass)
        {
            // Stubbed out
        }

        public static void setUserID(string userId)
        {
            // Stubbed out
        }

        public static void setAge(int age)
        {
            // Stubbed out
        }

        public static void setGender(FlurryGender gender)
        {
            // Stubbed out
        }

        public static void setLogEnabled(bool enable)
        {
            // Stubbed out
        }
    }
}