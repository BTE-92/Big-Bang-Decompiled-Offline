using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

public class Everyplay : MonoBehaviour
{
    public enum FaceCamPreviewOrigin
    {
        TopLeft = 0,
        TopRight = 1,
        BottomLeft = 2,
        BottomRight = 3
    }

    public enum FaceCamRecordingMode
    {
        RecordAudio = 0,
        RecordVideo = 1,
        PassThrough = 2
    }

    public enum UserInterfaceIdiom
    {
        Phone = 0,
        Tablet = 1,
        TV = 2,
        iPhone = Phone,
        iPad = Tablet
    }

    public delegate void WasClosedDelegate();
    public delegate void ReadyForRecordingDelegate(bool enabled);
    public delegate void RecordingStartedDelegate();
    public delegate void RecordingStoppedDelegate();
    public delegate void FaceCamSessionStartedDelegate();
    public delegate void FaceCamRecordingPermissionDelegate(bool granted);
    public delegate void FaceCamSessionStoppedDelegate();

    [Obsolete("Use ThumbnailTextureReadyDelegate(Texture2D texture,bool portrait) instead.")]
    public delegate void ThumbnailReadyAtTextureIdDelegate(int textureId, bool portrait);

    public delegate void ThumbnailTextureReadyDelegate(Texture2D texture, bool portrait);
    public delegate void UploadDidStartDelegate(int videoId);
    public delegate void UploadDidProgressDelegate(int videoId, float progress);
    public delegate void UploadDidCompleteDelegate(int videoId);
    public delegate void RequestReadyDelegate(string response);
    public delegate void RequestFailedDelegate(string error);

    private static string clientId;
    private static bool appIsClosing;
    private static EveryplayLegacy everyplayLegacy;
    private static Everyplay everyplayInstance;
    private static Texture2D currentThumbnailTargetTexture;

    [Obsolete("Calling Everyplay with SharedInstance is deprecated, you may remove SharedInstance.")]
    public static EveryplayLegacy SharedInstance
    {
        get
        {
            if (EveryplayInstance != null && everyplayLegacy == null)
            {
                everyplayLegacy = everyplayInstance.gameObject.AddComponent<EveryplayLegacy>();
            }
            return everyplayLegacy;
        }
    }

    private static Everyplay EveryplayInstance
    {
        get
        {
            if (everyplayInstance == null && !appIsClosing)
            {
                GameObject gameObject = new GameObject("Everyplay");
                if (gameObject != null)
                {
                    everyplayInstance = gameObject.AddComponent<Everyplay>();
                    UnityEngine.Object.DontDestroyOnLoad(gameObject);
                }
            }
            return everyplayInstance;
        }
    }

    public static event WasClosedDelegate WasClosed;
    public static event ReadyForRecordingDelegate ReadyForRecording;
    public static event RecordingStartedDelegate RecordingStarted;
    public static event RecordingStoppedDelegate RecordingStopped;
    public static event FaceCamSessionStartedDelegate FaceCamSessionStarted;
    public static event FaceCamRecordingPermissionDelegate FaceCamRecordingPermission;
    public static event FaceCamSessionStoppedDelegate FaceCamSessionStopped;

    [Obsolete("Use ThumbnailTextureReady instead.")]
    public static event ThumbnailReadyAtTextureIdDelegate ThumbnailReadyAtTextureId;

    public static event ThumbnailTextureReadyDelegate ThumbnailTextureReady;
    public static event UploadDidStartDelegate UploadDidStart;
    public static event UploadDidProgressDelegate UploadDidProgress;
    public static event UploadDidCompleteDelegate UploadDidComplete;

    public static void Initialize()
    {
        // Stubbed initialization
    }

    public static void Show()
    {
        EveryplayShow();
    }

    public static void ShowWithPath(string path)
    {
        EveryplayShowWithPath(path);
    }

    public static void PlayVideoWithURL(string url)
    {
        EveryplayPlayVideoWithURL(url);
    }

    public static void PlayVideoWithDictionary(Dictionary<string, object> dict)
    {
        EveryplayPlayVideoWithDictionary(string.Empty);
    }

