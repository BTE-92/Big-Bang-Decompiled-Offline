using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Command line builds:
//   Unity.exe -batchmode -nographics -quit -projectPath <p> -executeMethod BuildScripts.Android -logFile <log>
//   Unity.exe ... -executeMethod BuildScripts.Windows64   (Windows/64 bits, -> Builds/Windows64)
//   Unity.exe ... -executeMethod BuildScripts.Windows32   (Windows/32 bits, -> Builds/Windows32)
//   Unity.exe ... -executeMethod BuildScripts.IOS        (exports the Xcode project, which has to be built on a Mac)
// Output goes to <project>/../Builds (override with the BBR_BUILD_DIR environment variable).
public static class BuildScripts
{
	private const string OfflineAndroidId = "com.traplight.bigbangoffline";

	private static string OutDir
	{
		get
		{
			string dir = Environment.GetEnvironmentVariable("BBR_BUILD_DIR");
			if (string.IsNullOrEmpty(dir))
			{
				dir = Path.GetFullPath(Path.Combine(Application.dataPath, "../../Builds"));
			}
			Directory.CreateDirectory(dir);
			return dir;
		}
	}

	private static string[] Scenes
	{
		get { return EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(); }
	}

	private static void Finish(UnityEditor.Build.Reporting.BuildReport report)
	{
		Debug.Log("BUILD result: " + report.summary.result + ", errors: " + report.summary.totalErrors + ", size: " + report.summary.totalSize + ", output: " + report.summary.outputPath);
		EditorApplication.Exit(report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded ? 0 : 1);
	}

	public static void Android()
	{
		string ndk = Environment.GetEnvironmentVariable("BBR_NDK");
		string sdk = Environment.GetEnvironmentVariable("BBR_SDK");
		// defaults: the NDK r16b kept next to the project (Desktop/BBR 2.4.9/Toolchain) and the usual Android SDK folder
		if (string.IsNullOrEmpty(ndk))
		{
			string local = Path.GetFullPath(Path.Combine(Application.dataPath, "../../../Toolchain/android-ndk-r16b"));
			if (Directory.Exists(local)) ndk = local;
		}
		if (string.IsNullOrEmpty(sdk))
		{
			string local = Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA") ?? string.Empty, "Android/Sdk");
			if (Directory.Exists(local)) sdk = local;
		}
		if (!string.IsNullOrEmpty(ndk)) EditorPrefs.SetString("AndroidNdkRoot", ndk);
		if (!string.IsNullOrEmpty(sdk)) EditorPrefs.SetString("AndroidSdkRoot", sdk);

		// side by side with the patched online game (different signature, so it cannot be an update of it)
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, OfflineAndroidId);
		PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
		PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
		EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Internal;
		EditorUserBuildSettings.development = Environment.GetEnvironmentVariable("BBR_DEV") == "1";

