using UnityEngine;

namespace NoaDebugger
{
    /// <summary>
    /// Graphics information
    /// </summary>
    public sealed class GraphicsInfo
    {
        /// <summary>
        /// Maximum size of the texture supported by the graphics hardware
        /// </summary>
        public int MaxTexSize { private set; get; }

        /// <summary>
        /// Support for NPOT (Non-Power-Of-Two) textures
        /// </summary>
        public NPOTSupport NpotSupport { private set; get; }

        /// <summary>
        /// Whether compute shaders are supported
        /// </summary>
        public bool ComputeShaders { private set; get; }

        /// <summary>
        /// Whether built-in shadows are supported
        /// </summary>
        public bool Shadows { private set; get; }

        /// <summary>
        /// Whether sparse textures are supported
        /// </summary>
        public bool SparseTextures { private set; get; }

        /// <summary>
        /// Generates GraphicsInfo
        /// </summary>
        public GraphicsInfo()
        {
            MaxTexSize = SystemInfo.maxTextureSize;
            NpotSupport = SystemInfo.npotSupport;
            ComputeShaders = SystemInfo.supportsComputeShaders;
            Shadows = SystemInfo.supportsShadows;
            SparseTextures = SystemInfo.supportsSparseTextures;
        }
    }
}
