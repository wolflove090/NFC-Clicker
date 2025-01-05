using System;

namespace NoaDebugger
{
    /// <summary>
    /// You can get the information of the Information function through this class
    /// </summary>
    public class NoaInformation
    {
        /// <summary>
        /// Returns the system information it holds
        /// </summary>
        public static SystemInformation SystemInformation => NoaInformation._GetSystemInformation();

        /// <summary>
        /// Returns the Unity information it holds
        /// </summary>
        public static UnityInformation UnityInformation => NoaInformation._GetUnityInformation();

        /// <summary>
        /// Return the system information
        /// </summary>
        /// <returns>Returns the system information it holds.</returns>
        static SystemInformation _GetSystemInformation()
        {
            InformationPresenter presenter = NoaDebugger.GetPresenter<InformationPresenter>();

            if (presenter == null)
            {
                return null;
            }

            return presenter.CreateSystemInformation();
        }

        /// <summary>
        /// Returns the Unity information
        /// </summary>
        /// <returns>Returns the Unity information it holds</returns>
        static UnityInformation _GetUnityInformation()
        {
            InformationPresenter presenter = NoaDebugger.GetPresenter<InformationPresenter>();

            if (presenter == null)
            {
                return null;
            }

            return presenter.CreateUnityInformation();
        }

        /// <summary>
        /// Event triggered when logs are downloaded.
        /// Users can register their custom actions to be executed upon log download.
        /// The event provides the filename and the JSON data as strings.
        /// If the event handler returns true, the logs will be downloaded locally.
        /// If the event handler returns false, the logs will not be downloaded locally.
        /// </summary>
        public static Func<string, string, bool> OnDownload
        {
            get
            {
                var presenter = NoaDebugger.GetPresenter<InformationPresenter>();

                return presenter != null ? presenter._onDownload : null;
            }
            set
            {
                var presenter = NoaDebugger.GetPresenter<InformationPresenter>();

                if (presenter == null)
                {
                    return;
                }

                if (value == null)
                {
                    presenter._onDownload = null;
                }
                else
                {
                    presenter._onDownload += value;
                }
            }
        }
    }
}
