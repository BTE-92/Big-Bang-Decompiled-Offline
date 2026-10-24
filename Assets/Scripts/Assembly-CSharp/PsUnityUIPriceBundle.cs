using UnityEngine;
using UnityEngine.UI;

public class PsUnityUIPriceBundle : UnityUIBundle
{
	public Outline m_outline;

	public Text m_priceText;

	public Image m_icon;

	public void SetPrice(int value)
	{
		m_priceText.text = Loc.T(value.ToString());
	}

	public void SetPrice(int value, Resource type)
	{
		m_priceText.text = Loc.T(value.ToString());
		Frame frame = null;
		switch (type)
		{
		case Resource.Bolt:
			m_outline.effectColor = Color.red;
			frame = PsState.m_uiSheet.m_atlas.GetFrame("menu_resources_bolt_price");
			break;
		case Resource.Coin:
			m_outline.effectColor = Color.yellow;
			frame = PsState.m_uiSheet.m_atlas.GetFrame("menu_resources_coin_price");
			break;
		case Resource.Gem:
			m_outline.effectColor = Color.blue;
			frame = PsState.m_uiSheet.m_atlas.GetFrame("menu_resources_diamond_price");
			break;
		case Resource.Key:
			m_outline.effectColor = Color.blue;
			frame = PsState.m_uiSheet.m_atlas.GetFrame("menu_resources_key_price");
			break;
		}
	}
}
