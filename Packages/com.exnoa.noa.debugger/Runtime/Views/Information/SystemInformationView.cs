using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

namespace NoaDebugger
{
    sealed class SystemInformationView : ViewBase<SystemInformationViewLinker>
    {
        [SerializeField] ContextPanelView _contextPanelView;

        ContextPanelView _applicationPanelView;
        ContextPanelView _devicePanelView;
        ContextPanelView _cpuPanelView;
        ContextPanelView _gpuPanelView;
        ContextPanelView _memoryPanelView;
        ContextPanelView _displayPanelView;

        protected override void _OnStart()
        {
            base._OnStart();
            _contextPanelView.gameObject.SetActive(false);
        }

        protected override void _OnShow(SystemInformationViewLinker linker)
        {
            if (_applicationPanelView == null)
            {
                _applicationPanelView = Instantiate(_contextPanelView, _contextPanelView.transform.parent);
            }
            var applicationContextList = new Dictionary<string, string>()
            {
                {"Identification", linker._identification},
                {"Version", linker._version},
                {"Language", linker._language},
                {"Platform", linker._platform},
            };
            _applicationPanelView.SetText("Application", applicationContextList);
            _applicationPanelView.name = "Application";

            if (_devicePanelView == null)
            {
                _devicePanelView = Instantiate(_contextPanelView, _contextPanelView.transform.parent);
            }
            var deviceContextList = new Dictionary<string, string>()
            {
                {"Operating System", linker._os},
                {"Device Model", linker._deviceModel},
                {"Device Type", linker._deviceType},
                {"Device Name", linker._deviceName},
            };
            _devicePanelView.SetText("Device", deviceContextList);
            _devicePanelView.name = "Device";

            if (_cpuPanelView == null)
            {
                _cpuPanelView = Instantiate(_contextPanelView, _contextPanelView.transform.parent);
            }
            var cpuContextList = new Dictionary<string, string>()
            {
                {"CPU Type", linker._cpuType},
                {"CPU Count", linker._cpuCount},
            };
            _cpuPanelView.SetText("CPU Spec", cpuContextList);
            _cpuPanelView.name = "CPU";

            if (_gpuPanelView == null)
            {
                _gpuPanelView = Instantiate(_contextPanelView, _contextPanelView.transform.parent);
            }
            var gpuContextList = new Dictionary<string, string>()
            {
                {"GPU Device Name", linker._gpuDeviceName},
                {"GPU Device Type", linker._gpuDeviceType},
                {"GPU Device Version", linker._gpuDeviceVersion},
                {"GPU Device Vendor", linker._gpuDeviceVendor},
                {"GPU Memory Size", linker._gpuDeviceSize},
            };
            _gpuPanelView.SetText("GPU Spec", gpuContextList);
            _gpuPanelView.name = "GPU";

            if (_memoryPanelView == null)
            {
                _memoryPanelView = Instantiate(_contextPanelView, _contextPanelView.transform.parent);
            }
            var memoryContextList = new Dictionary<string, string>()
            {
                {"Memory Size", linker._memorySize},
            };
            _memoryPanelView.SetText("Memory Spec", memoryContextList);
            _memoryPanelView.name = "Memory";

            if (_displayPanelView == null)
            {
                _displayPanelView = Instantiate(_contextPanelView, _contextPanelView.transform.parent);
            }
            var displayContextList = new Dictionary<string, string>()
            {
                {"Refresh Rate", linker._refreshRate},
                {"Resolution", linker._resolution},
                {"Aspect", linker._aspect},
                {"DPI", linker._dpi},
                {"Orientation", linker._orientation},
            };
            _displayPanelView.SetText("Display", displayContextList);
            _displayPanelView.name = "Display";
            _displayPanelView.SetActiveLine(false);

            gameObject.SetActive(true);
        }

        protected override void _OnHide()
        {
            gameObject.SetActive(false);
        }
    }

    sealed class SystemInformationViewLinker : ViewLinkerBase
    {

        public string _identification;

        public string _version;

        public string _language;

        public string _platform;


        public string _os;

        public string _deviceModel;

        public string _deviceType;

        public string _deviceName;


        public string _cpuType;

        public string _cpuCount;


        public string _gpuDeviceName;

        public string _gpuDeviceType;

        public string _gpuDeviceVersion;

        public string _gpuDeviceVendor;

        public string _gpuDeviceSize;


        public string _memorySize;


        public string _refreshRate;

        public string _resolution;

        public string _aspect;

        public string _dpi;

        public string _orientation;
    }
}
