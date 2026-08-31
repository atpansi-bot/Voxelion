using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneSocial : SceneBase
{
    private Rectangle _btnBack;
    private readonly string[] _tabs = { "FRIENDS", "PARTY", "NEARBY", "INVITES", "MESSAGES", "BLOCKED" };
    private int _tab;
    private readonly (string Name, string Presence)[] _friends =
    {
        ("LUNA", "ONLINE"), ("KAEL", "IN WORLD"), ("MIRA", "IN HUB"),
        ("REX", "AWAY"), ("AYA", "OFFLINE")
    };

    public SceneSocial(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        _tab = 0;
        Layout();
    }

    private void Layout()
    {
        var vp = Game.GraphicsDevice.Viewport;
        _btnBack = new Rectangle((int)SafeLayout.Margin(vp), (int)SafeLayout.Margin(vp), 100, 40);
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Layout();
        if (!input.IsPointerReleased) return;
        var p = input.PointerPosition;
        if (_btnBack.Contains(p))
        {
            if (Game.StateMachine.CanGoBack) Game.GoBack();
            else Game.TransitionTo(ApplicationState.Hub);
            return;
        }
        var vp = Game.GraphicsDevice.Viewport;
        float m = SafeLayout.Margin(vp);
        for (int i = 0; i < _tabs.Length; i++)
        {
            var r = new Rectangle((int)(m + i * 110), (int)(m + 52), 104, 36);
            if (r.Contains(p)) { _tab = i; return; }
        }
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float m = SafeLayout.Margin(vp);
        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);
        Game.DrawRect(sb, 0, 0, w, 56, DesignTokens.Color.PanelBase);
        DrawBtn(sb, _btnBack, "BACK", DesignTokens.Color.PanelElevated);
        UiKit.CenterLabel(Game, sb, "SOCIAL", 16, DesignTokens.Color.TextPrimary, 2.2f, w);

        for (int i = 0; i < _tabs.Length; i++)
        {
            var r = new Rectangle((int)(m + i * 110), (int)(m + 52), 104, 36);
            DrawBtn(sb, r, _tabs[i], i == _tab ? DesignTokens.Color.AccentPrimary : DesignTokens.Color.PanelElevated);
        }

        if (_tab == 0)
        {
            float y = m + 110;
            foreach (var f in _friends)
            {
                var row = new Rectangle((int)m, (int)y, (int)(w - m * 2), 44);
                UiKit.Panel(Game, sb, row, DesignTokens.Color.PanelElevated, DesignTokens.Color.BorderSubtle, 1);
                Game.DrawIcon(sb, "user", new Rectangle(row.X + 10, row.Y + 8, 28, 28), DesignTokens.Color.AccentSecondary);
                Game.DrawText(sb, f.Name, new Vector2(row.X + 50, row.Y + 12), DesignTokens.Color.TextPrimary, 1.5f);
                Color pc = f.Presence switch
                {
                    "ONLINE" => DesignTokens.Color.AccentSuccess,
                    "IN WORLD" => DesignTokens.Color.AccentSecondary,
                    "IN HUB" => DesignTokens.Color.AccentPrimary,
                    "AWAY" => DesignTokens.Color.AccentWarning,
                    _ => DesignTokens.Color.TextMuted
                };
                var ps = Game.MeasureText(f.Presence, 1.3f);
                Game.DrawText(sb, f.Presence, new Vector2(row.Right - ps.X - 16, row.Y + 14), pc, 1.3f);
                y += 52;
            }
        }
        else
            UiKit.CenterLabel(Game, sb, _tabs[_tab] + " LIST IS EMPTY", h * 0.5f, DesignTokens.Color.TextMuted, 1.6f, w);
    }

    private void DrawBtn(SpriteBatch sb, Rectangle r, string label, Color fill)
    {
        Game.DrawRect(sb, r, fill);
        Game.DrawBorder(sb, r, DesignTokens.Color.BorderFocus, 1);
        float scale = label.Length > 8 ? 1.1f : 1.3f;
        var size = Game.MeasureText(label, scale);
        Game.DrawText(sb, label, new Vector2(r.X + (r.Width - size.X) * 0.5f, r.Y + (r.Height - size.Y) * 0.5f),
            DesignTokens.Color.TextPrimary, scale);
    }
}
