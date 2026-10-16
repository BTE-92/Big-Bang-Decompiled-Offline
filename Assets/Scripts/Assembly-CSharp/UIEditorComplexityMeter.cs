public class UIEditorComplexityMeter : UIComponent
{
	private UISprite m_fillSprite;

	private float m_currentValue;

	private Minigame m_currentMinigame;

	public UIEditorComplexityMeter(UIComponent _parent, float _appearLimit = 0.7f)
		: base(_parent, false, "ComplexityMeter", null, null, string.Empty)
	{
		m_currentMinigame = LevelManager.m_currentLevel as Minigame;
		m_currentValue = 0f;
		RemoveDrawHandler();
		UIVerticalList uIVerticalList = new UIVerticalList(this, "vlist");
		uIVerticalList.RemoveDrawHandler();
		uIVerticalList.SetWidth(0.06f, RelativeTo.ScreenShortest);
		UIFittedSprite parent = new UIFittedSprite(uIVerticalList, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_thermometer_top"));
		m_fillSprite = new UISprite(parent, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_thermometer_fill"), false);
		m_fillSprite.SetHeight(0f, RelativeTo.ParentHeight);
		m_fillSprite.SetWidth(0.25f, RelativeTo.ParentWidth);
		m_fillSprite.SetVerticalAlign(0f);
		new UIFittedSprite(uIVerticalList, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_thermometer_bottom"));
	}

	public override void Step()
	{
		UpdateMeter(m_currentMinigame.m_complexity, m_currentMinigame.m_maxComplexity);
		base.Step();
	}

	public void UpdateMeter(float _value, float _maxValue)
	{
		if (m_currentValue != _value)
		{
			float positionBetween = ToolBox.getPositionBetween(_value, 0f, _maxValue);
			m_fillSprite.SetHeight(positionBetween * 0.9f, RelativeTo.ParentHeight);
			m_currentValue = _value;
			m_fillSprite.Update();
		}
	}
}
