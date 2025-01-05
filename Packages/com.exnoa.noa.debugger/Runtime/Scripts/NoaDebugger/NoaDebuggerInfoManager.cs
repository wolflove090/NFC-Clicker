using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NoaDebugger
{
    sealed class NoaDebuggerInfoManager
    {
        const string TOOL_INFORMATION_ASSET_NAME = "NoaDebuggerInfo";

        static NoaDebuggerInfoManager _instance;
        static NoaDebuggerInfoManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NoaDebuggerInfoManager();
                }

                return _instance;
            }
        }

        NoaDebuggerInfo _noaDebuggerInfo;

        public static NoaDebuggerInfo GetNoaDebuggerInfo()
        {
            return Instance._GetNoaDebuggerInfo();
        }

        NoaDebuggerInfo _GetNoaDebuggerInfo()
        {
            if (_noaDebuggerInfo == null)
            {
                NoaDebuggerInfo info = Resources.Load<NoaDebuggerInfo>(TOOL_INFORMATION_ASSET_NAME);

                _noaDebuggerInfo = info;
            }

            return _noaDebuggerInfo;
        }

        public static void Dispose()
        {
            NoaDebuggerInfoManager._instance = null;
        }
    }
}
