using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Voxelion.Core.DesignSystem;
using Voxelion.Core.Input;
using Voxelion.Core.Localization;
using Voxelion.Core.State;
using Voxelion.Core.UI.Framework;
using Voxelion.Game.Scenes;
using Voxelion.Game.Systems;

namespace Voxelion.Game
{
    /// <summary>
    /// VOXELION root game class.
    /// Owns the complete UI/UX journey from boot to in-world HUD.
    /// No gameplay systems beyond UI/UX are implemented in this phase.
    /// </summary>
    public class VoxelionGame : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch? _spriteBatch;
        private InputManager? _input;
        private LocalizationManager? _localization;
        private UINavigationManager? _nav;
        private SceneManager? _scenes;
        private AudioUIManager? _audioUI;
        private SessionManager? _session;
        private float _bootTimer;
        private bool _contentLoaded;

        public static VoxelionGame? Instance { get; private set; }
        public GraphicsDeviceManager Graphics => _graphics;
        public SpriteBatch SpriteBatch => _spriteBatch!;
        public InputManager Input => _input!;
        public LocalizationManager Localization => _localization!;
        public UINavigationManager Navigation => _nav!;
        public SessionManager Session => _session!;

        public VoxelionGame()
        {
            Instance = this;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.Title = "VOXELION";
            Window.AllowUserResizing = true;

            // Landscape preferred defaults
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            _input = new InputManager();
            _localization = new LocalizationManager();
            _session = new SessionManager();
            _audioUI = new AudioUIManager();
            _nav = new UINavigationManager();
            _scenes = new SceneManager(this);

            // Force landscape awareness
            Window.ClientSizeChanged += OnClientSizeChanged;

            base.Initialize();
        }

        private void OnClientSizeChanged(object? sender, EventArgs e)
        {
            // Responsive: notify all scenes of new viewport
            _scenes?.OnViewportChanged(GraphicsDevice.Viewport.Bounds);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Order matches MASTER PROMPT boot sequence
            // Graphics already live
            // Input
            _input.Initialize();
            // Audio
            _audioUI.Initialize(Content);
            // Fonts
            LoadFonts();
            // Localization
            _localization.Initialize(Content);
            // UI Theme
            // Essential Assets (procedural or minimal for prototype purity)
            // Saved Session
            _session.LoadLocalSession();
            // Network State evaluated later

            _scenes.RegisterAll();
            _contentLoaded = true;

            // Start controlled boot
            _nav.TransitionTo(ApplicationState.Boot);
            _scenes.Activate(ApplicationState.Boot);
        }

        private void LoadFonts()
        {
            // MonoGame Content Pipeline would load .spritefont
            // For pure source delivery without prebuilt xnb we use a fallback
            // User must run MGCB to generate fonts. Runtime safety:
            try
            {
                Typography.FontRegular = Content.Load<SpriteFont>("Fonts/Regular");
                Typography.FontBold = Content.Load<SpriteFont>("Fonts/Bold");
                Typography.FontDisplay = Content.Load<SpriteFont>("Fonts/Display");
            }
            catch
            {
                // Fallback: system will use debug drawing until fonts are built
                // Production APK/PC build must include generated fonts via Content.mgcb
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape) && _nav.Current == ApplicationState.Title)
            {
                // Escape only exits from title or when explicitly allowed
            }

            _input.Update(gameTime);
            _nav.Update(gameTime);
            _scenes.Update(gameTime, _input.Current);
            _audioUI.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ColorTokens.BgDeep);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

            _scenes.Draw(_spriteBatch, GraphicsDevice.Viewport.Bounds);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            _audioUI?.Dispose();
            base.UnloadContent();
        }
    }
}
