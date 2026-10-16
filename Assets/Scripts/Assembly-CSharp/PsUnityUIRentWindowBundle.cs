using UnityEngine;
using UnityEngine.UI;

public class PsUnityUIRentWindowBundle : UnityUIBundle
{
	private const int x = 3;

	public PsUnityUIUpgradeBannerBundle m_banner;

	public Image[] m_icons = new Image[3];

	public Text[] m_upgradeTexts = new Text[3];

	public Text[] m_upgradeValues = new Text[3];

	public PsUnityUIButtonBundle m_rentButtonBundle;

	public PsUnityUIButtonBundle m_switchButton;

	public Animator m_animator;
}
