using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Input;
using Voxelion.Core.Localization;
using Voxelion.Core.State;
using Voxelion.Game.Systems;

namespace Voxelion.Game.Scenes
{
    /// <summary>
    /// Base class for every application scene in the VOXELION UI/UX journey.
    /// Constructor requires (VoxelionGame, ApplicationState).
    /// All derived scenes must call : base(game, ApplicationState.XXX)
    /// </summary>
    public abstract class SceneBase
    {
        protected readonly VoxelionGame Game;
        protected UINavigationManager Nav => Game.Navigation;
        protected LocalizationManager Loc => Game.Localization;
        protected SessionManager Session => Game.Session;

        public ApplicationState StateId { get; protected set; }
        public bool IsActive { get; set; }
        public bool IsLoaded { get; protected set; }

        protected SceneBase(VoxelionGame game, ApplicationState id)
        {
            Game = game ?? throw new System.ArgumentNullException(nameof(game));
            StateId = id;
        }

        public virtual void Enter()
        {
            IsActive = true;
        }

        public virtual void Exit()
        {
            IsActive = false;
        }

        public virtual void LoadContent()
        {
            IsLoaded = true;
        }

        public virtual void UnloadContent()
        {
            IsLoaded = false;
        }

        public virtual void Update(GameTime gameTime, InputState input)
        {
        }

        public virtual void Draw(SpriteBatch sb, Rectangle viewport)
        {
        }

        public virtual void OnViewportChanged(Rectangle viewport)
        {
        }

        public virtual bool HandleBack()
        {
            return false;
        }
    }
}
