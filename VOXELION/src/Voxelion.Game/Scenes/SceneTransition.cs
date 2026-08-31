using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.DesignSystem;
using Voxelion.Core.Input;
using Voxelion.Core.State;

namespace Voxelion.Game.Scenes
{
    public class SceneTransition : SceneBase
    {
        private float _enterAlpha;
        private Texture2D _pixel;

        public SceneTransition(VoxelionGame game) : base(game, ApplicationState.TransitionToHub) { }

        public override void Enter()
        {
            base.Enter();
            _enterAlpha = 0f;
        }

        public override void Update(GameTime gameTime, InputState input)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _enterAlpha = MotionSystem.Lerp(_enterAlpha, 1f, 1f - MathF.Exp(-8f * dt));
        }

        public override void Draw(SpriteBatch sb, Rectangle viewport)
        {
            EnsurePixel();
            var font = Typography.FontRegular;
            if (font == null) return;
            string label = StateId.ToString();
            var size = font.MeasureString(label);
            sb.DrawString(font, label, new Vector2(viewport.Width/2f - size.X/2f, viewport.Height/2f), 
                ColorTokens.WithAlpha(ColorTokens.TextPrimary, (byte)(_enterAlpha*255)));
        }

        public override bool HandleBack()
        {
            return Nav.GoBack();
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
