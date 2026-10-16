using System.Collections.Generic;
using UnityEngine;

namespace Helpshift
{
    // Platform-independent stub of the Helpshift Android bridge.
    // Public API is identical to the original so the rest of the project compiles
    // unchanged, but nothing talks to Java and no support screens are shown.
    public class HelpshiftAndroid
    {
        public HelpshiftAndroid()
        {
        }

        public void install(string apiKey, string domain, string appId, Dictionary<string, string> configMap)
        {
        }

        public void install()
        {
        }

        public int getNotificationCount(bool isAsync)
        {
            return 0;
        }

        public void setNameAndEmail(string userName, string email)
        {
        }

        public void setUserIdentifier(string identifier)
        {
        }

        public void registerDeviceToken(string deviceToken)
        {
        }

        public void leaveBreadCrumb(string breadCrumb)
        {
        }

        public void clearBreadCrumbs()
        {
        }

        public void login(string identifier, string userName, string email)
        {
        }

        public void logout()
        {
        }

        public void showConversation(Dictionary<string, object> configMap)
        {
        }

        public void showFAQSection(string sectionPublishId, Dictionary<string, object> configMap)
        {
        }

        public void showSingleFAQ(string questionPublishId, Dictionary<string, object> configMap)
        {
        }

        public void showFAQs(Dictionary<string, object> configMap)
        {
        }

        public void showConversation()
        {
        }

        public void showFAQSection(string sectionPublishId)
        {
        }

        public void showSingleFAQ(string questionPublishId)
        {
        }

        public void showFAQs()
        {
        }

        public void showConversationWithMeta(Dictionary<string, object> configMap)
        {
        }

        public void showFAQSectionWithMeta(string sectionPublishId, Dictionary<string, object> configMap)
        {
        }

        public void showSingleFAQWithMeta(string questionPublishId, Dictionary<string, object> configMap)
        {
        }

        public void showFAQsWithMeta(Dictionary<string, object> configMap)
        {
        }

        public void updateMetaData(Dictionary<string, object> metaData)
        {
        }

        public void handlePushNotification(string issueId)
        {
        }

        public void showAlertToRateAppWithURL(string url)
        {
        }

        public void registerSessionDelegates()
        {
        }

        public void registerForPushWithGcmId(string gcmId)
        {
        }
    }
}