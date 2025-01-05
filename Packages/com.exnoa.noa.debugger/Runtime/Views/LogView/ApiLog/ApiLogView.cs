using UnityEngine;
using UnityEngine.Assertions;

namespace NoaDebugger
{
    sealed class ApiLogView : LogViewBase
    {
        [SerializeField, Header("Log details")]
        ApiLogDetailView _logDetail;

        [SerializeField]
        OrientationLayoutRuntimeAdjuster _layoutAdjuster;

        protected override void _OnInit()
        {
            Assert.IsNotNull(_logDetail);
            Assert.IsNotNull(_layoutAdjuster);
        }

        protected override void _OnShow(LogViewLinker linker)
        {
            base._OnShow(linker);

            _layoutAdjuster.SetHandleTargetLayouts();
        }

        protected override void _OnUpdateDetail(ILogDetail detail)
        {
            if (detail is not ApiLog log)
            {
                return;
            }

            log.ConvertBody();

            _logDetail.ResetContent();
            _UpdateRequestTab(log);
            _UpdateResponseTab(log);
            _logDetail.SetCopyButton(() => OnCopy(log));
        }

        void _UpdateRequestTab(ApiLog log)
        {
            _logDetail.UpdateRequestTab(log);
        }

        void _UpdateResponseTab(ApiLog log)
        {
            _logDetail.UpdateResponseTab(log);
        }
    }
}
