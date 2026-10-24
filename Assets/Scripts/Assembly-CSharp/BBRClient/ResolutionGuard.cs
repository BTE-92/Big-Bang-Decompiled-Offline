using UnityEngine;

// Windows: a full screen game drawn at a lower resolution than the monitor is stretched by the screen and looks pixelated
// (for instance 1600x900 chosen in the launcher dialog on a 1920x1080 monitor). At start-up, full screen is always switched to
// the monitor's own resolution. Windowed mode is left alone.
public static class ResolutionGuard
{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Fix()
    {
        try
        {
            if (!Screen.fullScreen)
            {
                return;
            }
            Resolution desktop = Screen.currentResolution;
            if (desktop.width > 0 && desktop.height > 0 && (Screen.width != desktop.width || Screen.height != desktop.height))
            {
                Screen.SetResolution(desktop.width, desktop.height, FullScreenMode.FullScreenWindow);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("ResolutionGuard: " + e.Message);
        }
    }
#endif
}
