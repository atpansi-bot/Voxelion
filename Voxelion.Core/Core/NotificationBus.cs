using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Core;

public enum ToastKind { Info, Success, Warning, Error }

public sealed class ToastItem
{
    public string Message { get; init; } = "";
    public ToastKind Kind { get; init; }
    public float Life { get; set; } = 2.4f;
}

/// <summary>Simple toast queue — micro feedback without modal spam.</summary>
public sealed class NotificationBus
{
    private readonly Queue<ToastItem> _pending = new();
    private ToastItem? _active;

    public void Push(string message, ToastKind kind = ToastKind.Info)
    {
        if (string.IsNullOrWhiteSpace(message)) return;
        _pending.Enqueue(new ToastItem { Message = message.ToUpperInvariant(), Kind = kind });
    }

    public void Update(float dt)
    {
        if (_active == null)
        {
            if (_pending.Count > 0) _active = _pending.Dequeue();
            return;
        }
        _active.Life -= dt;
        if (_active.Life <= 0) _active = null;
    }

    public void Draw(VoxelionGame game, SpriteBatch sb, Viewport vp)
    {
        if (_active == null) return;
        Color accent = _active.Kind switch
        {
            ToastKind.Success => DesignTokens.Color.AccentSuccess,
            ToastKind.Warning => DesignTokens.Color.AccentWarning,
            ToastKind.Error => DesignTokens.Color.AccentDanger,
            _ => DesignTokens.Color.AccentSecondary
        };
        float alpha = MathHelper.Clamp(_active.Life, 0, 1);
        var size = game.MeasureText(_active.Message, 1.4f);
        float tw = size.X + 32;
        float th = size.Y + 20;
        float x = vp.Width * 0.5f - tw * 0.5f;
        float y = 56;
        game.DrawRect(sb, x, y, tw, th, DesignTokens.Color.PanelElevated * alpha);
        game.DrawRect(sb, x, y, 4, th, accent * alpha);
        game.DrawText(sb, _active.Message, new Vector2(x + 16, y + 10), DesignTokens.Color.TextPrimary * alpha, 1.4f);
    }
}
