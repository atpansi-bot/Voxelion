using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Xna.Framework;
using Voxelion.Core.Core;

namespace Voxelion.Android;

[Activity(
    Label = "@string/app_name",
    MainLauncher = true,
    Icon = "@mipmap/icon",
    Theme = "@style/MainTheme",
    AlwaysRetainTaskState = true,
    LaunchMode = LaunchMode.SingleInstance,
    ScreenOrientation = ScreenOrientation.SensorLandscape,
    ConfigurationChanges =
        ConfigChanges.Orientation |
        ConfigChanges.Keyboard |
        ConfigChanges.KeyboardHidden |
        ConfigChanges.ScreenSize |
        ConfigChanges.ScreenLayout |
        ConfigChanges.UiMode |
        ConfigChanges.SmallestScreenSize)]
public class MainActivity : AndroidGameActivity
{
    private VoxelionGame _game = null!;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Force immersive landscape game surface — never inflate activity_main / Hello layout
        _game = new VoxelionGame();
        var view = (View)_game.Services.GetService(typeof(View))!;
        SetContentView(view);
        _game.Run();
    }

    public override void OnWindowFocusChanged(bool hasFocus)
    {
        base.OnWindowFocusChanged(hasFocus);
        if (hasFocus)
            HideSystemUI();
    }

    private void HideSystemUI()
    {
        if (Window?.DecorView == null) return;
        if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
        {
            Window.SetDecorFitsSystemWindows(false);
            var controller = Window.InsetsController;
            if (controller != null)
            {
                controller.Hide(WindowInsets.Type.StatusBars() | WindowInsets.Type.NavigationBars());
                controller.SystemBarsBehavior = (int)WindowInsetsControllerBehavior.ShowTransientBarsBySwipe;
            }
        }
        else
        {
#pragma warning disable CS0618
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                SystemUiFlags.LayoutStable |
                SystemUiFlags.LayoutHideNavigation |
                SystemUiFlags.LayoutFullscreen |
                SystemUiFlags.HideNavigation |
                SystemUiFlags.Fullscreen |
                SystemUiFlags.ImmersiveSticky);
#pragma warning restore CS0618
        }
    }

    protected override void OnDestroy()
    {
        _game?.Dispose();
        base.OnDestroy();
    }
}
