using UnityEngine;

public class UIEditorObjectMenu : UIScaleToContentCanvas
{
	public UIRectSpriteButton m_objectMenuButton;

	public UIEditorComplexityMeter m_complexityMeter;

	public UIFittedText m_amount;

	public UIEditorObjectMenu(UIComponent _parent, string _tag)
		: base(_parent, _tag)
	{
		SetAlign(1f, 0f);
		SetMargins(0.02f, RelativeTo.ScreenShortest);
		RemoveTouchAreas();
		RemoveDrawHandler();
		int num = PsState.m_newEditorItemCount;
		for (int i = 0; i < 6; i++)
		{
			PsUnlockable psUnlockable = PsMetagameData.m_gameMaterials[0].m_items[i];
			for (int j = 0; j < PsState.m_newEditorItems.Length; j++)
			{
				if (PsState.m_newEditorItems[j] == psUnlockable.m_identifier)
				{
					num--;
				}
			}
		}
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, "Container");
		uIHorizontalList.SetSpacing(0.005f, RelativeTo.ScreenShortest);
		uIHorizontalList.RemoveTouchAreas();
		uIHorizontalList.RemoveDrawHandler();
		m_complexityMeter = new UIEditorComplexityMeter(uIHorizontalList);
		m_complexityMeter.SetSize(0.06f, 0.12f, RelativeTo.ScreenShortest);
		m_complexityMeter.SetVerticalAlign(0.2f);
		m_objectMenuButton = new UIRectSpriteButton(uIHorizontalList, "Add Object", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_object_add"), false);
		m_objectMenuButton.SetSize(0.2f, 0.2f, RelativeTo.ScreenShortest);
		if (num > 0)
		{
			UICanvas uICanvas = new UICanvas(m_objectMenuButton, false, "notification", null, string.Empty);
			uICanvas.SetSize(0.05f, 0.05f, RelativeTo.ScreenHeight);
			uICanvas.SetMargins(0.01f, -0.01f, 0.01f, -0.01f);
			uICanvas.SetAlign(0f, 1f);
			uICanvas.SetDepthOffset(-10f);
			uICanvas.RemoveDrawHandler();
			UICanvas uICanvas2 = new UICanvas(uICanvas, false, string.Empty, null, string.Empty);
			uICanvas2.SetSize(1f, 1f, RelativeTo.ParentHeight);
			uICanvas2.SetMargins(0.15f, RelativeTo.OwnHeight);
			uICanvas2.SetDrawHandler(PsUIDrawHandlers.NotificationDrawhandler);
			TweenC c = TweenS.AddTransformTween(uICanvas2.m_TC, TweenedProperty.Scale, TweenStyle.CubicInOut, new Vector3(1.1f, 1.1f, 1.1f), 0.5f, 0f, false);
			TweenS.SetAdditionalTweenProperties(c, -1, true, TweenStyle.CubicInOut);
			m_amount = new UIFittedText(uICanvas2, false, string.Empty, num.ToString(), "KGSecondChances_Font", true);
		}
		Update();
	}

	public override void Step()
	{
		if (m_objectMenuButton.m_hit)
		{
			EditorBaseState.RemoveTransformGizmo();
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorSelectorItem(PsState.m_editorLastSelectedItemCategory, m_amount));
			SoundS.PlaySingleShot("/UI/ButtonItems", Vector3.zero);
		}
		base.Step();
	}
}