    public static void MakeRequest(string method, string url, Dictionary<string, object> data, RequestReadyDelegate readyDelegate, RequestFailedDelegate failedDelegate)
    {
        if (EveryplayInstance != null)
        {
            EveryplayInstance.AsyncMakeRequest(method, url, data, readyDelegate, failedDelegate);
        }
    }

    public static string AccessToken()
    {
        return EveryplayAccountAccessToken();
    }

    public static void ShowSharingModal()
    {
        EveryplayShowSharingModal();
    }

    public static void PlayLastRecording()
    {
        EveryplayPlayLastRecording();
    }

    public static void StartRecording()
    {
        EveryplayStartRecording();
    }

    public static void StopRecording()
    {
        EveryplayStopRecording();
    }

    public static void PauseRecording()
    {
        EveryplayPauseRecording();
    }

    public static void ResumeRecording()
    {
        EveryplayResumeRecording();
    }

    public static bool IsRecording()
    {
        return EveryplayIsRecording();
    }

    public static bool IsRecordingSupported()
    {
        return EveryplayIsRecordingSupported();
    }

    public static bool IsPaused()
    {
        return EveryplayIsPaused();
    }

    public static bool SnapshotRenderbuffer()
    {
        return EveryplaySnapshotRenderbuffer();
    }

    public static bool IsSupported()
    {
        return EveryplayIsSupported();
    }

    public static bool IsSingleCoreDevice()
    {
        return EveryplayIsSingleCoreDevice();
    }

    public static int GetUserInterfaceIdiom()
    {
        return EveryplayGetUserInterfaceIdiom();
    }

    public static void SetMetadata(string key, object val)
    {
        EveryplaySetMetadata(string.Empty);
    }

    public static void SetMetadata(Dictionary<string, object> dict)
    {
        EveryplaySetMetadata(string.Empty);
    }

    public static void SetTargetFPS(int fps)
    {
        EveryplaySetTargetFPS(fps);
    }

    public static void SetMotionFactor(int factor)
    {
        EveryplaySetMotionFactor(factor);
    }

    public static void SetMaxRecordingMinutesLength(int minutes)
    {
        EveryplaySetMaxRecordingMinutesLength(minutes);
    }

    public static void SetLowMemoryDevice(bool state)
    {
        EveryplaySetLowMemoryDevice(state);
    }

    public static void SetDisableSingleCoreDevices(bool state)
    {
        EveryplaySetDisableSingleCoreDevices(state);
    }

    public static bool FaceCamIsVideoRecordingSupported()
    {
        return EveryplayFaceCamIsVideoRecordingSupported();
    }

    public static bool FaceCamIsAudioRecordingSupported()
    {
        return EveryplayFaceCamIsAudioRecordingSupported();
    }

    public static bool FaceCamIsHeadphonesPluggedIn()
    {
        return EveryplayFaceCamIsHeadphonesPluggedIn();
    }

    public static bool FaceCamIsSessionRunning()
    {
        return EveryplayFaceCamIsSessionRunning();
    }

    public static bool FaceCamIsRecordingPermissionGranted()
    {
        return EveryplayFaceCamIsRecordingPermissionGranted();
    }

    public static float FaceCamAudioPeakLevel()
    {
        return EveryplayFaceCamAudioPeakLevel();
    }

    public static float FaceCamAudioPowerLevel()
    {
        return EveryplayFaceCamAudioPowerLevel();
    }

    public static void FaceCamSetMonitorAudioLevels(bool enabled)
    {
        EveryplayFaceCamSetMonitorAudioLevels(enabled);
    }

    public static void FaceCamSetRecordingMode(FaceCamRecordingMode mode)
    {
        EveryplayFaceCamSetRecordingMode((int)mode);
    }

    public static void FaceCamSetAudioOnly(bool audioOnly)
    {
        EveryplayFaceCamSetAudioOnly(audioOnly);
    }

    public static void FaceCamSetPreviewVisible(bool visible)
    {
        EveryplayFaceCamSetPreviewVisible(visible);
    }

    public static void FaceCamSetPreviewScaleRetina(bool autoScale)
    {
        EveryplayFaceCamSetPreviewScaleRetina(autoScale);
    }

    public static void FaceCamSetPreviewSideWidth(int width)
    {
        EveryplayFaceCamSetPreviewSideWidth(width);
    }

