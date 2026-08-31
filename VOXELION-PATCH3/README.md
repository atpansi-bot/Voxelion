# VOXELION — PATCH3 (critical fix)

## Bug that caused the stuck emblem screen

`ApplicationStateMachine` started at `Boot`.  
`LoadContent()` called `TransitionTo(Boot)` which **no-oped** (`next == Current`).  
`OnStateChanged` never fired → `_currentScene` stayed **null** → only the static fallback emblem.

## Fix

- Initial state is now `None`
- `TransitionTo(Boot)` always fires once
- Safety net forces Boot scene if still null

## Expected flow after this APK

1. **Boot** — VOXELION + phase text + thick progress bar (~2s)
2. **Splash** — ENTER THE FRONTIER · tap or wait
3. **Loading** — percent bar
4. **Title** — PLAY / ACCOUNT / SETTINGS
5. **PLAY** → Character → Identity → Welcome → **Hub**

## Termux apply

```bash
cd ~/VOXELION/VOXELION
cp /sdcard/Download/VOXELION-PATCH3.zip .
unzip -o VOXELION-PATCH3.zip

mkdir -p Voxelion.Core/UI Voxelion.Core/Core Voxelion.Core/Scenes
cp -f VOXELION-PATCH3/Voxelion.Core/Core/ApplicationState.cs Voxelion.Core/Core/
cp -f VOXELION-PATCH3/Voxelion.Core/Core/VoxelionGame.cs Voxelion.Core/Core/
cp -f VOXELION-PATCH3/Voxelion.Core/UI/PixelFont.cs Voxelion.Core/UI/
cp -f VOXELION-PATCH3/Voxelion.Core/Scenes/*.cs Voxelion.Core/Scenes/
cp -f VOXELION-PATCH3/README.md .

git add -A
git commit -m "CRITICAL: fix stuck Boot — state machine starts at None"
git push
```

Username: `atpansi-bot`

Then: Actions → green → download **Signed APK** → uninstall old → install new.
