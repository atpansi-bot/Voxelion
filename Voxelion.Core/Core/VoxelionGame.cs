using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Voxelion.Core.Input;
using Voxelion.Core.Localization;
using Voxelion.Core.UI.Theme;
using Voxelion.Core.Scenes;
using Voxelion.Core.Systems;
using Voxelion.Core.Data;

namespace Voxelion.Core.Core;

public sealed class VoxelionGame : Game
{
    private GraphicsDeviceManager _graphics = null!;
    private SpriteBatch _spriteBatch = null!;
    private Texture2D _pixel = null!;
    private SpriteFont? _font;

    private readonly InputManager _input = new();
    private readonly ApplicationStateMachine _stateMachine = new();
    private readonly LocalizationManager _loc = new();
    private readonly SessionService _session = new();
    private readonly PlayerProfile _profile = new();

    private SceneBase? _currentScene;
    private readonly Dictionary<ApplicationState, Func<SceneBase>> _sceneFactories = new();

    private float _bootTimer;
    private bool _assetsReady;

    public LocalizationManager Loc => _loc;
    public SessionService Session => _session;
    public PlayerProfile Profile => _profile;
    public ApplicationStateMachine StateMachine => _stateMachine;
    public Texture2D Pixel => _pixel;
    public SpriteFont? Font => _font;
    public SpriteBatch SpriteBatch => _spriteBatch;
    public GraphicsDeviceManager Graphics => _graphics;

    public VoxelionGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = "VOXELION";
        Window.AllowUserResizing = true;

        // Landscape default 16:9
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
        _graphics.ApplyChanges();

        RegisterScenes();
    }

    private void RegisterScenes()
    {
        _sceneFactories[ApplicationState.Boot] = () => new SceneBoot(this);
        _sceneFactories[ApplicationState.Splash] = () => new SceneSplash(this);
        _sceneFactories[ApplicationState.Loading] = () => new SceneLoading(this);
        _sceneFactories[ApplicationState.Title] = () => new SceneTitle(this);
        _sceneFactories[ApplicationState.Authentication] = () => new SceneAuth(this);
        _sceneFactories[ApplicationState.Registration] = () => new SceneRegister(this);
        _sceneFactories[ApplicationState.CharacterCreation] = () => new SceneCharacter(this);
        _sceneFactories[ApplicationState.Identity] = () => new SceneIdentity(this);
        _sceneFactories[ApplicationState.Welcome] = () => new SceneWelcome(this);
        _sceneFactories[ApplicationState.Transition] = () => new SceneTransition(this);
        _sceneFactories[ApplicationState.Hub] = () => new SceneHub(this);
        _sceneFactories[ApplicationState.WorldDiscovery] = () => new SceneDiscover(this);
        _sceneFactories[ApplicationState.WorldConnecting] = () => new SceneConnect(this);
        _sceneFactories[ApplicationState.WorldLoading] = () => new SceneWorldLoading(this);
        _sceneFactories[ApplicationState.World] = () => new SceneWorld(this);
    }

    protected override void Initialize()
    {
        _stateMachine.OnStateChanged += OnStateChanged;
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        // Attempt to load default font; fallback to null and use procedural text metrics
        try
        {
            // MonoGame default content may not have font; we handle null gracefully
            // _font = Content.Load<SpriteFont>("Fonts/Default");
        }
        catch { /* pure procedural */ }

        _assetsReady = true;
        TransitionTo(ApplicationState.Boot);
    }

    private void OnStateChanged(ApplicationState prev, ApplicationState next)
    {
        _currentScene?.OnExit();
        if (_sceneFactories.TryGetValue(next, out var factory))
        {
            _currentScene = factory();
            _currentScene.OnEnter();
        }
    }

    public void TransitionTo(ApplicationState state) => _stateMachine.TransitionTo(state);

    public void GoBack() => _stateMachine.GoBack();

    protected override void Update(GameTime gameTime)
    {
        _input.Update();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.F4) && Keyboard.GetState().IsKeyDown(Keys.LeftAlt))
            Exit();

        _currentScene?.Update(gameTime, _input.Current);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(DesignTokens.Color.VoidBlack);

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

        _currentScene?.Draw(_spriteBatch, gameTime);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    public void DrawRect(SpriteBatch sb, Rectangle r, Color c) => sb.Draw(_pixel, r, c);

    public void DrawRect(SpriteBatch sb, float x, float y, float w, float h, Color c) =>
        sb.Draw(_pixel, new Rectangle((int)x, (int)y, (int)w, (int)h), c);

    public void DrawBorder(SpriteBatch sb, Rectangle r, Color c, int thickness = 1)
    {
        DrawRect(sb, r.X, r.Y, r.Width, thickness, c);
        DrawRect(sb, r.X, r.Bottom - thickness, r.Width, thickness, c);
        DrawRect(sb, r.X, r.Y, thickness, r.Height, c);
        DrawRect(sb, r.Right - thickness, r.Y, thickness, r.Height, c);
    }

    public void DrawText(SpriteBatch sb, string text, Vector2 pos, Color color, float scale = 1f)
    {
        if (_font != null)
        {
            sb.DrawString(_font, text, pos, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            return;
        }
        // Procedural fallback: draw simple block text representation
        float charW = 8f * scale;
        float charH = 14f * scale;
        for (int i = 0; i < text.Length; i++)
        {
            char ch = text[i];
            if (ch == ' ') continue;
            float px = pos.X + i * charW;
            // Simple glyph as filled rect for visibility
            DrawRect(sb, px, pos.Y, charW * 0.7f, charH * 0.8f, color * 0.9f);
        }
    }

    public Vector2 MeasureText(string text, float scale = 1f)
    {
        if (_font != null) return _font.MeasureString(text) * scale;
        return new Vector2(text.Length * 8f * scale, 14f * scale);
    }
}
