using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.DesignSystem;
using Voxelion.Core.Input;
using Voxelion.Core.State;

namespace Voxelion.Game.Scenes
{
    public class SceneSplash : SceneBase
    {
        private float _timer;
        private float _alpha;
        private const float MinDisplay = 1.4f;
        private bool _canSkip;

        public SceneSplash(VoxelionGame game) : base(game, ApplicationState.Splash) { }

        public override void Enter()
        {
            base.Enter();
            _timer = 0f;
            _alpha = 0f;
            _canSkip = false;
        }

        public override void Update(GameTime gameTime, InputState input)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _timer += dt;
            _alpha = MotionSystem.EaseOutCubic(MathHelper.Clamp(_timer / 0.6f, 0f, 1f));
            if (_timer >= MinDisplay) _canSkip = true;

            if ((_canSkip && (input.PointerPressed || input.Confirm || input.PrimaryAction)) || _timer > 3.2f)
                Nav.TransitionTo(ApplicationState.Loading);
        }

        public override void Draw(SpriteBatch sb, Rectangle viewport)
        {
            var font = Typography.FontDisplay ?? Typography.FontRegular;
            if (font == null) return;
            string title = "VOXELION";
            float scale = Typography.GetScale(Typography.Style.DisplayHero);
            var size = font.MeasureString(title) * scale;
            var pos = new Vector2(viewport.Width / 2f - size.X / 2f, viewport.Height / 2f - size.Y / 2f - 20);
            sb.DrawString(font, title, pos, ColorTokens.WithAlpha(ColorTokens.EmblemCore, (byte)(_alpha * 255)),
                0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
