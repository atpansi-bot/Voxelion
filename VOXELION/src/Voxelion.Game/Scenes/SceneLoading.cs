using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.DesignSystem;
using Voxelion.Core.Input;
using Voxelion.Core.State;

namespace Voxelion.Game.Scenes
{
    public class SceneLoading : SceneBase
    {
        private float _progress;
        private float _timer;
        private string _statusKey = "loading.preparing";
        private bool _failed;
        private Texture2D _pixel;

        public SceneLoading(VoxelionGame game) : base(game, ApplicationState.Loading) { }

        public override void Enter()
        {
            base.Enter();
            _progress = 0f;
            _timer = 0f;
            _failed = false;
            _statusKey = "loading.preparing";
        }

        public override void Update(GameTime gameTime, InputState input)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _timer += dt;

            // Real measurable progress simulation for essential assets
            // In production replace with actual Content.Load progress callbacks
            if (!_failed)
            {
                _progress = MathHelper.Clamp(_timer / 2.8f, 0f, 1f);
                if (_progress < 0.4f) _statusKey = "loading.preparing";
                else if (_progress < 0.85f) _statusKey = "loading.init";
                else _statusKey = "loading.init";

                if (_progress >= 1f)
                    Nav.TransitionTo(ApplicationState.Title);
            }

            if (_failed && input.PointerPressed)
            {
                _failed = false;
                _timer = 0f;
                _progress = 0f;
            }
        }

        public override void Draw(SpriteBatch sb, Rectangle viewport)
        {
            EnsurePixel();
            var font = Typography.FontRegular;
            if (font == null) return;

            // Top bar
            string brand = Loc["app.name"];
            sb.DrawString(font, brand, new Vector2(Spacing.Xl, Spacing.Lg), ColorTokens.TextPrimary, 0, Vector2.Zero, 1.1f, SpriteEffects.None, 0);

            // Language chip
            string lang = "[ 🌐 " + Loc.CurrentLanguage.ToUpper() + " ]";
            var langSize = font.MeasureString(lang);
            sb.DrawString(font, lang, new Vector2(viewport.Width - langSize.X - Spacing.Xl, Spacing.Lg), ColorTokens.TextSecondary);

            // Center status
            string status = Loc[_statusKey];
            var stSize = font.MeasureString(status);
            sb.DrawString(font, status, new Vector2(viewport.Width / 2f - stSize.X / 2f, viewport.Height / 2f + 40), ColorTokens.TextSecondary);

            // Crystal progress bar
            int barW = (int)(viewport.Width * 0.4f);
            int barH = 12;
            int barX = viewport.Width / 2 - barW / 2;
            int barY = viewport.Height / 2 + 80;
            sb.Draw(_pixel, new Rectangle(barX, barY, barW, barH), ColorTokens.WithAlpha(ColorTokens.CrystalProgress, 60));
            sb.Draw(_pixel, new Rectangle(barX, barY, (int)(barW * _progress), barH), ColorTokens.CrystalProgressFill);

            // Version
            sb.DrawString(font, "1.0.0", new Vector2(viewport.Width - 80, viewport.Height - 36), ColorTokens.TextMuted, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);

            if (_failed)
            {
                string retry = Loc["error.retry"];
                var rSize = font.MeasureString(retry);
                sb.DrawString(font, retry, new Vector2(viewport.Width / 2f - rSize.X / 2f, barY + 40), ColorTokens.TextError);
            }
        }

        private void EnsurePixel()
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(Game.GraphicsDevice, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }
        }
    }
}
