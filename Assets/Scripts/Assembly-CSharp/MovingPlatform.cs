using UnityEngine;

public class MovingPlatform : MovingPlatformBase
{
	private float m_stopTicks;

	public MovingPlatform(GraphElement _graphElement)
		: base(_graphElement, "MovingPlatformPrefab")
	{
		m_graphElement.m_isRotateable = true;
		m_stopTicks = 35f;
	}

	public override void Update()
	{
		base.Update();
		if (m_minigame.m_editing || !m_minigame.m_gameStarted)
		{
			return;
		}
		if (m_stopTicks > 0f)
		{
			m_stopTicks -= 1f * Main.m_timeScale;
		}
		if (m_ghostEntity != null)
		{
			DestroyEndGhost();
		}
		if (m_currentTime == -1f && m_stopTicks <= 0f)
		{
			m_startPos = m_cmb.TC.transform.position;
			m_currentTime = 0f;
		}
		else if (m_currentValue == m_moveMagnitude * m_rotMultiplier)
		{
			m_currentTime = -1f;
			m_rotMultiplier *= -1f;
			m_stopTicks = 35f;
		}
		if (m_currentTime != -1f)
		{
			m_currentTime += Main.m_gameDeltaTime;
			if (m_currentTime >= m_duration)
			{
				m_currentTime = m_duration;
			}
			m_currentValue = TweenS.tween(TweenStyle.QuadInOut, m_currentTime, m_duration, 0f, m_moveMagnitude * m_rotMultiplier);
		}
		float magnitude = ChipmunkProWrapper.ucpBodyGetVel(m_cmb.body).magnitude;
		ChipmunkProWrapper.ucpBodySetPos(m_cmb.body, m_startPos + m_moveDir * m_currentValue);
		TransformS.SetGlobalRotation(m_gearTC, m_gearTC.transform.rotation.eulerAngles + new Vector3(0f, 0f, magnitude * Main.m_gameDeltaTime) * m_rotMultiplier);
	}
}
