namespace Voxelion.Core.UI;

/// <summary>
/// Canonical asset paths under Content/Textures/.
/// Replace placeholder PNGs with final art without renaming keys.
/// Runtime still uses procedural VisualChrome until ContentManager loads these.
/// </summary>
public static class AssetCatalog
{
    public const string Root = "Textures";

    public static class Logo
    {
        public const string Emblem256 = "Textures/Logo/emblem_256";
        public const string Emblem128 = "Textures/Logo/emblem_128";
        public const string Emblem64 = "Textures/Logo/emblem_64";
    }

    public static class Backgrounds
    {
        public const string Void = "Textures/Backgrounds/bg_void_960x540";
        public const string Night = "Textures/Backgrounds/bg_night_960x540";
        public const string Hub = "Textures/Backgrounds/bg_hub_960x540";
        public const string Cloud1 = "Textures/Backgrounds/cloud_layer_1";
        public const string Cloud2 = "Textures/Backgrounds/cloud_layer_2";
        public const string Cloud3 = "Textures/Backgrounds/cloud_layer_3";
    }

    public static class Particles
    {
        public const string Dust = "Textures/Particles/particle_dust_8";
        public const string Spark = "Textures/Particles/particle_spark_8";
        public const string Ember = "Textures/Particles/particle_ember_8";
    }

    public static class UI
    {
        public const string Panel9Slice = "Textures/UI/panel_9slice_64";
        public const string FrameCornerTL = "Textures/UI/frame_corner_tl";
        public const string FrameCornerTR = "Textures/UI/frame_corner_tr";
        public const string FrameCornerBL = "Textures/UI/frame_corner_bl";
        public const string FrameCornerBR = "Textures/UI/frame_corner_br";
        public const string TabIdle = "Textures/UI/tab_idle_128x40";
        public const string TabActive = "Textures/UI/tab_active_128x40";
        public const string LoadingBar = "Textures/UI/loading_bar_256x16";
        public const string LoadingSpinner = "Textures/UI/loading_spinner_64";
        public const string BadgeInfo = "Textures/UI/badge_info_24";
        public const string BadgeSuccess = "Textures/UI/badge_success_24";
        public const string BadgeWarning = "Textures/UI/badge_warning_24";
        public const string BadgeError = "Textures/UI/badge_error_24";

        public static string Button(string kind, string state) =>
            $"Textures/UI/button_{kind}_{state}_192x48";
    }

    public static class Icons
    {
        public static string Get(string name, int size = 64) =>
            $"Textures/Icons/icon_{name}_{size}";

        public const string Play = "Textures/Icons/icon_play_64";
        public const string Account = "Textures/Icons/icon_account_64";
        public const string Settings = "Textures/Icons/icon_settings_64";
        public const string World = "Textures/Icons/icon_world_64";
        public const string Bag = "Textures/Icons/icon_bag_64";
        public const string Social = "Textures/Icons/icon_social_64";
        public const string Chat = "Textures/Icons/icon_chat_64";
        public const string Back = "Textures/Icons/icon_back_64";
        public const string Forward = "Textures/Icons/icon_forward_64";
        public const string Close = "Textures/Icons/icon_close_64";
        public const string Confirm = "Textures/Icons/icon_confirm_64";
        public const string Menu = "Textures/Icons/icon_menu_64";
        public const string Star = "Textures/Icons/icon_star_64";
        public const string Lang = "Textures/Icons/icon_lang_64";
        public const string ArrowLeft = "Textures/Icons/arrow_left_48";
        public const string ArrowRight = "Textures/Icons/arrow_right_48";
    }

    public static class Worlds
    {
        public static string Thumb(string id) => $"Textures/Worlds/thumb_{id}_320x180";
    }

    public static class Avatar
    {
        public const string Frame64 = "Textures/Avatar/frame_64";
        public const string Frame128 = "Textures/Avatar/frame_128";
    }

    public static class Items
    {
        public static string Frame(string rarity) => $"Textures/Items/frame_{rarity}_64";
    }

    public static class Controls
    {
        public const string PadBase = "Textures/Controls/pad_base_128";
        public const string PadKnob = "Textures/Controls/pad_knob_64";
        public const string BtnJump = "Textures/Controls/btn_jump_64";
        public const string BtnAction = "Textures/Controls/btn_action_64";
        public const string BtnInteract = "Textures/Controls/btn_interact_64";
        public const string PromptTap = "Textures/Controls/prompt_tap_128x32";
        public const string PromptHold = "Textures/Controls/prompt_hold_128x32";
        public const string PromptKeyE = "Textures/Controls/prompt_key_e_128x32";
    }
}
