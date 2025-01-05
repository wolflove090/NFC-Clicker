using UnityEngine;
using UnityEngine.Assertions;

namespace NoaDebugger
{
    sealed class ConsoleLogView : LogViewBase
    {
        const string UNAVAILABLE_STACK_TRACE_LABEL = "-- No stack trace available --";

        [SerializeField, Header("Log details")]
        ConsoleLogDetailView _logDetail;

        protected override void _OnInit()
        {
            Assert.IsNotNull(_logDetail);
        }

        protected override void _OnUpdateDetail(ILogDetail detail)
        {
            if (detail is not ConsoleLogDetail log)
            {
                return;
            }

            string detailString = log.LogDetailString;

            if (!log.IsRegisteredApi && !log.HasStackTrace())
            {
                detailString = $"{log.LogDetailString}\n{UNAVAILABLE_STACK_TRACE_LABEL}";
            }

            _logDetail.SetLogDetailText(detailString);
            _logDetail.SetCopyButton(() => OnCopy(log));
        }
    }
}
