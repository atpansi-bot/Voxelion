using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Voxelion.Game.Systems
{
    /// <summary>
    /// UI audio feedback. Independently volume-controlled.
    /// Sounds: hover, click, confirm, cancel, error, notification, menu open/close, world entry.
    /// </summary>
    public class AudioUIManager : IDisposable
    {
        private float _uiVolume = 0.7f;
        private ContentManager _content;

        public float UIVolume
        {
            get => _uiVolume;
            set => _uiVolume = MathHelper.Clamp(value, 0f, 1f);
        }

        public void Initialize(ContentManager content)
        {
            _content = content;
            // SoundEffect instances loaded from Content/Audio when available
            // Production: Content.Load<SoundEffect>("Audio/ui_click") etc.
        }

        public void PlayHover() { /* Play if loaded */ }
        public void PlayClick() { }
        public void PlayConfirm() { }
        public void PlayCancel() { }
        public void PlayError() { }
        public void PlayNotification() { }
        public void PlayMenuOpen() { }
        public void PlayMenuClose() { }
        public void PlayWorldEntry() { }

        public void Update(GameTime gameTime) { }

        public void Dispose() { }
    }
}
