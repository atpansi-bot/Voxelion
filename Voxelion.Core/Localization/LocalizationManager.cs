using System.Collections.Concurrent;

namespace Voxelion.Core.Localization;

public enum Language
{
    English,
    BahasaIndonesia,
    Japanese,
    Chinese,
    Korean
}

/// <summary>
/// All UI copy lives in keys. Scenes must call T("key") — never hard-code English in Draw.
/// JA/ZH/KO entries use native script; PixelFont shows placeholders until SpriteFont ships.
/// </summary>
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

    public LocalizationManager() => LoadBuiltIn();

    public string T(string key)
    {
        var table = TableFor(_current);
        if (table.TryGetValue(key, out var v) && !string.IsNullOrEmpty(v)) return v;
        if (_tables.TryGetValue("en", out var en) && en.TryGetValue(key, out var fb)) return fb;
        return key;
    }

    public string T(string key, params object[] args)
    {
        try { return string.Format(T(key), args); }
        catch { return T(key); }
    }

    private Dictionary<string, string> TableFor(Language lang) => lang switch
    {
        Language.BahasaIndonesia => _tables["id"],
        Language.Japanese => _tables["ja"],
        Language.Chinese => _tables["zh"],
        Language.Korean => _tables["ko"],
        _ => _tables["en"]
    };

    private void LoadBuiltIn()
    {
        _tables["en"] = En();
        _tables["id"] = Id();
        _tables["ja"] = Ja();
        _tables["zh"] = Zh();
        _tables["ko"] = Ko();
    }

    private static Dictionary<string, string> En() => new()
    {
        ["app.name"] = "VOXELION",
        ["app.tagline"] = "ENTER THE FRONTIER",
        ["app.version"] = "V1.0.0",
        ["boot.phase.graphics"] = "GRAPHICS",
        ["boot.phase.input"] = "INPUT",
        ["boot.phase.audio"] = "AUDIO",
        ["boot.phase.fonts"] = "FONTS",
        ["boot.phase.localization"] = "LOCALIZATION",
        ["boot.phase.ui"] = "UI THEME",
        ["boot.phase.assets"] = "ASSETS",
        ["boot.phase.session"] = "SESSION",
        ["boot.phase.network"] = "NETWORK",
        ["splash.continue"] = "TAP TO CONTINUE",
        ["title.play"] = "PLAY",
        ["title.account"] = "ACCOUNT",
        ["title.settings"] = "SETTINGS",
        ["title.credits"] = "CREDITS",
        ["auth.guest"] = "CONTINUE AS GUEST",
        ["auth.signin"] = "SIGN IN",
        ["auth.create"] = "CREATE ACCOUNT",
        ["auth.email"] = "EMAIL",
        ["auth.password"] = "PASSWORD",
        ["auth.back"] = "BACK",
        ["char.title"] = "CREATE CHARACTER",
        ["char.next"] = "NEXT",
        ["char.back"] = "BACK",
        ["char.randomize"] = "RANDOMIZE",
        ["identity.title"] = "IDENTITY",
        ["identity.confirm"] = "CONFIRM",
        ["welcome.title"] = "WELCOME",
        ["welcome.continue"] = "ENTER HUB",
        ["hub.discover"] = "DISCOVER",
        ["hub.inventory"] = "INVENTORY",
        ["hub.social"] = "SOCIAL",
        ["hub.settings"] = "SETTINGS",
        ["hub.profile"] = "PROFILE",
        ["discover.title"] = "WORLD DISCOVERY",
        ["discover.join"] = "JOIN",
        ["discover.back"] = "BACK",
        ["connect.title"] = "CONNECTING",
        ["worldloading.title"] = "ENTERING WORLD",
        ["world.hud.hint"] = "EXPLORE",
        ["inventory.title"] = "INVENTORY",
        ["settings.title"] = "SETTINGS",
        ["settings.back"] = "BACK",
        ["social.title"] = "SOCIAL",
        ["pause.title"] = "PAUSED",
        ["pause.resume"] = "RESUME",
        ["pause.quit"] = "QUIT TO HUB",
        ["common.ok"] = "OK",
        ["common.cancel"] = "CANCEL",
        ["common.confirm"] = "CONFIRM",
        ["common.loading"] = "LOADING",
        ["common.error"] = "ERROR",
        ["common.search"] = "SEARCH",
        ["lang.changed"] = "LANGUAGE {0}",
    };

    private static Dictionary<string, string> Id() => new()
    {
        ["app.name"] = "VOXELION",
        ["app.tagline"] = "MASUKI PERBATASAN",
        ["app.version"] = "V1.0.0",
        ["boot.phase.graphics"] = "GRAFIS",
        ["boot.phase.input"] = "INPUT",
        ["boot.phase.audio"] = "AUDIO",
        ["boot.phase.fonts"] = "FONT",
        ["boot.phase.localization"] = "BAHASA",
        ["boot.phase.ui"] = "TEMA UI",
        ["boot.phase.assets"] = "ASET",
        ["boot.phase.session"] = "SESI",
        ["boot.phase.network"] = "JARINGAN",
        ["splash.continue"] = "KETUK UNTUK LANJUT",
        ["title.play"] = "MAIN",
        ["title.account"] = "AKUN",
        ["title.settings"] = "PENGATURAN",
        ["title.credits"] = "KREDIT",
        ["auth.guest"] = "LANJUT SEBAGAI TAMU",
        ["auth.signin"] = "MASUK",
        ["auth.create"] = "BUAT AKUN",
        ["auth.email"] = "EMAIL",
        ["auth.password"] = "KATA SANDI",
        ["auth.back"] = "KEMBALI",
        ["char.title"] = "BUAT KARAKTER",
        ["char.next"] = "LANJUT",
        ["char.back"] = "KEMBALI",
        ["char.randomize"] = "ACAK",
        ["identity.title"] = "IDENTITAS",
        ["identity.confirm"] = "KONFIRMASI",
        ["welcome.title"] = "SELAMAT DATANG",
        ["welcome.continue"] = "MASUK HUB",
        ["hub.discover"] = "JELAJAHI",
        ["hub.inventory"] = "INVENTARIS",
        ["hub.social"] = "SOSIAL",
        ["hub.settings"] = "PENGATURAN",
        ["hub.profile"] = "PROFIL",
        ["discover.title"] = "TEMUKAN DUNIA",
        ["discover.join"] = "GABUNG",
        ["discover.back"] = "KEMBALI",
        ["connect.title"] = "MENGHUBUNGKAN",
        ["worldloading.title"] = "MEMASUKI DUNIA",
        ["world.hud.hint"] = "JELAJAHI",
        ["inventory.title"] = "INVENTARIS",
        ["settings.title"] = "PENGATURAN",
        ["settings.back"] = "KEMBALI",
        ["social.title"] = "SOSIAL",
        ["pause.title"] = "JEDA",
        ["pause.resume"] = "LANJUTKAN",
        ["pause.quit"] = "KELUAR KE HUB",
        ["common.ok"] = "OK",
        ["common.cancel"] = "BATAL",
        ["common.confirm"] = "KONFIRMASI",
        ["common.loading"] = "MEMUAT",
        ["common.error"] = "ERROR",
        ["common.search"] = "CARI",
        ["lang.changed"] = "BAHASA {0}",
    };

    private static Dictionary<string, string> Ja() => new()
    {
        ["app.name"] = "VOXELION",
        ["app.tagline"] = "フロンティアへ",
        ["app.version"] = "V1.0.0",
        ["boot.phase.graphics"] = "グラフィック",
        ["boot.phase.input"] = "入力",
        ["boot.phase.audio"] = "オーディオ",
        ["boot.phase.fonts"] = "フォント",
        ["boot.phase.localization"] = "言語",
        ["boot.phase.ui"] = "UIテーマ",
        ["boot.phase.assets"] = "アセット",
        ["boot.phase.session"] = "セッション",
        ["boot.phase.network"] = "ネットワーク",
        ["splash.continue"] = "タップして続行",
        ["title.play"] = "プレイ",
        ["title.account"] = "アカウント",
        ["title.settings"] = "設定",
        ["title.credits"] = "クレジット",
        ["auth.guest"] = "ゲストで続ける",
        ["auth.signin"] = "サインイン",
        ["auth.create"] = "アカウント作成",
        ["auth.email"] = "メール",
        ["auth.password"] = "パスワード",
        ["auth.back"] = "戻る",
        ["char.title"] = "キャラクター作成",
        ["char.next"] = "次へ",
        ["char.back"] = "戻る",
        ["char.randomize"] = "ランダム",
        ["identity.title"] = "アイデンティティ",
        ["identity.confirm"] = "確認",
        ["welcome.title"] = "ようこそ",
        ["welcome.continue"] = "ハブへ",
        ["hub.discover"] = "探索",
        ["hub.inventory"] = "インベントリ",
        ["hub.social"] = "ソーシャル",
        ["hub.settings"] = "設定",
        ["hub.profile"] = "プロフィール",
        ["discover.title"] = "ワールド探索",
        ["discover.join"] = "参加",
        ["discover.back"] = "戻る",
        ["connect.title"] = "接続中",
        ["worldloading.title"] = "ワールド入場",
        ["world.hud.hint"] = "探索",
        ["inventory.title"] = "インベントリ",
        ["settings.title"] = "設定",
        ["settings.back"] = "戻る",
        ["social.title"] = "ソーシャル",
        ["pause.title"] = "一時停止",
        ["pause.resume"] = "再開",
        ["pause.quit"] = "ハブへ戻る",
        ["common.ok"] = "OK",
        ["common.cancel"] = "キャンセル",
        ["common.confirm"] = "確認",
        ["common.loading"] = "読み込み中",
        ["common.error"] = "エラー",
        ["common.search"] = "検索",
        ["lang.changed"] = "言語 {0}",
    };

    private static Dictionary<string, string> Zh() => new()
    {
        ["app.name"] = "VOXELION",
        ["app.tagline"] = "进入边疆",
        ["app.version"] = "V1.0.0",
        ["boot.phase.graphics"] = "图形",
        ["boot.phase.input"] = "输入",
        ["boot.phase.audio"] = "音频",
        ["boot.phase.fonts"] = "字体",
        ["boot.phase.localization"] = "语言",
        ["boot.phase.ui"] = "界面主题",
        ["boot.phase.assets"] = "资源",
        ["boot.phase.session"] = "会话",
        ["boot.phase.network"] = "网络",
        ["splash.continue"] = "点击继续",
        ["title.play"] = "开始",
        ["title.account"] = "账户",
        ["title.settings"] = "设置",
        ["title.credits"] = "制作人员",
        ["auth.guest"] = "游客继续",
        ["auth.signin"] = "登录",
        ["auth.create"] = "创建账户",
        ["auth.email"] = "邮箱",
        ["auth.password"] = "密码",
        ["auth.back"] = "返回",
        ["char.title"] = "创建角色",
        ["char.next"] = "下一步",
        ["char.back"] = "返回",
        ["char.randomize"] = "随机",
        ["identity.title"] = "身份",
        ["identity.confirm"] = "确认",
        ["welcome.title"] = "欢迎",
        ["welcome.continue"] = "进入枢纽",
        ["hub.discover"] = "发现",
        ["hub.inventory"] = "背包",
        ["hub.social"] = "社交",
        ["hub.settings"] = "设置",
        ["hub.profile"] = "资料",
        ["discover.title"] = "世界发现",
        ["discover.join"] = "加入",
        ["discover.back"] = "返回",
        ["connect.title"] = "连接中",
        ["worldloading.title"] = "进入世界",
        ["world.hud.hint"] = "探索",
        ["inventory.title"] = "背包",
        ["settings.title"] = "设置",
        ["settings.back"] = "返回",
        ["social.title"] = "社交",
        ["pause.title"] = "已暂停",
        ["pause.resume"] = "继续",
        ["pause.quit"] = "返回枢纽",
        ["common.ok"] = "确定",
        ["common.cancel"] = "取消",
        ["common.confirm"] = "确认",
        ["common.loading"] = "加载中",
        ["common.error"] = "错误",
        ["common.search"] = "搜索",
        ["lang.changed"] = "语言 {0}",
    };

    private static Dictionary<string, string> Ko() => new()
    {
        ["app.name"] = "VOXELION",
        ["app.tagline"] = "프론티어로",
        ["app.version"] = "V1.0.0",
        ["boot.phase.graphics"] = "그래픽",
        ["boot.phase.input"] = "입력",
        ["boot.phase.audio"] = "오디오",
        ["boot.phase.fonts"] = "폰트",
        ["boot.phase.localization"] = "언어",
        ["boot.phase.ui"] = "UI 테마",
        ["boot.phase.assets"] = "에셋",
        ["boot.phase.session"] = "세션",
        ["boot.phase.network"] = "네트워크",
        ["splash.continue"] = "탭하여 계속",
        ["title.play"] = "플레이",
        ["title.account"] = "계정",
        ["title.settings"] = "설정",
        ["title.credits"] = "크레딧",
        ["auth.guest"] = "게스트로 계속",
        ["auth.signin"] = "로그인",
        ["auth.create"] = "계정 만들기",
        ["auth.email"] = "이메일",
        ["auth.password"] = "비밀번호",
        ["auth.back"] = "뒤로",
        ["char.title"] = "캐릭터 생성",
        ["char.next"] = "다음",
        ["char.back"] = "뒤로",
        ["char.randomize"] = "랜덤",
        ["identity.title"] = "신원",
        ["identity.confirm"] = "확인",
        ["welcome.title"] = "환영합니다",
        ["welcome.continue"] = "허브 입장",
        ["hub.discover"] = "탐험",
        ["hub.inventory"] = "인벤토리",
        ["hub.social"] = "소셜",
        ["hub.settings"] = "설정",
        ["hub.profile"] = "프로필",
        ["discover.title"] = "월드 탐색",
        ["discover.join"] = "참가",
        ["discover.back"] = "뒤로",
        ["connect.title"] = "연결 중",
        ["worldloading.title"] = "월드 입장",
        ["world.hud.hint"] = "탐험",
        ["inventory.title"] = "인벤토리",
        ["settings.title"] = "설정",
        ["settings.back"] = "뒤로",
        ["social.title"] = "소셜",
        ["pause.title"] = "일시정지",
        ["pause.resume"] = "계속",
        ["pause.quit"] = "허브로",
        ["common.ok"] = "확인",
        ["common.cancel"] = "취소",
        ["common.confirm"] = "확인",
        ["common.loading"] = "로딩",
        ["common.error"] = "오류",
        ["common.search"] = "검색",
        ["lang.changed"] = "언어 {0}",
    };
}
