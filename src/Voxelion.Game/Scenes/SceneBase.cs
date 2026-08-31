using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Localization;

namespace Voxelion.Game.Scenes
{
    public abstract class SceneBase
    {
        public bool IsLoaded { get; protected set; } = false;

        public virtual void LoadContent()
        {
            IsLoaded = true;
        }

        public virtual void UnloadContent()
        {
            IsLoaded = false;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
