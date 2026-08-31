using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;

namespace Voxelion.Core.Scenes;

public abstract class SceneBase
{
    protected readonly VoxelionGame Game;
    protected float EnterTime;
    protected float SceneTime;
    protected bool IsActive;

    protected SceneBase(VoxelionGame game) => Game = game;

    public virtual void OnEnter()
    {
        IsActive = true;
        EnterTime = 0f;
        SceneTime = 0f;
    }

    public virtual void OnExit()
    {
        IsActive = false;
    }

    public virtual void Update(GameTime gameTime, InputState input)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        EnterTime += dt;
        SceneTime += dt;
    }

    public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);

    protected float EaseOutCubic(float t) => 1f - MathF.Pow(1f - MathHelper.Clamp(t, 0, 1), 3);
    protected float EaseInOut(float t)
    {
        t = MathHelper.Clamp(t, 0, 1);
        return t < 0.5f ? 4 * t * t * t : 1 - MathF.Pow(-2 * t + 2, 3) / 2;
    }
}