    public static void FaceCamSetPreviewBorderWidth(int width)
    {
        EveryplayFaceCamSetPreviewBorderWidth(width);
    }

    public static void FaceCamSetPreviewPositionX(int x)
    {
        EveryplayFaceCamSetPreviewPositionX(x);
    }

    public static void FaceCamSetPreviewPositionY(int y)
    {
        EveryplayFaceCamSetPreviewPositionY(y);
    }

    public static void FaceCamSetPreviewBorderColor(float r, float g, float b, float a)
    {
        EveryplayFaceCamSetPreviewBorderColor(r, g, b, a);
    }

    public static void FaceCamSetPreviewOrigin(FaceCamPreviewOrigin origin)
    {
        EveryplayFaceCamSetPreviewOrigin((int)origin);
    }

    public static void FaceCamSetTargetTexture(Texture2D texture)
    {
        // Stubbed
    }

    [Obsolete("Use FaceCamSetTargetTexture(Texture2D texture) instead.")]
    public static void FaceCamSetTargetTextureId(int textureId)
    {
        EveryplayFaceCamSetTargetTextureId(textureId);
    }

    [Obsolete("Defining texture width is no longer required when FaceCamSetTargetTexture(Texture2D texture) is used.")]
    public static void FaceCamSetTargetTextureWidth(int textureWidth)
    {
        EveryplayFaceCamSetTargetTextureWidth(textureWidth);
    }

    [Obsolete("Defining texture height is no longer required when FaceCamSetTargetTexture(Texture2D texture) is used.")]
    public static void FaceCamSetTargetTextureHeight(int textureHeight)
    {
        EveryplayFaceCamSetTargetTextureHeight(textureHeight);
    }

    public static void FaceCamStartSession()
    {
        EveryplayFaceCamStartSession();
    }

    public static void FaceCamRequestRecordingPermission()
    {
        EveryplayFaceCamRequestRecordingPermission();
    }

    public static void FaceCamStopSession()
    {
        EveryplayFaceCamStopSession();
    }

    public static void SetThumbnailTargetTexture(Texture2D texture)
    {
        currentThumbnailTargetTexture = texture;
    }

    [Obsolete("Use SetThumbnailTargetTexture(Texture2D texture) instead.")]
    public static void SetThumbnailTargetTextureId(int textureId)
    {
        EveryplaySetThumbnailTargetTextureId(textureId);
    }

    [Obsolete("Defining texture width is no longer required when SetThumbnailTargetTexture(Texture2D texture) is used.")]
    public static void SetThumbnailTargetTextureWidth(int textureWidth)
    {
        EveryplaySetThumbnailTargetTextureWidth(textureWidth);
    }

    [Obsolete("Defining texture height is no longer required when SetThumbnailTargetTexture(Texture2D texture) is used.")]
    public static void SetThumbnailTargetTextureHeight(int textureHeight)
    {
        EveryplaySetThumbnailTargetTextureHeight(textureHeight);
    }

    public static void TakeThumbnail()
    {
        EveryplayTakeThumbnail();
    }

    private static void RemoveAllEventHandlers()
    {
        WasClosed = null;
        ReadyForRecording = null;
        RecordingStarted = null;
        RecordingStopped = null;
        FaceCamSessionStarted = null;
        FaceCamRecordingPermission = null;
        FaceCamSessionStopped = null;
        ThumbnailReadyAtTextureId = null;
        ThumbnailTextureReady = null;
        UploadDidStart = null;
        UploadDidProgress = null;
        UploadDidComplete = null;
    }

    private void AsyncMakeRequest(string method, string url, Dictionary<string, object> data, RequestReadyDelegate readyDelegate, RequestFailedDelegate failedDelegate)
    {
        StartCoroutine(MakeRequestEnumerator(method, url, data, readyDelegate, failedDelegate));
    }

    private IEnumerator MakeRequestEnumerator(string method, string url, Dictionary<string, object> data, RequestReadyDelegate readyDelegate, RequestFailedDelegate failedDelegate)
    {
        if (failedDelegate != null)
        {
            failedDelegate("Everyplay service is disabled/offline.");
        }
        yield break;
    }

