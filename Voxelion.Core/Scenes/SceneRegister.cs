using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneRegister : SceneBase
{
    private int _step;
    private int _hovered = -1;

    public SceneRegister(VoxelionGame game) : base(game) { }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;

        // Next / Back buttons
        var nextR = new Rectangle((int)(w * 0.55f), (int)(h * 0.82f), 160, 44);
        var backR = new Rectangle((int)(w * 0.35f), (int)(h * 0.82f), 160, 44);
        _hovered = -1;
        if (nextR.Contains(input.PointerPosition)) _hovered = 0;
        if (backR.Contains(input.PointerPosition)) _hovered = 1;

        if (input.IsPointerPressed)
        {
            if (_hovered == 0)
            {
                _step++;
                if (_step >= 4)
                {
                    Game.Session.CreateAccountSession(Game.Profile, "player@voxelion.local");
                    Game.TransitionTo(ApplicationState.CharacterCreation);
                }
            }
            else if (_hovered == 1)
            {
                if (_step > 0) _step--;
                else Game.GoBack();
            }
        }
        if (input.CancelPressed)
        {
            if (_step > 0) _step--;
            else Game.GoBack();
        }
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height, cx = w * 0.5f;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.DeepNight);

        // Progress steps
        string[] steps = { Game.Loc["register.step1"], Game.Loc["register.step2"], Game.Loc["register.step3"], Game.Loc["register.step4"] };
        float stepW = 100f, startX = cx - (steps.Length * stepW) * 0.5f;
        for (int i = 0; i < steps.Length; i++)
        {
            Color c = i <= _step ? DesignTokens.Color.AccentPrimary : DesignTokens.Color.TextMuted;
            Game.DrawRect(sb, startX + i * stepW + 20, 40, 12, 12, c);
            Game.DrawText(sb, steps[i], new Vector2(startX + i * stepW, 60), c, 0.7f);
            if (i < steps.Length - 1)
                Game.DrawRect(sb, startX + i * stepW + 50, 45, 40, 2, DesignTokens.Color.BorderSubtle);
        }

        // Content panel
        var panel = new Rectangle((int)(w * 0.25f), (int)(h * 0.25f), (int)(w * 0.5f), (int)(h * 0.45f));
        Game.DrawRect(sb, panel, DesignTokens.Color.PanelBase);
        Game.DrawBorder(sb, panel, DesignTokens.Color.BorderSubtle, 1);

        string msg = _step switch
        {
            0 => "Enter email & password",
            1 => "Choose display identity",
            2 => "Select starting avatar style",
            _ => "Account ready"
        };
        var ms = Game.MeasureText(msg, 1.1f);
        Game.DrawText(sb, msg, new Vector2(cx - ms.X * 0.5f, h * 0.4f), DesignTokens.Color.TextPrimary, 1.1f);

        // Buttons
        var nextR = new Rectangle((int)(w * 0.55f), (int)(h * 0.82f), 160, 44);
        var backR = new Rectangle((int)(w * 0.35f), (int)(h * 0.82f), 160, 44);
        DrawBtn(sb, nextR, Game.Loc["common.next"], _hovered == 0);
        DrawBtn(sb, backR, Game.Loc["common.back"], _hovered == 1);
    }

    private void DrawBtn(SpriteBatch sb, Rectangle r, string label, bool hover)
    {
        Game.DrawRect(sb, r, hover ? DesignTokens.Color.PanelElevated : DesignTokens.Color.PanelBase);
        Game.DrawBorder(sb, r, hover ? DesignTokens.Color.BorderFocus : DesignTokens.Color.BorderSubtle, 1);
        var ls = Game.MeasureText(label, 0.95f);
        Game.DrawText(sb, label, new Vector2(r.X + (r.Width - ls.X) * 0.5f, r.Y + (r.Height - ls.Y) * 0.5f),
            DesignTokens.Color.TextPrimary, 0.95f);
    }
}
