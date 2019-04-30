using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TODController : MonoBehaviour
{
    private float m_time;

    public Transform m_lightTransform;
    public TODSunRotation m_sunRotationConfig;

    void Update()
    {
        m_time += Time.deltaTime;
        if (m_time > 10)
            m_time -= 10;

        m_lightTransform.localEulerAngles = m_sunRotationConfig.Evaluate(m_time / 10);
    }
    
}
