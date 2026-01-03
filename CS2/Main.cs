using AliceInCradle;
using BepInEx;
using UnityEngine;

namespace PeakDGLab
{
    [BepInPlugin("com.user.peak.dglab", "PEAK X DGLAB", "1.0.0")]
    [BepInProcess("PEAK.exe")]
    public class Main : BaseUnityPlugin
    {
        private ConfigManager _configManager;
        private GameComponentManager _gameComponentManager;
        private PlayerStatusController _playerStatusController;
        private DGLabApiClient _apiClient;
        private UIManager _uiManager;

        private bool _originalCursorVisibleState;
        private CursorLockMode _originalCursorLockState;

        public void Awake()
        {
            Logger.LogInfo("PEAK 郊狼联动插件正在加载...");

            _apiClient = new DGLabApiClient(Logger);
            _configManager = new ConfigManager(this.Config);
            _gameComponentManager = new GameComponentManager(Logger);
            _playerStatusController = new PlayerStatusController(_configManager, _apiClient, Logger);
            _uiManager = new UIManager(_configManager);

            Logger.LogInfo("PEAK 郊狼联动插件加载完成！按 F10 打开设置菜单，按 P 键切换电击开关。");
        }

        public void Start()
        {
            _gameComponentManager.CacheGameComponents();
        }

        public void Update()
        {
            // 1. 快捷键：电击总开关
            if (UnityEngine.Input.GetKeyDown(_configManager.ToggleShockKey.Value))
            {
                _configManager.EnableShock.Value = !_configManager.EnableShock.Value;
                string status = _configManager.EnableShock.Value ? "开启" : "关闭";
                Logger.LogInfo($"[快捷键] 电击总开关已切换为: {status}");
            }

            // 2. 快捷键：UI 菜单
            if (UnityEngine.Input.GetKeyDown(_configManager.ToggleUiKey.Value))
            {
                _uiManager.IsVisible = !_uiManager.IsVisible;

                if (_uiManager.IsVisible)
                {
                    _originalCursorVisibleState = Cursor.visible;
                    _originalCursorLockState = Cursor.lockState;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Cursor.visible = _originalCursorVisibleState;
                    Cursor.lockState = _originalCursorLockState;
                }
            }

            // 3. 始终刷新缓存
            _gameComponentManager.CacheGameComponents();

            // [核心修复] 不要在这里 return！
            // 即使组件没准备好(AreComponentsReady为false)，也要调用 Controller。
            // 因为 Controller 内部现在有针对 "player == null" 的处理逻辑(归零、重置状态)。
            // 如果在这里 return 了，Controller 就没机会去停止震动了，导致回到主菜单时震动卡死。

            _playerStatusController.ProcessPlayerStatusUpdate(_gameComponentManager);
        }

        public void OnGUI()
        {
            _uiManager.OnGUI();
        }
    }
}