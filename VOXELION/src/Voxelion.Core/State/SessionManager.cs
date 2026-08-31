using System;
using System.IO;
using System.Text.Json;

namespace Voxelion.Core.State
{
    public class PlayerProfile
    {
        public string PlayerId { get; set; } = Guid.NewGuid().ToString("N");
        public string DisplayName { get; set; } = "";
        public string AvatarId { get; set; } = "default";
        public int Level { get; set; } = 1;
        public bool IsGuest { get; set; }
        public string LastWorldId { get; set; } = "";
        public string LastWorldName { get; set; } = "";
        public DateTime LastPlayed { get; set; } = DateTime.UtcNow;
        public string Language { get; set; } = "en";
    }

    public class SessionManager
    {
        private const string SessionFile = "voxelion_session.json";
        public PlayerProfile? Current { get; private set; }
        public SessionState State { get; private set; } = SessionState.Unknown;
        public bool IsOnline { get; set; } = true;

        public void LoadLocalSession()
        {
            try
            {
                if (File.Exists(SessionFile))
                {
                    var json = File.ReadAllText(SessionFile);
                    Current = JsonSerializer.Deserialize<PlayerProfile>(json);
                    if (Current != null && !string.IsNullOrEmpty(Current.DisplayName))
                    {
                        State = SessionState.Valid;
                        return;
                    }
                }
            }
            catch { /* corrupted session */ }
            Current = null;
            State = SessionState.None;
        }

        public void SaveSession()
        {
            if (Current == null) return;
            try
            {
                var json = JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SessionFile, json);
                State = SessionState.Valid;
            }
            catch { /* disk full / permission */ }
        }

        public void CreateGuest()
        {
            Current = new PlayerProfile
            {
                IsGuest = true,
                DisplayName = "Traveler_" + Random.Shared.Next(1000, 9999)
            };
            State = SessionState.Valid;
            SaveSession();
        }

        public void SetIdentity(string name, string avatarId)
        {
            if (Current == null) CreateGuest();
            Current!.DisplayName = name;
            Current.AvatarId = avatarId;
            SaveSession();
        }

        public void ClearSession()
        {
            Current = null;
            State = SessionState.None;
            if (File.Exists(SessionFile)) File.Delete(SessionFile);
        }

        public void MarkExpired() => State = SessionState.Expired;
        public void MarkNetworkUnavailable() => State = SessionState.NetworkUnavailable;
    }
}
