public class MinigameSearchParametres
{
	public string m_playerUnitFilter;

	public string[] m_items;

	public PsGameMode m_gameMode;

	public string[] m_features;

	public PsGameDifficulty m_difficulty;

	public MinigameSearchParametres(string _playerUnitFilter = null, string[] _items = null, PsGameMode _gameMode = PsGameMode.Any, string[] _features = null, PsGameDifficulty _difficulty = PsGameDifficulty.Any)
	{
		m_playerUnitFilter = _playerUnitFilter;
		m_items = _items;
		m_gameMode = _gameMode;
		m_features = _features;
		m_difficulty = _difficulty;
	}
}
