using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace Voxelion.Core.UI;

/// <summary>
/// Focus graph for keyboard/controller. Touch/mouse can still hit-test directly.
/// One information architecture — focus is an additional presentation layer.
/// </summary>
public sealed class FocusNav
{
    private readonly List<FocusTarget> _targets = new();
    private int _index = -1;

    public int Count => _targets.Count;
    public int Index => _index;
    public FocusTarget? Current => _index >= 0 && _index < _targets.Count ? _targets[_index] : null;
    public string? CurrentId => Current?.Id;

    public void Clear()
    {
        _targets.Clear();
        _index = -1;
    }

    public void Register(string id, Rectangle bounds, int order = -1, bool enabled = true)
    {
        if (!enabled) return;
        _targets.Add(new FocusTarget(id, bounds, order >= 0 ? order : _targets.Count));
    }

    public void EndRegister()
    {
        _targets.Sort((a, b) => a.Order.CompareTo(b.Order));
        if (_targets.Count == 0) { _index = -1; return; }
        if (_index < 0 || _index >= _targets.Count) _index = 0;
    }

    public void Update(InputState input)
    {
        if (_targets.Count == 0) return;

        if (input.FocusNext || input.NavRight || input.NavDown)
            Move(+1);
        else if (input.FocusPrev || input.NavLeft || input.NavUp)
            Move(-1);

        // Pointer hover/press adopts focus on mouse (not required for touch)
        if (input.ShowHover || input.LastDevice == InputDeviceKind.Mouse)
        {
            for (int i = 0; i < _targets.Count; i++)
            {
                if (_targets[i].Bounds.Contains(input.PointerPosition))
                {
                    _index = i;
                    break;
                }
            }
        }
    }

    public bool IsFocused(string id) => CurrentId == id;

    public bool Activated(InputState input, string id)
    {
        if (CurrentId != id) return false;
        return input.ConfirmPressed;
    }

    /// <summary>Unified activation: pointer release on bounds OR confirm while focused.</summary>
    public bool Activated(InputState input, string id, Rectangle bounds)
    {
        if (input.IsPointerReleased && bounds.Contains(input.PointerPosition))
            return true;
        return Activated(input, id);
    }

    public void DrawFocus(VoxelionGame game, SpriteBatch sb, InputState input, float time = 0f)
    {
        if (!input.ShowFocusRing || Current == null) return;
        var r = Current.Bounds;
        float pulse = 0.65f + 0.35f * MathF.Sin(time * 6f);
        var c = DesignTokens.Semantic.Focus * pulse;
        // Outer unmistakable ring
        game.DrawBorder(sb, new Rectangle(r.X - 3, r.Y - 3, r.Width + 6, r.Height + 6), c, 3);
        game.DrawBorder(sb, r, DesignTokens.Semantic.Secondary * pulse, 1);
        // Corner ticks
        int t = 8;
        XnaColor tick = DesignTokens.Semantic.Secondary;
        game.DrawRect(sb, r.X - 2, r.Y - 2, t, 2, tick);
        game.DrawRect(sb, r.X - 2, r.Y - 2, 2, t, tick);
        game.DrawRect(sb, r.Right - t + 2, r.Y - 2, t, 2, tick);
        game.DrawRect(sb, r.Right, r.Y - 2, 2, t, tick);
        game.DrawRect(sb, r.X - 2, r.Bottom, t, 2, tick);
        game.DrawRect(sb, r.X - 2, r.Bottom - t + 2, 2, t, tick);
        game.DrawRect(sb, r.Right - t + 2, r.Bottom, t, 2, tick);
        game.DrawRect(sb, r.Right, r.Bottom - t + 2, 2, t, tick);
    }

    public void DrawShortcut(VoxelionGame game, SpriteBatch sb, InputState input, Rectangle near, string label)
    {
        if (!input.ShowShortcutHints || string.IsNullOrEmpty(label)) return;
        var size = game.MeasureText(label, 1.1f);
        float x = near.Right + 8;
        float y = near.Y + (near.Height - size.Y) * 0.5f;
        game.DrawRect(sb, x - 4, y - 2, size.X + 8, size.Y + 6, DesignTokens.Semantic.SurfaceElevated);
        game.DrawBorder(sb, new Rectangle((int)(x - 4), (int)(y - 2), (int)(size.X + 8), (int)(size.Y + 6)),
            DesignTokens.Semantic.Border, 1);
        game.DrawText(sb, label, new Vector2(x, y), DesignTokens.Semantic.TextMuted, 1.1f);
    }

    private void Move(int delta)
    {
        if (_targets.Count == 0) return;
        if (_index < 0) _index = 0;
        else _index = (_index + delta + _targets.Count * 8) % _targets.Count;
    }
}

public sealed class FocusTarget
{
    public string Id { get; }
    public Rectangle Bounds { get; }
    public int Order { get; }

    public FocusTarget(string id, Rectangle bounds, int order)
    {
        Id = id;
        Bounds = bounds;
        Order = order;
    }
}

/// <summary>Helpers for device-adaptive metrics (target size, hover fill).</summary>
public static class InputAdaptive
{
    public static float TargetHeight(InputState input, float baseHeight = 52f) =>
        input.PreferLargeTargets
            ? Math.Max(DesignTokens.Layout.MinTouchTarget, baseHeight)
            : Math.Max(40f, baseHeight * 0.9f);

    public static bool Hover(InputState input, Rectangle r) =>
        input.ShowHover && r.Contains(input.PointerPosition);

    public static bool Pressed(InputState input, Rectangle r) =>
        r.Contains(input.PointerPosition) && input.IsPointerDown;
}
