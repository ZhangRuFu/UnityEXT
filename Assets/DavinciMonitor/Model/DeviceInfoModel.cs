using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceInfoModel : IModel
{
    public string CPUInfo { get; private set; }
    public int CPUCore { get; private set; }

    public int RAMSize { get; private set; }

    public string GraphicAPI { get; private set; }
    public string GPU { get; private set; }
    public int VRAM { get; private set; }
    public int MaxTextureSize { get; private set; }
    public int ShaderLevel { get; private set; }

    public int ScreenWidth { get; private set; }
    public int ScreenHeight { get; private set; }
    public int ScreenRefreshRate { get; private set; }
    public string OS { get; private set; }

    public override void Init()
    {
        GetSystemInfo();
    }

    public override void Update()
    {

    }

    void GetSystemInfo()
    {
        CPUInfo = SystemInfo.processorType;
        CPUCore = SystemInfo.processorCount;

        RAMSize = SystemInfo.systemMemorySize;

        GraphicAPI = SystemInfo.graphicsDeviceVersion;
        GPU = SystemInfo.graphicsDeviceName;

        VRAM = SystemInfo.graphicsMemorySize;
        MaxTextureSize = SystemInfo.maxTextureSize;
        ShaderLevel = SystemInfo.graphicsShaderLevel;

        Resolution res = Screen.currentResolution;
        ScreenWidth = res.width;
        ScreenHeight = res.height;
        ScreenRefreshRate = res.refreshRate;

        OS = SystemInfo.operatingSystem;
    }
}
