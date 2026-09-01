using System.Text;
using Voxelion.Core.Localization;

namespace Voxelion.Core.Data;

/// <summary>
/// Free local persistence (no cloud keys). File under app storage / current directory.
/// </summary>
public sealed class UserSettingsStore
{
    private readonly string _path;
    public Language Language { get; set; } = Language.English;
    public float UiScale { get; set; } = 1f;
    public bool ReduceMotion { get; set; }
    public bool HighReadability { get; set; }

    public UserSettingsStore(string? path = null)
    {
        _path = path ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "VOXELION", "user.settings");
        Load();
    }

    public void Load()
    {
        try
        {
            var dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (!File.Exists(_path)) return;
            foreach (var line in File.ReadAllLines(_path))
            {
                var p = line.Split('=', 2);
                if (p.Length != 2) continue;
                var k = p[0].Trim();
                var v = p[1].Trim();
                switch (k)
                {
                    case "language":
                        if (Enum.TryParse<Language>(v, true, out var lang)) Language = lang;
                        break;
                    case "ui_scale":
                        if (float.TryParse(v, out var s)) UiScale = s;
                        break;
                    case "reduce_motion":
                        ReduceMotion = v is "1" or "true";
                        break;
                    case "high_readability":
                        HighReadability = v is "1" or "true";
                        break;
                }
            }
        }
        catch
        {
            // keep defaults
        }
    }

    public void Save()
    {
        try
        {
            var dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var sb = new StringBuilder();
            sb.AppendLine("language=" + Language);
            sb.AppendLine("ui_scale=" + UiScale.ToString("0.##"));
            sb.AppendLine("reduce_motion=" + (ReduceMotion ? "1" : "0"));
            sb.AppendLine("high_readability=" + (HighReadability ? "1" : "0"));
            File.WriteAllText(_path, sb.ToString());
        }
        catch
        {
            // ignore write failures on restricted sandboxes
        }
    }
}
