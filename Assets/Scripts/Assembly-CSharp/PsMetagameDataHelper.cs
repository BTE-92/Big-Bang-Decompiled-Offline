using System.Collections.Generic;

public class PsMetagameDataHelper
{
	public int m_id;

	public string m_name;

	public float m_x;

	public float m_y;

	public MetagameNodeData m_metagameNodeData;

	public List<PsMetagameDataHelper> m_inputs;

	public List<PsMetagameDataHelper> m_outputs;

	public PsMetagameDataHelper()
	{
		m_inputs = new List<PsMetagameDataHelper>();
		m_outputs = new List<PsMetagameDataHelper>();
	}
}
