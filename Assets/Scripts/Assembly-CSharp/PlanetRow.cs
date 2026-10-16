using System.Collections.Generic;

public class PlanetRow
{
	public int m_rowId;

	public List<PsPlanetNode> m_nodes;

	public PlanetRow(int _rowId)
	{
		m_rowId = _rowId;
		m_nodes = new List<PsPlanetNode>();
	}
}
