using System.Collections.Generic;

public static class PsPlanetManager
{
	public static List<PsPlanet> m_planets = new List<PsPlanet>();

	private static PsPlanet m_currentPlanet;

	public static PsTimedEvents m_timedEvents;

	public static PsPlanet GetCurrentPlanet()
	{
		return m_currentPlanet;
	}

	public static void AddPlanet(PsPlanet _planet)
	{
		Debug.LogWarning("Planet Added");
		if (!m_planets.Contains(_planet))
		{
			m_planets.Add(_planet);
		}
		if (m_currentPlanet != null)
		{
			m_currentPlanet.DestroySpaceScene();
			m_currentPlanet = null;
		}
		m_currentPlanet = _planet;
	}

	public static void Update()
	{
		if (m_currentPlanet != null)
		{
			m_currentPlanet.Update();
		}
	}
}
