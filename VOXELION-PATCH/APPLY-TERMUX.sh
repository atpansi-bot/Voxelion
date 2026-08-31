#!/data/data/com.termux/files/usr/bin/bash
# Terapkan VOXELION-PATCH ke repo lalu push
set -e
cd ~/VOXELION/VOXELION

PATCH_DIR=""
for d in \
  /sdcard/Download/VOXELION-PATCH \
  ~/storage/downloads/VOXELION-PATCH \
  ~/Download/VOXELION-PATCH \
  ./VOXELION-PATCH
do
  if [ -d "$d" ]; then PATCH_DIR="$d"; break; fi
done

if [ -z "$PATCH_DIR" ]; then
  echo "Folder VOXELION-PATCH tidak ditemukan."
  echo "Letakkan hasil ekstrak zip di /sdcard/Download/VOXELION-PATCH"
  exit 1
fi

echo "Patch dari: $PATCH_DIR"

cp -f "$PATCH_DIR/Voxelion.Android/MainActivity.cs" Voxelion.Android/MainActivity.cs
cp -f "$PATCH_DIR/Voxelion.Core/Core/VoxelionGame.cs" Voxelion.Core/Core/VoxelionGame.cs
cp -f "$PATCH_DIR/Voxelion.Core/Input/InputState.cs" Voxelion.Core/Input/InputState.cs
cp -f "$PATCH_DIR/README.md" README.md

mkdir -p .github/workflows
cp -f "$PATCH_DIR/.github/workflows/build-android.yml" .github/workflows/build-android.yml

echo "=== File ter-update ==="
ls -la Voxelion.Android/MainActivity.cs
ls -la Voxelion.Core/Core/VoxelionGame.cs
ls -la Voxelion.Core/Input/InputState.cs
ls -la .github/workflows/build-android.yml
ls -la README.md

git add -A
git status
git commit -m "Apply VOXELION patch: fix workflow verify + black screen + README" || true
echo "Jalankan: git push"
echo "Username: atpansi-bot"
echo "Password: Personal Access Token"