		BuildPlayerOptions options = new BuildPlayerOptions();
		options.scenes = Scenes;
		options.target = BuildTarget.Android;
		options.locationPathName = Path.Combine(OutDir, "BigBangRacingOffline.apk");
		options.options = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
		Finish(BuildPipeline.BuildPlayer(options));
	}

	// The 32-bit and 64-bit FMOD / Chipmunk dlls (Plugins/x86, Plugins/x86_64) must not both be enabled for the same target.
	private static void FixWindowsPlugins()
	{
		foreach (string path in AssetDatabase.GetAllAssetPaths())
		{
			bool is32 = path.StartsWith("Assets/Plugins/x86/") && path.EndsWith(".dll");
			bool is64 = path.StartsWith("Assets/Plugins/x86_64/") && path.EndsWith(".dll");
			if (!is32 && !is64)
			{
				continue;
			}
			PluginImporter importer = AssetImporter.GetAtPath(path) as PluginImporter;
			if (importer == null)
			{
				continue;
			}
			bool changed = false;
			if (is32 && importer.GetCompatibleWithPlatform(BuildTarget.StandaloneWindows64))
			{
				importer.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows64, false);
				changed = true;
			}
			if (is32 && importer.GetCompatibleWithEditor())
			{
				importer.SetCompatibleWithEditor(false);
				changed = true;
			}
			if (is64 && importer.GetCompatibleWithPlatform(BuildTarget.StandaloneWindows))
			{
				importer.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows, false);
				changed = true;
			}
			if (changed)
			{
				importer.SaveAndReimport();
			}
		}
	}

	// kept for the old command line: 64-bit
	public static void Windows()
	{
		Windows64();
	}

	public static void Windows64()
	{
		WindowsBuild(BuildTarget.StandaloneWindows64, "Windows64");
	}

	public static void Windows32()
	{
		WindowsBuild(BuildTarget.StandaloneWindows, "Windows32");
	}

	private static void WindowsBuild(BuildTarget target, string folder)
	{
		FixWindowsPlugins();
		// Unity's "Big Bang Configuration" window at launch (resolution, quality, monitor, controls), with the game's banner
		PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Enabled;
		PlayerSettings.defaultIsFullScreen = true;
		PlayerSettings.defaultIsNativeResolution = false;
		PlayerSettings.defaultScreenWidth = 1920;
		PlayerSettings.defaultScreenHeight = 1080;
		PlayerSettings.resizableWindow = true;
		TextureImporter bannerImporter = AssetImporter.GetAtPath("Assets/Editor/LauncherBanner.png") as TextureImporter;
		if (bannerImporter != null && bannerImporter.npotScale != TextureImporterNPOTScale.None)
		{
			// keep the banner's real proportions (no resize to a power of two)
			bannerImporter.npotScale = TextureImporterNPOTScale.None;
			bannerImporter.mipmapEnabled = false;
			bannerImporter.textureCompression = TextureImporterCompression.Uncompressed;
			bannerImporter.SaveAndReimport();
		}
		Texture2D banner = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/LauncherBanner.png");
		if (banner != null)
		{
			PlayerSettings.resolutionDialogBanner = banner;
		}
		BuildPlayerOptions options = new BuildPlayerOptions();
		options.scenes = Scenes;
		options.target = target;
		options.locationPathName = Path.Combine(OutDir, folder + "/BBR 2.4.9.exe");
		options.options = BuildOptions.None;
		Finish(BuildPipeline.BuildPlayer(options));
	}

	public static void IOS()
	{
		// 32-bit (armv7) and 64-bit (arm64) in the same app: needs IL2CPP (Mono only does armv7). 0 = ARMv7, 1 = ARM64, 2 = Universal
		PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
		PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 2);
		// iPad: the status bar (clock) stayed on screen in the game. It has to be hidden in the player settings AND the app must be
		// full screen (an iPad app that allows Split View / Slide Over cannot hide the status bar); IosPostProcess also fixes Info.plist.
		PlayerSettings.statusBarHidden = true;
		PlayerSettings.iOS.requiresFullScreen = true;
		if (string.IsNullOrEmpty(PlayerSettings.iOS.buildNumber) || !System.Text.RegularExpressions.Regex.IsMatch(PlayerSettings.iOS.buildNumber, @"^[0-9]+(\.[0-9]+)*$"))
		{
			PlayerSettings.iOS.buildNumber = "1";
		}
		BuildPlayerOptions options = new BuildPlayerOptions();
		options.scenes = Scenes;
		options.target = BuildTarget.iOS;
		options.locationPathName = Path.Combine(OutDir, "iOS");
		options.options = BuildOptions.None;
		Finish(BuildPipeline.BuildPlayer(options));
	}

	// Needs the native physics (chipmunk) and geometry (auto geometry, libtess) libraries for WebGL, see Embedded/README.md.
	public static void WebGL()
	{
		PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
		PlayerSettings.WebGL.memorySize = 512;
		PlayerSettings.WebGL.dataCaching = true;
		BuildPlayerOptions options = new BuildPlayerOptions();
		options.scenes = Scenes;
		options.target = BuildTarget.WebGL;
		options.locationPathName = Path.Combine(OutDir, "WebGL");
		options.options = BuildOptions.None;
		Finish(BuildPipeline.BuildPlayer(options));
	}
}
