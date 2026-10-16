using UnityEngine;

public class TriggeredMovingPlatform : MovingPlatformBase
{
	private bool m_active;

	private bool m_move;

	private bool m_set;

	private bool m_setActive;

	private GameObject m_light;

	private Color m_inActiveColor;

	private Color m_activeColor;

	private Vector2 m_moveBeginPos;

	private Material m_lightMaterial;

	private float m_ticks;

	public TriggeredMovingPlatform(GraphElement _graphElement)
		: base(_graphElement, "TriggeredMovingPlatformPrefab")
	{
		if (!m_minigame.m_editing)
		{
			ChipmunkProS.AddCollisionHandler(m_cmb, PlatformCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)3, true, true, true);
		}
		m_childNode.m_maxDistanceFromParent = 2000f;
		PrefabC prefabC = PrefabS.AddComponent(m_platformTC, new Vector3(0f, 0f, -55f) + GetZBufferBias(), m_mainPrefab.transform.Find("ActiveLight").gameObject);
		m_light = prefabC.p_gameObject;
		m_lightMaterial = m_light.GetComponent<Renderer>().material;
		m_inActiveColor = new Color(9f / 85f, 0.1254902f, 0.101960786f);
		m_activeColor = new Color(16f / 51f, 1f, 2f / 85f);
		m_lightMaterial.color = m_inActiveColor;
		m_ticks = 0f;
		m_graphElement.m_isRotateable = true;
		m_active = (m_move = (m_set = (m_setActive = false)));
		m_moveBeginPos = m_startPos;
	}

	public override void Destroy()
	{
		Object.Destroy(m_lightMaterial);
		base.Destroy();
	}

	public override void Update()
	{
		base.Update();
		if (m_minigame.m_editing || !m_minigame.m_gameStarted)
		{
			return;
		}
		if (m_ghostEntity != null)
		{
			DestroyEndGhost();
		}
		if (m_setActive)
		{
			m_ticks += 1f * Main.m_timeScale;
			if (m_ticks >= 15f)
			{
				if (!m_active)
				{
					m_set = false;
				}
				m_active = true;
				m_ticks = 15f;
				m_setActive = false;
			}
		}
		else if (!m_setActive && !m_move)
		{
			m_ticks -= 1f * Main.m_timeScale;
			if (m_ticks <= 0f)
			{
				m_active = false;
				m_ticks = 0f;
			}
		}
		if (m_active && !m_move && !m_set)
		{
			if ((ChipmunkProWrapper.ucpBodyGetPos(m_cmb.body) - (Vector2)m_childNode.m_position).sqrMagnitude < (ChipmunkProWrapper.ucpBodyGetPos(m_cmb.body) - m_startPos).sqrMagnitude)
			{
				m_moveDir = m_startPos - (Vector2)m_childNode.m_position;
				m_moveBeginPos = m_childNode.m_position;
				m_rotMultiplier = 1f;
			}
			else
			{
				m_moveDir = (Vector2)m_childNode.m_position - m_startPos;
				m_moveBeginPos = m_startPos;
				m_rotMultiplier = -1f;
			}
			m_moveDir = m_moveDir.normalized;
			m_currentTime = 0f;
			m_currentValue = 0f;
			m_move = true;
			m_set = true;
			SoundS.PlaySingleShot("/InGame/Units/ElevatorActivated", m_cmb.TC.transform.position);
		}
		if (m_move)
		{
			m_currentTime += Main.m_gameDeltaTime;
			if (m_currentTime >= m_duration)
			{
				m_currentTime = m_duration;
			}
			m_currentValue = TweenS.tween(TweenStyle.QuadInOut, m_currentTime, m_duration, 0f, m_moveMagnitude);
		}
		ChipmunkProWrapper.ucpBodySetPos(m_cmb.body, m_moveBeginPos + m_moveDir * m_currentValue);
		TransformS.SetGlobalRotation(m_gearTC, m_gearTC.transform.rotation.eulerAngles + new Vector3(0f, 0f, ChipmunkProWrapper.ucpBodyGetVel(m_cmb.body).magnitude * Main.m_gameDeltaTime) * m_rotMultiplier);
		if (m_currentTime == m_duration)
		{
			m_move = false;
		}
		if (m_active || m_setActive || m_move)
		{
			TurnLight(true);
		}
		else
		{
			TurnLight(false);
		}
	}

	public void TurnLight(bool _on)
	{
		m_lightMaterial.color = ((!_on) ? m_inActiveColor : m_activeColor);
	}

	private void PlatformCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB];
		UnitC unitC = chipmunkBodyC.customComponent as UnitC;
		if (unitC != null && unitC.m_unit != null && (_phase == ucpCollisionPhase.Begin || _phase == ucpCollisionPhase.Persist))
		{
			m_setActive = true;
		}
	}
}
