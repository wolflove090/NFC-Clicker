using System;

namespace NoaDebugger
{
    /// <summary>
    /// The API logger.
    /// </summary>
    public static class ApiLogger
    {
        /// <summary>
        /// The type of callback called when an API log is received.
        /// </summary>
        internal delegate void LogCallback(ApiLog log);

        /// <summary>
        /// This gets called when an API log is received.
        /// </summary>
        internal static event LogCallback OnLogReceived;

        /// <summary>
        /// Convert the request body to string.
        /// </summary>
        public static Func<object, string> OnConvertRequestBodyToString { get; set; }

        /// <summary>
        /// Convert the response body to string.
        /// </summary>
        public static Func<object, string> OnConvertResponseBodyToString { get; set; }

        /// <summary>
        /// Outputs API logs.
        /// </summary>
        /// <param name="log">The API log to be outputted.</param>
        public static void Log(ApiLog log)
        {
            if (log.Url == null)
            {
                throw new ArgumentException("'Url' is not set.");
            }

            if (string.IsNullOrEmpty(log.Method))
            {
                throw new ArgumentException("'Method' is not set.");
            }

            OnLogReceived?.Invoke(log);
        }
    }
}
