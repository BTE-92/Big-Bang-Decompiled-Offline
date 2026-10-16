using UnityEngine;

public class PsUIInfoBar : UICanvas
{
	private UIText m_text;

	private string[] m_tipStrings;

	private UISprite m_leftSprite;

	private UISprite m_midleSprite;

	private UISprite m_rightSprite;

	private UIHorizontalList m_contentArea;

	private bool m_useTween;

	public PsUIInfoBar(UIComponent _parent, string _infoText = "", bool _useTween = true)
		: base(_parent, false, string.Empty, null, string.Empty)
	{
		m_useTween = _useTween;
		float num = 0.09f;
		float num2 = num - 0.03f;
		SetSize(0.9f, num, RelativeTo.ScreenHeight);
		SetMargins(0.035f, 0.035f, 0.015f, 0.015f);
		SetVerticalAlign(0f);
		RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetVerticalAlign(1f);
		Frame frame = PsState.m_uiSheet.m_atlas.GetFrame("menu_infobar");
		float num3 = frame.width / 3f;
		Frame frame2 = new Frame(frame.x, frame.y, num3, frame.height);
		Frame frame3 = new Frame(frame.x + num3, frame.y, num3, frame.height);
		Frame frame4 = new Frame(frame.x + num3 * 2f, frame.y, num3, frame.height);
		m_leftSprite = new UISprite(uIHorizontalList, false, string.Empty, PsState.m_uiSheet, frame2);
		m_midleSprite = new UISprite(uIHorizontalList, false, string.Empty, PsState.m_uiSheet, frame3);
		m_rightSprite = new UISprite(uIHorizontalList, false, string.Empty, PsState.m_uiSheet, frame4);
		m_leftSprite.SetSize(num2 * m_leftSprite.m_width / m_leftSprite.m_height, num2, RelativeTo.ScreenHeight);
		m_rightSprite.SetSize(num2 * m_rightSprite.m_width / m_rightSprite.m_height, num2, RelativeTo.ScreenHeight);
		m_midleSprite.SetSize(0.7f, num2, RelativeTo.ScreenHeight);
		m_contentArea = new UIHorizontalList(this, string.Empty);
		m_contentArea.SetDepthOffset(-4f);
		m_contentArea.SetHeight(num2, RelativeTo.ScreenHeight);
		m_contentArea.RemoveDrawHandler();
		m_contentArea.SetMargins(0f, 0.04f, 0f, 0f);
		m_contentArea.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		m_contentArea.SetAlign(0.5f, 1f);
		UISprite uISprite = new UISprite(m_contentArea, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_icon_bar_info"));
		uISprite.SetSize(0.072f, 0.072f * (uISprite.m_width / uISprite.m_height), RelativeTo.ScreenHeight);
		uISprite.SetColor(DebugDraw.HexToColor("#9df534") * Color.gray);
		m_text = new UIText(m_contentArea, false, "InfoText", _infoText, "HurmeBold_Font", 0.028f, RelativeTo.ScreenHeight, "#d1fc93");
		m_tipStrings = new string[4] { "Not enough coins for upgrade? Use gems instead!", "Can't clear a level? Try the RENTAL vehicle!", "When others LIKE the levels you've made, you EARN COINS", "Level too hard? Use the SKIP button." };
	}

	public void SetText(string _infoText)
	{
		m_text.SetText(_infoText);
	}

	public override void Update()
	{
		if (string.IsNullOrEmpty(m_text.m_text))
		{
			SetText(m_tipStrings[Random.Range(0, m_tipStrings.Length)]);
		}
		base.Update();
		m_midleSprite.SetWidth(m_contentArea.m_width - 0.18f, m_contentArea.m_widthRelativeTo);
		m_midleSprite.m_parent.Update();
		if (m_useTween)
		{
			TweenS.AddTransformTween(m_TC, TweenedProperty.Position, TweenStyle.BackOut, m_TC.transform.localPosition + new Vector3(0f, -80f, 0f), m_TC.transform.localPosition, 0.3f, 0f, true);
			m_useTween = false;
		}
	}

	public void TweenAway()
	{
		TweenS.AddTransformTween(m_TC, TweenedProperty.Position, TweenStyle.BackOut, m_TC.transform.localPosition, m_TC.transform.localPosition + new Vector3(0f, -80f, 0f), 0.3f, 0f, true);
	}
}
