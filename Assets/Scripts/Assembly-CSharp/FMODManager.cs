using System.Runtime.InteropServices;
using FMOD;
using FMOD.Studio;
using UnityEngine;

public static class FMODManager
{
    public static FMOD.System m_lowLevelSystem;

    public static FMOD_StudioSystem m_system;

    public static Bus m_bus;

#if UNITY_WEBGL && !UNITY_EDITOR
    // Resumes FMOD's browser AudioContext from the first user gesture (Assets/Plugins/WebGL/BBRWebGL.jslib).
    [DllImport("__Internal")]
    private static extern void FMOD_InstallAudioUnlock();
#endif

    public static void Initialize()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        FMOD_InstallAudioUnlock();
#endif
        m_system = FMOD_StudioSystem.instance;
        ERRCHECK(m_system.System.getBus("bus:/", out m_bus));
        ERRCHECK(m_system.System.getLowLevelSystem(out m_lowLevelSystem));
        uint bufferlength;
        int numbuffers;
        ERRCHECK(m_lowLevelSystem.getDSPBufferSize(out bufferlength, out numbuffers));
        UnityEngine.Debug.Log("FMOD Initialized with DSP Buffer: " + bufferlength + "/" + numbuffers);
    }

    public static void PlayOneShot(string _event, Vector3 _position, float _volume = 1f)
    {
        EventInstance eventInstance = m_system.GetEvent(_event);
        if (!eventInstance.isValid())
        {
            return;
        }
        FMOD.ATTRIBUTES_3D attributes = _position.to3DAttributes();
        ERRCHECK(eventInstance.set3DAttributes(attributes));
        ERRCHECK(eventInstance.setVolume(_volume));
        ERRCHECK(eventInstance.start());
        ERRCHECK(eventInstance.release());
    }

    public static void PlayOneShotWithParameter(string _event, Vector3 _position, string _parameter, float _value, float _volume = 1f)
    {
        EventInstance eventInstance = m_system.GetEvent(_event);
        if (!eventInstance.isValid())
        {
            return;
        }
        ParameterInstance parameterInstance = GetParameterInstance(eventInstance, _parameter);
        SetParameter(parameterInstance, _value);
        FMOD.ATTRIBUTES_3D attributes = _position.to3DAttributes();
        ERRCHECK(eventInstance.set3DAttributes(attributes));
        ERRCHECK(eventInstance.setVolume(_volume));
        ERRCHECK(eventInstance.start());
        ERRCHECK(eventInstance.release());
    }

    public static EventInstance NewEvent(string _event, Vector3 _position, bool _startImmediately = true, float _volume = 1f)
    {
        EventInstance eventInstance = m_system.GetEvent(_event);
        if (!eventInstance.isValid())
        {
            UnityEngine.Debug.LogError("FMOD event not found: " + _event);
            return default(EventInstance);
        }
        FMOD.ATTRIBUTES_3D attributes = _position.to3DAttributes();
        ERRCHECK(eventInstance.set3DAttributes(attributes));
        ERRCHECK(eventInstance.setVolume(_volume));
        if (_startImmediately)
        {
            ERRCHECK(eventInstance.start());
        }
        return eventInstance;
    }

    public static void ReleaseEvent(EventInstance _eventInstance)
    {
        if (_eventInstance.isValid())
        {
            ERRCHECK(_eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE));
            ERRCHECK(_eventInstance.release());
        }
    }

    public static void SetParameter(ParameterInstance _parameterInstance, float _value)
    {
        ERRCHECK(_parameterInstance.setValue(_value));
    }

    public static ParameterInstance GetParameterInstance(EventInstance _eventInstance, string _parameterName)
    {
        ParameterInstance instance = default(ParameterInstance);
        ERRCHECK(_eventInstance.getParameter(_parameterName, out instance));
        return instance;
    }

    public static void BusSetPause(bool _pause)
    {
        ERRCHECK(m_bus.setPaused(_pause));
    }

    public static void BusSetMute(bool _mute)
    {
        ERRCHECK(m_bus.setMute(_mute));
    }

    public static void BusStopAllEvents()
    {
        ERRCHECK(m_bus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT));
    }

    public static void ReleaseAllInstances()
    {
    }

    public static void ERRCHECK(RESULT result)
    {
        if (result != RESULT.OK)
        {
            UnityEngine.Debug.LogError("FMOD Error (" + result.ToString() + "): " + Error.String(result));
        }
    }
}