using System;
using UnityEngine;

namespace PeakDGLab
{
    public class UIManager
    {
        public bool IsVisible { get; set; } = false;

        private readonly ConfigManager _config;
        private Rect _windowRect = new Rect(20, 20, 480, 50);

        // 临时变量
        private string _lowStaminaStr, _staminaCurveStr, _energyMultStr;
        private string _wDrowsy, _wCold, _wHot, _wPoison, _wThorns, _wCurse, _wHunger;
        private string _passOutMultStr, _deathPunishStr, _deathDurationStr, _fallPunishStr;
        private string _intervalStr, _reductionStr;

        // 样式缓存
        private GUIStyle _cyanLabelStyle;
        private GUIStyle _headerStyle;

        public UIManager(ConfigManager config)
        {
            _config = config;
            RefreshStrings();
        }

        private void RefreshStrings()
        {
            _lowStaminaStr = _config.LowStaminaMaxStrength.Value.ToString("0.#");
            _staminaCurveStr = _config.StaminaCurvePower.Value.ToString("0.#");
            _energyMultStr = _config.EnergyLossMultiplier.Value.ToString("0.#");

            _wDrowsy = _config.WeightDrowsy.Value.ToString("0.#");
            _wCold = _config.WeightCold.Value.ToString("0.#");
            _wHot = _config.WeightHot.Value.ToString("0.#");
            _wPoison = _config.WeightPoison.Value.ToString("0.#");
            _wThorns = _config.WeightThorns.Value.ToString("0.#");
            _wCurse = _config.WeightCurse.Value.ToString("0.#");
            _wHunger = _config.WeightHunger.Value.ToString("0.#");

            _passOutMultStr = _config.PassOutMultiplier.Value.ToString("0.#");
            _deathPunishStr = _config.DeathPunishment.Value.ToString();
            _deathDurationStr = _config.DeathDuration.Value.ToString("0.#");
            _fallPunishStr = _config.FallPunishment.Value.ToString();

            _intervalStr = _config.CheckIntervalMs.Value.ToString();
            _reductionStr = _config.ReductionValue.Value.ToString("0.#");
        }

        public void OnGUI()
        {
            if (!IsVisible) return;

            if (_cyanLabelStyle == null)
            {
                _cyanLabelStyle = new GUIStyle(GUI.skin.label);
                _cyanLabelStyle.normal.textColor = Color.cyan;
                _cyanLabelStyle.fontSize = 18;
                _cyanLabelStyle.fontStyle = FontStyle.Bold;
                _cyanLabelStyle.alignment = TextAnchor.MiddleCenter;
            }
            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle(GUI.skin.label);
                _headerStyle.fontStyle = FontStyle.Bold;
                _headerStyle.normal.textColor = new Color(1f, 0.8f, 0.4f);
            }

            GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            _windowRect = GUILayout.Window(123456, _windowRect, DrawWindow, "PEAK x DGLAB 控制面板");
        }

