namespace NoaDebugger
{
    /// <summary>
    /// Memory Information
    /// </summary>
    public sealed class SystemMemoryInfo
    {
        /// <summary>
        /// Memory capacity of the device
        /// </summary>
        public float MemorySizeMB { private set; get; }

        /// <summary>
        /// Generates SystemMemoryInfo
        /// </summary>
        /// <param name="memorySizeMB">Specifies the memory capacity of the device</param>
        internal SystemMemoryInfo(float memorySizeMB)
        {
            MemorySizeMB = memorySizeMB;
        }
    }
}
