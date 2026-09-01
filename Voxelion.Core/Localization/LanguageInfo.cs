namespace Voxelion.Core.Localization;

/// <summary>
/// Canonical language metadata — display labels are localization keys, not hard-coded layouts.
/// </summary>
public readonly struct LanguageInfo
{
    public Language Id { get; }
    public string Code { get; }
    public string NativeName { get; }
    public string NameKey { get; }

    public LanguageInfo(Language id, string code, string nativeName, string nameKey)
    {
        Id = id;
        Code = code;
        NativeName = nativeName;
        NameKey = nameKey;
    }

    public static readonly LanguageInfo[] All =
    {
        new(Language.English, "EN", "English", "lang.name.en"),
        new(Language.BahasaIndonesia, "ID", "Bahasa Indonesia", "lang.name.id"),
        new(Language.Japanese, "JA", "日本語", "lang.name.ja"),
        new(Language.Chinese, "ZH", "中文", "lang.name.zh"),
        new(Language.Korean, "KO", "한국어", "lang.name.ko"),
    };

    public static LanguageInfo Get(Language lang)
    {
        foreach (var i in All)
            if (i.Id == lang) return i;
        return All[0];
    }

    public static int IndexOf(Language lang)
    {
        for (int i = 0; i < All.Length; i++)
            if (All[i].Id == lang) return i;
        return 0;
    }
}
