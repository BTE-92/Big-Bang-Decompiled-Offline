using UnityEngine;

public class PsUICreateSavedBanner : PsUICreateLevelBanner
{
	public PsUICreateSavedBanner(UIComponent _parent, PsGameLoop _gameLoop)
		: base(_parent, _gameLoop)
	{
	}

	public override void Initialize()
	{
		base.Initialize();
		m_bannerIconName = "hud_gizmo_delete";
	}

	public override void CreateButton(UIComponent _parent)
	{
		m_button = new PsUIGenericButton(_parent);
		m_button.SetText("Build", 0.04f);
		m_button.SetBlueColors();
		m_button.SetHorizontalAlign(1f);
		m_button.SetDepthOffset(-20f);
		if (PsState.m_newEditorItemCount > 0)
		{
			UICanvas uICanvas = new UICanvas(m_button, false, "notification", null, string.Empty);
			uICanvas.SetSize(0.04f, 0.04f, RelativeTo.ScreenHeight);
			uICanvas.SetMargins(-0.035f, 0.035f, -0.03f, 0.03f);
			uICanvas.SetRogue();
			uICanvas.SetAlign(0f, 1f);
			uICanvas.SetDepthOffset(-10f);
			uICanvas.RemoveDrawHandler();
			UICanvas uICanvas2 = new UICanvas(uICanvas, false, string.Empty, null, string.Empty);
			uICanvas2.SetSize(1f, 1f, RelativeTo.ParentHeight);
			uICanvas2.SetMargins(0.15f, RelativeTo.OwnHeight);
			uICanvas2.SetDrawHandler(PsUIDrawHandlers.NotificationDrawhandler);
			TweenC c = TweenS.AddTransformTween(uICanvas2.m_TC, TweenedProperty.Scale, TweenStyle.CubicInOut, new Vector3(1.1f, 1.1f, 1.1f), 0.5f, 0f, false);
			TweenS.SetAdditionalTweenProperties(c, -1, true, TweenStyle.CubicInOut);
			UIFittedText uIFittedText = new UIFittedText(uICanvas2, false, string.Empty, "!", "KGSecondChances_Font", true);
		}
	}

	public override void CreateScreenshot(UIComponent _parent)
	{
		m_screenshot = new PsUIScreenshot(_parent, false, string.Empty, Vector2.zero, m_loop, false, true, 0.02f);
		m_screenshot.SetSize(0.28f, 0.18f, RelativeTo.ScreenHeight);
		m_screenshot.SetMargins(-0.0125f, -0.0125f, -0.025f, -0.025f);
		m_screenshot.SetDrawHandler(PsUIScreenshot.AltDrawHandler);
		CreateGameMode(m_screenshot);
	}
}
