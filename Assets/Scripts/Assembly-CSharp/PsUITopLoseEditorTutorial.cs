public class PsUITopLoseEditorTutorial : UICanvas
{
	private PsUIGenericButton m_pauseButton;

	public PsUITopLoseEditorTutorial(UIComponent _parent)
		: base(_parent, false, "TopContent", null, string.Empty)
	{
		PsMetagameManager.HideResources();
		RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, "UpperRight");
		uIHorizontalList.SetMargins(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetAlign(1f, 1f);
		uIHorizontalList.RemoveDrawHandler();
		m_pauseButton = new PsUIGenericButton(uIHorizontalList);
		m_pauseButton.SetText("EDIT", 0.05f);
		m_pauseButton.SetOrangeColors();
	}

	public override void Step()
	{
		if (m_pauseButton.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
		}
		base.Step();
	}
}
