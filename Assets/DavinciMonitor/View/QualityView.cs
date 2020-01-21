using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QualityView : MonoBehaviour
{
    private QualityModel m_qualityModel;

    public Text QualityLevel;

    void Start()
    {
        m_qualityModel = MonitorManager.Instance.GetModel<QualityModel>();

        QualityLevel.text = m_qualityModel.Level.ToString();
    }
}
