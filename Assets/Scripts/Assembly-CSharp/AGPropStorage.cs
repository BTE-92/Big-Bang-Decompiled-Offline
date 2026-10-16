using System.Collections.Generic;
using UnityEngine;

public class AGPropStorage : MonoBehaviour
{
	public float m_density = 0.5f;

	public GameObject[] m_propLibrary;

	[HideInInspector]
	public List<AGPropSettings> m_propSettingsLookup = new List<AGPropSettings>();

	public void Initialize()
	{
		for (int i = 0; i < m_propLibrary.Length; i++)
		{
			AGPropSettings component = m_propLibrary[i].gameObject.GetComponent<AGPropSettings>();
			if (component != null)
			{
				for (int j = 0; j < component.m_propability; j++)
				{
					m_propSettingsLookup.Add(component);
				}
			}
		}
	}
}
