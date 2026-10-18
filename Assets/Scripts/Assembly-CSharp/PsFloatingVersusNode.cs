using UnityEngine;

public class PsFloatingVersusNode : PsFloatingPlanetNode
{
	protected string m_domeShader = "WOE/Units/Unlit";

	protected string m_trophyShader = "WOE/Fx/UnlitReflectiveRim";

	protected PsUIProfileImageSprite m_opponentImage;

	public bool m_grey;

	public PsFloatingVersusNode(PsTimedEventLoop _loop)
		: base(_loop)
	{
	}

	public override void SetPrefabName()
	{
		m_prefabName = "FloatingPlatformTournamentPrefab";
	}

	public virtual void AddGreyScale()
	{
		if (!m_grey && m_prefab != null)
		{
			m_prefab.p_gameObject.transform.Find("FloatingPlatform").transform.GetComponent<Renderer>().material.shader = Shader.Find("WOE/Units/Greyscale");
			m_prefab.p_gameObject.transform.Find("CollectibleTrophy").transform.GetComponent<Renderer>().material.shader = Shader.Find("WOE/Units/Greyscale");
			m_grey = true;
		}
	}

	public virtual void RemoveGreyScale()
	{
		if (m_grey && m_prefab != null)
		{
			m_prefab.p_gameObject.transform.Find("FloatingPlatform").transform.GetComponent<Renderer>().material.shader = Shader.Find(m_domeShader);
			m_prefab.p_gameObject.transform.Find("CollectibleTrophy").transform.GetComponent<Renderer>().material.shader = Shader.Find(m_trophyShader);
			m_grey = false;
		}
	}
}
