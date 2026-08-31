using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Input;
using Voxelion.Core.Localization;

namespace Voxelion.Game.Scenes
{
    public abstract class SceneBase
    {
        public bool IsLoaded { get; protected set; } = false;

        public virtual void Enter() { }
        public virtual void Exit() { }

        public virtual void LoadContent()
        {
            IsLoaded = true;
        }

        public virtual void UnloadContent()
        {
            IsLoaded = false;
        }

        public virtual void Update(GameTime gameTime) { }
        public virtual void Update(GameTime gameTime, InputState inputState)
        {
            Update(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime) { }
        public virtual void Draw(SpriteBatch spriteBatch, Rectangle bounds)
        {
            Draw(spriteBatch, new GameTime());
        }

        public virtual bool HandleBack()
        {
            return false;
        }
    }
}
