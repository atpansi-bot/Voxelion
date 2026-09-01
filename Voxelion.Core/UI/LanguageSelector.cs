using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.Localization;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.UI;

/// <summary>
/// Compact language chip (Title) or expanded list (Settings).
/// Layout is code/index based — no language-specific coordinate tables.
/// </summary>
public sealed class LanguageSelector
{
    public enum Mode { Chip, List }

    public Mode Presentation { get; set; } = Mode.Chip;
    public Rectangle Bounds { get; set; }
    public float RowHeight { get; set; } = 44f;
    public float ChipWidth { get; set; } = 96f;

    private readonly List<Rectangle> _rows = new();

    /// <summary>Returns true if language changed this frame.</summary>
    public bool Update(InputState input, LocalizationManager loc, Action<Language>? onChanged = null)
    {
        Rebuild(loc.Current);
        if (!input.IsPointerReleased) return false;

        if (Presentation == Mode.Chip)
        {
            if (Bounds.Contains(input.PointerPosition))
            {
                int idx = (LanguageInfo.IndexOf(loc.Current) + 1) % LanguageInfo.All.Length;
                var next = LanguageInfo.All[idx].Id;
                loc.Current = next;
                onChanged?.Invoke(next);
                return true;
            }
            return false;
        }

        for (int i = 0; i < _rows.Count; i++)
        {
            if (_rows[i].Contains(input.PointerPosition))
            {
                var next = LanguageInfo.All[i].Id;
                if (next == loc.Current) return false;
                loc.Current = next;
                onChanged?.Invoke(next);
                return true;
            }
        }
        return false;
    }

    public void Draw(VoxelionGame game, SpriteBatch sb, LocalizationManager loc, InputState? input = null)
    {
        Rebuild(loc.Current);
        if (Presentation == Mode.Chip)
        {
            var info = LanguageInfo.Get(loc.Current);
            VisualChrome.Panel(game, sb, Bounds, elevated: true);
            // Globe mark (simple)
            game.DrawRect(sb, Bounds.X + 10, Bounds.Y + 12, 14, 14, DesignTokens.Semantic.Secondary * 0.8f);
            game.DrawBorder(sb, new Rectangle(Bounds.X + 10, Bounds.Y + 12, 14, 14), DesignTokens.Semantic.Border, 1);
            string code = info.Code;
            var sz = game.MeasureText(code, TypeScale.Label);
            game.DrawText(sb, code,
                new Vector2(Bounds.X + 32, Bounds.Y + (Bounds.Height - sz.Y) * 0.5f),
                DesignTokens.Semantic.TextPrimary, TypeScale.Label);
            return;
        }

        for (int i = 0; i < LanguageInfo.All.Length; i++)
        {
            var info = LanguageInfo.All[i];
            var r = _rows[i];
            bool selected = info.Id == loc.Current;
            bool hover = input != null && r.Contains(input.PointerPosition);
            VisualChrome.Panel(game, sb, r, elevated: selected || hover, glow: selected);

            // Code badge
            var badge = new Rectangle(r.X + 10, r.Y + 10, 40, r.Height - 20);
            game.DrawRect(sb, badge, selected ? DesignTokens.Semantic.Primary : DesignTokens.Semantic.SurfaceSunken);
            var csz = game.MeasureText(info.Code, TypeScale.Caption);
            game.DrawText(sb, info.Code,
                new Vector2(badge.X + (badge.Width - csz.X) * 0.5f, badge.Y + (badge.Height - csz.Y) * 0.5f),
                DesignTokens.Semantic.TextPrimary, TypeScale.Caption);

            // Native name (CJK may use placeholder cells — still correct layout width)
            string native = info.NativeName;
            game.DrawText(sb, native, new Vector2(r.X + 60, r.Y + 14),
                DesignTokens.Semantic.TextPrimary, TypeScale.Body);

            // Localized name key as secondary if different presentation needed
            string locName = loc.T(info.NameKey);
            if (locName != info.NameKey && locName != native)
            {
                game.DrawText(sb, locName, new Vector2(r.X + 60, r.Y + 34),
                    DesignTokens.Semantic.TextMuted, TypeScale.Caption);
            }

            if (selected)
                game.DrawRect(sb, r.X, r.Y, 4, r.Height, DesignTokens.Semantic.Secondary);
        }
    }

    public float RequiredHeight() =>
        Presentation == Mode.Chip ? Bounds.Height : LanguageInfo.All.Length * RowHeight + (LanguageInfo.All.Length - 1) * 6;

    private void Rebuild(Language current)
    {
        _rows.Clear();
        if (Presentation == Mode.Chip)
        {
            if (Bounds.Width < 8)
                Bounds = new Rectangle(Bounds.X, Bounds.Y, (int)ChipWidth, 40);
            return;
        }
        for (int i = 0; i < LanguageInfo.All.Length; i++)
        {
            _rows.Add(new Rectangle(
                Bounds.X,
                Bounds.Y + (int)(i * (RowHeight + 6)),
                Bounds.Width,
                (int)RowHeight));
        }
    }
}
