using System;
using System.Collections.Generic;
using InAppPurchases;

public static class PsIAPManager
{
	private static bool m_initialized;

	public static void Initialize()
	{
	}

	public static void Initialize(Dictionary<string, object> _config)
	{
	}

	public static ServerProduct GetServerProductById(string _identifier)
	{
		return null;
	}

	public static IAPProduct GetIAPProductById(string _identifier)
	{
		return null;
	}

	public static bool CanMakePayments()
	{
		return false;
	}

	public static void PurchaseConsumable(string _id, Action<string, IAP.TransactionResult, string, string> _callback = null)
	{
		_callback(null, IAP.TransactionResult.Failed, "In-app purchases disabled", null);
	}

	public static void PurchaseNoconsumable(string _id, Action<StoreKitTransaction, IAP.TransactionResult, string> _callback = null)
	{
	}
}
