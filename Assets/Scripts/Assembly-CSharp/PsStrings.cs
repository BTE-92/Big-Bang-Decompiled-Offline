public static class PsStrings
{
	public static int currentLanguage;

	public static string Get(StringID id)
	{
		string text = StringTable.strings[(int)id * 6 + currentLanguage];
		if (text.Equals(string.Empty))
		{
			return StringTable.strings[(int)id * 6];
		}
		return text;
	}

	public static string Get(string stringLabel)
	{
		for (int i = 0; i < StringTable.stringIDs.Length; i++)
		{
			if (StringTable.stringIDs[i].Equals(stringLabel))
			{
				return Get((StringID)i);
			}
		}
		return "[no string found]";
	}
}
