using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace Voxelion.Core.UI.Components;

[Flags]
public enum UIState
{
    Normal = 0,
    Hover = 1,
    Pressed = 2,
    Focused = 4,
    Selected = 8,
    Disabled = 16,
    Loading = 32,
    Success = 64,
    Error = 128
}

/// <summary>Base interactive element — bounds, state flags, hit test.</summary>
public abstract class UIElement
{
    public string Id { get; set; } = "";
    public Rectangle Bounds { get; set; }
    public UIState State { get; set; } = UIState.Normal;
    public bool Visible { get; set; } = true;
    public bool Enabled
    {
        get => (State & UIState.Disabled) == 0;
        set
        {
            if (value) State &= ~UIState.Disabled;
            else State |= UIState.Disabled;
        }
    }

    public bool Is(UIState flag) => (State & flag) != 0;

    public virtual void Update(InputState input, bool focused)
    {
        if (!Visible || !Enabled)
        {
            State &= ~(UIState.Hover | UIState.Pressed | UIState.Focused);
            if (!Enabled) State |= UIState.Disabled;
            return;
        }

        if (focused) State |= UIState.Focused;
        else State &= ~UIState.Focused;

        bool hit = Bounds.Contains(input.PointerPosition);
        if (input.ShowHover && hit) State |= UIState.Hover;
        else State &= ~UIState.Hover;

        if (hit && input.IsPointerDown) State |= UIState.Pressed;
        else State &= ~UIState.Pressed;
    }

    public bool WasActivated(InputState input, bool focused)
    {
        if (!Visible || !Enabled) return false;
        if (input.IsPointerReleased && Bounds.Contains(input.PointerPosition)) return true;
        if (focused && input.ConfirmPressed) return true;
        return false;
    }

    public abstract void Draw(VoxelionGame game, SpriteBatch sb, float time);

    protected static XnaColor StateTint(UIState s, XnaColor baseColor)
    {
        if ((s & UIState.Disabled) != 0) return DesignTokens.Semantic.Disabled;
        if ((s & UIState.Pressed) != 0) return baseColor * 0.72f;
        if ((s & UIState.Hover) != 0) return XnaColor.Lerp(baseColor, DesignTokens.Semantic.Primary, 0.15f);
        return baseColor;
    }
}
