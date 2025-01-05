using UnityEngine;

namespace NoaDebugger
{
    /// <summary>
    /// Application information
    /// </summary>
    public sealed class ApplicationInfo
    {
        /// <summary>
        /// Application's product name
        /// </summary>
        public string Identification { private set; get; }

        /// <summary>
        /// Application's version information
        /// </summary>
        public string Version { private set; get; }

        /// <summary>
        /// Language information of the operating system in use
        /// </summary>
        public string Language { private set; get; }

        /// <summary>
        /// Execution platform of the running application
        /// </summary>
        public string Platform { private set; get; }

        /// <summary>
        /// Generates ApplicationInfo
        /// </summary>
        internal ApplicationInfo()
        {
            Identification = Application.identifier;
            Version = Application.version;
            Language = Application.systemLanguage.ToString();
            Platform = Application.platform.ToString();
        }
    }
}
