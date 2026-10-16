using UnityEngine;

public class EffectBoost : MonoBehaviour
{
	public enum BoostDirection
	{
		Left = 0,
		Right = 1,
		Both = 2
	}

	public ParticleSystem boostIdle;

	public ParticleSystem boostActiveLeft;

	public ParticleSystem boostActiveRight;

	public ParticleSystem boostGain;

	private bool m_activeLeft;

	private bool m_activeRight;

	private bool m_idle;

	private bool m_drop;

	public void GainBoost()
	{
		boostGain.Play();
	}

	public void ActivateBoost(BoostDirection boostDirection)
	{
		if (boostDirection == BoostDirection.Left && !m_activeLeft)
		{
			boostIdle.Stop();
			boostActiveLeft.Play();
			m_activeRight = false;
			m_activeLeft = true;
			m_idle = false;
			m_drop = false;
		}
		else if (boostDirection == BoostDirection.Right && !m_activeRight)
		{
			boostIdle.Stop();
			boostActiveRight.Play();
			m_activeRight = true;
			m_activeLeft = false;
			m_idle = false;
			m_drop = false;
		}
		else if (boostDirection == BoostDirection.Both)
		{
			IdleBoost();
		}
	}

	public void IdleBoost()
	{
		if (!m_idle)
		{
			m_activeRight = false;
			m_activeLeft = false;
			m_idle = true;
			m_drop = false;
			boostIdle.Play();
			boostActiveRight.Stop();
			boostActiveLeft.Stop();
		}
	}

	public void DropBoost()
	{
		if (!m_drop)
		{
			m_activeRight = false;
			m_activeLeft = false;
			m_idle = false;
			m_drop = true;
			boostIdle.Stop();
			boostActiveRight.Stop();
			boostActiveLeft.Stop();
		}
	}
}
