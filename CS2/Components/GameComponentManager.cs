using BepInEx.Logging;
using UnityEngine;

namespace PeakDGLab
{
    public class GameComponentManager
    {
        private readonly ManualLogSource _logger;

        public GameComponentManager(ManualLogSource logger)
        {
            _logger = logger;
        }

        public Character Player { get; private set; }

        public bool AreComponentsReady()
        {
            if (Character.localCharacter == null) return false;
            if (Character.localCharacter.data == null) return false;
            if (Character.localCharacter.refs == null) return false;
            if (Character.localCharacter.refs.afflictions == null) return false;
            return true;
        }

        // [核心] 刷新缓存的方法，PlayerStatusController 和 Main 都会调用
        public void CacheGameComponents()
        {
            Player = Character.localCharacter;
        }

        public void CheckAndLogStatus()
        {
            if (Character.localCharacter == null)
            {
                _logger.LogInfo("[Check] Character.localCharacter is null");
                return;
            }

            if (Character.localCharacter.data == null)
            {
                _logger.LogInfo("[Check] Character.localCharacter.data is null");
            }

            if (Character.localCharacter.refs == null)
            {
                _logger.LogInfo("[Check] Character.localCharacter.refs is null");
            }
            else if (Character.localCharacter.refs.afflictions == null)
            {
                _logger.LogInfo("[Check] Character.localCharacter.refs.afflictions is null");
            }
        }
    }
}