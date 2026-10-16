using UnityEngine;

public class EffectOffroadCarEngineT3 : MonoBehaviour
{
	[HideInInspector]
	public OffroadCar vehicle;

	public Transform beltRotator;

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
				beltRotator.Rotate(new Vector3(0f, 0f, vehicle.m_currentRPM / 20f));
			}
			else
			{
				beltRotator.Rotate(new Vector3(0f, 0f, defaultTurnRate));
			}
		}
	}
}
