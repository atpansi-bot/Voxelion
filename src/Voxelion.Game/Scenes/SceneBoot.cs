using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.DesignSystem;
using Voxelion.Core.Input;
using Voxelion.Core.State;

namespace Voxelion.Game.Scenes
{
    public class SceneBoot : SceneBase
    {
        private float _timer;
        private float _emblemAlpha;
        private readonly System.Random _rng = new();
        private Vector2[] _particles;
        private float[] _particleSpeed;

        public SceneBoot(VoxelionGame game) : base(game, ApplicationState.Boot)
        {
            _particles = new Vector2[48];
            _particleSpeed = new float[48];
            for (int i = 0; i < 48; i++)
            {
                _particles[i] = new Vector2(_rng.Next(0, 1920), _rng.Next(0, 1080));
                _particleSpeed[i] = 8f + (float)_rng.NextDouble() * 20f;
            }
        }

        public override void Enter()
        {
            base.Enter();
            _timer = 0f;
            _emblemAlpha = 0f;
        }

        public override void Update(GameTime gameTime, InputState input)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _timer += dt;
            _emblemAlpha = MotionSystem.EaseOutCubic(MathHelper.Clamp(_timer / 1.2f, 0f, 1f));

            for (int i = 0; i < _particles.Length; i++)
            {
                _particles[i].Y -= _particleSpeed[i] * dt;
                if (_particles[i].Y < -10)
                {
                    _particles[i].Y = 1100;
                    _particles[i].X = _rng.Next(0, 1920);
                }
            }

            // Controlled boot duration then move to Splash
            if (_timer > 2.2f)
                Nav.TransitionTo(ApplicationState.Splash);
        }

        public override void Draw(SpriteBatch sb, Rectangle viewport)
        {
            // Dark cinematic background
            // Particles
            foreach (var p in _particles)
            {
                var c = ColorTokens.WithAlpha(ColorTokens.ParticleLight, (byte)(30 + _rng.Next(40)));
                sb.Draw(GetPixel(), new Rectangle((int)p.X, (int)p.Y, 2, 2), c);
            }

            // Emblem placeholder (text until texture loaded)
            string emblem = "VOXELION";
            var font = Typography.FontDisplay ?? Typography.FontBold ?? Typography.FontRegular;
            if (font != null)
            {
                var size = font.MeasureString(emblem) * Typography.GetScale(Typography.Style.DisplayHero);
                var pos = new Vector2(viewport.Width / 2f - size.X / 2f, viewport.Height / 2f - size.Y / 2f);
                sb.DrawString(font, emblem, pos, ColorTokens.WithAlpha(ColorTokens.EmblemCore, (byte)(_emblemAlpha * 255)),
                    0f, Vector2.Zero, Typography.GetScale(Typography.Style.DisplayHero), SpriteEffects.None, 0f);
            }
        }

        private Texture2D _pixel;
        private Texture2D GetPixel()
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(Game.GraphicsDevice, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }
            return _pixel;
        }
    }
}
