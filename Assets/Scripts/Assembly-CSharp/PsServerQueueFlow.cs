using System;

public class PsServerQueueFlow : Flow
{
	public PsServerQueueFlow(Action _proceed, Action _serverCall, string[] _tags)
		: base(_proceed, null)
	{
		PsMetagameManager.m_serverQueueItems.Add(new ServerQueueItem(_tags, _proceed, _serverCall));
	}

	public PsServerQueueFlow(Action _proceed, Action _serverCall, string[] _tags, string _textBox)
		: base(_proceed, null)
	{
		PsUIBasePopup psUIBasePopup = new PsUIBasePopup(typeof(PsUIPopupServerQueue), null, null, null, false);
		psUIBasePopup.SetAction("Server", _serverCall);
		psUIBasePopup.SetAction("Proceed", _proceed);
		(psUIBasePopup.m_mainContent as PsUIPopupServerQueue).CreateContent(_tags, _textBox);
		psUIBasePopup.Update();
	}
}
