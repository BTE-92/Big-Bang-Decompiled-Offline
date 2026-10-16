using System;
using UnityEngine;

public class PsUISkipLevelButton : PsUIGenericButton
{
	private UIVerticalList m_vlist;

	private UIText m_unlockText;

	private int m_diamondPrice;

	private UICanvas offsetCanvas;

	public PsUIGenericButton m_lockButton;

	public Action m_proceed;

	public Action m_cancel;

	public PsUISkipLevelButton(UIComponent _parent, Action _proceed, Action _cancel)
		: base(_parent)
	{
		m_proceed = _proceed;
		m_cancel = _cancel;
		m_vlist = new UIVerticalList(this, "SkipLevelVerticalList");
		m_vlist.SetWidth(0.3f, RelativeTo.ScreenHeight);
		m_vlist.SetVerticalAlign(1f);
		m_vlist.RemoveDrawHandler();
		UIText uIText = new UIText(m_vlist, false, string.Empty, "Skip Level", "KGSecondChances_Font", 0.04f, RelativeTo.ScreenShortest);
	}

	public override void Step()
	{
		int unlockerTimeLeft = PsMetagameManager.GetUnlockerTimeLeft();
		if (unlockerTimeLeft < 1)
		{
			if (m_unlockText != null)
			{
				m_unlockText.Destroy();
				m_unlockText = null;
				offsetCanvas.Destroy();
				offsetCanvas = null;
				SetBlueColors();
				Update();
			}
			if (m_hit)
			{
				PsMetagameManager.SkipLevel(0, PsState.m_activeGameLoop.GetPlayerUnit());
				m_proceed();
			}
		}
		else
		{
			bool flag = false;
			int num = PsMetagameManager.SecondsToDiamonds(unlockerTimeLeft);
			if (m_unlockText == null)
			{
				m_unlockText = new UIText(m_vlist, false, string.Empty, string.Empty, "HurmeSemiBold_Font", 0.027f, RelativeTo.ScreenShortest);
				m_unlockText.SetShadowShift(new Vector2(2f, 1f), 0.035f);
				offsetCanvas = new UICanvas(this, false, "LockButtonOffsetCanvas", null, string.Empty);
				offsetCanvas.SetRogue();
				offsetCanvas.SetHorizontalAlign(1f);
				offsetCanvas.SetSize(1f, 1f, RelativeTo.ParentHeight);
				offsetCanvas.SetMargins(0f, -0.5f, 0f, 0f, RelativeTo.ParentHeight);
				offsetCanvas.RemoveDrawHandler();
				m_lockButton = new PsUIGenericButton(offsetCanvas);
				m_lockButton.SetMargins(0.01f, 0.01f, 0.01f, 0.01f);
				m_lockButton.SetDepthOffset(-20f);
				m_lockButton.SetOrangeColors();
				m_lockButton.SetIcon("menu_icon_unlock");
				m_lockButton.SetHorizontalAlign(1f);
				m_diamondPrice = num;
				m_lockButton.SetSkipPrice(m_diamondPrice, 0.03f);
				SetGrayColors();
				flag = true;
			}
			if (m_diamondPrice != num)
			{
				m_diamondPrice = num;
				m_lockButton.m_priceText.SetText(string.Empty + m_diamondPrice);
			}
			string timeStringFromSeconds = PsMetagameManager.GetTimeStringFromSeconds(unlockerTimeLeft, false);
			m_unlockText.SetText("Unlocks in: " + timeStringFromSeconds);
			if (m_lockButton.m_hit)
			{
				new PsSkipLevelFlow(m_proceed, m_cancel, m_diamondPrice);
			}
			if (flag)
			{
				Update();
			}
		}
		base.Step();
	}
}
