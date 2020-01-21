using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class RAMModel : IModel
{
    public long MonoHeapSize { get; private set; }
    public long MonoUsedSize { get; private set; }
    public long GfxRAM { get; private set; }
    public long TotalAllocated { get; private set; }
    public long TotalReserved { get; private set; }
    public long TotalUnusedReserved { get; private set; }

    public override void Init()
    {
        
    }

    public override void Update()
    {
        MonoHeapSize = Profiler.GetMonoHeapSizeLong();
        MonoUsedSize = Profiler.GetMonoUsedSizeLong();
        GfxRAM = Profiler.GetAllocatedMemoryForGraphicsDriver();
        TotalAllocated = Profiler.GetTotalAllocatedMemoryLong();
        TotalReserved = Profiler.GetTotalReservedMemoryLong();
        TotalUnusedReserved = Profiler.GetTotalUnusedReservedMemoryLong();
    }
}
