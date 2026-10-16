using System.Collections.Generic;

public class PsPlayerProfilerData
{
	public class ProfilerVehicleData
	{
		public string m_vehicleName;

		public int m_skill;

		public int m_preference;

		public ProfilerVehicleData(string _vehicleName, int _skill, int _preference)
		{
			m_vehicleName = _vehicleName;
			m_skill = _skill;
			m_preference = _preference;
		}
	}

	public int m_itemLevel;

	public List<ProfilerVehicleData> m_skills = new List<ProfilerVehicleData>();

	public Dictionary<string, float> m_subgenrePrefs = new Dictionary<string, float>();
}
