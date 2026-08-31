# VOXELION

2D multiplayer sandbox — MonoGame + C#  
**Package ID:** `com.voxelion.app`  
**Platform:** Android (arm64) + DesktopGL

---

## Download APK (paling mudah)

1. Buka **[Actions](https://github.com/atpansi-bot/Voxelion/actions)**
2. Pilih run **hijau terbaru** (workflow *Build VOXELION Android APK*)
3. Scroll ke bawah → **Artifacts**
4. Download **`voxelion-android-apk`**
5. Ekstrak zip → pasang file **`com.voxelion.app-Signed.apk`**

> Hanya pakai file **\*-Signed.apk**.  
> File `com.voxelion.app.apk` (tanpa Signed) **tidak bisa di-install**.

### Install di Android
```text
1. Uninstall versi lama VOXELION (jika ada)
2. Izinkan "Install from unknown sources" untuk File Manager / Chrome
3. Tap com.voxelion.app-Signed.apk → Install
4. Buka app — harus landscape, layar gelap cinematic + emblem ungu/cyan
```

---

## Apa yang terlihat saat benar

| Tahap | Tampilan |
|-------|----------|
| Boot | Background gelap, partikel, emblem ungu, progress bar |
| Splash | Emblem + "VOXELION" + TAP TO CONTINUE |
| Loading → Title | Menu PLAY / ACCOUNT / SETTINGS |
| Auth | CONTINUE AS GUEST / SIGN IN / CREATE ACCOUNT |

Jika **layar hitam total** lebih dari 5 detik: uninstall → pasang ulang Signed APK dari Actions terbaru.

---

## Build sendiri (Termux / PC)

### Syarat
- GitHub account + Personal Access Token (repo scope)
- Repo: `https://github.com/atpansi-bot/Voxelion`

### Termux — update source lalu push
```bash
cd ~/VOXELION/VOXELION

# Salin patch (sesuaikan path Download)
cp -rf /sdcard/Download/VOXELION-PATCH/* .
# Pastikan folder tersembunyi .github ikut
cp -rf /sdcard/Download/VOXELION-PATCH/.github . 2>/dev/null || true

git add -A
git status
git commit -m "Apply VOXELION patch: workflow verify + black screen fix"
git push
```
Username: `atpansi-bot`  
Password: **Personal Access Token** (bukan password GitHub biasa)

Tunggu Actions hijau → download artifact.

### PC (opsional)
```bash
dotnet workload install android
dotnet publish Voxelion.Android/Voxelion.Android.csproj \
  -c Release -f net8.0-android34.0 -r android-arm64 -o ./output \
  -p:CheckEolTargetFramework=false -p:CheckEolWorkloads=false
```

---

## Struktur proyek

```
Voxelion.Android/     → entry Android (MainActivity, Manifest, icons)
Voxelion.Core/        → game + UI scenes + design system
Voxelion.Desktop/     → DesktopGL (opsional)
.github/workflows/    → CI build APK otomatis
```

**Tidak ada template "Hello, Android!".**  
Surface 100% MonoGame `AndroidGameActivity`.

---

## Clarifikasi teknis

| Item | Nilai |
|------|--------|
| ApplicationId | `com.voxelion.app` |
| ApplicationTitle | `VOXELION` |
| TFM | `net8.0-android34.0` |
| MonoGame | `3.8.5.1` |
| Orientasi | Landscape (sensor) |
| Backend | Offline-first (siap MongoDB Atlas gratis nanti) |
| Key / API | Tidak wajib untuk build APK saat ini |

---

## Lisensi & kontribusi

Proyek milik maintainer repo.  
Pull request / issue: buka tab Issues di GitHub.

---

*VOXELION — Enter the Frontier*
