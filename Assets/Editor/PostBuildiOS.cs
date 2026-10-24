#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

// Makes the app's Documents folder visible in the Files app (Level export / import on iPhone and iPad).
public static class PostBuildiOS
{
	[PostProcessBuild(100)]
	public static void OnPostProcessBuild(BuildTarget target, string path)
	{
		if (target != BuildTarget.iOS)
		{
			return;
		}
		string plistPath = Path.Combine(path, "Info.plist");
		PlistDocument plist = new PlistDocument();
		plist.ReadFromFile(plistPath);
		PlistElementDict root = plist.root;
		root.SetBoolean("UIFileSharingEnabled", true);
		root.SetBoolean("LSSupportsOpeningDocumentsInPlace", true);
		plist.WriteToFile(plistPath);
	}
}
#endif
