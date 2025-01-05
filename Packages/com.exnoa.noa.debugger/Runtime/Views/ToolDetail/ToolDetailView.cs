using System;
using TMPro;
using UnityEngine;

namespace NoaDebugger
{
    sealed class ToolDetailView : NoaDebuggerToolViewBase<ToolDetailViewLinker>
    {
        [Header("BaseContent")]
        [SerializeField] TextMeshProUGUI _operatingEnv;
        [SerializeField] TextMeshProUGUI _copyright;

        [Header("SaveDataContent")]
        [SerializeField] SaveDataContent _noaPrefsSaveData;
        [SerializeField] SaveDataContent _debugCommandPropertySaveData;
        [SerializeField] SaveDataContent _toolSaveData;


        protected override void _OnShow(ToolDetailViewLinker linker)
        {
            _operatingEnv.text = $": {linker._operatingEnv}";
            _copyright.text = linker._copyright;

            _ShowNoaPrefsSaveDataContent();
            _ShowDebugCommandPropertySaveDataContent();
            _ShowToolSaveDataContent();
        }

        void _ShowNoaPrefsSaveDataContent()
        {
            string[] targetKeys = NoaDebuggerPrefs.GetKeyListFilterAt(NoaDebuggerPrefsDefine.PrefsKeyNoaPrefsDataPrefix).ToArray();
            _noaPrefsSaveData.Show(targetKeys);
        }

        void _ShowDebugCommandPropertySaveDataContent()
        {
            string[] targetKeys = NoaDebuggerPrefs.GetKeyListFilterAt(NoaDebuggerPrefsDefine.PrefsKeyDebugCommandPropertiesPrefix).ToArray();
            _debugCommandPropertySaveData.Show(targetKeys);
        }

        void _ShowToolSaveDataContent()
        {
            string[] targetKeys = NoaDebuggerPrefs.GetKeyListForToolOnly().ToArray();
            _toolSaveData.Show(targetKeys);
        }

        protected override void _OnHide()
        {
            gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            _operatingEnv = default;
            _copyright = default;
            _noaPrefsSaveData = default;
            _debugCommandPropertySaveData = default;
            _toolSaveData = default;
        }
    }


    sealed class ToolDetailViewLinker : ViewLinkerBase
    {
        public string _operatingEnv;

        public string _copyright;
    }
}
