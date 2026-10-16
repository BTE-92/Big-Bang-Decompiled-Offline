public class SpeedBooster : Booster
{
	public SpeedBooster(bool _free)
		: base(_free)
	{
	}

	public override void Use(Vehicle _vehicle)
	{
		if (_vehicle is ISpeedBoost && !m_used && PsMetagameManager.m_playerStats.GetBoosterValue() > 0)
		{
			(_vehicle as ISpeedBoost).TrickBoost(2, true);
			base.Use(_vehicle);
		}
	}
}
