using System.Collections;

public class PsUpgradeableEditorItem : PsEditorItem
{
	public Hashtable m_upgradeValues;

	public int m_upgradeSteps;

	public string[] m_upgradePrices;

	public string m_rentName;

	public string m_rentButton;

	public PsUpgradeableEditorItem(string _name)
		: base(_name)
	{
	}
}
