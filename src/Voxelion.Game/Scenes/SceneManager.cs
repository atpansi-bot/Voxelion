using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Input;
using Voxelion.Core.State;
using Voxelion.Game.Systems;

namespace Voxelion.Game.Scenes
{
    public class SceneManager
    {
        private readonly VoxelionGame _game;
        private readonly Dictionary<ApplicationState, SceneBase> _scenes = new();
        private SceneBase _active;

        public SceneManager(VoxelionGame game)
        {
            _game = game;
            _game.Navigation.OnStateChanged += OnNavChanged;
        }

        public void RegisterAll()
        {
            // Full journey registration — every stage from MASTER PROMPT
            Register(new SceneBoot(_game));
            Register(new SceneSplash(_game));
            Register(new SceneLoading(_game));
            Register(new SceneTitle(_game));
            Register(new SceneAuth(_game));
            Register(new SceneRegister(_game));
            Register(new SceneCharacter(_game));
            Register(new SceneIdentity(_game));
            Register(new SceneWelcome(_game));
            Register(new SceneTransition(_game));
            Register(new SceneHub(_game));
            Register(new SceneDiscover(_game));
            Register(new SceneWorldConnect(_game));
            Register(new SceneWorldLoading(_game));
            Register(new SceneWorld(_game));
        }

        private void Register(SceneBase scene) => _scenes[scene.StateId] = scene;

        private void OnNavChanged(ApplicationState prev, ApplicationState next)
        {
            if (_scenes.TryGetValue(prev, out var old)) old.Exit();
            if (_scenes.TryGetValue(next, out var neu))
            {
                _active = neu;
                neu.Enter();
            }
        }

        public void Activate(ApplicationState state)
        {
            if (_scenes.TryGetValue(state, out var s))
            {
                _active?.Exit();
                _active = s;
                s.Enter();
            }
        }

        public void Update(GameTime gt, InputState input)
        {
            _active?.Update(gt, input);
            if (input.Back)
                _active?.HandleBack();
        }

        public void Draw(SpriteBatch sb, Rectangle vp) => _active?.Draw(sb, vp);
        public void OnViewportChanged(Rectangle vp)
        {
            foreach (var s in _scenes.Values) s.OnViewportChanged(vp);
        }
    }
}
