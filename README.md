# VOXELION — Complete UI/UX Pipeline (MonoGame + C#)

**Original 2D multiplayer sandbox — Landscape-first UI/UX architecture**  
Platform targets: Android APK (Landscape) + Windows PC (Landscape)  
Engine: MonoGame 3.8.2 + .NET 8  
Design Language: Cinematic fantasy, pixel-art world presentation, refined modern hierarchy, magical light accents.

This repository contains the **complete UI/UX journey** from APK/PC shortcut launch until the player is inside a playable multiplayer world with full HUD, as specified in the MASTER UI/UX DEVELOPMENT PROMPT.

Gameplay systems beyond UI/UX are intentionally not implemented in this phase.

---

## Project Structure

```
VOXELION/
├── VOXELION.sln
├── Content/
│   ├── Content.mgcb
│   ├── Fonts/          ← place .spritefont files
│   ├── Textures/
│   ├── Audio/
│   └── Localization/   ← optional JSON overrides (en/id/ja/zh/ko)
├── src/
│   ├── Voxelion.Core/          ← Design system, UI framework, Input, Localization, State
│   ├── Voxelion.Game/          ← Scenes, Systems, VoxelionGame
│   ├── Voxelion.Desktop/       ← Windows / DesktopGL entry point
│   └── Voxelion.Android/       ← (extend for APK)
```

### Design System (Voxelion.Core.DesignSystem)
- `ColorTokens` — full cinematic palette
- `Typography` — Display / Heading / Body / Label hierarchy
- `Spacing` — 4pt scale
- `MotionSystem` — unified easing + reduced-motion path

### UI Architecture
- `UIElement` base with lifecycle (Update / Draw / HandleInput / Focus / Blur / Open / Close)
- Explicit `ApplicationState` + `OverlayState` machine
- Deterministic `UINavigationManager` (forward / back / escape / modal rules)
- Input abstraction (Touch / Mouse / Keyboard / Gamepad)

### Full Journey Implemented
Boot → Splash → Loading → Title → Session Check → Auth / Guest / Register → Character → Identity → Welcome → Transition → Hub → Discover → Connect → World Loading → Spawn → HUD

Localization built-in for: English, Bahasa Indonesia, 日本語, 中文, 한국어.

Session persistence: local JSON (`voxelion_session.json`). Guest is first-class.

---

## Requirements (Free)

### Build Machine
- .NET 8 SDK (free)
- MonoGame 3.8.2 templates / packages (NuGet, free)
- Visual Studio 2022 Community **or** VS Code + C# Dev Kit **or** JetBrains Rider (free tiers available)
- For Android APK: Android SDK + JDK 17 + MonoGame Android workload

### Optional Free Services
- **MongoDB Atlas** free tier (M0) — if you later add real multiplayer persistence / account linking
- No paid API keys required for this UI/UX phase. All network states are simulated with clear recovery paths.

---

## How to Build & Run (Desktop)

```bash
# 1. Clone / extract
cd VOXELION

# 2. Restore
dotnet restore

# 3. Build fonts (required for production look)
# Open Content/Content.mgcb with MGCB Editor
# Create Regular.spritefont, Bold.spritefont, Display.spritefont
# Build content

# 4. Run Desktop
dotnet run --project src/Voxelion.Desktop
```

Or open `VOXELION.sln` in Visual Studio and set Voxelion.Desktop as startup project.

---

## Android APK

1. Install MonoGame Android workload and Android SDK.
2. Create Android project targeting net8.0-android (or use MonoGame Android template and reference Voxelion.Game + Voxelion.Core).
3. Force landscape orientation in AndroidManifest.
4. `dotnet publish -f net8.0-android -c Release`
5. Sign & install the resulting `.apk`.

Full Android project scaffolding can be added by following official MonoGame Android documentation. Landscape + safe-area + touch controls are already designed for in the architecture.

---

## Managing via Termux (Android)

Termux is excellent for **source editing, git, and lightweight checks**. Full MonoGame + Android APK compilation from pure Termux is not practical (requires full Android NDK/SDK toolchain and heavy disk space).

Recommended Termux workflow:

```bash
# Install basics
pkg update && pkg install git openssh nano vim proot-distro
proot-distro install ubuntu
proot-distro login ubuntu

# Inside Ubuntu proot
apt update && apt install -y git wget curl

# Clone your private repo or scp the source
git clone <your-repo-url> VOXELION
cd VOXELION

# Edit with nano/vim
# Commit & push
git add . && git commit -m "ui polish" && git push

# For actual build: use GitHub Actions free tier or a free cloud Windows/Linux VM
# (GitHub Actions free minutes are sufficient for .NET restore + build)
```

You can keep the entire source under version control from Termux and trigger builds on a free CI (GitHub Actions / Codeberg / etc.).

---

## First-Session Acceptance Checklist

A tester must be able to:

1. Launch
2. See controlled boot + splash + loading (real progress)
3. Reach Title, change language
4. Continue as Guest or Sign In
5. Create character + display name
6. Enter Hub
7. Open Discover, inspect world, enter
8. See connection phases
9. Spawn, understand controls, interaction prompt
10. Open Quick Menu → Inventory → Social → Settings
11. Return to gameplay and exit world safely

All states (error, offline, empty, disabled, loading, success) are defined in the architecture.

---

## Expanding to Full Gameplay

Only after this UI/UX pipeline is visually coherent and navigable:

- Implement voxel world streaming
- Multiplayer networking (use free MongoDB Atlas + free WebSocket / SignalR self-host or free cloud)
- Inventory logic, combat, building, etc.

The UI component library (`UIElement`, `Panel`, `Button`, `InventorySlot`, `WorldCard`, …) is designed for composition so gameplay screens plug in cleanly.

---

## License / Identity

VOXELION is an original identity. Do not copy recognizable UI, terminology, layouts, or visual language of any existing commercial game.

---

**Build VOXELION's complete UI/UX journey first.**  
This repository delivers the production-ready foundation for that journey.
