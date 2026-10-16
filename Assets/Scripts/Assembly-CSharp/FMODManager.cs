using FMOD;
using FMOD.Studio;
using UnityEngine;

public static class FMODManager
{
	public static FMOD.System m_lowLevelSystem;

	public static FMOD_StudioSystem m_system;

	public static Bus m_bus;

	public static void Initialize()
	{
		m_system = FMOD_StudioSystem.instance;
		ERRCHECK(m_system.System.getBus("bus:/", out m_bus));
		ERRCHECK(m_system.System.getLowLevelSystem(out m_lowLevelSystem));
		uint bufferlength;
		int numbuffers;
		ERRCHECK(m_lowLevelSystem.getDSPBufferSize(out bufferlength, out numbuffers));
		Debug.Log("FMOD Initialized with DSP Buffer: " + bufferlength + "/" + numbuffers);
	}

	public static void PlayOneShot(string _event, Vector3 _position, float _volume = 1f)
	{
		EventInstance eventInstance = m_system.GetEvent(_event);
		FMOD.Studio.ATTRIBUTES_3D attributes = _position.to3DAttributes();
		ERRCHECK(eventInstance.set3DAttributes(attributes));
		ERRCHECK(eventInstance.setVolume(_volume));
		ERRCHECK(eventInstance.start());
		ERRCHECK(eventInstance.release());
	}

	public static void PlayOneShotWithParameter(string _event, Vector3 _position, string _parameter, float _value, float _volume = 1f)
	{
		EventInstance eventInstance = m_system.GetEvent(_event);
		ParameterInstance parameterInstance = GetParameterInstance(eventInstance, _parameter);
		SetParameter(parameterInstance, _value);
		FMOD.Studio.ATTRIBUTES_3D attributes = _position.to3DAttributes();
		ERRCHECK(eventInstance.set3DAttributes(attributes));
		ERRCHECK(eventInstance.setVolume(_volume));
		ERRCHECK(eventInstance.start());
		ERRCHECK(eventInstance.release());
	}

	public static EventInstance NewEvent(string _event, Vector3 _position, bool _startImmediately = true, float _volume = 1f)
	{
		EventInstance eventInstance = m_system.GetEvent(_event);
        if (eventInstance == null)
        {
            Debug.LogError("FMOD event not found: " + _event);
            return null;
        }
        FMOD.Studio.ATTRIBUTES_3D attributes = _position.to3DAttributes();
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
		if (_eventInstance != null)
		{
			ERRCHECK(_eventInstance.stop(STOP_MODE.IMMEDIATE));
			ERRCHECK(_eventInstance.release());
		}
	}

	public static void SetParameter(ParameterInstance _parameterInstance, float _value)
	{
		ERRCHECK(_parameterInstance.setValue(_value));
	}

	public static ParameterInstance GetParameterInstance(EventInstance _eventInstance, string _parameterName)
	{
		ParameterInstance instance = null;
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
		ERRCHECK(m_bus.stopAllEvents(STOP_MODE.ALLOWFADEOUT));
	}

	public static void ReleaseAllInstances()
	{
	}

	public static void ERRCHECK(RESULT result)
	{
		if (result != RESULT.OK)
		{
			Debug.LogError("FMOD Error (" + result.ToString() + "): " + Error.String(result));
		}
	}
}
