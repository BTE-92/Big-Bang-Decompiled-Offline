using UnityEngine;
using UnityEngine.UI;

public class PsUnityUIUpgradeBarsBundle : UnityUIBundle
{
	public PsUnityUIButtonBundle[] m_buttonBundles = new PsUnityUIButtonBundle[3];

	public Text[] m_texts = new Text[3];

	public Animator[] m_fillerAnimators = new Animator[3];

	public GameObject m_content;

	public Button m_levelUpButton;

	public GameObject m_banners;

	public PsUnityUIButtonBundle m_switchButton;

	public Animator m_animator;

	public CanvasGroup m_contentCanvasGroup;
}
