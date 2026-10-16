public class PsReplayEventManipulateGround : ReplayEvent
{
	public byte[] m_originalBytes;

	public byte[] m_changedBytes;

	public PsReplayEventManipulateGround()
		: base((ReplayEventType)32)
	{
	}
}
