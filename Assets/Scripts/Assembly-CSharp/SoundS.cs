using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public static class SoundS
{
    public static bool m_canPlaySounds = true;

    private static bool m_mixerPaused;

    public static GameObject m_mainCamera;

    public static DynamicArray<SoundC> m_components;

    public static GameObject m_listenerGO;

    public static FMOD_Listener m_listener;

    public static bool m_initialized;

    private static Hashtable m_combineSounds;

    public static void Initialize(GameObject _mainCamera)
    {
        FMOD_Listener fMOD_Listener = _mainCamera.GetComponent("FMOD_Listener") as FMOD_Listener;
        PsFMODManager.m_allowInit = true;
        fMOD_Listener.Initialize();
        FMODManager.Initialize();
        m_components = new DynamicArray<SoundC>();
        m_mainCamera = _mainCamera;
        SetListener(m_mainCamera.gameObject);
        m_combineSounds = new Hashtable();
        m_initialized = true;
    }

    public static void SetListener(GameObject _go, bool _forceZToZero = false)
    {
        m_listener = m_mainCamera.GetComponent("FMOD_Listener") as FMOD_Listener;
        PsFMODManager.m_listener = _go;
        PsFMODManager.m_forceListenerZToZero = _forceZToZero;
        m_listenerGO = _go;
        Debug.Log("Listener set to: " + _go.transform.name);
    }

    public static void PlaySingleShot(string _event, Vector3 _pos, float _volume = 1f)
    {
        if (m_canPlaySounds && m_initialized && !m_mixerPaused)
        {
            FMODManager.PlayOneShot("event:" + _event, _pos, _volume);
        }
    }

    public static void PlaySingleShotWithParameter(string _event, Vector3 _pos, string _parameter, float _value, float _volume = 1f)
    {
        if (m_canPlaySounds && m_initialized && !m_mixerPaused)
        {
            FMODManager.PlayOneShotWithParameter("event:" + _event, _pos, _parameter, _value, _volume);
        }
    }

    public static void PlaySound(SoundC _c, bool _forcePlay = false)
    {
        if ((m_canPlaySounds || _forcePlay) && m_initialized)
        {
            if (_c.isCombineSound)
            {
                _c.isPlaying = true;
            }
            else if (!_c.isPlaying)
            {
                FMODManager.ERRCHECK(_c.eventInstance.start());
                _c.isPlaying = true;
            }
            else
            {
                FMODManager.ERRCHECK(_c.eventInstance.setTimelinePosition(0));
            }
        }
    }

    public static void StopSound(SoundC _c)
    {
        if (_c.isCombineSound)
        {
            _c.isPlaying = false;
        }
        else if (_c.isPlaying)
        {
            FMODManager.ERRCHECK(_c.eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE));
            _c.isPlaying = false;
        }
    }

    public static void PauseSound(SoundC _c)
    {
        if (_c.isCombineSound && _c.isPlaying)
        {
            _c.isPaused = true;
        }
        else if (!_c.isPaused && _c.isPlaying)
        {
            FMODManager.ERRCHECK(_c.eventInstance.setPaused(true));
            _c.isPaused = true;
        }
    }

    public static void ResumeSound(SoundC _c)
    {
        if (_c.isCombineSound && _c.isPlaying)
        {
            _c.isPaused = false;
        }
        else if (_c.isPaused && _c.isPlaying)
        {
            FMODManager.ERRCHECK(_c.eventInstance.setPaused(false));
            _c.isPaused = false;
        }
    }

    public static void SetVolume(SoundC _c, float _volume)
    {
        FMODManager.ERRCHECK(_c.eventInstance.setVolume(_volume));
    }

    public static void SetSoundParameter(SoundC _c, string _parameterName, float _value)
    {
        if (m_initialized)
        {
            ParameterInstance parameterInstance = default(ParameterInstance);
            if (_c.parameterInstances == null)
            {
                _c.parameterInstances = new Hashtable();
            }
            if (!_c.parameterInstances.Contains(_parameterName))
            {
                _c.parameterInstances[_parameterName] = FMODManager.GetParameterInstance(_c.eventInstance, _parameterName);
            }
            parameterInstance = (ParameterInstance)_c.parameterInstances[_parameterName];
            FMODManager.SetParameter(parameterInstance, _value);
        }
    }

    private static void UpdatePosition(SoundC _c, Vector3 _pos)
    {
        if (!_c.eventInstance.isValid())
        {
            return;
        }
        FMOD.ATTRIBUTES_3D attributes = _pos.to3DAttributes();
        FMODManager.ERRCHECK(_c.eventInstance.set3DAttributes(attributes));
    }

    public static void SetPauseForAllSounds(bool _pause)
    {
        int aliveCount = m_components.m_aliveCount;
        for (int i = 0; i < aliveCount; i++)
        {
            int num = m_components.m_aliveIndices[i];
            SoundC c = m_components.m_array[num];
            if (_pause)
            {
                PauseSound(c);
            }
            else
            {
                ResumeSound(c);
            }
        }
    }

    public static void RemoveAllSounds()
    {
        if (m_initialized)
        {
            while (m_components.m_aliveCount > 0)
            {
                SoundC c = m_components.m_array[m_components.m_aliveIndices[0]];
                RemoveComponent(c);
            }
            FMODManager.BusStopAllEvents();
        }
    }

    public static void PauseMixer(bool _on)
    {
        if (_on != m_mixerPaused)
        {
            FMODManager.BusSetPause(_on);
            m_mixerPaused = _on;
        }
    }

    public static void MuteMixer(bool _mute)
    {
        FMODManager.BusSetMute(_mute);
    }

    public static void Update()
    {
        if (!m_initialized)
        {
            return;
        }
        int aliveCount = m_components.m_aliveCount;
        for (int i = 0; i < aliveCount; i++)
        {
            int num = m_components.m_aliveIndices[i];
            SoundC soundC = m_components.m_array[num];
            if (!soundC.isCombineSound)
            {
                TransformC p_TC = soundC.p_TC;
                if (soundC.forceAtListenerPosition)
                {
                    UpdatePosition(soundC, m_listenerGO.transform.position);
                }
                else if (p_TC.updatedPosition)
                {
                    UpdatePosition(soundC, p_TC.transform.position);
                }
            }
        }
        UpdateCombineSounds();
        m_components.Update();
    }

    public static void UpdateCombineSounds()
    {
        if (m_combineSounds.Count == 0)
        {
            return;
        }
        foreach (CombineSound value in m_combineSounds.Values)
        {
            int count = value.components.Count;
            int num = 0;
            int num2 = 0;
            float num3 = -1f;
            foreach (SoundC component in value.components)
            {
                if (component.isPlaying)
                {
                    num++;
                }
                if (component.isPaused)
                {
                    num2++;
                }
                if (component.isPlaying && !component.isPaused)
                {
                    float magnitude = (m_listenerGO.transform.position - component.p_TC.transform.position).magnitude;
                    if (magnitude < num3 || num3 < 0f)
                    {
                        num3 = magnitude;
                        value.nearestSoundC = component;
                    }
                }
            }
            if (!value.isPlaying && num > 0)
            {
                FMODManager.ERRCHECK(value.eventInstance.start());
                value.isPlaying = true;
            }
            if (value.isPlaying && num == 0)
            {
                FMODManager.ERRCHECK(value.eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE));
                value.isPlaying = false;
            }
            if (!value.isPlaying)
            {
                continue;
            }
            if (!value.isPaused && num2 == count)
            {
                FMODManager.ERRCHECK(value.eventInstance.setPaused(true));
                value.isPaused = true;
            }
            if (value.isPaused && num2 == 0)
            {
                FMODManager.ERRCHECK(value.eventInstance.setPaused(false));
                value.isPaused = false;
            }
            if (value.isPaused)
            {
                continue;
            }
            float num4 = 1f - ToolBox.getPositionBetween(num3, value.minDist, value.maxDist);
            if (!value.isOutOfRange)
            {
                if (num4 > 0f)
                {
                    FMODManager.ERRCHECK(value.eventInstance.setVolume(value.volume * num4));
                    Vector3 position = m_listenerGO.transform.position;
                    if (PsFMODManager.m_forceListenerZToZero)
                    {
                        position.z = 0f;
                    }
                    FMOD.ATTRIBUTES_3D attributes = position.to3DAttributes();
                    FMODManager.ERRCHECK(value.eventInstance.set3DAttributes(attributes));
                }
                else
                {
                    FMODManager.ERRCHECK(value.eventInstance.setPaused(true));
                    value.isOutOfRange = true;
                }
            }
            else if (num4 > 0f)
            {
                FMODManager.ERRCHECK(value.eventInstance.setPaused(false));
                FMODManager.ERRCHECK(value.eventInstance.setVolume(value.volume * num4));
                value.isOutOfRange = false;
            }
        }
    }

    public static CombineSound GetCombineSoundWithKey(string _key)
    {
        if (m_combineSounds.Contains(_key))
        {
            return m_combineSounds[_key] as CombineSound;
        }
        return null;
    }

    public static void RemoveCombineSoundWithTag(string _tag)
    {
        if (m_combineSounds.Contains(_tag))
        {
            CombineSound combineSound = m_combineSounds[_tag] as CombineSound;
            Debug.Log("Combine sound removed");
            combineSound.components.Clear();
            combineSound.components = null;
            if (combineSound.eventInstance.isValid())
            {
                FMODManager.ERRCHECK(combineSound.eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE));
                FMODManager.ReleaseEvent(combineSound.eventInstance);
                combineSound.eventInstance = default(EventInstance);
            }
            m_combineSounds.Remove(_tag);
        }
    }

    public static SoundC AddComponent(TransformC _parentTC, string _event, float _volume = 1f, bool _forceAtListenersPosition = false)
    {
        if (m_initialized)
        {
            SoundC soundC = m_components.AddItem();
            soundC.name = _event;
            soundC.eventInstance = FMODManager.NewEvent("event:" + _event, _parentTC.transform.position, false, _volume);
            soundC.p_TC = _parentTC;
            soundC.forceAtListenerPosition = _forceAtListenersPosition;
            soundC.isCombineSound = false;
            soundC.isPaused = false;
            soundC.isPlaying = false;
            EntityManager.AddComponentToEntity(_parentTC.p_entity, soundC);
            return soundC;
        }
        return null;
    }

    public static SoundC AddCombineSoundComponent(TransformC _parentTC, string _key, string _event, float _volume = 1f)
    {
        if (m_initialized)
        {
            CombineSound combineSound = GetCombineSoundWithKey(_key);
            if (combineSound == null)
            {
                Debug.Log("Combine sound created: " + _event);
                combineSound = new CombineSound();
                combineSound.components = new List<SoundC>();
                combineSound.eventInstance = FMODManager.NewEvent("event:" + _event, m_listenerGO.transform.position, false, _volume);
                combineSound.volume = _volume;
                combineSound.isOutOfRange = false;
                EventDescription description = default(EventDescription);
                combineSound.eventInstance.getDescription(out description);
                description.getMinimumDistance(out combineSound.minDist);
                description.getMaximumDistance(out combineSound.maxDist);
                m_combineSounds.Add(_key, combineSound);
            }
            SoundC soundC = m_components.AddItem();
            soundC.eventInstance = combineSound.eventInstance;
            soundC.p_TC = _parentTC;
            soundC.forceAtListenerPosition = false;
            soundC.isCombineSound = true;
            soundC.combineSoundKey = _key;
            soundC.isPaused = false;
            soundC.isPlaying = false;
            EntityManager.AddComponentToEntity(_parentTC.p_entity, soundC);
            combineSound.components.Add(soundC);
            return soundC;
        }
        return null;
    }

    public static void RemoveComponent(SoundC _c)
    {
        if (!m_initialized)
        {
            return;
        }
        if (_c.p_entity == null)
        {
            Debug.LogWarning("Trying to remove component that has already been removed");
            return;
        }
        if (_c.isCombineSound)
        {
            CombineSound combineSound = m_combineSounds[_c.combineSoundKey] as CombineSound;
            if (combineSound.nearestSoundC == _c)
            {
                combineSound.nearestSoundC = null;
            }
            combineSound.components.Remove(_c);
            if (combineSound.components.Count == 0)
            {
                RemoveCombineSoundWithTag(_c.combineSoundKey);
            }
        }
        if (!_c.isCombineSound && _c.eventInstance.isValid())
        {
            StopSound(_c);
            FMODManager.ReleaseEvent(_c.eventInstance);
            _c.eventInstance = default(EventInstance);
        }
        EntityManager.RemoveComponentFromEntity(_c);
        m_components.RemoveItem(_c);
    }
}