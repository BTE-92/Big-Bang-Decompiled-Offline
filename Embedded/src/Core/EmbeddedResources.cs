#nullable enable
using System;
using System.IO;

namespace BBR249Server.Core;

// The files built into the assembly (starter map, level templates, starter levels). The game can supply them itself through
// Provider when the platform does not expose the assembly's embedded resources (WebGL).
public static class EmbeddedResources
{
	public static Func<string, byte[]?>? Provider;

	public static Stream? Open(string name)
	{
		if (Provider != null)
		{
			byte[]? bytes = Provider(name);
			if (bytes != null)
			{
				return new MemoryStream(bytes, false);
			}
		}
		return typeof(EmbeddedResources).Assembly.GetManifestResourceStream(name);
	}
}
