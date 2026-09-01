# VOXELION Art Asset Strategy

## Principles

1. **Naming is API** — final art must keep the same path/filename as placeholders.
2. **Proportions locked** — sizes below are the layout contract.
3. **Palette** — Void `#08060E`, Primary `#945CFF`, Secondary `#48C4FF`, Accent `#FFA848`.
4. **Runtime today** — `VisualChrome` draws procedurally; PNGs are drop-in for Content Pipeline later.
5. **No third-party UI mimicry** — original frames, corners, badges only.

## Folder layout

```
Content/Textures/
  Logo/           emblem_64|128|256.png
  Backgrounds/    bg_*_960x540.png, cloud_layer_1..3.png
  Particles/      particle_{dust|spark|ember}_8.png
  UI/             panel, frame_corner_*, button_*, tab_*, badge_*, loading_*
  Icons/          icon_*_32|64.png, arrow_*_48.png
  Worlds/         thumb_*_320x180.png
  Avatar/         frame_64|128.png
  Items/          frame_{rarity}_64.png
  Controls/       pad_*, btn_*, prompt_*
```

## Spec table

| Category | Asset key example | Size | Notes |
|----------|-------------------|------|--------|
| Logo / Emblem | `Logo/emblem_256` | 256², 128², 64² | Square; transparent BG |
| Background | `Backgrounds/bg_void_960x540` | 960×540 | Scale to viewport; 16:9 |
| Cloud layer | `cloud_layer_1..3` | 512×128 | Parallax, alpha |
| Particles | `particle_dust_8` | 8² | Additive-friendly |
| Panel 9-slice | `panel_9slice_64` | 64² | 16px borders suggested |
| Frame corners | `frame_corner_{tl,tr,bl,br}` | 32² | Overlay on panels |
| Button primary/secondary | `button_*_{normal,hover,pressed,disabled}_192x48` | 192×48 | 9-slice later OK |
| Tab | `tab_{idle,active}_128x40` | 128×40 | Active has underline |
| Icons | `icon_{name}_64` / `_32` | 64² / 32² | Transparent, single-color friendly |
| Arrows | `arrow_left/right_48` | 48² | Navigation |
| Close / Confirm | `icon_close_64`, `icon_confirm_64` | 64² | Danger / success tint |
| World thumb | `thumb_{id}_320x180` | 320×180 | 16:9 cards |
| Avatar frame | `frame_64`, `frame_128` | 64² / 128² | Ring only |
| Item frame | `frame_{common…legendary}_64` | 64² | Rarity edge color |
| Badges | `badge_{info,success,warning,error}_24` | 24² | Notification dots |
| Touch pad | `pad_base_128`, `pad_knob_64` | 128 / 64 | Circular |
| Action buttons | `btn_jump/action/interact_64` | 64² | Circular |
| Prompts | `prompt_tap/hold/key_e_128x32` | 128×32 | Caption strip |
| Loading bar | `loading_bar_256x16` | 256×16 | Fill region center |
| Spinner | `loading_spinner_64` | 64² | Rotatable |

## Replace workflow

1. Export final PNG (same dimensions, RGBA).
2. Overwrite file under `Content/Textures/...`.
3. Rebuild MGCB / pipeline if used.
4. **Do not** change `AssetCatalog` string keys unless updating all call sites.

## Code reference

```csharp
// Path constants
AssetCatalog.Logo.Emblem128
AssetCatalog.Icons.Play
AssetCatalog.UI.Button("primary", "hover")
AssetCatalog.Worlds.Thumb("aether_reach")
```

Until ContentManager loads textures, UI continues via `VisualChrome` + `DesignTokens`.
