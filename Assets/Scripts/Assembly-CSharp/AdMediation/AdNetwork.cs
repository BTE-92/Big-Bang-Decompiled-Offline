using System;

namespace AdMediation
{
	public class AdNetwork
	{
		private bool m_enabled;

		private int m_priority;

		public Func<bool> adsAvailable;

		public Action<string, Action<AdResult>> showAd;

		public Action initialize;

		public string networkName;

		public bool enabled
		{
			get
			{
				return m_enabled;
			}
		}

		public int priority
		{
			get
			{
				return m_priority;
			}
		}

		public AdNetwork(string _name, int _priority, Action _initialize, Func<bool> _adsAvailable, Action<string, Action<AdResult>> _showAd, bool _enabled)
		{
			networkName = _name;
			adsAvailable = _adsAvailable;
			showAd = _showAd;
			m_priority = _priority;
			initialize = _initialize;
			m_enabled = _enabled;
		}

		public void Enable(bool _value)
		{
			m_enabled = _value;
		}

		public void SetPriority(int _value)
		{
			m_priority = _value;
		}

		public void SetConfig(NetworkConfig _cfg)
		{
			m_enabled = _cfg.enabled;
			m_priority = _cfg.priority;
		}
	}
}
