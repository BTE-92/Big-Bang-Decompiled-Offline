using System;
using UnityEngine;

public class PsUIVersusOwnTurnBanner : PsUIVersusBanner
{
	public PsUIVersusOwnTurnBanner(UIComponent _parent, PsGameLoopVersus _loop, Action _clickAction = null)
		: base(_parent, _loop, _clickAction)
	{
	}

	public override void CreateRoundInfo(UIComponent _parent)
	{
		string text = "ROUND " + m_loop.m_versusData.round;
		string color = "ffffff";
		if (m_loop.m_versusData.matchBall)
		{
			text = "CHANCE\nTO WIN!";
			color = "a1ff1b";
		}
		UICanvas uICanvas = new UICanvas(_parent, false, string.Empty, null, string.Empty);
		uICanvas.SetVerticalAlign(1f);
		uICanvas.SetMargins(0f, 0f, -0.01f, 0.01f);
		uICanvas.SetHeight(0.08f, RelativeTo.ScreenHeight);
		uICanvas.RemoveDrawHandler();
		UIText uIText = new UIText(uICanvas, false, string.Empty, text, "KGSecondChances_Font", 0.04f, RelativeTo.ScreenHeight, color, "000000");
		uIText.SetVerticalAlign(0f);
		uIText.SetShadowShift(new Vector2(1f, -0.5f), 0.05f);
	}

	public override void CreateLowerInfo(UIComponent _parent)
	{
		base.CreateLowerInfo(_parent);
		if (m_loop.m_versusData.matchBall)
		{
			m_proceed.SetGreenColors();
		}
	}

	public override void CreateOwnInfo(UIComponent _parent)
	{
		base.CreateOwnInfo(_parent);
		if (m_loop.m_versusData.matchBall)
		{
			UIFittedSprite uIFittedSprite = new UIFittedSprite(this, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_trophy_small_full"));
			uIFittedSprite.SetHeight(0.1f, RelativeTo.ScreenHeight);
			uIFittedSprite.SetHorizontalAlign(0f);
			uIFittedSprite.SetDepthOffset(-5f);
			TransformS.SetRotation(uIFittedSprite.m_TC, new Vector3(0f, 0f, 3.5f));
		}
	}

	public override void Step()
	{
		if (m_proceed.m_hit)
		{
			m_loop.StartLoop();
			if (m_clickAction != null)
			{
				m_clickAction();
			}
		}
		else
		{
			base.Step();
		}
	}
}
