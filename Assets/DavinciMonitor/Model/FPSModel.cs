using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

class FPSQueue
{
    private float[] m_data;
    private float[] m_returnArray;
    private int m_count;
    private int m_startIndex = 0;
    private int m_endIndex = 0;

    public int Count
    {
        get { return (m_endIndex - m_startIndex + Length) % Length; }
    }

    private int Length
    {
        get { return m_data.Length; }
    }

    public bool IsEmpty
    {
        get { return m_startIndex == m_endIndex; }
    }

    public bool IsFull
    {
        get { return (m_endIndex + 1) % Length == m_startIndex; }
    }

    public FPSQueue(int len)
    {
        m_data = new float[len + 1];
        m_returnArray = new float[len + 1];
    }

    public void Dequeue()
    {
        if (IsEmpty)
            return;

        m_startIndex = (m_startIndex + 1) % Length;
    }

    public void Enqueue(float fps)
    {
        if (IsFull)
            return;

        m_data[m_endIndex] = fps;
        m_endIndex = (m_endIndex + 1) % Length;
    }

    public float[] GetArray()
    {
        int i = m_startIndex;
        int j = 0;
        while(i != m_endIndex)
        {
            m_returnArray[j] = m_data[i];
            i = (i + 1) % Length;
            ++j;
        }
        m_returnArray[Length - 1] = m_returnArray[Length - 2];
        return m_returnArray;
    }

    public void GetMinMaxFPS(out float min, out float max)
    {
        min = float.MaxValue;
        max = float.MinValue;
        int i = m_startIndex;
        while (i != m_endIndex)
        {
            if (m_data[i] > max)
                max = m_data[i];
            if (m_data[i] < min)
                min = m_data[i];
            i = (i + 1) % Length;
        }
    }
}

public class FPSModel : IModel
{
    public float FPS { get; private set; }
    public float SPF { get { return 1.0f / FPS; } }
    public float Min { get; private set; }
    public float Max { get; private set; }

    public int TargetFPS { get; private set; }
    public int VSyncCount { get; private set; }

    public int SampleCapicity { get; private set; }
    public float[] Sample { get { return m_fpsQueue.GetArray(); }}
    private FPSQueue m_fpsQueue;

    private float m_sampleIntervalTime = 0.1f;
    private float m_curTime = 0.0f;
    private float m_lastTime;
    private int m_frameCount;

    public override void Init()
    {
        SampleCapicity = 64;
        m_fpsQueue = new FPSQueue(SampleCapicity);

        m_lastTime = Time.realtimeSinceStartup;
        for (int i = 0; i < SampleCapicity; ++i)
            m_fpsQueue.Enqueue(0);

    }

    public override void Update()
    {
        TargetFPS = Application.targetFrameRate;
        VSyncCount = QualitySettings.vSyncCount;
        
        m_curTime += Time.realtimeSinceStartup - m_lastTime;
        ++m_frameCount;

        if(m_curTime >= m_sampleIntervalTime)
        {
            FPS = m_frameCount / m_curTime;
            m_fpsQueue.Dequeue();
            m_fpsQueue.Enqueue(FPS);

            float min, max;
            m_fpsQueue.GetMinMaxFPS(out min, out max);
            Min = min;
            Max = max;

            m_curTime = 0;
            m_frameCount = 0;
        }

        m_lastTime = Time.realtimeSinceStartup;
    }
}
