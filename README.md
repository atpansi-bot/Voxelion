# VOXELION

Original 2D multiplayer sandbox — Landscape UI/UX (MonoGame + C#).

## CRITICAL — Why you saw "Hello, Android!"

The APK you installed was the **default .NET Android template**, not VOXELION.
Package was `com.companyname.Voxelion.Android` with layout `activity_main.xml` + TextView "Hello, Android!".

**This source removes that path entirely.**

| Item | Value |
|------|--------|
| Package ID | `com.voxelion.app` |
| App name | `VOXELION` |
| Entry | `MainActivity` → `VoxelionGame` (MonoGame) |
| Orientation | Landscape only |
| UI | Full journey Boot→…→World HUD (no XML Hello layout) |

## Delete old template on your machine / GitHub

```bash
# In your repo root
rm -rf any old Android project that still has:
#   - MainPage.xaml / MainPage.cs
#   - activity_main.xml with Hello text
#   - ApplicationId com.companyname.*

# Replace with THIS Voxelion/ folder completely
```

## Build Desktop

```bash
dotnet restore
dotnet run --project Voxelion.Desktop
```

## Build Android APK

```bash
dotnet workload install android
dotnet publish Voxelion.Android -c Release -f net8.0-android -o ./output
```

APK output: `output/*.apk`  
Install: `adb install -r output/*.apk`

Or push to GitHub — workflow `.github/workflows/build-android.yml` builds and **fails** if Hello template is detected.

## Journey implemented

Boot → Splash → Loading → Title → Auth/Guest → Character → Identity → Welcome → Transition → Hub → Discover → Connect → WorldLoading → World (HUD + movement + interact + tutorial)

## Termux

Edit + git only. Build APK on GitHub Actions (free) or PC with Android SDK.
