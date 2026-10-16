namespace Server
{
	public static class ResetFlow
	{
		private static bool m_active;

		public static bool IsActive
		{
			get
			{
				return m_active;
			}
		}

		public static void Start(bool removeOld = true)
		{
			m_active = true;
			TouchAreaS.Disable();
			PsMetagamePlayerData.m_playerData.Clear();
			PsMetagameManager.m_playerStats.upgrades.Clear();
			PsMetagameManager.m_playerStats.boosters.Clear();
			if (removeOld)
			{
				Player.RemovePlayer(RemoveOk, RemoveFailed);
			}
			else
			{
				RemoveOk(null);
			}
		}

		private static void RemoveOk(HttpC _c)
		{
			PsPlanetManager.GetCurrentPlanet().ResetLocalProgression();
			KeyChain.SetUserId(string.Empty);
			KeyChain.SetGameCenterId(string.Empty);
			PlayerPrefsX.DeleteAll();
			TouchAreaS.Enable();
			ServerManager.m_playerAuthenticated = false;
			m_active = false;
			Main.m_currentGame.m_sceneManager.ChangeScene(new StartupScene("StartupScene"));
		}

		private static void RemoveFailed(HttpC _c)
		{
			m_active = false;
			Player.RemovePlayer(RemoveOk, RemoveFailed);
		}
	}
}
