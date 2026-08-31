using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class ScenePause : SceneBase
{
    private readonly string[] _items = { "RESUME", "INVENTORY", "SOCIAL", "WORLDS", "SETTINGS", "EXIT WORLD" };
    private Rectangle[] _btns = Array.Empty<Rectangle>();

    public ScenePause(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        Layout();
    }

    private void Layout()
    {
        var vp = Game.GraphicsDevice.Viewport;
        float bw = Math.Min(280f, vp.Width * 0.45f);
        float bh = 48f;
        float gap = 10f;
        float total = _items.Length * bh + (_items.Length - 1) * gap;
        float y = vp.Height * 0.5f - total * 0.5f;
        float x = vp.Width * 0.5f - bw * 0.5f;
        _btns = new Rectangle[_items.Length];
        for (int i = 0; i < _items.Length; i++)
            _btns[i] = new Rectangle((int)x, (int)(y + i * (bh + gap)), (int)bw, (int)bh);
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Layout();
        if (!input.IsPointerReleased) return;
        for (int i = 0; i < _btns.Length; i++)
        {
            if (!_btns[i].Contains(input.PointerPosition)) continue;
            switch (i)
            {
                case 0: Game.TransitionTo(ApplicationState.World); break;
                case 1: Game.TransitionTo(ApplicationState.Inventory); break;
                case 2: Game.TransitionTo(ApplicationState.Social); break;
                case 3: Game.TransitionTo(ApplicationState.WorldDiscovery); break;
                case 4: Game.TransitionTo(ApplicationState.Settings); break;
                case 5:
                    Game.Toasts.Push("RETURNED TO HUB", ToastKind.Info);
                    Game.TransitionTo(ApplicationState.Hub);
                    break;
            }
            return;
        }
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        // Dimmed world feel
        Game.DrawRect(sb, 0, 0, vp.Width, vp.Height, DesignTokens.Color.DeepNight);
        UiKit.Dim(Game, sb, vp, 0.4f);
        UiKit.CenterLabel(Game, sb, "PAUSED", vp.Height * 0.18f, DesignTokens.Color.TextPrimary, 2.5f, vp.Width);
        UiKit.CenterLabel(Game, sb, "STILL CONNECTED TO WORLD", vp.Height * 0.24f, DesignTokens.Color.TextMuted, 1.3f, vp.Width);
        for (int i = 0; i < _items.Length; i++)
        {
            var fill = i == 5 ? DesignTokens.Color.AccentDanger : i == 0 ? DesignTokens.Color.AccentPrimary : DesignTokens.Color.PanelElevated;
            Game.DrawRect(sb, _btns[i], fill);
            Game.DrawBorder(sb, _btns[i], DesignTokens.Color.BorderFocus, 2);
            var size = Game.MeasureText(_items[i], 1.7f);
            Game.DrawText(sb, _items[i],
                new Vector2(_btns[i].X + (_btns[i].Width - size.X) * 0.5f, _btns[i].Y + 14),
                DesignTokens.Color.TextPrimary, 1.7f);
        }
    }
}
