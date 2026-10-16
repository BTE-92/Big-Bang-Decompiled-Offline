using System.Collections.Generic;

public class PsUnlockableCategory : PsUnlockable
{
	public List<PsUnlockable> m_items;

	public PsUnlockableCategory(string _categoryName)
		: base(PsUnlockableType.Undefined, _categoryName)
	{
		m_items = new List<PsUnlockable>();
	}
}
