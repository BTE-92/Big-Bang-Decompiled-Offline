using UnityEngine;

public class EffectMotorcycleExhausts : MonoBehaviour
{
	[HideInInspector]
	public Motorcycle vehicle;

	public EffectExhaustGroup[] exhausts;

	public float exhaustProcRate = 0.5f;

	private bool gasTriggered;

	private float exhaustTimer;

	private void Start()
	{
		if (PsState.m_activeMinigame != null)
		{
			vehicle = PsState.m_activeMinigame.m_playerUnit as Motorcycle;
		}
	}

	private void RearExhaust()
	{
		int num = Random.Range(0, exhausts.Length);
		if (exhausts.Length > 0)
		{
			ParticleSystem[] singleEffects = exhausts[num].singleEffects;
			foreach (ParticleSystem particleSystem in singleEffects)
			{
				particleSystem.Play();
			}
			if (exhausts[num].playPuffAudio)
			{
				SoundS.PlaySingleShot("/Ingame/Vehicles/EnginePopping_Dirtbike", Vector2.zero);
			}
			else
			{
				SoundS.PlaySingleShot("/Ingame/Vehicles/EngineBackfire_Dirtbike", Vector2.zero);
			}
		}
	}

	private void Update()
	{
		if (PsState.m_activeMinigame == null)
		{
			return;
		}
		if (Controller.GetButtonState("Throttle") == ControllerButtonState.OFF && Controller.GetButtonState("Reverse") == ControllerButtonState.OFF)
		{
			gasTriggered = false;
		}
		if ((Controller.GetButtonState("Throttle") == ControllerButtonState.ON && !gasTriggered) || (Controller.GetButtonState("Reverse") == ControllerButtonState.ON && !gasTriggered))
		{
			float value = Random.value;
			if (value < exhaustProcRate)
			{
				RearExhaust();
			}
			gasTriggered = true;
		}
		else
		{
			if (Controller.GetButtonState("Throttle") != ControllerButtonState.ON && Controller.GetButtonState("Reverse") != ControllerButtonState.ON)
			{
				return;
			}
			if (exhaustTimer <= 0f)
			{
				exhaustTimer = Random.Range(1f, 4f);
				if (exhausts != null)
				{
					RearExhaust();
				}
			}
			else
			{
				exhaustTimer -= Main.m_gameDeltaTime;
			}
		}
	}
}
