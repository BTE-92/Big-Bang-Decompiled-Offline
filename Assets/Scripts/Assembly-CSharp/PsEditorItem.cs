public class PsEditorItem : PsUnlockable
{
	public string m_graphNodeClassName;

	public int m_maxAmount;

	public int m_complexity;

	public int m_orderIndex;

	public PsEditorItem(string _name)
		: base(PsUnlockableType.Unit, _name)
	{
		m_orderIndex = 0;
	}
}
