namespace Voxelion.Core.UI.Theme;

/// <summary>
/// Typography hierarchy — map roles to PixelFont scale multipliers.
/// Do not hard-code scales inside scenes; use TypeScale.* .
/// </summary>
public static class TypeScale
{
    public const float Display = 3.6f;
    public const float Title = 2.8f;
    public const float Section = 2.2f;
    public const float Body = 1.55f;
    public const float Label = 1.4f;
    public const float Caption = 1.2f;
    public const float Numeric = 1.7f;
    public const float System = 1.05f;

    // Align with DesignTokens.Typography aliases
    public static float Of(string role) => role.ToLowerInvariant() switch
    {
        "display" => Display,
        "title" => Title,
        "section" or "heading" => Section,
        "body" => Body,
        "label" or "button" => Label,
        "caption" or "micro" => Caption,
        "numeric" or "number" => Numeric,
        "system" or "diagnostic" or "debug" => System,
        _ => Body
    };
}
