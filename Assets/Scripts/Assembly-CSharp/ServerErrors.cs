public static class ServerErrors
{
	private static string NETWORK_ERROR = "Unable to connect with the server. Check your internet connection and try again.";

	private static string MAINTENANCE_BREAK = "Servers are down for maintenance.";

	public static string GetNetworkError(string _wwwError)
	{
		Debug.LogError("Network error: " + _wwwError);
		if (!string.IsNullOrEmpty(_wwwError) && _wwwError.StartsWith("503"))
		{
			return MAINTENANCE_BREAK;
		}
		return NETWORK_ERROR;
	}
}
