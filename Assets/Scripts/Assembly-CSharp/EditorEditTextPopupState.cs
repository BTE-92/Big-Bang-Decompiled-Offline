public class EditorEditTextPopupState : BasicState
{
	public TextModel m_textModel;

	public UIModel m_model;

	private UIScrollableCanvas m_savePopupContainer;

	private UITextArea m_nameField;

	private UITextArea m_descriptionField;

	private UISavePopupFooter m_footer;

	private UITextbox m_cancelButton;

	private UITextbox m_saveButton;

	public EditorEditTextPopupState(TextModel _model)
	{
		Debug.Log(_model.m_text);
		m_textModel = _model;
	}

	public override void Enter(IStatedObject _parent)
	{
		m_model = new UIModel(m_textModel);
		m_savePopupContainer = new UIScrollableCanvas(null, "SavePopupContainer");
		m_savePopupContainer.SetWidth(1f, RelativeTo.ScreenWidth);
		m_savePopupContainer.SetHeight(1f, RelativeTo.ScreenHeight);
		m_savePopupContainer.SetMargins(0.125f, 0.125f, 0f, 0f);
		m_savePopupContainer.SetDrawHandler(UIDrawHandlers.EditorPopupBackground);
		UIVerticalList uIVerticalList = new UIVerticalList(m_savePopupContainer, "SavePopup");
		uIVerticalList.RemoveDrawHandler();
		new UIPopupHeader(uIVerticalList, "SavePopupHeader", "edit", string.Empty);
		UIPopupContentArea parent = new UIPopupContentArea(uIVerticalList, "ContentArea");
		m_nameField = new UITextArea(parent, "SavePopupNameField", "Text", string.Empty, null, m_model, "m_text");
		UIHorizontalList uIHorizontalList = new UIHorizontalList(uIVerticalList, string.Empty);
		uIHorizontalList.SetHorizontalAlign(1f);
		uIHorizontalList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		uIHorizontalList.RemoveDrawHandler();
		m_cancelButton = new UITextButton(uIHorizontalList, "SavePanelCancelButton", "Cancel", "HurmeBold_Font", 0.0375f);
		m_cancelButton.SetMargins(0.025f, 0.025f, 0.02f, 0.015f);
		m_cancelButton.SetHorizontalAlign(0f);
		m_cancelButton.SetDrawHandler(UIDrawHandlers.NegativeButton);
		m_saveButton = new UITextButton(uIHorizontalList, "SavePanelSaveButton", "Save", "HurmeBold_Font", 0.0375f);
		m_saveButton.SetMargins(0.025f, 0.025f, 0.02f, 0.015f);
		m_saveButton.SetHorizontalAlign(1f);
		m_saveButton.SetDrawHandler(UIDrawHandlers.PositiveButton);
		m_savePopupContainer.Update();
	}

	public override void Execute()
	{
		if (m_saveButton.m_hit)
		{
			m_textModel.m_done = true;
		}
		else if (m_cancelButton.m_hit)
		{
			m_textModel.m_cancelled = true;
		}
	}

	public override void Exit()
	{
		if (m_savePopupContainer != null)
		{
			m_savePopupContainer.Destroy();
			m_savePopupContainer = null;
		}
	}
}
