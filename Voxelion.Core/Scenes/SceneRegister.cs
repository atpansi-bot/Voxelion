using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

/// <summary>Short registration: Account → Identity → Avatar → Ready.</summary>
public sealed class SceneRegister : SceneBase
{
    private int _step; // 0..3
    private Rectangle _btnNext, _btnBack;
    private readonly string[] _steps = { "ACCOUNT", "IDENTITY", "AVATAR", "READY" };

    public SceneRegister(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        _step = 0;
        Layout();
    }

    private void Layout()
    {
        var vp = Game.GraphicsDevice.Viewport;
        float cx = vp.Width * 0.5f;
        float bw = Math.Min(160f, vp.Width * 0.28f);
        float bh = 48f;
        float y = vp.Height * 0.82f;
        _btnBack = new Rectangle((int)(cx - bw - 12), (int)y, (int)bw, (int)bh);
        _btnNext = new Rectangle((int)(cx + 12), (int)y, (int)bw, (int)bh);
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Layout();
        if (!input.IsPointerReleased) return;
        if (_btnBack.Contains(input.PointerPosition))
        {
            if (_step == 0) Game.TransitionTo(ApplicationState.Authentication);
            else _step--;
            return;
        }
        if (_btnNext.Contains(input.PointerPosition))
        {
            if (_step < 3) _step++;
            else
            {
                Game.Session.CreateAccountSession(Game.Profile, "player@voxelion.local");
                Game.Toasts.Push("ACCOUNT READY", ToastKind.Success);
                Game.TransitionTo(ApplicationState.CharacterCreation);
            }
        }
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height, cx = w * 0.5f;
        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.DeepNight);
        UiKit.CenterLabel(Game, sb, "CREATE ACCOUNT", h * 0.12f, DesignTokens.Color.TextPrimary, 2.4f, w);

        // Step indicator
        float total = _steps.Length * 70f;
        float sx = cx - total * 0.5f;
        for (int i = 0; i < _steps.Length; i++)
        {
            Color c = i <= _step ? DesignTokens.Color.AccentPrimary : DesignTokens.Color.ShadowIndigo;
            Game.DrawRect(sb, sx + i * 70, h * 0.22f, 50, 8, c);
            Game.DrawText(sb, (i + 1).ToString(), new Vector2(sx + i * 70 + 18, h * 0.26f), DesignTokens.Color.TextMuted, 1.2f);
        }
        UiKit.CenterLabel(Game, sb, _steps[_step], h * 0.34f, DesignTokens.Color.AccentSecondary, 2f, w);

        string body = _step switch
        {
            0 => "EMAIL AND PASSWORD ARE STORED LOCALLY IN THIS PROTOTYPE",
            1 => "CHOOSE HOW YOU APPEAR TO OTHERS",
            2 => "AVATAR WILL BE SET IN CHARACTER CREATION",
            _ => "YOU CAN ENTER VOXELION AFTER THIS STEP"
        };
        UiKit.CenterLabel(Game, sb, body, h * 0.48f, DesignTokens.Color.TextSecondary, 1.3f, w);

        DrawBtn(sb, _btnBack, "BACK", DesignTokens.Color.PanelBase);
        DrawBtn(sb, _btnNext, _step == 3 ? "FINISH" : "NEXT", DesignTokens.Color.AccentPrimary);
    }

    private void DrawBtn(SpriteBatch sb, Rectangle r, string label, Color fill)
    {
        Game.DrawRect(sb, r, fill);
        Game.DrawBorder(sb, r, DesignTokens.Color.BorderFocus, 2);
        var size = Game.MeasureText(label, 1.7f);
        Game.DrawText(sb, label, new Vector2(r.X + (r.Width - size.X) * 0.5f, r.Y + (r.Height - size.Y) * 0.5f),
            DesignTokens.Color.TextPrimary, 1.7f);
    }
}
