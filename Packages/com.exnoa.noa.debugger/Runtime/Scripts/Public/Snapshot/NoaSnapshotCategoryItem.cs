namespace NoaDebugger
{
    /// <summary>
    /// Element to include in the snapshot's category
    /// </summary>
    public class NoaSnapshotCategoryItem
    {
        /// <summary>
        /// Constructor
        /// Specifies key-value and color as arguments
        /// </summary>
        /// <param name="key">Key name of additional information</param>
        /// <param name="value">Value of additional information</param>
        /// <param name="color">Font color</param>
        public NoaSnapshotCategoryItem(string key, string value,
                                       NoaSnapshot.FontColor color = NoaSnapshot.FontColor.Gray)
        {
            Key = key;
            Value = value;
            Color = color;
        }

        /// <summary>
        /// Key of additional information
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Value of additional information
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Font color
        /// </summary>
        public NoaSnapshot.FontColor Color { get; }
    }
}
