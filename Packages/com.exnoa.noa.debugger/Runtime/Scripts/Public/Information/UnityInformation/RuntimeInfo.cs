using UnityEngine;
using UnityEngine.SceneManagement;

namespace NoaDebugger
{
    /// <summary>
    /// Runtime Information
    /// </summary>
    public sealed class RuntimeInfo
    {
        /// <summary>
        /// Total elapsed time
        /// </summary>
        public float PlayTime { private set; get; }

        /// <summary>
        /// Elapsed time in the current scene
        /// </summary>
        public float LevelPlayTime { private set; get; }

        /// <summary>
        /// Current scene name
        /// </summary>
        public string CurrentLevelSceneName { private set; get; }

        /// <summary>
        /// Current scene index number
        /// </summary>
        public int CurrentLevelBuildIndex { private set; get; }

        /// <summary>
        /// Quality level
        /// </summary>
        public int QualityLevel { private set; get; }

        /// <summary>
        /// Generates RuntimeInfo
        /// </summary>
        internal RuntimeInfo()
        {
            PlayTime = Time.realtimeSinceStartup;
            LevelPlayTime = Time.timeSinceLevelLoad;
            CurrentLevelSceneName = SceneManager.GetActiveScene().name;
            CurrentLevelBuildIndex = SceneManager.GetActiveScene().buildIndex;
            QualityLevel = QualitySettings.GetQualityLevel();
        }

        /// <summary>
        /// Updates the elapsed time
        /// </summary>
        internal void RefreshTime()
        {
            PlayTime = Time.realtimeSinceStartup;
            LevelPlayTime = Time.timeSinceLevelLoad;
        }

        /// <summary>
        /// Updates the scene information
        /// </summary>
        internal void RefreshScene()
        {
            CurrentLevelSceneName = SceneManager.GetActiveScene().name;
            CurrentLevelBuildIndex = SceneManager.GetActiveScene().buildIndex;
        }
    }
}
