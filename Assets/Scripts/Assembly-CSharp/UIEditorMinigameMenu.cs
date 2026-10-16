using UnityEngine;

public class UIEditorMinigameMenu : UICanvas
{
	public UIRectSpriteButton m_freeWizardButton;

	public UIEditorMinigameMenu(UIComponent _parent, string _tag)
		: base(_parent, false, _tag, null, string.Empty)
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(0.14f, RelativeTo.ParentHeight);
		SetVerticalAlign(1f);
		RemoveDrawHandler();
		Update();
	}

	public override void Step()
	{
		if (m_freeWizardButton.m_hit)
		{
			SoundS.PlaySingleShot("/UI/ButtonNormal", Vector3.zero);
			EditorBaseState.RemoveTransformGizmo();
			if (PsState.m_freeWizardLastSelectedItemCategory == string.Empty)
			{
				PsState.m_freeWizardLastSelectedItemCategory = UIEditorSelectorNavigator.WIZARD_CATEGORIES[0];
			}
			if (PsState.m_freeWizardLastSelectedItemCategory == UIEditorSelectorNavigator.WIZARD_CATEGORIES[0])
			{
				Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorSelectorPlayer(EditorSelectorContext.FREE_WIZARD));
			}
			else if (PsState.m_freeWizardLastSelectedItemCategory == UIEditorSelectorNavigator.WIZARD_CATEGORIES[1])
			{
				Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorSelectorGameMode(EditorSelectorContext.FREE_WIZARD));
			}
			else
			{
				Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorSelectorArea(EditorSelectorContext.FREE_WIZARD));
			}
		}
		base.Step();
	}
}
