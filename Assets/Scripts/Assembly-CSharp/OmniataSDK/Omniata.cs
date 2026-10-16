using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OmniataSDK
{
    public class Omniata : MonoBehaviour
    {
        public enum LogLevel
        {
            Verbose = 2,
            Debug = 3,
            Info = 4,
            Warn = 5,
            Error = 6,
            Assert = 7
        }

        public const string SDK_VERSION = "unitySDK-1.3.0";

        private const string EVENT_PARAM_API_KEY = "api_key";
        private const string EVENT_PARAM_CURRENCY_CODE = "currency_code";
        private const string EVENT_PARAM_EVENT_TYPE = "om_event_type";
        private const string EVENT_PARAM_TOTAL = "total";
        private const string EVENT_PARAM_UID = "uid";
        private const string EVENT_PARAM_OM_DELTA = "om_delta";
        private const string EVENT_PARAM_OM_DEVICE = "om_device";
        private const string EVENT_PARAM_OM_PLATFORM = "om_platform";
        private const string EVENT_PARAM_OM_OS_VERSION = "om_os_version";
        private const string EVENT_PARAM_OM_SDK_VERSION = "om_sdk_version";
        private const string EVENT_PARAM_OM_RETRY = "om_retry";
        private const string EVENT_PARAM_OM_DISCARDED = "om_discarded";
        private const string EVENT_PARAM_OM_UNITY_SDK_VERSION = "om_unity_sdk_version";

        private const string EVENT_TYPE_OM_LOAD = "om_load";
        private const string EVENT_TYPE_OM_REVENUE = "om_revenue";
        private const string CHANNEL_ID = "channel_id";

        private static string api_key;
        private static string uid;
        private static string org;

        public static string analyzerUrl;
        public static string engagerUrl;

        public string API_KEY = "<API KEY>";
        public string UID = "<User ID>";
        public string ORG = "<Orgnization Name>";

        public LogLevel LOGLEVEL = LogLevel.Verbose;

        public bool startManually;
        public bool trackInEditor;

        public static Omniata Instance { get; private set; }

        protected virtual void Awake()
        {
            Instance = this;
            if (!startManually)
            {
                appDidLaunch(API_KEY, UID, ORG, LOGLEVEL);
            }
        }

        public void appDidLaunch(string API_KEY, string UID, string ORG, LogLevel LOGLEVEL)
        {
            api_key = API_KEY;
            uid = UID;
            org = ORG;
            setURL(org);
            SetOmLoglevel(LOGLEVEL);
            Initialize(api_key, uid, org);
            TrackOmLoad();
        }

        public void SetOmLoglevel(LogLevel priority)
        {
            SetLogLevel((int)priority);
        }

        public void LogOm(string message)
        {
            OmniataLog(message);
        }

        public void TrackOmLoad()
        {
            TrackLoad();
        }

        public void TrackOmLoad(Dictionary<string, string> parameters)
        {
            TrackLoad(parameters);
        }

        public void TrackOmRevenue(double total, string currency_code)
        {
            TrackRevenue(total, currency_code);
        }

        public void TrackOmRevenue(double total, string currency_code, Dictionary<string, string> parameters)
        {
            TrackRevenue(total, currency_code, parameters);
        }

        public void TrackOm(string eventType, Dictionary<string, string> parameters)
        {
            Track(eventType, parameters);
        }

        public void TrackOmAdvertiserID()
        {
            TrackAdvertiserID();
        }

        public void TrackOmAdvertiserID(Dictionary<string, string> parameters)
        {
            TrackAdvertiserID(parameters);
        }

        public void LoadOmChannelMessage(int channelID)
        {
        }

        public void EnableOmPushNotifications(string device_token)
        {
            EnablePushNotifications(device_token);
        }

        public void EnablePushNotificationsFromTokenBytes(byte[] device_token)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("om_device_token", BitConverter.ToString(device_token).Replace("-", string.Empty));
            TrackOm("om_apns_enable", dictionary);
        }

        public void DisableOmPushNotifications()
        {
            DisablePushNotifications();
        }

        // ========================================================================
        // STUBBED INTERNAL CALLS (Replaces Android Java JNI & Web Network Calls)
        // ========================================================================

        private static void SetLogLevel(int priority) { }

        private static void OmniataLog(string message) { }

        private static void getContext() { }

        private static void Initialize(string apiKey, string userID, string org) { }

        private static void TrackLoad() { }

        private static void TrackLoad(Dictionary<string, string> dictPara) { }

        private static void TrackRevenue(double total, string currencyCode) { }

        private static void TrackRevenue(double total, string currencyCode, Dictionary<string, string> dictPara) { }

        private static void Track(string eventType, Dictionary<string, string> dictPara) { }

        private static void TrackAdvertiserID() { }

        private static void TrackAdvertiserID(Dictionary<string, string> dictPara) { }

        private static IEnumerator LoadOmChannelMessageCoroutine(int channelID)
        {
            yield break;
        }

        private static void EnablePushNotifications(string device_token) { }

        private static void DisablePushNotifications() { }

        private static void setURL(string morg)
        {
            analyzerUrl = "https://" + morg + ".analyzer.omniata.com/event?";
            engagerUrl = "https://" + morg + ".engager.omniata.com/channel?";
        }

        private static void AddAutomaticParameters(Dictionary<string, string> parameters)
        {
            RuntimePlatform platform = Application.platform;
            parameters.Add("om_event_type", "om_load");
            parameters.Add("om_platform", platform.ToString());
            parameters.Add("om_device", SystemInfo.deviceModel);
            parameters.Add("om_os_version", SystemInfo.operatingSystem);
            parameters.Add("om_sdk_version", "unitySDK-1.3.0");
        }

        private static void AddUnitySDKVersion(Dictionary<string, string> parameters)
        {
            parameters.Add("om_unity_sdk_version", "unitySDK-1.3.0");
        }

        private static string ToKeyValuePairString(Dictionary<string, string> parameters)
        {
            string text = string.Empty;
            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                string text2 = text;
                text = text2 + WWW.EscapeURL(parameter.Key) + "=" + WWW.EscapeURL(parameter.Value) + "\n";
            }
            return text;
        }

        private static string urlGenerator(string baseUrl, Dictionary<string, string> parameters)
        {
            return baseUrl + string.Join("&", ToKeyValuePairString(parameters).Split('\n'));
        }
    }
}