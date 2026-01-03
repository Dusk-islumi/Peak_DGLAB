using BepInEx.Configuration;
using UnityEngine;

namespace PeakDGLab
{
    public enum DeathDecayMode { Smooth, Instant }

    public class ConfigManager
    {
        // 1. 体力
        public ConfigEntry<float> LowStaminaMaxStrength { get; private set; }
        public ConfigEntry<float> StaminaCurvePower { get; private set; }
        // 2. 能量
        public ConfigEntry<float> EnergyLossMultiplier { get; private set; }
        // 3. 异常
        public ConfigEntry<float> WeightDrowsy { get; private set; }
        public ConfigEntry<float> WeightCold { get; private set; }
        public ConfigEntry<float> WeightHot { get; private set; }
        public ConfigEntry<float> WeightPoison { get; private set; }
        public ConfigEntry<float> WeightThorns { get; private set; }
        public ConfigEntry<float> WeightCurse { get; private set; }
        public ConfigEntry<float> WeightHunger { get; private set; }

        // 4. 特殊
        public ConfigEntry<float> PassOutMultiplier { get; private set; }
        public ConfigEntry<int> DeathPunishment { get; private set; }
        public ConfigEntry<float> DeathDuration { get; private set; }
        public ConfigEntry<DeathDecayMode> DeathDecayType { get; private set; }
        public ConfigEntry<int> FallPunishment { get; private set; }

        // 5. 系统
        public ConfigEntry<bool> EnableShock { get; private set; }
        public ConfigEntry<bool> EnableSpectatorShock { get; private set; }
        public ConfigEntry<KeyCode> ToggleShockKey { get; private set; }
        public ConfigEntry<KeyCode> ToggleUiKey { get; private set; }

        // 6. 内部
        public ConfigEntry<int> CheckIntervalMs { get; private set; }
        public ConfigEntry<float> ReductionValue { get; private set; }
        public ConfigEntry<bool> EnableDebugLog { get; private set; }

        public ConfigManager(ConfigFile config)
        {
            LowStaminaMaxStrength = config.Bind("1. 体力", "空体力最大强度", 60f, "体力耗尽时的震动强度。");
            StaminaCurvePower = config.Bind("1. 体力", "体力曲线", 1.0f, "1为线性，可改为2。");

            EnergyLossMultiplier = config.Bind("2. 能量", "能量刺痛倍率（目前不可用）", 1f, "SP消耗时的震动。");

            WeightDrowsy = config.Bind("3. 异常", "权重: 困倦(当困倦值满时增加10点强度，其他同理)", 10f, "");
            WeightCold = config.Bind("3. 异常", "权重: 寒冷", 20f, "");
            WeightHot = config.Bind("3. 异常", "权重: 过热", 20f, "");
            WeightPoison = config.Bind("3. 异常", "权重: 中毒", 50f, "");
            WeightThorns = config.Bind("3. 异常", "权重: 刺痛", 30f, "");
            WeightCurse = config.Bind("3. 异常", "权重: 诅咒", 40f, "");
            WeightHunger = config.Bind("3. 异常", "权重: 饥饿", 15f, "");

            PassOutMultiplier = config.Bind("4. 特殊", "昏迷_强度倍率", 0.0f, "");
            DeathPunishment = config.Bind("4. 特殊", "死亡_惩罚强度", 100, "");
            DeathDuration = config.Bind("4. 特殊", "死亡_惩罚持续时间(单位为秒)", 30.0f, "");
            DeathDecayType = config.Bind("4. 特殊", "死亡_结束方式", DeathDecayMode.Smooth, "");
            FallPunishment = config.Bind("4. 特殊", "摔倒惩罚强度", 50, "当角色处于摔倒/失控(Ragdoll)状态时的额外附加震动。");

            EnableShock = config.Bind("5. 系统", "总开关_启用电击", true, "紧急停止开关。");
            EnableSpectatorShock = config.Bind("5. 系统", "观战_感同身受", false, "观战时是否同步队友的状态进行震动。");
            ToggleShockKey = config.Bind("5. 系统", "快捷键_电击开关", KeyCode.P, "");
            ToggleUiKey = config.Bind("5. 系统", "快捷键_设置菜单", KeyCode.F10, "");

            CheckIntervalMs = config.Bind("6. 内部", "刷新间隔(ms)", 100, "");
            ReductionValue = config.Bind("6. 内部", "自然衰减速度", 2.0f, "");
            EnableDebugLog = config.Bind("6. 内部", "启用调试日志", true, "");
        }
    }
}
