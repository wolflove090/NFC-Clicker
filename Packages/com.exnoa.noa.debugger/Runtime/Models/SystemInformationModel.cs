using UnityEngine;

namespace NoaDebugger
{
    sealed class SystemInformationModel : ModelBase
    {
        public ApplicationInfo ApplicationInfo { private set; get; }
        public DeviceInfo DeviceInfo { private set; get; }
        public CpuInfo CpuInfo { private set; get; }
        public GpuInfo GpuInfo { private set; get; }
        public SystemMemoryInfo SystemMemoryInfo { private set; get; }
        public DisplayInfo DisplayInfo { private set; get; }

        public SystemInformationModel()
        {
            ApplicationInfo = new ApplicationInfo();
            
            DeviceInfo = new DeviceInfo();
            
            CpuInfo = new CpuInfo();
            
            GpuInfo = new GpuInfo();
            
#if UNITY_WEBGL && !UNITY_EDITOR
            float memorySize = -1;
#else
            float memorySize = SystemInfo.systemMemorySize;
#endif
            SystemMemoryInfo = new SystemMemoryInfo(memorySize);
            
            DisplayInfo = new DisplayInfo();
        }

        public void OnUpdate()
        {
            DisplayInfo.Refresh();
        }

        public IKeyValueParser[] CreateExportData()
        {
            return new IKeyValueParser[]
            {
                KeyObjectParser.CreateFromClass(ApplicationInfo, "Application"),
                KeyObjectParser.CreateFromClass(DeviceInfo, "Device"),
                KeyObjectParser.CreateFromClass(CpuInfo, "CPU Spec"),
                KeyObjectParser.CreateFromClass(GpuInfo, "GPU Spec"),
                KeyObjectParser.CreateFromClass(SystemMemoryInfo, "Memory Spec"),
                KeyObjectParser.CreateFromClass(DisplayInfo, "Display"),
            };
        }
    }
}