    private void OnApplicationQuit()
    {
        if (currentThumbnailTargetTexture != null)
        {
            SetThumbnailTargetTexture(null);
            currentThumbnailTargetTexture = null;
        }
        RemoveAllEventHandlers();
        appIsClosing = true;
        everyplayInstance = null;
    }

    // ========================================================================
    // STUBBED INTERNAL CALLS (Replaces Java JNI and Native Platform Bindings)
    // ========================================================================

    public static void InitEveryplay(string clientId, string clientSecret, string redirectURI) { }
    public static void EveryplayShow() { }
    public static void EveryplayShowWithPath(string path) { }
    public static void EveryplayPlayVideoWithURL(string url) { }
    public static void EveryplayPlayVideoWithDictionary(string dic) { }
    public static string EveryplayAccountAccessToken() { return null; }
    public static void EveryplayShowSharingModal() { }
    public static void EveryplayPlayLastRecording() { }
    public static void EveryplayStartRecording() { }
    public static void EveryplayStopRecording() { }
    public static void EveryplayPauseRecording() { }
    public static void EveryplayResumeRecording() { }
    public static bool EveryplayIsRecording() { return false; }
    public static bool EveryplayIsRecordingSupported() { return false; }
    public static bool EveryplayIsPaused() { return false; }
    public static bool EveryplaySnapshotRenderbuffer() { return false; }
    public static void EveryplaySetMetadata(string json) { }
    public static void EveryplaySetTargetFPS(int fps) { }
    public static void EveryplaySetMotionFactor(int factor) { }
    public static void EveryplaySetMaxRecordingMinutesLength(int minutes) { }
    public static void EveryplaySetLowMemoryDevice(bool state) { }
    public static void EveryplaySetDisableSingleCoreDevices(bool state) { }
    public static bool EveryplayIsSupported() { return false; }
    public static bool EveryplayIsSingleCoreDevice() { return false; }
    public static int EveryplayGetUserInterfaceIdiom() { return 0; }
    public static bool EveryplayFaceCamIsVideoRecordingSupported() { return false; }
    public static bool EveryplayFaceCamIsAudioRecordingSupported() { return false; }
    public static bool EveryplayFaceCamIsHeadphonesPluggedIn() { return false; }
    public static bool EveryplayFaceCamIsSessionRunning() { return false; }
    public static bool EveryplayFaceCamIsRecordingPermissionGranted() { return false; }
    public static float EveryplayFaceCamAudioPeakLevel() { return 0f; }
    public static float EveryplayFaceCamAudioPowerLevel() { return 0f; }
    public static void EveryplayFaceCamSetMonitorAudioLevels(bool enabled) { }
    public static void EveryplayFaceCamSetRecordingMode(int mode) { }
    public static void EveryplayFaceCamSetAudioOnly(bool audioOnly) { }
    public static void EveryplayFaceCamSetPreviewVisible(bool visible) { }
    public static void EveryplayFaceCamSetPreviewScaleRetina(bool autoScale) { }
    public static void EveryplayFaceCamSetPreviewSideWidth(int width) { }
    public static void EveryplayFaceCamSetPreviewBorderWidth(int width) { }
    public static void EveryplayFaceCamSetPreviewPositionX(int x) { }
    public static void EveryplayFaceCamSetPreviewPositionY(int y) { }
    public static void EveryplayFaceCamSetPreviewBorderColor(float r, float g, float b, float a) { }
    public static void EveryplayFaceCamSetPreviewOrigin(int origin) { }
    public static void EveryplayFaceCamSetTargetTextureId(int textureId) { }
    public static void EveryplayFaceCamSetTargetTextureWidth(int textureWidth) { }
    public static void EveryplayFaceCamSetTargetTextureHeight(int textureHeight) { }
    public static void EveryplayFaceCamStartSession() { }
    public static void EveryplayFaceCamRequestRecordingPermission() { }
    public static void EveryplayFaceCamStopSession() { }
    public static void EveryplaySetThumbnailTargetTextureId(int textureId) { }
    public static void EveryplaySetThumbnailTargetTextureWidth(int textureWidth) { }
    public static void EveryplaySetThumbnailTargetTextureHeight(int textureHeight) { }
    public static void EveryplayTakeThumbnail() { }
}