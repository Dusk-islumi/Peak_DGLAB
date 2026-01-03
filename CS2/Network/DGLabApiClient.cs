using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace PeakDGLab
{
    public class DGLabApiClient
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly ManualLogSource _logger;
        private const string BASE_URL = "http://127.0.0.1:8920/";
        private const string CLIENT_ID = "all"; // 使用 "all" 确保能控制所有连接设备

        private int _lastSentSet = -1;

        public DGLabApiClient(ManualLogSource logger)
        {
            _logger = logger;
            _httpClient.Timeout = TimeSpan.FromMilliseconds(500);
        }

        public async Task SendStrengthUpdateAsync(int set = -1, int add = 0, int sub = 0)
        {
            // 如果没有任何操作，直接返回
            if (set == -1 && add == 0 && sub == 0) return;

            JObject strengthBody = new JObject();
            bool shouldSend = false;

            // --- Set (持续震动) 去重逻辑 ---
            if (set != -1)
            {
                // 只有数值变化时才发送 Set 指令，防止刷屏拥堵
                if (set != _lastSentSet)
                {
                    strengthBody["set"] = set;
                    _lastSentSet = set;
                    shouldSend = true;
                }
            }

            // --- Add/Sub (瞬时脉冲) 总是发送 ---
            if (add > 0)
            {
                strengthBody["add"] = add;
                shouldSend = true;
            }
            if (sub > 0)
            {
                strengthBody["sub"] = sub;
                shouldSend = true;
            }

            if (!shouldSend) return;

            JObject payload = new JObject { ["strength"] = strengthBody };
            string url = $"{BASE_URL}api/game/{CLIENT_ID}/strength_config";
            string jsonContent = payload.ToString();

            try
            {
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // 使用 PostAsync 发送，无需等待返回结果
                await _httpClient.PostAsync(url, content);
            }
            catch (Exception)
            {
                // 保持静默，避免游戏内红字刷屏
            }
        }
    }
}
