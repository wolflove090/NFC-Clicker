namespace NoaDebugger
{
    /// <summary>
    /// Holds Unity information
    /// </summary>
    public sealed class UnityInformation
    {
        /// <summary>
        /// Unity information
        /// </summary>
        public UnityInfo UnityInfo { private set; get; }

        /// <summary>
        /// Runtime information
        /// </summary>
        public RuntimeInfo RuntimeInfo { private set; get; }

        /// <summary>
        /// Features information
        /// </summary>
        public FeaturesInfo FeaturesInfo { private set; get; }

        /// <summary>
        /// Graphics information
        /// </summary>
        public GraphicsInfo GraphicsInfo { private set; get; }

        /// <summary>
        /// Generates UnityInformation
        /// </summary>
        /// <param name="model">Specifies the source model for reference</param>
        internal UnityInformation(UnityInformationModel model)
        {
            UnityInfo = model.UnityInfo;
            RuntimeInfo = model.RuntimeInfo;
            FeaturesInfo = model.FeaturesInfo;
            GraphicsInfo = model.GraphicsInfo;
        }
    }
}
