using System;

public class PsUIFriendlyWaitingBanner : PsUIVersusBanner
{
	public PsUIFriendlyWaitingBanner(UIComponent _parent, PsGameLoopVersus _loop, Action _clickAction = null)
		: base(_parent, _loop, _clickAction)
	{
	}

	public new virtual void CreateOpponentInfo(UIComponent _parent)
	{
	}

	public PlayerData GetOpponentInfo(string _id)
	{
		return default(PlayerData);
	}
}
