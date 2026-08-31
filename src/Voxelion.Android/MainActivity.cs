using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Xna.Framework;

namespace Voxelion.Android
{
    [Activity(
        Label = "VOXELION",
        MainLauncher = true,
        Icon = "@drawable/icon",
        Theme = "@style/Theme.Splash",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.SensorLandscape,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize
    )]
    public class MainActivity : AndroidGameActivity
    {
        private Voxelion.Game.VoxelionGame _game;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _game = new Voxelion.Game.VoxelionGame();
            SetContentView((View)_game.Services.GetService(typeof(View)));
            _game.Run();
        }
    }
}
