using System;

public class TextModel
{
	public string m_text;

	public bool m_done;

	public bool m_cancelled;

	public UIModel m_uiModel;

	public TextModel(string _default, Action<string, object, object> _valueChangeDelegate = null)
	{
		m_text = _default;
		m_done = false;
		m_cancelled = false;
		m_uiModel = new UIModel(this, _valueChangeDelegate);
	}
}
