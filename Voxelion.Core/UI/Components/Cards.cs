using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.UI.Theme;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace Voxelion.Core.UI.Components;

public sealed class AvatarCard : UIElement
{
    public string Name { get; set; } = "";
    public XnaColor Primary { get; set; } = DesignTokens.Semantic.Primary;
    public XnaColor Secondary { get; set; } = DesignTokens.Semantic.Secondary;

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        VisualChrome.Panel(game, sb, Bounds, elevated: Is(UIState.Selected));
        VisualChrome.Avatar(game, sb, new Vector2(Bounds.X + 40, Bounds.Center.Y), 36, Primary, Secondary);
        game.DrawText(sb, Name, new Vector2(Bounds.X + 80, Bounds.Center.Y - 8),
            DesignTokens.Semantic.TextPrimary, DesignTokens.Typography.Body);
    }
}

public sealed class PlayerCard : UIElement
{
    public string Name { get; set; } = "";
    public string Status { get; set; } = "ONLINE";
    public XnaColor Accent { get; set; } = DesignTokens.Semantic.Success;

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        VisualChrome.Panel(game, sb, Bounds, elevated: Is(UIState.Selected) || Is(UIState.Focused));
        VisualChrome.Avatar(game, sb, new Vector2(Bounds.X + 36, Bounds.Center.Y), 28,
            DesignTokens.Semantic.Primary, DesignTokens.Semantic.Secondary);
        game.DrawText(sb, Name, new Vector2(Bounds.X + 70, Bounds.Y + 14),
            DesignTokens.Semantic.TextPrimary, DesignTokens.Typography.Body);
        VisualChrome.Badge(game, sb, new Vector2(Bounds.X + 70, Bounds.Y + 40), Status, Accent);
    }
}

public sealed class WorldCard : UIElement
{
    public string Name { get; set; } = "";
    public string Meta { get; set; } = "";
    public XnaColor Accent { get; set; } = DesignTokens.Semantic.Primary;
    public bool Favorite { get; set; }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        VisualChrome.WorldCard(game, sb, Bounds, Name, Meta, Accent,
            Is(UIState.Selected) || Is(UIState.Focused), Favorite);
    }
}

public sealed class ItemCard : UIElement
{
    public string Name { get; set; } = "";
    public string Rarity { get; set; } = "common";
    public int Quantity { get; set; } = 1;

    private static XnaColor RarityColor(string r) => r.ToLowerInvariant() switch
    {
        "uncommon" => DesignTokens.Semantic.RarityUncommon,
        "rare" => DesignTokens.Semantic.RarityRare,
        "epic" => DesignTokens.Semantic.RarityEpic,
        "legendary" => DesignTokens.Semantic.RarityLegendary,
        _ => DesignTokens.Semantic.RarityCommon
    };

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        var rc = RarityColor(Rarity);
        VisualChrome.Panel(game, sb, Bounds, elevated: true);
        game.DrawRect(sb, Bounds.X + 8, Bounds.Y + 8, Bounds.Width - 16, Bounds.Height - 40, rc * 0.45f);
        game.DrawRect(sb, Bounds.X, Bounds.Bottom - 4, Bounds.Width, 4, rc);
        game.DrawText(sb, Name, new Vector2(Bounds.X + 10, Bounds.Bottom - 28),
            DesignTokens.Semantic.TextPrimary, DesignTokens.Typography.Caption);
        if (Quantity > 1)
            game.DrawText(sb, "x" + Quantity, new Vector2(Bounds.Right - 36, Bounds.Y + 10),
                DesignTokens.Semantic.TextSecondary, DesignTokens.Typography.Caption);
    }
}

public sealed class InventorySlot : UIElement
{
    public bool Empty { get; set; } = true;
    public XnaColor? Rarity { get; set; }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        VisualChrome.InventorySlot(game, sb, Bounds, Empty,
            Is(UIState.Selected) || Is(UIState.Focused), Rarity);
    }
}

/// <summary>Image placeholder — solid block until Content textures load.</summary>
public sealed class Image : UIElement
{
    public XnaColor Tint { get; set; } = DesignTokens.Semantic.SurfaceElevated;

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        game.DrawRect(sb, Bounds, Tint);
        game.DrawBorder(sb, Bounds, DesignTokens.Semantic.Border, 1);
    }
}

/// <summary>Toast is driven by NotificationBus; this is a drawable row.</summary>
public sealed class ToastView : UIElement
{
    public string Message { get; set; } = "";
    public XnaColor Accent { get; set; } = DesignTokens.Semantic.Info;

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        game.DrawRect(sb, Bounds, DesignTokens.Semantic.SurfaceElevated);
        game.DrawRect(sb, Bounds.X, Bounds.Y, 4, Bounds.Height, Accent);
        game.DrawBorder(sb, Bounds, DesignTokens.Semantic.Border, 1);
        game.DrawText(sb, Message, new Vector2(Bounds.X + 14, Bounds.Y + 12),
            DesignTokens.Semantic.TextPrimary, DesignTokens.Typography.Body);
    }
}

/// <summary>Notification list row.</summary>
public sealed class NotificationView : UIElement
{
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public XnaColor Accent { get; set; } = DesignTokens.Semantic.Info;

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        VisualChrome.Panel(game, sb, Bounds, elevated: true);
        game.DrawRect(sb, Bounds.X, Bounds.Y, 4, Bounds.Height, Accent);
        game.DrawText(sb, Title, new Vector2(Bounds.X + 16, Bounds.Y + 10),
            DesignTokens.Semantic.TextPrimary, DesignTokens.Typography.Body);
        game.DrawText(sb, Body, new Vector2(Bounds.X + 16, Bounds.Y + 34),
            DesignTokens.Semantic.TextMuted, DesignTokens.Typography.Caption);
    }
}
