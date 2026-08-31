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

    private SceneBase? _currentScene;
    private readonly Dictionary<ApplicationState, Func<SceneBase>> _sceneFactories = new();
    private bool _started;
    private double _totalSeconds;

    public LocalizationManager Loc => _loc;
    public SessionService Session => _session;
    public PlayerProfile Profile => _profile;
    public ApplicationStateMachine StateMachine => _stateMachine;
    public Texture2D Pixel => _pixel;
    public PixelFont Font => _font;
    public SpriteBatch SpriteBatch => _spriteBatch;
    public GraphicsDeviceManager Graphics => _graphics;
    public double TotalSeconds => _totalSeconds;

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
            else
            {
                _currentScene = null;
            }
        }
        catch
        {
            _currentScene = null;
        }
    }

    public void TransitionTo(ApplicationState state) => _stateMachine.TransitionTo(state);
    public void GoBack() => _stateMachine.GoBack();

    protected override void Update(GameTime gameTime)
    {
        _totalSeconds = gameTime.TotalGameTime.TotalSeconds;

        try { _input.Update(); }
        catch { /* early Android frames */ }

        try { _currentScene?.Update(gameTime, _input.Current); }
        catch { /* keep loop alive */ }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(DesignTokens.Color.VoidBlack);

        if (!_started || _spriteBatch == null || _pixel == null)
        {
            base.Draw(gameTime);
            return;
        }

        var vp = GraphicsDevice.Viewport;
        if (vp.Width < 2 || vp.Height < 2)
        {
            base.Draw(gameTime);
            return;
        }

        _spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone);

        try
        {
            if (_currentScene != null)
                _currentScene.Draw(_spriteBatch, gameTime);
            else
            {
                float cx = vp.Width * 0.5f;
                float cy = vp.Height * 0.5f;
                DrawRect(_spriteBatch, cx - 40, cy - 40, 80, 80, DesignTokens.Color.AccentPrimary);
                DrawRect(_spriteBatch, cx - 20, cy - 20, 40, 40, DesignTokens.Color.AccentSecondary);
                DrawText(_spriteBatch, "VOXELION", new Vector2(cx - 56, cy + 52), DesignTokens.Color.TextPrimary, 2f);
            }
        }
        catch
        {
            DrawRect(_spriteBatch, 0, 0, vp.Width, 12, DesignTokens.Color.AccentDanger);
        }

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

    public Vector2 MeasureText(string text, float scale = 1f)
    {
        if (string.IsNullOrEmpty(text)) return Vector2.Zero;
        return _font.Measure(text, scale);
    }

    /// <summary>Draw a tappable panel button. Returns true if pressed this frame.</summary>
    public bool DrawButton(SpriteBatch sb, InputState input, Rectangle bounds, string label, Color fill, Color border, float textScale = 2f)
    {
        bool hover = bounds.Contains(input.PointerPosition);
        bool pressed = hover && input.IsPointerDown;
        Color bg = pressed ? fill * 0.75f : hover ? fill * 0.9f : fill;
        DrawRect(sb, bounds, bg);
        DrawBorder(sb, bounds, border, 2);

        var size = MeasureText(label, textScale);
        var tp = new Vector2(
            bounds.X + (bounds.Width - size.X) * 0.5f,
            bounds.Y + (bounds.Height - size.Y) * 0.5f);
        DrawText(sb, label, tp, DesignTokens.Color.TextPrimary, textScale);

        return hover && input.IsPointerReleased;
    }
}
