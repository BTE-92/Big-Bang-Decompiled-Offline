using UnityEngine;

public class EffectOffroadCarEngineT1 : MonoBehaviour
{
	[HideInInspector]
	public OffroadCar vehicle;

	public float defaultTurnRate = 25f;

	private void Start()
	{
		if (PsState.m_activeMinigame != null)
		{
			vehicle = PsState.m_activeMinigame.m_playerUnit as OffroadCar;
		}
	}

	private void Update()
	{
		if (PsState.m_activeMinigame != null)
		{
			if (vehicle != null)
			{
				base.transform.Rotate(new Vector3(0f, 0f, vehicle.m_currentRPM / 20f));
			}
			else
			{
				base.transform.Rotate(new Vector3(0f, 0f, defaultTurnRate));
			}
		}
	}
}
