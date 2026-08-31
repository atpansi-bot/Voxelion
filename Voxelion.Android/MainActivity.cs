using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Xna.Framework;
using Voxelion.Core.Core;

namespace Voxelion.Android;

[Activity(
    Label = "VOXELION",
    MainLauncher = true,
    Icon = "@mipmap/icon",
    Theme = "@android:style/Theme.NoTitleBar.Fullscreen",
    AlwaysRetainTaskState = true,
    LaunchMode = LaunchMode.SingleInstance,
    ScreenOrientation = ScreenOrientation.SensorLandscape,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize | ConfigChanges.ScreenLayout)]
public class MainActivity : AndroidGameActivity
{
    private VoxelionGame? _game;
    private View? _view;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        _game = new VoxelionGame();
        _view = _game.Services.GetService(typeof(View)) as View;
        SetContentView(_view);
        _game.Run();
    }

    protected override void OnDestroy()
    {
        _game?.Dispose();
        base.OnDestroy();
    }
}
