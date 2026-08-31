namespace Voxelion.Core.State
{
    /// <summary>
    /// Explicit application state machine.
    /// Navigation is deterministic. Invalid transitions are impossible at the type level where practical.
    /// </summary>
    public enum ApplicationState
    {
        Boot,
        Splash,
        Loading,
        Title,
        Authentication,
        Registration,
        CharacterCreation,
        Identity,
        Welcome,
        TransitionToHub,
        Hub,
        WorldDiscovery,
        WorldDetails,
        WorldConnecting,
        WorldLoading,
        World,
        // Overlay states managed separately
    }

    public enum OverlayState
    {
        None,
        Modal,
        Dialog,
        Notification,
        ContextMenu,
        QuickMenu,
        Chat,
        Tooltip,
        Settings,
        Inventory,
        Profile,
        Social,
        PauseMenu
    }

    public enum SessionState
    {
        Unknown,
        Valid,
        None,
        Expired,
        NetworkUnavailable
    }

    public enum AuthFlowState
    {
        Idle,
        Editing,
        Validating,
        Connecting,
        Success,
        Failed,
        Retry
    }

    public enum ConnectionPhase
    {
        Selected,
        Connecting,
        Authenticating,
        LoadingRegion,
        SyncingPlayer,
        Spawning,
        Ready,
        Failed,
        Timeout
    }
}
