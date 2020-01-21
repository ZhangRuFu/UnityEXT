using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RAMView : MonoBehaviour
{
    public Text MonoHeap;
    public Text MonoUsed;
    public Text GfxRAM;
    public Text TotalAllocated;
    public Text TotalReserved;
    public Text TotalUnusedReserved;

    private RAMModel m_ramModel;

    void Start()
    {
        m_ramModel = MonitorManager.Instance.GetModel<RAMModel>();
    }

    void Update()
    {
        MonoHeap.text = ByteConvert(m_ramModel.MonoHeapSize);
        MonoUsed.text = ByteConvert(m_ramModel.MonoUsedSize);
        GfxRAM.text = ByteConvert(m_ramModel.GfxRAM);
        TotalAllocated.text = ByteConvert(m_ramModel.TotalAllocated);
        TotalReserved.text = ByteConvert(m_ramModel.TotalReserved);
        TotalUnusedReserved.text = ByteConvert(m_ramModel.TotalUnusedReserved);
    }

    private string ByteConvert(double size)
    {
        string[] units = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
        double mod = 1024.0;
        int i = 0;
        while (size >= mod)
        {
            size /= mod;
            i++;
        }
        return string.Format("{0:N2} {1}", size, units[i]);
    }
}
