using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework.Content;

namespace Voxelion.Core.Localization
{
    /// <summary>
    /// Localization-ready text system.
    /// Supported: en, id, ja, zh, ko.
    /// Never hard-code strings in screen logic.
    /// </summary>
    public class LocalizationManager
    {
        private Dictionary<string, Dictionary<string, string>> _tables = new();
        private string _current = "en";

        public string CurrentLanguage => _current;
        public event Action OnLanguageChanged;

        public void Initialize(ContentManager content)
        {
            // Built-in fallback tables so the binary is self-contained
            RegisterBuiltIn();
            // Optional: load JSON from Content/Localization/{lang}.json at runtime
            TryLoadExternal("en");
            TryLoadExternal("id");
            TryLoadExternal("ja");
            TryLoadExternal("zh");
            TryLoadExternal("ko");
        }

        private void RegisterBuiltIn()
        {
            var en = new Dictionary<string, string>
            {
                ["app.name"] = "VOXELION",
                ["title.enter"] = "ENTER THE FRONTIER",
                ["title.play"] = "PLAY",
                ["title.account"] = "ACCOUNT",
                ["title.settings"] = "SETTINGS",
                ["title.credits"] = "CREDITS",
                ["title.continue"] = "CONTINUE YOUR JOURNEY",
                ["title.continue_btn"] = "CONTINUE",
                ["title.switch"] = "SWITCH ACCOUNT",
                ["auth.guest"] = "CONTINUE AS GUEST",
                ["auth.signin"] = "SIGN IN",
                ["auth.create"] = "CREATE ACCOUNT",
                ["auth.authenticating"] = "Authenticating...",
                ["char.create"] = "CREATE YOUR CHARACTER",
                ["char.next"] = "NEXT",
                ["char.back"] = "BACK",
                ["char.randomize"] = "RANDOMIZE",
                ["identity.choose"] = "CHOOSE YOUR NAME",
                ["identity.confirm"] = "CONFIRM IDENTITY",
                ["identity.checking"] = "Checking...",
                ["welcome.ready"] = "YOU ARE READY",
                ["welcome.enter"] = "ENTER",
                ["hub.world"] = "WORLD",
                ["hub.inventory"] = "INVENTORY",
                ["hub.social"] = "SOCIAL",
                ["hub.discover"] = "DISCOVER",
                ["hub.menu"] = "MENU",
                ["discover.title"] = "DISCOVER",
                ["discover.search"] = "SEARCH WORLD / ID",
                ["discover.enter"] = "ENTER",
                ["discover.favorite"] = "FAVORITE",
                ["connect.connecting"] = "Connecting...",
                ["connect.auth"] = "Authenticating...",
                ["connect.region"] = "Loading region...",
                ["connect.sync"] = "Syncing player...",
                ["connect.spawn"] = "Spawning...",
                ["loading.preparing"] = "Preparing your journey...",
                ["loading.init"] = "Initializing world interface",
                ["error.retry"] = "RETRY",
                ["error.return_hub"] = "RETURN TO HUB",
                ["error.connection_lost"] = "CONNECTION LOST",
                ["error.session_expired"] = "SESSION EXPIRED",
                ["common.back"] = "BACK",
                ["common.confirm"] = "CONFIRM",
                ["common.cancel"] = "CANCEL",
                ["common.close"] = "CLOSE",
                ["settings.graphics"] = "Graphics",
                ["settings.audio"] = "Audio",
                ["settings.controls"] = "Controls",
                ["settings.interface"] = "Interface",
                ["settings.accessibility"] = "Accessibility",
                ["settings.language"] = "Language",
                ["settings.notifications"] = "Notifications",
                ["settings.network"] = "Network",
                ["settings.privacy"] = "Privacy",
                ["settings.account"] = "Account",
                ["inventory.title"] = "INVENTORY",
                ["social.title"] = "SOCIAL",
                ["profile.title"] = "PROFILE",
                ["tutorial.look"] = "Look around",
                ["tutorial.move"] = "Move",
                ["tutorial.jump"] = "Jump",
                ["tutorial.interact"] = "Interact",
                ["tutorial.inventory"] = "Open Inventory",
                ["guest.explain"] = "Guest mode stores progress locally on this device. Link an account later to keep your journey across devices.",
                ["lang.en"] = "English",
                ["lang.id"] = "Bahasa Indonesia",
                ["lang.ja"] = "日本語",
                ["lang.zh"] = "中文",
                ["lang.ko"] = "한국어"
            };
            _tables["en"] = en;

            // Indonesian
            var id = new Dictionary<string, string>(en)
            {
                ["title.enter"] = "MASUKI PERBATASAN",
                ["title.play"] = "MAIN",
                ["title.account"] = "AKUN",
                ["title.settings"] = "PENGATURAN",
                ["title.credits"] = "KREDIT",
                ["title.continue"] = "LANJUTKAN PERJALANANMU",
                ["title.continue_btn"] = "LANJUTKAN",
                ["title.switch"] = "GANTI AKUN",
                ["auth.guest"] = "LANJUT SEBAGAI TAMU",
                ["auth.signin"] = "MASUK",
                ["auth.create"] = "BUAT AKUN",
                ["char.create"] = "BUAT KARAKTERMU",
                ["identity.choose"] = "PILIH NAMAMU",
                ["identity.confirm"] = "KONFIRMASI IDENTITAS",
                ["welcome.ready"] = "KAMU SIAP",
                ["welcome.enter"] = "MASUK",
                ["hub.world"] = "DUNIA",
                ["hub.inventory"] = "INVENTARIS",
                ["hub.social"] = "SOSIAL",
                ["hub.discover"] = "TEMUKAN",
                ["hub.menu"] = "MENU",
                ["discover.title"] = "TEMUKAN",
                ["loading.preparing"] = "Menyiapkan perjalananmu...",
                ["error.retry"] = "COBA LAGI",
                ["error.return_hub"] = "KEMBALI KE HUB",
                ["common.back"] = "KEMBALI",
                ["common.confirm"] = "KONFIRMASI",
                ["common.cancel"] = "BATAL",
                ["guest.explain"] = "Mode tamu menyimpan progres di perangkat ini. Hubungkan akun nanti agar perjalananmu tersimpan di semua perangkat."
            };
            _tables["id"] = id;

            // Minimal stubs for ja/zh/ko (full tables can be expanded via JSON)
            _tables["ja"] = new Dictionary<string, string>(en) { ["title.enter"] = "フロンティアへ入れ", ["title.play"] = "プレイ" };
            _tables["zh"] = new Dictionary<string, string>(en) { ["title.enter"] = "进入边疆", ["title.play"] = "开始" };
            _tables["ko"] = new Dictionary<string, string>(en) { ["title.enter"] = "프론티어로 진입", ["title.play"] = "플레이" };
        }

        private void TryLoadExternal(string lang)
        {
            // Runtime JSON override path for production updates without rebuild
            string path = Path.Combine("Content", "Localization", $"{lang}.json");
            if (!File.Exists(path)) return;
            try
            {
                var json = File.ReadAllText(path);
                var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (dict != null)
                {
                    if (!_tables.ContainsKey(lang)) _tables[lang] = new();
                    foreach (var kv in dict) _tables[lang][kv.Key] = kv.Value;
                }
            }
            catch { /* ignore malformed external */ }
        }

        public void SetLanguage(string code)
        {
            if (!_tables.ContainsKey(code)) return;
            _current = code;
            OnLanguageChanged?.Invoke();
        }

        public string Get(string key)
        {
            if (_tables.TryGetValue(_current, out var table) && table.TryGetValue(key, out var val))
                return val;
            if (_tables.TryGetValue("en", out var en) && en.TryGetValue(key, out var fallback))
                return fallback;
            return key; // never crash, show key for missing translation
        }

        public string this[string key] => Get(key);
    }
}
