using System.Collections.Concurrent;
using System.Text.Json;

namespace Voxelion.Core.Localization;

public enum Language
{
    English,
    BahasaIndonesia,
    Japanese,
    Chinese,
    Korean
}

public sealed class LocalizationManager
{
    private readonly ConcurrentDictionary<string, Dictionary<string, string>> _tables = new();
    private Language _current = Language.English;

    public Language Current
    {
        get => _current;
        set
        {
            if (_current == value) return;
            _current = value;
            OnLanguageChanged?.Invoke(value);
        }
    }

    public event Action<Language>? OnLanguageChanged;

    public LocalizationManager()
    {
        LoadBuiltIn();
    }

    private void LoadBuiltIn()
    {
        // English
        var en = new Dictionary<string, string>
        {
            ["app.name"] = "VOXELION",
            ["app.tagline"] = "ENTER THE FRONTIER",
            ["boot.preparing"] = "Preparing your journey...",
            ["boot.initializing"] = "Initializing world interface",
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
            ["auth.email"] = "Email",
            ["auth.password"] = "Password",
            ["auth.confirm"] = "Confirm Password",
            ["auth.authenticating"] = "Authenticating...",
            ["auth.success"] = "Welcome back",
            ["auth.failed"] = "Authentication failed. Please try again.",
            ["auth.network"] = "Network unavailable. Check connection.",
            ["register.step1"] = "ACCOUNT",
            ["register.step2"] = "IDENTITY",
            ["register.step3"] = "AVATAR",
            ["register.step4"] = "READY",
            ["char.create"] = "CREATE YOUR CHARACTER",
            ["char.next"] = "NEXT",
            ["char.back"] = "BACK",
            ["char.randomize"] = "RANDOMIZE",
            ["char.body"] = "Body",
            ["char.hair"] = "Hair",
            ["char.face"] = "Face",
            ["char.eyes"] = "Eyes",
            ["char.outfit"] = "Outfit",
            ["char.accessories"] = "Accessories",
            ["identity.choose"] = "CHOOSE YOUR NAME",
            ["identity.checking"] = "Checking...",
            ["identity.confirm"] = "CONFIRM IDENTITY",
            ["identity.available"] = "Name available",
            ["identity.taken"] = "Name already taken",
            ["identity.invalid"] = "Invalid characters",
            ["identity.short"] = "Name too short",
            ["identity.long"] = "Name too long",
            ["welcome.ready"] = "YOU ARE READY",
            ["welcome.enter"] = "ENTER",
            ["welcome.to"] = "Welcome to VOXELION",
            ["hub.world"] = "WORLD",
            ["hub.inventory"] = "INVENTORY",
            ["hub.social"] = "SOCIAL",
            ["hub.discover"] = "DISCOVER",
            ["hub.menu"] = "MENU",
            ["hub.friends"] = "FRIENDS",
            ["hub.mail"] = "MAIL",
            ["discover.title"] = "DISCOVER",
            ["discover.search"] = "Search world / ID",
            ["discover.recommended"] = "RECOMMENDED",
            ["discover.trending"] = "TRENDING",
            ["discover.new"] = "NEW",
            ["discover.friends"] = "FRIENDS",
            ["discover.enter"] = "ENTER",
            ["discover.favorite"] = "FAVORITE",
            ["connect.connecting"] = "Connecting...",
            ["connect.auth"] = "Authenticating...",
            ["connect.region"] = "Loading region...",
            ["connect.sync"] = "Syncing player...",
            ["connect.spawn"] = "Spawning...",
            ["connect.retry"] = "RETRY",
            ["connect.cancel"] = "CANCEL",
            ["connect.return"] = "RETURN TO HUB",
            ["connect.lost"] = "CONNECTION LOST",
            ["error.connection_lost"] = "CONNECTION LOST",
            ["error.retry"] = "RETRY",
            ["error.return_hub"] = "RETURN TO HUB",
            ["connect.unavailable"] = "SERVER UNAVAILABLE",
            ["world.entering"] = "Entering the world...",
            ["hud.inventory"] = "INVENTORY",
            ["hud.character"] = "CHARACTER",
            ["hud.worlds"] = "WORLDS",
            ["hud.social"] = "SOCIAL",
            ["hud.settings"] = "SETTINGS",
            ["hud.help"] = "HELP",
            ["pause.resume"] = "RESUME",
            ["pause.exit"] = "EXIT WORLD",
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
            ["common.back"] = "BACK",
            ["common.next"] = "NEXT",
            ["common.confirm"] = "CONFIRM",
            ["common.cancel"] = "CANCEL",
            ["common.retry"] = "RETRY",
            ["common.close"] = "CLOSE",
            ["common.loading"] = "Loading...",
            ["common.error"] = "Something went wrong",
            ["guest.explain"] = "Play instantly. Progress can be linked to an account later.",
            ["guest.local"] = "Local session active",
            ["guest.limit"] = "Some features require a full account",
            ["tutorial.look"] = "Look around",
            ["tutorial.move"] = "Move with the pad or WASD",
            ["tutorial.jump"] = "Jump",
            ["tutorial.interact"] = "Interact",
            ["tutorial.inventory"] = "Open Inventory",
            ["tutorial.slot"] = "Use Quick Slot",
            ["tutorial.menu"] = "Open World Menu",
            ["social.friends"] = "Friends",
            ["social.party"] = "Party",
            ["social.nearby"] = "Nearby",
            ["social.invitations"] = "Invitations",
            ["social.messages"] = "Messages",
            ["social.blocked"] = "Blocked",
            ["presence.online"] = "ONLINE",
            ["presence.inworld"] = "IN WORLD",
            ["presence.inhub"] = "IN HUB",
            ["presence.away"] = "AWAY",
            ["presence.offline"] = "OFFLINE"
        };
        _tables["en"] = en;

        // Bahasa Indonesia
        var id = new Dictionary<string, string>(en)
        {
            ["app.tagline"] = "MASUKI BATAS BARU",
            ["boot.preparing"] = "Menyiapkan perjalananmu...",
            ["boot.initializing"] = "Menginisialisasi antarmuka dunia",
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
            ["welcome.to"] = "Selamat datang di VOXELION",
            ["hub.world"] = "DUNIA",
            ["hub.inventory"] = "INVENTARIS",
            ["hub.social"] = "SOSIAL",
            ["hub.discover"] = "JELAJAH",
            ["hub.menu"] = "MENU",
            ["discover.title"] = "JELAJAH",
            ["discover.enter"] = "MASUK",
            ["common.back"] = "KEMBALI",
            ["common.next"] = "LANJUT",
            ["common.confirm"] = "KONFIRMASI",
            ["common.cancel"] = "BATAL",
            ["common.retry"] = "COBA LAGI",
            ["guest.explain"] = "Main langsung. Progres bisa dihubungkan ke akun nanti.",
            ["pause.resume"] = "LANJUTKAN",
            ["pause.exit"] = "KELUAR DUNIA"
        };
        _tables["id"] = id;

        // Japanese
        var ja = new Dictionary<string, string>(en)
        {
            ["app.tagline"] = "フロンティアへ",
            ["boot.preparing"] = "旅の準備中...",
            ["title.play"] = "プレイ",
            ["title.account"] = "アカウント",
            ["title.settings"] = "設定",
            ["auth.guest"] = "ゲストで続ける",
            ["auth.signin"] = "サインイン",
            ["auth.create"] = "アカウント作成",
            ["char.create"] = "キャラクター作成",
            ["identity.choose"] = "名前を選ぶ",
            ["welcome.ready"] = "準備完了",
            ["welcome.enter"] = "入る",
            ["hub.world"] = "ワールド",
            ["hub.inventory"] = "インベントリ",
            ["hub.social"] = "ソーシャル",
            ["hub.discover"] = "発見",
            ["common.back"] = "戻る",
            ["common.next"] = "次へ",
            ["common.confirm"] = "確認",
            ["common.cancel"] = "キャンセル"
        };
        _tables["ja"] = ja;

        // Chinese
        var zh = new Dictionary<string, string>(en)
        {
            ["app.tagline"] = "进入边疆",
            ["boot.preparing"] = "正在准备你的旅程...",
            ["title.play"] = "开始",
            ["title.account"] = "账户",
            ["title.settings"] = "设置",
            ["auth.guest"] = "以游客继续",
            ["auth.signin"] = "登录",
            ["auth.create"] = "创建账户",
            ["char.create"] = "创建角色",
            ["identity.choose"] = "选择你的名字",
            ["welcome.ready"] = "准备就绪",
            ["welcome.enter"] = "进入",
            ["hub.world"] = "世界",
            ["hub.inventory"] = "背包",
            ["hub.social"] = "社交",
            ["hub.discover"] = "发现",
            ["common.back"] = "返回",
            ["common.next"] = "下一步",
            ["common.confirm"] = "确认",
            ["common.cancel"] = "取消"
        };
        _tables["zh"] = zh;

        // Korean
        var ko = new Dictionary<string, string>(en)
        {
            ["app.tagline"] = "프론티어로 진입",
            ["boot.preparing"] = "여정을 준비 중...",
            ["title.play"] = "플레이",
            ["title.account"] = "계정",
            ["title.settings"] = "설정",
            ["auth.guest"] = "게스트로 계속",
            ["auth.signin"] = "로그인",
            ["auth.create"] = "계정 만들기",
            ["char.create"] = "캐릭터 생성",
            ["identity.choose"] = "이름 선택",
            ["welcome.ready"] = "준비 완료",
            ["welcome.enter"] = "입장",
            ["hub.world"] = "월드",
            ["hub.inventory"] = "인벤토리",
            ["hub.social"] = "소셜",
            ["hub.discover"] = "발견",
            ["common.back"] = "뒤로",
            ["common.next"] = "다음",
            ["common.confirm"] = "확인",
            ["common.cancel"] = "취소"
        };
        _tables["ko"] = ko;
    }

    public string Get(string key)
    {
        string code = _current switch
        {
            Language.BahasaIndonesia => "id",
            Language.Japanese => "ja",
            Language.Chinese => "zh",
            Language.Korean => "ko",
            _ => "en"
        };
        if (_tables.TryGetValue(code, out var table) && table.TryGetValue(key, out var val))
            return val;
        if (_tables["en"].TryGetValue(key, out var fallback))
            return fallback;
        return key;
    }

    public string this[string key] => Get(key);
}
