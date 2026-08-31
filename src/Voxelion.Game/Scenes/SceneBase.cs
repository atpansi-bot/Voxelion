using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Input;
using Voxelion.Core.State;
using Voxelion.Game.Systems;

namespace Voxelion.Game.Scenes
{
    public abstract class SceneBase
    {
        protected VoxelionGame Game;
        protected UINavigationManager Nav => Game.Navigation;
        protected Localization.LocalizationManager Loc => Game.Localization;
        protected SessionManager Session => Game.Session;
        public ApplicationState StateId { get; protected set; }
        public bool IsActive { get; set; }

        protected SceneBase(VoxelionGame game, ApplicationState id)
        {
            Game = game;
            StateId = id;
        }

        public virtual void Enter() { IsActive = true; }
        public virtual void Exit() { IsActive = false; }
        public virtual void Update(GameTime gameTime, InputState input) { }
        public virtual void Draw(SpriteBatch sb, Rectangle viewport) { }
        public virtual void OnViewportChanged(Rectangle viewport) { }
        public virtual bool HandleBack() => false;
    }
}
