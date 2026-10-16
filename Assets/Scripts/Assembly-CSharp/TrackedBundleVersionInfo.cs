using System;

public class TrackedBundleVersionInfo
{
	public delegate int CompareTo(TrackedBundleVersionInfo info1, TrackedBundleVersionInfo info2);

	public const string UndefinedVersionString = "_undefined";

	public static CompareTo compareTo = DefaultCompareTo;

	public string version;

	public int index;

	public TrackedBundleVersionInfo()
	{
		version = "_undefined";
		index = -1;
	}

	public TrackedBundleVersionInfo(string version, int index = -1)
	{
		this.version = version;
		this.index = index;
	}

	public static int DefaultCompareTo(TrackedBundleVersionInfo info1, TrackedBundleVersionInfo info2)
	{
		if ((object)info1 == null || (object)info2 == null)
		{
			return (((object)info1 == null) ? (-1) : 0) + (((object)info2 == null) ? 1 : 0);
		}
		if (info1.index < 0 && info2.index < 0)
		{
			return StringBasedCompareTo(info1, info2);
		}
		if (info1.index > info2.index)
		{
			return 1;
		}
		if (info1.index < info2.index)
		{
			return -1;
		}
		return 0;
	}

	public static int StringBasedCompareTo(TrackedBundleVersionInfo info1, TrackedBundleVersionInfo info2)
	{
		Version version = new Version(info1.version);
		Version value = new Version(info2.version);
		return version.CompareTo(value);
	}

	public bool AtLeast(TrackedBundleVersionInfo info2)
	{
		return compareTo(this, info2) >= 0;
	}

	public bool Before(TrackedBundleVersionInfo info2)
	{
		return compareTo(this, info2) < 0;
	}

	public override bool Equals(object obj2)
	{
		if (obj2 == null || !(obj2 is TrackedBundleVersionInfo))
		{
			return false;
		}
		return compareTo(this, (TrackedBundleVersionInfo)obj2) == 0;
	}

	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}

	public override string ToString()
	{
		return string.Format("{0}-{1}", version, index);
	}

	public static bool operator ==(TrackedBundleVersionInfo info1, TrackedBundleVersionInfo info2)
	{
		return compareTo(info1, info2) == 0;
	}

	public static bool operator !=(TrackedBundleVersionInfo info1, TrackedBundleVersionInfo info2)
	{
		return compareTo(info1, info2) != 0;
	}

	public static bool operator >(TrackedBundleVersionInfo info1, TrackedBundleVersionInfo info2)
	{
		return compareTo(info1, info2) > 0;
	}

	public static bool operator <(TrackedBundleVersionInfo info1, TrackedBundleVersionInfo info2)
	{
		return compareTo(info1, info2) < 0;
	}

	public static bool operator >=(TrackedBundleVersionInfo info1, TrackedBundleVersionInfo info2)
	{
		return compareTo(info1, info2) >= 0;
	}

	public static bool operator <=(TrackedBundleVersionInfo info1, TrackedBundleVersionInfo info2)
	{
		return compareTo(info1, info2) <= 0;
	}

	public static bool operator ==(TrackedBundleVersionInfo info1, string info2)
	{
		return string.Compare(info1.version, info2) == 0;
	}

	public static bool operator ==(string info1, TrackedBundleVersionInfo info2)
	{
		return string.Compare(info1, info2.version) == 0;
	}

	public static bool operator !=(TrackedBundleVersionInfo info1, string info2)
	{
		return string.Compare(info1.version, info2) != 0;
	}

	public static bool operator !=(string info1, TrackedBundleVersionInfo info2)
	{
		return string.Compare(info1, info2.version) != 0;
	}

	public static bool operator >(TrackedBundleVersionInfo info1, string info2)
	{
		return string.Compare(info1.version, info2) > 0;
	}

	public static bool operator >(string info1, TrackedBundleVersionInfo info2)
	{
		return string.Compare(info1, info2.version) > 0;
	}

	public static bool operator <(TrackedBundleVersionInfo info1, string info2)
	{
		return string.Compare(info1.version, info2) < 0;
	}

	public static bool operator <(string info1, TrackedBundleVersionInfo info2)
	{
		return string.Compare(info1, info2.version) < 0;
	}

	public static bool operator >=(TrackedBundleVersionInfo info1, string info2)
	{
		return string.Compare(info1.version, info2) >= 0;
	}

	public static bool operator >=(string info1, TrackedBundleVersionInfo info2)
	{
		return string.Compare(info1, info2.version) >= 0;
	}

	public static bool operator <=(TrackedBundleVersionInfo info1, string info2)
	{
		return string.Compare(info1.version, info2) <= 0;
	}

	public static bool operator <=(string info1, TrackedBundleVersionInfo info2)
	{
		return string.Compare(info1, info2.version) <= 0;
	}
}
