public class PsMultiTaskAchievement : Achievement
{
	public enum Tasks
	{
		None = 0,
		Task1 = 1,
		Task2 = 2,
		Task3 = 4
	}

	private bool m_task1;

	private bool m_task2;

	public PsMultiTaskAchievement(string _id, string _name, string _description)
		: base(_id)
	{
		m_name = _name;
		m_description = _description;
		m_value = 0;
		m_target = 3;
		CheckFlags();
	}

	public override void IncrementProgress(int _value)
	{
		if (!base.completed)
		{
			if (_value == 1 && (m_value & 1) == 0)
			{
				base.IncrementProgress(_value);
			}
			else if (_value == 2 && (m_value & 2) == 0)
			{
				base.IncrementProgress(_value);
			}
		}
	}

	public override void Refresh()
	{
		base.Refresh();
	}
}
