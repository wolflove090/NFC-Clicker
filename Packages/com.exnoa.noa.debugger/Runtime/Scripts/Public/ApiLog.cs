using System;
using System.Collections.Generic;

namespace NoaDebugger
{
    /// <summary>
    /// Content of the API log.
    /// </summary>
    public class ApiLog : ILogDetail
    {
        /// <summary>
        /// Request destination URL.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// Request method.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Status code.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Data size of the response.
        /// </summary>
        public long ContentSize { get; set; }

        /// <summary>
        /// Specify the time it took for the response in milliseconds.
        /// </summary>
        public long ResponseTimeMilliSeconds { get; set; }

        /// <summary>
        /// Request header.
        /// </summary>
        public Dictionary<string, string> RequestHeaders { get; set; }

        /// <summary>
        /// Request body.
        /// </summary>
        public string RequestBody { get; set; }

        /// <summary>
        /// Request body before conversion.
        /// </summary>
        public object RequestBodyRawData { get; set; }

        /// <summary>
        /// Response header.
        /// </summary>
        public Dictionary<string, string> ResponseHeaders { get; set; }

        /// <summary>
        /// Response body.
        /// </summary>
        public string ResponseBody { get; set; }

        /// <summary>
        /// Response body before conversion.
        /// </summary>
        public object ResponseBodyRawData { get; set; }

        /// <summary>
        /// If the status code represents a successful value, it returns true.
        /// </summary>
        public bool IsSuccess => StatusCode is >= 200 and < 300;

        /// <summary>
        /// If true and in JSON format, formatting will be executed.
        /// </summary>
        public bool PrettyPrint { get; set; } = true;


        string prettyPrintedRequestBody = null;

        /// <summary>
        /// Gets the pretty printed request body.
        /// </summary>
        public string PrettyPrintedRequestBody
        {
            get
            {
                prettyPrintedRequestBody ??= ToPrettyPrint(RequestBody);
                return prettyPrintedRequestBody;
            }
        }

        string prettyPrintedResponseBody = null;

        /// <summary>
        /// Gets the pretty printed response body.
        /// </summary>
        public string PrettyPrintedResponseBody
        {
            get
            {
                prettyPrintedResponseBody ??= ToPrettyPrint(ResponseBody);
                return prettyPrintedResponseBody;
            }
        }

        internal void ConvertBody()
        {
            if (RequestBodyRawData != null && ApiLogger.OnConvertRequestBodyToString != null)
            {
                RequestBody = ApiLogger.OnConvertRequestBodyToString.Invoke(RequestBodyRawData);
                RequestBodyRawData = null;
            }

            if (ResponseBodyRawData != null && ApiLogger.OnConvertResponseBodyToString != null)
            {
                ResponseBody = ApiLogger.OnConvertResponseBodyToString.Invoke(ResponseBodyRawData);
                ResponseBodyRawData = null;
            }
        }

        /// <summary>
        /// Gets the text to copy to the clipboard.
        /// </summary>
        string ILogDetail.GetClipboardText()
        {
            return ApiLogPresenter.CreateLogDetailString(this);
        }

        /// <summary>
        /// Gets the pretty printed text.
        /// </summary>
        /// <param name="text">Target text for pretty print.</param>
        string ToPrettyPrint(string text)
        {
            if (text != null && PrettyPrint)
            {
                string formattedString = JsonFormatter.Format(text);

                if (formattedString != null)
                {
                    return formattedString;
                }
            }

            return text;
        }
    }
}
