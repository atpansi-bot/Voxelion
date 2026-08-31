using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Voxelion.Core.State;

namespace Voxelion.Game.Systems
{
    /// <summary>
    /// Deterministic navigation. Forward, back, escape, modal rules.
    /// Player can never become trapped.
    /// </summary>
    public class UINavigationManager
    {
        private readonly Stack<ApplicationState> _history = new();
        public ApplicationState Current { get; private set; } = ApplicationState.Boot;
        public OverlayState Overlay { get; private set; } = OverlayState.None;
        public event Action<ApplicationState, ApplicationState> OnStateChanged;
        public event Action<OverlayState> OnOverlayChanged;

        private static readonly Dictionary<ApplicationState, ApplicationState> BackMap = new()
        {
            [ApplicationState.Splash] = ApplicationState.Boot,
            [ApplicationState.Loading] = ApplicationState.Splash,
            [ApplicationState.Title] = ApplicationState.Title, // root
            [ApplicationState.Authentication] = ApplicationState.Title,
            [ApplicationState.Registration] = ApplicationState.Authentication,
            [ApplicationState.CharacterCreation] = ApplicationState.Authentication,
            [ApplicationState.Identity] = ApplicationState.CharacterCreation,
            [ApplicationState.Welcome] = ApplicationState.Identity,
            [ApplicationState.TransitionToHub] = ApplicationState.Welcome,
            [ApplicationState.Hub] = ApplicationState.Hub,
            [ApplicationState.WorldDiscovery] = ApplicationState.Hub,
            [ApplicationState.WorldDetails] = ApplicationState.WorldDiscovery,
            [ApplicationState.WorldConnecting] = ApplicationState.WorldDetails,
            [ApplicationState.WorldLoading] = ApplicationState.WorldConnecting,
            [ApplicationState.World] = ApplicationState.Hub
        };

        public void TransitionTo(ApplicationState next)
        {
            if (next == Current) return;
            var prev = Current;
            if (CanPushHistory(prev))
                _history.Push(prev);
            Current = next;
            OnStateChanged?.Invoke(prev, next);
        }

        public bool GoBack()
        {
            if (Overlay != OverlayState.None)
            {
                CloseOverlay();
                return true;
            }
            if (_history.Count == 0) return false;
            var prev = Current;
            Current = _history.Pop();
            OnStateChanged?.Invoke(prev, Current);
            return true;
        }

        public void OpenOverlay(OverlayState overlay)
        {
            Overlay = overlay;
            OnOverlayChanged?.Invoke(overlay);
        }

        public void CloseOverlay()
        {
            Overlay = OverlayState.None;
            OnOverlayChanged?.Invoke(OverlayState.None);
        }

        private bool CanPushHistory(ApplicationState s)
        {
            return s != ApplicationState.Boot &&
                   s != ApplicationState.Splash &&
                   s != ApplicationState.Loading &&
                   s != ApplicationState.TransitionToHub &&
                   s != ApplicationState.WorldConnecting &&
                   s != ApplicationState.WorldLoading;
        }

        public void Update(GameTime gameTime) { /* transition timers if needed */ }
    }
}
