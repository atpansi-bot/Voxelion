using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Voxelion.Core.Input;
using Voxelion.Core.Localization;
using Voxelion.Core.UI;
using Voxelion.Core.UI.Theme;
using Voxelion.Core.Scenes;
using Voxelion.Core.Data;

namespace Voxelion.Core.Core;

public sealed class VoxelionGame : Game
{
    private GraphicsDeviceManager _graphics = null!;
    private SpriteBatch _spriteBatch = null!;
    private Texture2D _pixel = null!;
    private PixelFont _font = null!;

    private readonly InputManager _input = new();
    private readonly ApplicationStateMachine _stateMachine = new();
    private readonly LocalizationManager _loc = new();
    private readonly SessionService _session = new();
    private readonly PlayerProfile _profile = new();
    private readonly NotificationBus _toasts = new();

    private SceneBase? _currentScene;
    private readonly Dictionary<ApplicationState, Func<SceneBase>> _sceneFactories = new();
    private bool _started;

    public LocalizationManager Loc => _loc;
    public SessionService Session => _session;
    public PlayerProfile Profile => _profile;
    public ApplicationStateMachine StateMachine => _stateMachine;
    public NotificationBus Toasts => _toasts;
    public Texture2D Pixel => _pixel;
    public PixelFont Font => _font;
    public SpriteBatch SpriteBatch => _spriteBatch;
    public GraphicsDeviceManager Graphics => _graphics;

    public VoxelionGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = "VOXELION";
        Window.AllowUserResizing = true;
        _graphics.SupportedOrientations =
            DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
        _graphics.IsFullScreen = false;
        _graphics.SynchronizeWithVerticalRetrace = true;
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);
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
        _sceneFactories[ApplicationState.Inventory] = () => new SceneInventory(this);
        _sceneFactories[ApplicationState.Settings] = () => new SceneSettings(this);
        _sceneFactories[ApplicationState.Social] = () => new SceneSocial(this);
        _sceneFactories[ApplicationState.PauseMenu] = () => new ScenePause(this);
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
        _font = new PixelFont(_pixel);
        _started = true;
        TransitionTo(ApplicationState.Boot);
        if (_currentScene == null && _sceneFactories.TryGetValue(ApplicationState.Boot, out var f))
        {
            _currentScene = f();
            _currentScene.OnEnter();
        }
    }

    private void OnStateChanged(ApplicationState prev, ApplicationState next)
    {
        try
        {
            _currentScene?.OnExit();
            if (_sceneFactories.TryGetValue(next, out var factory))
            {
                _currentScene = factory();
                _currentScene.OnEnter();
            }
            else _currentScene = null;
        }
        catch { _currentScene = null; }
    }

    public void TransitionTo(ApplicationState state) => _stateMachine.TransitionTo(state);
    public void GoBack() => _stateMachine.GoBack();

    protected override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        try { _input.Update(); } catch { }
        try { _currentScene?.Update(gameTime, _input.Current); } catch { }
        _toasts.Update(dt);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(DesignTokens.Color.VoidBlack);
        if (!_started || _spriteBatch == null || _pixel == null) { base.Draw(gameTime); return; }
        var vp = GraphicsDevice.Viewport;
        if (vp.Width < 2 || vp.Height < 2) { base.Draw(gameTime); return; }

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
            DepthStencilState.None, RasterizerState.CullNone);
        try
        {
            if (_currentScene != null)
                _currentScene.Draw(_spriteBatch, gameTime);
            else
            {
                float cx = vp.Width * 0.5f, cy = vp.Height * 0.5f;
                DrawRect(_spriteBatch, cx - 40, cy - 40, 80, 80, DesignTokens.Color.AccentPrimary);
                DrawText(_spriteBatch, "VOXELION", new Vector2(cx - 56, cy + 52), DesignTokens.Color.TextPrimary, 2f);
            }
            _toasts.Draw(this, _spriteBatch, vp);
        }
        catch { DrawRect(_spriteBatch, 0, 0, vp.Width, 12, DesignTokens.Color.AccentDanger); }
        _spriteBatch.End();
        base.Draw(gameTime);
    }

    public void DrawRect(SpriteBatch sb, Rectangle r, Color c) => sb.Draw(_pixel, r, c);
    public void DrawRect(SpriteBatch sb, float x, float y, float w, float h, Color c)
    {
        if (w < 1 || h < 1) return;
        sb.Draw(_pixel, new Rectangle((int)x, (int)y, Math.Max(1, (int)w), Math.Max(1, (int)h)), c);
    }
    public void DrawBorder(SpriteBatch sb, Rectangle r, Color c, int thickness = 2)
    {
        DrawRect(sb, r.X, r.Y, r.Width, thickness, c);
        DrawRect(sb, r.X, r.Bottom - thickness, r.Width, thickness, c);
        DrawRect(sb, r.X, r.Y, thickness, r.Height, c);
        DrawRect(sb, r.Right - thickness, r.Y, thickness, r.Height, c);
    }
    public void DrawText(SpriteBatch sb, string text, Vector2 pos, Color color, float scale = 1f)
    {
        if (string.IsNullOrEmpty(text)) return;
        _font.Draw(sb, text, pos, color, scale);
    }
    public Vector2 MeasureText(string text, float scale = 1f) =>
        string.IsNullOrEmpty(text) ? Vector2.Zero : _font.Measure(text, scale);

    public void DrawIcon(SpriteBatch sb, string id, Rectangle bounds, Color color) =>
        Icons.Draw(sb, _pixel, id, bounds, color);
}
