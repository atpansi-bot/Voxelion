using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneIdentity : SceneBase
{
    private Rectangle _btnConfirm, _btnBack, _btnRegen;
    private string _name = "WANDERER";
    private string _validation = "NAME AVAILABLE";
    private Color _valColor = DesignTokens.Color.AccentSuccess;

    public SceneIdentity(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        _name = string.IsNullOrEmpty(Game.Profile.DisplayName)
            ? "WANDERER" + Random.Shared.Next(10, 99)
            : Game.Profile.DisplayName.ToUpperInvariant();
        Validate();
        Layout();
    }

    private void Layout()
    {
        var vp = Game.GraphicsDevice.Viewport;
        float cx = vp.Width * 0.5f;
        float bw = Math.Min(200f, vp.Width * 0.32f);
        float bh = 48f;
        float y = vp.Height * 0.72f;
        _btnBack = new Rectangle((int)(cx - bw - 8), (int)y, (int)bw, (int)bh);
        _btnConfirm = new Rectangle((int)(cx + 8), (int)y, (int)bw, (int)bh);
        _btnRegen = new Rectangle((int)(cx - 80), (int)(vp.Height * 0.58f), 160, 40);
    }

    private void Validate()
    {
        if (_name.Length < 3) { _validation = "NAME TOO SHORT"; _valColor = DesignTokens.Color.AccentDanger; return; }
        if (_name.Length > 16) { _validation = "NAME TOO LONG"; _valColor = DesignTokens.Color.AccentDanger; return; }
        foreach (char c in _name)
        {
            if (!(char.IsLetterOrDigit(c) || c == '_' || c == '-'))
            { _validation = "INVALID CHARACTERS"; _valColor = DesignTokens.Color.AccentDanger; return; }
        }
        if (_name is "ADMIN" or "MOD" or "SYSTEM")
        { _validation = "NAME RESTRICTED"; _valColor = DesignTokens.Color.AccentWarning; return; }
        _validation = "NAME AVAILABLE";
        _valColor = DesignTokens.Color.AccentSuccess;
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Layout();
        if (!input.IsPointerReleased) return;
        var p = input.PointerPosition;
        if (_btnRegen.Contains(p))
        {
            _name = "WANDERER" + Random.Shared.Next(10, 99);
            Validate();
            return;
        }
        if (_btnBack.Contains(p))
        {
            Game.TransitionTo(ApplicationState.CharacterCreation);
            return;
        }
        if (_btnConfirm.Contains(p))
        {
            Validate();
            if (_valColor == DesignTokens.Color.AccentDanger) return;
            Game.Profile.DisplayName = _name;
            Game.Profile.HasCharacter = true;
            Game.Toasts.Push("IDENTITY CONFIRMED", ToastKind.Success);
            Game.TransitionTo(ApplicationState.Welcome);
        }
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height, cx = w * 0.5f;
        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.DeepNight);
        UiKit.CenterLabel(Game, sb, "CHOOSE YOUR NAME", h * 0.16f, DesignTokens.Color.TextPrimary, 2.4f, w);

        float nw = Math.Min(360f, w * 0.6f);
        var panel = new Rectangle((int)(cx - nw * 0.5f), (int)(h * 0.40f), (int)nw, 56);
        UiKit.Panel(Game, sb, panel, DesignTokens.Color.PanelElevated, DesignTokens.Color.AccentPrimary, 2);
        var ns = Game.MeasureText(_name, 2.4f);
        Game.DrawText(sb, _name, new Vector2(panel.X + (panel.Width - ns.X) * 0.5f, panel.Y + 16), DesignTokens.Color.TextPrimary, 2.4f);

        UiKit.CenterLabel(Game, sb, _validation, h * 0.52f, _valColor, 1.4f, w);
        DrawBtn(sb, _btnRegen, "RANDOMIZE", DesignTokens.Color.PanelElevated);
        DrawBtn(sb, _btnBack, "BACK", DesignTokens.Color.PanelBase);
        DrawBtn(sb, _btnConfirm, "CONFIRM", DesignTokens.Color.AccentPrimary);
    }

    private void DrawBtn(SpriteBatch sb, Rectangle r, string label, Color fill)
    {
        Game.DrawRect(sb, r, fill);
        Game.DrawBorder(sb, r, DesignTokens.Color.BorderFocus, 2);
        var size = Game.MeasureText(label, 1.6f);
        Game.DrawText(sb, label, new Vector2(r.X + (r.Width - size.X) * 0.5f, r.Y + (r.Height - size.Y) * 0.5f),
            DesignTokens.Color.TextPrimary, 1.6f);
    }
}
