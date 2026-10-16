public static class PsResources
{
	public static string[] NameGeneratorBaseStrings = new string[17]
	{
		"bob", "mark", "john", "bubba", "jack", "keith", "rico", "stephen", "alex", "peter",
		"veera", "mike", "sam", "oliver", "jari", "olli", "tiina"
	};

	public static string[] LevelNameGeneratorBaseStrings;

	public static void LoadGameResources()
	{
		ResourcePool.LoadResourceGroup(RESOURCE_GROUP.GAME);
	}

	public static void UnloadGameResources()
	{
		ResourcePool.UnloadResourceGroup(RESOURCE_GROUP.GAME);
	}

	public static void LoadMenuResources()
	{
		ResourcePool.LoadResourceGroup(RESOURCE_GROUP.MENU);
	}

	public static void UnloadMenuResources()
	{
		ResourcePool.UnloadResourceGroup(RESOURCE_GROUP.MENU);
	}
}
