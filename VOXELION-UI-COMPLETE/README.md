# VOXELION UI Complete — Hub → World

Extends the working Boot→Hub prototype. Does **not** reimplement early flow.

## New / updated files

| File | Purpose |
|------|---------|
| `Data/PlayerProfile.cs` | + `SelectedWorld` for Discover→Connect |
| `Scenes/SceneHub.cs` | Hub HUD polish, bottom nav |
| `Scenes/SceneDiscover.cs` | World cards, tabs, detail overlay, ENTER |
| `Scenes/SceneConnect.cs` | Staged connection + retry/hub recovery |
| `Scenes/SceneWorldLoading.cs` | World identity loading |
| `Scenes/SceneWorld.cs` | Spawn view, HUD fade, pad/buttons, quick menu, tutorial tips |

## Journey after Hub

```
HUB → DISCOVER → (card detail) ENTER
    → CONNECT (stages) → WORLD LOADING → WORLD
    → Quick menu → WORLDS returns to Discover
```

## Termux apply

```bash
cd ~/VOXELION/VOXELION
cp /sdcard/Download/VOXELION-UI-COMPLETE.zip .
unzip -o VOXELION-UI-COMPLETE.zip

cp -f VOXELION-UI-COMPLETE/Voxelion.Core/Data/PlayerProfile.cs Voxelion.Core/Data/
cp -f VOXELION-UI-COMPLETE/Voxelion.Core/Scenes/SceneHub.cs Voxelion.Core/Scenes/
cp -f VOXELION-UI-COMPLETE/Voxelion.Core/Scenes/SceneDiscover.cs Voxelion.Core/Scenes/
cp -f VOXELION-UI-COMPLETE/Voxelion.Core/Scenes/SceneConnect.cs Voxelion.Core/Scenes/
cp -f VOXELION-UI-COMPLETE/Voxelion.Core/Scenes/SceneWorldLoading.cs Voxelion.Core/Scenes/
cp -f VOXELION-UI-COMPLETE/Voxelion.Core/Scenes/SceneWorld.cs Voxelion.Core/Scenes/

git add -A
git commit -m "UI complete: Discover, Connect, WorldLoading, World HUD"
git push
```

Then install **Signed APK** from Actions.
