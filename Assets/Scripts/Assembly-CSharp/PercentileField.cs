public class PercentileField
{
	public int index;

	public int percentile;

	public int reward;

	public override string ToString()
	{
		return "{Percentile: " + percentile.ToString() + ", Reward: " + reward + "}";
	}
}
