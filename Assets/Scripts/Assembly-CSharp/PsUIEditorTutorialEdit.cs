using System;

public class PsUIEditorTutorialEdit : PsUIEditorEdit
{
	public PsUIEditorTutorialEdit(Action _exitAction = null, Action _playAction = null, Action _objectAction = null)
		: base(_exitAction, _playAction, _objectAction)
	{
	}

	public override void CreateTopLeftArea(UIComponent _parent)
	{
		m_exitButton = new PsUIGenericButton(_parent);
		m_exitButton.SetIcon("menu_icon_exit", 0.06f);
		m_exitButton.SetOrangeColors();
	}

	public override void CreateAdminInfo()
	{
	}

	public override void CreateBottomRightArea(UIComponent _parent)
	{
	}
}
