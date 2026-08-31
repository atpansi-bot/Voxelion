namespace Voxelion.Core.Core;

public enum ApplicationState
{
    None = -1,
    Boot = 0,
    Splash,
    Loading,
    Title,
    Authentication,
    Registration,
    CharacterCreation,
    Identity,
    Welcome,
    Transition,
    Hub,
    WorldDiscovery,
    WorldDetails,
    WorldConnecting,
    WorldLoading,
    World,
    PauseMenu,
    Inventory,
    Settings,
    Social
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
    Inventory,
    Social,
    Profile,
    Settings,
    Error
}

public sealed class ApplicationStateMachine
{
    public ApplicationState Current { get; private set; } = ApplicationState.None;
    public ApplicationState Previous { get; private set; } = ApplicationState.None;
    public OverlayState Overlay { get; private set; } = OverlayState.None;

    public event Action<ApplicationState, ApplicationState>? OnStateChanged;
    public event Action<OverlayState, OverlayState>? OnOverlayChanged;

    private readonly Stack<ApplicationState> _history = new();

    public void TransitionTo(ApplicationState next)
    {
        if (next == Current) return;
        Previous = Current;
        if (Current != ApplicationState.None)
            _history.Push(Current);
        Current = next;
        OnStateChanged?.Invoke(Previous, Current);
    }

    public bool CanGoBack => _history.Count > 0;

    public void GoBack()
    {
        if (_history.Count == 0) return;
        Previous = Current;
        Current = _history.Pop();
        OnStateChanged?.Invoke(Previous, Current);
    }

    public void SetOverlay(OverlayState overlay)
    {
        if (overlay == Overlay) return;
        var prev = Overlay;
        Overlay = overlay;
        OnOverlayChanged?.Invoke(prev, Overlay);
    }

    public void ClearOverlay() => SetOverlay(OverlayState.None);
}
