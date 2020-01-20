using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class FPSView : MonoBehaviour
{
    FPSModel m_fpsModel;

    public Text MinFPSText;
    public Text MaxFPSText;
    public Text FPSText;
    public Text SPFText;
    public Text TargetFPS;
    public Text VSync;
    public Image FPSGraph;

    private Material m_fpsMat;

    void Start()
    {
        m_fpsModel = MonitorManager.Instance.GetModel<FPSModel>();

        m_fpsMat = FPSGraph.material;
    }

    void Update()
    {
        TargetFPS.text = m_fpsModel.TargetFPS.ToString();
        VSync.text = m_fpsModel.VSyncCount.ToString();

        /* TODO : GC Optimize */
        MinFPSText.text = string.Format("{0:N0}", m_fpsModel.Min);
        MaxFPSText.text = string.Format("{0:N0}", m_fpsModel.Max);
        
        FPSText.text = string.Format("{0:N1}", m_fpsModel.FPS);
        SPFText.text = string.Format("{0:N1}", m_fpsModel.SPF * 1000);

        float[] fpsData = m_fpsModel.Sample;
        m_fpsMat.SetFloat("FPSSampleLength", m_fpsModel.SampleCapicity);
        m_fpsMat.SetFloatArray("FPSSample", fpsData);
    }
}
