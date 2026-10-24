#nullable enable
using System;
using System.Security.Cryptography;

namespace BBR249Server.Core
{
	// APIs of newer .NET that the server uses and .NET Standard 2.0 (Unity) lacks.
	public static class PolyfillExtensions
	{
		public static bool Contains(this string s, char c) { return s.IndexOf(c) >= 0; }

		public static string[] Split(this string s, char sep, StringSplitOptions options) { return s.Split(new[] { sep }, options); }
	}

	public static class MathX
	{
		public static int Clamp(int v, int min, int max) { return v < min ? min : (v > max ? max : v); }
		public static long Clamp(long v, long min, long max) { return v < min ? min : (v > max ? max : v); }
		public static double Clamp(double v, double min, double max) { return v < min ? min : (v > max ? max : v); }
	}

	public static class Sha
	{
		public static byte[] Hash256(byte[] data)
		{
			using (SHA256 sha = SHA256.Create())
			{
				return sha.ComputeHash(data);
			}
		}
	}
}
