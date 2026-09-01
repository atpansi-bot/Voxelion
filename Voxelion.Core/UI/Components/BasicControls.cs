using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace Voxelion.Core.UI.Components;

public sealed class Panel : UIElement
{
    public bool Elevated { get; set; }
    public bool Glow { get; set; }
    public string Title { get; set; } = "";

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        VisualChrome.Panel(game, sb, Bounds, Elevated, Glow || Is(UIState.Focused));
        if (!string.IsNullOrEmpty(Title))
            game.DrawText(sb, Title, new Vector2(Bounds.X + 12, Bounds.Y + 10),
                DesignTokens.Semantic.TextPrimary, DesignTokens.Typography.Heading);
    }
}

public sealed class PanelHeader : UIElement
{
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        game.DrawRect(sb, Bounds, DesignTokens.Semantic.SurfaceElevated);
        game.DrawRect(sb, Bounds.X, Bounds.Bottom - 2, Bounds.Width, 2, DesignTokens.Semantic.Primary * 0.5f);
        game.DrawText(sb, Title, new Vector2(Bounds.X + 12, Bounds.Y + 10),
            DesignTokens.Semantic.TextPrimary, DesignTokens.Typography.Subheading);
        if (!string.IsNullOrEmpty(Subtitle))
            game.DrawText(sb, Subtitle, new Vector2(Bounds.X + 12, Bounds.Y + 36),
                DesignTokens.Semantic.TextMuted, DesignTokens.Typography.Caption);
    }
}

public sealed class Button : UIElement
{
    public string Label { get; set; } = "";
    public bool Primary { get; set; } = true;
    public float TextScale { get; set; } = DesignTokens.Typography.Button;

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        VisualChrome.Button(game, sb, Bounds, Label, Primary && Enabled,
            Is(UIState.Hover) || Is(UIState.Focused), Is(UIState.Pressed), TextScale);
        if (Is(UIState.Loading))
            VisualChrome.Spinner(game, sb, new Vector2(Bounds.Center.X, Bounds.Center.Y), 10, time);
        if (Is(UIState.Disabled))
            game.DrawRect(sb, Bounds, DesignTokens.Semantic.Overlay * 0.35f);
    }
}

public sealed class IconButton : UIElement
{
    public string Glyph { get; set; } = "*"; // pixel-font glyph placeholder
    public bool Primary { get; set; }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        var fill = Primary ? DesignTokens.Semantic.Primary : DesignTokens.Semantic.SurfaceElevated;
        fill = StateTint(State, fill);
        game.DrawRect(sb, Bounds, fill);
        game.DrawBorder(sb, Bounds,
            Is(UIState.Focused) ? DesignTokens.Semantic.Focus : DesignTokens.Semantic.Border, 2);
        var sz = game.MeasureText(Glyph, 1.8f);
        game.DrawText(sb, Glyph,
            new Vector2(Bounds.X + (Bounds.Width - sz.X) * 0.5f, Bounds.Y + (Bounds.Height - sz.Y) * 0.5f),
            DesignTokens.Semantic.TextPrimary, 1.8f);
    }
}

public sealed class Label : UIElement
{
    public string Text { get; set; } = "";
    public float Scale { get; set; } = DesignTokens.Typography.Body;
    public XnaColor? Color { get; set; }
    public bool Center { get; set; }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible || string.IsNullOrEmpty(Text)) return;
        var c = Color ?? DesignTokens.Semantic.TextPrimary;
        if (Is(UIState.Disabled)) c = DesignTokens.Semantic.TextDisabled;
        var sz = game.MeasureText(Text, Scale);
        float x = Center ? Bounds.X + (Bounds.Width - sz.X) * 0.5f : Bounds.X;
        float y = Bounds.Y + (Bounds.Height - sz.Y) * 0.5f;
        game.DrawText(sb, Text, new Vector2(x, y), c, Scale);
    }
}

public sealed class TextBlock : UIElement
{
    public string Text { get; set; } = "";
    public float Scale { get; set; } = DesignTokens.Typography.Body;
    public XnaColor? Color { get; set; }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        game.DrawText(sb, Text ?? "", new Vector2(Bounds.X, Bounds.Y),
            Color ?? DesignTokens.Semantic.TextSecondary, Scale);
    }
}

public sealed class Divider : UIElement
{
    public bool Vertical { get; set; }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        if (Vertical)
            game.DrawRect(sb, Bounds.X, Bounds.Y, Math.Max(1, Bounds.Width), Bounds.Height, DesignTokens.Semantic.Border);
        else
            game.DrawRect(sb, Bounds.X, Bounds.Y, Bounds.Width, Math.Max(1, Bounds.Height), DesignTokens.Semantic.Border);
    }
}

public sealed class ProgressBar : UIElement
{
    public float Progress { get; set; }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        VisualChrome.ProgressCrystal(game, sb, Bounds, Progress);
        if (Is(UIState.Error))
            game.DrawBorder(sb, Bounds, DesignTokens.Semantic.Error, 2);
        if (Is(UIState.Success))
            game.DrawBorder(sb, Bounds, DesignTokens.Semantic.Success, 2);
    }
}

public sealed class Toggle : UIElement
{
    public bool On { get; set; }

