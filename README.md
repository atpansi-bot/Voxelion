# VOXELION — UI/UX Complete (MonoGame + C#)

Original 2D multiplayer sandbox interface. Landscape-first for Android + PC.

## What was fixed / expanded

- Completed missing scenes: **Hub, Discover, Connect, WorldLoading, World**
- Wired full journey: Boot → Splash → Loading → Title → Auth → Character → Identity → Welcome → Transition → Hub → Discover → Connect → WorldLoading → World
- Desktop entry uses `VoxelionGame` (not empty Game1 template)
- Android project: package **`com.voxelion.app`** (not `com.companyname.*`)
- Localization keys for connection errors + tutorials
- Design tokens, input abstraction, session service, state machine

## Build Desktop

```bash
dotnet restore
dotnet run --project Voxelion.Desktop
```

## Build Android APK

Requires: .NET 8, Android SDK 34, JDK 17, MonoGame Android workload.

```bash
dotnet workload install android
dotnet publish Voxelion.Android -c Release -f net8.0-android -o ./output
```

Signed APK path depends on your keystore. Use:

```bash
dotnet publish Voxelion.Android -c Release -p:AndroidKeyStore=true \
  -p:AndroidSigningKeyStore=your.keystore \
  -p:AndroidSigningKeyAlias=alias \
  -p:AndroidSigningKeyPass=pass \
  -p:AndroidSigningStorePass=pass
```

## APK note

The uploaded `com.companyname.Voxelion.Android-Signed.apk` was a **blank template shell** (default companyname package, almost no game assemblies). This source tree replaces that with the real VOXELION UI pipeline and package id `com.voxelion.app`.

## Termux

Edit + git from Termux. Build APK on GitHub Actions / PC with Android SDK.

## Free backend (later)

MongoDB Atlas M0 free tier for accounts / worlds when you leave pure UI phase.
