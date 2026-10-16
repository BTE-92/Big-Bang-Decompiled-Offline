public class PsUIBoosterButtonSpeed : PsUIBoosterButton
{
	public PsUIBoosterButtonSpeed(UIComponent _parent, float _width = 0.345f)
		: base(_parent)
	{
	}

	public override void CreateBooster(Vehicle _vehicle, bool _freeBooster)
	{
		_vehicle.m_booster = new SpeedBooster(_freeBooster);
	}
}
