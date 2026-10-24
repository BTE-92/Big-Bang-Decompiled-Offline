# Embedded offline server

`BBR249.Embedded.csproj` (netstandard2.0) is the Big Bang Racing 2.4.9 server compiled as a library that runs **inside the game**.
The Unity project loads it from `Assets/Plugins/BBR249/BBR249.Embedded.dll`, so the game needs no internet connection and no server PC.

## Layout

* `src/Api`, `src/Core`, `src/Data` – the 2.4.9 server routes and SQL (see `ENDPOINTS.md`).
* `src/Compat/Json.cs` – replacement for `System.Text.Json.Nodes`, which Unity 2018 does not have.
* `src/Data/Db.cs` – SQLite through P/Invoke (`sqlite3`; on iOS SQLite is compiled into the app, see `BBRClient/IosSqlite.cs`).
* `src/Net/Ctx.cs`, `Router.cs` – request handling without ASP.NET; the game hands its requests straight to the router.
* `src/Resources/seed_levels.json` – the 4 starter levels of a fresh profile ("Starter ...").
* `patches/` – the client patches (diff against the original 2.4.9 client, helper script, notes).

## In the game

`GameWWW.cs` replaces `WWW` in `HttpC` / `HttpS`; `OfflineServer.cs` starts the server. The hosts `bbr.local`,
`woeprod.traplightgames.com` and `graph.facebook.com` are answered locally. Data lives in `persistentDataPath/BBROffline/Data`
(PC: `AppData\LocalLow\Traplight\Big Bang\BBROffline\Data`, Android: `/sdcard/Android/data/<package>/files/BBROffline/Data`).

## Rebuilding the server

```
cd Embedded
dotnet build -c Release
```

then copy `bin/Release/netstandard2.0/BBR249.Embedded.dll` to `Assets/Plugins/BBR249/`.

## Building the game

Unity 2018.4.8f1, `Assets/Editor/BuildScripts.cs`:

```
Unity.exe -batchmode -nographics -quit -projectPath <project> -executeMethod BuildScripts.<Android|Windows|IOS>
```

* Output goes to `<project>/../../Builds` unless `BBR_BUILD_DIR` is set.
* Android needs NDK r16b (`BBR_NDK`) and the Android SDK with platform 28 and build-tools 28.0.3 (`BBR_SDK`).
  `libsqlite3.so` for arm64-v8a, armeabi-v7a and x86 is already in `Assets/Plugins/Android/libs`.
* iOS exports an Xcode project (`Unity-iPhone.xcodeproj`) that has to be compiled on a Mac; SQLite is compiled with the app
  (`Assets/Plugins/iOS/sqlite/sqlite3.c`).
* After test runs on a PC, delete `AppData\LocalLow\Traplight\Big Bang` and the registry key `HKCU\Software\Traplight\Big Bang`
  to start from a fresh installation.
