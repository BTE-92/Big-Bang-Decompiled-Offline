using System;
using UnityEngine;

public class PsUIEditorEdit : UICanvas, IPlayMenu
{
	protected PsUIGenericButton m_exitButton;

	protected PsUIGenericButton m_playButton;

	protected UIRectSpriteButton m_objectButton;

	protected Action m_exitAction;

	protected Action m_playAction;

	protected Action m_objectAction;

	protected UITextbox m_adminInfo;

	protected UIEditorDrawMenu m_drawMenu;

	protected UIEditorObjectMenu m_objectMenu;

	protected UIRectSpriteButton m_freeWizardButton;

	public PsUIEditorEdit(Action _exitAction = null, Action _playAction = null, Action _objectAction = null)
		: base(null, false, string.Empty, null, string.Empty)
	{
		m_exitAction = _exitAction;
		m_playAction = _playAction;
		m_objectAction = _objectAction;
		RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, "MinigameMenu");
		uIHorizontalList.SetMargins(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetAlign(0f, 1f);
		uIHorizontalList.RemoveDrawHandler();
		CreateTopLeftArea(uIHorizontalList);
		UIHorizontalList uIHorizontalList2 = new UIHorizontalList(this, "BottomLeft");
		uIHorizontalList2.SetMargins(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList2.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList2.SetAlign(0f, 0f);
		if (PsState.m_editorIsLefty)
		{
			uIHorizontalList2.SetAlign(1f, 0f);
		}
		uIHorizontalList2.RemoveDrawHandler();
		CreateBottomLeftArea(uIHorizontalList2);
		UIHorizontalList uIHorizontalList3 = new UIHorizontalList(this, "EditorMenu");
		uIHorizontalList3.SetMargins(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList3.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList3.SetAlign(1f, 1f);
		uIHorizontalList3.RemoveDrawHandler();
		CreateTopRightArea(uIHorizontalList3);
		UIHorizontalList uIHorizontalList4 = new UIHorizontalList(this, "BottomRight");
		uIHorizontalList4.SetMargins(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList4.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList4.SetAlign(1f, 0f);
		if (PsState.m_editorIsLefty)
		{
			uIHorizontalList4.SetAlign(0f, 0f);
		}
		uIHorizontalList4.RemoveDrawHandler();
		CreateBottomRightArea(uIHorizontalList4);
		CreateAdminInfo();
		Update();
	}

	public virtual void CreateAdminInfo()
	{
	}

	public virtual void CreateTopLeftArea(UIComponent _parent)
	{
		m_exitButton = new PsUIGenericButton(_parent);
		m_exitButton.SetText("SAVE &" + Environment.NewLine + "EXIT", 0.025f);
		m_exitButton.SetOrangeColors();
		if (true)
		{
			m_freeWizardButton = new UIRectSpriteButton(_parent, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_settings"), false);
			m_freeWizardButton.SetHeight(0.12f, RelativeTo.ScreenShortest);
		}
	}

	public virtual void CreateBottomLeftArea(UIComponent _parent)
	{
		m_drawMenu = new UIEditorDrawMenu(_parent, "DrawMenu");
		m_drawMenu.OpenDrawWindow(PsState.m_editorIsLefty);
	}

	public virtual void CreateTopRightArea(UIComponent _parent)
	{
		m_playButton = new PsUIGenericButton(_parent);
		m_playButton.SetText("DRIVE", 0.05f);
		m_playButton.SetOrangeColors();
	}

	public virtual void CreateBottomRightArea(UIComponent _parent)
	{
		m_objectMenu = new UIEditorObjectMenu(_parent, "ObjectMenu");
	}

	public void ApplyLeftySettings()
	{
		if (m_drawMenu != null)
		{
			m_drawMenu.ApplyLeftySettings();
		}
		if (m_objectMenu != null)
		{
			m_objectMenu.SetHorizontalAlign(PsState.m_objectMenuButtonAlign);
			m_objectMenu.Update();
		}
	}

	public override void Step()
	{
		if ((object)null != null && PsState.m_activeMinigame != null)
		{
			m_adminInfo.SetText("Admin Mode\nLevel Requirement: " + PsState.m_activeMinigame.m_levelRequirement);
		}
		if (m_exitButton != null && m_exitButton.m_hit)
		{
			m_exitAction();
		}
		else if (m_playButton != null && m_playButton.m_hit)
		{
			m_playAction();
		}
		else if (m_objectButton != null && m_objectButton.m_hit)
		{
			m_objectAction();
		}
		else if (m_freeWizardButton != null && m_freeWizardButton.m_hit)
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