        private void DrawWindow(int windowID)
        {
            GUILayout.BeginVertical();

            // === 顶部：系统控制 ===
            GUILayout.Label($"=== 系统控制 (快捷键: {_config.ToggleShockKey.Value}) ===", _headerStyle);

            // 1. 电击总开关
            bool isEnabled = _config.EnableShock.Value;
            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = isEnabled ? Color.green : Color.red;

            if (GUILayout.Button(isEnabled ? "⚡ 电击输出: [已开启]" : "⚡ 电击输出: [已停止]", GUILayout.Height(30)))
            {
                _config.EnableShock.Value = !_config.EnableShock.Value;
            }

            // 2. 观战模式开关
            GUILayout.Space(5);
            bool isSpec = _config.EnableSpectatorShock.Value;
            GUI.backgroundColor = isSpec ? Color.cyan : Color.gray;

            if (GUILayout.Button(isSpec ? "👁 观战震动: [已开启] (同步队友)" : "👁 观战震动: [已关闭] (仅限自己)", GUILayout.Height(25)))
            {
                _config.EnableSpectatorShock.Value = !_config.EnableSpectatorShock.Value;
            }

            // 恢复颜色
            GUI.backgroundColor = originalColor;

            // === 滚动区域 (防止窗口过长) ===
            GUILayout.Space(5);
            GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));

            // === 1. 配置区域 ===
            GUILayout.Label("--- 参数配置 ---", _headerStyle);

            DrawSection("体力反馈", () =>
            {
                DrawFloatField("空体力最大强度:", ref _lowStaminaStr, _config.LowStaminaMaxStrength);
                DrawFloatField("体力曲线(1.0线性):", ref _staminaCurveStr, _config.StaminaCurvePower);
            });

            DrawSection("能量/SP反馈", () =>
            {
                DrawFloatField("能量消耗刺痛倍率:", ref _energyMultStr, _config.EnergyLossMultiplier);
            });

            DrawSection("异常状态权重", () =>
            {
                DrawFloatField("困倦:", ref _wDrowsy, _config.WeightDrowsy);
                DrawFloatField("寒冷:", ref _wCold, _config.WeightCold);
                DrawFloatField("过热:", ref _wHot, _config.WeightHot);
                DrawFloatField("中毒:", ref _wPoison, _config.WeightPoison);
                DrawFloatField("刺痛:", ref _wThorns, _config.WeightThorns);
                DrawFloatField("诅咒:", ref _wCurse, _config.WeightCurse);
                DrawFloatField("饥饿:", ref _wHunger, _config.WeightHunger);
            });

            DrawSection("特殊情境", () =>
            {
                DrawFloatField("昏迷震动倍率:", ref _passOutMultStr, _config.PassOutMultiplier);
                DrawIntField("死亡惩罚强度:", ref _deathPunishStr, _config.DeathPunishment);
                DrawFloatField("死亡惩罚时长(秒):", ref _deathDurationStr, _config.DeathDuration);
                DrawIntField("摔倒惩罚强度:", ref _fallPunishStr, _config.FallPunishment);
            });

            GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));

            // === 2. 实时监控区域 (根据开关智能显示目标) ===
            GUILayout.Label("=== 实时数据监控 ===", _headerStyle);

            Character player;
            if (_config.EnableSpectatorShock.Value)
            {
                player = Character.observedCharacter;
            }
            else
            {
                player = Character.localCharacter;
            }

            if (player != null && player.data != null)
            {
                var d = player.data;

                // 显示当前监控的是谁
                string targetName = player == Character.localCharacter ? "自己" : "队友/观察目标";
                GUILayout.Label($"监控目标: {targetName}");

                // 显示体力与能量
                GUILayout.BeginHorizontal();
                GUILayout.Label($"体力: {d.currentStamina * 100:F1}%");
                GUILayout.Label($"能量(SP): {d.extraStamina * 100:F1}%");
                GUILayout.EndHorizontal();

                // 显示异常状态 (只显示有数值的)
                if (player.refs != null && player.refs.afflictions != null)
                {
                    var aff = player.refs.afflictions;
                    string statusStr = "";

                    float v;
                    if ((v = aff.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Drowsy)) > 0) statusStr += $"困倦:{v:F1} ";
                    if ((v = aff.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Cold)) > 0) statusStr += $"寒冷:{v:F1} ";
                    if ((v = aff.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Hot)) > 0) statusStr += $"过热:{v:F1} ";
                    if ((v = aff.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Poison)) > 0) statusStr += $"中毒:{v:F1} ";
                    if ((v = aff.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Thorns)) > 0) statusStr += $"刺痛:{v:F1} ";
                    if ((v = aff.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Curse)) > 0) statusStr += $"诅咒:{v:F1} ";
                    if ((v = aff.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Hunger)) > 0) statusStr += $"饥饿:{v:F1} ";

                    if (string.IsNullOrEmpty(statusStr)) statusStr = "无异常状态";
                    GUILayout.Label($"状态: {statusStr}");
                }

                // 显示摔倒状态
                if (d.fallSeconds > 0.1f)
                {
                    GUILayout.Label($"<color=orange>摔倒/失控: {d.fallSeconds:F1}s</color>");
                }

                // [修复] 只有在 (真死) 或者 (濒死且瘫软) 时才显示红字
                if (d.dead || (d.deathTimer > 0 && d.currentRagdollControll < 0.5f))
                {
                    GUILayout.Label($"!! 死亡状态 !!", new GUIStyle(GUI.skin.label) { normal = { textColor = Color.red } });
                }
            }
            else
            {
                GUILayout.Label("等待游戏数据...");
            }

            // === 最终强度大字显示 ===
            GUILayout.Space(10);
            float finalStrength = PlayerStatusController.CurrentFinalStrength;
            GUILayout.Label($"当前输出强度: {finalStrength:F1}", _cyanLabelStyle);
            GUILayout.Space(5);

            // 底部按钮
            if (GUILayout.Button("关闭菜单")) IsVisible = false;

            GUILayout.EndVertical();

            // 让窗口可拖拽
            GUI.DragWindow();
        }

        private void DrawSection(string title, Action drawContent)
        {
            GUILayout.Label($"[{title}]");
            drawContent.Invoke();
            GUILayout.Space(3);
        }

        private void DrawFloatField(string label, ref string valueStr, BepInEx.Configuration.ConfigEntry<float> configEntry)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(180));
            string newVal = GUILayout.TextField(valueStr);
            if (newVal != valueStr)
            {
                valueStr = newVal;
                if (float.TryParse(valueStr, out float result)) configEntry.Value = result;
            }
            GUILayout.EndHorizontal();
        }

        private void DrawIntField(string label, ref string valueStr, BepInEx.Configuration.ConfigEntry<int> configEntry)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(180));
            string newVal = GUILayout.TextField(valueStr);
            if (newVal != valueStr)
            {
                valueStr = newVal;
                if (int.TryParse(valueStr, out int result)) configEntry.Value = result;
            }
            GUILayout.EndHorizontal();
        }
    }
}