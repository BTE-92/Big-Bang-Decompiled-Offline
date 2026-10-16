public class PsUnlockable
{
	public PsUnlockableType m_type;

	public PsUnlockable m_container;

	public string m_identifier;

	public string m_name;

	public string m_description;

	public string m_iconImage;

	public int m_itemLevel;

	public bool m_unlocked;

	public string m_tutorialIdentifier;

	public PsUnlockable(PsUnlockableType _itemType, string _name)
	{
		m_type = _itemType;
		m_name = _name;
	}
}
