using System;

public abstract class Flow
{
	public Action Proceed;

	public Action Cancel;

	public Flow m_rootFlow;

	public Flow(Action _proceed, Action _cancel, Flow _rootFlow = null)
	{
		Proceed = _proceed;
		Cancel = _cancel;
		m_rootFlow = _rootFlow;
	}
}
