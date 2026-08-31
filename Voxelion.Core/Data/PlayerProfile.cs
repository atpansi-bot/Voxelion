using Microsoft.Xna.Framework;

namespace Voxelion.Core.Data;

public sealed class PlayerProfile
{
    public string PlayerId { get; set; } = Guid.NewGuid().ToString("N");
    public string DisplayName { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public bool IsGuest { get; set; }
    public bool HasCharacter { get; set; }
    public CharacterAppearance Appearance { get; set; } = new();
    public string? LastWorldId { get; set; }
    public string? LastWorldName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastLogin { get; set; } = DateTime.UtcNow;
}

public sealed class CharacterAppearance
{
    public int BodyIndex { get; set; }
    public int HairIndex { get; set; }
    public int FaceIndex { get; set; }
    public int EyesIndex { get; set; }
    public int OutfitIndex { get; set; }
    public int AccessoryIndex { get; set; }
    public Color PrimaryColor { get; set; } = new(148, 92, 255);
    public Color SecondaryColor { get; set; } = new(72, 196, 255);
}

public sealed class WorldInfo
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string Name { get; set; } = "Unnamed World";
    public string Creator { get; set; } = "Unknown";
    public int PlayerCount { get; set; }
    public string Category { get; set; } = "Adventure";
    public string[] Tags { get; set; } = Array.Empty<string>();
    public string Description { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }
    public float ConnectionQuality { get; set; } = 1f;
}

public sealed class SessionService
{
    public bool HasValidSession { get; private set; }
    public bool IsNetworkAvailable { get; private set; } = true;
    public string? SessionToken { get; private set; }

    public event Action? OnSessionChanged;

    public void Evaluate()
    {
        // Pure local evaluation for offline-first prototype
        // In production this would hit free MongoDB Atlas or local store
        HasValidSession = !string.IsNullOrEmpty(SessionToken);
        OnSessionChanged?.Invoke();
    }

    public void CreateGuestSession(PlayerProfile profile)
    {
        profile.IsGuest = true;
        profile.DisplayName = string.IsNullOrEmpty(profile.DisplayName) ? $"Wanderer_{Random.Shared.Next(1000, 9999)}" : profile.DisplayName;
        SessionToken = "guest_" + Guid.NewGuid().ToString("N");
        HasValidSession = true;
        OnSessionChanged?.Invoke();
    }

    public void CreateAccountSession(PlayerProfile profile, string email)
    {
        profile.IsGuest = false;
        SessionToken = "acct_" + Guid.NewGuid().ToString("N");
        HasValidSession = true;
        OnSessionChanged?.Invoke();
    }

    public void ClearSession()
    {
        SessionToken = null;
        HasValidSession = false;
        OnSessionChanged?.Invoke();
    }

    public void SetNetwork(bool available) => IsNetworkAvailable = available;
}
