using System.Collections.Generic;

public class PsDialogue
{
	public string m_identifier;

	public PsNodeEventTrigger m_trigger;

	public List<PsDialogueStep> m_steps;

	public PsDialogue(string _identifier, PsNodeEventTrigger _trigger)
	{
		m_identifier = _identifier;
		m_trigger = _trigger;
		m_steps = new List<PsDialogueStep>();
	}
}
