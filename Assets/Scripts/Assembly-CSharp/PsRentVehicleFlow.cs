using System;

public class PsRentVehicleFlow : Flow
{
	public PsRentVehicleFlow(Action _proceed, Action _cancel, PsGameLoop _minigameInfo, int _rentPrice)
		: base(_proceed, _cancel)
	{
		PsRentVehicleFlow psRentVehicleFlow = this;
		if (PsMetagameManager.m_playerStats.diamonds >= _rentPrice)
		{
			new PsServerQueueFlow(delegate
			{
				psRentVehicleFlow.Proceed();
			}, delegate
			{
				PsMetagameManager.UseGemsForVehicleRent(_rentPrice);
			}, null, "Renting the vehicle...");
		}
		else
		{
			LastResourceView resourceview = PsMetagameManager.m_menuResourceView.SetLastView();
			Action action = delegate
			{
				PsMetagameManager.m_menuResourceView.ShowLastView(resourceview);
			};
			new PsGetDiamondsFlow(action, action);
		}
	}
}
