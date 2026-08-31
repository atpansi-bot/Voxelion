using System;
using Voxelion.Game;

namespace Voxelion.Desktop
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new VoxelionGame();
            game.Run();
        }
    }
}
