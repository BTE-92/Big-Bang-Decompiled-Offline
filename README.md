# Big Bang 2.4.9 Decompiled

This repo contains an updated version of the existing **Big Bang Racing 2.4.9 decompilation**, migrated to **Unity 2018.4.8f1**.

> ✅ **THIS VERSION IS FULLY OFFLINE.**
>
> The game server is embedded in the client: no internet connection, no server PC, no DNS and no certificate needed.
> It runs on Android, PC (Windows) and iOS.

# 📴 Offline version

* **Embedded server**: `Assets/Plugins/BBR249/BBR249.Embedded.dll` (the C# sources are in the `Embedded` folder at the root of this repository).
  Data is stored in SQLite under `persistentDataPath\BBROffline\Data`
  (PC: `AppData\LocalLow\Traplight\Big Bang\BBROffline\Data`).
  The game talks to the server through `GameWWW.cs` (replaces `WWW`) and `OfflineServer.cs` (startup);
  the hosts `bbr.local`, `woeprod.traplightgames.com` and `graph.facebook.com` are answered locally.
* **Client patches** (`Assets/Scripts/Assembly-CSharp/BBRClient`): admin mode, Dev Items (with hand-drawn icons), editor changes,
  level Import/Export, translations in 10 languages, a **HATS** button (hat chooser) and crash fixes.
* **Fresh profile**: 2,000,000 coins and diamonds and 4 starter levels ("Starter ..."); the adventure map serves the published Race levels.
* **Builds** (Unity 2018.4.8f1, `Assets/Editor/BuildScripts.cs`):
  `Unity.exe -batchmode -nographics -quit -projectPath <project> -executeMethod BuildScripts.<Android|Windows|IOS>`
  Output goes to `<project>/../../Builds` (override with the `BBR_BUILD_DIR` environment variable). Android uses NDK r16b (`BBR_NDK`) and the Android SDK (`BBR_SDK`).
  iOS exports an Xcode project that has to be compiled on a Mac.
* See `Embedded/README.md` for the full details.

# ⚠️ Important Information Before You Start

* You can build for Android (All architectures), Windows (because I restored the plugins 😁) and iOS (Xcode project export; it needs a Mac to compile). If you encounter any issues, please open an issue on GitHub to report them if related to Unity.
* Some enhancements have been made to this source, including:

  * Upgrade from FMOD 1.07.03 to FMOD 1.10.2
  * Changes to fix some specific issues (PhysX mesh cooking, ParticleSeedRandomiser)

# 🛠️ How to Open the Project

## 1. Install Unity 2018.4.8f1

I recommend using **Unity 2018.4.8f1**, as this is the exact version I used when migrating the project.

Using a later or earlier version may cause compatibility issues.

[Click here to download Unity 2018.4.8f1](https://unity.com/releases/editor/whats-new/2018.4.8f1#installers)

## 2. Open the Project

After installing Unity 2018.4.8f1, open this repository as a Unity project.


# 🎮 Controls

If you're running the game in the **Unity Editor on PC**, these are the controls:

* **A** — Zoom in
* **Z** — Zoom out
* **Left Shift** — Draw
* **Left Alt** — Erase
* **Left / Right Arrow Keys** — Lean Back/Lean Forward
* **Up / Down Arrow Keys** — Throttle/Brake

# Contact

For questions or general inquiries, feel free to reach out to me on Discord.

- **Discord:** `dodonickey`

Please keep GitHub Issues reserved for bug reports and relevant technical issues.


> yes I took this from my WOE repo and changed it up a bit.
