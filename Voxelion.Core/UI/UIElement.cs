using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Input;

namespace Voxelion.Core.UI;

public enum UIElementState
{
    Normal,
    Hovered,
    Pressed,
    Focused,
    Disabled,
    Hidden
}

/// <summary>
/// Base class for all VOXELION UI components. Composition root.
/// Zero-allocation hot path where possible. Deterministic lifecycle.
/// </summary>
public abstract class UIElement
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public Rectangle Bounds { get; set; }
    public Vector2 Position
    {
        get => new(Bounds.X, Bounds.Y);
        set => Bounds = new Rectangle((int)value.X, (int)value.Y, Bounds.Width, Bounds.Height);
    }
    public Vector2 Size
    {
        get => new(Bounds.Width, Bounds.Height);
        set => Bounds = new Rectangle(Bounds.X, Bounds.Y, (int)value.X, (int)value.Y);
    }

    public UIElementState State { get; protected set; } = UIElementState.Normal;
    public bool IsVisible { get; set; } = true;
    public bool IsEnabled { get; set; } = true;
    public bool IsFocusable { get; set; } = true;
    public float Opacity { get; set; } = 1f;
    public float Scale { get; set; } = 1f;
    public int ZIndex { get; set; }

    public UIElement? Parent { get; set; }
    public List<UIElement> Children { get; } = new();

    public event Action? OnClick;
    public event Action? OnFocus;
    public event Action? OnBlur;
    public event Action? OnHoverEnter;
    public event Action? OnHoverExit;

    protected float AnimProgress { get; set; }
    protected float AnimTarget { get; set; } = 1f;

    public virtual void AddChild(UIElement child)
    {
        child.Parent = this;
        Children.Add(child);
    }

    public virtual void RemoveChild(UIElement child)
    {
        if (Children.Remove(child))
            child.Parent = null;
    }

    public virtual void ClearChildren()
    {
        foreach (var c in Children)
            c.Parent = null;
        Children.Clear();
    }

    public virtual void Update(GameTime gameTime, InputState input)
    {
        if (!IsVisible || !IsEnabled)
        {
            State = UIElementState.Disabled;
            return;
        }

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        AnimProgress = MathHelper.Lerp(AnimProgress, AnimTarget, 1f - MathF.Exp(-12f * dt));

        bool contains = Bounds.Contains(input.PointerPosition);
        bool wasHovered = State == UIElementState.Hovered || State == UIElementState.Pressed;

        if (contains && input.IsPointerDown)
            State = UIElementState.Pressed;
        else if (contains)
        {
            if (!wasHovered)
                OnHoverEnter?.Invoke();
            State = UIElementState.Hovered;
        }
        else
        {
            if (wasHovered)
                OnHoverExit?.Invoke();
            State = UIElementState.Normal;
        }

        if (State == UIElementState.Pressed && input.IsPointerReleased && contains)
            OnClick?.Invoke();

        foreach (var child in Children)
            child.Update(gameTime, input);
    }

    public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (!IsVisible || Opacity <= 0.001f) return;

        foreach (var child in Children.OrderBy(c => c.ZIndex))
            child.Draw(spriteBatch, gameTime);
    }

    public virtual void Focus()
    {
        if (!IsFocusable || !IsEnabled) return;
        State = UIElementState.Focused;
        OnFocus?.Invoke();
    }

    public virtual void Blur()
    {
        if (State == UIElementState.Focused)
        {
            State = UIElementState.Normal;
            OnBlur?.Invoke();
        }
    }

    public virtual void Open() => AnimTarget = 1f;
    public virtual void Close() => AnimTarget = 0f;

    public virtual Rectangle GetAbsoluteBounds()
    {
        if (Parent == null) return Bounds;
        var p = Parent.GetAbsoluteBounds();
        return new Rectangle(p.X + Bounds.X, p.Y + Bounds.Y, Bounds.Width, Bounds.Height);
    }

    public bool HitTest(Point point) => IsVisible && IsEnabled && Bounds.Contains(point);
}
