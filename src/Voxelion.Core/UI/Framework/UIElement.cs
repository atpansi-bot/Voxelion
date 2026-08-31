using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.DesignSystem;
using Voxelion.Core.Input;

namespace Voxelion.Core.UI.Framework
{
    /// <summary>
    /// Base of every interactive visual element in VOXELION.
    /// Composition over inheritance. Deterministic lifecycle.
    /// Resolution-independent via anchors and relative layout.
    /// </summary>
    public abstract class UIElement
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
        public UIElement? Parent { get; set; }
        public List<UIElement> Children { get; } = new List<UIElement>();

        public Rectangle Bounds { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Anchor { get; set; } = new Vector2(0.5f, 0.5f); // 0..1
        public Vector2 Pivot { get; set; } = new Vector2(0.5f, 0.5f);
        public float Opacity { get; set; } = 1f;
        public float Scale { get; set; } = 1f;
        public bool Visible { get; set; } = true;
        public bool Enabled { get; set; } = true;
        public bool Focusable { get; set; } = true;
        public bool IsFocused { get; set; }
        public bool IsHovered { get; set; }
        public bool IsPressed { get; set; }
        public int ZIndex { get; set; }

        public event Action? OnClick;
        public event Action? OnFocus;
        public event Action? OnBlur;
        public event Action? OnHoverEnter;
        public event Action? OnHoverExit;

        protected float AnimProgress;
        protected float TargetOpacity = 1f;
        protected float CurrentOpacity = 1f;

        public virtual void Open()
        {
            Visible = true;
            TargetOpacity = 1f;
            AnimProgress = 0f;
        }

        public virtual void Close()
        {
            TargetOpacity = 0f;
        }

        public virtual void Focus()
        {
            if (!Focusable || !Enabled) return;
            IsFocused = true;
            OnFocus?.Invoke();
        }

        public virtual void Blur()
        {
            IsFocused = false;
            OnBlur?.Invoke();
        }

        public virtual void Update(GameTime gameTime, InputState input)
        {
            if (!Visible) return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            CurrentOpacity = MotionSystem.Lerp(CurrentOpacity, TargetOpacity, 1f - MathF.Exp(-12f * dt));
            if (CurrentOpacity < 0.01f && TargetOpacity <= 0f)
                Visible = false;

            Opacity = CurrentOpacity;

            // Input handling is delegated to derived or InputRouter
            foreach (var child in Children)
                child.Update(gameTime, input);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Rectangle viewport)
        {
            if (!Visible || Opacity <= 0.01f) return;
            foreach (var child in Children)
                child.Draw(spriteBatch, viewport);
        }

        public virtual bool HandleInput(InputState input)
        {
            if (!Visible || !Enabled) return false;
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                if (Children[i].HandleInput(input))
                    return true;
            }
            return false;
        }

        public void AddChild(UIElement child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public void RemoveChild(UIElement child)
        {
            Children.Remove(child);
            child.Parent = null;
        }

        public Rectangle GetAbsoluteBounds(Rectangle parentBounds)
        {
            float x = parentBounds.X + Position.X + (parentBounds.Width - Size.X) * Anchor.X;
            float y = parentBounds.Y + Position.Y + (parentBounds.Height - Size.Y) * Anchor.Y;
            return new Rectangle((int)x, (int)y, (int)(Size.X * Scale), (int)(Size.Y * Scale));
        }

        protected void InvokeClick()
        {
            if (Enabled)
                OnClick?.Invoke();
        }
    }
}
