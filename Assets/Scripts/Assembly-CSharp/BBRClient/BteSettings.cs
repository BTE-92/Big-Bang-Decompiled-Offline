using System;
using BBR249Server.Api;
using UnityEngine;

// Settings of the built-in friend BTE (admin panel): how many hours pass between two challenges that BTE sends to the player.
public static class BteSettings
{
    public const string PrefKey = "bbr_bte_hours";

    public static int Hours
    {
        get
        {
            int hours = BotPlayer.MinHoursBetweenChallenges * 2;
            try { hours = PlayerPrefs.GetInt(PrefKey, hours); } catch (Exception) { }
            return Clamp(hours);
        }
        set
        {
            int hours = Clamp(value);
            try
            {
                PlayerPrefs.SetInt(PrefKey, hours);
                PlayerPrefs.Save();
            }
            catch (Exception) { }
            Apply();
        }
    }

    public static int Clamp(int hours)
    {
        return Math.Max(BotPlayer.MinHoursBetweenChallenges, Math.Min(BotPlayer.MaxHoursBetweenChallenges, hours));
    }

    // gives the saved value to the embedded server (called when the server starts and when the value changes)
    public static void Apply()
    {
        BotPlayer.HoursBetweenChallenges = Hours;
    }
}