    public override void Update(InputState input, bool focused)
    {
        base.Update(input, focused);
        if (WasActivated(input, focused)) On = !On;
        if (On) State |= UIState.Selected;
        else State &= ~UIState.Selected;
    }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        var track = Bounds;
        var fill = On ? DesignTokens.Semantic.Primary : DesignTokens.Semantic.SurfaceSunken;
        if (!Enabled) fill = DesignTokens.Semantic.Disabled;
        game.DrawRect(sb, track, fill);
        game.DrawBorder(sb, track, Is(UIState.Focused) ? DesignTokens.Semantic.Focus : DesignTokens.Semantic.Border, 2);
        int knob = track.Height - 6;
        int kx = On ? track.Right - knob - 3 : track.X + 3;
        game.DrawRect(sb, kx, track.Y + 3, knob, knob, DesignTokens.Semantic.TextPrimary);
    }
}

public sealed class Checkbox : UIElement
{
    public bool Checked { get; set; }
    public string Label { get; set; } = "";

    public override void Update(InputState input, bool focused)
    {
        base.Update(input, focused);
        if (WasActivated(input, focused)) Checked = !Checked;
        if (Checked) State |= UIState.Selected;
        else State &= ~UIState.Selected;
    }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        var box = new Rectangle(Bounds.X, Bounds.Y + (Bounds.Height - 24) / 2, 24, 24);
        game.DrawRect(sb, box, DesignTokens.Semantic.SurfaceElevated);
        game.DrawBorder(sb, box, Is(UIState.Focused) ? DesignTokens.Semantic.Focus : DesignTokens.Semantic.Border, 2);
        if (Checked)
            game.DrawRect(sb, box.X + 5, box.Y + 5, 14, 14, DesignTokens.Semantic.Primary);
        if (!string.IsNullOrEmpty(Label))
            game.DrawText(sb, Label, new Vector2(box.Right + 10, Bounds.Y + 8),
                DesignTokens.Semantic.TextPrimary, DesignTokens.Typography.Body);
    }
}

public sealed class Radio : UIElement
{
    public bool Selected { get; set; }
    public string Label { get; set; } = "";
    public string Group { get; set; } = "default";

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        int s = 22;
        var box = new Rectangle(Bounds.X, Bounds.Y + (Bounds.Height - s) / 2, s, s);
        game.DrawRect(sb, box, DesignTokens.Semantic.SurfaceElevated);
        game.DrawBorder(sb, box, Is(UIState.Focused) ? DesignTokens.Semantic.Focus : DesignTokens.Semantic.Border, 2);
        if (Selected)
            game.DrawRect(sb, box.X + 5, box.Y + 5, s - 10, s - 10, DesignTokens.Semantic.Secondary);
        if (!string.IsNullOrEmpty(Label))
            game.DrawText(sb, Label, new Vector2(box.Right + 10, Bounds.Y + 6),
                DesignTokens.Semantic.TextPrimary, DesignTokens.Typography.Body);
    }
}

public sealed class Slider : UIElement
{
    public float Value { get; set; } // 0..1
    public float Min { get; set; }
    public float Max { get; set; } = 1f;

    public override void Update(InputState input, bool focused)
    {
        base.Update(input, focused);
        if (!Enabled) return;
        if (Bounds.Contains(input.PointerPosition) && input.IsPointerDown)
        {
            float t = (input.PointerPosition.X - Bounds.X) / (float)Math.Max(1, Bounds.Width);
            Value = MathHelper.Clamp(t, 0f, 1f);
        }
        if (focused)
        {
            if (input.NavLeft) Value = MathHelper.Clamp(Value - 0.05f, 0f, 1f);
            if (input.NavRight) Value = MathHelper.Clamp(Value + 0.05f, 0f, 1f);
        }
    }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        int cy = Bounds.Center.Y;
        game.DrawRect(sb, Bounds.X, cy - 3, Bounds.Width, 6, DesignTokens.Semantic.SurfaceSunken);
        int fillW = (int)(Bounds.Width * Value);
        game.DrawRect(sb, Bounds.X, cy - 3, fillW, 6, DesignTokens.Semantic.Primary);
        int kx = Bounds.X + fillW - 8;
        game.DrawRect(sb, kx, cy - 10, 16, 20,
            Is(UIState.Focused) ? DesignTokens.Semantic.Secondary : DesignTokens.Semantic.TextPrimary);
    }

    public float Mapped => Min + (Max - Min) * Value;
}

public class TextField : UIElement
{
    public string Value { get; set; } = "";
    public string Placeholder { get; set; } = "";
    public int MaxLength { get; set; } = 32;
    public bool IsPassword { get; set; }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        var border = Is(UIState.Error) ? DesignTokens.Semantic.Error
            : Is(UIState.Focused) ? DesignTokens.Semantic.Focus
            : DesignTokens.Semantic.Border;
        game.DrawRect(sb, Bounds, DesignTokens.Semantic.SurfaceElevated);
        game.DrawBorder(sb, Bounds, border, Is(UIState.Focused) ? 2 : 1);
        string show = string.IsNullOrEmpty(Value) ? Placeholder
            : (IsPassword ? new string('*', Value.Length) : Value);
        var col = string.IsNullOrEmpty(Value) ? DesignTokens.Semantic.TextMuted : DesignTokens.Semantic.TextPrimary;
        game.DrawText(sb, show, new Vector2(Bounds.X + 10, Bounds.Y + 14), col, DesignTokens.Typography.Body);
        if (Is(UIState.Focused) && (int)(time * 2) % 2 == 0)
        {
            var sz = game.MeasureText(string.IsNullOrEmpty(Value) ? "" : show, DesignTokens.Typography.Body);
            game.DrawRect(sb, Bounds.X + 10 + sz.X + 2, Bounds.Y + 12, 2, Bounds.Height - 24, DesignTokens.Semantic.Secondary);
        }
    }
}

public sealed class SearchField : TextField
{
    public SearchField() { Placeholder = "SEARCH"; }
}
