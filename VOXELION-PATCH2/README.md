# VOXELION

2D multiplayer sandbox — MonoGame + C#  
**Package:** `com.voxelion.app` · **Title:** VOXELION

---

## Download APK

1. Open **[Actions](https://github.com/atpansi-bot/Voxelion/actions)**
2. Open the latest **green** run
3. Download artifact **`voxelion-android-apk`**
4. Install **`com.voxelion.app-Signed.apk` only**

Uninstall any older VOXELION first.

### Expected flow after install

| Screen | What you see |
|--------|----------------|
| Boot | Emblem + **VOXELION** text + thick progress bar (~2s) |
| Splash | Logo + **ENTER THE FRONTIER** · tap or wait |
| Loading | **LOADING** + percent bar |
| Title | **PLAY / ACCOUNT / SETTINGS** buttons |
| Auth | **CONTINUE AS GUEST** etc. |
| Character | Avatar + **NEXT** |
| Identity | Name + **CONFIRM** |
| Welcome | **YOU ARE READY** → **ENTER** |
| Hub | **CELESTIAL HUB** + **DISCOVER** |

All text uses a built-in pixel font (no external font file).

---

## Apply this patch (Termux)

```bash
cd ~/VOXELION/VOXELION
cp /sdcard/Download/VOXELION-PATCH2.zip .
unzip -o VOXELION-PATCH2.zip

# Copy files (preserve structure)
cp -f VOXELION-PATCH2/Voxelion.Core/UI/PixelFont.cs Voxelion.Core/UI/
cp -f VOXELION-PATCH2/Voxelion.Core/Core/VoxelionGame.cs Voxelion.Core/Core/
cp -f VOXELION-PATCH2/Voxelion.Core/Scenes/*.cs Voxelion.Core/Scenes/
cp -f VOXELION-PATCH2/README.md .

git add -A
git commit -m "Pixel font + readable UI journey Boot to Hub"
git push
```

Username: `atpansi-bot` · Password: Personal Access Token

---

## Notes

- Use **Signed** APK only
- Landscape orientation forced
- Offline-first UI prototype (no paid keys required)
