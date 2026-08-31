using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Voxelion.Core.Input;
using Voxelion.Core.Localization;
using Voxelion.Core.UI.Theme;
using Voxelion.Core.Scenes;
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
    private bool _started;

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

        // Jangan hardcode resolusi di Android — biarkan surface activity
        _graphics.SupportedOrientations =
            DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
        _graphics.IsFullScreen = false;
        _graphics.SynchronizeWithVerticalRetrace = true;

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
        try
        {
            _input.Update();
        }
        catch
        {
            // TouchPanel/GamePad bisa gagal di frame awal Android — abaikan
        }

        try
        {
            _currentScene?.Update(gameTime, _input.Current);
        }
        catch
        {
            // scene error tidak boleh menghentikan loop
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        var clear = DesignTokens.Color.VoidBlack;
        GraphicsDevice.Clear(clear);

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
            {
                _currentScene.Draw(_spriteBatch, gameTime);
            }
            else
            {
                // Fallback visual supaya tidak pure black
                float cx = vp.Width * 0.5f;
                float cy = vp.Height * 0.5f;
                DrawRect(_spriteBatch, cx - 40, cy - 40, 80, 80, DesignTokens.Color.AccentPrimary);
                DrawRect(_spriteBatch, cx - 20, cy - 20, 40, 40, DesignTokens.Color.AccentSecondary);
                DrawText(_spriteBatch, "VOXELION", new Vector2(cx - 48, cy + 50), DesignTokens.Color.TextPrimary, 1.2f);
            }
        }
        catch
        {
            DrawRect(_spriteBatch, 0, 0, vp.Width, 8, DesignTokens.Color.AccentDanger);
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

    public void DrawBorder(SpriteBatch sb, Rectangle r, Color c, int thickness = 1)
    {
        DrawRect(sb, r.X, r.Y, r.Width, thickness, c);
        DrawRect(sb, r.X, r.Bottom - thickness, r.Width, thickness, c);
        DrawRect(sb, r.X, r.Y, thickness, r.Height, c);
        DrawRect(sb, r.Right - thickness, r.Y, thickness, r.Height, c);
    }

    public void DrawText(SpriteBatch sb, string text, Vector2 pos, Color color, float scale = 1f)
    {
        if (string.IsNullOrEmpty(text)) return;
        if (_font != null)
        {
            sb.DrawString(_font, text, pos, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            return;
        }
        float charW = 8f * scale;
        float charH = 14f * scale;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == ' ') continue;
            float px = pos.X + i * charW;
            DrawRect(sb, px, pos.Y, charW * 0.7f, charH * 0.8f, color * 0.95f);
        }
    }

    public Vector2 MeasureText(string text, float scale = 1f)
    {
        if (_font != null) return _font.MeasureString(text) * scale;
        if (string.IsNullOrEmpty(text)) return Vector2.Zero;
        return new Vector2(text.Length * 8f * scale, 14f * scale);
    }
}
