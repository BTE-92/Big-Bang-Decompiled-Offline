using System;

public static class DefaultSignalSequence
{
	public static void StartDefaultSignalSequence(Action _endSequenceCallback)
	{
		GameSceneEffectManager.CreateTVChannels("TVChannelFlicksMat_Material", 4);
		GameSceneEffectManager.m_endSequenceCallback = _endSequenceCallback;
		GameSceneEffectManager.SetChannel(0);
		GameSceneEffectManager.SetEffects(true);
		TimerS.AddComponent(GameSceneEffectManager.m_utilityEntity, string.Empty, 2f, 0f, false, OtherChannelGameNoise);
	}

	private static void OtherChannelGameNoise(TimerC _c)
	{
		TimerS.RemoveComponent(_c);
		GameSceneEffectManager.SetChannel(-1);
		GameSceneEffectManager.SetEffects(false);
		TimerS.AddComponent(GameSceneEffectManager.m_utilityEntity, string.Empty, 1f, 0f, false, OtherTheEnd);
	}

	private static void OtherTheEnd(TimerC _c)
	{
		TimerS.RemoveComponent(_c);
		GameSceneEffectManager.m_endSequenceCallback();
	}

	public static void StartDefaultSignalEndSequence(Action _endSequenceCallback)
	{
		GameSceneEffectManager.m_endSequenceCallback = _endSequenceCallback;
		GameSceneEffectManager.SetEffects(true);
		TimerS.AddComponent(GameSceneEffectManager.m_utilityEntity, string.Empty, 2f, 0f, false, OtherChannelGameNoiseEND);
	}

	private static void OtherChannelGameNoiseEND(TimerC _c)
	{
		TimerS.RemoveComponent(_c);
		GameSceneEffectManager.SetChannel(0);
		TimerS.AddComponent(GameSceneEffectManager.m_utilityEntity, string.Empty, 1f, 0f, false, OtherTheEnd2);
	}

	private static void OtherTheEnd2(TimerC _c)
	{
		TimerS.RemoveComponent(_c);
		GameSceneEffectManager.m_endSequenceCallback();
	}
}
