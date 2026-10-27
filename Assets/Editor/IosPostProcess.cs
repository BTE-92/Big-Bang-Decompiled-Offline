using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

// Info.plist of the exported Xcode project (plain text editing, no dependency on the iOS module):
//  - the status bar (clock) must stay hidden on iPad: UIStatusBarHidden = true, UIViewControllerBasedStatusBarAppearance = false and
//    UIRequiresFullScreen = true (otherwise Split View / Slide Over is allowed and iOS keeps the bar)
//  - the app's Documents folder (exported levels, import) is shown in the Files app: UIFileSharingEnabled + LSSupportsOpeningDocumentsInPlace
//  - 32-bit and 64-bit: no "armv7" / "arm64" entry in UIRequiredDeviceCapabilities (an "armv7" entry would exclude the 64-bit only iPhones)
public static class IosPostProcess
{
	[PostProcessBuild(1000)]
	public static void OnPostprocessBuild(BuildTarget target, string path)
	{
		if (target != BuildTarget.iOS)
		{
			return;
		}
		string plistPath = Path.Combine(path, "Info.plist");
		if (!File.Exists(plistPath))
		{
			Debug.LogWarning("IosPostProcess: no Info.plist in " + path);
			return;
		}
		string text = File.ReadAllText(plistPath);
		text = SetBool(text, "UIStatusBarHidden", true);
		text = SetBool(text, "UIViewControllerBasedStatusBarAppearance", false);
		text = SetBool(text, "UIRequiresFullScreen", true);
		text = SetBool(text, "UIFileSharingEnabled", true);
		text = SetBool(text, "LSSupportsOpeningDocumentsInPlace", true);
		text = Regex.Replace(text, @"\s*<string>(armv7|arm64)</string>", string.Empty);
		File.WriteAllText(plistPath, text);
		Debug.Log("IosPostProcess: Info.plist fixed (status bar hidden, full screen, Files app access, no architecture restriction)");
	}

	private static string SetBool(string text, string key, bool value)
	{
		string element = value ? "<true />" : "<false />";
		Regex existing = new Regex("<key>" + key + @"</key>\s*<(true|false)\s*/>");
		if (existing.IsMatch(text))
		{
			return existing.Replace(text, "<key>" + key + "</key>\n    " + element);
		}
		int end = text.LastIndexOf("</dict>");
		if (end < 0)
		{
			return text;
		}
		return text.Substring(0, end) + "  <key>" + key + "</key>\n    " + element + "\n" + text.Substring(end);
	}
}
